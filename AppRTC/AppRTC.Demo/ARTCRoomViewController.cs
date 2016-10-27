using Foundation;
using System;
using UIKit;

namespace AppRTC.Demo
{
	public partial class ARTCRoomViewController : UITableViewController, IUITableViewDelegate, IRoomController
	{
		public ARTCRoomViewController(IntPtr handle) : base(handle)
		{
			TableView.Delegate = this;
			TableView.DataSource = this;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			NavigationController.SetNavigationBarHidden(false, true);
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return 1;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Row == 0)
			{
				var roomTextInputViewCell = tableView.DequeueReusableCell("RoomInputCell", indexPath) as ARTCRoomTextInputViewCell;
				roomTextInputViewCell.RoomController = this;
				return roomTextInputViewCell;
			}
			return base.GetCell(tableView, indexPath);
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			var vc = segue.DestinationViewController as ARTCVideoChatViewController;
			vc?.SetRoomName(sender.ToString());
			base.PrepareForSegue(segue, sender);
		}

		public void ShouldJoinRoom(ARTCRoomTextInputViewCell cell, string room)
		{
			PerformSegue("ARTCVideoChatViewController", new NSString(room));
		}
	}

	internal interface IRoomController
	{
		void ShouldJoinRoom(ARTCRoomTextInputViewCell cell, string room);
	}
}