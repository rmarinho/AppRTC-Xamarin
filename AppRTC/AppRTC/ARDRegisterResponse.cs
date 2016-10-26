using System;
using System.Collections.Generic;

namespace AppRTC
{
	class ARDRegisterResponse
	{
		public Params @params { get; set; }
		public ARDRegisterResultType result { get; set; }
	}

	public class Params
	{
		public bool is_initiator { get; set; }
		public string turn_url { get; set; }
		public string room_link { get; set; }
		public bool is_loopback { get; set; }
		public string offer_options { get; set; }
		public IList<object> messages { get; set; }
		public string version_info { get; set; }
		public string pc_constraints { get; set; }
		public IList<object> error_messages { get; set; }
		public string include_loopback_js { get; set; }
		public string ice_server_url { get; set; }
		public IList<object> warning_messages { get; set; }
		public string room_id { get; set; }
		public string callstats_params { get; set; }
		public string ice_server_transports { get; set; }
		public string client_id { get; set; }
		public string bypass_join_confirmation { get; set; }
		public string wss_url { get; set; }
		public string wss_post_url { get; set; }
		public string media_constraints { get; set; }
		public string pc_config { get; set; }
	}

	public class PcConfig
	{
		public string rtcpMuxPolicy { get; set; }
		public string bundlePolicy { get; set; }
		public IList<string> iceServers { get; set; }
	}

	public class CallstatsParams
	{
		public string appSecret { get; set; }
		public string appId { get; set; }
	}

	public class VersionInfo
	{
		public string gitHash { get; set; }
		public string branch { get; set; }
		public DateTime time { get; set; }
	}

	enum ARDRegisterResultType
	{
		UNKNOWN = 0,
		SUCCESS = 1,
		FULL = 2
	}
}