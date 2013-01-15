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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Class that handles DICOM C-Store Requests for Non-Image objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This routine is a plugin implemeting the <see cref="IDicomScp{TContext}"/> interface for handling 
    /// Non-Image DICOM C-STORE-RQ messages.
    /// </para>
    /// <para>
    /// The method queries the PartitionSopClass table in the database to determine the services
    /// it should support.  It also only implements the default DICOM transfer syntaxes.
    /// </para>
    /// </remarks>
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class NonImageStorageScpExtension : StorageScp
    {
        #region Private Members
        private IList<SupportedSop> _list;
        private readonly string _type = "NonImage C-STORE-RQ";
        #endregion

        #region Properties

        public override string StorageScpType
        {
            get { return _type; }
        }

        #endregion

        #region IDicomScp Members

       
        /// <summary>
        /// Returns a list of the DICOM services supported by this plugin.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (_list == null)
            {
                // Load from the database the non-image sops that are current configured for this server partition.
                _list = new List<SupportedSop>();

                // Input parameters
                PartitionSopClassQueryParameters inputParms = new PartitionSopClassQueryParameters();
                inputParms.ServerPartitionKey = Partition.GetKey();

                // Do the query
                using (IReadContext read = _store.OpenReadContext())
                {
                    IQueryServerPartitionSopClasses broker = read.GetBroker<IQueryServerPartitionSopClasses>();
                    IList<PartitionSopClass> sopClasses = broker.Find(inputParms);
                    read.Dispose();

                    // Now process the SOP Class list
                    foreach (PartitionSopClass partitionSopClass in sopClasses)
                    {
                        if (partitionSopClass.Enabled
                            && partitionSopClass.NonImage)
                        {
                            SupportedSop sop = new SupportedSop();

                            sop.SopClass = SopClass.GetSopClass(partitionSopClass.SopClassUid);
                            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
                            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);

                            _list.Add(sop);
                        }
                    }
                }
            }

            return _list;
        }

        #endregion
    }
}