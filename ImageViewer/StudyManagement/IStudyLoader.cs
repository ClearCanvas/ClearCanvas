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
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    /// <summary>
    /// Basic study loader options; it's up to the implementation of the study loader to decide how to handle the instructions. 
    /// </summary>
    public class StudyLoaderOptions
    {
        /// <summary>
        /// Create an instance of <see cref="StudyLoaderOptions"/> with the default instructions
        /// </summary>
        public StudyLoaderOptions()
            : this(false)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="StudyLoaderOptions"/> with the given instructions
        /// </summary>
        /// <param name="ignoreIfStudyInUse"></param>
        public StudyLoaderOptions(bool ignoreIfStudyInUse)
        {
            IgnoreInUse = ignoreIfStudyInUse;
        }

        /// <summary>
        /// Indicates if the study loader should attempt to ignore if the study is in use
        /// and try to load the study anyway.
        /// </summary>
        /// <remarks>Note that it depends on the study loader whether it is even possible to ignore
        /// the fact that the study is in use and load it anyway. Just because this option is set
        /// does not mean the loader will not still throw an <see cref="InUseLoadStudyException"/>.</remarks>
        public bool IgnoreInUse { get;  set; }
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
	    /// <param name="options"> </param>
        /// TODO (CR Apr 2013): create constructor overload withtout the 'options' and update the code where null is passed in
	    public StudyLoaderArgs(string studyInstanceUid, IApplicationEntity server, StudyLoaderOptions options)
        {
            _studyInstanceUid = studyInstanceUid;
            _server = server;

            Options = options ?? new StudyLoaderOptions();
        }

        public StudyLoaderOptions Options { get; private set; }

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
