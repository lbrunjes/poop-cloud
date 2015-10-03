using System;
using BabyData.Data;

namespace BabyData
{
	public class UserResponder:Responder
	{
		public override bool HasPermision (
			User user, 
			System.Web.HttpRequest request, 
			IBabyDataSource DataSource)
		{
			bool okay = base.HasPermision (user, request, DataSource);
			if (!okay) {
				if (request.HttpMethod == "GET") {
					okay = true;
				} else {
					okay = user.Username == request ["id"];
				}
			}
			return okay;
		}
		public override void RespondToRequest (User user,
			System.Web.HttpRequest request, 
			System.Web.HttpResponse response, 
			IBabyDataSource DataSource)
		{
			User u;

			switch (request.HttpMethod.ToUpper ()) {

			case "GET":
				if (!String.IsNullOrEmpty(request ["id"])) {
					u = DataSource.ReadUser (request ["id"]);

					response.Write (u.ToJSON ());

				} else {
					throw new ArgumentNullException ("Username not specified");
				}

				break;

			case "POST":

				//users cannot be created here only via the regiestration portal.
				if (!String.IsNullOrEmpty(request ["id"])) {
					
					u = DataSource.ReadUser (request ["id"]);
					u.Email = request ["email"];
					u.Image = request ["image"];

					//PasswordChanges mathcing length, and old ps checked.
					if (String.IsNullOrEmpty (request ["password1"]) &&
						request ["password1"] == request ["password2"] &&
						request["password1"].Length >  Registration.MIN_PW_LENGTH &&
						String.IsNullOrEmpty (request ["password_old"]) &&
						u.BuildHash(request ["password_old"]) == u.Hash
					){
						u.Hash = u.BuildHash (request ["password1"]);
					}

					if (u.Username == user.Username || user.Role == User.Roles.ADMIN) {
						DataSource.SaveUser (u, user);
						response.Write (u.ToJSON ());
					} else {
						throw new AccessViolationException ("You can't just edit someone else's user details");
					}
				
				}
				else{
					throw new NotSupportedException ("Users must be created using the registration system");
				}

			break;

				default:
				throw new NotSupportedException ("Unsupported HTTP Method");
				break;


			}
		}
	}
}

