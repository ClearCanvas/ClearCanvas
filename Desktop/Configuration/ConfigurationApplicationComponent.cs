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

namespace ClearCanvas.Desktop.Configuration
{
	public abstract class ConfigurationApplicationComponentContainer: ApplicationComponentContainer, IConfigurationApplicationComponent
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConfigurationApplicationComponentContainer()
		{
		}

		/// <summary>
		/// Save any settings modified in the hosted component.
		/// </summary>
		public abstract void Save();

		#region IConfigurationApplicationComponent Members

		void IConfigurationApplicationComponent.Save()
		{
			this.Save();
			base.Modified = false;
		}

		#endregion
	}

	/// <summary>
	/// A component that hosts a configuration page, where some settings need to
	/// be saved when the user dismisses it.
	/// </summary>
	public abstract class ConfigurationApplicationComponent : ApplicationComponent, IConfigurationApplicationComponent
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConfigurationApplicationComponent()
		{
		}

		/// <summary>
		/// Save any settings modified in the hosted component.
		/// </summary>
		public abstract void Save();

		#region IConfigurationApplicationComponent Members

		void IConfigurationApplicationComponent.Save()
		{
			this.Save();
			Modified = false;
		}

		#endregion
	}
}
