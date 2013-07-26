using System;
using System.Web.Http;

namespace ClearCanvas.Enterprise.Core.Web
{
	/// <summary>
	/// Defines error types that can be returned to a REST client.
	/// </summary>
	public struct ErrorType : IEquatable<ErrorType>
	{
		/// <summary>
		/// The request was invalid.
		/// </summary>
		public static ErrorType InvalidRequest = new ErrorType("InvalidRequest");

		/// <summary>
		/// An object referenced by the request or its arguments could not be not found.
		/// </summary>
		public static ErrorType ObjectNotFound = new ErrorType("ObjectNotFound");


		/// <summary>
		/// An object could not be updated because it was modified by another user.
		/// </summary>
		public static ErrorType ConcurrentModification = new ErrorType("ConcurrentModification");

		/// <summary>
		/// An attempt to login with specified credentials was denied because the credentials are invalid or the account is disabled.
		/// </summary>
		public static ErrorType LoginAccessDenied = new ErrorType("LoginAccessDenied");

		/// <summary>
		/// An attempt to login with specified credentials was denied because the password has expired.
		/// </summary>
		public static ErrorType PasswordExpired = new ErrorType("PasswordExpired");


		public readonly string Key;

		public ErrorType(string key)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");

			this.Key = key;
		}

		/// <summary>
		/// Construct an <see cref="HttpError"/> object with the specified message and data.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public HttpError CreateHttpError(string message, object data = null)
		{
			var error = new HttpError(message);
			error["Error"] = this.Key;
			if (data != null)
			{
				error["Data"] = data;
			}
			return error;
		}

		public override string ToString()
		{
			return Key;
		}

		public bool Equals(ErrorType other)
		{
			return Equals(other.Key, Key);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (ErrorType)) return false;
			return Equals((ErrorType) obj);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		public static bool operator ==(ErrorType left, ErrorType right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ErrorType left, ErrorType right)
		{
			return !left.Equals(right);
		}
	}
}