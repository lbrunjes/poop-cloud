using System;
using System.Collections.Generic;

namespace BabyData.Data
{
	[Serializable]
	public abstract class DataObject
	{
		public virtual string ToJSON(){
			return String.Format(@"{{""TODO"":"":(""}}");
		}



	}
}

