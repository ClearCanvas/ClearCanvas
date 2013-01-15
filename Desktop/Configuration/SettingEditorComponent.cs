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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="SettingEditorComponent"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class SettingEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Used by the <see cref="SettingsManagementComponent"/> to show
    /// the default and current values of a setting, allowing the current value to be edited.
    /// </summary>
    [AssociateView(typeof(SettingEditorComponentViewExtensionPoint))]
    public class SettingEditorComponent : ApplicationComponent
    {
        private string _defaultValue;
        private string _currentValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingEditorComponent(string defaultValue, string currentValue)
        {
            _defaultValue = defaultValue;
            _currentValue = currentValue;
        }

        #region Presentation Model

		/// <summary>
		/// Gets the default setting value.
		/// </summary>
        public string DefaultValue
        {
            get { return _defaultValue; }
        }

		/// <summary>
		/// Gets or sets the current setting value.
		/// </summary>
        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                this.Modified = true;
            }
        }

		/// <summary>
		/// The user has accepted the changes (if any).
		/// </summary>
        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();
        }

		/// <summary>
		/// The user has cancelled the changes (if any).
		/// </summary>
		public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

        #endregion
    }
}
