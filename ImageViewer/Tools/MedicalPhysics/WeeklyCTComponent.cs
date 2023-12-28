using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Tools.Measurement;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{

    public sealed class WeeklyCTComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {

    }


    [AssociateView(typeof(WeeklyCTComponentViewExtensionPoint))]
    public class WeeklyCTComponent : ApplicationComponent
    {
        private double _waterHU;
        private double _waterStdDev;
        private double _highContrastResolution;
        private MoveControlGraphic _hcrGraphic;
        private MoveControlGraphic _contrastWaterGraphic;
        private MoveControlGraphic _contrastPerspexGraphic;
        private MoveControlGraphic _waterValueGraphic;
        private IDisplaySet _recon1;
        private IDisplaySet _recon2;
        private IDisplaySet _recon3;

        private IImageViewer _viewer;
        private double _contrastScale;
        private double _waterValue;

        public WeeklyCTComponent(IImageViewer viewer):base() 
        {
            _viewer = viewer;
            _viewer.Closing += Viewer_Closing;
           
        }

        private void Viewer_Closing(object sender, EventArgs e)
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        public double HighContrastResolution
        {
            get { return _highContrastResolution; }
        }

        public double ContrastScale
        {
            get { return _contrastScale; }
        }

        public double WaterValue
        {
            get { return _waterValue; }
        }


        public override void Start()
        {
            base.Start();
            SortDisplaySets();
            Layout();
            CreateROIs();
            
        }

        public override void Stop()
        {
            base.Stop();
            _viewer.Closing -= Viewer_Closing;
            _viewer = null;
            _contrastPerspexGraphic.Drawing -= Graphic_Drawing;
        }

        private void RoiGraphicChanged(object sender, EventArgs e)
        {
            Update();
        }

        private void SortDisplaySets()
        {
            if (_viewer is null)
                throw new Exception("The ImageViewer was null, should not be possible");

            _recon1 = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0];
            _recon2 = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1];
            
        }

        private void Layout()
        {
            LayoutComponent.SetImageBoxLayout(_viewer, 1, 3);
            _viewer.PhysicalWorkspace.ImageBoxes[0].DisplaySet = _recon1;
            var _recon1A = _recon1.CreateFreshCopy();
           
            _viewer.PhysicalWorkspace.ImageBoxes[1].DisplaySet = _recon1A;
            _viewer.PhysicalWorkspace.ImageBoxes[1].TopLeftPresentationImage = _recon1A.PresentationImages[6];
            _viewer.PhysicalWorkspace.ImageBoxes[2].DisplaySet = _recon2;
        }

        private void CreateROIs()
        {
            var overlayProvider = (_viewer.PhysicalWorkspace.ImageBoxes[0].Tiles[0].PresentationImage as IOverlayGraphicsProvider);
            _contrastPerspexGraphic = MakeROIGraphic(new RectanglePrimitive
            {
                TopLeft = new System.Drawing.PointF(180, 200),
                BottomRight = new PointF(200, 220)
            });
            _contrastPerspexGraphic.Drawing += Graphic_Drawing;           
            overlayProvider.OverlayGraphics.Add(_contrastPerspexGraphic);

            _contrastWaterGraphic = MakeROIGraphic(new RectanglePrimitive
            {
                TopLeft = new System.Drawing.PointF(250, 100),
                BottomRight = new PointF(270, 120)
            });
            _contrastWaterGraphic.Drawing += Graphic_Drawing;            
            overlayProvider.OverlayGraphics.Add(_contrastWaterGraphic);

            _hcrGraphic = MakeROIGraphic(new RectanglePrimitive
            {
                TopLeft = new System.Drawing.PointF(290, 180),
                BottomRight = new PointF(308, 198)
            });
            _hcrGraphic.Drawing += Graphic_Drawing;
            overlayProvider.OverlayGraphics.Add(_hcrGraphic);

            overlayProvider = (_viewer.PhysicalWorkspace.ImageBoxes[1].Tiles[0].PresentationImage as IOverlayGraphicsProvider);
            _waterValueGraphic = MakeROIGraphic(new EllipsePrimitive
            {
                TopLeft = new PointF(150, 150),
                BottomRight = new PointF(350, 350)
                 
            }
                );
            _waterValueGraphic.Drawing += Graphic_Drawing;
            overlayProvider.OverlayGraphics.Add(_waterValueGraphic);
        }

        private void Graphic_Drawing(object sender, EventArgs e)
        {
            Update();
        }
      
        private MoveControlGraphic MakeROIGraphic(IBoundableGraphic boundable)
        {
          
            boundable.LineStyle = LineStyle.Dot;
            MoveControlGraphic moveControlGraphic = new MoveControlGraphic(boundable );
            moveControlGraphic.Color = Color.White;
            
            //moveControlGraphic.Name = "Perspex";
            //var g = new CalloutGraphic("Perspex");
            //g.LineStyle = LineStyle.Solid;
            //g.AnchorPoint = moveControlGraphic.BoundingBox.Location;
            //g.ShowArrowhead = true;
            //moveControlGraphic.Graphics.Add(g);
            return moveControlGraphic;
        }

        private void Update()
        {
            _highContrastResolution = (_hcrGraphic.GetRoi() as RectangularRoi).StandardDeviation;
            _contrastScale = (_contrastPerspexGraphic.GetRoi() as RectangularRoi).Mean - (_contrastWaterGraphic.GetRoi() as RectangularRoi).Mean;
            _waterValue = (_waterValueGraphic.GetRoi() as EllipticalRoi).Mean;
            NotifyPropertyChanged("ContrastScale");
            NotifyPropertyChanged("HighContrastResolution");
            NotifyPropertyChanged("WaterValue");
        }
    }
}
