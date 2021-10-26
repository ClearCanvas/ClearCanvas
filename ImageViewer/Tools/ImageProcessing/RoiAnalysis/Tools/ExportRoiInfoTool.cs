using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis.Tools
{
    [IconSet("activate", "Icons.RoiHistogramToolSmall.png", "Icons.RoiHistogramToolSmall.png", "Icons.RoiHistogramToolSmall.png")]
    [Tooltip("activate", "Export ROI Info")]
    [ButtonAction("activate", "roihistogram-toolbar/ToolbarReplicate", "Export")]
    [ExtensionOf(typeof(RoiHistogramToolExtensionPoint))]
    public class ExportRoiInfoTool : Tool<IRoiHistogramToolContext>
    {
        public ExportRoiInfoTool() : base()
        {

        }

        public void Activate()
        {

        }

        public void Export()
        {
            Application.ShowMessageBox("Export", MessageBoxActions.Ok);
        }
    }
}
