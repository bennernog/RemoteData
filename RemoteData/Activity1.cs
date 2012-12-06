using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.ServiceModel;
using System.Linq;
using System.Json;
using System.Xml;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Net;
using Android.Graphics;
using System.Threading;
using Java.IO;

namespace RemoteData
{
	[Activity (Theme = "@android:style/Theme.Light.NoTitleBar.Fullscreen", Label = "RemoteData", MainLauncher = true)]
	public class Activity1 : Activity
	{
		ImageView image;
		AutoCompleteTextView user;
		ImageButton btnSearch, btnFavs;
		Button btnMinus, btnPlus;
		TextView tvTweetCount;

		ProgressDialog progressDialog;

		ISharedPreferences prefs;
		ISharedPreferencesEditor editor;
		bool firstTime;
		readonly string NAMES = "AUTOCOMPLETE_NAMES";
		readonly string FIRST = "FIRST";
		readonly string SOURCE = "SOURCE";
		readonly string COUNT = "COUNT";
		string userName, nms;
		string[] nameArray;
		int tweetCount;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			prefs = GetPreferences (FileCreationMode.Append);
			firstTime = prefs.GetBoolean (FIRST, true);
			tweetCount = prefs.GetInt (COUNT, 5);
			nms = prefs.GetString (NAMES, "flowpilots°");
			nameArray = AutocompleteNames (nms);
			
			image = FindViewById<ImageView> (Resource.Id.iv);
			user = FindViewById<AutoCompleteTextView> (Resource.Id.etUser);
			btnSearch = FindViewById<ImageButton> (Resource.Id.myButton);
			btnFavs = FindViewById<ImageButton> (Resource.Id.btnR);
			btnMinus = FindViewById<Button> (Resource.Id.btnL);
			btnPlus = FindViewById<Button> (Resource.Id.btnM);
			tvTweetCount = FindViewById<TextView> (Resource.Id.tv1);

			user.Typeface = Typeface.CreateFromAsset (this.Assets, "fonts/Greyscale Basic Regular Italic.ttf");
			tvTweetCount.Typeface = Typeface.CreateFromAsset (this.Assets, "fonts/Greyscale Basic Bold.ttf");
			tvTweetCount.Text = String.Format ("{0}",tweetCount);

			if (firstTime) {
				editor = prefs.Edit ();
				editor.PutBoolean (FIRST, false);
				editor.Commit ();
			} else {
				ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, nameArray);
				user.Adapter = autoCompleteAdapter;
			}


			btnSearch.Click += twitter_DownloadString;
			btnMinus.Click += ChangeTweetCount;
			btnPlus.Click += ChangeTweetCount;
			btnFavs.Click += (sender, e) => {
				StartActivity (typeof (ViewFavTweetsActivity));
			};
		
		}
		private void ChangeTweetCount (object sender, EventArgs e)
		{
			int n = tweetCount;
			var v = (View)sender;

			switch (v.Id) {
			case Resource.Id.btnL:
				if (n > 5) {
					tvTweetCount.Text = String.Format ("{0}",--n);
				}
				break;
			case Resource.Id.btnM:
				if (n < 50) {
					tvTweetCount.Text = String.Format ("{0}",++n);
				}
				break;
			}
			tweetCount = n;
		}
		string[] AutocompleteNames (string allNames)
		{
			string[] autoCompleteOptions = allNames.Split ('°');
			return autoCompleteOptions;
		}
		private void twitter_DownloadString (object sender, EventArgs e)
		{
			var c = tvTweetCount.Text; 
			userName = user.Text;
			if (userName.Length >0) {
				editor = prefs.Edit ();
				if (newName (userName)){
					string newNms = String.Format("{0}{1}°", nms, userName);
					editor.PutString (NAMES, newNms);
				}
				editor.PutInt (COUNT, tweetCount);
				editor.Commit ();
				string Url = String.Format("http://api.twitter.com/1/statuses/user_timeline.json?screen_name={0}&count={1}",userName, c);
				progressDialog = ProgressDialog.Show(this, "Downloading Tweets", String.Format("looking for {0}",userName), true);
				WebClient twitter = new WebClient ();
				twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler (twitter_DownloadStringCompleted);
				twitter.DownloadStringAsync (new System.Uri (Url));
			} else {
				Toast.MakeText(ApplicationContext, "Enter twitter Name", ToastLength.Short).Show();
			}
		}
		
		void twitter_DownloadStringCompleted (object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Error == null) {
				try {
					string result = e.Result;
					Intent intent = new Intent(this, typeof (ViewTweetsActivity));
					intent.PutExtra(SOURCE, result);
					StartActivity (intent);
					Finish ();
				} catch (WebException we) {
					Console.Error.WriteLine (String.Format("WebException : {0}" , we.Message));
				} catch (System.Exception sysExc) {
					Console.Error.WriteLine (String.Format("System.Exception : {0}\n{1}" , sysExc.Message , e.Error));//10484778

				}
			} else {
				Console.Error.WriteLine (String.Format ("Error: {0}", e.Error));
				if (progressDialog != null) {
					progressDialog.Dismiss ();
					progressDialog = null;
				}
				RunOnUiThread (() => {
					user.Text = "";
					Toast.MakeText(this, "Enter correct twitter Name", ToastLength.Short).Show();
				});
			}
		}

		bool newName (string name)
		{
			return !nameArray.Contains (name);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			
			if (progressDialog != null) {
				progressDialog.Dismiss ();
				progressDialog = null;
			}
		}
	}
}

