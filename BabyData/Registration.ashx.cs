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
				IBabyDataSource ds = new SqliteWrapper ("URI=file:test.db");
				List<string> errors = new List<string> ();

				string username = context.Request["user"];
				string password = context.Request["pass"];
				string email = context.Request["mail"];


				if(String.IsNullOrEmpty(username)){
					errors.Add("Must specify Username");
				}
				if(String.IsNullOrEmpty(password)){
					errors.Add("Must specify Password");
				}
				if(String.IsNullOrEmpty(email)){
					errors.Add("Must specify email");
				}

				User u=null;
				if(errors.Count == 0){
					//confirm that there is no user with that username
					u = ds.ReadUser(username);


					if (u != null && u.Username == username) {
						errors.Add ("Username in use");
					}

				
					//confirm password meets requirements
					if (password.Length < MIN_PW_LENGTH) {
						errors.Add ("Password must be at least " + MIN_PW_LENGTH +
							" characters long");
					}

					if (!email.Contains ("@")) {
						errors.Add ("Email addresses must contain @");
					}
				}
				//set headers
				context.Response.ContentType = "application/json";
				context.Response.ContentEncoding = System.Text.Encoding.GetEncoding ("UTF-8");

				//create a new user
				if(errors.Count ==0 ){
					u = new User();
					u.Username = username;
					u.Email = email;
					u.Hash = u.BuildHash (password);

					u = ds.CreateUser (u, u);
					context.Response.Write(String.Format(
						@"{{success:{{registered:{0} }} }}", u.ToJSON()));
				}
				else{
					context.Response.Write(String.Format(
					@"{{errors:[""{0}""]}}", String.Join("\",\"",errors)));
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
	
