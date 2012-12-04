using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Net;

namespace RemoteData

{
	public class Tweet
	{

		// TODO event
		// add a delegate here (takes a parameter Bitmap)
		public delegate void ImageDownloadedHandler (Bitmap bm);
		// add a static event here of the delegate type defined in the previous line
		public static event ImageDownloadedHandler DownloadCompleted;
		// This way other classes can easily subscribe to a profile picture downloaded event.
		// It's basically an alternate way of doing things.

		public string StatusText { get; set; }
		public string StatusDate { get; set; }
		public string UserName { get; set; }
		public string ScreenName { get; set; }
		public string ProfileImageUrl { get; set; }
		public Bitmap ProfileImage { get; set; }
		public string ID { get; set; }
		public string SourceString { get; set; }

		public Tweet(JsonValue source)
		{
			this.SourceString = source.ToString ();
			this.ID = GetID (source).ToString ();
			var result = source;
			var text = result ["text"];
			var date = result ["created_at"];
			var userInfo = result ["user"];

			var user = JsonValue.Parse (userInfo.ToString ());
			var name = user ["name"];
			var screen_name = user ["screen_name"];
			var imageUrl = user ["profile_image_url"];

			this.StatusText = displayString (text);
			this.StatusDate = displayString (date);
            this.UserName = displayString (name);
           	this.ScreenName = displayString (screen_name);
			this.ProfileImageUrl = displayString (imageUrl);
			DownloadImage (web_DownloadDataCompleted);
		}
		private int GetID (JsonValue source)
		{
			int i = source.ToString ().Length;
			var text = source ["text"];
			string s =text.ToString ();

			return i + s.Length;

		}
		private string displayString (JsonValue input)
		{
			string s = input.ToString ();
			int l = s.Length-2;
			string output = s.Substring(1, l);
			return output;
		}
		public void DownloadImage (DownloadDataCompletedEventHandler downloadDataCompleted)
		{

			WebClient web = new WebClient();
			web.DownloadDataAsync(new System.Uri (ProfileImageUrl));
			web.DownloadDataCompleted += new DownloadDataCompletedEventHandler(downloadDataCompleted);

		}
		void web_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			Bitmap bm;
			if (e.Error != null) {
				return;
			} else {				
				bm = BitmapFactory.DecodeByteArray(e.Result, 0, e.Result.Length);	
				// TODO event
				// check whether the event (defined above) is not null
				// if not null, fire the event with bm as parameter
				if (DownloadCompleted != null) {
					DownloadCompleted (bm);
				}
			}
			
			this.ProfileImage = bm;
		}
	}
}