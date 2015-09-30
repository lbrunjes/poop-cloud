using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BabyData.Data
{
	public class User:DataObject
	{
		public string Email;
		public string Username;
		protected string Salt;
		public string Hash;
		public string Image;
		public string Description;
		public enum Roles{ USER,
			ADMIN
			}
		public Roles Role = Roles.USER;
		public enum Flags{
			MUST_CHANGE_PASSWORD,
			LOCKED_OUT
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
			hasher.IterationCount = 1024;
			return Convert.ToBase64String (hasher.GetBytes (20));

		}

		protected byte[] GenerateSalt(){
			RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
			int saltlength =64;
			byte[] salt = new byte[saltlength];
			random.GetBytes(salt);
			return salt;
		}




	}
}

