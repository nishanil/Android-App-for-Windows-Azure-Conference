//
// ConferenceElements.cs: cells that can render a time slot
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;

namespace Monospace11
{
	public class SessionCell : UITableViewCell {
		static UIFont bigFont = AppDelegate.Current.FontCell;
		static UIFont midFont = AppDelegate.Current.FontCellMedium;
		static UIFont smallFont = AppDelegate.Current.FontCellSmall;
		static UIImage favorite = UIImage.FromFile ("Images/favorite.png");
		static UIImage favorited = UIImage.FromFile ("Images/favorited.png");
		UILabel bigLabel, smallLabel;
		UIButton button;
		MonkeySpace.Core.Session session;
		const int ImageSpace = 32;
		const int Padding = 8;
		
		public SessionCell (UITableViewCellStyle style, NSString ident, MonkeySpace.Core.Session session, string big, string small) : base (style, ident)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Blue;
			
			bigLabel = new UILabel () {
				TextAlignment = UITextAlignment.Left,
				BackgroundColor = UIColor.Clear
			};
			smallLabel = new UILabel () {
				TextAlignment = UITextAlignment.Left,
				Font = smallFont,
				TextColor = UIColor.DarkGray,
				BackgroundColor = UIColor.Clear
			};
			button = UIButton.FromType (UIButtonType.Custom);
			button.TouchDown += delegate {
				UpdateImage (ToggleFavorite ());
			};
			UpdateCell (session, big, small);
			
			ContentView.Add (bigLabel);
			ContentView.Add (smallLabel);
			ContentView.Add (button);
		}
		
		public void UpdateCell (MonkeySpace.Core.Session session, string big, string small)
		{
			this.session = session;
			UpdateImage (AppDelegate.UserData.IsFavorite (session.Id.ToString ()));
			
			bigLabel.Font = big.Length > 35 ? midFont : bigFont;
			bigLabel.Text = big;
			
			smallLabel.Text = small;
		}
		
		void UpdateImage (bool selected)
		{
			if (selected)				
				button.SetImage (favorited, UIControlState.Normal);
			else
				button.SetImage (favorite, UIControlState.Normal);
		}
		
		bool ToggleFavorite ()
		{
			var udata = AppDelegate.UserData;
			if (udata.IsFavorite (session.Code)){
				udata.RemoveFavoriteSession (session.Code);
				return false;
			} else {
				udata.AddFavoriteSession (session.Code);
				return true;
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var full = ContentView.Bounds;
			var bigFrame = full;
			
			bigFrame.Height = 22;
			bigFrame.X = Padding;
			bigFrame.Width -= ImageSpace+Padding;
			bigLabel.Frame = bigFrame;
			
			var smallFrame = full;
			smallFrame.Y = 22;
			smallFrame.Height = 21;
			smallFrame.X = Padding;
			smallFrame.Width = bigFrame.Width;
			smallLabel.Frame = smallFrame;
			
			button.Frame = new RectangleF (full.Width-ImageSpace, -3, ImageSpace, ImageSpace);
		}
	}
	
	// Renders a session
	public class SessionElement : Element {
		static NSString key = new NSString ("sessionElement");

		MonkeySpace.Core.Session session;
		string subtitle;
		
		public SessionElement (MonkeySpace.Core.Session session) : base (session.Title)
		{
			this.session = session;
			if(String.IsNullOrEmpty(session.Location))
				subtitle = String.Format ("{0}", session.GetSpeakerList ());
			else if (String.IsNullOrEmpty(session.GetSpeakerList()))
				subtitle = String.Format("{0}", session.LocationDisplay);
			else
				subtitle = String.Format ("{0}; {1}", session.LocationDisplay, session.GetSpeakerList ());

		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (key);
			if (cell == null)
				cell = new SessionCell (UITableViewCellStyle.Subtitle, key, session, Caption, subtitle);
			else
				((SessionCell)cell).UpdateCell (session, Caption, subtitle);
			
			return cell;
		}

		public override void Selected (DialogViewController dvc, UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
		{
			var svc = new SessionViewController (session);
			dvc.ActivateController (svc);
		}

	}	
}