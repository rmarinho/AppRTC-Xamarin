using System;
namespace AppRTC
{
	public enum ARDMessageResultType
	{
		Unknown,
		Success,
		InvalidRoom,
		InvalidClient
	};

	public class ARDMessageResponse
	{
		string _result;
		public string result
		{
			get { return _result; }
			set { _result = value; Type = ARDMessageResultTypeFromString(_result); }
		}

		public ARDMessageResultType Type { get; set; }

		public static ARDMessageResultType ARDMessageResultTypeFromString(string resultString)
		{
			ARDMessageResultType result = ARDMessageResultType.Unknown;
			if (resultString.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
			{
				result = ARDMessageResultType.Success;
			}
			else if (resultString.Equals("INVALID_CLIENT", StringComparison.OrdinalIgnoreCase))
			{
				result = ARDMessageResultType.InvalidClient;
			}
			else if (resultString.Equals("INVALID_ROOM", StringComparison.OrdinalIgnoreCase))
			{
				result = ARDMessageResultType.InvalidRoom;
			}
			return result;
		}

	}
}
