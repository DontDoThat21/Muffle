using System.Collections.ObjectModel;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class LibraryAcknowledgementsViewModel : BindableObject
    {
        private ObservableCollection<LibraryAcknowledgement> _libraries = new();

        public ObservableCollection<LibraryAcknowledgement> Libraries
        {
            get => _libraries;
            set { _libraries = value; OnPropertyChanged(); }
        }

        public LibraryAcknowledgementsViewModel()
        {
            var items = LibraryAcknowledgementsService.GetAcknowledgements();
            Libraries = new ObservableCollection<LibraryAcknowledgement>(items);
        }
    }
}
