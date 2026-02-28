namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a single versioned release with a list of changelog entries.
    /// </summary>
    public class PatchNote
    {
        public string Version { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<PatchNoteEntry> Entries { get; set; } = new();
        public string ReleaseDateDisplay => ReleaseDate.ToString("MMMM d, yyyy");
    }
}
