using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysControl2 : UserControl
    {
        private class Item
        {
            private readonly IOverlay _overlay;

            public Item(IOverlay overlay)
            {
                _overlay = overlay;
            }

            public string Name { get { return _overlay.Name; } }
            public bool IsSelected
            {
                get { return _overlay.IsSelected; }
                set { _overlay.IsSelected = value; }
            }

            public override string ToString()
            {
                return _overlay.DisplayName;
            }
        }

        public SelectOverlaysControl2(SelectOverlaysAction action, Action close)
        {
            InitializeComponent();

            _listOverlays.Sorted = true;

            _close.Click += (sender, args) => close();
            _applyToAll.Click += (sender, args) =>
                                     {
                                         action.ApplyEverywhere();
                                         close();
                                     };

            foreach (var overlay in action.Overlays)
                _listOverlays.Items.Add(new Item(overlay), overlay.IsSelected);

            _listOverlays.ItemCheck += ListOverlaysOnItemCheck;
        }

        private void ListOverlaysOnItemCheck(object sender, ItemCheckEventArgs itemCheckEventArgs)
        {
            var item = (Item)_listOverlays.Items[itemCheckEventArgs.Index];
            item.IsSelected = itemCheckEventArgs.NewValue == CheckState.Checked;
        }
    }
}
