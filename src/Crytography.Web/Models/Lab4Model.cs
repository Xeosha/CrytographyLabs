namespace Crytography.Web.Models
{
    public class Lab4Model
    {
        public string Key { get; set; } = string.Empty;
        public IFormFile? EncryptedFilePath { get; set; }
        public string EncryptedFilePathSave { get; set; } = string.Empty;
        public IFormFile? DecryptedFilePath { get; set; }
        public string DecryptedFileSavePath { get; set; } = string.Empty;
    }
}
