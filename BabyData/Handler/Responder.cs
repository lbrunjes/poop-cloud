using System;
using BabyData.Data;
using System.Web;

namespace BabyData
{
	public abstract class Responder
	{
		public virtual bool HasPermision(User user, 
			HttpRequest request, 
			IBabyDataSource DataSource,
			Permission.Types type = Permission.Types.READ
		){

			bool okay = false;
			if (user.Role == User.Roles.ADMIN) {
				okay = true;
			}
			//TODO
			return okay;

		}

		public virtual void RespondToRequest(User user, 
			HttpRequest request,
			HttpResponse response, 
			IBabyDataSource DataSource){
		}

		protected virtual bool TryGetParam(string param, HttpRequest req, out string value) {

			bool found = false;
			if (!String.IsNullOrEmpty (req [param])) {
				value = req [param];
				found = true;
			} else {
				value = "";
			}
			return found;
		}
	}
}

