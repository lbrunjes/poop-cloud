using System;
using System.Collections.Generic;
using BabyData.Data;

namespace BabyData.Data
{
	/// <summary>
	/// Baby.
	/// </summary>
	public class Baby:DataObject
	{

		public List<BabyEvent> Events;
		public string Id;
		public string Name;
		public string Image;
		public string Sex;
		public DateTime DOB;
		public bool IsPublic;
		public List<Permission> Permissions;

		public bool HasPermission(string username, Permission.Types type){
			bool found = false;
			foreach (Permission p in this.Permissions) {
				if (p.Username == username && p.Type >= type) {
					found = true;
					break;
				}
			}
			return found;
		}
	}
}

