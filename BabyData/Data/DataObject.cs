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
		public virtual List<string> ValidationErrors(){
			return new List<string> ();
		}
		//msbase 64 encoding is not url safe....
		public string ToURLSafeBase64(string base64string){
			return base64string
				.Replace ('+', '#')
				.Replace('/','_')
				.TrimEnd(new char[]{'='});
		}

		//convert url safed base 64 back to ms...:(
		public string FromURLSafeBase64(string base64string){
			string tmp = "";
			switch (base64string.Length % 4) {
			case 2:
				tmp += "==";
				break;
			case 3:
				tmp += "+";
				break;
				
			}

			return	base64string
				.Replace ('#', '+')
				.Replace ('_', '/')	+ tmp;
		}

	}
}

