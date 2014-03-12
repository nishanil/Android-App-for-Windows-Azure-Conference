using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AzureConf;

namespace MonkeySpace
{
    [Activity(Label = "Session")]
    public class SessionActivity : BaseActivity
    {
        string _code;
        MonkeySpace.Core.Session _session;
        bool isFavourite = false;
        Button _favouriteButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle); // BETTER: http://www.anddev.org/my_own_titlebar_backbutton_like_on_the_iphone-t4591.html
            SetContentView(Resource.Layout.Session);
            //Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.WindowTitle); // http://www.londatiga.net/it/how-to-create-custom-window-title-in-android/

            _favouriteButton = FindViewById<Button>(Resource.Id.FavouriteButton);
            _favouriteButton.Click += new EventHandler(_favouriteButton_Click);

            _code = Intent.GetStringExtra("Code");
            Console.WriteLine("[SessionActivity] " + _code);

			_session = (from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
                       where s.Code == _code
                       select s).FirstOrDefault();

            if (_session.Code != "")
            {
                FindViewById<TextView>(Resource.Id.Title).Text = _session.Title;
				FindViewById<TextView>(Resource.Id.SpeakerList).Text = _session.GetSpeakerList();
                if (_session.Location != "")
					FindViewById<TextView>(Resource.Id.Room).Text = _session.LocationDisplay;
                FindViewById<TextView>(Resource.Id.DateTimeDisplay).Text = _session.DateTimeDisplay;
                FindViewById<TextView>(Resource.Id.Brief).Text = _session.Abstract;
                //FindViewById<TextView>(Resource.Id.TagList).Text = _session.TagList;


                var sess = from s in Conf.Current.FavoriteSessions
                           where s.Code == _session.Code
                           select s;
                foreach (var s in sess)
                {
                    isFavourite = true;
                    break;
                }

                if (isFavourite) _favouriteButton.Text = "Un favorite";
                else _favouriteButton.Text = "Add favorite";
            }

        }

        void _favouriteButton_Click(object sender, EventArgs e)
        {
            isFavourite = !isFavourite;

            if (isFavourite) _favouriteButton.Text = "Un favorite";
            else _favouriteButton.Text = "Add favorite";

            Conf.Current.UpdateFavorite(_session, isFavourite);
        }
    }
}