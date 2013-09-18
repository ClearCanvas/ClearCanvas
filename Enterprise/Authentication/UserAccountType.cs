using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{
	/// <summary>
	/// Enumerates the types of accounts.
	/// </summary>
	[EnumValueClass(typeof(UserAccountTypeEnum))]
	public enum UserAccountType
	{
		/// <summary>
		/// User
		/// </summary>
		[EnumValue("User")]
		U,

		/// <summary>
		/// Group
		/// </summary>
		[EnumValue("Group")]
		G,

		/// <summary>
		/// System
		/// </summary>
		[EnumValue("System")]
		S,
	}
}