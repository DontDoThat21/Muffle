using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class EmojiService
    {
        public static List<Emoji> GetAllEmojis()
        {
            return new List<Emoji>
            {
                // Smileys
                new Emoji { Code = ":smile:",       Unicode = "😄", Category = "Smileys", Name = "smile" },
                new Emoji { Code = ":grin:",        Unicode = "😁", Category = "Smileys", Name = "grin" },
                new Emoji { Code = ":laugh:",       Unicode = "😂", Category = "Smileys", Name = "laugh" },
                new Emoji { Code = ":wink:",        Unicode = "😉", Category = "Smileys", Name = "wink" },
                new Emoji { Code = ":blush:",       Unicode = "😊", Category = "Smileys", Name = "blush" },
                new Emoji { Code = ":cool:",        Unicode = "😎", Category = "Smileys", Name = "cool" },
                new Emoji { Code = ":thinking:",    Unicode = "🤔", Category = "Smileys", Name = "thinking" },
                new Emoji { Code = ":cry:",         Unicode = "😢", Category = "Smileys", Name = "cry" },
                new Emoji { Code = ":sob:",         Unicode = "😭", Category = "Smileys", Name = "sob" },
                new Emoji { Code = ":angry:",       Unicode = "😠", Category = "Smileys", Name = "angry" },
                new Emoji { Code = ":surprised:",   Unicode = "😮", Category = "Smileys", Name = "surprised" },
                new Emoji { Code = ":flushed:",     Unicode = "😳", Category = "Smileys", Name = "flushed" },
                new Emoji { Code = ":scream:",      Unicode = "😱", Category = "Smileys", Name = "scream" },
                new Emoji { Code = ":pensive:",     Unicode = "😔", Category = "Smileys", Name = "pensive" },
                new Emoji { Code = ":sweat:",       Unicode = "😅", Category = "Smileys", Name = "sweat" },

                // Gestures
                new Emoji { Code = ":thumbsup:",    Unicode = "👍", Category = "Gestures", Name = "thumbs up" },
                new Emoji { Code = ":thumbsdown:",  Unicode = "👎", Category = "Gestures", Name = "thumbs down" },
                new Emoji { Code = ":clap:",        Unicode = "👏", Category = "Gestures", Name = "clap" },
                new Emoji { Code = ":wave:",        Unicode = "👋", Category = "Gestures", Name = "wave" },
                new Emoji { Code = ":pray:",        Unicode = "🙏", Category = "Gestures", Name = "pray" },

                // Symbols
                new Emoji { Code = ":heart:",       Unicode = "❤️", Category = "Symbols", Name = "heart" },
                new Emoji { Code = ":fire:",        Unicode = "🔥", Category = "Symbols", Name = "fire" },
                new Emoji { Code = ":star:",        Unicode = "⭐", Category = "Symbols", Name = "star" },
                new Emoji { Code = ":check:",       Unicode = "✅", Category = "Symbols", Name = "check" },
                new Emoji { Code = ":x:",           Unicode = "❌", Category = "Symbols", Name = "x" },
                new Emoji { Code = ":warning:",     Unicode = "⚠️", Category = "Symbols", Name = "warning" },
                new Emoji { Code = ":tada:",        Unicode = "🎉", Category = "Symbols", Name = "tada" },
                new Emoji { Code = ":100:",         Unicode = "💯", Category = "Symbols", Name = "100" },
                new Emoji { Code = ":muscle:",      Unicode = "💪", Category = "Symbols", Name = "muscle" },
                new Emoji { Code = ":rocket:",      Unicode = "🚀", Category = "Symbols", Name = "rocket" },
            };
        }
    }
}
