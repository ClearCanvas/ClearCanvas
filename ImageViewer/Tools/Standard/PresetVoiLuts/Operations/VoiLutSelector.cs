#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Layout;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
    public interface IVoiLutSelector
    {
        IUndoableOperation<IPresentationImage> GetOperation();
    }

    public abstract class VoiLutSelector : IVoiLutSelector
    {
        #region IVoiLutSelector Members

        public abstract IUndoableOperation<IPresentationImage> GetOperation();

        #endregion
    }

    public class AutoVoiLutSelector : VoiLutSelector
    {
        public bool IsData { get; set; }
        public string LutExplanation { get; set; }
        public int? LutIndex { get; set; }

        public static AutoVoiLutSelector CreateFrom(IPresentationImage image)
        {
            var applicator = AutoVoiLutApplicator.Create(image);
            if (applicator == null)
                return null;

            var autoLut = applicator.GetAppliedLut();
            if (autoLut == null)
                return null;

            return !autoLut.IsHeader ? null
                       : new AutoVoiLutSelector
                       {
                           IsData = autoLut.IsData,
                           LutExplanation = autoLut.Explanation,
                           LutIndex = autoLut.Index
                       };
        }

        public override IUndoableOperation<IPresentationImage> GetOperation()
        {
            return new BasicImageOperation(GetOriginator, Apply);
        }

        private static IMemorable GetOriginator(IPresentationImage image)
        {
            return !AutoVoiLutApplicator.CanCreate(image) ? null : ((IVoiLutProvider) image).VoiLutManager;
        }

        private void Apply(IPresentationImage image)
        {
            var applicator = AutoVoiLutApplicator.Create(image);
            if (applicator == null)
                return;

            if (IsData)
            {
                if (!String.IsNullOrEmpty(LutExplanation))
                    applicator.ApplyDataLut(LutExplanation);
                else
                    applicator.ApplyDataLut(LutIndex ?? 0); //just apply the first one.
            }
            else
            {
                if (!String.IsNullOrEmpty(LutExplanation))
                    applicator.ApplyLinearLut(LutExplanation);
                else if (LutIndex.HasValue)
                    applicator.ApplyLinearLut(LutIndex ?? 0); //just apply the first one.
            }
        }
    }

    public class LinearPresetVoiLutSelector : VoiLutSelector
    {
        [HpDataContract("{9F548831-7669-4abb-8CFC-A3AB676C09E6}")]
        private class Data
        {
            public string Modality { get; set; }
            public string Name { get; set; }
            public double WindowWidth { get; set; }
            public double WindowCenter { get; set; }
        }

        private readonly Data _dataContract;

        public LinearPresetVoiLutSelector(object dataContract)
            : this((Data)dataContract)
        {
        }

        private LinearPresetVoiLutSelector(string modality, string name, double windowWidth, double windowCenter)
            : this(new Data { Modality = modality, Name = name, WindowWidth = windowWidth, WindowCenter = windowCenter })
        {
        }

        private LinearPresetVoiLutSelector(Data dataContract)
        {
            _dataContract = dataContract;
            AlwaysApply = true;
            UseLatestValues = true;
        }

        public object DataContract
        {
            get
            {
                Update();
                return _dataContract;
            }
        }

        internal string Modality { get { return _dataContract.Modality; } }

        /// <summary>
        /// Specifies whether or not to always apply the LUT, even if it isn't technically
        /// a match for the image (e.g. not the same <see cref="Modality"/>), or the preset no longer exists.
        /// </summary>
        public bool AlwaysApply { get; set; }
        /// <summary>
        /// Specifies whether to automatically update <see cref="WindowWidth"/> and <see cref="WindowCenter"/>
        /// based on the user's current presets.
        /// </summary>
        public bool UseLatestValues { get; set; }

        public string Name { get { return _dataContract.Name; } }
        public double WindowWidth
        {
            get
            {
                Update();
                return _dataContract.WindowWidth;
            }
        }

        public double WindowCenter
        {
            get
            {
                Update();
                return _dataContract.WindowCenter;
            }
        }

        public string Description
        { 
            get
            {
                return String.Format(SR.FormatLinearPresetDescription
                                     , String.IsNullOrEmpty(Modality) ? "?" : Modality
                                     , String.IsNullOrEmpty(Name) ? "?" : Name);
            }
        }

        private void Update()
        {
            if (!UseLatestValues)
                return;

            var selector = CollectionUtils.SelectFirst(GetAllSelectors(),
                           s => s.Modality == _dataContract.Modality && s.Name == _dataContract.Name);

            if (selector == null)
                return;

            var otherContract = selector._dataContract;
            _dataContract.WindowWidth = otherContract.WindowWidth;
            _dataContract.WindowCenter = otherContract.WindowCenter;
        }

        public static LinearPresetVoiLutSelector CreateFrom(IPresentationImage image)
        {
            string modality;
            var lut = GetAppliedLut(image, out modality);
            return lut == null ? null : new LinearPresetVoiLutSelector(modality, lut.Name, lut.WindowWidth, lut.WindowCenter);
        }

        public static List<LinearPresetVoiLutSelector> GetAllSelectors()
        {
            var selectors = new List<LinearPresetVoiLutSelector>();
            var groups = PresetVoiLutSettings.DefaultInstance.GetPresetGroups();
            foreach (var @group in groups)
            {
                foreach (var preset in @group.Presets)
                {
                    var operation = (LinearPresetVoiLutOperationComponent)preset.Operation;
                    selectors.Add(new LinearPresetVoiLutSelector(@group.Modality, @operation.Name, @operation.WindowWidth, @operation.WindowCenter));
                }
            }

            return selectors;
        }
        
        public override IUndoableOperation<IPresentationImage> GetOperation()
        {
            return new BasicImageOperation(GetOriginator, Apply);
        }

        private IPresetVoiLutOperation GetRealOperation(IPresentationImage image)
        {
            var sopProvider = image as IImageSopProvider;
            if (sopProvider == null)
                return null;

            var imageSop = sopProvider.ImageSop;
            var groups = PresetVoiLutSettings.DefaultInstance.GetPresetGroups();
            foreach (var @group in groups)
            {
                if (!AlwaysApply && !group.AppliesTo(imageSop))
                    continue;

                foreach (var preset in @group.Presets)
                {
                    if (Equals(Name, preset.Operation.Name) && preset.Operation.AppliesTo(image))
                        return preset.Operation;
                }
            }

            return !AlwaysApply ? null : new LinearPresetVoiLutOperationComponent
                           {
                               PresetName = Name,
                               WindowWidth = WindowWidth,
                               WindowCenter = WindowCenter
                           };
        }

        private static NamedVoiLutLinear GetAppliedLut(IPresentationImage image, out string modality)
        {
            modality = null;

            var lutProvider = image as IVoiLutProvider;
            if (lutProvider == null)
                return null;

            var sopProvider = image as IImageSopProvider;
            if (sopProvider == null)
                return null;

            var namedLut = lutProvider.VoiLutManager.VoiLut as NamedVoiLutLinear;
            if (namedLut == null)
                return null;

            var imageSop = sopProvider.ImageSop;
            modality = imageSop.Modality;

            var groups = PresetVoiLutSettings.DefaultInstance.GetPresetGroups();
            foreach (var @group in groups)
            {
                if (!group.AppliesTo(imageSop))
                    continue;

                foreach (var preset in @group.Presets)
                {
                    if (!Equals(preset.Operation.Name, namedLut.Name) || !preset.Operation.AppliesTo(image))
                        continue;

                    //This only works for linear presets.
                    var operation = preset.Operation as LinearPresetVoiLutOperationComponent;
                    if (operation == null)
                        continue;

                    if (operation.WindowCenter == namedLut.WindowCenter || operation.WindowWidth == namedLut.WindowWidth)
                        return namedLut;
                }
            }

            return null;
        }

        private IMemorable GetOriginator(IPresentationImage image)
        {
            return GetRealOperation(image) == null ? null : ((IVoiLutProvider)image).VoiLutManager;
        }

        private void Apply(IPresentationImage image)
        {
            var operation = GetRealOperation(image);
            if (operation == null)
                return;

            operation.Apply(image);
        }
    }
}
