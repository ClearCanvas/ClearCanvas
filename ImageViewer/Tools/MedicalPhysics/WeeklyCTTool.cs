using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;

using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.Tools.Measurement;
using ClearCanvas.ImageViewer.Tools.MedicalPhysics.Properties;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Drawing;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    //[MenuAction("apply", "global-menus/MenuTools/Medical Physics/Weekly CT Phantom", "Apply")]
    //[Tooltip("apply", "Open the weekly CT phantom tool")]
    //[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class WeeklyCTImageViewerTool : ImageViewerTool
    {
        private WeeklyCTComponent _component;
        public WeeklyCTImageViewerTool() { }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Apply()
        {
           
            if (ValidateStudy(null))
            {
               
                
                if (_component == null)
                {
                    _component = new WeeklyCTComponent(ImageViewer);
                    ApplicationComponent.LaunchAsShelf(
                    this.DesktopWindow,
                    _component,
                    "Weekly CT QC",
                    ShelfDisplayHint.DockLeft);
                    _component.Stopped += Component_Stopped;
                }
                
                

            }
        }

        private void Component_Stopped(object sender, EventArgs e)
        {
            _component.Stopped -= Component_Stopped;
            _component = null;
        }

        protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
        {
            base.OnPresentationImageSelected(sender, e);
            if (e.SelectedPresentationImage is DicomGrayscalePresentationImage)
            {
                var d = e.SelectedPresentationImage as DicomGrayscalePresentationImage;
                if (d.ImageSop.Modality == "CT")
                    Enabled = true;
                else
                    Enabled = false;
            }
            else
                Enabled = false;
        }

        private bool ValidatePresentationImage(IPresentationImage image)
        {
            if (image is DicomGrayscalePresentationImage)
            {
                var d = image as DicomGrayscalePresentationImage;
                if (d.ImageSop.Modality == "CT")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private IGraphic CreateGraphic()
        {
            return new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new RectanglePrimitive())));
        }

        private RoiGraphic CreateRoiGraphic(bool initiallySelected)
        {
            //When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
            //see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
            //it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
            IGraphic graphic = CreateGraphic();
            IAnnotationCalloutLocationStrategy strategy = new DefaultRoiCalloutLocationStrategy();

            RoiGraphic roiGraphic;
            if (strategy == null)
                roiGraphic = new RoiGraphic(graphic);
            else
                roiGraphic = new RoiGraphic(graphic, strategy);

            roiGraphic.Name = "My rectangle";

           

            roiGraphic.State = initiallySelected ? roiGraphic.CreateSelectedState() : roiGraphic.CreateInactiveState();

            return roiGraphic;
        }

        private InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
        {
            return new InteractiveBoundableGraphicBuilder((IBoundableGraphic)graphic);
        }

        public static bool ValidateStudy(StudyTableItem study)
        {
            if (study is null)
                return false;
            if(study.ModalitiesInStudy.Contains("CT"))
                return true;

            return false;
        }
    }

    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [VisibleStateObserver("apply","Visible","VisibleChanged")]
    [MenuAction("apply", "dicomstudybrowser-contextmenu/Medical Physics/Weekly CT Phantom", "Apply")]
    [MenuAction("apply", "global-menus/MenuTools/Medical Physics/Weekly CT Phantom", "Apply")]
    [ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    public class WeeklyCTStudyBrowserTool : StudyBrowserTool
    {
        private WeeklyCTComponent _component;

        public WeeklyCTStudyBrowserTool()
        {

        }
        public override void Initialize()
        {
            base.Initialize();
        }

        public void Apply()
        {
            //Open the study;
            var helper = new OpenStudyHelper
            {
                WindowBehaviour = ViewerLaunchSettings.WindowBehaviour,
                AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer
            };

            foreach (var study in Context.SelectedStudies)
                helper.AddStudy(study.StudyInstanceUid, study.Server);
            helper.Title = "Weekly CT QC";
            var _viewer = helper.OpenStudies();
            if (_component == null)
            {
                _component = new WeeklyCTComponent(_viewer);
                ApplicationComponent.LaunchAsShelf(
                this.Context.DesktopWindow,
                _component,
                "Weekly CT QC",
                ShelfDisplayHint.DockLeft);
                _component.Stopped += Component_Stopped;
            }

        }

        private void Component_Stopped(object sender, EventArgs e)
        {
            _component.Stopped -= Component_Stopped;
            _component = null;
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            UpdateAvailability();
        }

        protected override void OnSelectedStudyChanged(object sender, EventArgs e)
        {
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            Enabled = WeeklyCTImageViewerTool.ValidateStudy(this.Context.SelectedStudy);
            Visible = WeeklyCTImageViewerTool.ValidateStudy(this.Context.SelectedStudy);
        }
    }
}
