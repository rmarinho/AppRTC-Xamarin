using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using WebRTCBinding;
using Newtonsoft.Json;

namespace AppRTC
{
	public class ARDAppClient : NSObject, IARDAppClient, IARDWebSocketChannelDelegate, IRTCPeerConnectionDelegate, IRTCSessionDescriptionDelegate
	{
		static string kARDAppClientErrorDomain = "ARDAppClient";
		static int kARDAppClientErrorUnknown = -1;
		static int kARDAppClientErrorRoomFull = -2;
		static int kARDAppClientErrorCreateSDP = -3;
		static int kARDAppClientErrorSetSDP = -4;
		static int kARDAppClientErrorNetwork = -5;
		static int kARDAppClientErrorInvalidClient = -6;
		static int kARDAppClientErrorInvalidRoom = -7;
		static string kARDRoomServerHostUrl = "https://apprtc.appspot.com";
		static string kARDRoomServerByeFormat = "{0}/leave/{1}/{2}";
		static string kARDRoomServerRegisterFormat = "{0}/join/{1}";
		static string kARDRoomServerMessageFormat = "{0}/message/{1}/{2}";
		static string kARDDefaultSTUNServerUrl = "stun:stun.l.google.com:19302";
		static string kARDTurnRequestUrl = "https://computeengineondemand.appspot.com/turn?username=iapprtc&key=4080218913";

		ARDAppClientState _state;
		IARDAppClientDelegate _delegate;
		RTCPeerConnectionDelegate _peerConnectionDelegate;
		string _serverHostUrl;
		string _clientId;
		string _roomId;
		string _webSocketURL;
		string _webSocketRestURL;
		NSMutableArray _iceServers;
		List<ARDSignalingMessage> _messageQueue;

		ARDWebSocketChannel _channel;
		RTCPeerConnection _peerConnection;
		RTCPeerConnectionFactory _factory;

		RTCAudioTrack defaultAudioTrack;
		RTCVideoTrack defaultVideoTrack;

		bool _isTurnComplete;
		bool _hasReceivedSdp;
		bool _isRegisteredWithRoomServer => !string.IsNullOrEmpty(_clientId);
		bool _isInitiator;
		bool _isSpeakerEnabled;

		public IARDAppClientDelegate ARDAppClientDelegate => _delegate;

		public ARDAppClient(IARDAppClientDelegate ardpAppDelegate)
		{
			_delegate = ardpAppDelegate;
			_serverHostUrl = kARDRoomServerHostUrl;
			_iceServers = new NSMutableArray();
			_iceServers.Add(DefaultSTUNServer());
			_factory = new RTCPeerConnectionFactory();
			_messageQueue = new List<ARDSignalingMessage>();
			_isSpeakerEnabled = true;
		}

		public async void ConnectToRoomWithId(string roomName)
		{
			_state = ARDAppClientState.Connecting;

			// Request TURN.
			var turnRequestURL = new Uri(kARDTurnRequestUrl);
			var turns = await RequestTURNServersWithURLAsync(turnRequestURL);
			for (int i = (int)turns.Count - 1; i >= 0; i--)
				_iceServers.Add(turns.GetItem<NSObject>((nuint)i));
			_isTurnComplete = true;
			// Register with room server.
			var response = await RegisterWithRoomServerForRoomId(roomName);
			if (response == null || response.result != ARDRegisterResultType.SUCCESS)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to register with room server. Result: {response.result}");
				Disconnect();
				NSError error = RoomServerNetworkError(kARDAppClientErrorRoomFull);
				ARDAppClientDelegate.DidError(this, error);
				return;
			}
			System.Diagnostics.Debug.WriteLine("Registered with room server.");

			_isInitiator = response.@params.is_initiator;
			_clientId = response.@params.client_id;
			_roomId = response.@params.room_id;
			_webSocketURL = response.@params.wss_url;
			_webSocketRestURL = response.@params.wss_post_url;

			foreach (var msg in response.@params.messages)
			{

				//if (message.type == kARDSignalingMessageTypeOffer ||
				//         message.type == kARDSignalingMessageTypeAnswer) {
				//       strongSelf.hasReceivedSdp = YES;
				//       [strongSelf.messageQueue insertObject:message atIndex:0];
				//     } else {
				//       [strongSelf.messageQueue addObject:message];
				//     }

			}

			RegisterWithColliderIfReady();
			StartSignalingIfReady();
		}

		public async void Disconnect()
		{
			if (_state == ARDAppClientState.Disconnected)
				return;
			if (_isRegisteredWithRoomServer)
				await UnregisterWithRoomServerAsync();


			if (_channel != null)
			{
				//  if (_channel.state == kARDWebSocketChannelStateRegistered) {
				//    // Tell the other client we're hanging up.
				//    ARDByeMessage *byeMessage = [[ARDByeMessage alloc] init];
				//    NSData *byeData = [byeMessage JSONData];
				//    [_channel sendData:byeData];
				//  }
				//  // Disconnect from collider.
				//  _channel = nil;
			}
			_clientId = null;
			_roomId = null;
			_isInitiator = false;
			_hasReceivedSdp = false;
			_messageQueue = new List<ARDSignalingMessage>();
			_peerConnection = null;
			_state = ARDAppClientState.Disconnected;
		}



		public void MuteAudioIn()
		{
			System.Diagnostics.Debug.WriteLine("audio muted");
			RTCMediaStream localStream = _peerConnection.LocalStreams[0];
			defaultAudioTrack = localStream.AudioTracks[0];
			localStream.RemoveAudioTrack(defaultAudioTrack);
			_peerConnection.RemoveStream(localStream);
			_peerConnection.AddStream(localStream);
		}

		public void SetServerHostUrl(string serverHostUrl)
		{
			_serverHostUrl = serverHostUrl;
		}

		public void SwapCameraToBack()
		{
			//throw new NotImplementedException();
		}

		public void SwapCameraToFront()
		{
			//throw new NotImplementedException();
		}

		public void UnmuteAudioIn()
		{
			System.Diagnostics.Debug.WriteLine("audio unmuted");
			RTCMediaStream localStream = _peerConnection.LocalStreams[0];
			localStream.AddAudioTrack(defaultAudioTrack);
			_peerConnection.RemoveStream(localStream);
			_peerConnection.AddStream(localStream);
			if (_isSpeakerEnabled)
				EnableSpeaker();
		}

		void SetState(ARDAppClientState state)
		{
			if (_state == state)
			{
				return;
			}
			_state = state;

			ARDAppClientDelegate.DidChangeState(this, state);
		}

		RTCVideoTrack CreateLocalVideoTrack()
		{
			// The iOS simulator doesn't provide any sort of camera capture
			// support or emulation (http://goo.gl/rHAnC1) so don't bother
			// trying to open a local stream.
			// TODO(tkchin): local video capture for OSX. See
			// https://code.google.com/p/webrtc/issues/detail?id=3417.

			RTCVideoTrack localVideoTrack = null;
#if !TARGET_IPHONE_SIMULATOR && TARGET_OS_IPHONE

    //NSString *cameraID = nil;
    //for (AVCaptureDevice *captureDevice in
    //     [AVCaptureDevice devicesWithMediaType:AVMediaTypeVideo]) {
    //    if (captureDevice.position == AVCaptureDevicePositionFront) {
    //        cameraID = [captureDevice localizedName];
    //        break;
    //    }
    //}
    //NSAssert(cameraID, @"Unable to get the front camera id");
    
    //RTCVideoCapturer *capturer = [RTCVideoCapturer capturerWithDeviceName:cameraID];
    //RTCMediaConstraints *mediaConstraints = [self defaultMediaStreamConstraints];
    //RTCVideoSource *videoSource = [_factory videoSourceWithCapturer:capturer constraints:mediaConstraints];
    //localVideoTrack = [_factory videoTrackWithID:@"ARDAMSv0" source:videoSource];
#endif
			return localVideoTrack;
		}

		RTCMediaStream CreateLocalMediaStream()
		{
			RTCMediaStream localStream = _factory.MediaStreamWithLabel("ARDAMS");

			RTCVideoTrack localVideoTrack = CreateLocalVideoTrack();

			if (localVideoTrack != null)
			{
				localStream.AddVideoTrack(localVideoTrack);
				ARDAppClientDelegate.DidReceiveLocalVideoTrack(this, localVideoTrack);
				//        [_delegate appClient:self didReceiveLocalVideoTrack:localVideoTrack];
			}

			localStream.AddAudioTrack(_factory.AudioTrackWithID("ARDAMSa0"));
			if (_isSpeakerEnabled)
				EnableSpeaker();

			return localStream;
		}

		void EnableSpeaker()
		{
			NSError error;
			AVAudioSession.SharedInstance().OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out error);
			_isSpeakerEnabled = true;
		}

		void DisableSpeaker()
		{
			NSError error;
			AVAudioSession.SharedInstance().OverrideOutputAudioPort(AVAudioSessionPortOverride.None, out error);
			_isSpeakerEnabled = false;
		}

		bool IsRegisteredWithRoomServer()
		{
			return _clientId.Length > 0;
		}

		void StartSignalingIfReady()
		{
			if (!_isTurnComplete || !_isRegisteredWithRoomServer)
				return;

			SetState(ARDAppClientState.Connected);

			RTCMediaConstraints constrains = DefaultPeerConnectionConstraints();

			_peerConnection = _factory.PeerConnectionWithICEServers(_iceServers, constrains, this);

			RTCMediaStream localStream = CreateLocalMediaStream();
			_peerConnection.AddStream(localStream);
			if (_isInitiator)
				SendOffer();
			else
				WaitForAnswer();

		}

		async Task SendSignalingMessage(ARDSignalingMessage message)
		{
			if (_isInitiator)
				await SendSignalingMessageToRoomServer(message);
			else
				SendSignalingMessageToCollider(message);
		}

		async Task SendSignalingMessageToRoomServer(ARDSignalingMessage message, Action<ARDMessageResponse> completionHandler = null)
		{
			var jsonData = message.JsonData;
			string urlString = string.Format(kARDRoomServerMessageFormat, _serverHostUrl, _roomId, _clientId);
			System.Diagnostics.Debug.WriteLine($"C->RS POST: {jsonData}");
			var client = new HttpClient { BaseAddress = new Uri(urlString) };
			ARDMessageResponse responseMessage = null;
			NSError error = null;
			try
			{
				var response = await client.PostAsync("", new StringContent(jsonData));

				if (!response.IsSuccessStatusCode)
				{
					error = RoomServerNetworkError(1);
					_delegate.DidError(this, error);
					return;
				}
				var responseContent = await response.Content.ReadAsStringAsync();
				responseMessage = JsonConvert.DeserializeObject<ARDMessageResponse>(responseContent);
				switch (responseMessage.Type)
				{
					case ARDMessageResultType.Success:
						break;
					case ARDMessageResultType.Unknown:
						error = RoomServerNetworkError(kARDAppClientErrorUnknown);
						break;
					case ARDMessageResultType.InvalidClient:
						error = RoomServerNetworkError(kARDAppClientErrorInvalidClient);
						break;
					case ARDMessageResultType.InvalidRoom:
						error = RoomServerNetworkError(kARDAppClientErrorInvalidRoom);
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				error = RoomServerNetworkError(kARDAppClientErrorUnknown, ex.Message);
			}
			if (error != null)
			{
				_delegate.DidError(this, error);

			}
			completionHandler?.Invoke(responseMessage);

		}

		void RegisterWithColliderIfReady()
		{
			if (!_isRegisteredWithRoomServer)
				return;
			// Open WebSocket connection.
			_channel = new ARDWebSocketChannel(_webSocketURL, _webSocketRestURL, this);
			_channel.RegisterForRoomId(_roomId, _clientId);
		}

		void SendSignalingMessageToCollider(ARDSignalingMessage message)
		{
			NSData data = null;
			_channel.SendData(data);
			//NSData* data = [message JSONData];
		}

		void SendOffer()
		{
			_peerConnection.CreateOfferWithDelegate(this, DefaultOfferConstraints());
		}

		void WaitForAnswer()
		{
			DrainMessageQueueIfReady();
		}

		void DrainMessageQueueIfReady()
		{
			if (_peerConnection == null || !_hasReceivedSdp)
			{
				return;
			}

			foreach (var messange in _messageQueue)
			{
				ProcessSignalingMessage(messange);
			}
			_messageQueue.Clear();
		}

		async Task<NSArray> RequestTURNServersWithURLAsync(Uri requestURL)
		{
			var client = new HttpClient { BaseAddress = requestURL };
			client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
			client.DefaultRequestHeaders.Add("origin", _serverHostUrl);

			NSArray turnServers = new NSArray();
			try
			{
				var result = await client.GetStringAsync("");
				var response = JsonConvert.DeserializeObject<TurnResponse>(result);
				var items = new List<RTCICEServer>();
				foreach (var item in response.uris)
				{
					items.Add(new RTCICEServer(new NSUrl(item), response.username, response.password));
				}
				turnServers = NSArray.FromObjects(items.ToArray());
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Unable to get TURN server.");
			}

			return turnServers;
		}

		async Task UnregisterWithRoomServerAsync()
		{
			var urlString = string.Format(kARDRoomServerByeFormat, _serverHostUrl);
			var url = new Uri(urlString);
			var client = new HttpClient();
			var result = await client.PostAsync(url, null);
			if (result.IsSuccessStatusCode)
			{
				System.Diagnostics.Debug.WriteLine("Unregistered from room server.");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Failed to unregister from room server.");
			}
		}

		async Task<ARDRegisterResponse> RegisterWithRoomServerForRoomId(string roomId)
		{
			var urlString = string.Format(kARDRoomServerRegisterFormat, _serverHostUrl, roomId);
			var roomURL = new Uri(urlString);

			var client = new HttpClient { BaseAddress = roomURL };
			client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
			client.DefaultRequestHeaders.Add("origin", _serverHostUrl);

			var response = await client.PostAsync("", null);
			ARDRegisterResponse registerResponse = null;
			if (response.IsSuccessStatusCode)
			{
				System.Diagnostics.Debug.WriteLine("roomURL with room server.");
				var result = await response.Content.ReadAsStringAsync();
				registerResponse = JsonConvert.DeserializeObject<ARDRegisterResponse>(result);
			}
			else
			{
				NSError error = RoomServerNetworkError(kARDAppClientErrorNetwork);
				ARDAppClientDelegate.DidError(this, error);
			}

			return registerResponse;
		}

		#region Defaults
		RTCMediaConstraints DefaultMediaStreamConstraints()
		{
			return new RTCMediaConstraints(null, null);
		}

		RTCMediaConstraints DefaultAnswerConstraints()
		{
			return DefaultOfferConstraints();
		}

		RTCMediaConstraints DefaultOfferConstraints()
		{
			NSArray mandatoryConstraints = NSArray.FromNSObjects(new RTCPair("OfferToReceiveAudio", "true"), new RTCPair("OfferToReceiveVideo", "true"));
			return new RTCMediaConstraints(mandatoryConstraints, null);
		}

		RTCMediaConstraints DefaultPeerConnectionConstraints()
		{
			NSArray optionalConstraints = NSArray.FromNSObjects(new RTCPair("DtlsSrtpKeyAgreement", "true"));
			return new RTCMediaConstraints(null, optionalConstraints);
		}

		RTCICEServer DefaultSTUNServer()
		{
			var defaultSTUNServerURL = new NSUrl(kARDDefaultSTUNServerUrl);
			return new RTCICEServer(defaultSTUNServerURL, "", "");
		}

		NSError RoomServerNetworkError(int errorCode, string info = "")
		{
			NSError error = new NSError(new NSString(kARDAppClientErrorDomain), errorCode, new NSDictionary());
			//                           userInfo:@{  NSLocalizedDescriptionKey: @"Room server network error",
			return error;
		}
		#endregion

		public void DidChangeState(ARDWebSocketChannelState state)
		{
			switch (state)
			{
				case ARDWebSocketChannelState.kARDWebSocketChannelStateOpen:
					break;
				case ARDWebSocketChannelState.kARDWebSocketChannelStateRegistered:
					break;
				case ARDWebSocketChannelState.kARDWebSocketChannelStateClosed:
				case ARDWebSocketChannelState.kARDWebSocketChannelStateError:
					// TODO(tkchin): reconnection scenarios. Right now we just disconnect
					// completely if the websocket connection fails.
					Disconnect();
					break;
			}
		}

		public void DidReceiveMessage(ARDSignalingMessage message)
		{
			switch (message.Type)
			{
				case ARDSignalingMessageType.Offer:
				case ARDSignalingMessageType.Answer:
					_hasReceivedSdp = true;
					_messageQueue.Insert(0, message);
					break;
				case ARDSignalingMessageType.Candidate:
					_messageQueue.Add(message);
					break;
				case ARDSignalingMessageType.Bye:
					ProcessSignalingMessage(message);
					break;
				default:
					break;
			}
			DrainMessageQueueIfReady();
		}

		void ProcessSignalingMessage(ARDSignalingMessage message)
		{

			switch (message.Type)
			{
				case ARDSignalingMessageType.Offer:
				case ARDSignalingMessageType.Answer:
					ARDSessionDescriptionMessage sdpMessage = message as ARDSessionDescriptionMessage;
					_peerConnection.SetRemoteDescriptionWithDelegate(this, sdpMessage.Description);
					break;
				case ARDSignalingMessageType.Candidate:
					ARDICECandidateMessage candidateMessage = message as ARDICECandidateMessage;
					_peerConnection.AddICECandidate(candidateMessage.Candidate);
					break;
				case ARDSignalingMessageType.Bye:
					// Other client disconnected.
					// TODO(tkchin): support waiting in room for next client. For now just
					Disconnect();
					break;
				default:
					break;
			}
		}

		public void PeerConnection(RTCPeerConnection peerConnection, RTCSignalingState stateChanged)
		{
			System.Diagnostics.Debug.WriteLine($"Signaling state changed: {stateChanged}");
		}

		public void PeerConnectionAdded(RTCPeerConnection peerConnection, RTCMediaStream stream)
		{
			//	  dispatch_async(dispatch_get_main_queue(), ^{
			//  NSLog(@"Received %lu video tracks and %lu audio tracks",
			//      (unsigned long)stream.videoTracks.count,
			//      (unsigned long)stream.audioTracks.count);
			//  if (stream.videoTracks.count) {
			//    RTCVideoTrack *videoTrack = stream.videoTracks[0];
			//    [_delegate appClient:self didReceiveRemoteVideoTrack:videoTrack];
			//    if (_isSpeakerEnabled) [self enableSpeaker]; //Use the "handsfree" speaker instead of the ear speaker.

			//  }
			//});

			System.Diagnostics.Debug.WriteLine($"Received {stream.VideoTracks.Count()} video tracks and {stream.AudioTracks.Count()} audio tracks");
			if (stream.VideoTracks.Count() > 0)
			{
				_delegate.DidReceiveRemoteVideoTrack(this, stream.VideoTracks[0]);
				if (_isSpeakerEnabled)
					EnableSpeaker();
			}

		}

		public void PeerConnectionRemoved(RTCPeerConnection peerConnection, RTCMediaStream stream)
		{
			System.Diagnostics.Debug.WriteLine("Stream was removed.");
		}

		public void PeerConnectionOnRenegotiationNeeded(RTCPeerConnection peerConnection)
		{
			System.Diagnostics.Debug.WriteLine("WARNING: Renegotiation needed but unimplemented.");
		}

		public void PeerConnection(RTCPeerConnection peerConnection, RTCICEConnectionState newState)
		{
			System.Diagnostics.Debug.WriteLine($"ICE state changed: {newState}");
		}

		public void PeerConnection(RTCPeerConnection peerConnection, RTCICEGatheringState newState)
		{
			System.Diagnostics.Debug.WriteLine($"ICE gathering changed: {newState}");
		}

		public void PeerConnection(RTCPeerConnection peerConnection, RTCICECandidate candidate)
		{
			ARDICECandidateMessage message = new ARDICECandidateMessage(candidate);
			SendSignalingMessage(message).Wait();
		}


		public void PeerConnection(RTCPeerConnection peerConnection, RTCDataChannel dataChannel)
		{
			System.Diagnostics.Debug.WriteLine($"Opened data channel: {dataChannel}");
		}

		public void DidCreateSessionDescription(RTCPeerConnection peerConnection, RTCSessionDescription sdp, NSError error)
		{
			// dispatch_async(dispatch_get_main_queue(), 
			if (error != null)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to create session description. Error: {error}");
				Disconnect();
				//    NSDictionary *userInfo = @{
				//      NSLocalizedDescriptionKey: @"Failed to create session description.",
				//    };
				//    NSError *sdpError =
				//        [[NSError alloc] initWithDomain:kARDAppClientErrorDomain
				//                                   code:kARDAppClientErrorCreateSDP
				//                               userInfo:userInfo];
				_delegate.DidError(this, error);
				return;
			}
			_peerConnection.SetLocalDescriptionWithDelegate(this, sdp);
			ARDSessionDescriptionMessage message = new ARDSessionDescriptionMessage(sdp);
			SendSignalingMessage(message).Wait();
		}


		public void DidSetSessionDescriptionWithError(RTCPeerConnection peerConnection, NSError error)
		{
			// dispatch_async(dispatch_get_main_queue(), 
			if (error != null)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to set session description. Error: {error}");
				Disconnect();
				//    NSDictionary *userInfo = @{
				//      NSLocalizedDescriptionKey: @"Failed to create session description.",
				//    };
				//    NSError *sdpError =
				//        [[NSError alloc] initWithDomain:kARDAppClientErrorDomain
				//                                   code:kARDAppClientErrorCreateSDP
				//                               userInfo:userInfo];
				_delegate.DidError(this, error);
				return;
			}
			if (!_isInitiator && _peerConnection.LocalDescription == null)
			{
				RTCMediaConstraints constraints = DefaultAnswerConstraints();
				_peerConnection.CreateAnswerWithDelegate(this, constraints);
			}

		}
	}
}
