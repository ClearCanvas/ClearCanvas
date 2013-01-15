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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Abstract base class for <see cref="IStudyLoader"/>.
	/// </summary>
	public abstract class StudyLoader : IStudyLoader
	{
		private readonly string _name;
        private IDicomServiceNode _currentServer;

		/// <summary>
		/// Constructs a new <see cref="StudyLoader"/> with the given <paramref name="name"/>.
		/// </summary>
		protected StudyLoader(string name)
		{
			_name = name;
		}

		#region IStudyLoader Members

		/// <summary>
		/// Gets the name of the study loader.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

	    /// <summary>
	    /// Gets or sets the study loader's pixel data prefetching strategy.
	    /// </summary>
	    public IPrefetchingStrategy PrefetchingStrategy { get; protected set; }

	    /// <summary>
		/// Called by <see cref="Start"/> to begin prefetching.
		/// </summary>
		protected abstract int OnStart(StudyLoaderArgs studyLoaderArgs);

		/// <summary>
		/// Creates a <see cref="Sop"/> from the given <see cref="ISopDataSource"/>.
		/// </summary>
		protected virtual Sop CreateSop(ISopDataSource dataSource)
		{
			return Sop.Create(dataSource);
		}

		/// <summary>
		/// Loads the next <see cref="SopDataSource"/> from which a
		/// <see cref="Sop"/> will be created.
		/// </summary>
		/// <returns>The next <see cref="SopDataSource"/> or <b>null</b> if there are none remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="StudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
		protected abstract SopDataSource LoadNextSopDataSource();


		/// <summary>
		/// Starts the enumeration of images that match the specified
		/// Study Instance UID.
		/// </summary>
		/// <param name="studyLoaderArgs"></param>
		/// <returns>Number of images in study.</returns>
		public int Start(StudyLoaderArgs studyLoaderArgs)
		{
            if (studyLoaderArgs.Server != null)
			    _currentServer = studyLoaderArgs.Server.ToServiceNode();

			try
			{
				return OnStart(studyLoaderArgs);
			}
			catch(LoadStudyException)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
			}
		}

		/// <summary>
		/// Loads the next <see cref="Sop"/>.
		/// </summary>
		/// <returns>The next <see cref="Sop"/> or <b>null</b> if there are none remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="IStudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
        public Sop LoadNextSop()
		{
		    SopDataSource dataSource = LoadNextSopDataSource();
		    if (dataSource == null)
		    {
		        _currentServer = null;
		        return null;
		    }

		    dataSource.Server = _currentServer;
		    return CreateSop(dataSource);
		}

	    #endregion

		/// <summary>
		/// Creates all available study loaders.
		/// </summary>
		/// <returns>All the loaders, or an empty array if none exist.</returns>
		public static List<IStudyLoader> CreateAll()
		{
			try
			{
			    return new StudyLoaderExtensionPoint().CreateExtensions().Cast<IStudyLoader>().ToList();
			}
			catch (NotSupportedException)
			{
                return new List<IStudyLoader>();
			}
		}

		/// <summary>
		/// Creates a single study loader, if it exists.
		/// </summary>
		/// <returns>The loader, or null if it doesn't exist.</returns>
		public static IStudyLoader Create(string name)
		{
		    return CreateAll().FirstOrDefault(loader => loader.Name == name);
		}
	}
}
