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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;

namespace ClearCanvas.ImageServer.Services.Streaming.DicomImport
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode=InstanceContextMode.PerSession)]
    public class DicomImportService : IDicomImportService
    {
        #region Private Members
        private SopInstanceImporterContext _importerContext;
        #endregion

        #region IDicomImportService Members

        public ImportSopResponse ImportSop(ImportSopRequest request)
        {
            try
            {
                var theFile = new DicomFile(string.Format("{0}{1}", request.SopInstanceUid, ServerPlatform.DicomFileExtension));

                using (var stream = new LargeMemoryStream(request.SopInstance))
                {
                    theFile.Load(stream);
                }

                var partition = ServerPartitionMonitor.Instance.GetPartition(request.CalledAETitle);

                string aeTitle = theFile.SourceApplicationEntityTitle;

                if (_importerContext == null)
                    _importerContext = new SopInstanceImporterContext(
                        String.Format("{0}_{1}", aeTitle, DateTime.Now.ToString("yyyyMMddhhmmss")),
                        partition.AeTitle, partition);

                var utility = new SopInstanceImporter(_importerContext);


                var importResult = utility.Import(theFile);
                if (!importResult.Successful)
                    Platform.Log(LogLevel.Error, "Failure importing file file from Web Service: {0}, SOP Instance UID: {1}", importResult.ErrorMessage, request.SopInstanceUid);
                else
                    Platform.Log(LogLevel.Info, "Processed import for SOP through Web Service: {0}.", request.SopInstanceUid);

                var result = new ImportSopResponse
                    {
                        DicomStatusCode = importResult.DicomStatus.Code,
                        FailureMessage = importResult.ErrorMessage,
                        Successful = importResult.Successful
                    };

                return result;
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to import files: {0}, SOP Instance UID: {1}", ex.Message, request.SopInstanceUid);
                Platform.Log(LogLevel.Error, message);
                throw new FaultException(message);
            }
        }

    	#endregion
    }
}
