using System;
using System.Collections.Generic;
using WebRTCBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppRTC
{

	public static class ARDSignalingMessageExtensions
	{
		public static ARDSignalingMessage MessageFromJSONString(string json)
		{
			var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			ARDSignalingMessage message = new ARDSignalingMessage();
			var type = values["type"];
			if (type == "candidate")
			{
				RTCICECandidate candidate = new RTCICECandidate(values["id"], nint.Parse(values["label"]), values["candidate"]);
				message = new ARDICECandidateMessage(candidate);
			}
			else if (type == "offer" || type == "answer")
			{
				RTCSessionDescription description = new RTCSessionDescription(type, values["sdp"]);
				message = new ARDSessionDescriptionMessage(description);
			}
			else if (type == "bye")
			{
				message = new ARDByeMessage();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Unexpected type: {type}");
			}
			return message;
		}

		public static string AsJSON(this RTCSessionDescription rtcSessionDescription)
		{
			dynamic jsonObject = new JObject();
			jsonObject.type = rtcSessionDescription.Type;
			jsonObject.sdp = rtcSessionDescription.Description;
			return JsonConvert.SerializeObject(jsonObject);
		}

		public static string AsJSON(this RTCICECandidate rtcICEcandidate)
		{
			dynamic jsonObject = new JObject();
			try
			{
				jsonObject.type = "candidate";
				jsonObject.id = (int)rtcICEcandidate.SdpMLineIndex;
				jsonObject.label = rtcICEcandidate.SdpMid;
				jsonObject.candidate = rtcICEcandidate.Sdp;
			}
			catch (Exception ex)
			{

			}

			return JsonConvert.SerializeObject(jsonObject);
		}

	}

	public enum ARDSignalingMessageType
	{
		Candidate,
		Offer,
		Answer,
		Bye,
	}

	public class ARDSignalingMessage
	{

		public ARDSignalingMessageType Type
		{
			get;
			set;
		}

		public virtual string JsonData
		{
			get;
		}
	}

	public class ARDICECandidateMessage : ARDSignalingMessage
	{
		public ARDICECandidateMessage(RTCICECandidate candidate)
		{
			Type = ARDSignalingMessageType.Candidate;
			Candidate = candidate;
		}

		public RTCICECandidate Candidate
		{
			get;
			set;
		}

		public override string JsonData => Candidate.AsJSON();
	}

	public class ARDSessionDescriptionMessage : ARDSignalingMessage
	{
		public ARDSessionDescriptionMessage(RTCSessionDescription description)
		{
			Description = description;
			if (Description.Type.Equals("offer", StringComparison.Ordinal))
			{
				Type = ARDSignalingMessageType.Offer;
			}
			else if (Description.Type.Equals("answer", StringComparison.Ordinal))
			{
				Type = ARDSignalingMessageType.Answer;
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Unexpected type: {Type}");
			}
		}

		public RTCSessionDescription Description
		{
			get;
			set;
		}

		public override string JsonData => Description.AsJSON();
	}

	public class ARDByeMessage : ARDSignalingMessage
	{

		public ARDByeMessage()
		{

		}
		public override string JsonData
		{
			get
			{
				dynamic jsonObject = new JObject();
				jsonObject.type = "bye";
				return JsonConvert.SerializeObject(jsonObject);
			}
		}
	}
}
