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
	/// <summary>
	/// Defines the interface to an applicability contributor.
	/// </summary>
	public interface IHpApplicabilityContributor : IHpContributor
	{
	}

	public interface IHpProtocolApplicabilityContext
	{
        /// <summary>
        /// The "primary" study for which the protocol applicability will be tested.
        /// </summary>
		Study PrimaryStudy { get; }
	}


	/// <summary>
	/// Defines the interface to a "protocol applicability" contributor.
	/// </summary>
	public interface IHpProtocolApplicabilityContributor : IHpApplicabilityContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpProtocolApplicabilityContext context);

		/// <summary>
		/// Tests the applicability based on the specified context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		HpApplicabilityResult Test(IHpProtocolApplicabilityContext context);
	}

	public interface IHpLayoutApplicabilityContext : IHpProtocolApplicabilityContext
	{
        StudyTree StudyTree { get; }
		ILogicalWorkspace LogicalWorkspace { get; }
	}


	/// <summary>
	/// Defines the interface to a "layout applicability" contributor.
	/// </summary>
	public interface IHpLayoutApplicabilityContributor : IHpApplicabilityContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpLayoutApplicabilityContext context);

		/// <summary>
		/// Tests the applicability based on the specified context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool Test(IHpLayoutApplicabilityContext context);
	}
}
