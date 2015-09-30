using System;

namespace BabyData.Data
{
	[Serializable]
	public abstract class DataObject
	{
		bool active =true;
		public string ToJSON(){
			return "{}";
		}
		public void Delete(){
			active = false;
		}
	}
}

