using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BabyData.Data
{
	public class User:DataObject
	{
		public enum Roles{ USER = 1,
			ADMIN = 2,
		}
		public enum Flags{
			NONE=0,
			MUST_CHANGE_PASSWORD = 1,
			LOCKED_OUT = 2
		}

		public string Email;
		public string Username;
		public string Salt;
		public string Hash;
		public string Image;
		public DateTime Joined = DateTime.Now;
		public Roles Role = Roles.USER;
		public Flags Flag = Flags.NONE;

		public User(string email, 
			string username, 
			string salt, 
			string hash, 
			string image, 
			DateTime joined){

			Email = email;
			Username = username;
			Salt = salt;
			Hash = hash;
			Image = image;
			Joined = joined;

		}

		public User(){

		}


		public void ResetPassword(string password){
			this.Salt = "";
			this.Hash = BuildHash (password);
			//TODO REMOVE MUST CHAGE flas
		}

		public string BuildHash(string password){
			
			byte[] sbytes = null;
			if (string.IsNullOrEmpty (Salt)) {
				sbytes = GenerateSalt ();
				Salt = Convert.ToBase64String (sbytes);	
			} else {
				sbytes = Convert.FromBase64String (Salt);
			}
			var hasher = new Rfc2898DeriveBytes (password, sbytes);
			hasher.IterationCount =512;
			return Convert.ToBase64String (hasher.GetBytes (20));

		}

		protected byte[] GenerateSalt(){
			RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
			int saltlength =64;
			byte[] salt = new byte[saltlength];
			random.GetBytes(salt);
			return salt;
		}


		public override string ToJSON ()
		{
			return String.Format ("{{\"type\": \"user\"," +
				"\"username\":\"{0}\"," +
				"\"image\":\"{1}\"," +
				"\"email\":\"{2}\"," +
				"\"joined\":\"{3:yyyy-MM-dd hh:mm:ss zzz}\"}}", 
				this.Username, 
				this.Image, this.Email,this.Joined);
		}


	}
}

