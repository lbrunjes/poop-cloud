using System;
using System.Web;
using System.Web.UI;
using BabyData.Authentication;
using BabyData.Data;
using System.Collections.Generic;

namespace BabyData
{
	
	public class Service : System.Web.IHttpHandler
	{
		public void ProcessRequest (HttpContext context)
		{
			try{
				IBabyDataSource Sql = new SqliteWrapper ("URI=file:test.db");
				AuthMethod Authentication = new HttpBasic(Sql);

				//set headers
				context.Response.ContentType = "application/json";
				context.Response.ContentEncoding = System.Text.Encoding.GetEncoding ("UTF-8");

				//get teh user?
				User LoggedIn =null;
				try{
					LoggedIn = Authentication.Login (context.Request);
				}
				catch(AuthException ae){
					Authentication.HandleFailure (context.Response, ae);
				}

				//if the user cannot login exit. 
				//They will have been redirected or tnotified by the auth system.
				if (LoggedIn != null) {

					//What is the user trying to do?
					string key = context.Request ["type"];

					//is that supported? Can we do that?
					if (Responders.ContainsKey (key)&&
						Responders[key].HasPermision(LoggedIn, context.Request,Sql)
					) {
						Responders [key].RespondToRequest (LoggedIn, context.Request, context.Response, Sql);
					}
					else{
						throw new NotSupportedException(" Your request is not supported, yet:(");	
					}
				}
			
			}
			catch(Exception ex){
				context.Response.StatusCode = 500;
				context.Response.Write(String.Format(
					@"{{server_error:{{""message"":""{0}"",""type"":""{1}""}}}}", 
					ex.Message, ex.GetType()));
			}


		}

		public bool IsReusable {
			get {
				return false;
			}
		}

		public Dictionary<string, Responder> Responders = new Dictionary<string, Responder> {
			{"baby",new BabyResponder()},
			{"babyevents",new BabyEventResponder()},
			{"permissions", new PermissionResponder()},
			{"user", new UserResponder()}
		};
	}
}
	
