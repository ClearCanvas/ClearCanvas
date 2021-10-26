using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

using ClearCanvas.Desktop.Actions;

using ClearCanvas.ImageViewer;

using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    namespace MyPlugin.Basics

    {

        [MenuAction("apply", "global-menus/MenuTools/Medical Physics/Daily QC", "Apply")]       
        [Tooltip("apply", "TooltipMyImageViewerTool")]
        [IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        // ... (other action attributes here)
        [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
        public class CountRateTool : ImageViewerTool
        {
            private IShelf _shelf;

            /// <summary>
            /// Default constructor.  A no-args constructor is required by the
            /// framework.  Do not remove.
            /// </summary>
            public CountRateTool() { }

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
                // Add code here to implement the functionality of the tool
                if (this.SelectedPresentationImage == null)
                    return;
                
                //this.ImageViewer.DesktopWindow.ShowMessageBox(this.SelectedPresentationImage.ParentDisplaySet.Description, MessageBoxActions.Ok);
                CountRateComponent component = new CountRateComponent(this.DesktopWindow);
                
                _shelf = ApplicationComponent.LaunchAsShelf(
                    this.DesktopWindow,
                    component,
                    "Daily QC Tool",
                    ShelfDisplayHint.DockLeft);
                _shelf.Closed += Shelf_Closed;
            }



            protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
            {
                base.OnPresentationImageSelected(sender, e);
                //this.ImageViewer.DesktopWindow.ShowMessageBox("OnPresentationImagSelected", MessageBoxActions.Ok);
            }

            /// <summary>
            /// Event handler for IShelf.Closed
            /// </summary>
            private void Shelf_Closed(object sender, ClosedEventArgs e)
            {
                _shelf.Closed -= Shelf_Closed;
                _shelf = null;
            }


        }

    }
}
