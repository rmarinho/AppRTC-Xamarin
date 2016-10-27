using Foundation;
using System;
using UIKit;

namespace AppRTC.Demo
{
	public partial class ARTCRoomTextInputViewCell : UITableViewCell
	{

		public ARTCRoomTextInputViewCell(IntPtr handle) : base(handle)
		{
		}

		public ARTCRoomViewController RoomController { get; internal set; }

		partial void TouchButtonPressed(UIButton sender)
		{
			RoomController?.ShouldJoinRoom(this, textField.Text);
		}
	}
}