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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a single page in a <see cref="StackTabComponentContainer"/>.
	/// </summary>
	public class StackTabPage : TabPage
	{
		private string _title;
		private IconSet _iconSet;
		private IResourceResolver _resourceResolver;

		private event EventHandler _titleChanged;
		private event EventHandler _iconSetChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the page.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in this page.</param>
		/// <param name="title">The text to display on the title bar.</param>
		/// <param name="iconSet">The icon to display on the title bar.</param>
		/// <param name="fallbackResolver">Resource resolver to fall back on in case the default failed to find resources.</param>
		public StackTabPage(string name, 
			IApplicationComponent component, 
			string title, 
			IconSet iconSet,
			IResourceResolver fallbackResolver)
			: base(name, component)
		{
			_title = title;
			_iconSet = iconSet;
			_resourceResolver = new ApplicationThemeResourceResolver(typeof(StackTabPage).Assembly, fallbackResolver);
		}

		/// <summary>
		/// The text to display on the title bar.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title == value)
					return;

				_title = value;
				EventsHelper.Fire(_titleChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ClearCanvas.Desktop.IconSet"/> that should be displayed for the folder.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
			set 
			{
				_iconSet = value;
				EventsHelper.Fire(_iconSetChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the resource resolver that is used to resolve the Icon.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			set { _resourceResolver = value; }
		}

		/// <summary>
		/// Occurs when <see cref="Title"/> has changed.
		/// </summary>
		public event EventHandler TitleChanged
		{
			add { _titleChanged += value; }
			remove { _titleChanged -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="IconSet"/> has changed.
		/// </summary>
		public event EventHandler IconSetChanged
		{
			add { _iconSetChanged += value; }
			remove { _iconSetChanged -= value; }
		}
	}
}
