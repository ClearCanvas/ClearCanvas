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

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	/// <summary>
	/// WCF client proxy for <see cref="IStudyLocator"/> services.
	/// </summary>
	public class StudyLocatorServiceClient : ClientBase<IStudyLocator>, IStudyLocator
	{
		/// <summary>
		/// Constructor - uses default configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyLocatorServiceClient() {}

		/// <summary>
		/// Constructor - uses input configuration name to configure endpoint and bindings.
		/// </summary>
		public StudyLocatorServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName) {}

		/// <summary>
		/// Constructor - uses input endpoint and binding.
		/// </summary>
		public StudyLocatorServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress) {}

		/// <summary>
		/// Constructor - uses input endpoint, loads bindings from given configuration name.
		/// </summary>
		public StudyLocatorServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress) {}

		#region Implementation of IStudyLocator

		public LocateStudiesResult LocateStudies(LocateStudiesRequest request)
		{
			return Channel.LocateStudies(request);
		}

		public LocateSeriesResult LocateSeries(LocateSeriesRequest request)
		{
			return Channel.LocateSeries(request);
		}

		public LocateImagesResult LocateImages(LocateImagesRequest request)
		{
			return Channel.LocateImages(request);
		}

		#endregion
	}
}