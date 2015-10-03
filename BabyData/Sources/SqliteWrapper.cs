using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Globalization;


namespace BabyData.Data
{
	public class SqliteWrapper:IBabyDataSource
	{
		#region sql strings
		private static readonly string READ_BABY =@"SELECT

Name,
Image,
Sex,
DateOfBirth,
IsPublic
		
FROM Baby 
WHERE Id=@id";

		private static readonly string READ_BABY_PERMISSIONS =@"SELECT 
Id
Type,
Username,
BabyId,
Added
FROM Permission 
WHERE BabyId =@id
ORDER BY Added ASC
";
		private static readonly string READ_BABY_EVENTS =@"SELECT 
Id
Type,
Username,
BabyId,
Event,
Details
Reported
FROM BabyEvents 
WHERE BabyId =@id
ORDER BY Reported DESC";
		
		private static readonly string SAVE_BABY =@"UPDATE Baby
SET
Name =@name,
Image = @image,
Sex = @sex,
DateOfBirth =@dateofbirth,
IsPublic = @ispublic
		
WHERE Id=@id";
		private static readonly string CREATE_BABY =@"INSERT INTO Baby
(Name,	Image,	Sex,	DateOfBirth,	IsPublic,	Id)
VALUES
(@name,	@image,	@sex,	@dateofbirth,	@ispublic,	@id)

		";

		private static readonly string READ_BABYEVENT =@"SELECT 
Id,
Username,
BabyId,
Event,
Reported,
Details
FROM BabyEvents WHERE Id =@id";
		private static readonly string SAVE_BABYEVENT =@"UPDATE BabyEvent 
SET


Username = @username,
BabyId = @babyid,
Event = @event,
Reported = @reported,
Details = @dteails

WHERE Id =@id;";
		private static readonly string CREATE_BABYEVENT =@"INSERT INTO BabyEvent
(	Username,	BabyId, 	Event,	Reported, 	Details)
VALUES
(	@username,	@babyId,	@event,	@reported,	@details)
		
";

		private static readonly string READ_PERMISSION =@"SELECT 
Type,
Username,
BabyId,
Added
FROM Permission WHERE Id =@id";
		private static readonly string SAVE_PERMISSION =@"";
		private static readonly string CREATE_PERMISSION =@"INSERT INTO Permission
(Type,	Username,	BabyId,	Added)
VALUES
(@type,	@username,	@babyid,	@added)";

		private static readonly string READ_USER =@"SELECT

Email,
Salt,
Hash,
Image,
Role,
Flag,
Joined
		
FROM User 
WHERE Username=@username";
		private static readonly string SAVE_USER =@"UPDATE User
SET

Email = @email,
Salt = @salt,
Hash = @hash,
Image = @image,
Role = @role,
Flag = @flag

WHERE
Username=@username;";
		private static readonly string CREATE_USER =@"INSERT INTO User
(Username,	Email,	Salt,	Hash,	Image,	Role,	Flag, 	Joined)
VALUES
(@username,	@email,	@salt,	@hash,	@image,	@role,	@flag,	@joined)";

		private static readonly string VERFIY_DB =@"";
		private static readonly string CREATE_DB =@"";

		private static readonly int BABY_ID_LENGTH = 16;

		#endregion 
		private SqliteConnection db =null;

		public SqliteWrapper(string connectionString){
			db = new SqliteConnection (connectionString);

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

			SqliteCommand cmd = new SqliteCommand (READ_BABY, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@Id", Id);

			SqliteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				b.Id = Id;
				b.DOB =  DateTime.ParseExact(r ["DateOfBirth"].ToString (), 
					"yyyy-MM-dd HH:mm:ss zzz",
					CultureInfo.InvariantCulture );;
				b.Name = r ["Name"].ToString ();
				b.Image = r ["Image"].ToString ();
				b.Sex = r ["Sex"].ToString ();
				b.IsPublic = r ["IsPublic"].ToString () == "Y";
			}
			r.Close();

			//only get teh full baby details if we really want it.
			return b;
		}
		public bool SaveBaby (Baby baby, User user){

			SqliteCommand cmd = new SqliteCommand (SAVE_BABY, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@Id", baby.Id);
			cmd.Parameters.AddWithValue("@name",baby.Name);
			cmd.Parameters.AddWithValue("@image",baby.Image);
			cmd.Parameters.AddWithValue("@sex",baby.Sex);
			cmd.Parameters.AddWithValue("@dateofbirth",baby.DOB.ToString("yyyy-MM-dd HH:mm:ss zzz"));
			cmd.Parameters.AddWithValue("@ispublic",baby.IsPublic);

			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}
		public Baby CreateBaby (Baby baby, User user){
	
			Baby b = baby;

			SqliteCommand cmd = new SqliteCommand (CREATE_BABY, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			if (String.IsNullOrEmpty (baby.Id)) {
				Random r = new Random ();

				//TODO Collision Checks
				byte[] id =new byte[BABY_ID_LENGTH];
				r.NextBytes (id);
				cmd.Parameters.AddWithValue ("@Id", Convert.ToBase64String(id));
				b.Id = Convert.ToBase64String (id);
			} else {
				cmd.Parameters.AddWithValue ("@Id", baby.Id);
			}
			cmd.Parameters.AddWithValue("@name",b.Name);
			cmd.Parameters.AddWithValue("@image",b.Image);
			cmd.Parameters.AddWithValue("@sex",b.Sex);
			cmd.Parameters.AddWithValue("@dateofbirth",b.DOB.ToString("yyyy-MM-dd HH:mm:ss zzz"));
			cmd.Parameters.AddWithValue("@ispublic",b.IsPublic);

			bool saved = SaveBaby (b, user);
			if (saved) {
				return b;
			}
			return new Baby();


		}
		#endregion

		#region baby events
		public BabyEvent ReadBabyEvent (int Id, User user){
			BabyEvent be = new BabyEvent ();

			SqliteCommand cmd = new SqliteCommand (READ_BABYEVENT, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@id", Id);

			SqliteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				be.Id = Id;
				be.ReportUser = r ["Username"].ToString();
				be.BabyId = r ["BabyId"].ToString();
				be.ReportTime = DateTime.Parse (r ["Reported"].ToString ());
				be.Event = r ["event"].ToString ();
				be.Details = r ["details"].ToString ();
			}
			r.Close ();

			return be;
		}
		public bool SaveBabyEvent (BabyEvent babyevent, User user){
			SqliteCommand cmd = new SqliteCommand (SAVE_BABYEVENT, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@username", babyevent.ReportUser);
			cmd.Parameters.AddWithValue ("@babyId", babyevent.BabyId);
			cmd.Parameters.AddWithValue ("@event", babyevent.Event);
			cmd.Parameters.AddWithValue ("@reported", babyevent.ReportTime.ToString ("yyyy-MM-dd HH:mm:ss zzz"));
			cmd.Parameters.AddWithValue ("@details", babyevent.Details);
			cmd.Parameters.AddWithValue ("@id", babyevent.Id);

			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}

		public BabyEvent CreateBabyEvent (BabyEvent babyevent, User user){
			BabyEvent be = babyevent;

			SqliteCommand cmd = new SqliteCommand (CREATE_BABYEVENT, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			cmd.Parameters.AddWithValue ("@username", babyevent.ReportUser);
			cmd.Parameters.AddWithValue ("@babyId", babyevent.BabyId);
			cmd.Parameters.AddWithValue ("@event", babyevent.Event);
			cmd.Parameters.AddWithValue ("@reported", babyevent.ReportTime.ToString ("yyyy-MM-dd HH:mm:ss zzz"));
			cmd.Parameters.AddWithValue ("@details", babyevent.Details);

			bool saved = cmd.ExecuteNonQuery () > 0;
			if (saved) {
				return babyevent;
			}
			return be;
		}
		#endregion

		#region permissions
		public Permission ReadPermission (int Id, User user){
			Permission p = new Permission ();

			SqliteCommand cmd = new SqliteCommand (READ_PERMISSION, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue("@id",Id);

			SqliteDataReader r  = cmd.ExecuteReader ();

			if (r.Read ()) {
				p.Id = Id;
				p.Added =  DateTime.ParseExact(r ["Added"].ToString (), 
					"yyyy-MM-dd HH:mm:ss zzz",
					CultureInfo.InvariantCulture );
				p.BabyId = r ["BabyId"].ToString ();
				p.Type = (Permission.Types)int.Parse (r ["Type"].ToString ());
				p.Username = r ["Username"].ToString ();
			}
			r.Close ();

			return p;
		}


		public bool SavePermission (Permission permission, User user){
			SqliteCommand cmd = new SqliteCommand (SAVE_PERMISSION, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@type", permission.Type);
			cmd.Parameters.AddWithValue("@username",permission.Username);
			cmd.Parameters.AddWithValue("@babyid",permission.BabyId);
			cmd.Parameters.AddWithValue("@added",permission.Added.ToString("yyyy-MM-dd HH:mm:ss zzz"));
			cmd.Parameters.AddWithValue("@id",permission.Id);

			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}


		public Permission CreatePermission (Permission permission, User user){
			Permission p = new Permission();

			SqliteCommand cmd = new SqliteCommand (CREATE_PERMISSION, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@type", permission.Type);
			cmd.Parameters.AddWithValue("@username",permission.Username);
			cmd.Parameters.AddWithValue("@babyid",permission.BabyId);
			cmd.Parameters.AddWithValue("@added",permission.Added.ToString("yyyy-MM-dd HH:mm:ss zzz"));


			bool saved = cmd.ExecuteNonQuery () > 0;
			if (saved) {
				return permission;
			}
			return p;
		}
		#endregion


		#region users
		public User ReadUser (string username){
			User u = new User ();

			SqliteCommand cmd = new SqliteCommand (READ_USER, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@username", username);

			SqliteDataReader r  = cmd.ExecuteReader ();


			if (r.Read ()) {
				u.Username = username;
				u.Email = r ["Email"].ToString ();
				u.Hash = r ["Hash"].ToString ();
				u.Salt = r ["Salt"].ToString ();
				u.Image = r ["Image"].ToString ();
				u.Joined = DateTime.ParseExact(r ["Joined"].ToString (),
					"yyyy-MM-dd HH:mm:ss zzz",
					CultureInfo.InvariantCulture );
				u.Role = (User.Roles)int.Parse (r ["Role"].ToString());
				u.Flag = (User.Flags)int.Parse (r ["Flag"].ToString());
			}
			r.Close ();

			return u;
		}


		public bool SaveUser (User target, User user){
			SqliteCommand cmd = new SqliteCommand (SAVE_USER, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}
			cmd.Parameters.AddWithValue ("@username", target.Username);
			cmd.Parameters.AddWithValue ("@email", target.Email);
			cmd.Parameters.AddWithValue ("@salt", target.Salt);
			cmd.Parameters.AddWithValue ("@image", target.Image);
			cmd.Parameters.AddWithValue ("@role", (int)target.Role);
			cmd.Parameters.AddWithValue ("@flag", (int)target.Flag);

			int items = cmd.ExecuteNonQuery();

			return items > 0;
		}


		public User CreateUser (User target, User user){
			User u = new User();

			SqliteCommand cmd = new SqliteCommand (CREATE_USER, db);

			if (db.State != System.Data.ConnectionState.Open) {
				db.Open ();
			}

			cmd.Parameters.AddWithValue ("@username", target.Username);
			cmd.Parameters.AddWithValue ("@email", target.Email);
			cmd.Parameters.AddWithValue ("@salt", target.Salt);
			cmd.Parameters.AddWithValue ("@hash", target.Hash);
			cmd.Parameters.AddWithValue ("@image", target.Image);
			cmd.Parameters.AddWithValue ("@role", (int)target.Role);
			cmd.Parameters.AddWithValue ("@flag", (int)target.Flag);
			cmd.Parameters.AddWithValue ("@joined", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));


			int saved = cmd.ExecuteNonQuery ();
			if (saved > 0) {
				return target;
			}
			return u;
		}


		#endregion

		public List<BabyEvent> GetEventsForBaby(Baby baby, User user){

			List<BabyEvent> Events = new List<BabyEvent> ();

			SqliteCommand cmd = new SqliteCommand (READ_BABY_EVENTS, this.db);

			cmd.Parameters.AddWithValue ("@id", baby.Id);

			SqliteDataReader r = cmd.ExecuteReader ();
			while (r.Read ()) {
				BabyEvent be = new BabyEvent ();

				be.Id = int.Parse (r ["Id"].ToString ());
				be.ReportUser = r ["Username"].ToString ();
				be.BabyId = r ["BabyId"].ToString ();
				be.ReportTime = DateTime.Parse (r ["Reported"].ToString ());
				be.Event = r ["event"].ToString ();
				be.Details = r ["details"].ToString ();

				Events.Add (be);
			}
			r.Close ();
			return Events;
		}

		public List<Permission> GetPermissionsForBaby(Baby baby, User user){
			List<Permission> Permissions = new List<Permission> ();

			SqliteCommand cmd = new SqliteCommand (READ_BABY_PERMISSIONS, this.db);

			cmd.Parameters.AddWithValue ("@id", baby.Id);
		
			SqliteDataReader r = cmd.ExecuteReader ();
			while (r.Read ()) {
				Permission p = new Permission ();

				p.Id = int.Parse (r ["Id"].ToString ());
				p.Added =  DateTime.ParseExact(r ["Added"].ToString (), 
					"yyyy-MM-dd HH:mm:ss zzz",
					CultureInfo.InvariantCulture );
				p.BabyId = r ["BabyId"].ToString ();
				p.Type = (Permission.Types)int.Parse (r ["Type"].ToString ());
				p.Username = r ["Username"].ToString ();

				Permissions.Add (p);
			}
			r.Close ();
			return Permissions;
		}

		public List<BabyEvent> GetEventsForBaby(Baby baby, User user ,Filter filter){
			List<BabyEvent> Events = new List<BabyEvent> ();

			SqliteCommand cmd = new SqliteCommand (READ_BABY_EVENTS, this.db);

			cmd.Parameters.AddWithValue ("@id", baby.Id);

			cmd.CommandText = cmd.CommandText.Replace ("ORDER BY Reported DESC", "");

			if (filter.Start > DateTime.MinValue) {
				cmd.CommandText += " AND Reported >= @start ";
				cmd.Parameters.AddWithValue ("@start", filter.Start);
			}
			if (filter.End > DateTime.MinValue) {
				cmd.CommandText += " AND Reported <= @end ";
				cmd.Parameters.AddWithValue ("@end", filter.End);
			}
			if (!String.IsNullOrEmpty (filter.EventType)) {
				switch(filter.Match){

				case Filter.Matches.EQUAL:
					cmd.CommandText += " AND Event = @event ";
					cmd.Parameters.AddWithValue ("@event", filter.EventType);
					break;
				case Filter.Matches.LIKE:
					cmd.CommandText += " AND Event like @event ";
					cmd.Parameters.AddWithValue ("@event", filter.EventType);
					break;
				case Filter.Matches.NOT_EQUAL:
					cmd.CommandText += " AND Event like @event ";
					cmd.Parameters.AddWithValue ("@event", filter.EventType);
					break;
				}
			}


			cmd.CommandText += " ORDER BY Reported DESC ";
			if (filter.Count > 0) {
				cmd.CommandText += " LIMIT @count ";

				cmd.Parameters.AddWithValue ("@count", filter.Count);
				if (filter.Offset > 0 ) {
					cmd.CommandText += " OFFSET @offset ";
					cmd.Parameters.AddWithValue("@offset", filter.Offset);
				}
			}




			SqliteDataReader r = cmd.ExecuteReader ();
			while (r.Read ()) {
				BabyEvent be = new BabyEvent ();

				be.Id = int.Parse (r ["Id"].ToString ());
				be.ReportUser = r ["Username"].ToString ();
				be.BabyId = r ["BabyId"].ToString ();
				be.ReportTime =  DateTime.ParseExact(r ["Reported"].ToString (),
					"yyyy-MM-dd HH:mm:ss zzz",
					CultureInfo.InvariantCulture );
				be.Event = r ["event"].ToString ();
				be.Details = r ["details"].ToString ();

				Events.Add (be);
			}
			r.Close ();
			return Events;
		}
	}
}

