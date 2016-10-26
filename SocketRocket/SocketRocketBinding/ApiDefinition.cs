using System;
using System.Runtime.InteropServices;
//using CFNetwork;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace SocketRocketBinding
{

	//	[Static]
	//	[Verify(ConstantsInterfaceAssociation)]
	//partial interface Constants
	//{
	//	// extern NSString *const SRWebSocketErrorDomain;
	//	[Field("SRWebSocketErrorDomain", "__Internal")]
	//	NSString SRWebSocketErrorDomain { get; }

	//	// extern NSString *const SRHTTPResponseErrorKey;
	//	[Field("SRHTTPResponseErrorKey", "__Internal")]
	//	NSString SRHTTPResponseErrorKey { get; }
	//}

	// @interface SRWebSocket : NSObject <NSStreamDelegate>
	[BaseType(typeof(NSObject))]
	interface SRWebSocket : INSStreamDelegate
	{
		[Wrap("WeakDelegate")]
		SRWebSocketDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<SRWebSocketDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// @property (readonly, nonatomic) SRReadyState readyState;
		[Export("readyState")]
		SRReadyState ReadyState { get; }

		// @property (readonly, retain, nonatomic) NSURL * url;
		[Export("url", ArgumentSemantic.Retain)]
		NSUrl Url { get; }

		// @property (readonly, nonatomic) CFHTTPMessageRef receivedHTTPHeaders;
		//[Export("receivedHTTPHeaders")]
		//[DllImport ("/System/Library/Frameworks/CFNetwork.framework/CFNetwork")]
		//unsafe CFHTTPMessageRef* ReceivedHTTPHeaders { get; }

		// @property (readwrite, nonatomic) NSArray * requestCookies;
		[Export("requestCookies", ArgumentSemantic.Assign)]
		//[Verify(StronglyTypedNSArray)]
		NSObject[] RequestCookies { get; set; }

		// @property (readonly, copy, nonatomic) NSString * protocol;
		[Export("protocol")]
		string Protocol { get; }

		// -(id)initWithURLRequest:(NSURLRequest *)request protocols:(NSArray *)protocols allowsUntrustedSSLCertificates:(BOOL)allowsUntrustedSSLCertificates;
		[Export("initWithURLRequest:protocols:allowsUntrustedSSLCertificates:")]
		//[Verify(StronglyTypedNSArray)]
		IntPtr Constructor(NSUrlRequest request, NSObject[] protocols, bool allowsUntrustedSSLCertificates);

		// -(id)initWithURLRequest:(NSURLRequest *)request protocols:(NSArray *)protocols;
		[Export("initWithURLRequest:protocols:")]
		//[Verify(StronglyTypedNSArray)]
		IntPtr Constructor(NSUrlRequest request, NSObject[] protocols);

		// -(id)initWithURLRequest:(NSURLRequest *)request;
		[Export("initWithURLRequest:")]
		IntPtr Constructor(NSUrlRequest request);

		// -(id)initWithURL:(NSURL *)url protocols:(NSArray *)protocols allowsUntrustedSSLCertificates:(BOOL)allowsUntrustedSSLCertificates;
		[Export("initWithURL:protocols:allowsUntrustedSSLCertificates:")]
		//[Verify(StronglyTypedNSArray)]
		IntPtr Constructor(NSUrl url, NSObject[] protocols, bool allowsUntrustedSSLCertificates);

		// -(id)initWithURL:(NSURL *)url protocols:(NSArray *)protocols;
		[Export("initWithURL:protocols:")]
		//[Verify(StronglyTypedNSArray)]
		IntPtr Constructor(NSUrl url, NSObject[] protocols);

		// -(id)initWithURL:(NSURL *)url;
		[Export("initWithURL:")]
		IntPtr Constructor(NSUrl url);

		// -(void)setDelegateOperationQueue:(NSOperationQueue *)queue;
		[Export("setDelegateOperationQueue:")]
		void SetDelegateOperationQueue(NSOperationQueue queue);

		// -(void)setDelegateDispatchQueue:(dispatch_queue_t)queue;
		[Export("setDelegateDispatchQueue:")]
		void SetDelegateDispatchQueue(DispatchQueue queue);

		// -(void)scheduleInRunLoop:(NSRunLoop *)aRunLoop forMode:(NSString *)mode;
		[Export("scheduleInRunLoop:forMode:")]
		void ScheduleInRunLoop(NSRunLoop aRunLoop, string mode);

		// -(void)unscheduleFromRunLoop:(NSRunLoop *)aRunLoop forMode:(NSString *)mode;
		[Export("unscheduleFromRunLoop:forMode:")]
		void UnscheduleFromRunLoop(NSRunLoop aRunLoop, string mode);

		// -(void)open;
		[Export("open")]
		void Open();

		// -(void)close;
		[Export("close")]
		void Close();

		// -(void)closeWithCode:(NSInteger)code reason:(NSString *)reason;
		[Export("closeWithCode:reason:")]
		void CloseWithCode(nint code, string reason);

		// -(void)send:(id)data;
		[Export("send:")]
		void Send(NSObject data);

		// -(void)sendPing:(NSData *)data;
		[Export("sendPing:")]
		void SendPing(NSData data);
	}

	// @protocol SRWebSocketDelegate <NSObject>
	[BaseType(typeof(NSObject))]
	[Model, Protocol]
	interface SRWebSocketDelegate
	{
		// @required -(void)webSocket:(SRWebSocket *)webSocket didReceiveMessage:(id)message;
		[Abstract]
		[Export("webSocket:didReceiveMessage:")]
		void WebSocket(SRWebSocket webSocket, NSObject message);

		// @optional -(void)webSocketDidOpen:(SRWebSocket *)webSocket;
		[Export("webSocketDidOpen:")]
		void WebSocketDidOpen(SRWebSocket webSocket);

		// @optional -(void)webSocket:(SRWebSocket *)webSocket didFailWithError:(NSError *)error;
		[Export("webSocket:didFailWithError:")]
		void WebSocket(SRWebSocket webSocket, NSError error);

		// @optional -(void)webSocket:(SRWebSocket *)webSocket didCloseWithCode:(NSInteger)code reason:(NSString *)reason wasClean:(BOOL)wasClean;
		[Export("webSocket:didCloseWithCode:reason:wasClean:")]
		void WebSocket(SRWebSocket webSocket, nint code, string reason, bool wasClean);

		// @optional -(void)webSocket:(SRWebSocket *)webSocket didReceivePong:(NSData *)pongPayload;
		[Export("webSocket:didReceivePong:")]
		void WebSocket(SRWebSocket webSocket, NSData pongPayload);

		// @optional -(BOOL)webSocketShouldConvertTextFrameToString:(SRWebSocket *)webSocket;
		[Export("webSocketShouldConvertTextFrameToString:")]
		bool WebSocketShouldConvertTextFrameToString(SRWebSocket webSocket);
	}

	//public interface ISRWebSocketDelegate
	//{ }

	// @interface SRCertificateAdditions (NSURLRequest)
	//[Category]
	//[BaseType(typeof(NSUrlRequest))]
	//interface NSURLRequest_SRCertificateAdditions
	//{
	//	// @property (readonly, retain, nonatomic) NSArray * SR_SSLPinnedCertificates;
	//	[Export("SR_SSLPinnedCertificates", ArgumentSemantic.Retain)]
	//	//	[Verify(StronglyTypedNSArray)]
	//	NSObject[] SR_SSLPinnedCertificates { get; }
	//}

	//// @interface SRCertificateAdditions (NSMutableURLRequest)
	//[Category]
	//[BaseType(typeof(NSMutableUrlRequest))]
	//interface NSMutableURLRequest_SRCertificateAdditions
	//{
	//	// @property (retain, nonatomic) NSArray * SR_SSLPinnedCertificates;
	//	[Export("SR_SSLPinnedCertificates", ArgumentSemantic.Retain)]
	//	//[Verify(StronglyTypedNSArray)]
	//	NSObject[] SR_SSLPinnedCertificates { get; set; }
	//}

	// @interface SRWebSocket (NSRunLoop)
	[Category]
	[BaseType(typeof(NSRunLoop))]
	interface NSRunLoop_SRWebSocket
	{
		// +(NSRunLoop *)SR_networkRunLoop;
		[Static]
		[Export("SR_networkRunLoop")]
		//	[Verify(MethodToProperty)]
		NSRunLoop SR_networkRunLoop { get; }
	}

	//[Static]
	////	[Verify(ConstantsInterfaceAssociation)]
	//partial interface Constants
	//{
	//	// extern double SocketRocketVersionNumber;
	//	[Field("SocketRocketVersionNumber", "__Internal")]
	//	double SocketRocketVersionNumber { get; }

	//	// extern const unsigned char [] SocketRocketVersionString;
	//	//[Field("SocketRocketVersionString", "__Internal")]
	//	//IntPtr SocketRocketVersionString { get; }
	//}

}
