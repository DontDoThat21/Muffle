using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for theme settings — light/dark mode, custom themes, accent colors
    /// </summary>
    public class ThemeSettingsViewModel : BindableObject
    {
        private string _selectedTheme = "Dark";
        private bool _isDarkMode = true;
        private string _selectedAccentColor = "#7289DA";
        private string _selectedAccentColorName = "Blurple";
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public string SelectedTheme
        {
            get => _selectedTheme;
            set { _selectedTheme = value; OnPropertyChanged(); }
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                _isDarkMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModeDisplayName));
            }
        }

        public string SelectedAccentColor
        {
            get => _selectedAccentColor;
            set { _selectedAccentColor = value; OnPropertyChanged(); }
        }

        public string SelectedAccentColorName
        {
            get => _selectedAccentColorName;
            set { _selectedAccentColorName = value; OnPropertyChanged(); }
        }

        public string ModeDisplayName => IsDarkMode ? "Dark Mode" : "Light Mode";

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

        public ObservableCollection<string> AvailableThemes { get; set; } = new();
        public ObservableCollection<AccentColorOption> AccentColorOptions { get; set; } = new();

        // Commands
        public ICommand SaveThemeCommand { get; }
        public ICommand ToggleDarkModeCommand { get; }
        public ICommand SelectAccentColorCommand { get; }
        public ICommand SelectThemeCommand { get; }

        public ThemeSettingsViewModel()
        {
            SaveThemeCommand = new Command(async () => await SaveThemeAsync());
            ToggleDarkModeCommand = new Command(ToggleDarkMode);
            SelectAccentColorCommand = new Command<AccentColorOption>(SelectAccentColor);
            SelectThemeCommand = new Command<string>(SelectTheme);

            foreach (var theme in ThemeService.AvailableThemes)
            {
                AvailableThemes.Add(theme);
            }

            foreach (var kvp in ThemeService.AccentColors)
            {
                AccentColorOptions.Add(new AccentColorOption { Name = kvp.Key, ColorHex = kvp.Value });
            }

            LoadCurrentTheme();
        }

        private void LoadCurrentTheme()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                if (user == null) return;

                var pref = ThemeService.GetThemePreference(user.UserId);
                SelectedTheme = pref.ThemeName;
                IsDarkMode = pref.IsDarkMode;
                SelectedAccentColor = pref.AccentColor;

                // Find the accent color name
                var match = ThemeService.AccentColors.FirstOrDefault(kvp => kvp.Value == pref.AccentColor);
                SelectedAccentColorName = match.Key ?? "Blurple";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading theme: {ex.Message}";
            }
        }

        private void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            SelectedTheme = IsDarkMode ? "Dark" : "Light";
        }

        private void SelectAccentColor(AccentColorOption option)
        {
            if (option == null) return;
            SelectedAccentColor = option.ColorHex;
            SelectedAccentColorName = option.Name;
        }

        private void SelectTheme(string themeName)
        {
            if (string.IsNullOrEmpty(themeName)) return;
            SelectedTheme = themeName;
            IsDarkMode = themeName != "Light";
        }

        private async Task SaveThemeAsync()
        {
            var user = CurrentUserService.CurrentUser;
            if (user == null) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            try
            {
                var success = ThemeService.SaveThemePreference(
                    user.UserId,
                    SelectedTheme,
                    IsDarkMode,
                    SelectedAccentColor
                );

                if (success)
                {
                    // Apply theme to the app
                    ApplyTheme();
                    StatusMessage = "Theme saved successfully!";
                }
                else
                {
                    StatusMessage = "Failed to save theme.";
                }
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

        private void ApplyTheme()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current != null)
                {
                    Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
                }
            });
        }
    }

    /// <summary>
    /// Represents an accent color option for display in the UI
    /// </summary>
    public class AccentColorOption
    {
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
    }
}
