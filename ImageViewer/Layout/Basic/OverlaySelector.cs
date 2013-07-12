using System.Collections.Generic;
using ClearCanvas.ImageViewer.Layout.Basic.OverlaySelectors;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public interface IOverlaySelector
    {
        string Name { get; }
        string DisplayName { get; }

        bool IsConfigurable { get; }

        bool IsSelectedByDefault(string modality);

        void ShowOverlay(IPresentationImage image);
        void HideOverlay(IPresentationImage image);
    }

    public abstract class OverlaySelector : IOverlaySelector
    {
        protected OverlaySelector(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
            IsConfigurable = true;
        }

        #region Implementation of IOverlaySelection

        public string Name { get; private set; }
        public string DisplayName { get; private set; }

        public bool IsConfigurable { get; protected set; }

        public abstract bool IsSelectedByDefault(string modality);

        public void ShowOverlay(IPresentationImage image)
        {
            SetOverlayVisible(image, true);
        }

        public void HideOverlay(IPresentationImage image)
        {
            SetOverlayVisible(image, false);
        }

        #endregion

        public abstract void SetOverlayVisible(IPresentationImage image, bool visible);

        public static IList<IOverlaySelector> CreateAll()
        {
            return new List<IOverlaySelector>
                       {
                           new TextOverlaySelector(),
                           new ScaleOverlaySelector(),
                           new ShutterOverlaySelector(),
                           new DicomOverlaySelector(),
                           new ColorBarSelector()
                       };
        } 
    }
}
