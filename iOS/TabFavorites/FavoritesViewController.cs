using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Diagnostics;

namespace Monospace11
{
    [Register]
    public class FavoritesViewController : DialogViewController
    {
		public FavoritesViewController () : base (UITableViewStyle.Plain, null)
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			Root = GenerateRoot ();
			
			Debug.WriteLine ("Summary " + Root.Summary() );

			// SLIDEOUT BUTTON
			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.Current.FlyoutNavigation.ToggleMenu();
			});
			NavigationItem.SetLeftBarButtonItem (bbi, false);

			TableView.BackgroundView = new UIImageView (UIImage.FromBundle ("Images/Background"));
		}
		
		RootElement GenerateRoot ()
		{
			var favs = AppDelegate.UserData.GetFavoriteCodes();
			var root = 	new CustomRootElement ("Favorites") {
				from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList () //AppDelegate.ConferenceData.Sessions
							where favs.Contains(s.Code )
							group s by s.Start.Ticks into g
							orderby g.Key
							select new Section (HomeViewController.MakeCaption ("", new DateTime (g.Key))) {
							from hs in g
							   select (Element) new SessionElement (hs)
			}};	
			
			if(favs.Count == 0)
			{
				var section = new Section("Whoops, Star a few sessions first!");
				root.Add(section);
			}
			return root;
        }
    }
}
