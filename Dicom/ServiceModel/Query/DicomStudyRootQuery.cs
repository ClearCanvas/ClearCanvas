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
using System.ServiceModel;
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
			return Query<StudyRootStudyIdentifier, StudyRootFindScu>(queryCriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			return Query<SeriesIdentifier, StudyRootFindScu>(queryCriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			return Query<ImageIdentifier, StudyRootFindScu>(queryCriteria);
		}

		#endregion

		private IList<TIdentifier> Query<TIdentifier, TFindScu>(TIdentifier queryCriteria)
			where TIdentifier : Identifier, new()
			where TFindScu : FindScuBase, new()
		{
			Platform.CheckForEmptyString(_localAE, "localAE");
			Platform.CheckForNullReference(_remoteAE, "remoteAE");

            Platform.CheckForEmptyString(_remoteAE.AETitle, "AETitle");

            Platform.CheckForNullReference(_remoteAE.ScpParameters, "ScpParameters");
            Platform.CheckArgumentRange(_remoteAE.ScpParameters.Port, 1, 65535, "Port");
            Platform.CheckForEmptyString(_remoteAE.ScpParameters.HostName, "HostName");

			if (queryCriteria == null)
			{
				string message = "The query identifier cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IList<DicomAttributeCollection> scuResults;
			using (TFindScu scu = new TFindScu())
			{
				DicomAttributeCollection criteria;

			    var oldCharacterSet = queryCriteria.SpecificCharacterSet;
                const string utf8 = "ISO_IR 192";
                //.NET strings are unicode, so the query criteria are unicode.
                queryCriteria.SpecificCharacterSet = utf8;

				try
				{
					criteria = queryCriteria.ToDicomAttributeCollection();
					criteria[DicomTags.InstanceAvailability] = null;
					criteria[DicomTags.RetrieveAeTitle] = null;
				}
				catch (DicomException e)
				{
					DataValidationFault fault = new DataValidationFault();
					fault.Description = "Failed to convert contract object to DicomAttributeCollection.";
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}
				catch (Exception e)
				{
					DataValidationFault fault = new DataValidationFault();
					fault.Description = "Unexpected exception when converting contract object to DicomAttributeCollection.";
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}
                finally
				{
                    queryCriteria.SpecificCharacterSet = oldCharacterSet;
				}

			    try
				{
					scuResults = scu.Find(_localAE, _remoteAE.AETitle, _remoteAE.ScpParameters.HostName, _remoteAE.ScpParameters.Port, criteria);
					scu.Join();

					if (scu.Status == ScuOperationStatus.Canceled)
					{
						String message = String.Format("The remote server cancelled the query ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.ConnectFailed)
					{
						String message = String.Format("Connection failed ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.AssociationRejected)
					{
						String message = String.Format("Association rejected ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.Failed)
					{
						String message = String.Format("The query operation failed ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.TimeoutExpired)
					{
						String message = String.Format("The connection timeout expired ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.NetworkError)
					{
						String message = String.Format("An unexpected network error has occurred.");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.UnexpectedMessage)
					{
						String message = String.Format("An unexpected message was received; aborted association.");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}

				}
				catch (FaultException)
				{
					throw;
				}
				catch (Exception e)
				{
					QueryFailedFault fault = new QueryFailedFault();
					fault.Description = String.Format("An unexpected error has occurred ({0})",
													  scu.FailureDescription ?? "no failure description provided");
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
				}
			}

			List<TIdentifier> results = new List<TIdentifier>();
			foreach (DicomAttributeCollection result in scuResults)
			{
				TIdentifier identifier = Identifier.FromDicomAttributeCollection<TIdentifier>(result);
                if (String.IsNullOrEmpty(identifier.RetrieveAeTitle) || identifier.RetrieveAeTitle == _remoteAE.AETitle)
                    identifier.RetrieveAE = _remoteAE;

			    results.Add(identifier);
			}

			return results;
		}

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
