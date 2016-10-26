using System;
using Foundation;
using Newtonsoft.Json;
using SocketRocketBinding;

namespace AppRTC
{
	public enum ARDWebSocketChannelState
	{
		kARDWebSocketChannelStateClosed,
		// State when connection is established but not ready for use.
		kARDWebSocketChannelStateOpen,
		// State when connection is established and registered.
		kARDWebSocketChannelStateRegistered,
		// State when connection encounters a fatal error.
		kARDWebSocketChannelStateError
	}

	public class ARDWebSocketChannel : SRWebSocketDelegate // : ISRWebSocketDelegate
	{
		IARDWebSocketChannelDelegate _delegate;
		string _webSocketUrl;
		string _webSockerRestUrl;
		string _roomId;
		string _clientId;
		SRWebSocket _socket;
		ARDWebSocketChannelState _state;


		//didReceiveMessage
		public override void WebSocket(SRWebSocket webSocket, NSObject message)
		{

			SocketResponse socketResponse;
			try
			{
				socketResponse = JsonConvert.DeserializeObject<SocketResponse>(message.ToString());
				if (!string.IsNullOrEmpty(socketResponse.error))
				{
					System.Diagnostics.Debug.WriteLine($"WSS error: {socketResponse.error}");
					return;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Invalid json  error: {ex.Message}");
				return;
			}

			System.Diagnostics.Debug.WriteLine(@"WSS->C: %@", socketResponse.msg);
			ARDSignalingMessage signalingMessage = ARDSignalingMessageExtensions.MessageFromJSONString(socketResponse.msg);
			_delegate.DidReceiveMessage(signalingMessage);
		}

		public override void WebSocketDidOpen(SRWebSocket webSocket)
		{
			System.Diagnostics.Debug.WriteLine(@"WebSocket connection opened.");
			SetState(ARDWebSocketChannelState.kARDWebSocketChannelStateOpen);
			if (!string.IsNullOrEmpty(_roomId) && !string.IsNullOrEmpty(_clientId))
				RegisterWithCollider(_roomId, _clientId);
		}


		public ARDWebSocketChannel(string webSocketUrl, string webSockerRestUrl, IARDWebSocketChannelDelegate @delegate)
		{
			_webSocketUrl = webSocketUrl;
			_webSockerRestUrl = webSockerRestUrl;
			_delegate = @delegate;
			_socket = new SRWebSocket(new NSUrl(_webSocketUrl));
			_socket.Delegate = this;
			System.Diagnostics.Debug.WriteLine("Opening WebSocket.");
			_socket.Open();
		}

		public void RegisterForRoomId(string roomId, string cliendId)
		{
			_roomId = roomId;
			_clientId = cliendId;
			if (_state == ARDWebSocketChannelState.kARDWebSocketChannelStateOpen)
				RegisterWithCollider(_roomId, _clientId);
		}

		public void SendData(object data)
		{
			if (_state == ARDWebSocketChannelState.kARDWebSocketChannelStateRegistered)
			{
				//  NSString *payload =
				//      [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
				//  NSDictionary *message = @{
				//    @"cmd": @"send",
				//    @"msg": payload,
				//  };
				//  NSData *messageJSONObject =
				//      [NSJSONSerialization dataWithJSONObject:message
				//                                      options:NSJSONWritingPrettyPrinted
				//                                        error:nil];
				//  NSString *messageString =
				//      [[NSString alloc] initWithData:messageJSONObject
				//                            encoding:NSUTF8StringEncoding];
				//  NSLog(@"C->WSS: %@", messageString);
				//  [_socket send:messageString];
				//} else {
				//  NSString *dataString =
				//      [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
				//  NSLog(@"C->WSS POST: %@", dataString);
				//  NSString *urlString =
				//      [NSString stringWithFormat:@"%@/%@/%@",
				//          [_restURL absoluteString], _roomId, _clientId];
				//  NSURL *url = [NSURL URLWithString:urlString];
				//  [NSURLConnection sendAsyncPostToURL:url
				//                             withData:data
				//                    completionHandler:nil];
			}
		}

		void SetState(ARDWebSocketChannelState state)
		{
			if (_state == state)
			{
				return;
			}
			_state = state;
			_delegate.DidChangeState(state);
		}

		void RegisterWithCollider(string roomId, string clientId)
		{
			if (_state == ARDWebSocketChannelState.kARDWebSocketChannelStateRegistered)
				return;
			NSDictionary registerMessage = new NSDictionary("cmd", "register", "roomid", roomId, "clientid", clientId);
			NSError error = null;
			NSData message = NSJsonSerialization.Serialize(registerMessage, NSJsonWritingOptions.PrettyPrinted, out error);
			NSString messageString = new NSString(message, NSStringEncoding.UTF8);
			System.Diagnostics.Debug.WriteLine($"Registering on WSS for rid:{roomId} cid:{clientId}");
			//// Registration can fail if server rejects it. For example, if the room is
			//// full.
			_socket.Send(messageString);
			SetState(ARDWebSocketChannelState.kARDWebSocketChannelStateRegistered);
		}

	}
}