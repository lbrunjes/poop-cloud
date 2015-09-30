using System;

namespace BabyData.Data
{
	public class Permission:DataObject
	{
		public enum Types{
			READ,
			UPDATE,
			OWNER
		}
		public Types Type;
		public string BabyId;
		public string Username;
		public string Id;
	}
}

