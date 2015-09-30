using System;
using System.Web;
using System.Web.UI;
using BabyData.Data;
using System.Collections.Generic;

namespace BabyData
{
	
	public class Registration : System.Web.IHttpHandler
	{
		public static readonly int MIN_PW_LENGTH = 8;
	
		public void ProcessRequest (HttpContext context)
		{
			try{
				IBabyDataSource Sql = new SqliteWrapper ("test");

				string username = context.Request["user"];
				string password = context.Request["pass"];
				string email = context.Request["mail"];

				List<string> errors = new List<string> ();
				//confirm that there is no user with that username
				User u = Sql.ReadUser(username);

				if (u != null && u.Username == username) {
					errors.Add ("User name in use");
				}

				//confirm password meets requirements
				if (password.Length < MIN_PW_LENGTH) {
					errors.Add ("Password must be at least " + MIN_PW_LENGTH +
						" characters long");
				}

				if (!email.Contains ("@")) {
					errors.Add ("Email addresses must contain @");
				}

				//set headers
				context.Response.ContentType = "application/json";
				context.Response.ContentEncoding = System.Text.Encoding.GetEncoding ("UTF-8");

				//create a new user
				if(errors.Count ==0){
					User newUser = new User();
					u.Username = username;
					u.Email = email;
					u.Hash = u.BuildHash (password);

					bool saved = Sql.SaveUser (newUser, newUser);
					context.Response.Write(String.Format(
						@"{{success:{{registered:{0} }} }}", u.ToJSON()));
				}
				else{
					context.Response.Write(String.Format(
					@"{{errors:[{0}]}}", String.Join(",",errors)));
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
	}
}
	
