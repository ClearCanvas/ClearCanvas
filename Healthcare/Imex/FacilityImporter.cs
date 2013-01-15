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
using System.Text;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "Facility Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class FacilityImporter : CsvDataImporterBase
    {
        private IEnumBroker _enumBroker;
        private List<InformationAuthorityEnum> _authorities;

        public FacilityImporter()
        {

        }

        /// <summary>
        /// Import external practitioner from CSV format.
        /// </summary>
        /// <param name="rows">
        /// Each string in the list must contain 4 CSV fields, as follows:
        ///     0 - Facility ID
        ///     1 - Facility Name
        ///     2 - Information Authority ID
        ///     3 - Information Authoirty Name
        /// </param>
        /// <param name="context"></param>
        public override void Import(List<string> rows, IUpdateContext context)
        {
            _enumBroker = context.GetBroker<IEnumBroker>();
            _authorities = new List<InformationAuthorityEnum>(_enumBroker.Load<InformationAuthorityEnum>(true));

            List<Facility> facilities = new List<Facility>();

            foreach (string line in rows)
            {
                // expect 4 fields in the row
                string[] fields = ParseCsv(line, 4);

                string facilityId = fields[0];
                string facilityName = fields[1];
				string facilityDescription = fields[2];
				string informationAuthorityId = fields[3];
                string informationAuthorityName = fields[4];

                // first check if we have it in memory
                Facility facility = CollectionUtils.SelectFirst(facilities,
                    delegate(Facility f) { return f.Code == facilityId && f.Name == facilityName; });

                // if not, check the database
                if (facility == null)
                {
                    FacilitySearchCriteria where = new FacilitySearchCriteria();
                    where.Code.EqualTo(facilityId);
                    where.Name.EqualTo(facilityName);

                    IFacilityBroker broker = context.GetBroker<IFacilityBroker>();
                    facility = CollectionUtils.FirstElement(broker.Find(where));

                    // if not, create a new instance
                    if (facility == null)
                    {
						facility = new Facility(facilityId, facilityName, facilityDescription, GetAuthority(informationAuthorityId, informationAuthorityName));
                        context.Lock(facility, DirtyState.New);
                    }

                    facilities.Add(facility);
                }
            }
        }

        private InformationAuthorityEnum GetAuthority(string id, string name)
        {
            InformationAuthorityEnum authority = CollectionUtils.SelectFirst(_authorities,
                 delegate(InformationAuthorityEnum a) { return a.Code == id; });

            if(authority == null)
            {
                // create a new value
                InformationAuthorityEnum lastValue = CollectionUtils.LastElement(_authorities);
                authority = (InformationAuthorityEnum) _enumBroker.AddValue(typeof(InformationAuthorityEnum), id, id, name, lastValue == null ? 1 : lastValue.DisplayOrder + 1, false);
                _authorities.Add(authority);
            }
            return authority;
        }
    }
}
