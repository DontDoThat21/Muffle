namespace Muffle.Data.Models
{
    public class LinkPreviewResult
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string SiteName { get; set; } = string.Empty;
    }
}
