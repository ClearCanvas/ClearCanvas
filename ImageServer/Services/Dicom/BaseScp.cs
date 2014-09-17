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
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Base class for all DicomScpExtensions for the ImageServer.
    /// </summary>
    public abstract class BaseScp : IDicomScp<DicomScpContext>
    {

        #region Protected Members
        protected IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
    	private DicomScpContext _context;

        #endregion

        #region Private Methods

        #endregion

        #region Properties
        /// <summary>
		/// The <see cref="ServerPartition"/> associated with the Scp.
		/// </summary>
        protected ServerPartition Partition
        {
            get { return _context.Partition; }
        }

        /// <summary>
        /// The <see cref="DicomScpContext"/> associated with the Scp.
        /// </summary>
        protected DicomScpContext Context
        {
            get { return _context; }
        }

        protected Device Device { get; set; }

        #endregion

        #region Protected methods
        /// <summary>
        /// Called during association verification.
        /// </summary>
        /// <param name="association"></param>
        /// <param name="pcid"></param>
        /// <returns></returns>
        protected abstract DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid);

        #endregion

        #region Public Methods

		/// <summary>
		/// Verify a presentation context.
		/// </summary>
		/// <param name="association"></param>
		/// <param name="pcid"></param>
		/// <returns></returns>
        public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
        {
            bool isNew;
			Device = Context.Device ?? DeviceManager.LookupDevice(Partition, association, out isNew);

            // Let the subclass perform the verification
            DicomPresContextResult result = OnVerifyAssociation(association, pcid);
            if (result!=DicomPresContextResult.Accept)
            {
                Platform.Log(LogLevel.Debug, "Rejecting Presentation Context {0}:{1} in association between {2} and {3}.",
                             pcid, association.GetAbstractSyntax(pcid).Description,
                             association.CallingAE, association.CalledAE);
            }

            return result;
        }

        
        /// <summary>
        /// Helper method to load a <see cref="StudyXml"/> instance for a given study location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            var theXml = new StudyXml();

            if (File.Exists(streamFile))
            {
                // allocate the random number generator here, in case we need it below
                var rand = new Random();

                // Go into a retry loop, to handle if the study is being processed right now 
                for (int i = 0; ;i++ )
                    try
                    {
                        using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
                        {
                            var theMemento = new StudyXmlMemento();

                            StudyXmlIo.Read(theMemento, fileStream);

                            theXml.SetMemento(theMemento);

                            fileStream.Close();

                            return theXml;
                        }
                    }
                    catch (IOException)
                    {
                        if (i < 5)
                        {
                            Thread.Sleep(rand.Next(5, 50)); // Sleep 5-50 milliseconds
                            continue;
                        }

                        throw;
                    }
            }

            return theXml;
        }

		/// <summary>
		/// Get the Status of a study.
		/// </summary>
		/// <param name="studyInstanceUid">The Study to check for.</param>
		/// <param name="studyStorage">The returned study storage object</param>
		/// <returns>true on success, false on no records found.</returns>
		public bool GetStudyStatus(string studyInstanceUid, out StudyStorage studyStorage)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				var selectBroker = read.GetBroker<IStudyStorageEntityBroker>();
				var criteria = new StudyStorageSelectCriteria();

				criteria.ServerPartitionKey.EqualTo(Partition.GetKey());
				criteria.StudyInstanceUid.EqualTo(studyInstanceUid);

				IList<StudyStorage> storageList = selectBroker.Find(criteria);

				foreach (StudyStorage studyLocation in storageList)
				{
					studyStorage = studyLocation;
					return true;
				}
				studyStorage = null;
				return false;
			}
		}

        #endregion

        #region IDicomScp Members

        public void SetContext(DicomScpContext parms)
        {
        	_context = parms;
        }

        public virtual void Cleanup()
        {
        }

        public virtual bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomMessage message)
        {
            throw new Exception("The method or operation is not implemented.  The method must be overriden.");
        }

		public virtual IDicomFilestreamHandler OnStartFilestream(DicomServer server, ServerAssociationParameters association,
																 byte presentationId, DicomMessage message)
		{
			return null;
		}

	    public virtual bool ReceiveMessageAsFileStream(DicomServer server, ServerAssociationParameters association, byte presentationId,
	                                           DicomMessage message)
	    {
		    return false;
	    }

	    public virtual IList<SupportedSop> GetSupportedSopClasses()
        {
            throw new Exception("The method or operation is not implemented.  The method must be overriden.");
        }

        #endregion
    }
}