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
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;

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
            if (!Context.AllowStorage)
                return new List<SupportedSop>();

            // Note: this method is called on startup to set the server's presentation contexts and then on every association.
            // If the settings change between those calls, the server may either throw an exception (if the sop is removed) or 
            // does not behave as expected unless the server is restarted.

            if (_list == null)
            {
                // Load from the database the non-image sops that are current configured for this server partition.
                _list = new List<SupportedSop>();
                
                var partitionSopClassConfig = new PartitionSopClassConfiguration();
                var sopClasses = partitionSopClassConfig.GetAllPartitionSopClasses(Partition);
                if (sopClasses != null)
                {
                    // Now process the SOP Class list
                    foreach (PartitionSopClass partitionSopClass in sopClasses)
                    {
                        if (partitionSopClass.Enabled
                            && partitionSopClass.NonImage)
                        {
                            var sop = new SupportedSop
                                {
                                    SopClass = SopClass.GetSopClass(partitionSopClass.SopClassUid)
                                };

                            if (!partitionSopClass.ImplicitOnly)
                            {
                                sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
                            }
          
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