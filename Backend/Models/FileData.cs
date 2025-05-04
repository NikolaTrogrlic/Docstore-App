namespace Backend.Models
{
    public class FileData
    {
        public string Name { get; set; } = "";
        public string FileSize { get; set; } = "0 kB";
        public DateTime? ModifiedOn { get; set; } = DateTime.UtcNow;
    }
}
