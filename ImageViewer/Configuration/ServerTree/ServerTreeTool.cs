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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	public abstract class ServerTreeTool : Tool<IServerTreeToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		public ServerTreeTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}
	}
}