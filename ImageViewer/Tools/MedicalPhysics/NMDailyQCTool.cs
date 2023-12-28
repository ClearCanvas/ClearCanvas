using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [VisibleStateObserver("apply", "Visible", "VisibleChanged")]
    [MenuAction("apply", "dicomstudybrowser-contextmenu/Medical Physics/NM Daily QC", "Apply")]    
    [ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    public class NMDailyQCTool : StudyBrowserTool
    {
        public NMDailyQCTool()
        {

        }


        public void Apply()
        {
            var helper = new OpenStudyHelper
            {
                WindowBehaviour = ViewerLaunchSettings.WindowBehaviour,
                AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer
            };

            foreach (var study in Context.SelectedStudies)
                helper.AddStudy(study.StudyInstanceUid, study.Server);
            helper.Title = "NM Daily QC";
            var _viewer = helper.OpenStudies();

            ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, new NMDailyQCComponent(_viewer), "NM Daily QC",
                ShelfDisplayHint.DockLeft);
        }

        protected override void OnSelectedStudyChanged(object sender, EventArgs e)
        {
            base.OnSelectedStudyChanged(sender, e);
            UpdateEnabled(); 
        }
        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            if(this.Context.SelectedStudy is null)
            {
                Enabled = false;
                Visible = false;
            }
            else if(this.Context.SelectedStudy.StudyDescription == "NM Daily QC")
            {
                Enabled = true;
                Visible = true;
            }
            else
            {
                Enabled = false;
                Visible = false;
            }
        }
    }
}
