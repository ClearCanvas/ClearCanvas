using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            if (image == null)
                return String.Empty;

            var provider = image as IImageSopProvider;
            return provider != null ? provider.ImageSop.Modality : String.Empty;
        }

        public static IList<IOverlaySelection> GetOverlaySettings(this string modality)
        {
            return GetDefaultOverlayStates()[modality].Cast<IOverlaySelection>().ToList();
        }

        public static IImageOverlays GetOverlays(this IPresentationImage image)
        {
            return new ImageOverlays(image);
        }

        private static IEnumerable<OverlayState> GetOverlaySelectionStates(IPresentationImage image)
        {
            if (image == null)
                return GetDefaultOverlayStates().AllImagesStates.Select(s => new OverlayState(s.Name, s.IsSelected));

            var data = image.ExtensionData[typeof (Key)] as IList<OverlayState>;
            if (data == null)
            {
                var defaults = GetDefaultOverlayStates(image.ParentDisplaySet);
                data = defaults[image.GetModality()].Select(s => new OverlayState(s.Name, s.IsSelected)).ToList();
                image.ExtensionData[typeof (Key)] = data;
            }

            return data;
        }

        private static ModalityOverlayStates GetDefaultOverlayStates(IDisplaySet displaySet)
        {
            if (displaySet == null)
                return GetDefaultOverlayStates();

            var data = GetDefaultOverlayStates(displaySet.ImageViewer);
            if (data != null)
                return data;

            //Cache all the per-modality defaults in the display set so it can be looked up fast for its images.
            data = displaySet.ExtensionData[typeof(Key)] as ModalityOverlayStates;
            if (data == null)
            {
                data = GetDefaultOverlayStates();
                displaySet.ExtensionData[typeof(Key)] = data;
            }

            return data;
        }

        private static ModalityOverlayStates GetDefaultOverlayStates(IImageViewer viewer)
        {
            //Cache all the per-modality defaults in the display set so it can be looked up fast for its images.
            var data = viewer.ExtensionData[typeof(Key)] as ModalityOverlayStates;
            if (data == null)
                viewer.ExtensionData[typeof(Key)] = data = GetDefaultOverlayStates();

            return data;
        }

        private static ModalityOverlayStates GetDefaultOverlayStates()
        {
            var states = new ModalityOverlayStates();
            var settings = DisplaySetCreationSettings.DefaultInstance.GetStoredSettings();
            foreach (var setting in settings)
            {
                var modalityOverlayStates = setting.OverlaySelections.Select(s => new OverlayState(s.Name, s.IsSelected)).ToList();
                states[setting.Modality] = modalityOverlayStates;
            }

            foreach (var overlayManager in OverlayManagers)
                states.AllImagesStates.Add(new OverlayState(overlayManager.Name, overlayManager.IsSelectedByDefault(String.Empty)));

            return states;
        }

        internal static void OverlaySettingsChanged(IImageViewer viewer)
        {
            viewer.ExtensionData[typeof(Key)] = null;
        }

        #region State Classes

        private class Key
        {
        }

        private class OverlayState : IOverlaySelection
        {
            public OverlayState(string name, bool isSelected)
            {
                Name = name;
                IsSelected = isSelected;
            }

            #region IOverlaySelection Members

            public string Name { get; private set; }
            public bool IsSelected { get; set; }

            #endregion
        }

        private class ModalityOverlayStates
        {
            private readonly Dictionary<string, IList<OverlayState>> _modalityStates;

            public ModalityOverlayStates()
            {
                _modalityStates = new Dictionary<string, IList<OverlayState>>();
                AllImagesStates = new List<OverlayState>();
            }

            public readonly List<OverlayState> AllImagesStates;

            public IList<OverlayState> this[string modality]
            {
                get
                {
                    if (modality == null)
                        modality = String.Empty;

                    IList<OverlayState> overlays;
                    if (!_modalityStates.TryGetValue(modality, out overlays))
                        return AllImagesStates.AsReadOnly();

                    return overlays;
                }
                set
                {
                    if (modality == null)
                        modality = String.Empty;

                    if (value == null)
                        _modalityStates.Remove(modality);
                    else
                        _modalityStates[modality] = value;
                }
            }
        }

        #endregion
    }
}
