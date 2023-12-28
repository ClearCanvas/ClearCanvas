using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Export
{

    [MenuAction("activate", "global-menus/MenuTools/MenuExport/MenuExportDicomFiles", "Activate")]
    [MenuAction("activate", "imageviewer-contextmenu/MenuExportDicomFiles", "Activate")]
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

            var result = DesktopWindow.ShowSelectFolderDialogBox(new SelectFolderDialogCreationArgs());
            if (result.Action == DialogBoxAction.Ok)
            {
                Platform.Log(LogLevel.Debug, "Result filename is " + result.FileName);
            }

            var modality = image.ImageSop.Modality;
            switch (modality)
            {
                case "NM":
                    var msgSource = image.ImageSop.DataSource as IStreamingSopDataSource;
                    if (msgSource is null)
                        return;
                    var rows = image.ImageGraphic.Rows;
                    var columns = image.ImageGraphic.Columns;
                    var bitsAllocated = image.ImageSop[DicomTags.BitsStored].GetInt16(0, 16);
                    var samplesPerPixel = image.ImageSop[DicomTags.SamplesPerPixel].GetInt16(0, 1);
                    
                    var df = msgSource.SourceMessage as DicomFile; 
                    byte[] pixelData = null;
                    int imgNum = 0;
                    int bytesInImage = rows * columns * (bitsAllocated / 8) * samplesPerPixel;
                    if (image.ImageSop.IsMultiframe)
                    {
                        pixelData = new byte[bytesInImage * image.ImageSop[DicomTags.NumberOfFrames].GetInt16(0,1)];
                        foreach(var img in image.ParentDisplaySet.PresentationImages)
                        {
                            var imgPixel = (img as IDicomPresentationImage).ImageGraphic.PixelData.Raw;
                            Buffer.BlockCopy(imgPixel,0,pixelData, (imgNum * bytesInImage), bytesInImage);
                            imgNum++;
                        }
                    }
                    else
                    {
                        pixelData = new byte[rows * columns * (bitsAllocated / 8) * samplesPerPixel];
                        pixelData = image.ImageGraphic.PixelData.Raw;
                        
                        
                    }                    
                    df.DataSet[DicomTags.PixelData].Values = pixelData;
                    df.Save(result.FileName + "\\" + image.ImageSop.SeriesDescription + ".dcm");
                    break;
                case "CT":
                case "PT":

                    break;
            }
           

        }
    }
}
