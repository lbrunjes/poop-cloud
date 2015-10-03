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
	
//			bool okay = base.HasPermision (user, request, DataSource);
//			if (!okay) {
//				//check for things
//
//
//			}
			return true;
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
						b.Events = DataSource.GetEventsForBaby (b, user);
						response.Write (b.ToJSON ());
					} else {
						throw new AuthException ("You don't have permission to view this baby's data");
					}
				} else {
					throw new ArgumentNullException ("Baby id not specified");
				}

				break;

			case "POST":




				break;
			default:
				throw new NotSupportedException ("Unsupported HTTP Method");
				break;

			}

	
		}
	}
}

