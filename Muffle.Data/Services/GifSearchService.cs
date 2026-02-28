using System.Net.Http;
using System.Text.Json;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class GifSearchService
    {
        private static readonly HttpClient _httpClient = new();
        private const string ApiKey = "PLACEHOLDER";

        public static async Task<List<GifResult>> SearchGifsAsync(string query)
        {
            var url = $"https://tenor.googleapis.com/v2/search?q={Uri.EscapeDataString(query)}&key={ApiKey}&limit=20";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var doc = JsonDocument.Parse(response);

                var results = new List<GifResult>();

                if (!doc.RootElement.TryGetProperty("results", out var resultsArray))
                    return results;

                foreach (var item in resultsArray.EnumerateArray())
                {
                    var id = item.TryGetProperty("id", out var idEl) ? idEl.GetString() ?? string.Empty : string.Empty;
                    var title = item.TryGetProperty("title", out var titleEl) ? titleEl.GetString() ?? string.Empty : string.Empty;

                    string previewUrl = string.Empty;
                    string fullUrl = string.Empty;

                    if (item.TryGetProperty("media_formats", out var formats))
                    {
                        if (formats.TryGetProperty("tinygif", out var tiny) &&
                            tiny.TryGetProperty("url", out var tinyUrl))
                            previewUrl = tinyUrl.GetString() ?? string.Empty;

                        if (formats.TryGetProperty("gif", out var full) &&
                            full.TryGetProperty("url", out var fullUrlEl))
                            fullUrl = fullUrlEl.GetString() ?? string.Empty;
                    }

                    results.Add(new GifResult
                    {
                        Id = id,
                        Title = title,
                        PreviewUrl = previewUrl,
                        FullUrl = fullUrl
                    });
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GIF search error: {ex.Message}");
                return new List<GifResult>();
            }
        }
    }
}
