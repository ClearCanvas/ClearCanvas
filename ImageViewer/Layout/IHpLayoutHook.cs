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

namespace ClearCanvas.ImageViewer.Layout
{
	public interface IHpLayoutHookContext
	{
		/// <summary>
		/// Gets the relevant image viewer.
		/// </summary>
		IImageViewer ImageViewer { get; }

        /// <summary>
        /// Called by the hook to lay out the default physicial workspace
        /// </summary>
        void PerformDefaultPhysicalWorkspaceLayout();

        /// <summary>
        /// Called by the hook to fill the existing physicial workspace with images
        /// </summary>
        void PerformDefaultFillPhysicalWorkspace();
        
	}

	/// <summary>
	/// Defines an interface that allows a hanging protocols service to hook into
	/// the layout procedure.
	/// </summary>
	public interface IHpLayoutHook
	{
		/// <summary>
		/// Handles the initial layout when a viewer is first opened.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool HandleLayout(IHpLayoutHookContext context);
	}

    /// <summary>
    /// Allows a hanging protocols service to hook into
    /// the layout procedure.
    /// </summary>
    public abstract class HpLayoutHook : IHpLayoutHook
    {
        /// TODO (CR Nov 2011): Rename to "Empty"
        private class Nil : IHpLayoutHook
        {
            #region IHpLayoutHook Members

            public bool HandleLayout(IHpLayoutHookContext context)
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        /// A default hook that can be used if needed; it does nothing.
        /// </summary>
        public static IHpLayoutHook Default = new Nil();

        #region IHpLayoutHook Members

        /// <summary>
        /// Handles the initial layout when a viewer is first opened.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool HandleLayout(IHpLayoutHookContext context);

        #endregion
    }
}
