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
		public delegate void ImageDownloadedHandler (Bitmap bm);
		public static event ImageDownloadedHandler DownloadCompleted;

		public string StatusText { get; set; }
		public string StatusDate { get; set; }
		public string UserName { get; set; }
		public string ScreenName { get; set; }
		public string ProfileImageUrl { get; set; }
		public Bitmap ProfileImage { get; set; }
		public string ID { get; set; }
		public string SourceString { get; set; }

		public Tweet(JsonValue result)
		{
			var text = result ["text"];
			var date = result ["created_at"];
			var userInfo = result ["user"];
			var user = JsonValue.Parse (userInfo.ToString ());
			var name = user ["name"];
			var screen_name = user ["screen_name"];
			var imageUrl = user ["profile_image_url"];
			var id = user ["id"];

			this.SourceString = result.ToString ();
			this.ID = id.ToString ();
			this.StatusText = displayString (text);
			this.StatusDate = displayString (date);
            this.UserName = displayString (name);
           	this.ScreenName = displayString (screen_name);
			this.ProfileImageUrl = displayString (imageUrl);
			DownloadImage (web_DownloadDataCompleted);
		}

		private string displayString (JsonValue input)
		{
			return System.Web.HttpUtility.HtmlDecode (input);
		}

		void DownloadImage (DownloadDataCompletedEventHandler downloadDataCompleted)
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
				if (DownloadCompleted != null) {
					DownloadCompleted (bm);
				}
			}			
			this.ProfileImage = bm;
		}
	}
}