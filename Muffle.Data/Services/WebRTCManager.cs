//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Muffle.Data.Services;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using WebRTCme;
////using WebRTCme.Connection;

//public class WebRTCManager
//{
//    private IRTCPeerConnection _peerConnection;
//    private IMediaStream _localStream;
//    private readonly ISignalingService _signalingService;

//    public WebRTCManager(ISignalingService signalingService)
//    {
//        _signalingService = signalingService;
//    }

//    public async Task InitializeAsync()
//    {
//        var config = new RTCConfiguration
//        {
//            IceServers = new RTCIceServer[]
//            {
//                new RTCIceServer { Urls = new string[] { "stun:stun.l.google.com:19302" }  }
//            }
//        };
//        _peerConnection = WebRtcFactory.CreatePeerConnection(config);

//        _peerConnection.OnIceCandidate += OnIceCandidateHandler;
//        _peerConnection.OnTrack += OnTrackHandler;

//        var constraints = new MediaStreamConstraints
//        {
//            Video = true,
//            Audio = true
//        };
//        _localStream = await Navigator.GetUserMedia(constraints);

//        foreach (var track in _localStream.GetTracks())
//        {
//            _peerConnection.AddTrack(track, _localStream);
//        }
//    }

//    private void OnIceCandidateHandler(IRTCPeerConnectionIceEvent evt)
//    {
//        if (evt.Candidate != null)
//        {
//            _signalingService.SendMessageAsync(evt.Candidate.ToJson());
//        }
//    }

//    private void OnTrackHandler(IRTCPeerConnectionTrackEvent evt)
//    {
//        // Handle the track event (e.g., display remote stream)
//    }

//    public async Task StartCallAsync()
//    {
//        var offer = await _peerConnection.CreateOffer();
//        await _peerConnection.SetLocalDescription(offer);

//        _signalingService.SendMessageAsync(offer.Sdp);
//    }

//    public async Task ReceiveCallAsync(string sdp)
//    {
//        var description = new RTCSessionDescription(RTCSdpType.Offer, sdp);
//        await _peerConnection.SetRemoteDescription(description);

//        var answer = await _peerConnection.CreateAnswer();
//        await _peerConnection.SetLocalDescription(answer);

//        _signalingService.SendMessageAsync(answer.Sdp);
//    }

//    public async Task HandleIceCandidateAsync(string candidate)
//    {
//        var iceCandidate = new RTCIceCandidate(new RTCIceCandidateInit
//        {
//            Candidate = candidate
//        });
//        await _peerConnection.AddIceCandidate(iceCandidate);
//    }
//}
