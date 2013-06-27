#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides application settings for check-in.
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Configures behaviour of check-in procedures.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	public sealed class CheckInSettings : global::System.Configuration.ApplicationSettingsBase
	{
		private static CheckInSettings defaultInstance = ((CheckInSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new CheckInSettings())));
		
		public CheckInSettings()
		{
		}

		public static CheckInSettings Default {
			get {
				return defaultInstance;
			}
		}
		
		/// <summary>
		/// Specifies how early a procedure can be checked in without triggering a warning. (in minutes).
		/// </summary>
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.SettingsDescriptionAttribute("Specifies how early a procedure can be checked in without triggering a warning. (in minutes)")]
		[global::System.Configuration.DefaultSettingValueAttribute("120")]
		public int EarlyCheckInWarningThreshold {
			get {
				return ((int)(this["EarlyCheckInWarningThreshold"]));
			}
		}

		/// <summary>
		/// Specifies how late a procedure can be checked in without triggering a warning. (in minutes).
		/// </summary>
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.SettingsDescriptionAttribute("Specifies how late a procedure can be checked in without triggering a warning. (in minutes)")]
		[global::System.Configuration.DefaultSettingValueAttribute("180")]
		public int LateCheckInWarningThreshold {
			get {
				return ((int)(this["LateCheckInWarningThreshold"]));
			}
		}

		public enum ValidateResult { Success, TooEarly, TooLate, NotScheduled }

		public static ValidateResult Validate(DateTime? scheduledTime, DateTime checkInTime, out string message)
		{
			message = "";
			if (scheduledTime == null)
			{
				message = SR.MessageCheckInUnscheduledProcedure;
				return ValidateResult.NotScheduled;
			}

			var earlyBound = scheduledTime.Value.AddMinutes(-Default.EarlyCheckInWarningThreshold);
			var lateBound = scheduledTime.Value.AddMinutes(Default.LateCheckInWarningThreshold);

			if (checkInTime < earlyBound)
			{
				var threshold = TimeSpan.FromMinutes(Default.EarlyCheckInWarningThreshold);
				message = string.Format(SR.MessageAlertCheckingInTooEarly, TimeSpanFormat.FormatDescriptive(threshold));
				return ValidateResult.TooEarly;
			}

			if (checkInTime > lateBound)
			{
				var threshold = TimeSpan.FromMinutes(Default.LateCheckInWarningThreshold);
				message = string.Format(SR.MessageAlertCheckingInTooLate, TimeSpanFormat.FormatDescriptive(threshold));
				return ValidateResult.TooLate;
			}

			return ValidateResult.Success;
		}
	}
}
