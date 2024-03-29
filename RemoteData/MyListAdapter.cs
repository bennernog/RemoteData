
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
using Android.Graphics;
using System.Net;
using Java.Util;

namespace RemoteData
{
	class MyListAdapter : BaseAdapter
	{
		Activity myActivity;
		public List<Tweet> Items;
		public MyListAdapter (Activity context, List<Tweet> items) : base ()
		{
			this.myActivity = context;
			this.Items = items;
		}
		public override int Count
		{
			get { return Items.Count; }
		}
		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = (convertView ?? myActivity.LayoutInflater.Inflate(
				Resource.Layout.tweet_display, parent, false)) 
				as LinearLayout;
			var ivProfile = view.FindViewById (Resource.Id.ivProfile) as ImageView;
			var tvName = view.FindViewById (Resource.Id.tvName) as TextView;
			var tvScreenName = view.FindViewById (Resource.Id.tvScreenName) as TextView;
			var tvTweet = view.FindViewById (Resource.Id.tvTweet) as TextView;
			var tvDate = view.FindViewById (Resource.Id.tvDate) as TextView;

			var tweet = Items [position];  
			var tDate = new Date (tweet.StatusDate);

			myActivity.RunOnUiThread (() => ivProfile.SetImageBitmap (tweet.ProfileImage));
			myActivity.RunOnUiThread (() => tvScreenName.Text = "@"+tweet.ScreenName);
			myActivity.RunOnUiThread (() => tvName.Text = tweet.UserName);
			myActivity.RunOnUiThread (() => tvTweet.Text = tweet.StatusText);
			myActivity.RunOnUiThread (() => tvDate.Text = tDate.ToLocaleString ());

			return view;
		}
		public Tweet GetTweet (int position)
		{
			return Items [position];
		}
		public Bitmap GetHeaderInfo ()
		{
			Tweet tw = Items [0];
			return tw.ProfileImage;
		}
	}
}

