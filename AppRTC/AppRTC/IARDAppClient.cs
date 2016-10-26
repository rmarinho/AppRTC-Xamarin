using System;
using Foundation;
using WebRTCBinding;

namespace AppRTC
{

	public enum ARDAppClientState : long
	{
		Disconnected,
		Connecting,
		Connected
	}
	public interface IARDAppClient
	{
		void SetServerHostUrl(string serverHostUrl);
		void ConnectToRoomWithId(string roomName);
		void Disconnect();
		void SwapCameraToBack();
		void SwapCameraToFront();
		void MuteAudioIn();
		void UnmuteAudioIn();
		IARDAppClientDelegate ARDAppClientDelegate { get; }
	}

	public interface IARDAppClientDelegate
	{
		void DidChangeState(IARDAppClient client, ARDAppClientState state);
		void DidReceiveLocalVideoTrack(IARDAppClient client, RTCVideoTrack localVideoTrack);
		void DidReceiveRemoteVideoTrack(IARDAppClient client, RTCVideoTrack remoteVideoTrack);
		void DidError(IARDAppClient client, NSError error);
	}
}
