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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class CompressedStorageScpExtension : StorageScp
    {
        #region Private Members
        private IList<SupportedSop> _sopList;
        private IList<PartitionTransferSyntax> _syntaxList;
        private readonly string _type = "Compressed C-STORE-RQ";

        public override string StorageScpType
        {
            get { return _type; }
        }

        #endregion
        
        #region Private Methods

        #endregion

        #region IDicomScp Members

        /// <summary>
        /// Returns a list of the services supported by this plugin.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (!Context.AllowStorage)
                return new List<SupportedSop>();

            if (_sopList == null)
            {
                _sopList = new List<SupportedSop>();

                // Get the SOP Classes
                using (IReadContext read = _store.OpenReadContext())
                {
                    // Get the transfer syntaxes
                    _syntaxList = LoadTransferSyntaxes(read, Partition.GetKey(),true);
                }

                var partitionSopClassConfig = new PartitionSopClassConfiguration();
                var sopClasses = partitionSopClassConfig.GetAllPartitionSopClasses(Partition);
                if (sopClasses != null)
                {
                    // Now process the SOP Class List
                    foreach (PartitionSopClass partitionSopClass in sopClasses)
                    {
                        if (partitionSopClass.Enabled
                            && !partitionSopClass.NonImage)
                        {
                            var sop = new SupportedSop
                                {
                                    SopClass = SopClass.GetSopClass(partitionSopClass.SopClassUid)
                                };

                            foreach (PartitionTransferSyntax syntax in _syntaxList)
                            {
                                sop.SyntaxList.Add(TransferSyntax.GetTransferSyntax(syntax.Uid));
                            }
                            _sopList.Add(sop);
                        }
                    }
                }

            }
            return _sopList;
        }

        #endregion
    }
}
