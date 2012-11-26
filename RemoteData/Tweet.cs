using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RemoteData

{
	public class Tweet
	{
		public Tweet()
		{
		}
		public string UserName { get; set; }
		public string ProfileImage { get; set; }
		public string Status { get; set; }
		public string StatusId { get; set; }
		public string StatusDate { get; set; }

		public string TweetString (){
			string u = UserName;
			string s = Status;
			string d = StatusDate;

			return String.Format("{0} - {1} - {2}",u, s, d);
		}
	}
}