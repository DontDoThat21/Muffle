using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for developer settings — debug mode, WebSocket inspector, and dev tools
    /// </summary>
    public class DeveloperSettingsViewModel : BindableObject
    {
        private bool _debugMode = false;
        private bool _webSocketInspector = false;
        private bool _enableDevTools = false;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public bool DebugMode
        {
            get => _debugMode;
            set { _debugMode = value; OnPropertyChanged(); }
        }

        public bool WebSocketInspector
        {
            get => _webSocketInspector;
            set { _webSocketInspector = value; OnPropertyChanged(); }
        }

        public bool EnableDevTools
        {
            get => _enableDevTools;
            set { _enableDevTools = value; OnPropertyChanged(); }
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

        public ICommand SaveDeveloperSettingsCommand { get; }

        public DeveloperSettingsViewModel()
        {
            SaveDeveloperSettingsCommand = new Command(async () => await SaveDeveloperSettingsAsync());
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                if (user == null) return;

                var settings = DeveloperSettingsService.GetDeveloperSettings(user.UserId);

                DebugMode = settings.DebugMode;
                WebSocketInspector = settings.WebSocketInspector;
                EnableDevTools = settings.EnableDevTools;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading developer settings: {ex.Message}";
            }
        }

        private async Task SaveDeveloperSettingsAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = string.Empty;

                var user = CurrentUserService.CurrentUser;
                if (user == null)
                {
                    StatusMessage = "No user is logged in.";
                    return;
                }

                var settings = new DeveloperSettings
                {
                    UserId = user.UserId,
                    DebugMode = DebugMode,
                    WebSocketInspector = WebSocketInspector,
                    EnableDevTools = EnableDevTools
                };

                var success = DeveloperSettingsService.SaveDeveloperSettings(settings);
                StatusMessage = success ? "Developer settings saved." : "Failed to save developer settings.";

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving developer settings: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
