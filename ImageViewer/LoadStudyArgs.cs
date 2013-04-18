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

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Holds the parameters that specify the study to be loaded.
	/// </summary>
	public class LoadStudyArgs
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="server"></param>
		public LoadStudyArgs(string studyInstanceUid, IDicomServiceNode server)
            : this(studyInstanceUid, server, null)
        {}

	    /// <summary>
	    /// Constructor.
	    /// </summary>
	    /// <param name="studyInstanceUid"></param>
	    /// <param name="server"></param>
        /// <param name="studyLoaderOptions"> </param>
        public LoadStudyArgs(string studyInstanceUid, IDicomServiceNode server, StudyLoaderOptions studyLoaderOptions)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUids");
            Platform.CheckForNullReference(server, "server");
            StudyInstanceUid = studyInstanceUid;
            Server = server;
            StudyLoaderOptions = studyLoaderOptions ?? new StudyLoaderOptions();
        }

        /// <summary>
	    /// Gets the Study Instance UID of the study to be loaded.
	    /// </summary>
	    public string StudyInstanceUid { get; private set; }

        /// <summary>
	    /// Gets the options requested by the caller.
	    /// </summary>
	    public StudyLoaderOptions StudyLoaderOptions { get; private set; }

        /// <summary>
        /// Gets the server from which the study can be loaded.
        /// </summary>
	    public IDicomServiceNode Server { get; private set; }
	}
}
