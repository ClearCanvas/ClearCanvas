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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Used by <see cref="ClickAction"/> objects to establish a handler for a click.
	/// </summary>
	public delegate void ClickHandlerDelegate();
	
	/// <summary>
    /// Default implementation of <see cref="IClickAction"/> which models a user-interface action that is invoked by
    /// a click, such as a toolbar button or a menu item.
    /// </summary>
    public class ClickAction : Action, IClickAction
    {
		private readonly ClickActionFlags _flags;
        private ClickHandlerDelegate _clickHandler;

	    private bool _checked;
        private event EventHandler _checkedChanged;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that control the style of the action.</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve text and image resources.</param>
        public ClickAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, resourceResolver)
        {
            _flags = flags;
            _checked = false;
        }

        /// <summary>
        /// Sets the delegate that will respond when this action is clicked.
        /// </summary>
        public void SetClickHandler(ClickHandlerDelegate clickHandler)
        {
            _clickHandler = clickHandler;
        }

        #region IClickAction members

	    /// <summary>
	    /// Gets the keystroke that the UI should attempt to intercept to invoke the action.
	    /// </summary>
	    public XKeys KeyStroke { get; set; }

	    /// <summary>
        /// Gets a value indicating whether this action is a "check" action, that is, an action that behaves as a toggle.
        /// </summary>
        public bool IsCheckAction
        {
            get { return (_flags & ClickActionFlags.CheckAction) != 0; }
        }

        /// <summary>
        /// Gets the checked state that the action should present in the UI, if this is a "check" action.
        /// </summary>
        /// <remarks>
        /// This property has no meaning if <see cref="IClickAction.IsCheckAction"/> returns false.
        /// </remarks>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IClickAction.Checked"/> property of this action changes.
        /// </summary>
        public event EventHandler CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }

        /// <summary>
        /// Gets a value indicating whether parent items should be checked if this
        /// <see cref="IClickAction"/> is checked.
        /// </summary>
        public bool CheckParents
		{
			get { return (_flags & ClickActionFlags.CheckParents) == ClickActionFlags.CheckParents; }
		}

        /// <summary>
        /// Called by the UI when the user clicks on the action.
        /// </summary>
        /// <remarks>This method will do nothing when <see cref="CanClick"/> returns false.</remarks>
        public void Click()
        {
            if (_clickHandler != null && CanClick())
                _clickHandler();
        }

        #endregion

        /// <summary>
        /// Determines whether or not the internal "click handler" can be called,
        /// based on the <see cref="IAction.Visible"/>, <see cref="IAction.Enabled"/>
        /// and <see cref="IAction.Permissible"/> properties.
        /// </summary>
        private bool CanClick()
        {
            //Although Visible doesn't technically apply to a KeyboardAction, it's always true
            //by default, and there is little reason for anybody to ever set it to false; hence,
            //why this method is not overridden in KeyboardAction.
            return Visible && Enabled && Permissible;
        }
	}
}
