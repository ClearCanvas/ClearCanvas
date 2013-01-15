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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters
{
	[ExtensionPoint]
	public sealed class AutoFilterToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IAutoFilterToolContext : IToolContext
	{
		StudyFilterColumn Column { get; }
	}

	public abstract class AutoFilterTool : Tool<IAutoFilterToolContext>
	{
		public event EventHandler VisibleChanged;

		private bool _visible = true;

		public bool Visible
		{
			get { return _visible; }
			protected set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(this.VisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public StudyFilterColumn Column
		{
			get { return base.Context.Column; }
		}

		public CompositeFilterPredicate AutoFilterRoot
		{
			get { return base.Context.Column.AutoFilterRoot; }
		}

		public IStudyFilter StudyFilter
		{
			get { return base.Context.Column.Owner; }
		}

		protected virtual bool IsColumnSupported()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Visible = this.IsColumnSupported();
		}
	}
}