using MixedAPI.Models;

namespace MixedAPI.Services
{
    public class VideoService : IVideoService
    {
        private readonly ApplicationDbContext _context;
        private static readonly List<string> AllowedExtensions = new() { ".mp4", ".avi", ".mov", ".mkv" };

        public VideoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> UploadChunk(int chunkIndex, int totalChunks, string originalFileName, string uploadSessionId, IFormFile chunk)
        {
            if (chunk == null || chunk.Length == 0)
                throw new ArgumentException("Chunk is missing or empty.");

            if (chunkIndex < 1 || chunkIndex > totalChunks)
                throw new ArgumentException($"Invalid chunkIndex: {chunkIndex}. It must be between 1 and {totalChunks}.");

            var fileExtension = Path.GetExtension(originalFileName).ToLower();
            if (!AllowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Unsupported file format. Only videos are allowed (.mp4, .avi, .mov, .mkv).");

            // unical folder generate
            var uniqueFolderName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", $"{originalFileName}_{uploadSessionId}");
            if (!Directory.Exists(uniqueFolderName))
            {
                Directory.CreateDirectory(uniqueFolderName);
            }

            // save chunk file 
            var tempFilePath = Path.Combine(uniqueFolderName, $"{chunkIndex}_{uploadSessionId}.chunk");
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream);
            }

            // combine last chunk
            if (chunkIndex == totalChunks)
            {
                await MergeChunks(originalFileName, uploadSessionId, totalChunks);
                return "Upload complete and file merged successfully!";
            }

            return $"Chunk {chunkIndex} uploaded successfully!";
        }

        public async Task<byte[]> DownloadChunk(string originalFileName, string uploadSessionId, int chunkIndex, int chunkSize)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", $"{originalFileName}_{uploadSessionId}.mp4");

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"File '{originalFileName}' not found.");
            }

            var fileInfo = new FileInfo(filePath);
            var fileLength = fileInfo.Length;

            var start = (chunkIndex - 1) * chunkSize;
            var end = Math.Min(start + chunkSize, fileLength);

            if (start >= fileLength)
            {
                throw new ArgumentException("Invalid chunk index or chunk size.");
            }

            byte[] buffer = new byte[end - start];
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(start, SeekOrigin.Begin);
                await stream.ReadAsync(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        private async Task MergeChunks(string originalFileName, string uploadSessionId, int totalChunks)
        {
            var uniqueFolderName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", $"{originalFileName}_{uploadSessionId}");
            var finalFileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", $"{originalFileName}_{uploadSessionId}.mp4");

            using (var finalStream = new FileStream(finalFileName, FileMode.Create))
            {
                for (int i = 1; i <= totalChunks; i++)
                {
                    var chunkFilePath = Path.Combine(uniqueFolderName, $"{i}_{uploadSessionId}.chunk");
                    if (!System.IO.File.Exists(chunkFilePath))
                    {
                        throw new Exception($"Chunk {i} is missing.");
                    }

                    var buffer = await System.IO.File.ReadAllBytesAsync(chunkFilePath);
                    await finalStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }

            // Save to db
            var fileDetails = new DocumentAttachment
            {
                FileName = $"{originalFileName}_{uploadSessionId}.mp4",
                FileExtension = ".mp4",
                DateUploaded = DateTime.Now,
                FilePath = Path.Combine("wwwroot/Videos", $"{originalFileName}_{uploadSessionId}.mp4")
            };

            _context.DocumentAttachments.Add(fileDetails);
            await _context.SaveChangesAsync();

            // delete temporary folder
            Directory.Delete(uniqueFolderName, true);
        }
    }
}
