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
			return true;
		}
		public override void RespondToRequest (User user,
			System.Web.HttpRequest request, 
			System.Web.HttpResponse response, 
			IBabyDataSource DataSource)
		{
			Baby b;

			if (!String.IsNullOrEmpty(request ["id"])) {
				b = DataSource.ReadBaby (request ["id"], user);

				if (b.HasPermission (user.Username, Permission.Types.READ)) {

					switch (request.HttpMethod.ToUpper()) {

					case "GET":
						b.Permissions = DataSource.GetPermissionsForBaby (b, user);
						response.Write (b.ToJSON ());
						break;

					case "POST":

						//TODO CHECKPERMISSIONS
						Permission p = new Permission ();
						p.BabyId = b.Id;
						p.Username = request ["username"];
						Enum.TryParse<Permission.Types> (request ["type"], out p.Type);

						p = DataSource.CreatePermission (p, user);
						b.Permissions.Add (p);
						response.Write(b.ToJSON());
						break;
					default:
						throw new NotSupportedException ("Unsupported HTTP Method");
						break;

					}
				} else {
					throw new AuthException ("You don't have permission to view this baby's data");
				}
			} else {
				throw new ArgumentNullException ("Baby id not specified");
			}
		}
	}
}

