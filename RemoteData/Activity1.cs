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
			// TODO clean up spaces :)

			btnSearch.Click += twitter_DownloadString;
			btnMinus.Click += ChangeTweetCount;
			btnPlus.Click += ChangeTweetCount;
			btnFavs.Click += (sender, e) => {
				StartActivity (typeof (ViewFavTweetsActivity));
			};
		
		}
		private void ChangeTweetCount (object sender, EventArgs e)
		{
			// TODO this variable assignment makes your code (a little) harder to read while there is no real benefit

			int n = tweetCount;

			// TODO instead of casting us 'as' operator
			/*
			 * From "Effective c#"
			 * The correct choice is to use the as operator whenever you can because it
			 * is safer than blindly casting and is more efficient at runtime.
			 * The as and is operators do not perform any user-defined conversions. 
			 * They succeed only if the runtime type matches the sought type; 
			 * they never construct a new object to satisfy a request.
			 * 
			 */
			var v = (View)sender;

			// TODO include default for your switch
			switch (v.Id) {
			case Resource.Id.btnL:
				if (n > 5) {
					// TODO --n makes your code a little harder to read, and could be confusing while debugging, but works just fine of course. :)
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
			// TODO give variables meaningful names!
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

				progressDialog = ProgressDialog.Show(this, "Downloading Tweets", String.Format("looking for {0}",userName), true);
				string Url = String.Format("http://api.twitter.com/1/statuses/user_timeline.json?screen_name={0}&count={1}",userName, c);
				WebClient twitter = new WebClient ();
				twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler (twitter_DownloadStringCompleted);
				twitter.DownloadStringAsync (new System.Uri (Url));
			} else {
				Toast.MakeText(ApplicationContext, "Enter a twitter name", ToastLength.Short).Show();
			}
		}
		
		void twitter_DownloadStringCompleted (object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Error != null) {
				Console.Error.WriteLine (String.Format ("Error: {0}", e.Error));
				if (progressDialog != null) {
					progressDialog.Dismiss ();
					progressDialog = null;
				}
				RunOnUiThread (() => Toast.MakeText(this, "Something went wrong!\nCheck spelling and try again.", ToastLength.Short).Show());
			} else {
				try {
					string result = e.Result;
					Intent intent = new Intent(this, typeof (ViewTweetsActivity));
					intent.PutExtra(SOURCE, result);
					StartActivity (intent);
					Finish ();
				} catch (WebException we) {
					Console.Error.WriteLine (String.Format("WebException : {0}" , we.Message));
				} catch (System.Exception sysExc) {
					Console.Error.WriteLine (String.Format("System.Exception : {0}\n{1}" , sysExc.Message , sysExc.StackTrace));
				}
			}
		}

		// TODO: methods start with a capital in C#
		// TODO: use meaningful names: 
		/*
		 * newName -> I would expect a new name based on the parameter
		 * IsNewName -> I would expect a bool based on the parameter
		 */
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

