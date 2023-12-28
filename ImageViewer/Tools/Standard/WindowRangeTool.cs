using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarWindowRange", "Activate")]
    [IconSet("activate", "Icons.WindowRangeToolSmall.png", "Icons.WindowRangeToolMedium.png", "Icons.WindowRangeToolLarge.png")]
    [MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuWindowRange", "Activate")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class WindowRangeTool : ImageViewerTool
    {
        [ThreadStatic]
        private static Dictionary<IDesktopWindow, IShelf> _shelves;
        public WindowRangeTool() : base()
        {

        }

        #region static
        private static Dictionary<IDesktopWindow, IShelf> Shelves
        {
            get
            {
                if (_shelves == null)
                    _shelves = new Dictionary<IDesktopWindow, IShelf>();
                return _shelves;
            }
        }

        private static void LaunchShelf(IDesktopWindow desktopWindow, IApplicationComponent component, ShelfDisplayHint shelfDisplayHint)
        {
            IShelf shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleWindowRange, "WindowRange", shelfDisplayHint);
            Shelves[desktopWindow] = shelf;
            Shelves[desktopWindow].Closed += OnShelfClosed;
        }

        private static void OnShelfClosed(object sender, ClosedEventArgs e)
        {
            // We need to cache the owner DesktopWindow (_desktopWindow) because this tool is an 
            // ImageViewer tool, disposed when the viewer component is disposed.  Shelves, however,
            // exist at the DesktopWindow level and there can only be one of each type of shelf
            // open at the same time per DesktopWindow (otherwise things look funny).  Because of 
            // this, we need to allow this event handling method to be called after this tool has
            // already been disposed (e.g. viewer workspace closed), which is why we store the 
            // _desktopWindow variable.

            IShelf shelf = (IShelf)sender;
            shelf.Closed -= OnShelfClosed;
            Shelves.Remove(shelf.DesktopWindow);
        }
        #endregion
        public void Activate()
        {
            IDesktopWindow desktopWindow = this.Context.DesktopWindow;
            // check if a layout component is already displayed
            if (Shelves.ContainsKey(desktopWindow))
            {
                Shelves[desktopWindow].Activate();
            }
            else
            {
                LaunchShelf(desktopWindow, new WindowRangeApplicationComponent(desktopWindow,this), ShelfDisplayHint.ShowNearMouse);
            }
        }
    }
}
