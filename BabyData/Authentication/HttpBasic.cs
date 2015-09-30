using System;
using System.Text;
using BabyData.Data;
using Npgsql;

namespace BabyData.Authentication
{
	public class HttpBasic:AuthMethod
	{
		protected IBabyDataSource DataSource;

		public HttpBasic(IBabyDataSource data){
			this.DataSource = data;
		}
		public override void HandleFailure (System.Web.HttpResponse response, AuthException ae)
		{
			response.AddHeader("WWW-Authenticate", "Basic realm=\""+Environment.MachineName+"\"");
		}
		public override User Login (System.Web.HttpRequest request)
		{
			string data = request.Headers ["Authorization"];
			//data: 
			//Basic username:base64password
			User u= null;
			if (!String.IsNullOrEmpty (data)) {
				if (!data.StartsWith ("Basic ")) {
					throw new InvalidLoginException ("Invalid Authentication Header");
				}

				string text = Encoding.UTF8.GetString (Convert.FromBase64String (data.Substring (6)));
				int colon = text.IndexOf (":");

				string user = text.Substring (0, colon);
				string password = text.Substring (colon + 1);


				User ReferenceUser = DataSource.ReadUser(user);

				if (ReferenceUser!= null &&
					ReferenceUser.Hash == ReferenceUser.BuildHash(password)) {
					u = ReferenceUser;
				} else {
					throw new InvalidLoginException ("User cannot be Logged In. Please Check Username and  Password.");
				}
			}

			return u;
		}
		public override void Logout (User user)
		{
			base.Logout (user);
		}
		public override User Register (User user)
		{
			return base.Register (user);
		}

	}
}

