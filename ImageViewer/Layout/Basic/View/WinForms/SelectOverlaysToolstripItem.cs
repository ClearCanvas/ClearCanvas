using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysToolstripItem : ToolStripControlHost
    {
        private Panel _panel;
        private ToolStrip _owner;

        public SelectOverlaysToolstripItem(SelectOverlaysAction action)
            : base(new Panel())
        {
            InitializeComponent();
            _panel = (Panel)Control;
            _panel.BackColor = Color.Transparent;
            var table = new TableLayoutPanel {AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.Transparent};
            table.RowStyles.Clear();
            table.ColumnStyles.Clear();
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            foreach (var overlay in action.Overlays)
            {
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var theOverlay = overlay;

                var check = new CheckBox { Text = theOverlay.DisplayName, Checked = theOverlay.IsSelected };
                check.CheckedChanged += (sender, args) => theOverlay.IsSelected = check.Checked;
                table.Controls.Add(check);
            }

            _panel.Controls.Add(table);
        }

        /// <remarks>
        /// Yes, this is an incredibly convoluted way to determine max width of toolstripitems in the same menu at runtime
        /// However, it is the only one that seems to work.
        /// </remarks>
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
            int maxWidth = _panel.Width;
            foreach (ToolStripItem item in this.MyOwner.Items)
            {
                maxWidth = Math.Max(item.Width, maxWidth);
            }
            _panel.Size = new Size(maxWidth, _panel.Height);
        }

        protected override bool DismissWhenClicked
        {
            get { return true; }
        }

        public override System.Drawing.Size GetPreferredSize(System.Drawing.Size constrainingSize)
        {
            return base.GetPreferredSize(constrainingSize);
        }
    }
}
