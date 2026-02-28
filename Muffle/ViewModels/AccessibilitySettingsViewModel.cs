using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for accessibility settings — font size, high contrast, and screen reader support
    /// </summary>
    public class AccessibilitySettingsViewModel : BindableObject
    {
        private double _selectedFontSize = 14.0;
        private bool _highContrast = false;
        private bool _screenReader = false;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public double SelectedFontSize
        {
            get => _selectedFontSize;
            set { _selectedFontSize = value; OnPropertyChanged(); OnPropertyChanged(nameof(FontSizeDisplay)); }
        }

        public bool HighContrast
        {
            get => _highContrast;
            set { _highContrast = value; OnPropertyChanged(); }
        }

        public bool ScreenReader
        {
            get => _screenReader;
            set { _screenReader = value; OnPropertyChanged(); }
        }

        public string FontSizeDisplay => $"{(int)SelectedFontSize}pt";

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

        public ObservableCollection<double> FontSizes { get; } = new();

        public ICommand SaveAccessibilitySettingsCommand { get; }

        public AccessibilitySettingsViewModel()
        {
            SaveAccessibilitySettingsCommand = new Command(async () => await SaveAccessibilitySettingsAsync());

            PopulateFontSizes();
            LoadCurrentSettings();
        }

        private void PopulateFontSizes()
        {
            FontSizes.Clear();
            foreach (var size in AccessibilitySettingsService.AvailableFontSizes)
                FontSizes.Add(size);
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                if (user == null) return;

                var settings = AccessibilitySettingsService.GetAccessibilitySettings(user.UserId);

                SelectedFontSize = FontSizes.Contains(settings.FontSize)
                    ? settings.FontSize
                    : 14.0;

                HighContrast = settings.HighContrast;
                ScreenReader = settings.ScreenReader;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading accessibility settings: {ex.Message}";
            }
        }

        private async Task SaveAccessibilitySettingsAsync()
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

                var settings = new AccessibilitySettings
                {
                    UserId = user.UserId,
                    FontSize = SelectedFontSize,
                    HighContrast = HighContrast,
                    ScreenReader = ScreenReader
                };

                var success = AccessibilitySettingsService.SaveAccessibilitySettings(settings);
                StatusMessage = success ? "Accessibility settings saved." : "Failed to save accessibility settings.";

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
