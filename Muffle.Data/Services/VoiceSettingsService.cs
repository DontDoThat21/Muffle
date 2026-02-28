using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing per-user voice and audio settings
    /// </summary>
    public static class VoiceSettingsService
    {
        /// <summary>
        /// Returns a list of available audio input (microphone) device names.
        /// "Default" is always included. Platform-specific enumeration can be
        /// injected by replacing this list at startup.
        /// </summary>
        public static List<string> AvailableInputDevices { get; set; } = new() { "Default" };

        /// <summary>
        /// Returns a list of available audio output (speaker/headphone) device names.
        /// "Default" is always included.
        /// </summary>
        public static List<string> AvailableOutputDevices { get; set; } = new() { "Default" };

        /// <summary>
        /// Retrieve voice settings for a user. Returns defaults if none are saved.
        /// </summary>
        public static VoiceSettings GetVoiceSettings(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM VoiceSettings WHERE UserId = @UserId;";
                var settings = connection.QueryFirstOrDefault<VoiceSettings>(query, new { UserId = userId });

                return settings ?? new VoiceSettings { UserId = userId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting voice settings: {ex.Message}");
                return new VoiceSettings { UserId = userId };
            }
        }

        /// <summary>
        /// Save (upsert) voice settings for a user.
        /// </summary>
        public static bool SaveVoiceSettings(VoiceSettings settings)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    INSERT INTO VoiceSettings
                        (UserId, InputDevice, OutputDevice, PushToTalk, PushToTalkKey, NoiseSuppression, InputVolume, OutputVolume)
                    VALUES
                        (@UserId, @InputDevice, @OutputDevice, @PushToTalk, @PushToTalkKey, @NoiseSuppression, @InputVolume, @OutputVolume)
                    ON CONFLICT(UserId) DO UPDATE SET
                        InputDevice     = @InputDevice,
                        OutputDevice    = @OutputDevice,
                        PushToTalk      = @PushToTalk,
                        PushToTalkKey   = @PushToTalkKey,
                        NoiseSuppression = @NoiseSuppression,
                        InputVolume     = @InputVolume,
                        OutputVolume    = @OutputVolume;";

                connection.Execute(query, settings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving voice settings: {ex.Message}");
                return false;
            }
        }
    }
}
