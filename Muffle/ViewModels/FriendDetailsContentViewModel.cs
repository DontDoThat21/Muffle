using Muffle.Data.Models;
using Muffle.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Muffle.ViewModels
{
    public class FriendDetailsContentViewModel : BindableObject
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();
        private WebSocketService _webSocketService;
        private UsersService _userService;
        private string _messageToSend;


        public Friend _friendSelected;
        public User _userSelected;

        public ICommand SendMessageCommand { get; }

        public string MessageToSend
        {
            get => _messageToSend;
            set
            {
                _messageToSend = value;
                OnPropertyChanged();
            }
        }

        public FriendDetailsContentViewModel()
        {
            _webSocketService = new WebSocketService();
            _userService = new UsersService();
            SendMessageCommand = new Command<string>(SendMessage);

            Task.Run(async () => await InitializeAsync());
        }

        // open ws conn
        private async Task InitializeAsync()
        {
            try
            {
                await _webSocketService.ConnectAsync(new Uri("ws://localhost:8080"));
                // Connection successful
                StartReceivingMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Handle connection error
            }
        }

        private async void StartReceivingMessages()
        {
            while (true)
            {
                string message = await _webSocketService.ReceiveMessageAsync(); // Receive message from WebSocket server
                Device.BeginInvokeOnMainThread(() =>
                {
                    ChatMessages.Add(new ChatMessage { Content = message, Sender = _userSelected, Timestamp = DateTime.Now });
                });
            }
        }

        public async void SendMessage(string message)
        {
            // Send message to WebSocket server
            if (!string.IsNullOrEmpty(MessageToSend))
            {
                await _webSocketService.SendMessageAsync(MessageToSend);
                ChatMessages.Add(new ChatMessage { Content = MessageToSend, Sender = _userSelected, Timestamp = DateTime.Now });
                MessageToSend = string.Empty; // Clear the message input after sending
            }
        }

    }
}
