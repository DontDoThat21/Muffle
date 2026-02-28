using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for fetching user status from external APIs (Spotify, Steam, Xbox Live).
    /// Currently provides a framework for integration — actual API calls require 
    /// valid API keys and OAuth tokens to be configured per-service.
    /// </summary>
    public static class ExternalStatusService
    {
        /// <summary>
        /// Represents an activity fetched from an external service
        /// </summary>
        public class ExternalActivity
        {
            public string ServiceName { get; set; } = string.Empty;
            public string ActivityType { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
            public DateTime? StartedAt { get; set; }
        }

        /// <summary>
        /// Get the current activity from Spotify (requires OAuth token)
        /// </summary>
        public static async Task<ExternalActivity?> GetSpotifyActivityAsync(string? oauthToken)
        {
            if (string.IsNullOrEmpty(oauthToken))
                return null;

            try
            {
                // Placeholder: In production, call Spotify Web API
                // GET https://api.spotify.com/v1/me/player/currently-playing
                // Authorization: Bearer {oauthToken}
                await Task.CompletedTask;

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Spotify activity: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get the current activity from Steam (requires Steam Web API key)
        /// </summary>
        public static async Task<ExternalActivity?> GetSteamActivityAsync(string? steamId, string? apiKey)
        {
            if (string.IsNullOrEmpty(steamId) || string.IsNullOrEmpty(apiKey))
                return null;

            try
            {
                // Placeholder: In production, call Steam Web API
                // GET https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={steamId}
                await Task.CompletedTask;

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Steam activity: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get the current activity from Xbox Live (requires Xbox Live token)
        /// </summary>
        public static async Task<ExternalActivity?> GetXboxLiveActivityAsync(string? xboxLiveToken)
        {
            if (string.IsNullOrEmpty(xboxLiveToken))
                return null;

            try
            {
                // Placeholder: In production, call Xbox Live API
                // GET https://xbl.io/api/v2/account
                await Task.CompletedTask;

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Xbox Live activity: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all external activities for a user based on their connected services
        /// </summary>
        public static async Task<List<ExternalActivity>> GetAllActivitiesAsync(int userId)
        {
            var activities = new List<ExternalActivity>();

            try
            {
                var connections = ProfileConnectionService.GetConnections(userId);

                foreach (var conn in connections)
                {
                    ExternalActivity? activity = conn.ServiceName switch
                    {
                        "Spotify" => await GetSpotifyActivityAsync(conn.ServiceUrl),
                        "Steam" => await GetSteamActivityAsync(conn.ServiceUsername, null),
                        "Xbox Live" => await GetXboxLiveActivityAsync(conn.ServiceUrl),
                        _ => null
                    };

                    if (activity != null)
                    {
                        activities.Add(activity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all activities: {ex.Message}");
            }

            return activities;
        }

        /// <summary>
        /// Format an activity as a custom status string
        /// </summary>
        public static string FormatActivityAsStatus(ExternalActivity activity)
        {
            return activity.ActivityType switch
            {
                "Listening" => $"Listening to {activity.Details}",
                "Playing" => $"Playing {activity.Details}",
                "Watching" => $"Watching {activity.Details}",
                "Streaming" => $"Streaming {activity.Details}",
                _ => activity.Details
            };
        }
    }
}
