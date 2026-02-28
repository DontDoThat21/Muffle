using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for voice settings — input/output device, push-to-talk, noise suppression, volume
    /// </summary>
    public class VoiceSettingsViewModel : BindableObject
    {
        private string _selectedInputDevice = "Default";
        private string _selectedOutputDevice = "Default";
        private bool _pushToTalk = false;
        private string _pushToTalkKey = string.Empty;
        private bool _noiseSuppression = true;
        private double _inputVolume = 100;
        private double _outputVolume = 100;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public string SelectedInputDevice
        {
            get => _selectedInputDevice;
            set { _selectedInputDevice = value; OnPropertyChanged(); }
        }

        public string SelectedOutputDevice
        {
            get => _selectedOutputDevice;
            set { _selectedOutputDevice = value; OnPropertyChanged(); }
        }

        public bool PushToTalk
        {
            get => _pushToTalk;
            set { _pushToTalk = value; OnPropertyChanged(); OnPropertyChanged(nameof(PushToTalkKeyVisible)); }
        }

        public string PushToTalkKey
        {
            get => _pushToTalkKey;
            set { _pushToTalkKey = value; OnPropertyChanged(); }
        }

        public bool PushToTalkKeyVisible => PushToTalk;

        public bool NoiseSuppression
        {
            get => _noiseSuppression;
            set { _noiseSuppression = value; OnPropertyChanged(); }
        }

        public double InputVolume
        {
            get => _inputVolume;
            set { _inputVolume = value; OnPropertyChanged(); OnPropertyChanged(nameof(InputVolumeDisplay)); }
        }

        public double OutputVolume
        {
            get => _outputVolume;
            set { _outputVolume = value; OnPropertyChanged(); OnPropertyChanged(nameof(OutputVolumeDisplay)); }
        }

        public string InputVolumeDisplay => $"{(int)InputVolume}%";
        public string OutputVolumeDisplay => $"{(int)OutputVolume}%";

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

        public ObservableCollection<string> InputDevices { get; } = new();
        public ObservableCollection<string> OutputDevices { get; } = new();

        public ICommand SaveVoiceSettingsCommand { get; }

        public VoiceSettingsViewModel()
        {
            SaveVoiceSettingsCommand = new Command(async () => await SaveVoiceSettingsAsync());

            PopulateDeviceLists();
            LoadCurrentSettings();
        }

        private void PopulateDeviceLists()
        {
            InputDevices.Clear();
            foreach (var device in VoiceSettingsService.AvailableInputDevices)
                InputDevices.Add(device);

            OutputDevices.Clear();
            foreach (var device in VoiceSettingsService.AvailableOutputDevices)
                OutputDevices.Add(device);
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                if (user == null) return;

                var settings = VoiceSettingsService.GetVoiceSettings(user.UserId);

                SelectedInputDevice = InputDevices.Contains(settings.InputDevice)
                    ? settings.InputDevice
                    : InputDevices.FirstOrDefault() ?? "Default";

                SelectedOutputDevice = OutputDevices.Contains(settings.OutputDevice)
                    ? settings.OutputDevice
                    : OutputDevices.FirstOrDefault() ?? "Default";

                PushToTalk = settings.PushToTalk;
                PushToTalkKey = settings.PushToTalkKey;
                NoiseSuppression = settings.NoiseSuppression;
                InputVolume = Math.Clamp(settings.InputVolume, 0, 150);
                OutputVolume = Math.Clamp(settings.OutputVolume, 0, 150);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading voice settings: {ex.Message}";
            }
        }

        private async Task SaveVoiceSettingsAsync()
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

                var settings = new VoiceSettings
                {
                    UserId = user.UserId,
                    InputDevice = SelectedInputDevice,
                    OutputDevice = SelectedOutputDevice,
                    PushToTalk = PushToTalk,
                    PushToTalkKey = PushToTalkKey,
                    NoiseSuppression = NoiseSuppression,
                    InputVolume = (int)InputVolume,
                    OutputVolume = (int)OutputVolume
                };

                bool success = VoiceSettingsService.SaveVoiceSettings(settings);
                StatusMessage = success ? "Voice settings saved." : "Failed to save voice settings.";

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving voice settings: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
