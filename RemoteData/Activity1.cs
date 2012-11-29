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
			listView = FindViewById<ListView>(Resource.Id.lvResult);
			
			user.AfterTextChanged += usernameForRequest;
			button.Click += twitter_DownloadString;
			listView.ItemClick += listView_ItemClick;
		
		}
		void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Toast.MakeText(this, " Clicked!", ToastLength.Short).Show();
		}

		void usernameForRequest (object sender, EventArgs e)
		{
			userName = user.Text;
		}

		private void twitter_DownloadString (object sender, EventArgs e)
		{
			if (userName != null) {
				//TODO count issue
				/* I don't always get the correct count (see ViewTweetsActivity)
				 */
				string Url = "http://api.twitter.com/1/statuses/user_timeline.json?screen_name=" + userName + "&count=5";
				//TODO progressdialog issue
				/*wanted to show a progressdialog while downloading tweets, but i'm getting error messages?
				 */
				//progressDialog = ProgressDialog.Show(this, "Downloading Tweets", "looking for "+userName, true);

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
				//progressDialog.Hide();
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

	}
}

