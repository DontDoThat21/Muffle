using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class LibraryAcknowledgementsView : ContentView
    {
        public LibraryAcknowledgementsView()
        {
            InitializeComponent();
            BindingContext = new LibraryAcknowledgementsViewModel();
        }

        public LibraryAcknowledgementsView(LibraryAcknowledgementsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
