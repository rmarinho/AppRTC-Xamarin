using Foundation;
using System;
using UIKit;
using WebRTCBinding;
using CoreGraphics;

namespace AppRTC.Demo
{
	public partial class ARTCVideoChatViewController : UIViewController, IVideoChatController, IARDAppClientDelegate
	{

		string roomUrl;
		string roomName;

		IARDAppClient client;
		RTCVideoTrack localVideoTrack;
		RTCVideoTrack remoteVideoTrack;
		CGSize localVideoSize;
		CGSize remoteVideoSize;
		const string SERVER_HOST_URL = "https://apprtc.appspot.com";
		RTCEAGLVideoView remoteView;
		RTCEAGLVideoView localView;

		UIView footerView;
		UILabel urlLabel;
		UIView buttonContainerView;
		UIButton audioButton;
		UIButton videoButton;
		UIButton hangupButton;

		bool isZoom; //used for double tap remote view
		bool isAudioMute;
		bool isVideoMute;


		RTCEAGLVideoViewDelegate _rtcvideoDeleagate;
		public ARTCVideoChatViewController(IntPtr handle) : base(handle)
		{
		}
		public override void ViewDidLoad()
		{
			try
			{
				SetupUI();
			}
			catch (Exception ex)
			{

			}

			isZoom = false;
			isAudioMute = false;
			isVideoMute = false;
			audioButton.Layer.CornerRadius = 20.0f;
			videoButton.Layer.CornerRadius = 20.0f;
			hangupButton.Layer.CornerRadius = 20.0f;

			View.AddGestureRecognizer(new UITapGestureRecognizer((obj) => ToggleButtonContainer()));
			View.AddGestureRecognizer(new UITapGestureRecognizer((obj) => ZoomRemote()) { NumberOfTapsRequired = 2 });

			NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, OrientationChanged);


			// //RTCEAGLVideoViewDelegate provides notifications on video frame dimensions
			_rtcvideoDeleagate = new CustomRTCEAGLVideoViewDelegate(this);
			remoteView.Delegate = _rtcvideoDeleagate;
			localView.Delegate = _rtcvideoDeleagate;

			base.ViewDidLoad();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			NavigationController.SetNavigationBarHidden(true, true);

			// //Display the Local View full screen while connecting to Room
			// [self.localViewBottomConstraint setConstant:0.0f];
			// [self.localViewRightConstraint setConstant:0.0f];
			// [self.localViewHeightConstraint setConstant:self.view.frame.size.height];
			// [self.localViewWidthConstraint setConstant:self.view.frame.size.width];
			// [self.footerViewBottomConstraint setConstant:0.0f];

			// //Connect to the room
			Disconnect();
			client = new ARDAppClient(this);
			client.SetServerHostUrl(SERVER_HOST_URL);
			client.ConnectToRoomWithId(roomName);
			urlLabel.Text = roomUrl;
		}

		public override void ViewWillDisappear(bool animated)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(UIDevice.OrientationDidChangeNotification);
			Disconnect();
			base.ViewWillDisappear(animated);
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		void SetupUI()
		{
			var width = View.Frame.Width;
			var height = View.Frame.Height;
			int y = 0;
			remoteView = new RTCEAGLVideoView(new CGRect(0, y, width, height));
			y = y + 100;
			localView = new RTCEAGLVideoView(new CGRect(0, y, width, 100));
			localView.Hidden = true;
			y = y + 200;
			buttonContainerView = new UIView();
			buttonContainerView.Frame = new CGRect(0, y, width, 100);

			audioButton = UIButton.FromType(UIButtonType.RoundedRect);
			audioButton.SetTitle("audio", UIControlState.Normal);
			audioButton.TouchUpInside += AudioButton_TouchUpInside;

			videoButton = UIButton.FromType(UIButtonType.RoundedRect);
			videoButton.SetTitle("video", UIControlState.Normal);
			videoButton.TouchUpInside += VideoButton_TouchUpInside;

			hangupButton = UIButton.FromType(UIButtonType.RoundedRect);
			hangupButton.SetTitle("hang-up", UIControlState.Normal);
			hangupButton.TouchUpInside += HangupButton_TouchUpInside;

			buttonContainerView.Add(audioButton);
			buttonContainerView.Add(videoButton);
			buttonContainerView.Add(hangupButton);

			urlLabel = new UILabel { TextColor = UIColor.White };

			y = y + 100;
			urlLabel.Frame = new CGRect(0, y, width, 100);

			Add(remoteView);
			Add(localView);
			Add(buttonContainerView);
			Add(urlLabel);

		}

		internal void Disconnect()
		{
			if (client != null)
			{

				localVideoTrack?.RemoveRenderer(localView);
				remoteVideoTrack?.RemoveRenderer(remoteView);
				localVideoTrack = null;
				localView.RenderFrame(null);
				RemoteDisconnected();
				client.Disconnect();
			}
		}

		void RemoteDisconnected()
		{
			remoteVideoTrack?.RemoveRenderer(remoteView);
			remoteVideoTrack = null;
			remoteView.RenderFrame(null);

			//[self videoView:self.localView didChangeVideoSize:self.localVideoSize];   
		}

		void LocalClear()
		{
			localVideoTrack?.RemoveRenderer(localView);
			localVideoTrack = null;
			localView.RenderFrame(null);
		}

		void OrientationChanged(NSNotification notification)
		{
			// [self videoView:self.localView didChangeVideoSize:self.localVideoSize];
			// [self videoView:self.remoteView didChangeVideoSize:self.remoteVideoSize];

			Console.WriteLine("Received a notification UIDevice", notification);
		}

		public void SetRoomName(string name)
		{
			roomName = name;
			roomUrl = $"{SERVER_HOST_URL}/r/{roomName}";
		}

		void ToggleButtonContainer()
		{
			//[UIView animateWithDuration:0.3f animations:^{
			//       if (self.buttonContainerViewLeftConstraint.constant <= -40.0f) {
			//           [self.buttonContainerViewLeftConstraint setConstant:20.0f];
			//           [self.buttonContainerView setAlpha:1.0f];
			//       } else {
			//           [self.buttonContainerViewLeftConstraint setConstant:-40.0f];
			//           [self.buttonContainerView setAlpha:0.0f];
			//       }
			//       [self.view layoutIfNeeded];
			//   }];

		}

		void ZoomRemote()
		{
			isZoom = !isZoom;
			//[self videoView:self.remoteView didChangeVideoSize:self.remoteVideoSize];
		}

		void AudioButton_TouchUpInside(object sender, EventArgs e)
		{
			if (isAudioMute)
			{
				client.UnmuteAudioIn();
			}
			else
			{
				client.MuteAudioIn();
			}
			isAudioMute = !isAudioMute;
		}

		void VideoButton_TouchUpInside(object sender, EventArgs e)
		{
			if (isVideoMute)
			{
				client.SwapCameraToFront();
			}
			else
			{
				client.SwapCameraToBack();
			}
			isVideoMute = !isVideoMute;
		}

		void HangupButton_TouchUpInside(object sender, EventArgs e)
		{
			client?.Disconnect();
			NavigationController.PopToRootViewController(true);
		}

		public void DidChangeState(IARDAppClient client, ARDAppClientState state)
		{
			switch (state)
			{
				case ARDAppClientState.Connected:
					Console.WriteLine("Client connected");
					break;
				case ARDAppClientState.Connecting:

					Console.WriteLine("Client connecting");
					break;
				case ARDAppClientState.Disconnected:
					RemoteDisconnected();
					Console.WriteLine("Client disconnected");
					break;
				default:
					break;
			}
		}

		public void DidReceiveLocalVideoTrack(IARDAppClient client, RTCVideoTrack localVideoTrack)
		{
			LocalClear();
			this.localVideoTrack = localVideoTrack;
			localVideoTrack.AddRenderer(localView);
		}

		public void DidReceiveRemoteVideoTrack(IARDAppClient client, RTCVideoTrack remoteVideoTrack)
		{
			this.remoteVideoTrack = remoteVideoTrack;
			this.remoteVideoTrack.AddRenderer(remoteView);

			//[UIView animateWithDuration:0.4f animations:^{
			//    //Instead of using 0.4 of screen size, we re-calculate the local view and keep our aspect ratio
			//    UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
			//    CGRect videoRect = CGRectMake(0.0f, 0.0f, self.view.frame.size.width/4.0f, self.view.frame.size.height/4.0f);
			//    if (orientation == UIDeviceOrientationLandscapeLeft || orientation == UIDeviceOrientationLandscapeRight) {
			//        videoRect = CGRectMake(0.0f, 0.0f, self.view.frame.size.height/4.0f, self.view.frame.size.width/4.0f);
			//    }
			//    CGRect videoFrame = AVMakeRectWithAspectRatioInsideRect(_localView.frame.size, videoRect);

			//    [self.localViewWidthConstraint setConstant:videoFrame.size.width];
			//    [self.localViewHeightConstraint setConstant:videoFrame.size.height];


			//    [self.localViewBottomConstraint setConstant:28.0f];
			//    [self.localViewRightConstraint setConstant:28.0f];
			//    [self.footerViewBottomConstraint setConstant:-80.0f];
			//    [self.view layoutIfNeeded];
			//}];
		}

		public void DidError(IARDAppClient client, NSError error)
		{
			UIAlertView alertView = new UIAlertView("", error.ToString(), null, "OK", null);
			alertView.Show();
			Disconnect();
		}
	}


	class CustomRTCEAGLVideoViewDelegate : RTCEAGLVideoViewDelegate
	{
		ARTCVideoChatViewController _controller;
		public CustomRTCEAGLVideoViewDelegate(ARTCVideoChatViewController controller)
		{
			_controller = controller;
		}
		public override void DidChangeVideoSize(RTCEAGLVideoView videoView, CGSize size)
		{
		}
	}

	public interface IVideoChatController
	{
		void SetRoomName(string name);
	}
}