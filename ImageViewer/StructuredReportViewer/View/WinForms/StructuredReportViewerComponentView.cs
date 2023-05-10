using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.StructuredReportViewer.View.WinForms
{
    [ExtensionOf(typeof(StructuredReportViewerComponentViewExtensionPoint))]
    public class StructuredReportViewerComponentView : WinFormsView, IApplicationComponentView
    {
        private StructuredReportViewerComponent _component;
        private StructuredReportViewerComponentControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control is null)
                {
                    _control = new StructuredReportViewerComponentControl(_component);
                }
                return _control;
            }
        }

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StructuredReportViewerComponent)component;
        }
    }
}
