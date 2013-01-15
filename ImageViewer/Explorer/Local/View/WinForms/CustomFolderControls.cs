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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	internal class CustomFolderTree : FolderTree
	{
		private event EventHandler<ItemEventArgs<Exception>> _exceptionRaised;

		public event EventHandler<ItemEventArgs<Exception>> ExceptionRaised
		{
			add { _exceptionRaised += value; }
			remove { _exceptionRaised -= value; }
		}

		protected override void HandleBrowseException(Exception exception)
		{
			EventsHelper.Fire(_exceptionRaised, this, new ItemEventArgs<Exception>(exception));
		}

		protected override void HandleInitializationException(Exception exception)
		{
			Platform.Log(LogLevel.Error, exception, "Failed to initialize the {0} control.", this.GetType().Name);
		}
	}

	internal class CustomFolderView : FolderView
	{
		private event EventHandler<ItemEventArgs<Exception>> _exceptionRaised;

		public event EventHandler<ItemEventArgs<Exception>> ExceptionRaised
		{
			add { _exceptionRaised += value; }
			remove { _exceptionRaised -= value; }
		}

		protected override void HandleBrowseException(Exception exception)
		{
			EventsHelper.Fire(_exceptionRaised, this, new ItemEventArgs<Exception>(exception));
		}

		protected override void HandleInitializationException(Exception exception)
		{
			Platform.Log(LogLevel.Error, exception, "Failed to initialize the {0} control.", this.GetType().Name);
		}
	}
}