namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents an open-source library used by Muffle and its attribution details.
    /// </summary>
    public class LibraryAcknowledgement
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
