using System;
using System.Collections.Generic;
using BabyData.Data;
using System.Text;

namespace BabyData.Data
{
	/// <summary>
	/// Baby.
	/// </summary>
	public class Baby:DataObject
	{

		public List<BabyEvent> Events = new List<BabyEvent> ();
		public string Id;
		public string Name;
		public string Image;
		public string Sex;
		public DateTime DOB;
		public bool IsPublic;
		public List<Permission> Permissions = new List<Permission> ();



		public bool HasPermission(string username, Permission.Types type){
			bool found = false;
			if (type == Permission.Types.READ && this.IsPublic) {
				return true;
			}

			foreach (Permission p in this.Permissions) {
				if (p.Username == username && p.Type >= type) {
					found = true;
					break;
				}
			}
			return found;
		}


		public override string ToJSON ()
		{
			string prefix = "";
			StringBuilder PermissionJSON = new StringBuilder ();
			foreach (Permission p in this.Permissions) {
				PermissionJSON.Append (prefix );
				PermissionJSON.Append (p.ToJSON () );
				prefix = ",";
			}
			prefix = "";
			StringBuilder EventJSON = new StringBuilder ();
			foreach (BabyEvent e in this.Events) {
				EventJSON.Append (prefix );
				EventJSON.Append (e.ToJSON () + ",");
				prefix = ",";
			}

			return string.Format ("{{" +
			"\"id\":\"{0}\"," +
			"\"name\":\"{1}\"," +
			"\"image\":\"{2}\"," +
			"\"sex\":\"{3}\"," +
			"\"dateofbirth\":{4:yyyy-MM-dd hh:mm:ss zzz}\"," +
			"\"public\":\"{5}\"," +
			"\"events\":[{6}]," +
			"\"permissions\":[{7}]}}", 

				this.Id, 
				this.Name,
				this.Image, 
				this.Sex, 
				this.DOB,
				this.IsPublic,//5
				PermissionJSON, EventJSON
			);
		}
	}
}

