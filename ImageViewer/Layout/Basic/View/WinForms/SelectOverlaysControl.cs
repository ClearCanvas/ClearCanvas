using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysControl : UserControl
    {
        public SelectOverlaysControl(SelectOverlaysAction action, Action close)
        {
            InitializeComponent();

            SuspendLayout();

            _itemsTable.ColumnCount = 1;
            _itemsTable.RowCount = action.Items.Count;
            _itemsTable.RowStyles.Clear();
            _itemsTable.ColumnStyles.Clear();
            _itemsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute) {Width= 100});

            _itemsTable.Enabled = action.Enabled;
            _applyToAll.Enabled = action.Enabled;

            foreach (var overlay in action.Items)
            {
                _itemsTable.RowStyles.Add(new RowStyle(SizeType.Percent) {Height = 100});
                var theOverlay = overlay;
                var check = new CheckBox {
                    AutoSize = true, Text = theOverlay.DisplayName, Checked = theOverlay.IsSelected
                };
                check.CheckedChanged += (sender, args) => theOverlay.IsSelected = check.Checked;
                _itemsTable.Controls.Add(check);
            }

            ResumeLayout(true);
            Size = PreferredSize;

            _close.Click += (sender, args) => close();
            _applyToAll.Click += (sender, args) =>
                                     {
                                         action.ApplyEverywhere();
                                         close();
                                     };
        }
    }
}
