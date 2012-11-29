
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
using System.Json;
using System.Net;
using Android.Graphics;

namespace RemoteData
{
	[Activity (Theme = "@android:style/Theme.Light.NoTitleBar.Fullscreen", Label = "ViewTweetsActivity")]			
	public class ViewTweetsActivity : Activity
	{
		readonly string SOURCE = "SOURCE";
		string sourceString = "error";
		ListView listView;
		MyListAdapter listAdapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.list);
			sourceString = Intent.GetStringExtra(SOURCE) ?? "error";
			listView = FindViewById<ListView> (Resource.Id.list);
			Console.WriteLine (sourceString);
			if (sourceString != null && !sourceString.Equals ("error")) {
				try {

					var result = JsonValue.Parse (sourceString);
					List<Tweet> tweets = new List<Tweet> ();
					//TODO count issue continued
					/* result.Count is not always 5?
				 	 */
					Console.WriteLine (result.Count.ToString ());

					for (int i = 0; i<result.Count; i++) {
						
						Tweet tweet = new Tweet (result [i]);
						tweets.Add (tweet);
					}
					if (tweets.Count > 0) {
						listAdapter = new MyListAdapter (this, tweets);
						//TODO Header issue
						/* I wanted to make a static header, so I only have to load the profileimage and name once
						 * But i can't get it to show anything and it scrolls with rest of the list.
						 */
						Bitmap bm = listAdapter.GetHeaderInfo ();
						ViewGroup vg = FindViewById<LinearLayout> (Resource.Id.mainll);
						View v = this.LayoutInflater.Inflate (Resource.Layout.list_header, vg, false)as LinearLayout;
						ImageView image = v.FindViewById (Resource.Id.imageView1) as ImageView;
						image.SetImageBitmap (bm);
						listView.AddHeaderView (v);
						listView.Adapter = listAdapter;

						listView.ItemClick += ListClick;

					}
				} catch (WebException we) {
					Console.Error.WriteLine ("WebException : " + we.Message);
				} catch (System.Exception sysExc) {
					Console.Error.WriteLine ("System.Exception : " + sysExc.Message + "\n" + sysExc.StackTrace);
				}	
			} else {
				Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
				StartActivity(typeof (Activity1));
				Finish ();
			}
		}
		public void ListClick (object sender, AdapterView.ItemClickEventArgs args)
		{
			int i = (int) listAdapter.GetItemId (args.Position);
			Toast.MakeText(this, "clicked "+i.ToString (), ToastLength.Short).Show();
			Tweet t = listAdapter.GetTweet (i-1);
			var dbT = new TweetCommands (this);
			dbT.AddTweet (t);

			StartActivity(typeof (ViewFavTweetsActivity));
			Finish ();

		}
	}
}

