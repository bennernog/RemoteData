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

namespace RemoteData
{
	[Activity (Label = "RemoteData", MainLauncher = true)]
	public class Activity1 : Activity
	{
		Button button;
		TextView tv;
		EditText user;
		string userName;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			
			tv = FindViewById<TextView> (Resource.Id.tv1);
			user = FindViewById<EditText> (Resource.Id.etUser);
			button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += button1_Click;
			user.AfterTextChanged += usernameForRequest;
			
		}

		void usernameForRequest (object sender, EventArgs e)
		{
			userName = user.Text;
		}

		private void button1_Click (object sender, EventArgs e)
		{
			if (userName != null) {
				string Url = "http://api.twitter.com/1/statuses/user_timeline.json?screen_name=" + userName + "&count=5";
				Console.WriteLine (Url);
				WebClient twitter = new WebClient ();
				twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler (twitter_DownloadStringCompleted);
				twitter.DownloadStringAsync (new System.Uri (Url));
			}
		}
		
		void twitter_DownloadStringCompleted (object sender, DownloadStringCompletedEventArgs e)
		{
			//RunOnUiThread (() => tv.Text = (e.Result));
			if (e.Error != null)
				return;
			try {
				//JsonArray jsonArray = new JsonArray (e.Result);

				Console.WriteLine (e.Result);

				var result = JsonValue.Parse (e.Result);

				var first_result = result [0];
				var text = first_result ["text"];

				Console.WriteLine (text.ToString ());
				//System.IO.StreamReader strm = new System.IO.StreamReader (e.Result);
				//System.Json.JsonArray jsonArray = (System.Json.JsonArray)System.Json.JsonArray.Load (strm);
				/*
				var twt = (from jsonTweet in jsonArray
				           select new Tweet
				           {
					ProfileImage = jsonTweet["ProfileImage"].ToString(),
					Status = jsonTweet["Status"].ToString(),
					StatusDate = jsonTweet["StatusDate"],
					StatusId = jsonTweet["StatusId"].ToString(),
					UserName = jsonTweet["UserName"].ToString()
				}).ToList<Tweet> ();
				foreach (Tweet t in twt) {
					RunOnUiThread (() => tv.Text += t.TweetString () + "\n\n");
				}
				*/
			} catch (WebException we) {
				Console.Error.WriteLine ("WebException : " + we.Message);
			} catch (System.Exception sysExc) {
				Console.Error.WriteLine ("System.Exception : " + sysExc.Message);
			}	
			
			XElement xmlTweets = XElement.Parse (e.Result);
			Console.WriteLine (e.Result);
					
			var message = (from tweet in xmlTweets.Descendants ("status")
						               select tweet.Element ("text").Value).FirstOrDefault ();
						
			
			RunOnUiThread (() => tv.Text = message);
		}
	}
}

