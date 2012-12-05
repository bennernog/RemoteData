
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
				Resource.Layout.list_tweet, parent, false)) 
				as LinearLayout;
			var tvTweet = view.FindViewById (Resource.Id.tvTweet) as TextView;
			var tvDate = view.FindViewById (Resource.Id.tvDate) as TextView;

			tvDate.Typeface = Typeface.CreateFromAsset (myActivity.Assets, "fonts/Greyscale Basic Regular Italic.ttf");

			var tweet = Items [position];  
			var tDate = new Date (tweet.StatusDate);

			myActivity.RunOnUiThread (() => 
			                          {
				tvTweet.Text = tweet.StatusText;
				tvDate.Text = tDate.ToLocaleString ();
			});

			return view;
		}
		public Tweet GetTweet (int position)
		{
			return Items [position];
		}
	}
}

