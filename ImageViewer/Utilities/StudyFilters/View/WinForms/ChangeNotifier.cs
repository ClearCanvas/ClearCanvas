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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	internal class ChangeNotifier<T> where T : class
	{
		public delegate void ValueChangeEventHandler(T oldValue, T newValue);

		public event ValueChangeEventHandler BeforeValueChange;
		public event ValueChangeEventHandler AfterValueChange;

		public ChangeNotifier() {}

		public ChangeNotifier(ValueChangeEventHandler beforeValueChangeEventHandler, ValueChangeEventHandler afterValueChangeEventHandler)
		{
			this.BeforeValueChange += beforeValueChangeEventHandler;
			this.AfterValueChange += afterValueChangeEventHandler;
		}

		private T _value;

		public T Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					T oldValue = _value;
					T newValue = value;

					if (this.BeforeValueChange != null)
						this.BeforeValueChange(oldValue, newValue);

					_value = value;

					if (this.AfterValueChange != null)
						this.AfterValueChange(oldValue, newValue);
				}
			}
		}
	}
}