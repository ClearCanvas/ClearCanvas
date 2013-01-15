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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Externals
{
	public interface IExternalPropertiesComponent : IApplicationComponent
	{
		void Load(IExternal external);
		void Update(IExternal external);
	}

	public abstract class ExternalPropertiesComponent<T> : ApplicationComponent, IExternalPropertiesComponent where T : IExternal
	{
		private string _name;
		private string _label;
		private bool _enabled;
		private WindowStyle _windowStyle;

		[ValidateLength(1, Message = "MessageValueCannotBeEmpty")]
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					this.NotifyPropertyChanged("Name");
				}
			}
		}

		[ValidateLength(1, Message = "MessageValueCannotBeEmpty")]
		public string Label
		{
			get { return _label; }
			set
			{
				if (_label != value)
				{
					_label = value;
					this.NotifyPropertyChanged("Label");
				}
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					this.NotifyPropertyChanged("Enabled");
				}
			}
		}

		public WindowStyle WindowStyle
		{
			get { return _windowStyle; }
			set
			{
				if (_windowStyle != value)
				{
					_windowStyle = value;
					this.NotifyPropertyChanged("WindowStyle");
				}
			}
		}

		public virtual void Load(T external)
		{
			Platform.CheckForNullReference(external, "external");

			this.Name = external.Name;
			this.Label = external.Label;
			this.Enabled = external.Enabled;
			this.WindowStyle = external.WindowStyle;
			this.Modified = false;
		}

		public virtual void Update(T external)
		{
			Platform.CheckForNullReference(external, "external");

			external.Name = this.Name;
			external.Label = this.Label;
			external.Enabled = this.Enabled;
			external.WindowStyle = this.WindowStyle;
		}

		void IExternalPropertiesComponent.Load(IExternal external)
		{
			this.Load((T) external);
		}

		void IExternalPropertiesComponent.Update(IExternal external)
		{
			this.Update((T) external);
		}
	}
}