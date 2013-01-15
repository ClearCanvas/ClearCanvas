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

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Container for arguments passed to <see cref="IViewerIntegration.ViewStudies"/>.
	/// </summary>
	public class ViewStudiesArgs
	{
		/// <summary>
		/// The set of studies to be viewed.
		/// </summary>
		public string[] StudyInstanceUids { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether to open a separate viewer instance for each study.
		/// </summary>
		/// <remarks>
		/// If multiple study instance UIDs are specified in <see cref="StudyInstanceUids"/>, setting
		/// this property to true indicates that a viewer instance should be opened for each study,
		/// with that study as the viewer's primary study.  If this property is set to false, a single
		/// viewer instance will be opened containing all specified studies.  In this case, there
		/// is no control over which study is 'primary'.
		/// </remarks>
		public bool InstancePerStudy { get; set; }
	}


	/// <summary>
	/// Defines the interface to an object that provides image viewing functionality.
	/// </summary>
	public interface IViewerIntegration
	{
		/// <summary>
		/// Opens the specified studies for viewing.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>One or more instances of a study viewer.</returns>
		/// <remarks>
		/// Call this method to request that studies matching the specified UIDs be
		/// displayed to the user in one or more image viewers.  Note that the implementation
		/// may return an already open viewer matching the specified arguments, if one exists.
		/// </remarks>
		IStudyViewer[] ViewStudies(ViewStudiesArgs args);
	}

	/// <summary>
	/// Defines an interface to an open study viewer.
	/// </summary>
	public interface IStudyViewer
	{
		/// <summary>
		/// Ensures this viewer is the currently active viewer.
		/// </summary>
		/// <returns>True if the viewer was activated, or false if the viewer could not be activated.</returns>
		/// <remarks>
		/// In the event this viewer has already been closed (possibly by the user), this method does nothing and
		/// returns false. It does not attempt to re-open the viewer.
		/// </remarks>
		bool Activate();


		/// <summary>
		/// Closes this viewer.
		/// </summary>
		/// <remarks>If this viewer has already been closed, this method is a no-op.</remarks>
		void Close();
	}
}
