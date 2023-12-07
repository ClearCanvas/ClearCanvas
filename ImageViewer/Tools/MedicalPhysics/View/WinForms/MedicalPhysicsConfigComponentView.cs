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
    [ExtensionOf(typeof(MedicalPhysicsConfigComponentViewExtensionPoint))]
    public class MedicalPhysicsConfigComponentView : WinFormsView, IApplicationComponentView
    {
        private MedicalPhysicsConfigComponent _component;
        private MedicalPhysicsConfigControl _control;

        public void SetComponent(IApplicationComponent component)
        {
            _component = (MedicalPhysicsConfigComponent)component;
        }



        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new MedicalPhysicsConfigControl(_component);
                }
                return _control;

            }

        }
    }
}
