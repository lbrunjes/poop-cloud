using System;
using System.Data.SQLite;

namespace BabyData.Data
{
	public class SqliteWrapper:IBabyDataSource
	{
		#region sql strings
		private static readonly string READ_BABY =@"";
		private static readonly string SAVE_BABY =@"";
		private static readonly string CREATE_BABY =@"";

		private static readonly string READ_BABYEVENT =@"";
		private static readonly string SAVE_BABYEVENT =@"";
		private static readonly string CREATE_BABYEVENT =@"";

		private static readonly string READ_PERMISSION =@"";
		private static readonly string SAVE_PERMISSION =@"";
		private static readonly string CREATE_PERMISSION =@"";

		private static readonly string READ_USER =@"";
		private static readonly string SAVE_USER =@"";
		private static readonly string CREATE_USER =@"";

		private static readonly string VERFIY_DB =@"";
		private static readonly string CREATE_DB =@"";



		#endregion 
		private System.Data.SQLite.SQLiteConnection db =null;

		public SqliteWrapper(string connectionString){
			db = new SQLiteConnection (connectionString);

			//run VERFIY_DB 
			//if ^ fails run CREATE_DB
		}

		public void Dispose(){
			if (this.db != null) {
				this.db.Dispose ();
			}
		}

		#region baby CRUD
		public Baby ReadBaby (string Id, User user){

			Baby b = new Baby ();

			SQLiteCommand cmd = new SQLiteCommand (READ_BABY, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			SQLiteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				//TODO read into b
			}

			return b;
		}
		public bool SaveBaby (Baby baby, User user){

			SQLiteCommand cmd = new SQLiteCommand (SAVE_BABY, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}
		public Baby CreateBaby (Baby baby, User user){
	
			Baby b = baby;

			SQLiteCommand cmd = new SQLiteCommand (CREATE_BABY, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			//TODO GET NEXT BABYID And assign to baby b

			bool saved = SaveBaby (b, user);
			if (!saved) {
				//panic
			}
		

			return b;
		}
		#endregion

		#region baby events
		public BabyEvent ReadBabyEvent (string Id, User user){
			BabyEvent be = new BabyEvent ();

			SQLiteCommand cmd = new SQLiteCommand (READ_BABYEVENT, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			SQLiteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				//TODO read into be
			}

			return be;
		}
		public bool SaveBabyEvent (BabyEvent babyevent, User user){
			SQLiteCommand cmd = new SQLiteCommand (SAVE_BABYEVENT, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}
		public BabyEvent CreateBabyEvent (BabyEvent babyevent, User user){
			BabyEvent be = babyevent;

			SQLiteCommand cmd = new SQLiteCommand (CREATE_BABYEVENT, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			//TODO GET NEXT BABYID And assign to babyevent be

			bool saved = SaveBabyEvent (be, user);
			if (!saved) {
				//panic
			}


			return be;
		}
		#endregion

		#region permissions
		public Permission ReadPermission (string Id, User user){
			Permission p = new Permission ();

			SQLiteCommand cmd = new SQLiteCommand (READ_PERMISSION, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			SQLiteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				//TODO read into p
			}

			return p;
		}
		public bool SavePermission (Permission permission, User user){
			SQLiteCommand cmd = new SQLiteCommand (SAVE_PERMISSION, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}
		public Permission CreatePermission (Permission permission, User user){
			Permission p = permission;

			SQLiteCommand cmd = new SQLiteCommand (CREATE_PERMISSION, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			//TODO GET NEXT BABYID And assign to permission p

			bool saved = SavePermission (p, user);
			if (!saved) {
				//panic
			}

			return p;
		}
		#endregion


		#region users
		public User ReadUser (string username){
			User u = new User ();

			SQLiteCommand cmd = new SQLiteCommand (READ_USER, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			SQLiteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				//TODO read into u
			}

			return u;
		}
		public bool SaveUser (User target, User user){
			SQLiteCommand cmd = new SQLiteCommand (SAVE_USER, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			//TODO ADD  PARAMS
			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}
		public User CreateUser (User target, User user){
			User u = new User();

			SQLiteCommand cmd = new SQLiteCommand (CREATE_USER, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			//TODO GET NEXT ID And assign to user u

			bool saved = SaveUser (u, user);
			if (!saved) {
				//panic
			}


			return u;
		}
		#endregion
	}
}

