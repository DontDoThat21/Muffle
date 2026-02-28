using System.Collections.ObjectModel;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for the patch notes viewer — displays the app changelog newest-first.
    /// </summary>
    public class PatchNotesViewModel : BindableObject
    {
        private ObservableCollection<PatchNote> _patchNotes = new();

        public ObservableCollection<PatchNote> PatchNotes
        {
            get => _patchNotes;
            set { _patchNotes = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasPatchNotes)); }
        }

        public bool HasPatchNotes => PatchNotes.Count > 0;

        public PatchNotesViewModel()
        {
            LoadPatchNotes();
        }

        private void LoadPatchNotes()
        {
            var notes = PatchNotesService.GetPatchNotes();
            PatchNotes = new ObservableCollection<PatchNote>(notes);
        }
    }
}
