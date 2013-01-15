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

using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    /// <summary>
    /// Tells the study loader what to check when loading a study.
    /// Note: it's up to the implementation of the study loader to decide on how to handle the instructions. 
    /// </summary>
    public class StudyLoaderCheckOptions
    {
        private static readonly StudyLoaderCheckOptions _default = new StudyLoaderCheckOptions(true);

        public static StudyLoaderCheckOptions Default{ get { return _default; }}

        /// <summary>
        /// Create an instance of <see cref="StudyLoaderCheckOptions"/> with the given instructions
        /// </summary>
        /// <param name="checkIfStudyIsInUse"></param>
        public StudyLoaderCheckOptions(bool checkIfStudyIsInUse)
        {
            CheckInUse = checkIfStudyIsInUse;
        }

        /// <summary>
        /// Indicates if the study loader should check if the study is "in use" when loading the study.
        /// The study loader will throw <see cref="InUseLoadStudyException"/> if the study is in use.
        /// </summary>
        public bool CheckInUse { get; private set; }

    }

	/// <summary>
	/// Holds the parameters that specify the study to be loaded.
	/// </summary>
	public class StudyLoaderArgs
	{
		private readonly string _studyInstanceUid;
        private readonly IApplicationEntity _server;

		/// <summary>
		/// Constructs a new <see cref="StudyLoaderArgs"/> using the specified parameters.
		/// </summary>
		/// <param name="studyInstanceUid">The Study Instance UID of the study to be loaded.</param>
		/// <param name="server">The server from which the study should be loaded.</param>
		public StudyLoaderArgs(string studyInstanceUid, IApplicationEntity server)
            :this(studyInstanceUid, server, StudyLoaderCheckOptions.Default)
		{
		    
		}

        /// <summary>
        /// Constructs a new <see cref="StudyLoaderArgs"/> using the specified parameters.
        /// </summary>
        /// <param name="studyInstanceUid">The Study Instance UID of the study to be loaded.</param>
        /// <param name="server">The server from which the study should be loaded.</param>
        public StudyLoaderArgs(string studyInstanceUid, IApplicationEntity server, StudyLoaderCheckOptions options)
        {
            _studyInstanceUid = studyInstanceUid;
            _server = server;

            StudyCheckOptions = options;
        }

        public StudyLoaderCheckOptions StudyCheckOptions { get; private set; }

		/// <summary>
		/// Gets the Study Instance UID of the study to be loaded.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		/// <summary>
		/// Gets the server to load the study from.
		/// </summary>
        public IApplicationEntity Server
		{
			get { return _server; }
		}
	}

    //TODO (Marmot): Can this stuff be moved to Common? Arguably almost everything in this namespace could move.
	/// <summary>
	/// Defines a study loader.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyLoader"/> abstracts the loading of studies,
	/// allowing different many means of loading studies (e.g., local file system,
	/// DICOM WADO, DICOMDIR CD, streaming, etc.) to be treated in the same way.
	/// </remarks>
    public interface IStudyLoader
    {
		/// <summary>
		/// Gets the name of the study loader.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the study loader's pixel data prefetching strategy.
		/// </summary>
		IPrefetchingStrategy PrefetchingStrategy { get; }

		/// <summary>
		/// Starts the enumeration of images that match the specified
		/// Study Instance UID.
		/// </summary>
		/// <param name="studyLoaderArgs"></param>
		/// <returns>Number of images in study.</returns>
		int Start(StudyLoaderArgs studyLoaderArgs);

		/// <summary>
		/// Loads the next <see cref="Sop"/>.
		/// </summary>
		/// <returns>The next <see cref="Sop"/> or <b>null</b> if there are none remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="IStudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
		Sop LoadNextSop();
    }
}
