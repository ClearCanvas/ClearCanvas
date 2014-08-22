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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using LocalDicomServer = ClearCanvas.ImageViewer.Common.DicomServer.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof (DicomScpExtensionPoint<IDicomServerContext>))]
	public class ImageStorageScpExtension : StoreScpExtension
	{
		public ImageStorageScpExtension()
			: base(GetSupportedSops()) {}

		public override bool ReceiveMessageAsFileStream(Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationId,
		                                                DicomMessage message)
		{
			var sopClassUid = message.AffectedSopClassUid;

			if (sopClassUid.Equals(SopClass.BreastTomosynthesisImageStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedCtImageStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedMrColorImageStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedMrImageStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedPetImageStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedUsVolumeStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedXaImageStorageUid)
			    || sopClassUid.Equals(SopClass.EnhancedXrfImageStorageUid)
			    || sopClassUid.Equals(SopClass.UltrasoundMultiFrameImageStorageUid)
			    || sopClassUid.Equals(SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid)
			    || sopClassUid.Equals(SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid)
			    || sopClassUid.Equals(SopClass.MultiFrameSingleBitSecondaryCaptureImageStorageUid)
			    || sopClassUid.Equals(SopClass.MultiFrameTrueColorSecondaryCaptureImageStorageUid))
			{
				server.DimseDatasetStopTag = DicomTagDictionary.GetDicomTag(DicomTags.ReconstructionIndex); // Random tag at the end of group 20
				server.StreamMessage = true;
				return true;
			}

			return false;
		}

		public override IDicomFilestreamHandler OnStartFilestream(Dicom.Network.DicomServer server, ServerAssociationParameters association,
		                                                          byte presentationId, DicomMessage message)
		{
			if (_importContext == null)
			{
				LoadImportContext(association);
			}

			return new StorageFilestreamHandler(Context, _importContext);
		}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			var extendedConfiguration = LocalDicomServer.GetExtendedConfiguration();

			foreach (SopClass sopClass in GetSopClasses(extendedConfiguration.ImageStorageSopClassUids))
			{
				var supportedSop = new SupportedSop
				                   {
					                   SopClass = sopClass
				                   };

				supportedSop.AddSyntax(TransferSyntax.ExplicitVrLittleEndian);
				supportedSop.AddSyntax(TransferSyntax.ImplicitVrLittleEndian);

				foreach (TransferSyntax transferSyntax in GetTransferSyntaxes(extendedConfiguration.StorageTransferSyntaxUids))
				{
					if (transferSyntax.DicomUid.UID != TransferSyntax.ExplicitVrLittleEndianUid &&
					    transferSyntax.DicomUid.UID != TransferSyntax.ImplicitVrLittleEndianUid)
					{
						supportedSop.AddSyntax(transferSyntax);
					}
				}

				yield return supportedSop;
			}
		}
	}

	[ExtensionOf(typeof (DicomScpExtensionPoint<IDicomServerContext>))]
	public class NonImageStorageScpExtension : StoreScpExtension
	{
		public NonImageStorageScpExtension()
			: base(GetSupportedSops()) {}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			var extendedConfiguration = LocalDicomServer.GetExtendedConfiguration();
			foreach (SopClass sopClass in GetSopClasses(extendedConfiguration.NonImageStorageSopClassUids))
			{
				var supportedSop = new SupportedSop
				                   {
					                   SopClass = sopClass
				                   };
				supportedSop.AddSyntax(TransferSyntax.ExplicitVrLittleEndian);
				supportedSop.AddSyntax(TransferSyntax.ImplicitVrLittleEndian);
				yield return supportedSop;
			}
		}
	}

	public abstract class StoreScpExtension : ScpExtension
	{
		protected DicomReceiveImportContext _importContext;

		protected StoreScpExtension(IEnumerable<SupportedSop> supportedSops)
			: base(supportedSops) {}

		protected static IEnumerable<SopClass> GetSopClasses(IEnumerable<string> sopClassUids)
		{
			foreach (var sopClassUid in sopClassUids)
			{
				if (!String.IsNullOrEmpty(sopClassUid))
				{
					SopClass sopClass = SopClass.GetSopClass(sopClassUid);
					if (sopClass != null)
						yield return sopClass;
				}
			}
		}

		protected static IEnumerable<TransferSyntax> GetTransferSyntaxes(IEnumerable<string> transferSyntaxUids)
		{
			foreach (var transferSyntaxUid in transferSyntaxUids)
			{
				if (!String.IsNullOrEmpty(transferSyntaxUid))
				{
					TransferSyntax syntax = TransferSyntax.GetTransferSyntax(transferSyntaxUid);
					if (syntax != null)
					{
						//at least for now, restrict to available codecs for compressed syntaxes.
						if (!syntax.Encapsulated || DicomCodecRegistry.GetCodec(syntax) != null)
							yield return syntax;
					}
				}
			}
		}

		public override bool OnReceiveRequest(Dicom.Network.DicomServer server,
		                                      ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			string studyInstanceUid;
			string seriesInstanceUid;
			DicomUid sopInstanceUid;

			bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
			if (ok) ok = message.DataSet[DicomTags.SeriesInstanceUid].TryGetString(0, out seriesInstanceUid);
			if (ok) ok = message.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid);

			if (!ok)
			{
				Platform.Log(LogLevel.Error, "Unable to retrieve UIDs from request message, sending failure status.");

				server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID,
				                          DicomStatuses.ProcessingFailure);

				return true;
			}

			if (_importContext == null)
			{
				LoadImportContext(association);
			}

			var importer = new ImportFilesUtility(_importContext);

			var result = importer.Import(message, BadFileBehaviourEnum.Ignore, FileImportBehaviourEnum.Save);
			if (result.Successful)
			{
				if (!String.IsNullOrEmpty(result.AccessionNumber))
					Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (A#:{3} StudyUid:{4})",
					             result.SopInstanceUid, association.CallingAE, association.CalledAE, result.AccessionNumber,
					             result.StudyInstanceUid);
				else
					Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (StudyUid:{3})",
					             result.SopInstanceUid, association.CallingAE, association.CalledAE,
					             result.StudyInstanceUid);
				server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, result.DicomStatus);
			}
			else
			{
				if (result.DicomStatus == DicomStatuses.ProcessingFailure)
					Platform.Log(LogLevel.Error, "Failure importing sop: {0}", result.ErrorMessage);

				//OnReceiveError(message, result.ErrorMessage, association.CallingAE);
				server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid,
				                          result.DicomStatus, result.ErrorMessage);
			}

			return true;
		}

		public override void Cleanup()
		{
			if (_importContext != null)
				_importContext.Cleanup();
		}

		private static string GetRemoteHostName(AssociationParameters association)
		{
			string remoteHostName = null;
			try
			{
				if (association.RemoteEndPoint != null)
				{
					try
					{
						IPHostEntry entry = Dns.GetHostEntry(association.RemoteEndPoint.Address);
						remoteHostName = entry.HostName;
					}
					catch
					{
						remoteHostName = association.RemoteEndPoint.Address.ToString();
					}
				}
			}
			catch (Exception e)
			{
				remoteHostName = null;
				Platform.Log(LogLevel.Warn, e, "Unable to resolve remote host name.");
			}

			return remoteHostName;
		}

		protected void LoadImportContext(ServerAssociationParameters association)
		{
			_importContext = new DicomReceiveImportContext(association.CallingAE, GetRemoteHostName(association), StudyStore.GetConfiguration(), EventSource.CurrentProcess);

			// Publish new WorkItems as they're added to the context
			lock (_importContext.StudyWorkItemsSyncLock)
			{
				_importContext.StudyWorkItems.ItemAdded += (sender, args) =>
				                                           {
					                                           _importContext.PublishWorkItemActivity(WorkItemDataHelper.FromWorkItem(args.Item));

					                                           var auditedInstances = new AuditedInstances();
					                                           var request = args.Item.Request as DicomReceiveRequest;
					                                           if (request != null)
						                                           auditedInstances.AddInstance(request.Patient.PatientId, request.Patient.PatientsName, request.Study.StudyInstanceUid);

					                                           AuditHelper.LogReceivedInstances(
						                                           association.CallingAE, GetRemoteHostName(association),
						                                           auditedInstances, EventSource.CurrentProcess,
						                                           EventResult.Success, EventReceiptAction.ActionUnknown);
				                                           };

				_importContext.StudyWorkItems.ItemChanged += (sender, args) =>
					_importContext.PublishWorkItemActivity(WorkItemDataHelper.FromWorkItem(args.Item));
			}
		}
	}
}