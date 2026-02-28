namespace Muffle.Data.Models
{
    /// <summary>
    /// A single line item in a patch note — categorised as New, Fixed, Improved, or Changed.
    /// </summary>
    public class PatchNoteEntry
    {
        /// <summary>New, Fixed, Improved, or Changed</summary>
        public string EntryType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
