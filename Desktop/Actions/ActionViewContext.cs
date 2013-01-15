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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Interface for an <see cref="IActionView"/>'s context.
	/// </summary>
	public interface IActionViewContext
	{
		/// <summary>
		/// Gets the associated <see cref="IAction"/>.
		/// </summary>
		IAction Action { get; }

		/// <summary>
		/// Gets or sets the <see cref="IconSize"/> to be shown by the <see cref="IActionView"/>.
		/// </summary>
		IconSize IconSize { get; set; }

		/// <summary>
		/// Fires when the <see cref="IconSize"/> has changed.
		/// </summary>
		event EventHandler IconSizeChanged;
	}

	/// <summary>
	/// Simple implementation of an <see cref="IActionViewContext"/>.
	/// </summary>
	public class ActionViewContext : IActionViewContext
	{
		private readonly IAction _action;
		private IconSize _iconSize;
		private event EventHandler _iconSizeChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The associated <see cref="IAction"/>.</param>
		public ActionViewContext(IAction action)
			: this(action, default(IconSize))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The associated <see cref="IAction"/>.</param>
		/// <param name="iconSize">The initial icon size.</param>
		public ActionViewContext(IAction action, IconSize iconSize)
		{
			Platform.CheckForNullReference(action, "action");
			_action = action;
			_iconSize = iconSize;
		}

		#region IActionViewContext Members

		/// <summary>
		/// Gets the associated <see cref="IAction"/>.
		/// </summary>
		public IAction Action
		{
			get { return _action; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IActionViewContext.IconSize"/> to be shown by the <see cref="IActionView"/>.
		/// </summary>
		public IconSize IconSize
		{
			get
			{
				return _iconSize;
			}
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					EventsHelper.Fire(_iconSizeChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Fires when the <see cref="IActionViewContext.IconSize"/> has changed.
		/// </summary>
		public event EventHandler IconSizeChanged
		{
			add { _iconSizeChanged += value; }
			remove { _iconSizeChanged -= value; }
		}

		#endregion
	}
}