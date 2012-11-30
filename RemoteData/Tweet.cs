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
			downloadImage (displayString (imageUrl));
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
				bm = BitmapFactory.DecodeByteArray(e.Result, 0, e.Result.Length);							
			}
			
			this.ProfileImage = bm;
		}
	}
}