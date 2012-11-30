
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
		ListView listView;
		MyListAdapter listAdapter;
		Bitmap pic;
		ImageView ivProfile;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.list);
			sourceString = Intent.GetStringExtra(SOURCE) ?? "error";
			listView = FindViewById<ListView> (Resource.Id.list);
			Console.WriteLine (sourceString);
			if (sourceString != null && !sourceString.Equals ("error")) {
				try {

					var result = JsonValue.Parse (sourceString);
					List<Tweet> tweets = new List<Tweet> ();
					//TODO COUNT issue
					/* result.Count is not always 5?
				 	 */
					Console.WriteLine (result.Count.ToString ());

					for (int i = 0; i<result.Count; i++) {
						
						Tweet tweet = new Tweet (result [i]);
						tweets.Add (tweet);
					}
					if (tweets.Count > 0) {
						//TODO HEADER issue
						/* Can't get the image to display
						 * I tried  getting it while creating the list of tweets (foreach just above this)
						 * I tried calling a tweet from the list and getting the pic from the tweet
						 * tried it but first calling a tweet from the adapter
						 * I tried calling the pic from the listadapter as a paramater and as a custom Get-method
						 * The name/screen_name always work, but not the image???
						 */
						listAdapter = new MyListAdapter (this, tweets);
						Tweet twt = listAdapter.GetTweet (1);
						pic = twt.ProfileImage;
						if (pic == null) Console.WriteLine ("No Pic");
						ivProfile = FindViewById<ImageView> (Resource.Id.ivProfileH);
						var tvName = FindViewById<TextView> (Resource.Id.tvNameH);
						var tvScreenName = FindViewById<TextView> (Resource.Id.tvScreenNameH);

						// TODO group RunOnUiThread
						RunOnUiThread (() => tvName.Text = twt.UserName);
						RunOnUiThread (() => tvScreenName.Text = "@"+twt.ScreenName);
						RunOnUiThread (() => ivProfile.SetImageBitmap (pic));
						downloadImage (twt.ProfileImageUrl);
						listView.Adapter = listAdapter;
						
						listView.ItemClick += ListClick;

					}
				} catch (WebException we) {
					// TODO: use String.Format :)
					Console.Error.WriteLine ("WebException : " + we.Message);
				} catch (System.Exception sysExc) {
					// TODO: use String.Format :)
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
			//TODO COUNT issue related?
			/*position sometimes starts with 0 sometimes with 1
			 * perhaps related to the count issue
			 */
			int i = (int) listAdapter.GetItemId (args.Position);
			Toast.MakeText(this, "clicked "+i.ToString (), ToastLength.Short).Show();
			Tweet t = listAdapter.GetTweet (i);
			var dbT = new TweetCommands (this);
			dbT.AddTweet (t);

			StartActivity(typeof (ViewFavTweetsActivity));
			Finish ();

		}
		private void downloadImage (string imageUrl)
		{
			
			WebClient web = new WebClient();
			web.DownloadDataAsync(new System.Uri (imageUrl));
			web.DownloadDataCompleted += new DownloadDataCompletedEventHandler(web_DownloadDataCompleted);
			
		}
		void web_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			Bitmap bm;
			if (e.Error != null) {
				return;
			} else {	
				var ivProfile = FindViewById<ImageView> (Resource.Id.ivProfileH);
				bm = BitmapFactory.DecodeByteArray(e.Result, 0, e.Result.Length);	
				RunOnUiThread (() => ivProfile.SetImageBitmap (bm));
			}
			
			
		}
	}
}

