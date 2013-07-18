using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysControl2 : UserControl
    {
        public SelectOverlaysControl2(SelectOverlaysAction action, Action close)
        {
            InitializeComponent();

            _applyToAll.Enabled = action.Enabled;

            _close.Click += (sender, args) =>
                                {
                                    action.Apply();
                                    close();
                                };
            _applyToAll.Click += (sender, args) =>
                                     {
                                         action.ApplyEverywhere();
                                         close();
                                     };

            foreach (var overlayItem in action.Items)
            {
                var item = overlayItem;
                var check = new CheckBox {Checked = overlayItem.IsSelected, Enabled = action.Enabled, Text = item.DisplayName};
                check.CheckedChanged += (sender, args) =>
                                            {
                                                item.IsSelected = check.Checked;
                                                action.Apply();
                                            };
                
                _overlaysPanel.Controls.Add(check);
            }
        }
    }
}
