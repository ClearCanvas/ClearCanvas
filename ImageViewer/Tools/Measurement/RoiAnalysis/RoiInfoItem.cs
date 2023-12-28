using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis
{
    [ExtensionPoint()]
    public sealed class RoiInfoItemExtensionPoint : ExtensionPoint<IRoiInfoItem>
    {

    }

    public abstract class RoiInfoItem : IRoiInfoItem
    {
        protected Roi _roi;
        protected RoiAnalysisComponentContainer _component;
        protected string _name;
        protected object _value;
        protected string _valueAsString;

        public RoiInfoItem()
        {

        }

        public RoiInfoItem(Roi roi)
        {
            _roi = roi;
        }

        public void SetRoi(Roi roi)
        {
            _roi = roi;
        }

        public void SetComponent(RoiAnalysisComponentContainer component)
        {
            _component = component;
            
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public string ValueAsString
        {
            get
            {
                return _valueAsString;
            }
        }

        public virtual bool ComputeValue()
        {
            return false;
        }

        public virtual void SetValue(object value)
        {
            //Do nothing at the moment
        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class MaxPixelValueInfoItem : RoiInfoItem
    {

        public MaxPixelValueInfoItem() : base()
        {
            _name = "Maximum Pixel Value";
        }
        public MaxPixelValueInfoItem(Roi roi) : base(roi)
        {
            _name = "Maximum Pixel Value";
        }

        public override bool ComputeValue()
        {
            var statisticsProvider = _roi as IRoiStatisticsProvider;
            if (statisticsProvider is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                //_value = _roi.GetPixelValues().Min();
                _value = statisticsProvider.Max;
                _valueAsString = ((double)_value).ToString("N0") + " " + _roi.ModalityLutUnits.Label;

                return true;
            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class MinPixelValueInfoItem : RoiInfoItem
    {
        public MinPixelValueInfoItem() : base()
        {
            _name = "Minimum Pixel Value";
        }
        public MinPixelValueInfoItem(Roi roi) : base(roi)
        {
            _name = "Minimum Pixel Value";
            
           
        }

        public override bool ComputeValue()
        {
           
            var statisticsProvider = _roi as IRoiStatisticsProvider;
            if (statisticsProvider is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                //_value = _roi.GetPixelValues().Min();
                _value = statisticsProvider.Min;
                _valueAsString = ((double)_value).ToString("N0") + " " + _roi.ModalityLutUnits.Label;

                return true;
            }



        }
    }
    
    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class AveragePixelValueInfoItem : RoiInfoItem
    {
        public AveragePixelValueInfoItem() : base()
        {
            _name = "Average Pixel Value";
        }
        public AveragePixelValueInfoItem(Roi roi) : base(roi)
        {
            _name = "Minimum Pixel Value";
        }

        public override bool ComputeValue()
        {
            var statisticsProvider = _roi as IRoiStatisticsProvider;
            if (statisticsProvider is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                //_value = _roi.GetPixelValues().Min();
                _value = statisticsProvider.Mean;
                _valueAsString = ((double)_value).ToString("N0") + " " + _roi.ModalityLutUnits.Label;

                return true;
            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class TotalPixelValueInfoItem : RoiInfoItem
    {
        public TotalPixelValueInfoItem() : base()
        {
            _name = "Total Pixel Value";
        }
       
        public override bool ComputeValue()
        {
            _value = 5;
            if (_roi is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                var statisticsProvider = _roi as IRoiStatisticsProvider;
                if (statisticsProvider is null)
                {
                    _value = 0;

                    _valueAsString = "0";
                    return false;
                }
                else
                {
                    //_value = _roi.GetPixelValues().Min();
                    _value = statisticsProvider.Total;
                    _valueAsString = ((double)_value).ToString("N0") + " " + _roi.ModalityLutUnits.Label;

                    return true;
                }
                _valueAsString = ((double)_value).ToString("N0");

                return true;
            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class StandardDeviationInfoItem : RoiInfoItem
    {
        public StandardDeviationInfoItem() : base()
        {
            _name = "Standard Deviation";
        }
       

        public override bool ComputeValue()
        {
            _value = 5;
            if (_roi is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                var statisticsProvider = _roi as IRoiStatisticsProvider;
                if (statisticsProvider is null)
                {
                    _value = 0;

                    _valueAsString = "0";
                    return false;
                }
                else
                {
                    //_value = _roi.GetPixelValues().Min();
                    _value = statisticsProvider.StandardDeviation;
                    _valueAsString = ((double)_value).ToString("N0") + " " + _roi.ModalityLutUnits.Label;

                    return true;
                }
              

                
            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class RelativeStandardDeviationInfoItem : RoiInfoItem
    {
        public RelativeStandardDeviationInfoItem() : base()
        {
            _name = "Relative Standard Deviation";
        }


        public override bool ComputeValue()
        {
            _value = 5;
            if (_roi is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                var statisticsProvider = _roi as IRoiStatisticsProvider;
                if (statisticsProvider is null)
                {
                    _value = 0;

                    _valueAsString = "0";
                    return false;
                }
                else
                {
                    //_value = _roi.GetPixelValues().Min();
                    _value = statisticsProvider.StandardDeviation/statisticsProvider.Mean;
                    _valueAsString = ((double)_value).ToString("P1");

                    return true;
                }



            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class AreaRoiInfoItem : RoiInfoItem
    {
        public AreaRoiInfoItem() : base()
        {
            _name = "Count";
        }

        public override bool ComputeValue()
        {
            _value = 5;
            
            if (_roi is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                var areaProvider = _roi as IRoiAreaProvider;
                if (areaProvider is null)
                {
                    _value = 0;

                    _valueAsString = "0";
                    return false;
                }
                else
                {
                    //_value = _roi.GetPixelValues().Min();
                    areaProvider.Units = Units.Pixels;
                    _value = areaProvider.Area;
                   
                    _valueAsString = ((double)_value).ToString("N1") + "  pixels";

                    return true;
                }



            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class CalibratedAreaRoiInfoItem : RoiInfoItem
    {
        public CalibratedAreaRoiInfoItem() : base()
        {
            _name = "Area";
        }

        public override bool ComputeValue()
        {
            _value = 5;
            if (_roi is null)
            {
                _value = 0;

                _valueAsString = "0";
                return false;
            }
            else
            {
                var areaProvider = _roi as IRoiAreaProvider;
                if (areaProvider is null)
                {
                    _value = 0;

                    _valueAsString = "0";
                    return false;
                }
                else
                {
                    //_value = _roi.GetPixelValues().Min();
                    areaProvider.Units = Units.Centimeters;
                    _value = areaProvider.Area;

                    _valueAsString = ((double)_value).ToString("N1") + " cm²";

                    return true;
                }



            }



        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class HeightRoiInfoItem : RoiInfoItem
    {
        public HeightRoiInfoItem() : base()
        {
            _name = "Height";
        }

        public override bool ComputeValue()
        {
            if(_roi is null)
            {
                _value = 0;
                _valueAsString = "0";
                return true;
            }
           if(_roi.BoundingBox != null)
            {
                if (_roi.NormalizedPixelSpacing.IsNull)
                {
                    _value = _roi.BoundingBox.Height;
                    _valueAsString = ((double)_value).ToString("N0") + " pixels";
                    return true;
                }
                else
                {
                    _value = _roi.BoundingBox.Height * _roi.NormalizedPixelSpacing.Row;
                    _valueAsString = ((double)_value).ToString("N0") + " mm";
                    return true;
                }
            }

            _value = 5;
            _valueAsString = "Hello World";
            return true;
        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class WidthRoiInfoItem : RoiInfoItem
    {
        public WidthRoiInfoItem() : base()
        {
            _name = "Width";
        }

        public override bool ComputeValue()
        {
            if (_roi is null)
            {
                _value = 0;
                _valueAsString = "0";
                return true;
            }
            if (_roi.BoundingBox != null)
            {
                if (_roi.NormalizedPixelSpacing.IsNull)
                {
                    _value = _roi.BoundingBox.Width;
                    _valueAsString = ((double)_value).ToString("N0") + " pixels";
                    return true;
                }
                else
                {
                    _value = _roi.BoundingBox.Width * _roi.NormalizedPixelSpacing.Row;
                    _valueAsString = ((double)_value).ToString("N0") + " mm";
                    return true;
                }
            }

            _value = 5;
            _valueAsString = "Hello World";
            return true;
        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class NameRoiInfoItem : RoiInfoItem
    {
        public NameRoiInfoItem() : base()
        {
            _name = "Name";
        }

        public override bool ComputeValue()
        {
            if(_component != null)
            {
                if(_component.SelectedRoiGraphic != null)
                {
                    _value = _component.SelectedRoiGraphic.Name;                   
                    _valueAsString = _value.ToString();
                    
                    
                    return true;
                }
                
            }
            _value = "";
            _valueAsString = "";
            return true;


        }

        public override void SetValue(object value)
        {
            if (_component != null)
            {
                if (_component.SelectedRoiGraphic != null)
                {
                    if(_component.SelectedRoiGraphic.Name != value.ToString())
                    {
                        _component.SelectedRoiGraphic.Name = value.ToString();
                        _valueAsString = _value.ToString();
                        _component.SelectedRoiGraphic.Refresh();
                        _component.SelectedRoiGraphic.ParentPresentationImage.Draw();
                    }
                    
                   
                }

            }
        }
    }

    [ExtensionOf(typeof(RoiInfoItemExtensionPoint))]
    public class RotationRoiInfoItem : RoiInfoItem
    {
        public RotationRoiInfoItem() : base()
        {
            _name = "Rotation";
        }

        public override bool ComputeValue()
        {
            if (_component != null)
            {
                if (_component.SelectedRoiGraphic != null)
                {
                    _value = _component.SelectedRoiGraphic.SpatialTransform.RotationXY;
                    
                    _valueAsString = _value.ToString() + "°";
                    return true;
                }

            }
            _value = "";
            _valueAsString = "";
            return true;


        }

        public override void SetValue(object value)
        {
            if (_component != null)
            {
                if (_component.SelectedRoiGraphic != null)
                {
                    if(int.TryParse(value.ToString(),out int rotation))
                    {
                        _component.SelectedRoiGraphic.SpatialTransform.RotationXY = rotation;
                    }
                    

                    
                   
                }

            }
        }
    }

}
