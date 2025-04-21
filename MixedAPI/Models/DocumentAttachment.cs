namespace MixedAPI.Models
{
    public class DocumentAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public DateTime DateUploaded { get; set; }
        public string FilePath { get; set; }
    }
}
