using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
namespace ClearCanvas.ImageViewer.Tools.Measurement
{
    //[MenuAction("apply", "global-menus/MenuTools/MenuMeasurement/MenuMeasurementsList", "Apply")]
    //[Tooltip("apply", "TooltipMyImageViewerTool")]    
    //[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //// ... (other action attributes here)
    //[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class MeasurementListTool : ImageViewerTool
    {
        public MeasurementListTool() : base()
        {

        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // TODO: add any significant initialization code here rather than in the constructor
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            if (SelectedPresentationImage is null)
                return;

            var overlay = SelectedPresentationImage as IOverlayGraphicsProvider;
            if (overlay is null)
                return;

            foreach(var g in overlay.OverlayGraphics)
            {
                var a = 1;
            }
        }

        
    }
}
