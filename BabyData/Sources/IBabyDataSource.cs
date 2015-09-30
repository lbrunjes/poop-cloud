using System;

namespace BabyData.Data
{
	public interface IBabyDataSource:IDisposable
	{
		
		Baby ReadBaby (string Id, User user);
		bool SaveBaby (Baby baby, User user);
		Baby CreateBaby (Baby baby, User user);

		BabyEvent ReadBabyEvent (string Id, User user);
		bool SaveBabyEvent (BabyEvent babyevent, User user);
		BabyEvent CreateBabyEvent (BabyEvent babyevent, User user);

		Permission ReadPermission (string Id, User user);
		bool SavePermission (Permission permission, User user);
		Permission CreatePermission (Permission permission, User user);

		User ReadUser (string username);
		bool SaveUser (User target, User user);
		User CreateUser (User target, User user);

	}
}

