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
		ImageButton button;
		TextView tv;

		ProgressDialog progressDialog;

		ISharedPreferences prefs;
		ISharedPreferencesEditor editor;
		bool firstTime;
		readonly string NAMES = "AUTOCOMPLETE_NAMES";
		readonly string FIRST = "FIRST";
		readonly string SOURCE = "SOURCE";
		string userName, nms;
		string[] nameArray;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			prefs = GetPreferences (FileCreationMode.Append);
			firstTime = prefs.GetBoolean (FIRST, true);
			nms = prefs.GetString (NAMES, "flowpilots°");
			nameArray = AutocompleteNames (nms);

			//Stream input = Assets.Open ("AboutAssets.txt");  
			//Typeface tf = Typeface.CreateFromAsset (Context.Assets, "fonts/Greyscale Basic Regular.ttf");
			
			image = FindViewById<ImageView> (Resource.Id.iv);
			user = FindViewById<AutoCompleteTextView> (Resource.Id.etUser);
			button = FindViewById<ImageButton> (Resource.Id.myButton);
			tv = FindViewById<TextView> (Resource.Id.tv1);

			user.Typeface = Typeface.CreateFromAsset (this.Assets, "fonts/Greyscale Basic Regular.ttf");

			if (firstTime) {
				editor = prefs.Edit ();
				editor.PutBoolean (FIRST, false);
				editor.Commit ();
			} else {
				ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, nameArray);
				user.Adapter = autoCompleteAdapter;
			}


			button.Click += twitter_DownloadString;
		
		}
		string[] AutocompleteNames (string allNames)
		{
			string[] autoCompleteOptions = allNames.Split ('°');
			return autoCompleteOptions;
		}
		private void twitter_DownloadString (object sender, EventArgs e)
		{
			userName = user.Text;
			if (userName != null) {
				if (newName (userName)){
					string newNms = String.Format("{0}{1}°", nms, userName);
					editor = prefs.Edit ();
					editor.PutString (NAMES, newNms);
					editor.Commit ();
				}

				string Url = String.Format("http://api.twitter.com/1/statuses/user_timeline.json?screen_name={0}&count=5",userName);
				progressDialog = ProgressDialog.Show(this, "Downloading Tweets", String.Format("looking for {0}",userName), true);
				WebClient twitter = new WebClient ();
				twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler (twitter_DownloadStringCompleted);
				twitter.DownloadStringAsync (new System.Uri (Url));
			} else {
				Toast.MakeText(this, "Enter twitter Name", ToastLength.Short).Show();
			}
		}
		
		void twitter_DownloadStringCompleted (object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Error != null)
				return;
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

