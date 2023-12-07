using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Export
{

    [MenuAction("activate", "global-menus/MenuTools/MenuExport/MenuExportDicomFiles", "Activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class ExportDicomTool : ImageViewerTool
    {
        public ExportDicomTool()
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
        {
            base.OnPresentationImageSelected(sender, e);
            if(e.SelectedPresentationImage is DicomGrayscalePresentationImage)            
                Enabled = true;           
            else
                Enabled = false;
        }

        public void Activate()
        {
            
            var image = this.SelectedPresentationImage as DicomGrayscalePresentationImage;
            if (image is null)
                return;
            var modality = image.ImageSop.Modality;
            switch (modality)
            {
                case "NM":
                    if(image.ImageSop.IsMultiframe)
                    {

                    }
                    else
                    {
                        var msgSource = image.ImageSop.DataSource as IStreamingSopDataSource;
                        if (msgSource is null)
                            return;                       
                        var rows = image.ImageGraphic.Rows;
                        var columns = image.ImageGraphic.Columns;
                        var bitsAllocated = image.ImageSop[DicomTags.BitsStored].GetInt16(0,16);
                        var samplesPerPixel = image.ImageSop[DicomTags.SamplesPerPixel].GetInt16(0,1);
                        byte[] pixelData = new byte[rows * columns * (bitsAllocated/8) * samplesPerPixel];
                        var df = msgSource.SourceMessage as DicomFile;
                        pixelData = image.ImageGraphic.PixelData.Raw;
                        df.DataSet[DicomTags.PixelData].Values = pixelData;
                        //var dialog = new Microsoft.Win32.
                        df.Save("ouput.dcm", DicomWriteOptions.Default);
                    }
                    break;
                case "CT":
                case "PT":

                    break;
            }
           

        }
    }
}
