using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class EmojiPickerViewModel : BindableObject
    {
        public ObservableCollection<Emoji> Emojis { get; } = new();

        public event Action<string>? EmojiSelected;

        public ICommand SelectEmojiCommand { get; }

        public EmojiPickerViewModel()
        {
            SelectEmojiCommand = new Command<Emoji>(ExecuteSelectEmoji);

            foreach (var emoji in EmojiService.GetAllEmojis())
                Emojis.Add(emoji);
        }

        private void ExecuteSelectEmoji(Emoji? emoji)
        {
            if (emoji == null) return;
            EmojiSelected?.Invoke(emoji.Unicode);
        }
    }
}
