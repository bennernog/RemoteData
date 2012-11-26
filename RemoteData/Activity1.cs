using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.ServiceModel;
using System.Linq;
using System.Json;
using System.Xml;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Net;

namespace RemoteData
{
	[Activity (Label = "RemoteData", MainLauncher = true)]
	public class Activity1 : Activity
	{
		Button button;
		TextView tv;
		EditText user;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			tv = FindViewById<TextView> (Resource.Id.tv1);
			user = FindViewById<EditText> (Resource.Id.etUser);
			button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += btn_Click;

		}
		//TODO don't fully understand this
		/* I understand you need to formulate the request and send it to a "location"(url) where the
		 * request is handeled and a response is given.
		 */
		void btn_Click (object sender, EventArgs e)
		{
			/* my biggest issue is to formulate the request how do I know which url to use, according to the twitter developers website the url-request should look like:
			 * https://api.twitter.com/1/statuses/user_timeline.json?include_entities=true&include_rts=true&screen_name=twitterapi&count=2
			 */
			string Url = "http://www.twtmstr.com/webservices/remoteapi.svc/GetUserTimeLine";
			/* So 2 JSON object (which have keys and values)
			 * I tried filling it in with my twitter information, but then nothing happened?
			 * I wanted to try to get the timeline from any user - the username provided by the app-user?
			 */
			System.Json.JsonObject ld = new System.Json.JsonObject() 
			{ { "UserName", "MonoDroidBookEx" },
				{ "PassWord", "MonoDroidIsGreat" },
				{ "AppKey", "blah" } };
			System.Json.JsonObject bd = new System.Json.JsonObject()
			{ { "ld", ld },
				{ "TwitterId", "monodroidbookex"},
				{ "PageIndex", 1 }};
			string Body = bd.ToString (); 
			byte[] byteData = System.Text.UTF8Encoding.UTF8.GetBytes(Body); //this was present in several online examples but is never used & works without?
			try
			{
				//defining the type of request
				HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
				request.ContentLength = Body.Length;
				request.Method = "POST";
				request.ContentType = "text/json";
				//encoding/writing the request
				StreamWriter stOut = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
				stOut.Write(Body);
				stOut.Close(); 
				
				request.BeginGetResponse(new AsyncCallback(ProcessResponse), request);
			}
			catch (WebException we)
			{
				Console.Error.WriteLine("WebException: " + we.Message);
			}
			catch (System.Exception sysExc)
			{
				Console.Error.WriteLine("System.Exception: " + sysExc.Message);
			}		
		}
		void ProcessResponse (IAsyncResult iar)
		{
			//This I pretty much understand
			HttpWebRequest request = (HttpWebRequest)iar.AsyncState;
			HttpWebResponse response;
			response = (HttpWebResponse)request.EndGetResponse (iar);
			Console.Error.WriteLine ("getting response.");
			System.IO.StreamReader strm = new System.IO.StreamReader (response.GetResponseStream ());
			System.Json.JsonArray jsonArray = (System.Json.JsonArray)System.Json.JsonArray.Load (strm);
			var twt = (from jsonTweet in jsonArray
			           select new Tweet
			           {
				ProfileImage = jsonTweet["ProfileImage"].ToString(),
				Status = jsonTweet["Status"].ToString(),
				StatusDate = jsonTweet["StatusDate"],
				StatusId = jsonTweet["StatusId"].ToString(),
				UserName = jsonTweet["UserName"].ToString()
			}).ToList<Tweet> ();
			foreach (Tweet t in twt) {
				RunOnUiThread (() => tv.Text += t.TweetString ()+"\n\n");// not sure why it has to be run on a thread
			}
		}
	}
}


