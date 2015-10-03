using System;
using BabyData.Data;
using BabyData.Authentication;

namespace BabyData
{
	public class PermissionResponder:Responder
	{
		public override bool HasPermision (	User user, 
			System.Web.HttpRequest request, 
			IBabyDataSource DataSource)
		{
			bool okay = base.HasPermision (user, request, DataSource);
			if (!okay) {
				//check for things
			}
			return okay;
		}
		public override void RespondToRequest (User user,
			System.Web.HttpRequest request, 
			System.Web.HttpResponse response, 
			IBabyDataSource DataSource)
		{
			Baby b;
			switch (request.HttpMethod.ToUpper()) {

			case "GET":

				if (!String.IsNullOrEmpty(request ["id"])) {
					b = DataSource.ReadBaby (request ["id"], user);

					if (b.HasPermission (user.Username, Permission.Types.READ)) {
						b.Permissions = DataSource.GetPermissionsForBaby (b, user);
						response.Write (b.ToJSON ());
					} else {
						throw new AuthException ("You don't have permission to view this baby's data");
					}
				} else {
					throw new ArgumentNullException ("Baby id not specified");
				}

				break;

			case "POST":

				b = new Baby();
				b.Name = request["name"];
				b.Sex = request["sex"];
				b.IsPublic = request["public"] =="Y";
				DateTime.TryParse(request["dob"], out b.DOB);
				b.Image = request["image"];

				if(String.IsNullOrEmpty(request["id"])){
					Baby fromDb = DataSource.CreateBaby(b,user);
					response.Write (fromDb.ToJSON ());
				}
				else{
					DataSource.SaveBaby(b, user);
				}


				break;
			default:
				throw new NotSupportedException ("Unsupported HTTP Method");
				break;

			}
		}
	}
}

