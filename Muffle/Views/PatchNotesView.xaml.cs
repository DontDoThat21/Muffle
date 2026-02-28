using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class PatchNotesView : ContentView
    {
        public PatchNotesView()
        {
            InitializeComponent();
            BindingContext = new PatchNotesViewModel();
        }

        public PatchNotesView(PatchNotesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
