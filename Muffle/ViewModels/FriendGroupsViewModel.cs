using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class FriendGroupsViewModel : BindableObject
    {
        private readonly int _currentUserId;

        public ObservableCollection<FriendGroup> Groups { get; set; } = new();
        public List<Friend> AllFriends { get; set; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasGroups)); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);
        public bool HasGroups => !IsLoading && Groups.Count > 0;

        public ICommand CreateGroupCommand { get; }
        public ICommand DeleteGroupCommand { get; }
        public ICommand RenameGroupCommand { get; }
        public ICommand AddFriendToGroupCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler<FriendGroup> GroupCallRequested;

        public FriendGroupsViewModel()
        {
            var user = CurrentUserService.CurrentUser;
            _currentUserId = user?.UserId ?? 0;

            CreateGroupCommand = new Command(async () => await CreateGroupAsync());
            DeleteGroupCommand = new Command<FriendGroup>(async g => await DeleteGroupAsync(g));
            RenameGroupCommand = new Command<FriendGroup>(async g => await RenameGroupAsync(g));
            AddFriendToGroupCommand = new Command<FriendGroup>(async g => await AddFriendToGroupAsync(g));
            RefreshCommand = new Command(async () => await LoadGroupsAsync());

            AllFriends = UsersService.GetUsersFriends() ?? new List<Friend>();
            _ = LoadGroupsAsync();
        }

        public async Task LoadGroupsAsync()
        {
            IsLoading = true;
            StatusMessage = null;
            try
            {
                var groups = await Task.Run(() => FriendGroupService.GetGroupsForUser(_currentUserId));
                Groups.Clear();
                foreach (var g in groups)
                    Groups.Add(g);

                if (Groups.Count == 0)
                    StatusMessage = "No groups yet. Create one to organize your friends.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load groups: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CreateGroupAsync()
        {
            var name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "New Friend Group", "Enter a name for the group:", placeholder: "e.g. Gaming, Work");

            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                await Task.Run(() => FriendGroupService.CreateGroup(_currentUserId, name.Trim()));
                await LoadGroupsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to create group: {ex.Message}";
            }
        }

        private async Task DeleteGroupAsync(FriendGroup group)
        {
            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Group", $"Delete group \"{group.Name}\"? Members won't be removed from your friends list.", "Delete", "Cancel");

            if (!confirmed) return;

            try
            {
                await Task.Run(() => FriendGroupService.DeleteGroup(group.GroupId, _currentUserId));
                await LoadGroupsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to delete group: {ex.Message}";
            }
        }

        private async Task RenameGroupAsync(FriendGroup group)
        {
            var newName = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Rename Group", "Enter a new name:", initialValue: group.Name);

            if (string.IsNullOrWhiteSpace(newName) || newName.Trim() == group.Name) return;

            try
            {
                await Task.Run(() => FriendGroupService.RenameGroup(group.GroupId, _currentUserId, newName.Trim()));
                await LoadGroupsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to rename group: {ex.Message}";
            }
        }

        private async Task AddFriendToGroupAsync(FriendGroup group)
        {
            var available = await Task.Run(() =>
                FriendGroupService.GetFriendsNotInGroup(group.GroupId, _currentUserId));

            if (available.Count == 0)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Add Friend", "All friends are already in this group.", "OK");
                return;
            }

            var names = available.Select(f => f.Name).ToArray();
            var chosen = await Application.Current!.MainPage!.DisplayActionSheet(
                $"Add friend to \"{group.Name}\"", "Cancel", null, names);

            if (string.IsNullOrEmpty(chosen) || chosen == "Cancel") return;

            var friend = available.FirstOrDefault(f => f.Name == chosen);
            if (friend == null) return;

            try
            {
                await Task.Run(() => FriendGroupService.AddFriendToGroup(group.GroupId, friend.Id));
                await LoadGroupsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to add friend: {ex.Message}";
            }
        }

        public void RemoveFriendFromGroup(FriendGroup group, Friend friend)
        {
            try
            {
                FriendGroupService.RemoveFriendFromGroup(group.GroupId, friend.Id);
                group.Members.Remove(friend);
                // Refresh count display by re-inserting the group
                var idx = Groups.IndexOf(group);
                if (idx >= 0)
                {
                    Groups.Remove(group);
                    Groups.Insert(idx, group);
                }
                OnPropertyChanged(nameof(HasGroups));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to remove friend: {ex.Message}";
            }
        }

        public void RequestGroupCall(FriendGroup group)
        {
            GroupCallRequested?.Invoke(this, group);
        }
    }
}
