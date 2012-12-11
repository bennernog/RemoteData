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
	class MyFavListAdapter : BaseAdapter
	{
		Activity myActivity;

		public List<Tweet> Items { get; set; }
		
		// TODO: add spacing
		/*like this?
		 */ 
		public MyFavListAdapter (Activity context, List<Tweet> items) : base ()
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
			// TODO: you can also use
			// var ivProfile = view.FindViewById<ImageView> (Resource.Id.ivProfile);
			/* that's the first thing I tried, but it doesn't work, I got errors then
			 * var ivProfile = (ImageView) view.FindViewById (Resource.Id.ivProfile); also works
			 */
			var ivProfile = view.FindViewById (Resource.Id.ivProfile) as ImageView;
			var tvName = view.FindViewById (Resource.Id.tvName) as TextView;
			var tvScreenName = view.FindViewById (Resource.Id.tvScreenName) as TextView;
			var tvTweet = view.FindViewById (Resource.Id.tvTweet) as TextView;
			var tvDate = view.FindViewById (Resource.Id.tvDate) as TextView;
			
			tvName.Typeface = Typeface.CreateFromAsset (myActivity.Assets, "fonts/Greyscale Basic Bold.ttf");
			tvScreenName.Typeface = Typeface.CreateFromAsset (myActivity.Assets, "fonts/Greyscale Basic Regular Italic.ttf");
			
			var tweet = Items [position];  
			var tDate = new Date (tweet.StatusDate);
			
			myActivity.RunOnUiThread (() => 
			                          {
				ivProfile.SetImageBitmap (tweet.ProfileImage);
				// TODO: use String.Format
				/* Why? "@"+string works just fine, no?
				 */
				tvScreenName.Text = String.Format("@{0}", tweet.ScreenName);
				tvName.Text = tweet.UserName;
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
