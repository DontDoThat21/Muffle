using Muffle.Data.Models;
using Muffle.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Muffle.ViewModels
{
    public class FriendDetailsContentViewModel : BindableObject
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();
        private readonly ISignalingService _signalingService;
        private UsersService _userService;
        private string _messageToSend;

        //private readonly WebRTCManager _webRTCManager;
        public Command StartCallCommand { get; }
        public Command ReceiveCallCommand { get; }

        public Friend _friendSelected;
        public User _userSelected;
        public string MessageToSend
        {
            get => _messageToSend;
            set
            {
                _messageToSend = value;
                OnPropertyChanged();
            }
        }
        public ICommand SendMessageCommand { get; }
        public ICommand SendImageCommand { get; }

        public FriendDetailsContentViewModel()
        {
            _signalingService = new SignalingService();
            _userService = new UsersService();
            SendMessageCommand = new Command<string>(SendMessage);
            SendImageCommand = new Command(async () => await SendImage());
            //_webRTCManager = new WebRTCManager(new SignalingService());
            //StartCallCommand = new Command(async () => await _webRTCManager.StartCall());
            //ReceiveCallCommand = new Command<string>(async (sdp) => await _webRTCManager.ReceiveCall(sdp));

            Task.Run(async () => await InitializeAsync());
        }

        // open ws conn
        private async Task InitializeAsync()
        {
            try
            {
                await _signalingService.ConnectAsync(new Uri("ws://localhost:8080"));
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
                string message = await _signalingService.ReceiveMessageAsync(); // Receive message from WebSocket server
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        // Try to parse as JSON to determine message type
                        var messageData = JsonSerializer.Deserialize<JsonElement>(message);
                        var messageType = messageData.GetProperty("type").GetString();
                        var content = messageData.GetProperty("content").GetString();

                        var chatMessage = new ChatMessage
                        {
                            Sender = _userSelected,
                            Timestamp = DateTime.Now
                        };

                        if (messageType == "image")
                        {
                            chatMessage.Type = MessageType.Image;
                            chatMessage.ImageData = Convert.FromBase64String(content);
                            if (messageData.TryGetProperty("fileName", out var fileNameElement))
                            {
                                chatMessage.ImageFileName = fileNameElement.GetString();
                            }
                        }
                        else
                        {
                            chatMessage.Type = MessageType.Text;
                            chatMessage.Content = content;
                        }

                        ChatMessages.Add(chatMessage);
                    }
                    catch (JsonException)
                    {
                        // Fallback for plain text messages (backward compatibility)
                        ChatMessages.Add(new ChatMessage 
                        { 
                            Content = message, 
                            Sender = _userSelected, 
                            Timestamp = DateTime.Now,
                            Type = MessageType.Text
                        });
                    }
                });
            }
        }

        public async void SendMessage(string message)
        {
            // Send message to WebSocket server
            if (!string.IsNullOrEmpty(MessageToSend))
            {
                await _signalingService.SendMessageAsync(MessageToSend);
                ChatMessages.Add(new ChatMessage 
                { 
                    Content = MessageToSend, 
                    Sender = _userSelected, 
                    Timestamp = DateTime.Now,
                    Type = MessageType.Text
                });
                MessageToSend = string.Empty; // Clear the message input after sending
            }
        }

        public async Task SendImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please select a photo"
                });

                if (result != null)
                {
                    // Read the image data
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageData = memoryStream.ToArray();

                    // Send image via WebSocket
                    await _signalingService.SendImageAsync(imageData, result.FileName);

                    // Add to local chat
                    ChatMessages.Add(new ChatMessage
                    {
                        Type = MessageType.Image,
                        ImageData = imageData,
                        ImageFileName = result.FileName,
                        Sender = _userSelected,
                        Timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking image: {ex.Message}");
                // Handle error - could show user notification
            }
        }

    }
}
