#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	/// <summary>
	/// <see cref="AnnotationItem"/> for a Contrast/Bolus Agent attribute defined in either the Contrast/Bolus Module as a root level attribute,
	/// or the Enhanced Contrast/Bolus Module as an attribute of the Contrast/Bolus Agent Sequence Items referenced by the frame.
	/// </summary>
	internal class ContrastBolusAgentAnnotationItem : DicomAnnotationItem<string>
	{
		public ContrastBolusAgentAnnotationItem(string identifier, IAnnotationResourceResolver resolver, Func<IDicomAttributeProvider, string> standardModuleResultGetter, Func<DicomSequenceItem[], string> enhancedModuleResultGetter)
			: base(identifier, resolver, new FrameDataRetriever(standardModuleResultGetter, enhancedModuleResultGetter).RetrieveData, FormatResult) {}

		private static string FormatResult(string s)
		{
			return s;
		}

		private static IEnumerable<DicomSequenceItem> EnumerateSequenceItems(DicomAttribute dicomAttribute)
		{
			var dicomAttributeSQ = dicomAttribute as DicomAttributeSQ;
			return dicomAttributeSQ != null && !dicomAttributeSQ.IsNull && !dicomAttributeSQ.IsEmpty ? Enumerable.Range(0, (int) dicomAttributeSQ.Count).Select(n => dicomAttributeSQ[n]) : Enumerable.Empty<DicomSequenceItem>();
		}

		private class FrameDataRetriever
		{
			private readonly Func<IDicomAttributeProvider, string> _standardModuleResultGetter;
			private readonly Func<DicomSequenceItem[], string> _enhancedModuleResultGetter;

			public FrameDataRetriever(Func<IDicomAttributeProvider, string> standardModuleResultGetter, Func<DicomSequenceItem[], string> enhancedModuleResultGetter)
			{
				_enhancedModuleResultGetter = enhancedModuleResultGetter;
				_standardModuleResultGetter = standardModuleResultGetter;
			}

			public string RetrieveData(Frame f)
			{
				// if the image is a multiframe, the C/B Usage Functional Group tells us which items in the C/B Agent Sequence (Enhanced C/B Module) were used
				if (f.ParentImageSop.IsMultiframe)
				{
					DicomAttribute contrastBolusAgentSequence;
					if (f.ParentImageSop.TryGetAttribute(DicomTags.ContrastBolusAgentSequence, out contrastBolusAgentSequence))
					{
						var agentIds = EnumerateSequenceItems(f[DicomTags.ContrastBolusUsageSequence]).Select(x => x[DicomTags.ContrastBolusAgentNumber].GetInt32(0, -1)).ToList();
						var agentsUsed = EnumerateSequenceItems(contrastBolusAgentSequence).Where(x => agentIds.Contains(x[DicomTags.ContrastBolusAgentNumber].GetInt32(0, -1)));
						return _enhancedModuleResultGetter.Invoke(agentsUsed.ToArray());
					}
					return string.Empty;
				}
				return _standardModuleResultGetter.Invoke(f.ParentImageSop);
			}
		}
	}
}