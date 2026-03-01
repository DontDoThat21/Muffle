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
using WebRTCme;

namespace Muffle.ViewModels
{
    public class FriendDetailsContentViewModel : BindableObject
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();
        private readonly ISignalingService _signalingService;
        private readonly IImagePickerService _imagePickerService;
        private UsersService _userService;
        private string _messageToSend;
        private WebRTCManager? _webRTCManager;

        private bool _isScreenSharing;
        public bool IsScreenSharing
        {
            get => _isScreenSharing;
            private set
            {
                if (_isScreenSharing == value) return;
                _isScreenSharing = value;
                OnPropertyChanged();
                ScreenSharingStateChanged?.Invoke(this, value);
            }
        }

        /// <summary>Raised whenever screen-sharing starts or stops. Arg is the new IsScreenSharing value.</summary>
        public event EventHandler<bool> ScreenSharingStateChanged;

        public ICommand SendImageCommand { get; }
        public ICommand EndCallCommand { get; }

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
            EndCallCommand = new Command(async () => await EndCallAsync());

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
                    
                    MessageWrapper? messageWrapper = null;
                    try
                    {
                        messageWrapper = JsonSerializer.Deserialize<MessageWrapper>(messageJson);
                    }
                    catch
                    {
                        // If deserialization fails, treat as legacy text message
                    }

                    if (messageWrapper != null)
                    {
                        await HandleMessageWrapperAsync(messageWrapper);
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ChatMessages.Add(new ChatMessage 
                            { 
                                Content = messageJson, 
                                Sender = _userSelected, 
                                Timestamp = DateTime.Now,
                                Type = MessageType.Text
                            });
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving message: {ex.Message}");
                    break;
                }
            }
        }

        private async Task HandleMessageWrapperAsync(MessageWrapper messageWrapper)
        {
            switch (messageWrapper.Type)
            {
                case MessageType.Text:
                case MessageType.Image:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var chatMessage = new ChatMessage
                        {
                            Content = messageWrapper.Content,
                            Sender = _userSelected,
                            Timestamp = messageWrapper.Timestamp,
                            Type = messageWrapper.Type,
                            ImageData = messageWrapper.ImageData
                        };
                        ChatMessages.Add(chatMessage);

                        if (messageWrapper.Type == MessageType.Text)
                            _ = FetchAndAttachLinkPreviewAsync(chatMessage, messageWrapper.Content);
                    });
                    break;

                case MessageType.CallInvite:
                    await HandleIncomingCallInviteAsync(messageWrapper);
                    break;

                case MessageType.WebRtcOffer:
                    await HandleWebRtcOfferAsync(messageWrapper);
                    break;

                case MessageType.WebRtcAnswer:
                    await HandleWebRtcAnswerAsync(messageWrapper);
                    break;

                case MessageType.IceCandidate:
                    await HandleIceCandidateAsync(messageWrapper);
                    break;

                case MessageType.CallEnd:
                    await HandleCallEndAsync();
                    break;

                case MessageType.ScreenShareStart:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ChatMessages.Add(new ChatMessage
                        {
                            Content = $"🖥️ {messageWrapper.SenderName ?? "Contact"} started sharing their screen",
                            Sender = _userSelected,
                            Timestamp = messageWrapper.Timestamp,
                            Type = MessageType.Text
                        });
                    });
                    break;

                case MessageType.ScreenShareStop:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ChatMessages.Add(new ChatMessage
                        {
                            Content = "🖥️ Screen sharing ended",
                            Sender = _userSelected,
                            Timestamp = messageWrapper.Timestamp,
                            Type = MessageType.Text
                        });
                    });
                    break;

                default:
                    Console.WriteLine($"Unknown message type: {messageWrapper.Type}");
                    break;
            }
        }

        private async Task HandleIncomingCallInviteAsync(MessageWrapper messageWrapper)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var chatMessage = new ChatMessage
                {
                    Content = $"📞 Incoming {messageWrapper.CallType} call from {messageWrapper.SenderName}",
                    Sender = _userSelected,
                    Timestamp = messageWrapper.Timestamp,
                    Type = MessageType.Text
                };
                ChatMessages.Add(chatMessage);
            });
            
            Console.WriteLine($"Incoming {messageWrapper.CallType} call from user {messageWrapper.SenderId}");
        }

        private async Task HandleWebRtcOfferAsync(MessageWrapper messageWrapper)
        {
            if (_webRTCManager == null || string.IsNullOrEmpty(messageWrapper.SdpOffer))
            {
                Console.WriteLine("Cannot handle offer: no WebRTC manager or missing SDP");
                return;
            }

            bool includeVideo = messageWrapper.CallType == "video";
            await _webRTCManager.AcceptCallAsync(messageWrapper.SdpOffer, includeVideo);
        }

        private async Task HandleWebRtcAnswerAsync(MessageWrapper messageWrapper)
        {
            if (_webRTCManager == null || string.IsNullOrEmpty(messageWrapper.SdpAnswer))
            {
                Console.WriteLine("Cannot handle answer: no WebRTC manager or missing SDP");
                return;
            }

            await _webRTCManager.HandleAnswerAsync(messageWrapper.SdpAnswer);
        }

        private async Task HandleIceCandidateAsync(MessageWrapper messageWrapper)
        {
            if (_webRTCManager == null || string.IsNullOrEmpty(messageWrapper.IceCandidateData))
            {
                Console.WriteLine("Cannot handle ICE candidate: no WebRTC manager or missing data");
                return;
            }

            await _webRTCManager.HandleIceCandidateAsync(messageWrapper.IceCandidateData);
        }

        private async Task HandleCallEndAsync()
        {
            if (_webRTCManager != null)
            {
                _webRTCManager.Cleanup();
                _webRTCManager = null;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                var chatMessage = new ChatMessage
                {
                    Content = "📞 Call ended",
                    Sender = _userSelected,
                    Timestamp = DateTime.Now,
                    Type = MessageType.Text
                };
                ChatMessages.Add(chatMessage);
            });
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
                    SenderId = _userSelected?.UserId ?? 0
                };

                await _signalingService.SendMessageWrapperAsync(messageWrapper);

                var sentContent = MessageToSend;
                var chatMessage = new ChatMessage
                {
                    Content = sentContent,
                    Sender = _userSelected,
                    Timestamp = DateTime.Now,
                    Type = MessageType.Text
                };
                ChatMessages.Add(chatMessage);
                MessageToSend = string.Empty; // Clear the message input after sending

                _ = FetchAndAttachLinkPreviewAsync(chatMessage, sentContent);
            }
        }

        private async Task FetchAndAttachLinkPreviewAsync(ChatMessage chatMessage, string content)
        {
            var links = MessageSearchService.ExtractLinks(content);
            if (links.Count == 0) return;

            var preview = await LinkPreviewService.FetchPreviewAsync(links[0]);
            if (preview != null)
            {
                Device.BeginInvokeOnMainThread(() => chatMessage.LinkPreview = preview);
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
                    SenderId = _userSelected?.UserId ?? 0
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

        public async Task StartVoiceCallAsync()
        {
            try
            {
                if (_friendSelected == null)
                {
                    Console.WriteLine("No friend selected for voice call");
                    return;
                }

                Console.WriteLine("Starting voice call...");
                
                var userId = _userSelected?.UserId ?? 0;
                var window = CrossWebRtc.Current.Window(null);
                _webRTCManager = new WebRTCManager(_signalingService, userId, window);

                _webRTCManager.OnCallStateChanged += (state) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var chatMessage = new ChatMessage
                        {
                            Content = $"📞 Voice call state: {state}",
                            Sender = _userSelected,
                            Timestamp = DateTime.Now,
                            Type = MessageType.Text
                        };
                        ChatMessages.Add(chatMessage);
                    });
                };

                _webRTCManager.OnError += (error) =>
                {
                    Console.WriteLine($"WebRTC Error: {error}");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var chatMessage = new ChatMessage
                        {
                            Content = $"❌ Call error: {error}",
                            Sender = _userSelected,
                            Timestamp = DateTime.Now,
                            Type = MessageType.Text
                        };
                        ChatMessages.Add(chatMessage);
                    });
                };

                await _webRTCManager.StartCallAsync(_friendSelected.Id, includeVideo: false);
                
                Device.BeginInvokeOnMainThread(() =>
                {
                    var chatMessage = new ChatMessage
                    {
                        Content = $"📞 Calling {_friendSelected.Name}...",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    };
                    ChatMessages.Add(chatMessage);
                });
                
                Console.WriteLine("Voice call initiated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting voice call: {ex.Message}");
                Device.BeginInvokeOnMainThread(() =>
                {
                    var chatMessage = new ChatMessage
                    {
                        Content = $"❌ Failed to start voice call: {ex.Message}",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    };
                    ChatMessages.Add(chatMessage);
                });
            }
        }

        public async Task StartVideoCallAsync()
        {
            try
            {
                if (_friendSelected == null)
                {
                    Console.WriteLine("No friend selected for video call");
                    return;
                }

                Console.WriteLine("Starting video call...");
                
                var userId = _userSelected?.UserId ?? 0;
                var window = CrossWebRtc.Current.Window(null);
                _webRTCManager = new WebRTCManager(_signalingService, userId, window);

                _webRTCManager.OnCallStateChanged += (state) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var chatMessage = new ChatMessage
                        {
                            Content = $"📹 Video call state: {state}",
                            Sender = _userSelected,
                            Timestamp = DateTime.Now,
                            Type = MessageType.Text
                        };
                        ChatMessages.Add(chatMessage);
                    });
                };

                _webRTCManager.OnError += (error) =>
                {
                    Console.WriteLine($"WebRTC Error: {error}");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var chatMessage = new ChatMessage
                        {
                            Content = $"❌ Call error: {error}",
                            Sender = _userSelected,
                            Timestamp = DateTime.Now,
                            Type = MessageType.Text
                        };
                        ChatMessages.Add(chatMessage);
                    });
                };

                _webRTCManager.OnRemoteStreamAdded += (stream) =>
                {
                    Console.WriteLine("Remote video stream received");
                };

                await _webRTCManager.StartCallAsync(_friendSelected.Id, includeVideo: true);
                
                Device.BeginInvokeOnMainThread(() =>
                {
                    var chatMessage = new ChatMessage
                    {
                        Content = $"📹 Video calling {_friendSelected.Name}...",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    };
                    ChatMessages.Add(chatMessage);
                });
                
                Console.WriteLine("Video call initiated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting video call: {ex.Message}");
                Device.BeginInvokeOnMainThread(() =>
                {
                    var chatMessage = new ChatMessage
                    {
                        Content = $"❌ Failed to start video call: {ex.Message}",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    };
                    ChatMessages.Add(chatMessage);
                });
            }
        }

        public async Task EndCallAsync()
        {
            try
            {
                if (_webRTCManager == null)
                {
                    Console.WriteLine("No active call to end");
                    return;
                }

                await _webRTCManager.EndCallAsync();
                _webRTCManager = null;
                IsScreenSharing = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    var chatMessage = new ChatMessage
                    {
                        Content = "📞 Call ended",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    };
                    ChatMessages.Add(chatMessage);
                });

                Console.WriteLine("Call ended successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ending call: {ex.Message}");
            }
        }

        public async Task StartScreenShareAsync()
        {
            try
            {
                if (_webRTCManager == null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ChatMessages.Add(new ChatMessage
                        {
                            Content = "❌ Start a voice or video call first before sharing your screen",
                            Sender = _userSelected,
                            Timestamp = DateTime.Now,
                            Type = MessageType.Text
                        });
                    });
                    return;
                }

                await _webRTCManager.StartScreenShareAsync(_friendSelected?.Id ?? 0);
                IsScreenSharing = true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    ChatMessages.Add(new ChatMessage
                    {
                        Content = "🖥️ Screen sharing started",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting screen share: {ex.Message}");
                Device.BeginInvokeOnMainThread(() =>
                {
                    ChatMessages.Add(new ChatMessage
                    {
                        Content = $"❌ Failed to share screen: {ex.Message}",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    });
                });
            }
        }

        public async Task StopScreenShareAsync()
        {
            try
            {
                if (_webRTCManager == null) return;

                await _webRTCManager.StopScreenShareAsync();
                IsScreenSharing = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    ChatMessages.Add(new ChatMessage
                    {
                        Content = "🖥️ Screen sharing stopped",
                        Sender = _userSelected,
                        Timestamp = DateTime.Now,
                        Type = MessageType.Text
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping screen share: {ex.Message}");
            }
        }

    }
}
