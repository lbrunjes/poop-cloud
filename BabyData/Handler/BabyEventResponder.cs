using System;
using BabyData.Data;
using BabyData.Authentication;

namespace BabyData
{
	public class BabyEventResponder:Responder
	{
		public override bool HasPermision (User user, 
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
			if (!String.IsNullOrEmpty (request ["id"])) {
				b = DataSource.ReadBaby (request ["id"], user);
				if (b.HasPermission (user.Username, Permission.Types.READ)) {

					switch (request.HttpMethod.ToUpper ()) {

					case "GET":
						b.Events = DataSource.GetEventsForBaby (b, user);
						response.Write (b.ToJSON ());
						break;

					case "POST":
						BabyEvent be = new BabyEvent (
							               b.Id,
							               user.Username,
							               String.IsNullOrEmpty (request ["event"]) ? "UNKNOWN" : request ["event"],
							               String.IsNullOrEmpty (request ["details"]) ? "" : request ["details"]);
						be = DataSource.CreateBabyEvent (be, user);
						b.Events.Add (be);
						response.Write (b.ToJSON());

						break;
					default:
						throw new NotSupportedException ("Unsupported HTTP Method");
						break;

					}
				
				} else {
					throw new AuthException ("You don't have permission to view this baby's data");
				}
			}
			else {
				throw new ArgumentNullException ("Baby id not specified");
			}
	
		}
	}
}

