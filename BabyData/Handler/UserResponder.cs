using System;
using BabyData.Data;

namespace BabyData
{
	public class UserResponder:Responder
	{
		public override bool HasPermision (
			User user, 
			System.Web.HttpRequest request, 
			IBabyDataSource DataSource,
			Permission.Types type = Permission.Types.READ
		)
		{
			return true;
		}
		public override void RespondToRequest (User user,
			System.Web.HttpRequest request, 
			System.Web.HttpResponse response, 
			IBabyDataSource DataSource)
		{
			User u;
			if (!String.IsNullOrEmpty (request ["id"])) {
				u = DataSource.ReadUser (request ["id"]);
			} else {
				u = DataSource.ReadUser (user.Username);
				u.Permissions = DataSource.GetPermissionsForUser (user);
			}
			switch (request.HttpMethod.ToUpper ()) {

			case "GET":
				response.Write (u.ToJSON ());
				break;

			case "POST":

				//users cannot be created here only via the regiestration portal.
				u = DataSource.ReadUser (request ["id"]);
				u.Email = request ["email"];
				u.Image = request ["image"];

				//PasswordChanges matching length, and old ps checked.
				if (String.IsNullOrEmpty (request ["password1"]) &&
				    request ["password1"] == request ["password2"] &&
				    request ["password1"].Length > Registration.MIN_PW_LENGTH &&
				    String.IsNullOrEmpty (request ["password_old"]) &&
				    u.BuildHash (request ["password_old"]) == u.Hash) {
					u.Hash = u.BuildHash (request ["password1"]);
				}

				if (u.Username == user.Username || user.Role == User.Roles.ADMIN) {
					DataSource.SaveUser (u, user);
					response.Write (u.ToJSON ());
				} else {
					throw new AccessViolationException ("You can't just edit someone else's user details");
				}
				
				

				break;

			default:
				throw new NotSupportedException ("Unsupported HTTP Method");
				break;
			}

		}
	}
}

