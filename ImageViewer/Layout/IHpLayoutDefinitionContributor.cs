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

using ClearCanvas.ImageViewer.StudyManagement;
namespace ClearCanvas.ImageViewer.Layout
{
	public interface IHpLayoutDefinitionContext
	{
		/// <summary>
		/// Gets the primary study.
		/// </summary>
		Study PrimaryStudy { get; }

        /// <summary>
        /// Gets the study tree.
        /// </summary>
        StudyTree StudyTree { get; }

        /// <summary>
        /// Gets the relevant logical workspace.
        /// </summary>
        ILogicalWorkspace LogicalWorkspace { get; }
        
        /// <summary>
		/// Gets the relevant physical workspace.
		/// </summary>
		IPhysicalWorkspace PhysicalWorkspace { get; }
    }

	/// <summary>
	/// Defines the interface to a "layout definition" contributor.
	/// </summary>
	public interface IHpLayoutDefinitionContributor : IHpContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpLayoutDefinitionContext context);

		/// <summary>
		/// Applies the state to the specified context.
		/// </summary>
		/// <param name="context"></param>
		void ApplyTo(IHpLayoutDefinitionContext context);
	}
}
