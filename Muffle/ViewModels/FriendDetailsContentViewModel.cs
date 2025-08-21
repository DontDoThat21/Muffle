using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.Json;

namespace Muffle.ViewModels
{
    public class FriendDetailsContentViewModel : BindableObject
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();
        private readonly ISignalingService _signalingService;
        private readonly IImagePickerService _imagePickerService;
        private UsersService _userService;
        private string _messageToSend;

        //private readonly WebRTCManager _webRTCManager;
        public Command StartCallCommand { get; }
        public Command ReceiveCallCommand { get; }
        public ICommand SendImageCommand { get; }

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

        public FriendDetailsContentViewModel()
        {
            _signalingService = new SignalingService();
            _imagePickerService = new ImagePickerService();
            _userService = new UsersService();
            SendMessageCommand = new Command<string>(SendMessage);
            SendImageCommand = new Command(async () => await SendImageAsync());
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
                try
                {
                    string messageJson = await _signalingService.ReceiveMessageAsync();
                    
                    // Try to deserialize as MessageWrapper first
                    MessageWrapper? messageWrapper = null;
                    try
                    {
                        messageWrapper = JsonSerializer.Deserialize<MessageWrapper>(messageJson);
                    }
                    catch
                    {
                        // If deserialization fails, treat as legacy text message
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (messageWrapper != null)
                        {
                            // Handle structured message
                            var chatMessage = new ChatMessage 
                            { 
                                Content = messageWrapper.Content,
                                Sender = _userSelected, 
                                Timestamp = messageWrapper.Timestamp,
                                Type = messageWrapper.Type,
                                ImageData = messageWrapper.ImageData
                            };
                            ChatMessages.Add(chatMessage);
                        }
                        else
                        {
                            // Handle legacy text message
                            ChatMessages.Add(new ChatMessage 
                            { 
                                Content = messageJson, 
                                Sender = _userSelected, 
                                Timestamp = DateTime.Now,
                                Type = MessageType.Text
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving message: {ex.Message}");
                    break;
                }
            }
        }

        public async void SendMessage(string message)
        {
            // Send message to WebSocket server
            if (!string.IsNullOrEmpty(MessageToSend))
            {
                var messageWrapper = new MessageWrapper
                {
                    Type = MessageType.Text,
                    Content = MessageToSend,
                    Timestamp = DateTime.Now,
                    SenderName = _userSelected?.Name ?? "Unknown",
                    SenderId = _userSelected?.Id ?? 0
                };

                await _signalingService.SendMessageWrapperAsync(messageWrapper);
                
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

        public async Task SendImageAsync()
        {
            try
            {
                // Pick an image using the image picker service
                string? imagePath = await _imagePickerService.PickImageAsync();
                if (imagePath == null)
                {
                    Console.WriteLine("No image selected");
                    return;
                }

                // Convert image to byte array
                byte[]? imageBytes = await _imagePickerService.ConvertImageToByteArrayAsync(imagePath);
                if (imageBytes == null)
                {
                    Console.WriteLine("Failed to convert image to byte array");
                    return;
                }

                // Convert to base64 for transmission
                string imageData = _imagePickerService.ConvertByteArrayToBase64(imageBytes);
                
                var messageWrapper = new MessageWrapper
                {
                    Type = MessageType.Image,
                    Content = $"📷 Image: {Path.GetFileName(imagePath)}",
                    ImageData = imageData,
                    Timestamp = DateTime.Now,
                    SenderName = _userSelected?.Name ?? "Unknown",
                    SenderId = _userSelected?.Id ?? 0
                };

                await _signalingService.SendMessageWrapperAsync(messageWrapper);
                
                ChatMessages.Add(new ChatMessage 
                { 
                    Content = $"📷 Image: {Path.GetFileName(imagePath)}", 
                    Sender = _userSelected, 
                    Timestamp = DateTime.Now,
                    Type = MessageType.Image,
                    ImageData = imageData,
                    ImagePath = imagePath
                });
                
                Console.WriteLine($"Image sent successfully: {imagePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending image: {ex.Message}");
            }
        }

    }
}
