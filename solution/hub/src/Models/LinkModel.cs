namespace Hub.Models
{
    public class LinkModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public int FileModelId { get; set; }
        public FileModel File { get; set; }
    }
}