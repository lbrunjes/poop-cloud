using System;

namespace BabyData.Data
{
	public class Permission:DataObject
	{
		public enum Types{
			READ = 1, //viewing
			UPDATE =2,//updating
			PARENT=4 //allows adding of permissions
		}
		public Types Type;
		public string BabyId;
		public string Username;
		public int Id;
		public DateTime Added =DateTime.Now;

		public Permission(){}
		public Permission(string babyId, string username, Types type){
			this.Type= type;
			this.Username = username;
			this.BabyId= babyId;
		}
		public override string ToJSON ()
		{
			return string.Format ("{{" +
				"\"type\":\"{0}\"," +
				"\"baby\":\"{1}\"," +
				"\"user\":\"{2}\"," +
				"\"added\":\"{3:yyyy-MM-dd HH:mm:ss zzz}\"}}",
				this.Type,
				this.BabyId,
				this.Username,
				this.Added);
		}
	}
}

