using System.Collections.Generic;
using ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public interface IOverlayManager
    {
        string Name { get; }
        string DisplayName { get; }

        bool IsConfigurable { get; }

        bool IsSelectedByDefault(string modality);

        void ShowOverlay(IPresentationImage image);
        void HideOverlay(IPresentationImage image);
    }

    public abstract class OverlayManager : IOverlayManager
    {
        protected OverlayManager(string name, string displayName)
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

        internal static List<IOverlayManager> CreateAll()
        {
            return new List<IOverlayManager>
                       {
                           new TextOverlayManager(),
                           new ScaleOverlayManager(),
                           new ShutterOverlayManager(),
                           new DicomOverlayManager(),
                           new ColorBarManager()
                       };
        } 
    }
}
