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
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ImportDiagnosticServicesApplication : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length < 1)
                throw new Exception("Import file path required");

            string fileName = args[0];

            List<string[]> rows = new List<string[]>();
            using (StreamReader reader = File.OpenText(fileName))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] row = line.Split(new string[] { "," }, StringSplitOptions.None);
                    rows.Add(row);
                }
            }

            IDiagnosticServiceAdminService service = ApplicationContext.GetService<IDiagnosticServiceAdminService>();
            service.BatchImport(rows);
        }

        #endregion
    }
}
