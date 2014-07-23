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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	[ExtensionOf(typeof (ServiceNodeServiceProviderExtensionPoint))]
	internal class StudyLoaderServiceProvider : ServiceNodeServiceProvider
	{
		private bool IsStreamingServiceNode
		{
			get
			{
				var dicomServiceNode = Context.ServiceNode as IDicomServiceNode;
				return dicomServiceNode != null && dicomServiceNode.StreamingParameters != null;
			}
		}

		public override bool IsSupported(Type type)
		{
			return type == typeof (IStudyLoader) && IsStreamingServiceNode;
		}

		public override object GetService(Type type)
		{
			return IsSupported(type) ? new StreamingStudyLoader() : null;
		}
	}

	[ExtensionOf(typeof (StudyLoaderExtensionPoint))]
	public class StreamingStudyLoader : StudyLoader
	{
		private const string _loaderName = "CC_STREAMING";

		private IEnumerator<InstanceXml> _instances;
		private IApplicationEntity _serverAe;

		public StreamingStudyLoader()
			: this(_loaderName)
		{
		}

		public StreamingStudyLoader(string name) :
			base(name)
		{
			InitStrategy();
		}

		protected virtual void InitStrategy()
		{
			PrefetchingStrategy = new WeightedWindowPrefetchingStrategy(new StreamingCorePrefetchingStrategy(), _loaderName, SR.DescriptionPrefetchingStrategy)
			                      	{
			                      		Enabled = StreamingSettings.Default.RetrieveConcurrency > 0,
			                      		RetrievalThreadConcurrency = Math.Max(StreamingSettings.Default.RetrieveConcurrency, 1),
			                      		DecompressionThreadConcurrency = Math.Max(StreamingSettings.Default.DecompressConcurrency, 1),
			                      		FrameLookAheadCount = StreamingSettings.Default.ImageWindow >= 0 ? (int?) StreamingSettings.Default.ImageWindow : null,
			                      		SelectedImageBoxWeight = Math.Max(StreamingSettings.Default.SelectedWeighting, 1),
			                      		UnselectedImageBoxWeight = Math.Max(StreamingSettings.Default.UnselectedWeighting, 0)
			                      	};
		}

		protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
		{
			var serverAe = studyLoaderArgs.Server;
			Platform.CheckForNullReference(serverAe, "Server");
			Platform.CheckMemberIsSet(serverAe.StreamingParameters, "StreamingParameters");
			_serverAe = serverAe;

			EventResult result = EventResult.Success;
			AuditedInstances loadedInstances = new AuditedInstances();
			try
			{

				StudyXml studyXml = RetrieveStudyXml(studyLoaderArgs);
				_instances = GetInstances(studyXml).GetEnumerator();

				loadedInstances.AddInstance(studyXml.PatientId, studyXml.PatientsName, studyXml.StudyInstanceUid);

				return studyXml.NumberOfStudyRelatedInstances;

			}
			finally
			{
				AuditHelper.LogOpenStudies(new string[] {_serverAe.AETitle}, loadedInstances, EventSource.CurrentUser, result);
			}
		}

		private IEnumerable<InstanceXml> GetInstances(StudyXml studyXml)
		{
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					yield return instanceXml;
				}
			}
		}

		protected override SopDataSource LoadNextSopDataSource()
		{
			if (!_instances.MoveNext())
				return null;

			var loader = new ImageServerWadoLoader(_serverAe.ScpParameters.HostName,
			                                       _serverAe.AETitle, _serverAe.StreamingParameters.WadoServicePort);
			
			return new StreamingSopDataSource(_instances.Current, loader);
		}

		private StudyXml RetrieveStudyXml(StudyLoaderArgs studyLoaderArgs)
		{
			var headerParams = new HeaderStreamingParameters
			                   	{
			                   		StudyInstanceUID = studyLoaderArgs.StudyInstanceUid,
			                   		ServerAETitle = _serverAe.AETitle,
			                   		ReferenceID = Guid.NewGuid().ToString(),
			                   		IgnoreInUse = studyLoaderArgs.Options != null && studyLoaderArgs.Options.IgnoreInUse
			                   	};

			HeaderStreamingServiceClient client = null;
			try
			{

				string uri = String.Format(StreamingSettings.Default.FormatHeaderServiceUri,
				                           _serverAe.ScpParameters.HostName, _serverAe.StreamingParameters.HeaderServicePort);

				client = new HeaderStreamingServiceClient(new Uri(uri));
				client.Open();
				var studyXml = client.GetStudyXml(ServerDirectory.GetLocalServer().AETitle, headerParams);
				client.Close();
				return studyXml;
			}
			catch (FaultException<StudyIsInUseFault> e)
			{
				throw new InUseLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
			}
			catch (FaultException<StudyIsNearlineFault> e)
			{
				throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e) {IsStudyBeingRestored = e.Detail.IsStudyBeingRestored};
			}
			catch (FaultException<StudyNotFoundFault> e)
			{
				throw new NotFoundLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
			}
			catch (FaultException e)
			{
				//TODO: Some versions (pre-Team) of the ImageServer
				//throw a generic fault when a study is nearline, instead of the more specialized one.
				string message = e.Message.ToLower();
				if (message.Contains("nearline"))
					throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e) {IsStudyBeingRestored = true}; //assume true in legacy case.

				throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
			}
			catch (Exception e)
			{
				if (client != null)
					client.Abort();

				throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
			}
		}
	}
}