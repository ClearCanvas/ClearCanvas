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
	public interface IHpImageBoxDefinitionContext : IHpLayoutDefinitionContext // should probably inherit everything from layout level context
	{
	    //TODO (CR July 2011): Not entirely sure about this, but if we don't do it,
        //then the "prior placeholder" has values for W/L, etc even when the display set wasn't captured.
        /// <summary>
        /// Gets whether or not the display set itself was successfully captured.
        /// </summary>
        bool DisplaySetCaptured { get; set; }

		/// <summary>
		/// Gets the relevant image box.
		/// </summary>
		IImageBox ImageBox { get; }
	}

	/// <summary>
	/// Defines the interface to an "imagebox definition" contributor.
	/// </summary>
	public interface IHpImageBoxDefinitionContributor : IHpContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpImageBoxDefinitionContext context);

		/// <summary>
		/// Applies the state to the specified context.
		/// </summary>
		/// <param name="context"></param>
		void ApplyTo(IHpImageBoxDefinitionContext context);
	}
}
