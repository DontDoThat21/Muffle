namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a user's voice and audio settings
    /// </summary>
    public class VoiceSettings
    {
        public int UserId { get; set; }
        public string InputDevice { get; set; } = "Default";
        public string OutputDevice { get; set; } = "Default";
        public bool PushToTalk { get; set; } = false;
        public string PushToTalkKey { get; set; } = string.Empty;
        public bool NoiseSuppression { get; set; } = true;
        public int InputVolume { get; set; } = 100;
        public int OutputVolume { get; set; } = 100;
    }
}
