
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
	public class ViewFavTweetsActivity : Activity
	{
		ListView listView;
		MyFavListAdapter listAdapter;
		List<Tweet> allFavTweets;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.fav_list);

			listView = FindViewById<ListView> (Resource.Id.fav_list);
			allFavTweets = new List<Tweet> ();

			try {
				var dbT = new TweetCommands (this);
				allFavTweets = dbT.GetAllTweets ();
			} catch (System.Exception sysExc) {
				Console.Error.WriteLine (String.Format("System.Exception : {0}\n{1}" , sysExc.Message , sysExc.StackTrace));
			}	

			if (allFavTweets.Count > 0) {
				try {
						listAdapter = new MyFavListAdapter (this, allFavTweets);
						listView.Adapter = listAdapter;
						listView.ItemClick += ListClick;

				} catch (WebException we) {
					Console.Error.WriteLine ("WebException : " + we.Message);
				} catch (System.Exception sysExc) {
					Console.Error.WriteLine ("System.Exception : " + sysExc.Message + "\n" + sysExc.StackTrace);
				}	
			} else {
				Toast.MakeText(this, "no tweets in memory", ToastLength.Short).Show();
				//StartActivity(typeof (Activity1));
				Finish ();
			}
		}
		public void ListClick (object sender, AdapterView.ItemClickEventArgs args)
		{
			var alert = new AlertDialog.Builder (this);
			alert.SetTitle("Are You Sure?");
			alert.SetMessage("Do you wish to delete this tweet from favorites?");
			alert.SetPositiveButton("Delete tweet", delegate {
				var dbT = new TweetCommands (this);
				Tweet tw = allFavTweets [args.Position];
				dbT.DeleteTweet (tw.ID);
				StartActivity (typeof (ViewFavTweetsActivity));
				Finish ();
			});
			alert.SetNegativeButton("Cancel", delegate {
				Toast.MakeText(this, "Clicked cancel", ToastLength.Short).Show();
			});
			alert.Show ();
		}
	}
}

