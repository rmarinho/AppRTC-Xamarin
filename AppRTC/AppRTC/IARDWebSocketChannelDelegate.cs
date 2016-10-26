using System;

namespace AppRTC
{

	public interface IARDWebSocketChannelDelegate
	{
		void DidChangeState(ARDWebSocketChannelState state);
		void DidReceiveMessage(ARDSignalingMessage message);
	}
}