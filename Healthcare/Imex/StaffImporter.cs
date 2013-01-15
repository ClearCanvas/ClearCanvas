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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "Staff Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class StaffImporter : CsvDataImporterBase
    {
        private const int _numFields = 9;

        private IPersistenceContext _context;

        #region CsvDataImporterBase overrides

        /// <summary>
        /// Import staff from CSV format.
        /// </summary>
        /// <param name="rows">
        /// Each string in the list must contain 25 CSV fields, as follows:
        ///     0 - UserName
        ///     1 - StaffType
        ///     2 - Id
        ///     3 - FamilyName
        ///     4 - GivenName
        ///     5 - MiddleName
        ///     6 - Prefix
        ///     7 - Suffix
        ///     8 - Degree
        /// </param>
        /// <param name="context"></param>
        public override void Import(List<string> rows, IUpdateContext context)
        {
            _context = context;

            List<Staff> importedStaff = new List<Staff>();

            foreach (string row in rows)
            {
                string[] fields = ParseCsv(row, _numFields);

                string userName = fields[0];

            	StaffTypeEnum staffType = context.GetBroker<IEnumBroker>().Find<StaffTypeEnum>(fields[1]);

                string staffId = fields[2];
                string staffFamilyName = fields[3];
                string staffGivenName = fields[4];
                string staffMiddlename = fields[5];
                string staffPrefix = fields[6];
                string staffSuffix = fields[7];
                string staffDegree = fields[8];

                Staff staff = GetStaff(staffId, importedStaff);

                if (staff == null)
                {
                    staff = new Staff();

                    staff.Id = staffId;
                    staff.Type = staffType;
                    staff.Name = new PersonName(staffFamilyName, staffGivenName, staffMiddlename, staffPrefix, staffSuffix, staffDegree);
                    staff.UserName = userName;

                    _context.Lock(staff, DirtyState.New);

                    importedStaff.Add(staff);
                }

            }
        }

        #endregion

        #region Private Methods

        private Staff GetStaff(string staffId, IList<Staff> importedStaff)
        {
            Staff staff = null;

            staff = CollectionUtils.SelectFirst<Staff>(importedStaff,
                delegate(Staff s) { return s.Id == staffId; });

            if (staff == null)
            {
                StaffSearchCriteria criteria = new StaffSearchCriteria();
                criteria.Id.EqualTo(staffId);

                IStaffBroker broker = _context.GetBroker<IStaffBroker>();
                staff = CollectionUtils.FirstElement(broker.Find(criteria));
            }

            return staff;
        }

        #endregion

    }
}
