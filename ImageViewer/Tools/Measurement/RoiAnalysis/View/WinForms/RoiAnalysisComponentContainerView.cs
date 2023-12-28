using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis.View.WinForms
{
    [ExtensionOf(typeof(RoiAnalysisComponentContainerViewExtensionPoint))]
    public class RoiAnalysisComponentContainerView : WinFormsView, IApplicationComponentView
    {
        private RoiAnalysisComponentContainerControl _control;
        private RoiAnalysisComponentContainer _component;

        public override object GuiElement 
        {
            get
            {
                if(_control == null)
                {
                    _control = new RoiAnalysisComponentContainerControl(_component);
                }
                return _control;
            }
        }

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RoiAnalysisComponentContainer)component;
        }
    }
}
