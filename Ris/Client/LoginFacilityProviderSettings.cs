using System;
using System.Configuration;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Acts a local cache for the LoginFacilityProvider.
	/// </summary>
	/// <remarks>
	/// Note that although these are "user"-scoped settings, they are not actually associated with a RIS user.
	/// Rather, they are associated with the Windows login of the local machine. As such, they behave
	/// more as application settings, which is the intent.
	/// </remarks>
	[SettingsGroupDescription("Acts a local cache for the LoginFacilityProvider.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	internal sealed partial class LoginFacilityProviderSettings
	{
		public bool? IsRisCoreFeatureLicensed
		{
			get
			{
				var bstr = IsRisCoreFeatureLicensedInternal;
				return string.IsNullOrEmpty(bstr) ? (bool?) null : bool.Parse(bstr);
			}
			set
			{
				IsRisCoreFeatureLicensedInternal = value.HasValue ? Convert.ToString(value.Value) : null;
			}
		}
	}
}
