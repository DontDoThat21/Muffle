using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class EmojiPickerView : ContentView
    {
        public EmojiPickerView()
        {
            InitializeComponent();
            BindingContext = new EmojiPickerViewModel();
        }
    }
}
