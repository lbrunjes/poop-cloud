using System;

namespace BabyData.Data
{
	public class BabyEvent:DataObject
	{
		
		public DateTime ReportTime;
		public string ReportUser;
		public string BabyId;
		public int Id;
		public string Event;
		public string Details;

		public BabyEvent ()
		{
		}
		 public override string ToJSON ()
		{
			return string.Format ("{{" +
				"\"id\":\"{0}\"" +
				"\"baby\":\"{1}\"" +
				"\"event\":\"{2}\"" +
				"\"user\":\"{3}\"," +
				"\"time\":\"{4:yyyy-MM-dd hh:mm:ss zzz}\"" +
				"\"details\":\"{5}\"" +
				"}}", 
				this.Id,
				this.BabyId, 
				this.Event, 
				this.ReportUser, 
				this.ReportTime,
				this.Details);
		}
	}
}

