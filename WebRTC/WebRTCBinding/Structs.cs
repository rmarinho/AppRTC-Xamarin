using System;
using ObjCRuntime;

namespace WebRTCBinding
{

	public enum RTCICEConnectionState : uint
	{
		New,
		Checking,
		Connected,
		Completed,
		Failed,
		Disconnected,
		Closed,
		Max
	}
	
	public enum RTCStatsOutputLevel : uint
	{
		Standard,
		Debug
	}
	
	public enum RTCSourceState : uint
	{
		Initializing,
		Live,
		Ended,
		Muted
	}
	
	public enum RTCTrackState : uint
	{
		Initializing,
		Live,
		Ended,
		Failed
	}
	
	
	public enum RTCICEGatheringState : uint
	{
		New,
		Gathering,
		Complete
	}
	
	[Native]
	public enum SRReadyState : long
	{
		Connecting = 0,
		Open = 1,
		Closing = 2,
		Closed = 3
	}
	
	[Native]
	public enum SRStatusCode : long
	{
		CodeNormal = 1000,
		CodeGoingAway = 1001,
		CodeProtocolError = 1002,
		CodeUnhandledType = 1003,
		NoStatusReceived = 1005,
		CodeInvalidUTF8 = 1007,
		CodePolicyViolated = 1008,
		CodeMessageTooBig = 1009
	}
	
	public enum RTCDataChannelState : uint
	{
		Connecting,
		Open,
		Closing,
		Closed
	}
	
	public enum RTCSignalingState : uint
	{
		Stable,
		HaveLocalOffer,
		HaveLocalPrAnswer,
		HaveRemoteOffer,
		HaveRemotePrAnswer,
		Closed
	}
	
	
	[Native]
	public enum RTCIceTransportsType : uint	
	{
		None,
		Relay,
		NoHost,
		All
	}
	
	[Native]
	public enum RTCBundlePolicy : uint
	{
		Balanced,
		MaxBundle,
		MaxCompat
	}
	
	[Native]
	public enum RTCRtcpMuxPolicy : uint
	{
		Negotiate,
		Require
	}
	
	[Native]
	public enum RTCTcpCandidatePolicy : uint
	{
		Enabled,
		Disabled
	}
}
