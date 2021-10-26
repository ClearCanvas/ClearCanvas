using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    [ExtensionOf(typeof(WindowRangeApplicationComponentViewExtensionPoint))]
    public class WindowRangeApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private WindowRangeApplicationComponentControl _control;
        private WindowRangeApplicationComponent _component;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WindowRangeApplicationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new WindowRangeApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
