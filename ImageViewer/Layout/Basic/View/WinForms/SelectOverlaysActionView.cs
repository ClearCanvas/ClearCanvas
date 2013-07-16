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
                    var control = new SelectOverlaysControl2(action, CloseToolstrip){BackColor = Color.Transparent};
                    var panel = new Panel
                    { 
                        AutoSize = false,
                        BackColor = Color.Transparent,
                        //The toolstrip cuts off a few pixels on the right of the control, so just add some padding
                        Padding = new Padding(0, 0, 5, 5),
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

                    //var control = new SelectOverlaysControl(action, CloseToolstrip);
                    //control.Size = control.MinimumSize = control.PreferredSize;

                    var container = new Panel();
                    container.Controls.Add(control);
                    container.Size = container.MinimumSize = container.PreferredSize + new Size(10, 10);

                    _toolStripControlHost = new ToolStripControlHost(container)
                                                {
                                                    AutoSize = true, 
                                                    ControlAlign = ContentAlignment.TopLeft
                                                };

                    control.SizeChanged += (sender, args) =>
                                               {
                                                    Trace.WriteLine(String.Format("Control: {0}", control.Size));
                                                   //control.MinimumSize = control.Size;
                                                   container.Size = container.MinimumSize = container.PreferredSize + new Size(10, 10);
                                               };

                    container.SizeChanged += (sender, args) =>
                    {
                        Trace.WriteLine(String.Format("Container: {0}", container.Size));
                        Trace.WriteLine(String.Format("Host     : {0}", _toolStripControlHost.Size));
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
