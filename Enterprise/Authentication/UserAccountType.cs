using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{
	/// <summary>
	/// UserAccountType enumeration
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
		/// Service
		/// </summary>
		[EnumValue("Service")]
		S,
	}
}