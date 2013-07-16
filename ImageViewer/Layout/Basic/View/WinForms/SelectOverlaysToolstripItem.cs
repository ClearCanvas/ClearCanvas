using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysToolstripItem : ToolStripControlHost
    {
        private ToolStrip _owner;
        private Panel _panel;
        private SelectOverlaysControl _control;
        private Size _defaultSize;
        private Size _preferredSize;

        public SelectOverlaysToolstripItem(SelectOverlaysAction action)
            : base(new Panel())
        {
            AutoSize = false;

            _panel = (Panel)Control;

            _panel.AutoSize = false;
            _panel.Padding = new Padding(0);
            _panel.Margin = new Padding(0);

            _control = new SelectOverlaysControl(action, PerformClick);
            _control.SizeChanged += (o, args) => SetPanelSize();

            _panel.Controls.Add(_control);

            SetPanelSize();
            SetSize();
            _defaultSize = _preferredSize = Size;
            base.ControlAlign = ContentAlignment.TopCenter;
            this.MyOwner = base.Owner;

            InitializeComponent();
        }

        private void SetPanelSize()
        {
            _panel.Size = _panel.MinimumSize = _control.Size + _control.Margin.Size;
        }

        private void SetSize()
        {
            _preferredSize = base.Size = _panel.Size + new Size(5, 5);
        }

        private ToolStrip MyOwner
        {
            get { return _owner; }
            set
            {
                if (_owner != value)
                {
                    if (_owner != null)
                        _owner.Resize -= OnOwnerResize;

                    _owner = value;

                    if (_owner != null)
                        _owner.Resize += OnOwnerResize;
                }
            }
        }

        private void OnOwnerResize(object sender, EventArgs e)
        {
            SetPanelSize();
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            this.MyOwner = base.Owner;
            base.OnOwnerChanged(e);
        }
        
        protected override Size DefaultSize
        {
            get
            {
                return _defaultSize;
            }
        }

        protected override void OnHostedControlResize(EventArgs e)
        {
            SetSize();

            base.OnHostedControlResize(e);
            
            Trace.WriteLine(String.Format("Control       : {0}", _control.Size));
            Trace.WriteLine(String.Format("Outer Panel   : {0}", _panel.Size));
            Trace.WriteLine(String.Format("Host          : {0}", Size));
            Trace.WriteLine(String.Format("Host (default): {0}", DefaultSize));
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            return _preferredSize;
        }

        protected override bool DismissWhenClicked
        {
            get { return true; }
        }
    }
}
