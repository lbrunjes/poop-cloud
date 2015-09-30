using System;

namespace BabyData.Authentication
{
	/// <summary>
	/// Base for all exceptions from the Auth System
	/// </summary>
	public class AuthException:Exception{
		public AuthException(string message)
			: base( message){}
		public AuthException(string message,Exception innerException )
			: base( message, innerException){}
	}
	/// <summary>
	/// Invalid token exception.
	/// 
	/// Returned if the auth token is invlaid
	/// </summary>
	public class InvalidTokenException:AuthException{
		public InvalidTokenException(string message)
			: base( message){}
		public InvalidTokenException(string message,Exception innerException )
			: base( message, innerException){}
	}
	/// <summary>
	/// Returned if the system cannot log the user in.
	/// </summary>
	public class InvalidLoginException:AuthException{
		public InvalidLoginException(string message)
			: base( message){}
		public InvalidLoginException(string message,Exception innerException )
			: base( message, innerException){}
	
	}
	/// <summary>
	/// Registration exception.
	/// 
	/// Reutrned if Registration failed.
	/// </summary>
	public class RegistrationException:AuthException{
		public RegistrationException(string message)
			: base( message){}
		public RegistrationException(string message,Exception innerException )
			: base( message, innerException){}

	}


}

