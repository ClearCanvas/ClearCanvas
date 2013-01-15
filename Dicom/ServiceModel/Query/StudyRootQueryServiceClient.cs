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

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// WCF client proxy for <see cref="IStudyRootQuery"/> services.
	/// </summary>
	public class StudyRootQueryServiceClient : ClientBase<IStudyRootQuery>, IStudyRootQuery
	{
		/// <summary>
		/// Constructor - uses default configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyRootQueryServiceClient()
		{
		}

		/// <summary>
		/// Constructor - uses input configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyRootQueryServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint and binding.
		/// </summary>
		public StudyRootQueryServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint, loads bindings from given configuration name.
		/// </summary>
		public StudyRootQueryServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
		}

		#region IStudyRootQuery Members

		/// <summary>
		/// Performs a STUDY level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			return base.Channel.StudyQuery(queryCriteria);
		}

		/// <summary>
		/// Performs a SERIES level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			return base.Channel.SeriesQuery(queryCriteria);
		}

		/// <summary>
		/// Performs an IMAGE level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			return base.Channel.ImageQuery(queryCriteria);
		}

		#endregion
	}
}
