
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
				Console.Error.WriteLine ("System.Exception : " + sysExc.Message + "\n" + sysExc.StackTrace);
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
				Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
				StartActivity(typeof (Activity1));
				Finish ();
			}
		}
		public void ListClick (object sender, AdapterView.ItemClickEventArgs args)
		{
			//TODO delete tweet from db (no questions for now)
			int i = (int)listAdapter.GetItemId (args.Position);
			Toast.MakeText(this, "clicked "+i.ToString (), ToastLength.Short).Show();		

			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle("Alert!");
			alert.SetMessage("This is the content of the alert");
			alert.SetPositiveButton("Delete tweet", delegate {
				var dbT = new TweetCommands (this);
				Tweet tw = allFavTweets [args.Position];
				dbT.DeleteTweet (tw.ID);
				Toast.MakeText(this, "Clicked ok", ToastLength.Short).Show();
			});
			alert.SetNeutralButton("New Search", delegate {
				StartActivity (typeof (Activity1));
				Finish ();
			});
			alert.SetNegativeButton("Cancel", delegate {
				Toast.MakeText(this, "Clicked cancel", ToastLength.Short).Show();
			});
			alert.Show ();
		}
	}
}

