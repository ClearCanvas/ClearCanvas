using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public interface IOverlayManager
    {
        /// <summary>
        /// Gets a unique name for the manager.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a name for display to the user.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets whether or not "selection" of this overlay is configurable.
        /// </summary>
        /// <remarks>
        /// If false, it is expected that <see cref="IsSelectedByDefault"/> will always return true,
        /// since the only reason for not allowing configuration is that it's important for the
        /// user to see it, at least initially.
        /// </remarks>
        bool IsConfigurable { get; }

        /// <summary>
        /// Gets an <see cref="IconSet"/> for display to the user.
        /// </summary>
        IconSet IconSet { get; }

        /// <summary>
        /// An <see cref="IResourceResolver"/> for resolving the <see cref="IconSet"/> resources.
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Gets whether or not the overlay is to be "selected" by default for the given modality.
        /// </summary>
        bool IsSelectedByDefault(string modality);

        /// <summary>
        /// Shows the overlay on the given image.
        /// </summary>
        void ShowOverlay(IPresentationImage image);

        /// <summary>
        /// Hides the overlay on the given image.
        /// </summary>
        void HideOverlay(IPresentationImage image);
    }

    public abstract class OverlayManager : IOverlayManager
    {
        private IResourceResolver _resolver;

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

        public IconSet IconSet { get; protected set; }

        public IResourceResolver ResourceResolver
        {
            get
            {
                return _resolver ?? (_resolver = new ApplicationThemeResourceResolver(this.GetType().Assembly));
            }
            protected set { _resolver = value; }
        }

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
                           new DicomOverlayManager(),
                           new ShutterOverlayManager(),
                           new ColorBarManager()
                       };
        } 
    }
}
