using System;
using System.Web;
using System.Web.UI;
using BabyData.Authentication;
using BabyData.Data;

namespace BabyData
{
	
	public class Service : System.Web.IHttpHandler
	{
		public void ProcessRequest (HttpContext context)
		{
			IBabyDataSource Sql = new SqliteWrapper ("test");
			AuthMethod Authentication = new HttpBasic(Sql);


			User LoggedIn =null;
			try{
				LoggedIn = Authentication.Login (context.Request);
			}
			catch(AuthException ae){
				Authentication.HandleFailure (context.Response, ae);
			}

			//if the user cannot login exit. They will have been redirected
			if (LoggedIn == null) {
				return;
			}

			//What is the user trying to do?
			string action = context.Request ["action"];
			string method = context.Request.HttpMethod;

			//do they have permission?


		}

		public bool IsReusable {
			get {
				return false;
			}
		}
	}
}
	
