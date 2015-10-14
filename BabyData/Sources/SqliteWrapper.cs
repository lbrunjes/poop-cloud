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

		private static readonly string READ_USER_PERMISSIONS =@"SELECT 
Id,
Type,
Username,
BabyId,
Added
FROM Permission 
WHERE Username = @username
ORDER BY Added ASC
";
		private static readonly string READ_BABY_PERMISSIONS =@"SELECT 
Id,
Type,
Username,
BabyId,
Added
FROM Permission 
WHERE BabyId =@id
ORDER BY Added ASC
";
		private static readonly string READ_BABY_EVENTS =@"SELECT 
Id,
Username,
BabyId,
Type, 
Subtype,
Reported,
Details
FROM BabyEvent 
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
Type,
Subtype,
Reported,
Details
FROM BabyEvent WHERE Id =@id";
		private static readonly string SAVE_BABYEVENT =@"UPDATE BabyEvent 
SET


Username = @username,
BabyId = @babyid,
Type = @type,
Subtype =@subtype,
Reported = @reported,
Details = @dteails

WHERE Id =@id;";
		private static readonly string CREATE_BABYEVENT =@"INSERT INTO BabyEvent
(	Username,	BabyId, 	Type, 	Subtype,	Reported, 	Details)
VALUES
(	@username,	@babyId,	@type,	@subtype,	@reported,	@details)
		
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


		private static readonly int BABY_ID_LENGTH = 4;
		private static string DB_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss zzz";
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
				b.DOB = DateTime.ParseExact (r ["DateOfBirth"].ToString (), 
					DB_DATE_FORMAT,
					CultureInfo.InvariantCulture);

				b.Name = r ["Name"].ToString ();
				b.Image = r ["Image"].ToString ();
				b.Sex = r ["Sex"].ToString ();
				b.IsPublic = r ["IsPublic"].ToString () == "1";
			} else {
				throw new ArgumentException ("No baby found for that id. ");
			}
			r.Close();

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
			cmd.Parameters.AddWithValue("@dateofbirth",baby.DOB.ToString(DB_DATE_FORMAT));
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
				b.Id = Convert.ToBase64String (id)
					.Replace ('+', '-')
					.Replace('/','_')
					.TrimEnd(new char[]{'='});
				cmd.Parameters.AddWithValue ("@Id",b.Id);

			} else {
				cmd.Parameters.AddWithValue ("@Id", baby.Id);
			}
			cmd.Parameters.AddWithValue("@name",b.Name);
			cmd.Parameters.AddWithValue("@image",b.Image);
			cmd.Parameters.AddWithValue("@sex",b.Sex);
			cmd.Parameters.AddWithValue("@dateofbirth",b.DOB.ToString(DB_DATE_FORMAT));
			cmd.Parameters.AddWithValue("@ispublic",b.IsPublic);

			bool saved = cmd.ExecuteNonQuery () > 0;
			if (saved) {

				Permission p = new Permission (b.Id,user.Username,Permission.Types.PARENT);
				this.CreatePermission (p,user);
				b.Permissions.Add (p);

				BabyEvent be = new BabyEvent (b.Id, user.Username, "INFO", "CREATED");
				this.CreateBabyEvent (be, user);
				b.Events.Add (be);

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
				be.Id = int.Parse (r ["Id"].ToString ());
				be.ReportUser = r ["Username"].ToString();
				be.BabyId = r ["BabyId"].ToString();
				be.ReportTime = DateTime.Parse (r ["Reported"].ToString ());
				be.Type = r ["Type"].ToString ();
				be.Subtype = r ["Subtype"].ToString ();
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
			cmd.Parameters.AddWithValue ("@type", babyevent.Type);
			cmd.Parameters.AddWithValue ("@subtype", babyevent.Subtype);
			cmd.Parameters.AddWithValue ("@reported", babyevent.ReportTime.ToString (DB_DATE_FORMAT));
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
			cmd.Parameters.AddWithValue ("@type", babyevent.Type);
			cmd.Parameters.AddWithValue ("@subtype", babyevent.Subtype);
			cmd.Parameters.AddWithValue ("@reported", babyevent.ReportTime.ToString (DB_DATE_FORMAT));
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
					DB_DATE_FORMAT,
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
			cmd.Parameters.AddWithValue("@added",permission.Added.ToString(DB_DATE_FORMAT));
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
			cmd.Parameters.AddWithValue ("@type", (int)permission.Type);
			cmd.Parameters.AddWithValue("@username",permission.Username);
			cmd.Parameters.AddWithValue("@babyid",permission.BabyId);
			cmd.Parameters.AddWithValue("@added",permission.Added.ToString(DB_DATE_FORMAT));


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
					DB_DATE_FORMAT,
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
			cmd.Parameters.AddWithValue ("@joined", DateTime.Now.ToString(DB_DATE_FORMAT));


			int saved = cmd.ExecuteNonQuery ();
			if (saved > 0) {
				return target;
			}
			return u;
		}


		#endregion

		public List<BabyEvent> GetEventsForBaby(Baby baby, User user){

			return GetEventsForBaby (baby, user, Filter.Empty);
		}

		public List<Permission> GetPermissionsForBaby(Baby baby, User user){
			List<Permission> Permissions = new List<Permission> ();

			SqliteCommand cmd = new SqliteCommand (READ_BABY_PERMISSIONS, this.db);

			cmd.Parameters.AddWithValue ("@id", baby.Id);
		
			SqliteDataReader r = cmd.ExecuteReader ();
			while (r.Read ()) {
				Permission p = new Permission ();

				 int.TryParse (r ["Id"].ToString (),out p.Id);
				p.Added =  DateTime.ParseExact(r ["Added"].ToString (), 
					DB_DATE_FORMAT,
					CultureInfo.InvariantCulture );
				p.BabyId = r ["BabyId"].ToString ();
				p.Type = (Permission.Types)int.Parse (r ["Type"].ToString ());
				p.Username = r ["Username"].ToString ();

				Permissions.Add (p);
			}
			r.Close ();
			return Permissions;
		}

		public List<Permission> GetPermissionsForUser( User user){
			List<Permission> Permissions = new List<Permission> ();

			SqliteCommand cmd = new SqliteCommand (READ_USER_PERMISSIONS, this.db);

			cmd.Parameters.AddWithValue ("@username", user.Username);

			SqliteDataReader r = cmd.ExecuteReader ();
			while (r.Read ()) {
				Permission p = new Permission ();

				int.TryParse (r ["Id"].ToString (),out p.Id);
				p.Added =  DateTime.ParseExact(r ["Added"].ToString (), 
					DB_DATE_FORMAT,
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
					cmd.CommandText += " AND Type = @type ";
					cmd.Parameters.AddWithValue ("@type", filter.EventType);
					break;
				case Filter.Matches.LIKE:
					cmd.CommandText += " AND Type like @type ";
					cmd.Parameters.AddWithValue ("@type", filter.EventType);
					break;
				case Filter.Matches.NOT_EQUAL:
					cmd.CommandText += " AND Type like @type ";
					cmd.Parameters.AddWithValue ("@type", filter.EventType);
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

				int.TryParse (r ["Id"].ToString (),out be.Id  );
				be.ReportUser = r ["Username"].ToString ();
				be.BabyId = r ["BabyId"].ToString ();
				be.ReportTime  = DateTime.ParseExact(r ["Reported"].ToString (),
					DB_DATE_FORMAT,
					CultureInfo.InvariantCulture

					);
				be.Type = r ["Type"].ToString ();
				be.Subtype = r ["Subtype"].ToString ();
				be.Details = r ["Details"].ToString ();

				Events.Add (be);
			}
			r.Close ();
			return Events;
		}
	}
}

