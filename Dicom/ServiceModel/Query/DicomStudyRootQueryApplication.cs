#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// A simple wrapper class that implements <see cref="IStudyRootQueryApplication"/>.
	/// </summary>
	/// <remarks>
	/// The wrapper is intended to be able operate independently from our applications, all parameters and methods for performing
	/// the query are self contained.  IE, the remote application configuration is passed in.
	/// </remarks>
	public class DicomStudyRootQueryApplication : IStudyRootQueryApplication
	{
		#region IRemoteStudyRootQuery

		public virtual StudyQueryResponse StudyQuery(StudyQueryRequest request)
		{
			var response = new StudyQueryResponse();
			var result = Query<StudyRootStudyIdentifier, StudyRootFindScu>(request.Criteria, request, response);
			response.Results = (List<StudyRootStudyIdentifier>) result;
			return response;
		}

		public virtual SeriesQueryResponse SeriesQuery(SeriesQueryRequest request)
		{
			var response = new SeriesQueryResponse();
			var result = Query<SeriesIdentifier, StudyRootFindScu>(request.Criteria, request, response);
			response.Results = (List<SeriesIdentifier>) result;
			return response;
		}

		public virtual ImageQueryResponse ImageQuery(ImageQueryRequest request)
		{
			var response = new ImageQueryResponse();
			var result = Query<ImageIdentifier, StudyRootFindScu>(request.Criteria, request, response);
			response.Results = (List<ImageIdentifier>) result;
			return response;
		}

		#endregion

		private IList<TIdentifier> Query<TIdentifier, TFindScu>(TIdentifier queryCriteria, QueryRequest request, QueryResponse response)
			where TIdentifier : Identifier, new()
			where TFindScu : FindScuBase, new()
		{
			Platform.CheckForEmptyString(request.LocalApplicationEntity, "localAE");
			Platform.CheckForNullReference(request.RemoteApplicationEntity, "remoteAE");

			Platform.CheckForEmptyString(request.RemoteApplicationEntity.AETitle, "AETitle");

			Platform.CheckForNullReference(request.RemoteApplicationEntity.ScpParameters, "ScpParameters");
			Platform.CheckArgumentRange(request.RemoteApplicationEntity.ScpParameters.Port, 1, 65535, "Port");
			Platform.CheckForEmptyString(request.RemoteApplicationEntity.ScpParameters.HostName, "HostName");

			if (queryCriteria == null)
			{
				const string message = "The query identifier cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IList<DicomAttributeCollection> scuResults;
			using (var scu = new TFindScu())
			{
				if (request.MaxResults.HasValue)
					scu.MaxResults = request.MaxResults.Value;

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
					var fault = new DataValidationFault
					            {
						            Description = "Failed to convert contract object to DicomAttributeCollection."
					            };
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}
				catch (Exception e)
				{
					var fault = new DataValidationFault
					            {
						            Description = "Unexpected exception when converting contract object to DicomAttributeCollection."
					            };
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}
				finally
				{
					queryCriteria.SpecificCharacterSet = oldCharacterSet;
				}

				try
				{
					scuResults = scu.Find(request.LocalApplicationEntity, request.RemoteApplicationEntity.AETitle, request.RemoteApplicationEntity.ScpParameters.HostName, request.RemoteApplicationEntity.ScpParameters.Port, criteria);
					scu.Join();

					if (scu.Status == ScuOperationStatus.ConnectFailed)
					{
						String message = String.Format("Connection failed ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						HandleError(scu, request, message);
					}
					if (scu.Status == ScuOperationStatus.AssociationRejected)
					{
						String message = String.Format("Association rejected ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						HandleError(scu, request, message);
					}
					if (scu.Status == ScuOperationStatus.Failed)
					{
						String message = String.Format("The query operation failed ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						HandleError(scu, request, message);
					}
					if (scu.Status == ScuOperationStatus.TimeoutExpired)
					{
						String message = String.Format("The connection timeout expired ({0})",
						                               scu.FailureDescription ?? "no failure description provided");
						HandleError(scu, request, message);
					}
					if (scu.Status == ScuOperationStatus.NetworkError)
					{
						String message = String.Format("An unexpected network error has occurred.");
						HandleError(scu, request, message);
					}
					if (scu.Status == ScuOperationStatus.UnexpectedMessage)
					{
						String message = String.Format("An unexpected message was received; aborted association.");
						HandleError(scu, request, message);
					}

					response.MaxResultsReached = scu.Status == ScuOperationStatus.Canceled;

					// Narrow down the results to the exact amount asked for
					if (scu.MaxResults != -1 && scu.MaxResults < scuResults.Count)
					{
						response.MaxResultsReached = true;
						scuResults = scuResults.Take(scu.MaxResults).ToList();
					}
				}
				catch (FaultException)
				{
					throw;
				}
				catch (Exception e)
				{
					var fault = new QueryFailedFault
					            {
						            Description = String.Format("An unexpected error has occurred ({0})",
						                                        scu.FailureDescription ?? "no failure description provided")
					            };
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
				}
			}

			var results = new List<TIdentifier>(scuResults.Count);
			foreach (DicomAttributeCollection result in scuResults)
			{
				var identifier = Identifier.FromDicomAttributeCollection<TIdentifier>(result);
				if (String.IsNullOrEmpty(identifier.RetrieveAeTitle) || identifier.RetrieveAeTitle == request.RemoteApplicationEntity.AETitle)
					identifier.RetrieveAE = request.RemoteApplicationEntity;

				results.Add(identifier);
			}

			return results;
		}

		private static void HandleError(FindScuBase scu, QueryRequest request, string message)
		{
			if (request.IgnoreFailureOnPartialResults && scu.Results.Any())
			{
				Platform.Log(LogLevel.Info, "Partial results returned with error: {0}", message);
			}
			else
			{
				var fault = new QueryFailedFault {Description = message};
				throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
		}
	}
}