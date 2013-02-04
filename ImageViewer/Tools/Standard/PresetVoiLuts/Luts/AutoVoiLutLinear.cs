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
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	[Cloneable(true)]
    internal abstract class AutoVoiLutLinear : CalculatedVoiLutLinear, IAutoVoiLut
	{
		#region Memento

		private class AutoVoiLutLinearMemento : IEquatable<AutoVoiLutLinearMemento>
		{
			public readonly int Index;

			public AutoVoiLutLinearMemento(int index)
			{
				this.Index = index;
			}

			public override int GetHashCode()
			{
				return this.Index.GetHashCode() ^ 0x09bf0923;
			}

			public override bool Equals(object obj)
			{
				if (obj is AutoVoiLutLinearMemento)
					return this.Equals((AutoVoiLutLinearMemento) obj);
				return false;
			}

			public bool Equals(AutoVoiLutLinearMemento other)
			{
				return other != null && this.Index == other.Index;
			}
		}

		#endregion

		#region Private Fields

		[CloneCopyReference]
		private readonly IList<VoiWindow> _windows;
		private int _index;

		#endregion

		#region Constructors

		protected AutoVoiLutLinear(IList<VoiWindow> windows)
		{
			_windows = new ReadOnlyCollection<VoiWindow>(windows);
			_index = 0;
		}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		protected AutoVoiLutLinear() {}

		#endregion

        #region Public Properties/Methods

        public abstract bool IsHeader { get; }

        public bool IsData { get { return false; } }

        public int Index
        {
            get { return _index; }
            set
            {
                Platform.CheckArgumentRange(value, 0, _windows.Count - 1, "index");
                if (Equals(value, _index))
                    return;

                _index = value;
                base.OnLutChanged();
            }
        }

		public override sealed double WindowWidth
		{
			get { return _windows[_index].Width; }
		}

		public override sealed double WindowCenter
		{
			get { return _windows[_index].Center; }
		}

		public string Explanation
		{
			get { return _windows[_index].Explanation; }	
		}

		public bool IsLast
		{
			get { return _index == _windows.Count - 1; }
		}

		public void ApplyNext()
		{
            if (IsLast)
                Index = 0;
		    else
                Index++;
		}

		public override string GetDescription()
		{
			var message = string.IsNullOrEmpty(Explanation) ? SR.FormatDescriptionAutoLinearLutNoExplanation : SR.FormatDescriptionAutoLinearLut;
			return String.Format(message, WindowWidth, WindowCenter, Explanation);
		}

		public override sealed object CreateMemento()
		{
			return new AutoVoiLutLinearMemento(Index);
		}

		public override sealed void SetMemento(object memento)
		{
			var autoMemento = (AutoVoiLutLinearMemento) memento;
			Index = autoMemento.Index;
		}

		#endregion
	}

	[Cloneable(true)]
    internal sealed class AutoImageVoiLutLinear : AutoVoiLutLinear
	{
		private AutoImageVoiLutLinear(IList<VoiWindow> windows) : base(windows) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoImageVoiLutLinear() : base() {}

        public override bool IsHeader
        {
            get { return true; }
        }

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider, string lutExplanation)
		{
		    return provider != null 
                && null != CollectionUtils.SelectFirst(provider.DicomVoiLuts.ImageVoiLinearLuts,lut => lut.Explanation == lutExplanation);
        }

        public static bool CanCreateFrom(IDicomVoiLutsProvider provider, int lutIndex)
		{
			return provider != null && provider.DicomVoiLuts.ImageVoiLinearLuts.Count > lutIndex;
        }

        public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
    	{
            return CanCreateFrom(provider, 0);
		}

		public static AutoImageVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider, string lutExplanation)
		{
			var luts = provider.DicomVoiLuts.ImageVoiLinearLuts;
		    int index;
            for(index = 0; index < luts.Count; ++index)
                if (luts[index].Explanation == lutExplanation)break;

		    if (index < luts.Count)
				return new AutoImageVoiLutLinear(luts) {Index = index};
			return null;
		}

        public static AutoImageVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider, int lutIndex)
		{
			var luts = provider.DicomVoiLuts.ImageVoiLinearLuts;
			if (luts.Count > lutIndex)
				return new AutoImageVoiLutLinear(luts) {Index = lutIndex};
			return null;
		}

    	public static AutoImageVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider)
		{
    	    return CreateFrom(provider, 0);
		}
    }

	[Cloneable(true)]
    internal sealed class AutoPresentationVoiLutLinear : AutoVoiLutLinear
	{
		private AutoPresentationVoiLutLinear(IList<VoiWindow> windows) : base(windows) {}

		/// <summary>
		/// Cloning constructor
		/// </summary>
		private AutoPresentationVoiLutLinear() : base() {}

        public override bool IsHeader
        {
            get { return false; }
        }

		public override string GetDescription()
		{
			var explanation = string.IsNullOrEmpty(Explanation) ? SR.LabelPresentationStateVoiLinearLut : Explanation;
			return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter, explanation);
		}

		public static bool CanCreateFrom(IDicomVoiLutsProvider provider)
		{
			return provider != null && provider.DicomVoiLuts.PresentationVoiLinearLuts.Count > 0;
		}

        public static AutoPresentationVoiLutLinear CreateFrom(IDicomVoiLutsProvider provider)
		{
			IDicomVoiLuts luts = provider.DicomVoiLuts;
			if (luts.PresentationVoiLinearLuts.Count > 0)
				return new AutoPresentationVoiLutLinear(luts.PresentationVoiLinearLuts);
			return null;
		}
	}
}