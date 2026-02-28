using System.Text.RegularExpressions;

namespace Muffle.Data.Services
{
    public static class MentionService
    {
        private static readonly Regex MentionRegex = new(@"@(\w+)", RegexOptions.Compiled);

        public static List<string> ParseMentions(string messageContent)
        {
            return MentionRegex.Matches(messageContent)
                .Select(m => m.Groups[1].Value)
                .ToList();
        }

        public static string ResolveMentions(string content)
        {
            return MentionRegex.Replace(content, m => $"**@{m.Groups[1].Value}**");
        }
    }
}
