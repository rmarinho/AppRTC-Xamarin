using Foundation;
using System;
using UIKit;
using WebRTCBinding;
using CoreGraphics;

namespace AppRTC.Demo
{
	public partial class ARTCVideoChatViewController : UIViewController, IVideoChatController, IARDAppClientDelegate, IRTCEAGLVideoViewDelegate
	{

		string roomUrl;
		string roomName;

		IARDAppClient client;
		RTCVideoTrack localVideoTrack;
		RTCVideoTrack remoteVideoTrack;
		CGSize localVideoSize;
		CGSize remoteVideoSize;
		const string SERVER_HOST_URL = "https://apprtc.appspot.com";
		internal RTCEAGLVideoView remoteView;
		internal RTCEAGLVideoView localView;

		UIView footerView;
		UILabel urlLabel;
		UIView buttonContainerView;
		UIButton audioButton;
		UIButton videoButton;
		UIButton hangupButton;

		bool isZoom; //used for double tap remote view
		bool isAudioMute;
		bool isVideoMute;


		//	RTCEAGLVideoViewDelegate _rtcvideoDeleagate;
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
			//	_rtcvideoDeleagate = new CustomRTCEAGLVideoViewDelegate(this);
			//	remoteView.Delegate = (IRTCEAGLVideoViewDelegate)this;
			//	localView.Delegate = this;

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
			this.DidChangeVideoSize(localView, localVideoSize);
		}

		void LocalClear()
		{
			localVideoTrack?.RemoveRenderer(localView);
			localVideoTrack = null;
			localView.RenderFrame(null);
		}

		void OrientationChanged(NSNotification notification)
		{
			this.DidChangeVideoSize(localView, localVideoSize);
			this.DidChangeVideoSize(remoteView, remoteVideoSize);

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
			this.DidChangeVideoSize(remoteView, remoteVideoSize); ;
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

			UIView.Animate(0.4f, () =>
			{
				//    //Instead of using 0.4 of screen size, we re-calculate the local view and keep our aspect ratio
				UIDeviceOrientation orientation = UIDevice.CurrentDevice.Orientation;
				var containerWidth = View.Frame.Size.Width;
				var containerHeight = View.Frame.Size.Height;
				var videoRect = new CGRect(0.0f, 0.0f, containerWidth / 4.0f, containerHeight / 4.0f);
				if (orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight)
				{
					videoRect = new CGRect(0.0f, 0.0f, containerHeight / 4.0f, containerWidth / 4.0f);
				}
				CGRect videoFrame = AVFoundation.AVUtilities.WithAspectRatio(videoRect, localView.Frame.Size); //AVMakeRectWithAspectRatioInsideRect(aspectRatio, videoRect);

				//    [self.localViewWidthConstraint setConstant:videoFrame.size.width];
				//    [self.localViewHeightConstraint setConstant:videoFrame.size.height];


				//    [self.localViewBottomConstraint setConstant:28.0f];
				//    [self.localViewRightConstraint setConstant:28.0f];
				//    [self.footerViewBottomConstraint setConstant:-80.0f];
				View.LayoutIfNeeded();
			}); ;
		}

		[Export("videoView:didChangeVideoSize:")]
		public void DidChangeVideoSize(RTCEAGLVideoView videoView, CGSize size)
		{
			UIDeviceOrientation orientation = UIDevice.CurrentDevice.Orientation;
			UIView.Animate(0.4f, () =>
			{
				var containerWidth = View.Frame.Size.Width;
				var containerHeight = View.Frame.Size.Height;
				var defaultAspectRatio = new CGSize(4, 3);

				CGSize aspectRatio = size.IsEmpty ? defaultAspectRatio : size;
				CGRect videoRect = View.Bounds;
				if (videoView == localView)
				{
					//Resize the Local View depending if it is full screen or thumbnail
					localVideoSize = size;

					if (remoteVideoTrack != null)
					{
						videoRect = new CGRect(0.0f, 0.0f, containerWidth / 4.0f, containerHeight / 4.0f);
						if (orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight)
						{
							videoRect = new CGRect(0.0f, 0.0f, containerHeight / 4.0f, containerWidth / 4.0f);
						}
					}
					CGRect videoFrame = AVFoundation.AVUtilities.WithAspectRatio(videoRect, aspectRatio); //AVMakeRectWithAspectRatioInsideRect(aspectRatio, videoRect);


					//			//Resize the localView accordingly
					//			[self.localViewWidthConstraint setConstant: videoFrame.size.width];

					//			[self.localViewHeightConstraint setConstant:videoFrame.size.height];
					//            if (self.remoteVideoTrack) {
					//                [self.localViewBottomConstraint setConstant:28.0f]; //bottom right corner
					//                [self.localViewRightConstraint setConstant:28.0f];
					//            } else {
					//                [self.localViewBottomConstraint setConstant:containerHeight/2.0f - videoFrame.size.height/2.0f]; //center
					//                [self.localViewRightConstraint setConstant:containerWidth/2.0f - videoFrame.size.width/2.0f]; //center
					//            }
				}
				else if (videoView == remoteView)
				{
					//Resize Remote View
					remoteVideoSize = size;

					CGRect videoFrame = AVFoundation.AVUtilities.WithAspectRatio(videoRect, aspectRatio); //AVMakeRectWithAspectRatioInsideRect(aspectRatio, videoRect);
					if (isZoom)
					{
						//Set Aspect Fill
						var scale = Math.Max(containerWidth / videoFrame.Size.Width, containerHeight / videoFrame.Size.Height);

						var newSize = new CGSize(videoFrame.Size.Width * scale, videoFrame.Size.Height * scale);

						//videoFrame.Size.Width *= scale;
						//videoFrame.size.height *= scale;
						videoFrame.Size = newSize;

					}
					//            [self.remoteViewTopConstraint setConstant:containerHeight/2.0f - videoFrame.size.height/2.0f];
					//            [self.remoteViewBottomConstraint setConstant:containerHeight/2.0f - videoFrame.size.height/2.0f];
					//            [self.remoteViewLeftConstraint setConstant:containerWidth/2.0f - videoFrame.size.width/2.0f]; //center
					//            [self.remoteViewRightConstraint setConstant:containerWidth/2.0f - videoFrame.size.width/2.0f]; //center
				}
				View.LayoutIfNeeded();
			});
		}

		public void DidError(IARDAppClient client, NSError error)
		{
			var alertView = new UIAlertView("", error.ToString(), null, "OK", null);
			alertView.Show();
			Disconnect();
		}
	}

	public interface IVideoChatController
	{
		void SetRoomName(string name);
	}
}