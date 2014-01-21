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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class CodeSequenceAnnotationItem : DicomAnnotationItem<string>
	{
		public CodeSequenceAnnotationItem(string identifier, IAnnotationResourceResolver resolver, uint codeSequenceTag, uint? descriptorTag = null, bool showAllValues = true, Func<Frame, IDicomAttributeProvider> dataSourceGetter = null)
			: base(identifier, resolver, new FrameDataRetriever(codeSequenceTag, descriptorTag, showAllValues, dataSourceGetter).RetrieveData, FormatResult) {}

		public CodeSequenceAnnotationItem(string identifier, string displayName, string label, uint codeSequenceTag, uint? descriptorTag = null, bool showAllValues = true, Func<Frame, IDicomAttributeProvider> dataSourceGetter = null)
			: base(identifier, displayName, label, new FrameDataRetriever(codeSequenceTag, descriptorTag, showAllValues, dataSourceGetter).RetrieveData, FormatResult) {}

		private static string FormatResult(string s)
		{
			return s;
		}

		private class FrameDataRetriever
		{
			private readonly Func<Frame, IDicomAttributeProvider> _dataSourceGetter;
			private readonly uint _codeSequenceTag;
			private readonly uint? _descriptorTag;
			private readonly bool _showAllValues;

			public FrameDataRetriever(uint codeSequenceTag, uint? descriptorTag, bool showAllValues, Func<Frame, IDicomAttributeProvider> dataSourceGetter)
			{
				_codeSequenceTag = codeSequenceTag;
				_descriptorTag = descriptorTag;
				_showAllValues = showAllValues;
				_dataSourceGetter = dataSourceGetter;
			}

			public string RetrieveData(Frame f)
			{
				var dataSource = _dataSourceGetter != null ? (_dataSourceGetter.Invoke(f) ?? f) : f;
				return FormatCodeSequence(dataSource, _codeSequenceTag, _descriptorTag, _showAllValues);
			}
		}

		public static string FormatCodeSequence(IDicomAttributeProvider dicomAttributeProvider, uint codeSequenceTag, uint? textDescriptionTag, bool showAllItems = true)
		{
			const int maxItems = 100; // if it's got more than this, there's no way it would have fit onscreen anyway...
			try
			{
				var codeSequence = dicomAttributeProvider[codeSequenceTag] as DicomAttributeSQ;
				var descriptor = textDescriptionTag.HasValue ? dicomAttributeProvider[textDescriptionTag.Value].ToString() : null;

				IList<CodeSequenceMacro> codeItems = null;
				if (codeSequence != null && !codeSequence.IsEmpty && !codeSequence.IsNull && codeSequence.Count > 0)
				{
					if (showAllItems)
						codeItems = Enumerable.Range(0, (int) codeSequence.Count).Select(n => new CodeSequenceMacro(codeSequence[n])).Take(maxItems).ToList();
					else
						codeItems = new[] {new CodeSequenceMacro(codeSequence[0])};
				}

				if (!string.IsNullOrEmpty(descriptor) && (codeItems == null || codeItems.All(x => string.IsNullOrEmpty(x.CodeMeaning))))
					return descriptor;

				return FormatCodeSequence(codeItems);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to parse code sequence attribute at tag ({0:X4},{1:X4})", (codeSequenceTag >> 16) & 0x00FFFF, codeSequenceTag & 0x00FFFF);
				return string.Empty;
			}
		}

		public static string FormatCodeSequence(IEnumerable<CodeSequenceMacro> codeSequenceItems)
		{
			if (codeSequenceItems != null)
			{
				var descriptors = codeSequenceItems.Select(x => x.GetName());
				return string.Join(@"\", descriptors.Where(s => !string.IsNullOrEmpty(s)).ToArray());
			}
			return string.Empty;
		}
	}
}