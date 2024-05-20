using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Muffle.Data.Models;

namespace Muffle.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        //private readonly MainPageModel MainPage;
        public List<Server> Servers { get; set; }
        public ObservableCollection<Friend> Friends { get; set; }

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

        public MainPageViewModel()
        {
            User = new User();
            User = User.GetUser();
            SelectServerCommand = new Command<Server>(ExecuteSelectServerCommand);
            Servers = User.GetUsersServers();
            Friends = User.GetUsersFriends();
        }

        private void ExecuteSelectServerCommand(Server server)
        {
            SelectedServer = server;
        }

    }
}
