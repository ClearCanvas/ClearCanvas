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
        string DisplayName { get; }

        bool IsSelected { get; set; }
    }

    public static class OverlayExtensions
    {
        public static void ShowSelectedOverlays(this IPresentationImage image)
        {
            OverlayManager.ShowSelectedOverlays(image);
        }

        public static void HideOverlays(this IPresentationImage image)
        {
            OverlayManager.HideOverlays(image);
        }
    }

    public static class OverlayManager
    {
        private class Key{}

        [Cloneable(false)]
        private class OverlayState : IOverlaySelection
        {
            private readonly IOverlaySelection _setting;
            private readonly bool _isSelectedByDefault;

            public OverlayState(IOverlaySelection setting, bool isSelected)
            {
                _setting = setting;
                IsSelected = _isSelectedByDefault = isSelected;
            }

            public OverlayState(OverlayState original)
            {
                _setting = original._setting;
                _isSelectedByDefault = original._isSelectedByDefault;
                IsSelected = original.IsSelected;
            }

            private OverlayState(OverlayState original, ICloningContext context)
                : this(original)
            {
            }

            #region IOverlaySelection Members

            public string Name { get { return _setting.Name; } }
            public string DisplayName { get { return _setting.DisplayName; } }
            public bool IsSelected { get; set; }

            #endregion
        }

        public static IList<IOverlaySelector> Selectors = new ReadOnlyCollection<IOverlaySelector>(OverlaySelector.CreateAll());

        public static string GetModality(IPresentationImage image)
        {
            var provider = image as IImageSopProvider;
            return provider != null ? provider.ImageSop.Modality : string.Empty;
        }

        public static IList<IOverlaySelection> GetOverlaySelections(string modality)
        {
            var settings = DisplaySetCreationSettings.DefaultInstance.GetStoredSettings();
            var setting = settings.FirstOrDefault(s => s.Modality == modality);
            if (setting == null)
                return new List<IOverlaySelection>();

            return setting.OverlaySelectionOptions.Select(o => (IOverlaySelection)new OverlayState(o, o.IsSelected)).ToList();
        }

        public static IList<IOverlaySelection> GetOverlaySelections(IPresentationImage image)
        {
            var data = image.ExtensionData[typeof(Key)] as IList<IOverlaySelection>;
            if (data == null)
            {
                //var parentDisplaySet = image.ParentDisplaySet;
                //if (parentDisplaySet != null)
                //{
                //}
                
                data = GetOverlaySelections(GetModality(image));
                image.ExtensionData[typeof (Key)] = data;
            }

            return data;
        }

        public static void SetOverlaySelection(IPresentationImage image, string name, bool isSelected)
        {
            SetOverlaySelections(image, new Dictionary<string, bool>{{name, isSelected}});
        }

        public static void SetOverlaySelections(IPresentationImage image, IDictionary<string, bool> selections)
        {
            var imageSelections = GetOverlaySelections(image);
            foreach (var selection in selections)
            {
                var s = selection;
                var imageSelection = imageSelections.FirstOrDefault(i => i.Name == s.Key);
                if (imageSelection != null)
                    imageSelection.IsSelected = s.Value;
            }
        }

        public static void ShowSelectedOverlays(IPresentationImage image)
        {
            var selections = GetOverlaySelections(image);
            foreach (var selection in selections)
            {
                var n = selection;
                var selector = Selectors.FirstOrDefault(s => s.Name == n.Name);
                if (selector == null)continue;

                if (selection.IsSelected)
                    selector.ShowOverlay(image);
                else
                    selector.HideOverlay(image);
            }
        }

        public static void HideOverlays(IPresentationImage image)
        {
            var selections = GetOverlaySelections(image);
            foreach (var selection in selections)
            {
                var n = selection;
                var selector = Selectors.FirstOrDefault(s => s.Name == n.Name);
                if (selector != null)
                    selector.HideOverlay(image);
            }
        }
    }
}
