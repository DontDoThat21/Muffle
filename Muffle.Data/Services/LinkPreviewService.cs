using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class LinkPreviewService
    {
        private static readonly HttpClient _httpClient = new(new HttpClientHandler { AllowAutoRedirect = true })
        {
            Timeout = TimeSpan.FromSeconds(6)
        };

        static LinkPreviewService()
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        // Patterns that handle both attribute orderings (property-first and content-first)
        private static readonly Regex MetaTagRegex =
            new(@"<meta\b([^>]+?)(?:/)?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex PropRegex =
            new(@"(?:property|name)\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ContentRegex =
            new(@"content\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex TitleTagRegex =
            new(@"<title\b[^>]*>([^<]*)</title>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static async Task<LinkPreviewResult?> FetchPreviewAsync(string url)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var result = new LinkPreviewResult { Url = url };

                var metaValues = ExtractMetaValues(html);

                result.Title = metaValues.GetValueOrDefault("og:title")
                    ?? metaValues.GetValueOrDefault("twitter:title")
                    ?? ExtractTitleTag(html)
                    ?? string.Empty;

                result.Description = metaValues.GetValueOrDefault("og:description")
                    ?? metaValues.GetValueOrDefault("twitter:description")
                    ?? metaValues.GetValueOrDefault("description")
                    ?? string.Empty;

                var rawImage = metaValues.GetValueOrDefault("og:image")
                    ?? metaValues.GetValueOrDefault("twitter:image");
                result.ImageUrl = ResolveUrl(rawImage, url);

                result.SiteName = metaValues.GetValueOrDefault("og:site_name")
                    ?? new Uri(url).Host;

                result.Title = WebUtility.HtmlDecode(result.Title).Trim();
                result.Description = WebUtility.HtmlDecode(result.Description).Trim();
                result.SiteName = WebUtility.HtmlDecode(result.SiteName).Trim();

                return string.IsNullOrEmpty(result.Title) ? null : result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LinkPreviewService error for {url}: {ex.Message}");
                return null;
            }
        }

        private static Dictionary<string, string> ExtractMetaValues(string html)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (Match m in MetaTagRegex.Matches(html))
            {
                var attrs = m.Groups[1].Value;
                var propMatch = PropRegex.Match(attrs);
                var contentMatch = ContentRegex.Match(attrs);

                if (propMatch.Success && contentMatch.Success)
                {
                    var key = propMatch.Groups[1].Value;
                    var value = contentMatch.Groups[1].Value;
                    result.TryAdd(key, value);
                }
            }

            return result;
        }

        private static string? ExtractTitleTag(string html)
        {
            var m = TitleTagRegex.Match(html);
            return m.Success ? m.Groups[1].Value : null;
        }

        private static string? ResolveUrl(string? rawUrl, string pageUrl)
        {
            if (string.IsNullOrEmpty(rawUrl)) return null;
            if (rawUrl.StartsWith("http://") || rawUrl.StartsWith("https://")) return rawUrl;

            try
            {
                var baseUri = new Uri(pageUrl);
                return new Uri(baseUri, rawUrl).AbsoluteUri;
            }
            catch
            {
                return null;
            }
        }
    }
}
