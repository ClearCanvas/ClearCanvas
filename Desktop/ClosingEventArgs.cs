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
    /// Defines possible reasons why a <see cref="DesktopObject"/> might close.
    /// </summary>
    [Flags]
    public enum CloseReason
    {
        /// <summary>
        /// The close request was initiated by the user via the user-interface.
        /// </summary>
        UserInterface = 0x0001,

        /// <summary>
        /// The close request was initiated by the application.
        /// </summary>
        Program = 0x0002,

        /// <summary>
        /// The close request is occuring because the application has been asked to terminate.
        /// </summary>
        /// <remarks>
        /// The <see cref="Application.Quit"/> API may have been invoked, or the request
        /// may have come from the operating system.
		/// </remarks>
        ApplicationQuit = 0x0004,

        /// <summary>
        /// The object is being closed because it's parent window is closing.
        /// </summary>
		/// <remarks>
		/// Applicable to <see cref="Workspace"/> and <see cref="Shelf"/> objects.
		/// This value is combined with one of <see cref="UserInterface"/>, 
		/// <see cref="Program"/> or <see cref="ApplicationQuit"/>, indicating why the parent is closing.
		/// </remarks>
		ParentClosing = 0x0010

    }

    /// <summary>
    /// Provides data for Closing events, where the request may need to be cancelled.
    /// </summary>
    public class ClosingEventArgs : EventArgs
    {
        private CloseReason _reason;
        private bool _cancel;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="reason">The reason the <see cref="DesktopObject"/> is closing.</param>
		/// <param name="interaction">The user interaction policy for the closing object.</param>
		internal ClosingEventArgs(CloseReason reason, UserInteraction interaction)
            : this(reason, interaction, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="reason">The reason the <see cref="DesktopObject"/> is closing.</param>
		/// <param name="interaction">The user interaction policy for the closing object.</param>
		/// <param name="cancel">A boolean value indicating whether the close operation should be cancelled.</param>
		internal ClosingEventArgs(CloseReason reason, UserInteraction interaction, bool cancel)
        {
            _reason = reason;
            _interaction = interaction;
            _cancel = cancel;
        }

        /// <summary>
        /// Gets the reason the object is closing.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the user-interaction policy for this closing operation, which handlers must abide by.
        /// </summary>
        public UserInteraction Interaction
        {
            get { return _interaction; }
        }

		/// <summary>
		/// Gets and set whether to cancel the current closing request.  Set to true to prevent the <see cref="DesktopObject"/> from closing.
		/// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = _cancel || value; }
        }
    }

    /// <summary>
    /// Provides data for Closing events, where the request may need to be cancelled.
    /// </summary>
    public class ClosingItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private bool _cancel;
        private CloseReason _reason;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="reason">The reason the <paramref name="item"/> is closing.</param>
		/// <param name="item">The item that is being closed.</param>
		/// <param name="interaction">The user interaction policy for the closing object.</param>
		/// <param name="cancel">A boolean value indicating whether the close operation should be cancelled.</param>
		internal ClosingItemEventArgs(TItem item, CloseReason reason, UserInteraction interaction, bool cancel)
            :base(item)
        {
            _reason = reason;
            _cancel = cancel;
            _interaction = interaction;
        }

        /// <summary>
        /// Gets the reason the item is closing.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the user interaction policy for this closing operation, which handlers must abide by.
        /// </summary>
        public UserInteraction Interaction
        {
            get { return _interaction; }
        }


        // maybe we can expose this later if needed
        internal bool Cancel
        {
            get { return _cancel; }
            set
            {
                // don't allow uncancelling
                _cancel = _cancel || value;
            }
        }
    }
}
