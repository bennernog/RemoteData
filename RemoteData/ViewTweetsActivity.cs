
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Json;
using System.Net;
using Android.Graphics;

namespace RemoteData
{
	[Activity (Theme = "@android:style/Theme.Light.NoTitleBar.Fullscreen", Label = "ViewTweetsActivity")]			
	public class ViewTweetsActivity : Activity
	{
		readonly string SOURCE = "SOURCE";
		string sourceString = "error";
		ImageView ivProfile;
		TextView tvScreenName, tvName;
		ImageButton newSearch, seeFavs;
		ListView listView;
		MyListAdapter listAdapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.list);
			sourceString = Intent.GetStringExtra(SOURCE) ?? "error";

			listView = FindViewById<ListView> (Resource.Id.list);
			ivProfile = FindViewById<ImageView> (Resource.Id.ivProfileH);
			tvName = FindViewById<TextView> (Resource.Id.tvNameH);
			tvScreenName = FindViewById<TextView> (Resource.Id.tvScreenNameH);
			newSearch = FindViewById<ImageButton> (Resource.Id.ibNew);
			seeFavs = FindViewById<ImageButton> (Resource.Id.ibFavs);

			tvName.Typeface = Typeface.CreateFromAsset (this.Assets, "fonts/Greyscale Basic Bold.ttf");
			tvScreenName.Typeface = Typeface.CreateFromAsset (this.Assets, "fonts/Greyscale Basic Regular Italic.ttf");

			newSearch.Click += button_Click;
			seeFavs.Click += button_Click;
			Tweet.DownloadCompleted += HandleDownloadCompleted;

			if (sourceString != null && !sourceString.Equals ("error")) {
				try {

					var result = JsonValue.Parse (sourceString);
					List<Tweet> tweets = new List<Tweet> ();

					for (int i = 0; i<result.Count; i++) {						
						Tweet tweet = new Tweet (result [i]);
						tweets.Add (tweet);
					}

					if (tweets.Count > 0) {
						listAdapter = new MyListAdapter (this, tweets);
						Tweet twt = listAdapter.GetTweet (0);

						RunOnUiThread (() => 
						{
							tvName.Text = twt.UserName;
							tvScreenName.Text = "@"+twt.ScreenName;
						});

						listView.Adapter = listAdapter;
						listView.ItemClick += ListClick;

					}
				} catch (WebException we) {
					Console.Error.WriteLine (String.Format("WebException : {0}" , we.Message));
				} catch (System.Exception sysExc) {
					Console.Error.WriteLine (String.Format("System.Exception : {0}\n{1}" , sysExc.Message , sysExc.StackTrace));
				}	
			} else {
				Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
				StartActivity(typeof (Activity1));
				Finish ();
			}
		}
		void button_Click (object sender, EventArgs e)
		{
			switch (((ImageButton)sender).Id) {
			case Resource.Id.ibNew:
				StartActivity (typeof (Activity1));
				Finish ();
				break;
			case Resource.Id.ibFavs:
				StartActivity (typeof (ViewFavTweetsActivity));
				break;
			default:
				break;
			}
		}
		public void ListClick (object sender, AdapterView.ItemClickEventArgs args)
		{
			Tweet t = listAdapter.GetTweet (args.Position);
			var dbT = new TweetCommands (this);
			dbT.AddTweet (t);
			StartActivity (typeof (ViewFavTweetsActivity));
		}
		void HandleDownloadCompleted (Bitmap bm)
		{
			Tweet.DownloadCompleted -= HandleDownloadCompleted;
			RunOnUiThread (() => ivProfile.SetImageBitmap (bm));
		}
	}
}

