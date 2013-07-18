using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    [ExtensionOf(typeof(SelectOverlaysActionViewExtensionPoint))]
    public class SelectOverlaysActionView : WinFormsActionView
    {
        private ToolStripControlHost _toolStripControlHost;

        #region Overrides of WinFormsView

        public override object GuiElement
        {
            get
            {
                if (_toolStripControlHost == null)
                {
                    var action = (SelectOverlaysAction)base.Context.Action;
                    var control = new SelectOverlaysControl(action, CloseToolstrip){BackColor = Color.Transparent};
                    //Seems we have to put the control inside a panel in order for it to be sized properly.
                    var panel = new Panel
                    { 
                        AutoSize = false,
                        BackColor = Color.Transparent,
                        Padding = new Padding(0, 0, 0, 0),
                        Margin = new Padding(0) 
                    };
                    
                    panel.Controls.Add(control);
                    control.Location = new Point(0,0);
                    panel.Size = panel.MinimumSize = panel.PreferredSize;
                    
                    return _toolStripControlHost = new ToolStripControlHost(panel)
                                                       {
                                                           AutoSize = false,
                                                           ControlAlign = ContentAlignment.TopLeft,
                                                           Margin = new Padding(0),
                                                           Padding = new Padding(0)
                                                       };
                }

                return _toolStripControlHost;
            }
        }

        private void CloseToolstrip()
        {
            _toolStripControlHost.PerformClick();
        }

        #endregion
    }
}
