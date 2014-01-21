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

using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public interface IModalityDisplaySetCreationOptions
	{
		string Modality { get; }

		bool CreateAllImagesDisplaySet { get; }
		bool CreateSingleImageDisplaySets { get; }
		bool ShowOriginalSeries { get; }

		bool SplitMultiEchoSeries { get; }
		bool ShowOriginalMultiEchoSeries { get; }

		bool SplitMultiStackSeries { get; }
		bool ShowOriginalMultiStackSeries { get; }

		bool SplitMixedMultiframes { get; }
		bool ShowOriginalMixedMultiframeSeries { get; }

		bool ShowGrayscaleInverted { get; }
	}

	public interface IDisplaySetCreationOptions : IEnumerable<IModalityDisplaySetCreationOptions>
	{
		IModalityDisplaySetCreationOptions this[string modality] { get; set; }
	}

	public class DisplaySetCreationOptions : IDisplaySetCreationOptions
	{
		private readonly SortedDictionary<string, IModalityDisplaySetCreationOptions> _options;

		public DisplaySetCreationOptions()
		{
			_options = new SortedDictionary<string, IModalityDisplaySetCreationOptions>();
			foreach (var storedSetting in DisplaySetCreationSettings.DefaultInstance.GetStoredSettings())
				_options[storedSetting.Modality] = storedSetting;
		}

		public IModalityDisplaySetCreationOptions this[string modality]
		{
			get
			{
				IModalityDisplaySetCreationOptions value;
				return _options.TryGetValue(modality, out value) ? value : null;
			}
			set { _options[modality] = value; }
		}

		#region IEnumerable<IModalityDisplaySetCreationOptions> Members

		public IEnumerator<IModalityDisplaySetCreationOptions> GetEnumerator()
		{
			return _options.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}