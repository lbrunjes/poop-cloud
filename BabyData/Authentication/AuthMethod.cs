using System;
using System.Web;
using BabyData.Data;

namespace BabyData.Authentication
{
	public abstract class AuthMethod
	{
		public virtual void HandleFailure(HttpResponse response, AuthException ae){
			
			throw ae;
		}
		public virtual User Login(HttpRequest request){
			throw new InvalidLoginException ("Authentication method does not support login");
		}

		public virtual void Logout(User user){
			throw new AuthException ("Authentication method does not support logout");
		}

		public virtual User Register(User user){
			throw new RegistrationException ("Authentication method does not support registration");
		}


	}
}

