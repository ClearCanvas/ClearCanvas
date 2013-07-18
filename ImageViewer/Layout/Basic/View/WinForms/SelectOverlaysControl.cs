using System;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysControl : UserControl
    {
        public SelectOverlaysControl(SelectOverlaysAction action, Action close)
        {
            InitializeComponent();

            SuspendLayout();

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
                var check = new IconCheckBox
                                {
                                    Checked = overlayItem.IsSelected, 
                                    Enabled = action.Enabled, 
                                    Text = item.DisplayName
                                };

                if (item.IconSet != null)
                {
                    var icon = item.IconSet.CreateIcon(IconSize.Small, action.ResourceResolver);
                    check.Image = icon;
                }

                check.CheckedChanged += (sender, args) =>
                                            {
                                                item.IsSelected = check.Checked;
                                                action.Apply();
                                            };
                
                _overlaysPanel.Controls.Add(check);
            }

            ResumeLayout();
        }
    }
}
