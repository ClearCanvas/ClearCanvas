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

using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Externals
{
	public interface IExternal : INotifyPropertyChanged
	{
		string Name { get; set; }
		string Label { get; set; }
		bool Enabled { get; set; }
		WindowStyle WindowStyle { get; set; }

		bool IsValid { get; }
	}

	public abstract class External : IExternal
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private WindowStyle _windowStyle = WindowStyle.Normal;
		private string _name = string.Empty;
		private string _label = string.Empty;
		private bool _enabled = true;

		protected External() {}

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
			get { return this._windowStyle; }
			set
			{
				if (_windowStyle != value)
				{
					_windowStyle = value;
					this.NotifyPropertyChanged("WindowStyle");
				}
			}
		}

		public virtual bool IsValid
		{
			get { return true; }
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}
	}
}