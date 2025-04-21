using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MixedAPI.Models;
using MixedAPI.Services;
using System.Text.RegularExpressions;

namespace MixedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        public static ApplicationDbContext _context;
        private readonly IVideoService _videoService;
        private readonly IWebHostEnvironment _env;

        public VideosController(ApplicationDbContext context, IVideoService videoService, IWebHostEnvironment env)
        {
            _context = context;
            _videoService = videoService;
            _env = env;
        }

        // POST: Upload a chunk
        [HttpPost("upload-chunk")]
        public async Task<IActionResult> UploadChunk([FromQuery] int chunkIndex, [FromQuery] int totalChunks, [FromQuery] string originalFileName, [FromQuery] string uploadSessionId, IFormFile chunk)
        {
            try
            {
                var result = await _videoService.UploadChunk(chunkIndex, totalChunks, originalFileName, uploadSessionId, chunk);
                return Ok(new { Message = result, UploadSessionId = uploadSessionId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download")]
        public IActionResult DownloadFile(string originalFileName, string uploadSessionId)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", $"{originalFileName}_{uploadSessionId}.mp4");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"File '{originalFileName}' not found.");
            }

            var contentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType($"{originalFileName}.mp4", out var mime))
            {
                contentType = mime;
            }

            return PhysicalFile(filePath, contentType, $"{originalFileName}.mp4");
        }

        [HttpGet("download-chunk")]
        public async Task<IActionResult> DownloadChunk([FromQuery] string originalFileName, [FromQuery] string uploadSessionId, [FromQuery] int chunkIndex, [FromQuery] int chunkSize)
        {
            try
            {
                var buffer = await _videoService.DownloadChunk(originalFileName, uploadSessionId, chunkIndex, chunkSize);
                return File(buffer, "application/octet-stream", $"{originalFileName}_chunk_{chunkIndex}");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error while processing chunk: {ex.Message}");
            }
        }
        [HttpGet("stream")]
        public IActionResult StreamVideo([FromQuery] string originalFileName, [FromQuery] string uploadSessionId)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", $"{originalFileName}_{uploadSessionId}.mp4");

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"[ERROR] File not found: {filePath}");
                    return NotFound($"File '{originalFileName}' not found.");
                }

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileInfo = new FileInfo(filePath);
                var fileLength = fileInfo.Length;
                var rangeHeader = Request.Headers["Range"].ToString();
                if (string.IsNullOrEmpty(rangeHeader))
                {
                    Console.WriteLine("[INFO] No Range header, sending full file.");
                    return File(new FileStream(filePath, FileMode.Open, FileAccess.Read), "video/mp4");
                }

                if (string.IsNullOrEmpty(rangeHeader))
                {
                    Console.WriteLine($"[INFO] Sending full file: {filePath}");
                    Response.Headers["Content-Length"] = fileLength.ToString();
                    return File(fileStream, "video/mp4");
                }

                long start = 0, end = fileLength - 1;
                var rangeMatch = System.Text.RegularExpressions.Regex.Match(rangeHeader, @"bytes=(\d+)-(\d+)?");

                if (rangeMatch.Success)
                {
                    start = long.Parse(rangeMatch.Groups[1].Value);
                    if (!string.IsNullOrEmpty(rangeMatch.Groups[2].Value))
                    {
                        end = long.Parse(rangeMatch.Groups[2].Value);
                    }
                }

                var contentLength = end - start + 1;
                byte[] buffer = new byte[contentLength];

                fileStream.Seek(start, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                var stream = new MemoryStream(buffer);
                stream.Position = 0;

                Response.StatusCode = 206; // Partial Content
                Response.Headers["Accept-Ranges"] = "bytes";
                Response.Headers["Content-Range"] = $"bytes {start}-{end}/{fileLength}";
                Response.Headers["Content-Length"] = contentLength.ToString();

                Console.WriteLine($"[INFO] Streaming range: {start}-{end}/{fileLength}");

                return File(stream, "video/mp4", enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER ERROR] {ex.Message}");
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
        [HttpGet("stream2")]
        public IActionResult StreamVideo2([FromQuery] string originalFileName, [FromQuery] string uploadSessionId)
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, "Videos", $"{originalFileName}_{uploadSessionId}.mp4");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"File '{originalFileName}' not found.");
                }

                var fileInfo = new FileInfo(filePath);
                var fileSize = fileInfo.Length;
                var rangeHeader = Request.Headers["Range"].ToString();

                Response.Headers["Accept-Ranges"] = "bytes";

                if (string.IsNullOrEmpty(rangeHeader))
                {
                    Response.Headers["Content-Length"] = fileSize.ToString();
                    return PhysicalFile(filePath, "video/mp4", enableRangeProcessing: true);
                }

                long start = 0, end = fileSize - 1;
                var rangeMatch = Regex.Match(rangeHeader, @"bytes=(\d+)-(\d+)?");

                if (rangeMatch.Success)
                {
                    start = long.Parse(rangeMatch.Groups[1].Value);
                    if (!string.IsNullOrEmpty(rangeMatch.Groups[2].Value))
                    {
                        end = long.Parse(rangeMatch.Groups[2].Value);
                    }
                }

                long contentLength = end - start + 1;

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileStream.Seek(start, SeekOrigin.Begin);

                Response.StatusCode = 206; // ✅ Partial Content Streaming
                Response.Headers["Content-Range"] = $"bytes {start}-{end}/{fileSize}";
                Response.Headers["Content-Length"] = contentLength.ToString();
                Response.Headers["Content-Type"] = "video/mp4";
                Response.Headers["Content-Disposition"] = "inline"; // 📌 Browserdə birbaşa aç
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";

                return File(fileStream, "video/mp4", enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("stream3")]
        public async Task<IActionResult> GetVideo(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "Videos", $"{fileName}");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Video file not found");
            }

            var memoryStream = new MemoryStream();
            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            return File(memoryStream, "video/mp4", enableRangeProcessing: true);
        }
    }
}
