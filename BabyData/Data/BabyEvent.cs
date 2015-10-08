using System;

namespace BabyData.Data
{
	public class BabyEvent:DataObject
	{
		
		public DateTime ReportTime = DateTime.Now;
		public string ReportUser;
		public string BabyId;
		public int Id;
		public string Event;
		public string Details;

		public BabyEvent ()
		{
		}
		public BabyEvent(string babyId, string username, string eventname, string details = ""){
			BabyId = babyId;
			ReportUser = username;
			Event = eventname;
			Details = details;
		}
		 public override string ToJSON ()
		{
			return string.Format ("{{" +
				"\"id\":\"{0}\"," +
				"\"baby\":\"{1}\"," +
				"\"event\":\"{2}\"," +
				"\"user\":\"{3}\"," +
				"\"time\":\"{4:yyyy-MM-dd hh:mm:ss zzz}\"," +
				"\"details\":\"{5}\"" +
				"}}", 
				this.Id,
				this.ToURLSafeBase64(this.BabyId), 
				this.Event, 
				this.ReportUser, 
				this.ReportTime,
				this.Details);
		}
	}
}

