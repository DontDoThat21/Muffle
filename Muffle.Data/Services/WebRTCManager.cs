using Muffle.Data.Models;
using Muffle.Data.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WebRTCme;

namespace Muffle.Data.Services
{
    public enum CallState
    {
        Idle,
        Calling,
        Ringing,
        Connected,
        Ended
    }

    public class WebRTCManager
    {
        private IRTCPeerConnection? _peerConnection;
        private IMediaStream? _localStream;
        private IMediaStream? _screenShareStream;
        private readonly ISignalingService _signalingService;
        private readonly IWindow _window;
        private readonly int _userId;
        private string _callType = "voice";

        public bool IsScreenSharing { get; private set; }

        public event Action<IMediaStream>? OnRemoteStreamAdded;
        public event Action<CallState>? OnCallStateChanged;
        public event Action<string>? OnError;

        public CallState CurrentCallState { get; private set; } = CallState.Idle;

        public WebRTCManager(ISignalingService signalingService, int userId, IWindow window)
        {
            _signalingService = signalingService;
            _userId = userId;
            _window = window;
        }

        public async Task InitializeAsync(bool includeVideo)
        {
            try
            {
                _callType = includeVideo ? "video" : "voice";
                
                var config = new RTCConfiguration
                {
                    IceServers = new RTCIceServer[]
                    {
                        new RTCIceServer { Urls = new string[] { "stun:stun.l.google.com:19302" } }
                    }
                };
                
                _peerConnection = _window.RTCPeerConnection(config);

                _peerConnection.OnIceCandidate += OnIceCandidateHandler;
                _peerConnection.OnTrack += OnTrackHandler;
                _peerConnection.OnIceConnectionStateChange += OnIceConnectionStateChangeHandler;

                var constraints = new MediaStreamConstraints
                {
                    Audio = new MediaStreamContraintsUnion { Value = true },
                    Video = includeVideo ? new MediaStreamContraintsUnion { Value = true } : null
                };

                _localStream = await _window.Navigator().MediaDevices.GetUserMedia(constraints);

                foreach (var track in _localStream.GetTracks())
                {
                    _peerConnection.AddTrack(track, _localStream);
                }

                Console.WriteLine($"WebRTC initialized for {_callType} call");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to initialize WebRTC: {ex.Message}");
                throw;
            }
        }

        private void OnIceCandidateHandler(object? sender, IRTCPeerConnectionIceEvent evt)
        {
            try
            {
                if (evt?.Candidate != null)
                {
                    var candidateWrapper = new MessageWrapper
                    {
                        Type = MessageType.IceCandidate,
                        IceCandidateData = JsonSerializer.Serialize(new
                        {
                            candidate = evt.Candidate.Candidate,
                            sdpMid = evt.Candidate.SdpMid,
                            sdpMLineIndex = evt.Candidate.SdpMLineIndex
                        }),
                        SenderId = _userId,
                        Timestamp = DateTime.Now
                    };
                    
                    _signalingService.SendMessageWrapperAsync(candidateWrapper).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling ICE candidate: {ex.Message}");
            }
        }

        private void OnTrackHandler(object? sender, IRTCTrackEvent evt)
        {
            try
            {
                if (evt?.Streams != null && evt.Streams.Length > 0)
                {
                    OnRemoteStreamAdded?.Invoke(evt.Streams[0]);
                    Console.WriteLine("Remote stream received");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling track: {ex.Message}");
            }
        }

        private void OnIceConnectionStateChangeHandler(object? sender, EventArgs e)
        {
            try
            {
                if (_peerConnection != null)
                {
                    var state = _peerConnection.IceConnectionState;
                    Console.WriteLine($"ICE connection state: {state}");
                    
                    switch (state)
                    {
                        case RTCIceConnectionState.Connected:
                        case RTCIceConnectionState.Completed:
                            UpdateCallState(CallState.Connected);
                            break;
                        case RTCIceConnectionState.Disconnected:
                        case RTCIceConnectionState.Failed:
                        case RTCIceConnectionState.Closed:
                            UpdateCallState(CallState.Ended);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling ICE connection state change: {ex.Message}");
            }
        }

        public async Task StartCallAsync(int targetUserId, bool includeVideo)
        {
            try
            {
                await InitializeAsync(includeVideo);
                UpdateCallState(CallState.Calling);

                var callInvite = new MessageWrapper
                {
                    Type = MessageType.CallInvite,
                    CallType = includeVideo ? "video" : "voice",
                    SenderId = _userId,
                    TargetUserId = targetUserId,
                    Timestamp = DateTime.Now
                };
                
                await _signalingService.SendMessageWrapperAsync(callInvite);

                var offer = await _peerConnection!.CreateOffer();
                await _peerConnection.SetLocalDescription(offer);

                var offerWrapper = new MessageWrapper
                {
                    Type = MessageType.WebRtcOffer,
                    SdpOffer = offer.Sdp,
                    CallType = _callType,
                    SenderId = _userId,
                    TargetUserId = targetUserId,
                    Timestamp = DateTime.Now
                };

                await _signalingService.SendMessageWrapperAsync(offerWrapper);
                Console.WriteLine($"Call initiated to user {targetUserId}");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to start call: {ex.Message}");
                UpdateCallState(CallState.Ended);
                throw;
            }
        }

        public async Task AcceptCallAsync(string sdpOffer, bool includeVideo)
        {
            try
            {
                await InitializeAsync(includeVideo);
                UpdateCallState(CallState.Connected);

                var description = new RTCSessionDescriptionInit
                {
                    Type = RTCSdpType.Offer,
                    Sdp = sdpOffer
                };
                
                await _peerConnection!.SetRemoteDescription(description);

                var answer = await _peerConnection.CreateAnswer();
                await _peerConnection.SetLocalDescription(answer);

                var answerWrapper = new MessageWrapper
                {
                    Type = MessageType.WebRtcAnswer,
                    SdpAnswer = answer.Sdp,
                    SenderId = _userId,
                    Timestamp = DateTime.Now
                };

                await _signalingService.SendMessageWrapperAsync(answerWrapper);
                Console.WriteLine("Call accepted, answer sent");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to accept call: {ex.Message}");
                UpdateCallState(CallState.Ended);
                throw;
            }
        }

        public async Task HandleAnswerAsync(string sdpAnswer)
        {
            try
            {
                if (_peerConnection == null)
                {
                    Console.WriteLine("No active peer connection");
                    return;
                }

                var description = new RTCSessionDescriptionInit
                {
                    Type = RTCSdpType.Answer,
                    Sdp = sdpAnswer
                };
                
                await _peerConnection.SetRemoteDescription(description);
                Console.WriteLine("Answer received and applied");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to handle answer: {ex.Message}");
                throw;
            }
        }

        public async Task HandleIceCandidateAsync(string candidateJson)
        {
            try
            {
                if (_peerConnection == null)
                {
                    Console.WriteLine("No active peer connection");
                    return;
                }

                var candidateData = JsonSerializer.Deserialize<Dictionary<string, object>>(candidateJson);
                if (candidateData == null) return;

                var candidateInit = new RTCIceCandidateInit
                {
                    Candidate = candidateData.ContainsKey("candidate") ? candidateData["candidate"].ToString() : null,
                    SdpMid = candidateData.ContainsKey("sdpMid") ? candidateData["sdpMid"].ToString() : null,
                    SdpMLineIndex = candidateData.ContainsKey("sdpMLineIndex") 
                        ? ushort.Parse(candidateData["sdpMLineIndex"].ToString() ?? "0") 
                        : (ushort)0
                };

                await _peerConnection.AddIceCandidate(candidateInit);
                Console.WriteLine("ICE candidate added");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling ICE candidate: {ex.Message}");
            }
        }

        public async Task StartScreenShareAsync(int targetUserId)
        {
            try
            {
                if (_peerConnection == null)
                    throw new InvalidOperationException("No active call. Start a voice or video call before sharing your screen.");

                var constraints = new MediaStreamConstraints
                {
                    Video = new MediaStreamContraintsUnion { Value = true }
                };

                _screenShareStream = await _window.Navigator().MediaDevices.GetDisplayMedia(constraints);

                foreach (var track in _screenShareStream.GetTracks())
                {
                    _peerConnection.AddTrack(track, _screenShareStream);
                }

                IsScreenSharing = true;

                var message = new MessageWrapper
                {
                    Type = MessageType.ScreenShareStart,
                    SenderId = _userId,
                    TargetUserId = targetUserId,
                    Timestamp = DateTime.Now
                };

                await _signalingService.SendMessageWrapperAsync(message);
                Console.WriteLine("Screen share started");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to start screen share: {ex.Message}");
                throw;
            }
        }

        public async Task StopScreenShareAsync()
        {
            try
            {
                if (_screenShareStream != null)
                {
                    foreach (var track in _screenShareStream.GetTracks())
                        track.Stop();
                    _screenShareStream = null;
                }

                IsScreenSharing = false;

                var message = new MessageWrapper
                {
                    Type = MessageType.ScreenShareStop,
                    SenderId = _userId,
                    Timestamp = DateTime.Now
                };

                await _signalingService.SendMessageWrapperAsync(message);
                Console.WriteLine("Screen share stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping screen share: {ex.Message}");
            }
        }

        public async Task EndCallAsync()
        {
            try
            {
                var endCallWrapper = new MessageWrapper
                {
                    Type = MessageType.CallEnd,
                    SenderId = _userId,
                    Timestamp = DateTime.Now
                };

                await _signalingService.SendMessageWrapperAsync(endCallWrapper);

                Cleanup();
                UpdateCallState(CallState.Ended);
                Console.WriteLine("Call ended");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ending call: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            try
            {
                if (_screenShareStream != null)
                {
                    foreach (var track in _screenShareStream.GetTracks())
                        track.Stop();
                    _screenShareStream = null;
                }

                IsScreenSharing = false;

                if (_localStream != null)
                {
                    foreach (var track in _localStream.GetTracks())
                    {
                        track.Stop();
                    }
                    _localStream = null;
                }

                _peerConnection?.Close();
                _peerConnection = null;

                Console.WriteLine("WebRTC resources cleaned up");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        private void UpdateCallState(CallState newState)
        {
            CurrentCallState = newState;
            OnCallStateChanged?.Invoke(newState);
        }
    }
}
