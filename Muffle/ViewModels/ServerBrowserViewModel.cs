using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class ServerBrowserViewModel : BindableObject
    {
        private ObservableCollection<Server> _publicServers = new();
        private string _searchText = string.Empty;

        public ObservableCollection<Server> PublicServers
        {
            get => _publicServers;
            set
            {
                _publicServers = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand JoinServerCommand { get; }

        public ServerBrowserViewModel()
        {
            SearchCommand = new Command(ExecuteSearch);
            LoadCommand = new Command(ExecuteLoad);
            JoinServerCommand = new Command<Server>(ExecuteJoinServer);

            ExecuteLoad();
        }

        private void ExecuteLoad()
        {
            try
            {
                var servers = ServerBrowserService.GetPublicServers();
                PublicServers.Clear();
                foreach (var server in servers)
                    PublicServers.Add(server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading public servers: {ex.Message}");
            }
        }

        private void ExecuteSearch()
        {
            try
            {
                var servers = string.IsNullOrWhiteSpace(SearchText)
                    ? ServerBrowserService.GetPublicServers()
                    : ServerBrowserService.SearchServers(SearchText);

                PublicServers.Clear();
                foreach (var server in servers)
                    PublicServers.Add(server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching servers: {ex.Message}");
            }
        }

        private void ExecuteJoinServer(Server? server)
        {
            if (server == null) return;

            try
            {
                var userId = CurrentUserService.GetCurrentUserId();
                ServerBrowserService.JoinServer((int)server.Id, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining server: {ex.Message}");
            }
        }
    }
}
