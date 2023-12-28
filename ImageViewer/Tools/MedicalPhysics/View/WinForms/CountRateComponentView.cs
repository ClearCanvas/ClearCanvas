using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{

    [ExtensionOf(typeof(CountRateComponentViewExtensionPoint))]
    public class CountRateComponentView : WinFormsView, IApplicationComponentView
    {
        private CountRateComponent _component;
        private CountRateComponentControl _control;
        public void SetComponent(IApplicationComponent component)
        {
            _component = (CountRateComponent)component;
        }

        public override object GuiElement
        {
            get
            {
                if(_control is null)
                {
                    _control = new CountRateComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
