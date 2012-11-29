
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
	class TweetHelper : Android.Database.Sqlite.SQLiteOpenHelper
	{
		private const string DbName = "TweetDb";
		private const int DbVersion = 1;
		
		public TweetHelper (Context context) : base (context, DbName, null, DbVersion)
		{
			
		}
		
		
		public override void OnCreate (Android.Database.Sqlite.SQLiteDatabase db)
		{
			db.ExecSQL (@"CREATE TABLE IF NOT EXISTS Tweet (" +
			            "TweetID INTEGER PRIMARY KEY AUTOINCREMENT, SOURCE VARCHAR(8000) NOT NULL)");
		}
		
		public override void OnUpgrade (Android.Database.Sqlite.SQLiteDatabase db, int oldVersion, int newVersion)
		{
			db.ExecSQL ("DROP TABLE IF EXISTS Question");
			
			OnCreate (db);
		}

	}
}

