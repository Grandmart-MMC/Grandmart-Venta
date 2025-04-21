namespace MixedAPI.Services
{
    public interface IVideoService
    {
        Task<string> UploadChunk(int chunkIndex, int totalChunks, string originalFileName, string uploadSessionId, IFormFile chunk);
        Task<byte[]> DownloadChunk(string originalFileName, string uploadSessionId, int chunkIndex, int chunkSize);
    }
}
