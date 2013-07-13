using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    [ExtensionOf(typeof(SelectOverlaysActionViewExtensionPoint))]
    public class SelectOverlaysActionView : WinFormsActionView
    {
        private object _guiElement;

        #region Overrides of WinFormsView

        public override object GuiElement
        {
            get
            {
                if (_guiElement == null)
                    _guiElement = new SelectOverlaysToolstripItem((SelectOverlaysAction)base.Context.Action);

                return _guiElement;
            }
        }

        #endregion
    }
}
