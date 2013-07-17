using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    public partial class SelectOverlaysControl2 : UserControl
    {
        private class Item
        {
            private readonly SelectOverlaysAction.Item _overlayItem;

            public Item(SelectOverlaysAction.Item overlayItem)
            {
                _overlayItem = overlayItem;
            }

            public string Name { get { return _overlayItem.Name; } }
            public bool IsSelected
            {
                get { return _overlayItem.IsSelected; }
                set { _overlayItem.IsSelected = value; }
            }

            public override string ToString()
            {
                return _overlayItem.DisplayName;
            }
        }

        public SelectOverlaysControl2(SelectOverlaysAction action, Action close)
        {
            InitializeComponent();

            _listOverlays.Sorted = false;

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
                _listOverlays.Items.Add(new Item(overlayItem), overlayItem.IsSelected);

            _listOverlays.ItemCheck += (s, args) =>
            {
                var item = (Item)_listOverlays.Items[args.Index];
                item.IsSelected = args.NewValue == CheckState.Checked;
                
                //Apply for each check
                action.Apply();
            };
        }
    }
}
