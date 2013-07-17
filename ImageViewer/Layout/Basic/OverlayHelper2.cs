using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public interface IImageOverlays : IEnumerable<IImageOverlay>
    {
        int Count { get; }
        IImageOverlay this[string name] { get; }

        void ShowSelected(bool draw);
        void Hide(bool draw);
    }

    public interface IImageOverlay : IOverlaySelection
    {
        string DisplayName { get; }
        new bool IsSelected { get; set; }

        void ShowIfSelected();
        void Hide();
    }
    
    public static partial class OverlayHelper
    {
        private class ImageOverlay : IImageOverlay
        {
            private readonly IPresentationImage _image;
            private readonly IOverlayManager _manager;
            private readonly OverlayState _state;

            internal ImageOverlay(IPresentationImage image, OverlayState state)
            {
                _image = image;
                _manager = OverlayManagers.First(m => m.Name == state.Name);
                _state = state;
            }

            #region Implementation of IOverlaySelection

            public string Name
            {
                get { return _manager.Name; }
            }

            public bool IsSelected
            {
                get { return _state.IsSelected; }
                set { _state.IsSelected = value; }
            }

            #endregion

            #region Implementation of IOverlay

            public string DisplayName
            {
                get { return _manager.DisplayName; }
            }

            public void ShowIfSelected()
            {
                if (_image == null)return;

                if (IsSelected)
                    _manager.ShowOverlay(_image);
                else
                    _manager.HideOverlay(_image);
            }

            public void Hide()
            {
                if (_image != null)
                    _manager.HideOverlay(_image);
            }

            #endregion
        }

        public class ImageOverlays : IImageOverlays
        {
            private readonly IPresentationImage _image;
            private readonly IList<IImageOverlay> _overlays;

            internal ImageOverlays(IPresentationImage image)
            {
                _image = image;
                _overlays = GetOverlaySelectionStates(image).Select(state => (IImageOverlay)new ImageOverlay(image, state)).ToList();
            }

            #region Implementation of IImageOverlays

            public int Count
            {
                get { return _overlays.Count; }
            }

            public IImageOverlay this[string name]
            {
                get { return _overlays.FirstOrDefault(o => o.Name == name); }
            }

            public void ShowSelected(bool draw)
            {
                foreach (ImageOverlay overlay in _overlays)
                    overlay.ShowIfSelected();

                if (draw)
                    _image.Draw();
            }

            public void Hide(bool draw)
            {
                foreach (ImageOverlay overlay in _overlays)
                    overlay.Hide();

                if (draw)
                    _image.Draw();
            }

            #endregion

            #region Implementation of IEnumerable

            public IEnumerator<IImageOverlay> GetEnumerator()
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