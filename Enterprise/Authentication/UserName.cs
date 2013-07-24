using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Authentication
{
	/// <summary>
	/// Utilities for working with UserNames.
	/// </summary>
	static class UserName
	{
		private static readonly List<char> IllegalUserNameChars = new List<char> { '"', '<', '>', '|', ':', ';', '*', '?', '\\', '/' };

		/// <summary>
		/// Returns a value indicating if a potential UserName is legal or not.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static bool IsLegalUserName(string userName)
		{
			return !string.IsNullOrEmpty(userName) && !ContainsIllegalChars(userName);
		}

		/// <summary>
		/// Returns a value indicating if a potential UserName contains illegal characters.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		private static bool ContainsIllegalChars(string userName)
		{
			if (string.IsNullOrEmpty(userName))
				return false;

			// use loop here, rather than Linq, for speed
			for (var i = 0; i < userName.Length; i++)
			{
				var c = userName[i];
				if (c < 0x20 || IllegalUserNameChars.Contains(c))
					return true;
			}
			return false;
		}
	}
}
