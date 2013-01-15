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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// A base class for tools that operate on <see cref="IGraphic"/>
	/// objects.
	/// </summary>
	public abstract class GraphicTool : Tool<IGraphicToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		private bool _visible = true;
		private event EventHandler _visibleChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the tool is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the tool is visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			protected set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(_visibleChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Visible"/> property has changed.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add { _visibleChanged += value; }
			remove { _visibleChanged -= value; }
		}
	}
}
