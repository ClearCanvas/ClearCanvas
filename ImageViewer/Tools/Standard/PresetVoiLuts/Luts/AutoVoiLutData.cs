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
using System.Collections.ObjectModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;
using DataLut=ClearCanvas.ImageViewer.Imaging.DataLut;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
    public interface IAutoVoiLut : IVoiLut
	{
        bool IsHeader { get; }
        bool IsData { get; }

        int Index { get; }
        string Explanation { get; }

        bool IsLast { get; }
		void ApplyNext();
	}

	[Cloneable]
	internal class AdjustableAutoVoiDataLut : AdjustableDataLut, IAutoVoiLut
	{
		public AdjustableAutoVoiDataLut(AutoVoiDataLut lut) : base(lut) {}
		protected AdjustableAutoVoiDataLut(AdjustableAutoVoiDataLut source, ICloningContext context) 
			: base(source, context)
		{
		}

        public AutoVoiDataLut AutoVoiDataLut { get { return ((AutoVoiDataLut) base.DataLut); } }

	    public bool IsHeader
	    {
            get { return AutoVoiDataLut.IsHeader; }
	    }

        public bool IsData
        {
            get { return AutoVoiDataLut.IsData; }
        }

        public string Explanation
        {
            get { return AutoVoiDataLut.Explanation; }
        }

	    public int Index
	    {
            get { return AutoVoiDataLut.Index; }    
            set
            {
                AutoVoiDataLut.Index = value;
                Reset();
            }
	    }

        public bool IsLast
        {
            get { return AutoVoiDataLut.IsLast; }
        }
        
        public void ApplyNext()
		{
			AutoVoiDataLut.ApplyNext();
			Reset();
		}
	}

	[Cloneable(true)]
    internal abstract class AutoVoiDataLut : DataVoiLut, IAutoVoiLut
	{
		#region Memento

		private class AutoVoiDataLutMemento : IEquatable<AutoVoiDataLutMemento>
		{
			public readonly int Index;

			public AutoVoiDataLutMemento(int index)
			{
				this.Index = index;
			}

			public override int GetHashCode()
			{
				return this.Index.GetHashCode() ^ 0x589bf89d;
			}

			public override bool Equals(object obj)
			{
				if (obj is AutoVoiDataLutMemento)
					return this.Equals((AutoVoiDataLutMemento) obj);
				return false;
			}

			public bool Equals(AutoVoiDataLutMemento other)
			{
				return other != null && this.Index == other.Index;
			}
		}

		#endregion

		#region Private Fields

		[CloneCopyReference]
		private readonly IList<VoiDataLut> _dataLuts;

		private readonly string _keyPrefix;
		private int _index;

		#endregion

		#region Constructors

		protected AutoVoiDataLut(IList<VoiDataLut> dataLuts, string keyPrefix)
		{
			Platform.CheckForNullReference(dataLuts, "dataLuts");
			Platform.CheckPositive(dataLuts.Count, "dataLuts.Count");
			Platform.CheckForEmptyString(keyPrefix, "keyPrefix");

			_keyPrefix = keyPrefix;
			_dataLuts = new ReadOnlyCollection<VoiDataLut>(dataLuts);
			_index = -1;

			ApplyNext();
		}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		protected AutoVoiDataLut() {}

		#endregion

		#region Public Properties/Methods

        public abstract bool IsHeader { get; }

        public bool IsData { get { return true; } }

        public int Index
        {
            get { return _index; }
            set
            {
                Platform.CheckArgumentRange(value, 0, _dataLuts.Count-1, "index");
                if (Equals(value, _index))
                    return;

                _index = value;
                VoiDataLut lut = _dataLuts[_index];
                base.MinInputValue = lut.FirstMappedPixelValue;
                base.MaxInputValue = lut.LastMappedPixelValue;
                base.MinOutputValue = lut.MinOutputValue;
                base.MaxOutputValue = lut.MaxOutputValue;

                base.OnLutChanged();
            }
        }
        
        public bool IsLast
		{
			get { return _index == _dataLuts.Count - 1; }
		}

		public override int FirstMappedPixelValue
		{
			get { return _dataLuts[_index].FirstMappedPixelValue; }
		}

		public override int LastMappedPixelValue
		{
			get { return _dataLuts[_index].LastMappedPixelValue; }
		}

		public string Explanation
		{
			get { return _dataLuts[_index].Explanation; }
		}

		public override sealed int[] Data
		{
			get { return _dataLuts[_index].Data; }
		}

		public void ApplyNext()
		{
            if (IsLast)
                Index = 0;
		    else
                Index++;
		}

		public override string GetKey()
		{
			return String.Format("{0}:VOIDATA:{1}", _keyPrefix, _index);
		}

		public override string GetDescription()
		{
			string name = Explanation;
			if (String.IsNullOrEmpty(name))
				name = String.Format("{0}{1}", SR.PrefixDefaultVoiDataLutExplanation, _index + 1);

			return String.Format(SR.FormatAutoVoiDataLutDescription, name);
		}

		public override sealed object CreateMemento()
		{
			return new AutoVoiDataLutMemento(this.Index);
		}

		public override sealed void SetMemento(object memento)
		{
			var lutMemento = (AutoVoiDataLutMemento) memento;
			this.Index = lutMemento.Index;
		}

		#endregion
	}

	[Cloneable(true)]
	internal sealed class AutoImageVoiDataLut : AutoVoiDataLut
	{
		private AutoImageVoiDataLut(IList<VoiDataLut> dataLuts, string keyPrefix) : base(dataLuts, keyPrefix) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoImageVoiDataLut() : base() {}

        public override bool IsHeader
        {
            get { return true; }
        }

        public static bool CanCreateFrom(IDicomVoiLutsProvider provider, string lutExplanation)
        {
            return provider != null && null != CollectionUtils.SelectFirst(provider.DicomVoiLuts.ImageVoiDataLuts, lut => lut.Explanation == lutExplanation);
        }

        public static bool CanCreateFrom(IDicomVoiLutsProvider provider, int lutIndex)
        {
            return provider != null && provider.DicomVoiLuts.ImageVoiDataLuts.Count > lutIndex;
        }

        public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
        {
            return CanCreateFrom(provider, 0);
        }

		public static AutoImageVoiDataLut CreateFrom(IDicomVoiLutsProvider provider)
		{
		    return CreateFrom(provider, 0);
		}

		public static AutoImageVoiDataLut CreateFrom(IDicomVoiLutsProvider provider, string lutExplanation)
		{
            var luts = provider.DicomVoiLuts.ImageVoiDataLuts;
            int index;
            for (index = 0; index < luts.Count; ++index)
                if (luts[index].Explanation == lutExplanation) break;

		    return CreateFrom(provider, index);
		}

	    public static AutoImageVoiDataLut CreateFrom(IDicomVoiLutsProvider provider, int lutIndex)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			IList<VoiDataLut> dataLuts;
			if (luts.ImageVoiDataLuts.Count > lutIndex)
				dataLuts = luts.ImageVoiDataLuts;
			else
				return null;

			foreach (VoiDataLut lut in dataLuts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

            string keyPrefix = string.Format("{0}:{1}", luts.ImageSopInstanceUid, luts.ImageSopFrameNumber);
	        return new AutoImageVoiDataLut(dataLuts, keyPrefix) {Index = lutIndex};
		}
	}

	[Cloneable(true)]
	internal sealed class AutoPresentationVoiDataLut : AutoVoiDataLut
	{
		private AutoPresentationVoiDataLut(IList<VoiDataLut> dataLuts, string keyPrefix) : base(dataLuts, keyPrefix) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoPresentationVoiDataLut() : base() {}

        public override bool IsHeader
        {
            get { return false; }
        }

		public override string GetDescription()
		{
			string name = base.Explanation;
			if (String.IsNullOrEmpty(name))
				name = SR.LabelPresentationStateVoiDataLut;

			return String.Format(SR.FormatAutoVoiDataLutDescription, name);
		}

        public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
        {
            return provider != null && provider.DicomVoiLuts.PresentationVoiDataLuts.Count > 0;
        }
        
        public static AutoPresentationVoiDataLut CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			IList<VoiDataLut> dataLuts;
			if (luts.PresentationVoiDataLuts.Count > 0)
				dataLuts = luts.PresentationVoiDataLuts;
			else
				return null;

			foreach (VoiDataLut lut in dataLuts)
				lut.CorrectMinMaxOutput(); //see the comment for this method.

			return new AutoPresentationVoiDataLut(dataLuts, provider.DicomVoiLuts.PresentationStateSopInstanceUid);
		}
	}
}