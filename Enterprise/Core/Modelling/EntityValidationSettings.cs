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

using System.Configuration;

namespace ClearCanvas.Enterprise.Core.Modelling
{

	[SettingsGroupDescription("Settings that affect the behaviour of custom entity validation rules.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class EntityValidationSettings
	{
		///<summary>
		/// Public constructor.  Server-side settings classes should be instantiated via constructor rather
		/// than using the <see cref="EntityValidationSettings.Default"/> property to avoid creating a static instance.
		///</summary>
		public EntityValidationSettings()
		{
		}
	}
}
