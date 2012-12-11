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

namespace RemoteData
{
	class TweetCommands
	{
		private TweetHelper twtHelp;
		public TweetCommands (Context context)
		{
			twtHelp = new TweetHelper (context);
			twtHelp.OnCreate (twtHelp.WritableDatabase);
		}
		
		public List<Tweet> GetAllTweets ()
		{
			Android.Database.ICursor tCursor = twtHelp.ReadableDatabase.Query ("Tweet", null, null, null, null, null, null, null);
			var tweets = new List<Tweet> ();
			while (tCursor.MoveToNext ())
			{
				Tweet twt = NewTweet (tCursor);
				tweets.Add (twt);
			}
			return tweets;
		}
		
		public long AddTweet (Tweet tweet)
		{
			var values = new ContentValues ();
			values.Put ("Source", tweet.SourceString);
			values.Put ("ID", tweet.ID);
			
			return twtHelp.WritableDatabase.Insert ("Tweet", null, values);
		}
		
		public void DeleteTweet (string tweetID)
		{
			string[] vals = new string[1];
			vals[0] = tweetID;
			
			twtHelp.WritableDatabase.Delete ("Tweet", "Id=?", vals);
		}
		
		private Tweet NewTweet (Android.Database.ICursor cursor)
		{
			var jsonString = JsonValue.Parse (cursor.GetString (1));
			Tweet twt = new Tweet (jsonString);
			
			return (twt);
		}
		
	}
}
