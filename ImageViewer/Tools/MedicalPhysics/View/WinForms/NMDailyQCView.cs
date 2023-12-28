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
    [ExtensionOf(typeof(NMDailyQCComponentViewExtensionPoint))]
    public class NMDailyQCView : WinFormsView, IApplicationComponentView
    {
        private NMDailyQCComponent _component;
        private NMDailyQCControl _control;
        public void SetComponent(IApplicationComponent component)
        {
            _component = (NMDailyQCComponent)component;
        }

        public override object GuiElement
        {
            get
            {
                if (_control is null)
                {
                    _control = new NMDailyQCControl(_component);
                }
                return _control;
            }
        }
    }
}
