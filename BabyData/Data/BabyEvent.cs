using System;

namespace BabyData.Data
{
	public class BabyEvent:DataObject
	{
		
		public DateTime ReportTime = DateTime.Now;
		public string ReportUser;
		public string BabyId;
		public int Id;
		public string Type;
		public string Subtype;
		public string Details;

		public BabyEvent ()
		{
		}
		public BabyEvent(string babyId, string username, string eventname, string subtype ="", string details = ""){
			BabyId = babyId;
			ReportUser = username;
			Type = eventname;
			Subtype = subtype;
			Details = details;
		}
		 public override string ToJSON ()
		{
			return string.Format ("{{" +
//				"\"id\":\"{0}\"," +
//				"\"baby\":\"{1}\"," +
				"\"type\":\"{2}\"," +
				"\"subtype\":\"{6}\"," +
				"\"user\":\"{3}\"," +
				"\"time\":\"{4:yyyy-MM-ddTHH:mm:sszzz}\"," +
				"\"details\":\"{5}\"" +
				"}}", 
				this.Id,
				this.BabyId, 
				this.Type, 
				this.ReportUser, 
				this.ReportTime,
				this.Details,//5
				this.Subtype
			);
		}
	}
}

