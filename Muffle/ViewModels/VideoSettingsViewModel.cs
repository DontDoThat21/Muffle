using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for video settings — camera selection, resolution, and frame rate
    /// </summary>
    public class VideoSettingsViewModel : BindableObject
    {
        private string _selectedCamera = "Default";
        private string _selectedResolution = "1280x720";
        private int _selectedFps = 30;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public string SelectedCamera
        {
            get => _selectedCamera;
            set { _selectedCamera = value; OnPropertyChanged(); }
        }

        public string SelectedResolution
        {
            get => _selectedResolution;
            set { _selectedResolution = value; OnPropertyChanged(); }
        }

        public int SelectedFps
        {
            get => _selectedFps;
            set { _selectedFps = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ObservableCollection<string> Cameras { get; } = new();
        public ObservableCollection<string> Resolutions { get; } = new();
        public ObservableCollection<int> FpsOptions { get; } = new();

        public ICommand SaveVideoSettingsCommand { get; }

        public VideoSettingsViewModel()
        {
            SaveVideoSettingsCommand = new Command(async () => await SaveVideoSettingsAsync());

            PopulateLists();
            LoadCurrentSettings();
        }

        private void PopulateLists()
        {
            Cameras.Clear();
            foreach (var camera in VideoSettingsService.AvailableCameras)
                Cameras.Add(camera);

            Resolutions.Clear();
            foreach (var resolution in VideoSettingsService.AvailableResolutions)
                Resolutions.Add(resolution);

            FpsOptions.Clear();
            foreach (var fps in VideoSettingsService.AvailableFpsOptions)
                FpsOptions.Add(fps);
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                if (user == null) return;

                var settings = VideoSettingsService.GetVideoSettings(user.UserId);

                SelectedCamera = Cameras.Contains(settings.Camera)
                    ? settings.Camera
                    : Cameras.FirstOrDefault() ?? "Default";

                SelectedResolution = Resolutions.Contains(settings.Resolution)
                    ? settings.Resolution
                    : "1280x720";

                SelectedFps = FpsOptions.Contains(settings.Fps)
                    ? settings.Fps
                    : 30;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading video settings: {ex.Message}";
            }
        }

        private async Task SaveVideoSettingsAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = string.Empty;

                var user = CurrentUserService.CurrentUser;
                if (user == null)
                {
                    StatusMessage = "No user logged in.";
                    return;
                }

                var settings = new VideoSettings
                {
                    UserId = user.UserId,
                    Camera = SelectedCamera,
                    Resolution = SelectedResolution,
                    Fps = SelectedFps
                };

                bool success = VideoSettingsService.SaveVideoSettings(settings);
                StatusMessage = success ? "Video settings saved." : "Failed to save video settings.";

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving video settings: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
