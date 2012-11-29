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

namespace RemoteData
{
	[Activity (Theme = "@android:style/Theme.Light.NoTitleBar.Fullscreen", Label = "RemoteData", MainLauncher = true)]
	public class Activity1 : Activity
	{
		ImageView image;
		EditText user;
		Button button;
		TextView tv;
		ListView listView;

		ProgressDialog progressDialog;

		readonly string SOURCE = "SOURCE";
		string userName;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			
			image = FindViewById<ImageView> (Resource.Id.iv);
			user = FindViewById<EditText> (Resource.Id.etUser);
			button = FindViewById<Button> (Resource.Id.myButton);
			tv = FindViewById<TextView> (Resource.Id.tv1);
			user.Text = "janvdp";
			listView = FindViewById<ListView>(Resource.Id.lvResult);

			// TODO: Why is this handler present?
			// You can just retrieve the user.Text value when the user clicks the button
			// Could be something you're trying out of course.
			//user.AfterTextChanged += usernameForRequest;
			button.Click += twitter_DownloadString;
			listView.ItemClick += listView_ItemClick;
		}
		void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Toast.MakeText(this, " Clicked!", ToastLength.Short).Show();
		}

		/*
		void usernameForRequest (object sender, EventArgs e)
		{


			userName = user.Text;
		}
		*/

		private void twitter_DownloadString (object sender, EventArgs e)
		{
			// You can get the user name when the user clicks the button
			userName = user.Text;
			if (userName != null) {
				//TODO count issue
				/* I don't always get the correct count (see ViewTweetsActivity)
				 * 
				 * -> This appears to be an issue with the Twitter API, although I could not find a real explanation for this.
				 * 
				 */
				string Url = "http://api.twitter.com/1/statuses/user_timeline.json?screen_name=" + userName + "&count=5";
				//TODO progressdialog issue
				/*wanted to show a progressdialog while downloading tweets, but i'm getting error messages?
				 */

				// TODO Use String.Format for building strings!
				progressDialog = ProgressDialog.Show(this, "Downloading Tweets", "looking for "+userName, true);

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

				RunOnUiThread (() => {
					progressDialog.Hide();
				});

				string result = e.Result;
				Console.WriteLine (result);
				Intent intent = new Intent(this, typeof (ViewTweetsActivity));
				intent.PutExtra(SOURCE, result);
				StartActivity (intent);
				Finish ();
			
			} catch (WebException we) {
				Console.Error.WriteLine ("WebException : " + we.Message);
			} catch (System.Exception sysExc) {
				Console.Error.WriteLine ("System.Exception : " + sysExc.Message + "\n" +sysExc.StackTrace);
			}	
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// TODO the error is caused by the progressDialog that is still present after the Activity has Paused
			// http://stackoverflow.com/questions/2850573/activity-has-leaked-window-that-was-originally-added
			if (progressDialog != null) {
				progressDialog.Dismiss ();
				progressDialog = null;
			}
		}
	}
}

