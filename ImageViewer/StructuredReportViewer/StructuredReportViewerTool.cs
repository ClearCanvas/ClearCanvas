using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
//using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyLoaders.Local;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Tools.StructuredReportViewer
{
    [ButtonAction("activate", ToolbarActionSite + "/ToolbarOpenStructuredReport", "OpenStructuredReport")]
    [MenuAction("activate", ContextMenuActionSite + "/MenuOpenStructuredReport", "OpenStructuredReport")]    
    [Tooltip("activate", "ToolTipOpenStructuredReport")]
    [IconSet("activate", "Icons.StructuredReportViewerToolSmall.png", "Icons.StructuredReportViewerToolSmall.png", "Icons.StructuredReportViewerToolSmall.png")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]   
    [ExtensionOf(typeof(SeriesDetailsToolExtensionPoint))]
    public class StructuredReportViewerTool : SeriesDetailsTool
    {
        private IShelf _shelf;
        private IWorkspace _workspace;

        public void OpenStructuredReport()
        {
            BlockingOperation.Run(OpenStructuredReportInternal);
        }

        private void OpenStructuredReportInternal()
        {
            if (!Enabled)
                return;

            var report = Context.SelectedSeries.Where(x => x.Modality == "SR").First();
            var s = report.FindServer(true);
            if (s.IsLocal)
            {
                IEnumerator<ISopInstance> _sops;
                using (var context = new DataAccessContext())
                {
                    IStudy study = context.GetStudyBroker().GetStudy(new StudyLoaderArgs(Study.StudyInstanceUid, s, null).StudyInstanceUid);
                    _sops = study.GetSopInstances().GetEnumerator();
                    _sops.MoveNext();
                    DicomFile df = new DicomFile(_sops.Current.FilePath);
                    df.Load();
                    StructuredReportViewerComponent component = new StructuredReportViewerComponent(Context.DesktopWindow, df.DataSet);
                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                   this.Context.DesktopWindow,
                   component,
                  "Structured Report");

                    _workspace.Closed += Workspace_Closed;
                    //_sops.Current[]
                }
            }
            else
            {
                Platform.Log(LogLevel.Info, "s = " + s.ToString());
                StudyXml studyXml = RetrieveStudyXml(new StudyLoaderArgs(Study.StudyInstanceUid, s, null));
                if (studyXml.Count() < 1)
                {

                    return;
                }
                var series = studyXml.Where(x => x.SeriesInstanceUid == report.SeriesInstanceUid).FirstOrDefault();
                var instance = series.First();


                StructuredReportViewerComponent component = new StructuredReportViewerComponent(Context.DesktopWindow, instance.Collection);
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                   "Structured Report");

                _workspace.Closed += Workspace_Closed;
            }
           

             
        }
        protected override void OnSelectedSeriesChanged()
        {
            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            Enabled = Context.SelectedSeries.Count > 0
                && Context.SelectedSeries.Where(x=>x.Modality == "SR").Any();
            
                        
        }
        private void Workspace_Closed(object sender, ClosedEventArgs e)
        {
            _workspace.Closed -= Workspace_Closed;
            _workspace = null;
        }
        private void Shelf_Closed(object sender, ClosedEventArgs e)
        {
            _shelf.Closed -= Shelf_Closed;
            _shelf = null;

        }
        private StudyXml RetrieveStudyXml(StudyLoaderArgs studyLoaderArgs)
        {
            var headerParams = new HeaderStreamingParameters
            {
                StudyInstanceUID = studyLoaderArgs.StudyInstanceUid,
                ServerAETitle = Server.AETitle,
                ReferenceID = Guid.NewGuid().ToString(),
                IgnoreInUse = studyLoaderArgs.Options != null && studyLoaderArgs.Options.IgnoreInUse
            };
            
            HeaderStreamingServiceClient client = null;
            try
            {

                string uri = String.Format("http://{0}:{1}/HeaderStreaming/HeaderStreaming",
                                           Server.ScpParameters.HostName, Server.StreamingParameters.HeaderServicePort);

                client = new HeaderStreamingServiceClient(new Uri(uri));
                client.Open();
                var studyXml = client.GetStudyXml(ServerDirectory.GetLocalServer().AETitle, headerParams);
                client.Close();
                return studyXml;
            }
            catch (FaultException<StudyIsInUseFault> e)
            {
                throw new InUseLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (FaultException<StudyIsNearlineFault> e)
            {
                throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e) { IsStudyBeingRestored = e.Detail.IsStudyBeingRestored };
            }
            catch (FaultException<Dicom.ServiceModel.Streaming.StudyNotFoundFault> e)
            {
                throw new NotFoundLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (FaultException e)
            {
                //TODO: Some versions (pre-Team) of the ImageServer
                //throw a generic fault when a study is nearline, instead of the more specialized one.
                string message = e.Message.ToLower();
                if (message.Contains("nearline"))
                    throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e) { IsStudyBeingRestored = true }; //assume true in legacy case.

                throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (Exception e)
            {
                if (client != null)
                    client.Abort();

                throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
        }

       
    }
}
