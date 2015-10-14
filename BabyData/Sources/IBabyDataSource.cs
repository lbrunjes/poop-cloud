using System;
using System.Collections.Generic;


namespace BabyData.Data
{
	public interface IBabyDataSource:IDisposable
	{


		Baby ReadBaby (string Id, User user);
		bool SaveBaby (Baby baby, User user);
		Baby CreateBaby (Baby baby, User user);

		BabyEvent ReadBabyEvent (int Id, User user);
		bool SaveBabyEvent (BabyEvent babyevent, User user);
		BabyEvent CreateBabyEvent (BabyEvent babyevent, User user);

		Permission ReadPermission (int Id, User user);
		bool SavePermission (Permission permission, User user);
		Permission CreatePermission (Permission permission, User user);

		User ReadUser (string username);
		bool SaveUser (User target, User user);
		User CreateUser (User target, User user);

		List<Permission> GetPermissionsForBaby(Baby baby, User user);
		List<Permission> GetPermissionsForUser(User user);

		List<BabyEvent> GetEventsForBaby(Baby baby, User user);
		List<BabyEvent> GetEventsForBaby (Baby baby, User user, Filter filter);

	}

	public struct Filter
	{
		public DateTime Start;
		public DateTime End;
		public int Count;
		public int Offset;
		public string EventType;
		public Matches Match;
		public static readonly Filter Empty = new Filter();

		public enum Matches{
			EQUAL,
			NOT_EQUAL,
			LIKE
		}
		public Filter (DateTime start, DateTime end, string eventType = "", Matches match=Matches.EQUAL){
			Start = start;
			End = end;
			Count = -1;
			Offset = -1;
			EventType = eventType;
			Match = match;
		}
		public Filter(int count, int offset = 0, string eventType = "", Matches match=Matches.EQUAL){
			Count = count;
			Offset = offset;
			Start = DateTime.MinValue;
			End = DateTime.MinValue;
			EventType = eventType;
			Match = match;
		}
	}
}

