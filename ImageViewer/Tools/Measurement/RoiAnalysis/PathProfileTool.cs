using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.RoiGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis
{

    [MenuAction("apply", "basicgraphic-menu/PathProfileTool", "Apply")]
    [VisibleStateObserver("apply","Visible","VisibleChanged")]
    [ExtensionOf(typeof(GraphicToolExtensionPoint))]
    public class PathProfileTool : GraphicTool
    {
        public PathProfileTool() { }

        public override void Initialize()
        {
            base.Initialize();
            if(Graphic is RoiGraphic)
            {
                if (Graphic.GetRoi() is LinearRoi)                
                    Visible = true;                
                else
                    Visible = false;
            }
            else
                Visible = false;
        }

        
        public void Apply()
        {
        }

       
    }
}
