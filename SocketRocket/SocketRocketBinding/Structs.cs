using System;
using ObjCRuntime;


namespace SocketRocketBinding
{
	[Native]
	public enum SRReadyState : ulong
	{
		Connecting = 0,
		Open = 1,
		Closing = 2,
		Closed = 3
	}

	[Native]
	public enum SRStatusCode : uint
	{
		CodeNormal = 1000,
		CodeGoingAway = 1001,
		CodeProtocolError = 1002,
		CodeUnhandledType = 1003,
		NoStatusReceived = 1005,
		CodeAbnormal = 1006,
		CodeInvalidUTF8 = 1007,
		CodePolicyViolated = 1008,
		CodeMessageTooBig = 1009,
		CodeMissingExtension = 1010,
		CodeInternalError = 1011,
		CodeServiceRestart = 1012,
		CodeTryAgainLater = 1013,
		CodeTLSHandshake = 1015
	}
}
