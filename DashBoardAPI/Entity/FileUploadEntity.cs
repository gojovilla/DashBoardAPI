namespace DashBoardAPI.Entity
{
    public class FileUploadEntity:BaseEntity
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
        public string FilePath { get;set; }
        public IFormFile MyFile { get; set; }
    }
}
