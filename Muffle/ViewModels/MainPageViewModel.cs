using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.Services;
using Muffle.Views;

namespace Muffle.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        //private readonly MainPageModel MainPage;
        public ObservableCollection<Server> Servers { get; set; }
        public List<Friend> Friends { get; set; }

        private Server _selectedServer;
        private Friend _selectedFriend;
        public Server SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (_selectedServer != value)
                {
                    _selectedServer = value;
                    OnPropertyChanged(nameof(SelectedServer));
                    PopulateDynamicContentForSelectedServer();
                }
            }
        }

        public Friend SelectedFriend
        {
            get => _selectedFriend;
            set
            {
                if (_selectedFriend != value)
                {
                    _selectedFriend = value;
                    OnPropertyChanged(nameof(SelectedFriend));
                    PopulateDynamicContentForSelectedFriend();
                }
            }
        }

        private ContentView _dynamicServerServerContent;
        private ContentView _dynamicFriendServerContent;

        public ContentView DynamicFriendContent
        {
            get => _dynamicFriendServerContent;
            set
            {
                if (_dynamicFriendServerContent != value)
                {
                    _dynamicFriendServerContent = value;
                    OnPropertyChanged(nameof(DynamicFriendContent));
                }
            }
        }

        public ContentView DynamicServerContent
        {
            get => _dynamicServerServerContent;
            set
            {
                if (_dynamicServerServerContent != value)
                {
                    _dynamicServerServerContent = value;
                    OnPropertyChanged(nameof(DynamicServerContent));
                }
            }
        }

        public User User { get; set; }

        private void PopulateDynamicContentForSelectedServer()
        {
            // Clear existing content
            DynamicServerContent.Content = null;

            // Populate dynamic content based on the selected server
            if (SelectedServer != null)
            {
                var label = new Label
                {
                    Text = $"Content for {SelectedServer.Name}",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                DynamicServerContent.Content = label;
            }
        }

        private void PopulateDynamicContentForSelectedFriend()
        {
            // Clear existing content
            DynamicFriendContent.Content = null;

            // Populate dynamic content based on the selected server
            if (SelectedFriend != null)
            {
                var label = new Label
                {
                    Text = $"Content for {SelectedFriend.Name}",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                DynamicFriendContent.Content = label;
            }
        }

        public ICommand SelectServerCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainPageViewModel()
        {
            User = new User();
            User = UsersService.GetUser();
            SelectServerCommand = new Command<Server>(ExecuteSelectServerCommand);
            LogoutCommand = new Command(async () => await ExecuteLogoutCommandAsync());
            Servers = new ObservableCollection<Server>(UsersService.GetUsersServers() ?? new List<Server>());
            Friends = UsersService.GetUsersFriends();
        }

        private void ExecuteSelectServerCommand(Server server)
        {
            SelectedServer = server;
        }

        public void RefreshServers()
        {
            var updatedServers = UsersService.GetUsersServers() ?? new List<Server>();
            Servers.Clear();
            foreach (var server in updatedServers)
            {
                Servers.Add(server);
            }
        }

        private async Task ExecuteLogoutCommandAsync()
        {
            try
            {
                // Clear secure storage token
                TokenStorageService.RemoveToken();

                // Logout from current session (this also revokes the token in the database)
                CurrentUserService.Logout();

                // Navigate back to authentication page
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var authPage = new AuthenticationPage();
                    authPage.AuthenticationSucceeded += (sender, user) =>
                    {
                        CurrentUserService.CurrentUser = user;
                        Application.Current!.MainPage = new AppShell();
                    };
                    Application.Current!.MainPage = authPage;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout: {ex.Message}");
            }
        }

    }
}
