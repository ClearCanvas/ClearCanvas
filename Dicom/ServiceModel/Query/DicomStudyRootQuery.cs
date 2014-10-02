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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// A simple wrapper class that implements the <see cref="IStudyRootQuery"/> service contract,
	/// but internally uses a <see cref="StudyRootFindScu"/>.
	/// </summary>
	public class DicomStudyRootQuery : IStudyRootQuery
	{
		private readonly string _localAE;
	    private readonly IApplicationEntity _remoteAE;
		private readonly int _maxResults;

		public DicomStudyRootQuery(string localAETitle, string remoteAETitle, string remoteHost, int remotePort)
            : this(localAETitle, new ApplicationEntity{Name = remoteAETitle, AETitle  = remoteAETitle, ScpParameters = new ScpParameters(remoteHost, remotePort)})
		{
		}

		public DicomStudyRootQuery(string localAETitle, IApplicationEntity remoteAE)
		{
		    _localAE = localAETitle;
		    _remoteAE = remoteAE;
		}

	    #region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			var request = new StudyQueryRequest
				{
					Criteria = queryCriteria,
					LocalApplicationEntity = _localAE,
					RemoteApplicationEntity = new ApplicationEntity(_remoteAE)
				};

			var query = new DicomStudyRootQueryApplication();
			var result = query.StudyQuery(request);
			return result.Results;
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			var request = new SeriesQueryRequest
			{
				Criteria = queryCriteria,
				LocalApplicationEntity = _localAE,
				RemoteApplicationEntity = new ApplicationEntity(_remoteAE)
			};

			var query = new DicomStudyRootQueryApplication();
			var result = query.SeriesQuery(request);
			return result.Results;
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			var request = new ImageQueryRequest
			{
				Criteria = queryCriteria,
				LocalApplicationEntity = _localAE,
				RemoteApplicationEntity = new ApplicationEntity(_remoteAE)
			};

			var query = new DicomStudyRootQueryApplication();
			var result = query.ImageQuery(request);
			return result.Results;
		}

		#endregion

		public override string ToString()
		{
			return _remoteAE.ToString();
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
