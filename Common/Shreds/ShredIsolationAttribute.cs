using System;

namespace ClearCanvas.Common.Shreds
{
	public enum ShredIsolationLevel
	{
		/// <summary>
		/// Dedicate a new AppDomain to the shred.
		/// </summary>
		OwnAppDomain = 0,
		/// <summary>
		/// The shred will be instantiated in the default AppDomain for the main shredhost service process.
		/// </summary>
		None
		//NamedAppDomain?, NewProcess?
	}

	/// <summary>
	/// Use to specify how a given shred should be isolated, if at all.
	/// </summary>
	public class ShredIsolationAttribute : Attribute
	{
		public ShredIsolationAttribute()
		{
			Level = ShredIsolationLevel.OwnAppDomain;
		}

		/// <summary>
		/// Specifies the level of isolation for a <see cref="IShred">shred</see>.
		/// </summary>
		public ShredIsolationLevel Level;
	}
}
