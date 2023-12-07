using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    [ExtensionOf(typeof(WeeklyCTComponentViewExtensionPoint))]
    public class WeeklyCTComponentView : WinFormsView, IApplicationComponentView
    {
        private WeeklyCTComponent _component;
        private WeeklyCTComponentControl _control;
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                    _control = new WeeklyCTComponentControl(_component);

                return _control;
            }

        }

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WeeklyCTComponent)component;
        }
    }
}
