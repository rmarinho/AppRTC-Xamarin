using System;

using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

//sharpie bind --namespace=JinglePeerconnection ../libjingle_peerconnection/Pods/libjingle_peerconnection/libjingle_peerconnection/Headers/*.h

namespace WebRTCBinding
{


	// @interface RTCPeerConnection : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCPeerConnection
	{
		[Wrap("WeakDelegate")]
		RTCPeerConnectionDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<RTCPeerConnectionDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// @property (readonly, nonatomic, strong) NSArray * localStreams;
		[Export("localStreams", ArgumentSemantic.Strong)]
		//	[Verify(StronglyTypedNSArray)]
		RTCMediaStream[] LocalStreams { get; }

		// @property (readonly, assign, nonatomic) RTCSessionDescription * localDescription;
		[Export("localDescription", ArgumentSemantic.Assign)]
		RTCSessionDescription LocalDescription { get; }

		// @property (readonly, assign, nonatomic) RTCSessionDescription * remoteDescription;
		[Export("remoteDescription", ArgumentSemantic.Assign)]
		RTCSessionDescription RemoteDescription { get; }

		// @property (readonly, assign, nonatomic) RTCSignalingState signalingState;
		[Export("signalingState", ArgumentSemantic.Assign)]
		RTCSignalingState SignalingState { get; }

		// @property (readonly, assign, nonatomic) RTCICEConnectionState iceConnectionState;
		[Export("iceConnectionState", ArgumentSemantic.Assign)]
		RTCICEConnectionState IceConnectionState { get; }

		// @property (readonly, assign, nonatomic) RTCICEGatheringState iceGatheringState;
		[Export("iceGatheringState", ArgumentSemantic.Assign)]
		RTCICEGatheringState IceGatheringState { get; }

		// -(BOOL)addStream:(RTCMediaStream *)stream;
		[Export("addStream:")]
		bool AddStream(RTCMediaStream stream);

		// -(void)removeStream:(RTCMediaStream *)stream;
		[Export("removeStream:")]
		void RemoveStream(RTCMediaStream stream);

		// -(RTCDataChannel *)createDataChannelWithLabel:(NSString *)label config:(RTCDataChannelInit *)config;
		[Export("createDataChannelWithLabel:config:")]
		RTCDataChannel CreateDataChannelWithLabel(string label, RTCDataChannelInit config);

		// -(void)createOfferWithDelegate:(id<RTCSessionDescriptionDelegate>)delegate constraints:(RTCMediaConstraints *)constraints;
		[Export("createOfferWithDelegate:constraints:")]
		void CreateOfferWithDelegate(IRTCSessionDescriptionDelegate @delegate, RTCMediaConstraints constraints);

		// -(void)createAnswerWithDelegate:(id<RTCSessionDescriptionDelegate>)delegate constraints:(RTCMediaConstraints *)constraints;
		[Export("createAnswerWithDelegate:constraints:")]
		void CreateAnswerWithDelegate(IRTCSessionDescriptionDelegate @delegate, RTCMediaConstraints constraints);

		// -(void)setLocalDescriptionWithDelegate:(id<RTCSessionDescriptionDelegate>)delegate sessionDescription:(RTCSessionDescription *)sdp;
		[Export("setLocalDescriptionWithDelegate:sessionDescription:")]
		void SetLocalDescriptionWithDelegate(IRTCSessionDescriptionDelegate @delegate, RTCSessionDescription sdp);

		// -(void)setRemoteDescriptionWithDelegate:(id<RTCSessionDescriptionDelegate>)delegate sessionDescription:(RTCSessionDescription *)sdp;
		[Export("setRemoteDescriptionWithDelegate:sessionDescription:")]
		void SetRemoteDescriptionWithDelegate(IRTCSessionDescriptionDelegate @delegate, RTCSessionDescription sdp);

		// -(BOOL)setConfiguration:(RTCConfiguration *)configuration;
		[Export("setConfiguration:")]
		bool SetConfiguration(RTCConfiguration configuration);

		// -(BOOL)addICECandidate:(RTCICECandidate *)candidate;
		[Export("addICECandidate:")]
		bool AddICECandidate(RTCICECandidate candidate);

		// -(void)close;
		[Export("close")]
		void Close();

		// -(BOOL)getStatsWithDelegate:(id<RTCStatsDelegate>)delegate mediaStreamTrack:(RTCMediaStreamTrack *)mediaStreamTrack statsOutputLevel:(RTCStatsOutputLevel)statsOutputLevel;
		[Export("getStatsWithDelegate:mediaStreamTrack:statsOutputLevel:")]
		bool GetStatsWithDelegate(RTCStatsDelegate @delegate, RTCMediaStreamTrack mediaStreamTrack, RTCStatsOutputLevel statsOutputLevel);
	}

	// @interface RTCMediaConstraints : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCMediaConstraints
	{
		// -(id)initWithMandatoryConstraints:(NSArray *)mandatory optionalConstraints:(NSArray *)optional;
		[Export("initWithMandatoryConstraints:optionalConstraints:")]
		//	[Verify(StronglyTypedNSArray), Verify(StronglyTypedNSArray)]
		IntPtr Constructor([NullAllowed]NSArray mandatory, [NullAllowed]NSArray optional);
	}
	// @protocol RTCMediaStreamTrackDelegate <NSObject>
	[Model]
	[BaseType(typeof(NSObject))]
	interface RTCMediaStreamTrackDelegate
	{
		// @required -(void)mediaStreamTrackDidChange:(RTCMediaStreamTrack *)mediaStreamTrack;
		[Abstract]
		[Export("mediaStreamTrackDidChange:")]
		void MediaStreamTrackDidChange(RTCMediaStreamTrack mediaStreamTrack);
	}

	// @interface RTCMediaStreamTrack : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCMediaStreamTrack
	{
		// @property (readonly, nonatomic) NSString * kind;
		[Export("kind")]
		string Kind { get; }

		// @property (readonly, nonatomic) NSString * label;
		[Export("label")]
		string Label { get; }

		[Wrap("WeakDelegate")]
		RTCMediaStreamTrackDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<RTCMediaStreamTrackDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// -(BOOL)isEnabled;
		[Export("isEnabled")]
		//[Verify(MethodToProperty)]
		bool IsEnabled { get; }

		// -(BOOL)setEnabled:(BOOL)enabled;
		[Export("setEnabled:")]
		bool SetEnabled(bool enabled);

		// -(RTCTrackState)state;
		[Export("state")]
		//	[Verify(MethodToProperty)]
		RTCTrackState State { get; }

		// -(BOOL)setState:(RTCTrackState)state;
		[Export("setState:")]
		bool SetState(RTCTrackState state);
	}

	// @interface RTCVideoTrack : RTCMediaStreamTrack
	[BaseType(typeof(RTCMediaStreamTrack))]
	[DisableDefaultCtor]
	interface RTCVideoTrack
	{
		//	 @property (readonly, nonatomic) RTCVideoSource * source;
		[Export("source")]
		RTCVideoSource Source { get; }

		// -(instancetype)initWithFactory:(RTCPeerConnectionFactory *)factory source:(RTCVideoSource *)source trackId:(NSString *)trackId;
		[Export("initWithFactory:source:trackId:")]
		IntPtr Constructor(RTCPeerConnectionFactory factory, RTCVideoSource source, string trackId);

		// -(void)addRenderer:(id<RTCVideoRenderer>)renderer;
		[Export("addRenderer:")]
		void AddRenderer(IRTCVideoRenderer renderer);

		// -(void)removeRenderer:(id<RTCVideoRenderer>)renderer;
		[Export("removeRenderer:")]
		void RemoveRenderer(IRTCVideoRenderer renderer);
	}


	// @interface RTCICECandidate : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCICECandidate
	{
		// @property (readonly, copy, nonatomic) NSString * sdpMid;
		[Export("sdpMid")]
		string SdpMid { get; }

		// @property (readonly, assign, nonatomic) NSInteger sdpMLineIndex;
		[Export("sdpMLineIndex")]
		nint SdpMLineIndex { get; }

		// @property (readonly, copy, nonatomic) NSString * sdp;
		[Export("sdp")]
		string Sdp { get; }

		// -(id)initWithMid:(NSString *)sdpMid index:(NSInteger)sdpMLineIndex sdp:(NSString *)sdp;
		[Export("initWithMid:index:sdp:")]
		IntPtr Constructor(string sdpMid, nint sdpMLineIndex, string sdp);
	}

	// @interface RTCSessionDescription : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCSessionDescription
	{
		// @property (readonly, copy, nonatomic) NSString * description;
		[Export("description")]
		string Description { get; }

		// @property (readonly, copy, nonatomic) NSString * type;
		[Export("type")]
		string Type { get; }

		// -(id)initWithType:(NSString *)type sdp:(NSString *)sdp;
		[Export("initWithType:sdp:")]
		IntPtr Constructor(string type, string sdp);
	}


	// @interface RTCConfiguration : NSObject
	[BaseType(typeof(NSObject))]
	interface RTCConfiguration
	{
		// @property (assign, nonatomic) RTCIceTransportsType iceTransportsType;
		//	[Export("iceTransportsType", ArgumentSemantic.Assign)]
		//	RTCIceTransportsType IceTransportsType { get; set; }

		// @property (copy, nonatomic) NSArray * iceServers;
		[Export("iceServers", ArgumentSemantic.Copy)]
		//	[Verify(StronglyTypedNSArray)]
		NSObject[] IceServers { get; set; }

		// @property (assign, nonatomic) RTCBundlePolicy bundlePolicy;
		//[Export("bundlePolicy", ArgumentSemantic.Assign)]
		//RTCBundlePolicy BundlePolicy { get; set; }

		// @property (assign, nonatomic) RTCRtcpMuxPolicy rtcpMuxPolicy;
		//[Export("rtcpMuxPolicy", ArgumentSemantic.Assign)]
		//RTCRtcpMuxPolicy RtcpMuxPolicy { get; set; }

		//// @property (assign, nonatomic) RTCTcpCandidatePolicy tcpCandidatePolicy;
		//[Export("tcpCandidatePolicy", ArgumentSemantic.Assign)]
		//RTCTcpCandidatePolicy TcpCandidatePolicy { get; set; }

		// @property (assign, nonatomic) int audioJitterBufferMaxPackets;
		[Export("audioJitterBufferMaxPackets")]
		int AudioJitterBufferMaxPackets { get; set; }

		// @property (assign, nonatomic) int iceConnectionReceivingTimeout;
		[Export("iceConnectionReceivingTimeout")]
		int IceConnectionReceivingTimeout { get; set; }

		// -(instancetype)initWithIceTransportsType:(RTCIceTransportsType)iceTransportsType bundlePolicy:(RTCBundlePolicy)bundlePolicy rtcpMuxPolicy:(RTCRtcpMuxPolicy)rtcpMuxPolicy tcpCandidatePolicy:(RTCTcpCandidatePolicy)tcpCandidatePolicy audioJitterBufferMaxPackets:(int)audioJitterBufferMaxPackets iceConnectionReceivingTimeout:(int)iceConnectionReceivingTimeout;
		//	[Export("initWithIceTransportsType:bundlePolicy:rtcpMuxPolicy:tcpCandidatePolicy:audioJitterBufferMaxPackets:iceConnectionReceivingTimeout:")]
		//		IntPtr Constructor(RTCIceTransportsType iceTransportsType, RTCBundlePolicy bundlePolicy, RTCRtcpMuxPolicy rtcpMuxPolicy, RTCTcpCandidatePolicy tcpCandidatePolicy, int audioJitterBufferMaxPackets, int iceConnectionReceivingTimeout);
	}

	// @interface RTCMediaStream : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCMediaStream
	{
		// @property (readonly, nonatomic, strong) NSArray * audioTracks;
		[Export("audioTracks", ArgumentSemantic.Strong)]
		//	[Verify(StronglyTypedNSArray)]
		RTCAudioTrack[] AudioTracks { get; }

		// @property (readonly, nonatomic, strong) NSArray * videoTracks;
		[Export("videoTracks", ArgumentSemantic.Strong)]
		//	[Verify(StronglyTypedNSArray)]
		RTCVideoTrack[] VideoTracks { get; }

		// @property (readonly, nonatomic, strong) NSString * label;
		[Export("label", ArgumentSemantic.Strong)]
		string Label { get; }

		// -(BOOL)addAudioTrack:(RTCAudioTrack *)track;
		[Export("addAudioTrack:")]
		bool AddAudioTrack(RTCAudioTrack track);

		// -(BOOL)addVideoTrack:(RTCVideoTrack *)track;
		[Export("addVideoTrack:")]
		bool AddVideoTrack(RTCVideoTrack track);

		// -(BOOL)removeAudioTrack:(RTCAudioTrack *)track;
		[Export("removeAudioTrack:")]
		bool RemoveAudioTrack(RTCAudioTrack track);

		// -(BOOL)removeVideoTrack:(RTCVideoTrack *)track;
		[Export("removeVideoTrack:")]
		bool RemoveVideoTrack(RTCVideoTrack track);
	}

	// @interface RTCPeerConnectionFactory : NSObject
	[BaseType(typeof(NSObject))]
	interface RTCPeerConnectionFactory
	{
		// +(void)initializeSSL;
		[Static]
		[Export("initializeSSL")]
		void InitializeSSL();

		// +(void)deinitializeSSL;
		[Static]
		[Export("deinitializeSSL")]
		void DeinitializeSSL();

		// -(RTCPeerConnection *)peerConnectionWithICEServers:(NSArray *)servers constraints:(RTCMediaConstraints *)constraints delegate:(id<RTCPeerConnectionDelegate>)delegate;
		[Export("peerConnectionWithICEServers:constraints:delegate:")]
		//[Verify(StronglyTypedNSArray)]
		RTCPeerConnection PeerConnectionWithICEServers(NSArray servers, RTCMediaConstraints constraints, IRTCPeerConnectionDelegate @delegate);

		// -(RTCPeerConnection *)peerConnectionWithConfiguration:(RTCConfiguration *)configuration constraints:(RTCMediaConstraints *)constraints delegate:(id<RTCPeerConnectionDelegate>)delegate;
		[Export("peerConnectionWithConfiguration:constraints:delegate:")]
		RTCPeerConnection PeerConnectionWithConfiguration(RTCConfiguration configuration, RTCMediaConstraints constraints, IRTCPeerConnectionDelegate @delegate);

		// -(RTCMediaStream *)mediaStreamWithLabel:(NSString *)label;
		[Export("mediaStreamWithLabel:")]
		RTCMediaStream MediaStreamWithLabel(string label);

		// -(RTCVideoSource *)videoSourceWithCapturer:(RTCVideoCapturer *)capturer constraints:(RTCMediaConstraints *)constraints;
		[Export("videoSourceWithCapturer:constraints:")]
		RTCVideoSource VideoSourceWithCapturer(RTCVideoCapturer capturer, RTCMediaConstraints constraints);

		// -(RTCVideoTrack *)videoTrackWithID:(NSString *)videoId source:(RTCVideoSource *)source;
		[Export("videoTrackWithID:source:")]
		RTCVideoTrack VideoTrackWithID(string videoId, RTCVideoSource source);

		// -(RTCAudioTrack *)audioTrackWithID:(NSString *)audioId;
		[Export("audioTrackWithID:")]
		RTCAudioTrack AudioTrackWithID(string audioId);
	}
	public interface IRTCPeerConnectionDelegate { }
	// @protocol RTCPeerConnectionDelegate <NSObject>
	[Protocol]
	[BaseType(typeof(NSObject))]
	interface RTCPeerConnectionDelegate
	{
		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection signalingStateChanged:(RTCSignalingState)stateChanged;
		[Abstract]
		[Export("peerConnection:signalingStateChanged:")]
		void PeerConnection(RTCPeerConnection peerConnection, RTCSignalingState stateChanged);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection addedStream:(RTCMediaStream *)stream;
		[Abstract]
		[Export("peerConnection:addedStream:")]
		void PeerConnectionAdded(RTCPeerConnection peerConnection, RTCMediaStream stream);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection removedStream:(RTCMediaStream *)stream;
		[Abstract]
		[Export("peerConnection:removedStream:")]
		void PeerConnectionRemoved(RTCPeerConnection peerConnection, RTCMediaStream stream);

		// @required -(void)peerConnectionOnRenegotiationNeeded:(RTCPeerConnection *)peerConnection;
		[Abstract]
		[Export("peerConnectionOnRenegotiationNeeded:")]
		void PeerConnectionOnRenegotiationNeeded(RTCPeerConnection peerConnection);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection iceConnectionChanged:(RTCICEConnectionState)newState;
		[Abstract]
		[Export("peerConnection:iceConnectionChanged:")]
		void PeerConnection(RTCPeerConnection peerConnection, RTCICEConnectionState newState);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection iceGatheringChanged:(RTCICEGatheringState)newState;
		[Abstract]
		[Export("peerConnection:iceGatheringChanged:")]
		void PeerConnection(RTCPeerConnection peerConnection, RTCICEGatheringState newState);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection gotICECandidate:(RTCICECandidate *)candidate;
		[Abstract]
		[Export("peerConnection:gotICECandidate:")]
		void PeerConnection(RTCPeerConnection peerConnection, RTCICECandidate candidate);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection didOpenDataChannel:(RTCDataChannel *)dataChannel;
		[Abstract]
		[Export("peerConnection:didOpenDataChannel:")]
		void PeerConnection(RTCPeerConnection peerConnection, RTCDataChannel dataChannel);
	}

	//public interface IRTCPeerConnectionDelegate { }

	// @interface RTCAudioTrack : RTCMediaStreamTrack
	[BaseType(typeof(RTCMediaStreamTrack))]
	[DisableDefaultCtor]
	interface RTCAudioTrack
	{
	}

	// @protocol RTCStatsDelegate <NSObject>
	[Model]
	[BaseType(typeof(NSObject))]
	interface RTCStatsDelegate
	{
		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection didGetStats:(NSArray *)stats;
		[Abstract]
		[Export("peerConnection:didGetStats:")]
		//[Verify(StronglyTypedNSArray)]
		void DidGetStats(RTCPeerConnection peerConnection, NSObject[] stats);
	}

	// @interface RTCDataChannelInit : NSObject
	[BaseType(typeof(NSObject))]
	interface RTCDataChannelInit
	{
		// @property (nonatomic) BOOL isOrdered;
		[Export("isOrdered")]
		bool IsOrdered { get; set; }

		// @property (nonatomic) NSInteger maxRetransmitTimeMs;
		[Export("maxRetransmitTimeMs")]
		nint MaxRetransmitTimeMs { get; set; }

		// @property (nonatomic) NSInteger maxRetransmits;
		[Export("maxRetransmits")]
		nint MaxRetransmits { get; set; }

		// @property (nonatomic) BOOL isNegotiated;
		[Export("isNegotiated")]
		bool IsNegotiated { get; set; }

		// @property (nonatomic) NSInteger streamId;
		[Export("streamId")]
		nint StreamId { get; set; }

		// @property (nonatomic) NSString * protocol;
		[Export("protocol")]
		string Protocol { get; set; }
	}

	// @interface RTCDataBuffer : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCDataBuffer
	{
		// @property (readonly, nonatomic) NSData * data;
		[Export("data")]
		NSData Data { get; }

		// @property (readonly, nonatomic) BOOL isBinary;
		[Export("isBinary")]
		bool IsBinary { get; }

		// -(instancetype)initWithData:(NSData *)data isBinary:(BOOL)isBinary;
		[Export("initWithData:isBinary:")]
		IntPtr Constructor(NSData data, bool isBinary);
	}

	// @protocol RTCDataChannelDelegate <NSObject>
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface RTCDataChannelDelegate
	{
		// @required -(void)channelDidChangeState:(RTCDataChannel *)channel;
		[Abstract]
		[Export("channelDidChangeState:")]
		void ChannelDidChangeState(RTCDataChannel channel);

		// @required -(void)channel:(RTCDataChannel *)channel didReceiveMessageWithBuffer:(RTCDataBuffer *)buffer;
		[Abstract]
		[Export("channel:didReceiveMessageWithBuffer:")]
		void Channel(RTCDataChannel channel, RTCDataBuffer buffer);

		// @optional -(void)channel:(RTCDataChannel *)channel didChangeBufferedAmount:(NSUInteger)amount;
		[Export("channel:didChangeBufferedAmount:")]
		void Channel(RTCDataChannel channel, nuint amount);
	}

	// @interface RTCDataChannel : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCDataChannel
	{
		// @property (readonly, nonatomic) NSString * label;
		[Export("label")]
		string Label { get; }

		// @property (readonly, nonatomic) BOOL isReliable;
		[Export("isReliable")]
		bool IsReliable { get; }

		// @property (readonly, nonatomic) BOOL isOrdered;
		[Export("isOrdered")]
		bool IsOrdered { get; }

		// @property (readonly, nonatomic) NSUInteger maxRetransmitTime;
		[Export("maxRetransmitTime")]
		nuint MaxRetransmitTime { get; }

		// @property (readonly, nonatomic) NSUInteger maxRetransmits;
		[Export("maxRetransmits")]
		nuint MaxRetransmits { get; }

		// @property (readonly, nonatomic) NSString * protocol;
		[Export("protocol")]
		string Protocol { get; }

		// @property (readonly, nonatomic) BOOL isNegotiated;
		[Export("isNegotiated")]
		bool IsNegotiated { get; }

		// @property (readonly, nonatomic) NSInteger streamId;
		[Export("streamId")]
		nint StreamId { get; }

		// @property (readonly, nonatomic) RTCDataChannelState state;
		[Export("state")]
		RTCDataChannelState State { get; }

		// @property (readonly, nonatomic) NSUInteger bufferedAmount;
		[Export("bufferedAmount")]
		nuint BufferedAmount { get; }

		[Wrap("WeakDelegate")]
		RTCDataChannelDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<RTCDataChannelDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// -(void)close;
		[Export("close")]
		void Close();

		// -(BOOL)sendData:(RTCDataBuffer *)data;
		[Export("sendData:")]
		bool SendData(RTCDataBuffer data);
	}


	//[Static]
	////[Verify(ConstantsInterfaceAssociation)]
	//partial interface Constants
	//{
	//	// extern NSString *const kRTCSessionDescriptionDelegateErrorDomain;
	//	[Field("kRTCSessionDescriptionDelegateErrorDomain", "__Internal")]
	//	NSString kRTCSessionDescriptionDelegateErrorDomain { get; }

	//	// extern const int kRTCSessionDescriptionDelegateErrorCode;
	//	[Field("kRTCSessionDescriptionDelegateErrorCode", "__Internal")]
	//	int kRTCSessionDescriptionDelegateErrorCode { get; }
	//}

	// @protocol RTCSessionDescriptionDelegate <NSObject>
	public interface IRTCSessionDescriptionDelegate { }
	[Model, Protocol]
	[BaseType(typeof(NSObject))]
	interface RTCSessionDescriptionDelegate
	{
		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection didCreateSessionDescription:(RTCSessionDescription *)sdp error:(NSError *)error;
		[Abstract]
		[Export("peerConnection:didCreateSessionDescription:error:")]
		void DidCreateSessionDescription(RTCPeerConnection peerConnection, RTCSessionDescription sdp, NSError error);

		// @required -(void)peerConnection:(RTCPeerConnection *)peerConnection didSetSessionDescriptionWithError:(NSError *)error;
		[Abstract]
		[Export("peerConnection:didSetSessionDescriptionWithError:")]
		void DidSetSessionDescriptionWithError(RTCPeerConnection peerConnection, NSError error);
	}


	// @interface RTCMediaSource : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCMediaSource
	{
		// @property (readonly, assign, nonatomic) RTCSourceState state;
		[Export("state", ArgumentSemantic.Assign)]
		RTCSourceState State { get; }
	}

	// @interface RTCVideoSource : RTCMediaSource
	[BaseType(typeof(RTCMediaSource))]
	[DisableDefaultCtor]
	interface RTCVideoSource
	{
	}


	// @interface RTCVideoCapturer : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCVideoCapturer
	{
		// +(RTCVideoCapturer *)capturerWithDeviceName:(NSString *)deviceName;
		[Static]
		[Export("capturerWithDeviceName:")]
		RTCVideoCapturer CapturerWithDeviceName(string deviceName);
	}

	// @interface RTCI420Frame : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCI420Frame
	{
		// @property (readonly, nonatomic) NSUInteger width;
		[Export("width")]
		nuint Width { get; }

		// @property (readonly, nonatomic) NSUInteger height;
		[Export("height")]
		nuint Height { get; }

		// @property (readonly, nonatomic) NSUInteger chromaWidth;
		[Export("chromaWidth")]
		nuint ChromaWidth { get; }

		// @property (readonly, nonatomic) NSUInteger chromaHeight;
		[Export("chromaHeight")]
		nuint ChromaHeight { get; }

		// @property (readonly, nonatomic) NSUInteger chromaSize;
		[Export("chromaSize")]
		nuint ChromaSize { get; }

		// @property (readonly, nonatomic) const uint8_t * yPlane;
		[Export("yPlane")]
		unsafe IntPtr YPlane { get; }

		// @property (readonly, nonatomic) const uint8_t * uPlane;
		[Export("uPlane")]
		unsafe IntPtr UPlane { get; }

		// @property (readonly, nonatomic) const uint8_t * vPlane;
		[Export("vPlane")]
		unsafe IntPtr VPlane { get; }

		// @property (readonly, nonatomic) NSInteger yPitch;
		[Export("yPitch")]
		nint YPitch { get; }

		// @property (readonly, nonatomic) NSInteger uPitch;
		[Export("uPitch")]
		nint UPitch { get; }

		// @property (readonly, nonatomic) NSInteger vPitch;
		[Export("vPitch")]
		nint VPitch { get; }

		// -(BOOL)makeExclusive;
		[Export("makeExclusive")]
		//[Verify(MethodToProperty)]
		bool MakeExclusive { get; }
	}

	// @protocol RTCEAGLVideoViewDelegate
	[BaseType(typeof(NSObject))]
	[Protocol, Model]
	interface RTCEAGLVideoViewDelegate
	{
		// @required -(void)videoView:(RTCEAGLVideoView *)videoView didChangeVideoSize:(CGSize)size;
		//[Abstract]
		[Export("videoView:didChangeVideoSize:")]
		void DidChangeVideoSize(RTCEAGLVideoView videoView, CGSize size);
	}


	// @protocol RTCVideoRenderer <NSObject>
	[BaseType(typeof(NSObject))]
	[Protocol]
	interface RTCVideoRenderer
	{
		// @required -(void)setSize:(CGSize)size;
		[Abstract]
		[Export("setSize:")]
		void SetSize(CGSize size);

		// @required -(void)renderFrame:(RTCI420Frame *)frame;
		[Abstract]
		[Export("renderFrame:")]
		void RenderFrame(RTCI420Frame frame);
	}


	public interface IRTCVideoRenderer { }
	// @interface RTCEAGLVideoView : UIView <RTCVideoRenderer>
	[BaseType(typeof(UIView))]
	interface RTCEAGLVideoView : RTCVideoRenderer
	{
		[Export("initWithFrame:")]
		IntPtr Constructor(CGRect frame);

		[Wrap("WeakDelegate")]
		RTCEAGLVideoViewDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<RTCEAGLVideoViewDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

	}


	// @interface RTCICEServer : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCICEServer
	{
		// @property (readonly, nonatomic, strong) NSURL * URI;
		[Export("URI", ArgumentSemantic.Strong)]
		NSUrl URI { get; }

		// @property (readonly, copy, nonatomic) NSString * username;
		[Export("username")]
		string Username { get; }

		// @property (readonly, copy, nonatomic) NSString * password;
		[Export("password")]
		string Password { get; }

		// -(id)initWithURI:(NSURL *)URI username:(NSString *)username password:(NSString *)password;
		[Export("initWithURI:username:password:")]
		IntPtr Constructor(NSUrl URI, string username, string password);
	}


	// @interface RTCPair : NSObject
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface RTCPair
	{
		// @property (readonly, nonatomic, strong) NSString * key;
		[Export("key", ArgumentSemantic.Strong)]
		string Key { get; }

		// @property (readonly, nonatomic, strong) NSString * value;
		[Export("value", ArgumentSemantic.Strong)]
		string Value { get; }

		// -(id)initWithKey:(NSString *)key value:(NSString *)value;
		[Export("initWithKey:value:")]
		IntPtr Constructor(string key, string value);
	}

}
