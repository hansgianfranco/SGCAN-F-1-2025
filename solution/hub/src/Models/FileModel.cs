namespace Hub.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public string Status { get; set; } // "Pending", "Processing", "Completed"
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public List<LinkModel> Links { get; set; }
    }
}