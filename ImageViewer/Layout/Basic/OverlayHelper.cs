using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public interface IOverlaySelection
    {
        string Name { get; }
        bool IsSelected { get; }
    }

    public static partial class OverlayHelper
    {
        public static IList<IOverlayManager> OverlayManagers = new ReadOnlyCollection<IOverlayManager>(OverlayManager.CreateAll());

        public static string GetModality(this IPresentationImage image)
        {
            var provider = image as IImageSopProvider;
            return provider != null ? provider.ImageSop.Modality : String.Empty;
        }

        public static IList<IOverlaySelection> GetOverlaySettings(this string modality)
        {
            var settings = DisplaySetCreationSettings.DefaultInstance.GetStoredSettings();
            var setting = settings.FirstOrDefault(s => s.Modality == modality);
            return setting == null ? 
                new List<IOverlaySelection>()
                : setting.OverlaySelections.Cast<IOverlaySelection>().ToList();
        }

        public static IOverlays GetOverlays(this IPresentationImage image)
        {
            return new ImageOverlays(image);
        }

        public static IOverlays GetOverlays(IDisplaySet displaySet)
        {
            if (displaySet == null || displaySet.PresentationImages.Count == 0)
                return null;

            return GetOverlays(displaySet.PresentationImages.First());
        }

        public static IOverlays GetOverlays(IImageBox imageBox)
        {
            if (imageBox.DisplaySet == null || imageBox.DisplaySet.PresentationImages.Count == 0)
                return null;

            if (imageBox.SelectedTile != null && imageBox.SelectedTile.PresentationImage != null)
                return GetOverlays(imageBox.SelectedTile.PresentationImage);

            if (imageBox.Tiles.Count > 0 && imageBox.Tiles[0].PresentationImage != null)
            return GetOverlays(imageBox.Tiles[0].PresentationImage);

            return GetOverlays(imageBox.DisplaySet.PresentationImages[0]);
        }

        private static IEnumerable<OverlayState> GetOverlaySelectionStates(IPresentationImage image)
        {
            var data = image.ExtensionData[typeof(Key)] as IList<OverlayState>;
            if (data == null)
            {
                data = GetOverlaySettings(GetModality(image)).Select(s => new OverlayState(s.Name, s.IsSelected)).ToList();
                image.ExtensionData[typeof(Key)] = data;
            }

            return data;
        }

        #region State Classes

        private class Key { }

        [Cloneable(false)]
        private class OverlayState : IOverlaySelection
        {
            private readonly bool _isSelectedByDefault;

            public OverlayState(string name, bool isSelected)
            {
                Name = name;
                IsSelected = _isSelectedByDefault = isSelected;
            }

            public OverlayState(OverlayState original)
            {
                _isSelectedByDefault = original._isSelectedByDefault;
                IsSelected = original.IsSelected;
            }

            private OverlayState(OverlayState original, ICloningContext context)
                : this(original)
            {
            }

            #region IOverlaySelection Members

            public string Name { get; private set; }
            public bool IsSelected { get; set; }

            #endregion
        }

        #endregion
    }
}
