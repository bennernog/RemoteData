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
			amper (s);
			int l = s.Length-2;
			string output = s.Substring(1, l);
			System.Web.HttpUtility.HtmlDecode (output);
			return /*amper (*/output/*)*/;
		}
		private string amper (string input)
		{
			string[] txt;
			int x;
			string result = null;
			if (input.Contains ("&")) {
				txt = input.Split ('&');
				x = txt.Length;
				if (x>0){
					result = txt [0];
					for (int i = 1 ; i < x ; i++) {
						var temp = txt [i];
						result += reformat (temp);
					}
				} 
			}
			return (result != null) ? result : input;
		}
		string reformat (string input)
		{
			int index = input.IndexOf (';') + 1;
			string s = null;
			if (input.StartsWith ("amp")) {
				s = "&";
			} else if (input.StartsWith ("lt")) {
				s = "<";
			} else if (input.StartsWith ("gt")) {
				s = ">";
			}
			return String.Format ("{0}{1}", s, input.Substring (index));
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