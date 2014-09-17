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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.Import;
using System.ServiceModel;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuImportData", "Launch")]

	// JR (may 2008): this tool is not terribly useful or usable - arguably all importing should be done via command line on server
    //[ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class DataImportTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    DataImportComponent component = new DataImportComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleImportData);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // failed to launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="ImportDiagnosticServicesComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DataImportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ImportDiagnosticServicesComponent class
    /// </summary>
    [AssociateView(typeof(DataImportComponentViewExtensionPoint))]
    public class DataImportComponent : ApplicationComponent
    {
        private string _filename;
        private int _batchSize;

        private string[] _importTypeChoices;
        private string _importType;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataImportComponent()
        {
            // set default size
            _filename = "";
            _batchSize = 10;
        }

        public override void Start()
        {
            Platform.GetService<IImportService>(
                delegate(IImportService service)
                {
                    ListImportersResponse response = service.ListImporters(new ListImportersRequest());
                    response.Importers.Add(null);
                    _importTypeChoices = response.Importers.ToArray();
                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        [ValidateGreaterThan(0)]
        [ValidateLessThan(40, Inclusive=true)]
        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }

        [ValidateNotNull]
        public string ImportType
        {
            get { return _importType; }
            set { _importType = value; }
        }

        public string[] ImportTypeChoices
        {
            get { return _importTypeChoices; }
        }

        public void StartImport()
        {
            // validate input
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            // create a background task to import diagnostic services
            BackgroundTask task = new BackgroundTask(
                delegate(IBackgroundTaskContext context)
                {
                    DoImport(context);
				}, true) { ThreadUICulture = Desktop.Application.CurrentUICulture };

            try
            {
                // run task and block
                ProgressDialog.Show(task, this.Host.DesktopWindow);
            }
            catch (Exception e)
            {
                // we know that the background task has wrapped up an inner exception, so extract it
                ExceptionHandler.Report(e, SR.MessageImportFailed, this.Host.DesktopWindow);
            }
        }

        #endregion

        /// <summary>
        /// Validates that the file exists, and returns the total line count in the file.
        /// </summary>
        /// <returns></returns>
        private int PrescanFile()
        {
            try
            {
                int lineCount = 0;
                using (StreamReader reader = File.OpenText(_filename))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                        lineCount++;
                }
                return lineCount;
            }
            catch (Exception e)
            {
                throw new Exception("Invalid data file: " + e.Message);
            }
        }

        private List<string> ReadLines(StreamReader reader, int numLines)
        {
            List<string> lines = new List<string>();
            string line = null;
            while (lines.Count < numLines && (line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                    lines.Add(line);
            }
            return lines;
        }
        
        private void UpdateProgress(IBackgroundTaskContext context, string status, int batch, int lineCount)
        {
            int importedRows = Math.Min(batch * _batchSize, lineCount);
            float percentage = (lineCount == 0) ? 0 : 100 * ((float)importedRows) / (float)lineCount;
            string message = string.Format("{0} - processed {1} rows.", status, importedRows);
            context.ReportProgress(new BackgroundTaskProgress((int)percentage, message));
        }

        private void DoImport(IBackgroundTaskContext context)
        {
            int lineCount = 0;
            int batch = 0;
            try
            {
                lineCount = PrescanFile();

                using (StreamReader reader = File.OpenText(_filename))
                {
                    // treat as csv
                    List<string> lines = null;
                    while ((lines = ReadLines(reader, _batchSize)).Count > 0)
                    {
                        if (context.CancelRequested)
                            break;

                        try
                        {
                            Platform.GetService<IImportService>(
                                delegate(IImportService service)
                                {
                                    service.ImportCsv(new ImportCsvRequest(_importType, lines));
                                });
                        }
                        catch (FaultException<ImportException> e)
                        {
                            // unwrap the fault exception
                            throw e.Detail;
                        }

                        UpdateProgress(context, "Importing", batch++, lineCount);
                    }
                }

                if (context.CancelRequested)
                {
                    UpdateProgress(context, "Cancelled", batch, lineCount);
                    context.Cancel();
                }
                else
                {
                    UpdateProgress(context, "Completed", batch, lineCount);
                    context.Complete(null);
                }
            }
            catch (Exception e)
            {
                UpdateProgress(context, "Error", batch, lineCount);
                context.Error(e);
            }
        }
    }
}
 
