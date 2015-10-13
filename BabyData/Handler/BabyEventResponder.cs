using System;
using BabyData.Data;
using BabyData.Authentication;

namespace BabyData
{
	public class BabyEventResponder:Responder
	{
		public override bool HasPermision (User user, 
			System.Web.HttpRequest request, 
			IBabyDataSource DataSource,
			Permission.Types type = Permission.Types.READ)
		{
			return true;
		}
		public override void RespondToRequest (User user,
			System.Web.HttpRequest request, 
			System.Web.HttpResponse response, 
			IBabyDataSource DataSource)
		{
			Baby b;
			if (!String.IsNullOrEmpty (request ["id"])) {
				b = DataSource.ReadBaby (request ["id"], user);


					switch (request.HttpMethod.ToUpper ()) {

					case "GET":
						if (b.HasPermission (user.Username, Permission.Types.READ)) {
							b.Events = DataSource.GetEventsForBaby (b, user);
							response.Write (b.ToJSON ());
						} else {
							throw new AuthException ("You don't have permission to view this baby's data");
						}
						break;

				case "POST":
					b.Permissions = DataSource.GetPermissionsForBaby (b, user);
						if(b.HasPermission(user.Username, Permission.Types.UPDATE)){
							BabyEvent be = new BabyEvent (
								               b.Id,
								               user.Username,
								               String.IsNullOrEmpty (request ["eventtype"]) ? "UNKNOWN" : request ["eventtype"],
								               String.IsNullOrEmpty (request ["subtype"]) ? "" : request ["subtype"],
								               String.IsNullOrEmpty (request ["details"]) ? "" : request ["details"]);
							be = DataSource.CreateBabyEvent (be, user);
							b.Events.Add (be);
							response.Write (b.ToJSON());
						}
						else {
							throw new AuthException ("You don't have permission to Update this baby's data");
						}
						break;
					default:
						throw new NotSupportedException ("Unsupported HTTP Method");
						break;

					}
				
				
			}
			else {
				throw new ArgumentNullException ("Baby id not specified as 'id'");
			}
	
		}
	}
}

