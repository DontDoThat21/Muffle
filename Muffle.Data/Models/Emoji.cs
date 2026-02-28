namespace Muffle.Data.Models
{
    public class Emoji
    {
        public string Code { get; set; } = string.Empty;     // e.g. ":smile:"
        public string Unicode { get; set; } = string.Empty;  // e.g. "😄"
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
