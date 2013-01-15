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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.Import;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin.Import
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IImportService))]
    public class ImportService : ApplicationServiceBase, IImportService
    {
        #region IImportService Members

        public ListImportersResponse ListImporters(ListImportersRequest request)
        {
            List<string> importers = CollectionUtils.Map<ExtensionInfo, string, List<string>>(
                (new CsvDataImporterExtensionPoint()).ListExtensions(),
                delegate(ExtensionInfo info)
                {
                    return info.Name ?? info.FormalName;
                });

            return new ListImportersResponse(importers);
        }

        [UpdateOperation]
        public ImportCsvResponse ImportCsv(ImportCsvRequest request)
        {
            ICsvDataImporter importer = null;
            try
            {
                importer = (ICsvDataImporter)
                    (new CsvDataImporterExtensionPoint()).CreateExtension(
                                            delegate(ExtensionInfo info)
                                            {
                                                return info.Name == request.Importer || info.FormalName == request.Importer;
                                            });
            }
            catch (NotSupportedException)
            {
                throw new RequestValidationException(string.Format("{0} is not supported.", request.Importer));
            }

            importer.Import(request.Rows, (IUpdateContext)PersistenceContext);

            return new ImportCsvResponse();
        }

        #endregion
    }
}
