using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public class SelectOverlaysActionViewExtensionPoint : ExtensionPoint<IActionView>
    {
    }

    [AssociateView(typeof(SelectOverlaysActionViewExtensionPoint))]
    public class SelectOverlaysAction : Action
    {
        public SelectOverlaysAction(IImageViewer viewer, string actionID, ActionPath path, IResourceResolver resourceResolver)
            : base(actionID, path, resourceResolver)
        {
            Overlays = new ImageBoxOverlays(viewer.SelectedImageBox);
        }

        public IOverlays Overlays { get; private set; }

        private class ImageBoxOverlay : IOverlay
        {
            private readonly IImageBox _imageBox;
            private readonly IOverlayManager _manager;
            private bool _isSelected;

            public ImageBoxOverlay(IImageBox imageBox, string name, bool isSelected)
            {
                _imageBox = imageBox;
                _manager = OverlayHelper.OverlayManagers.First(m => m.Name == name);
                _isSelected = isSelected;
            }

            #region Implementation of IOverlaySelection

            public string Name
            {
                get { return _manager.Name; }
            }

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    if (_isSelected == value) return;

                    _isSelected = value;

                    if (_isSelected)
                        ShowIfSelected();
                    else
                        Hide();
                }
            }

            #endregion

            #region Implementation of IOverlay

            public string DisplayName
            {
                get { return _manager.DisplayName; }
            }

            public void ShowIfSelected()
            {
                foreach (var image in _imageBox.DisplaySet.PresentationImages)
                {
                    var overlay = image.GetOverlays()[Name];
                    overlay.IsSelected = _isSelected;
                    overlay.ShowIfSelected();
                }

                _imageBox.Draw();
            }

            public void Hide()
            {
                foreach (var image in _imageBox.DisplaySet.PresentationImages)
                {
                    var overlay = image.GetOverlays()[Name];
                    overlay.IsSelected = _isSelected;
                    overlay.Hide();
                }

                _imageBox.Draw();
            }

            #endregion
        }

        private class ImageBoxOverlays : IOverlays
        {
            private readonly IImageBox _imageBox;
            private readonly IList<IOverlay> _overlays;

            public ImageBoxOverlays(IImageBox imageBox)
            {
                _imageBox = imageBox;
                IPresentationImage image;
                if (imageBox.SelectedTile != null && imageBox.SelectedTile.PresentationImage != null)
                {
                    image = imageBox.SelectedTile.PresentationImage;
                }
                else if (imageBox.Tiles.Count > 0 && imageBox.Tiles[0].PresentationImage != null)
                {
                    image = imageBox.Tiles[0].PresentationImage;
                }
                else if (imageBox.DisplaySet != null && imageBox.DisplaySet.PresentationImages.Count > 0)
                    image = imageBox.DisplaySet.PresentationImages[0];
                else
                {
                    image = null;
                }

                _overlays = image != null
                    ? image.GetOverlays().Select(o => (IOverlay)new ImageBoxOverlay(_imageBox, o.Name, o.IsSelected)).ToList()
                    : new List<IOverlay>();
            }

            #region IOverlays Members

            public int Count
            {
                get { return _overlays.Count; }
            }

            public IOverlay this[string name]
            {
                get { return _overlays.FirstOrDefault(o => o.Name == name); }
            }

            public void ShowSelected(bool draw)
            {
                foreach (var overlay in _overlays)
                    overlay.ShowIfSelected();

                if (draw)
                    _imageBox.Draw();
            }

            public void Hide(bool draw)
            {
                foreach (var overlay in _overlays)
                    overlay.Hide();

                if (draw)
                    _imageBox.Draw();
            }

            #endregion

            #region Implementation of IEnumerable

            public IEnumerator<IOverlay> GetEnumerator()
            {
                return _overlays.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

    }
}