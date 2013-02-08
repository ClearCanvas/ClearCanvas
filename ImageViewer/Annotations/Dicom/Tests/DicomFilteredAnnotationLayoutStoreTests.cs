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

#if UNIT_TESTS

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Dicom.Tests
{
#pragma warning disable 1591,0419,1574,1587

	[TestFixture]
	public class DicomFilteredAnnotationLayoutStoreTests
	{
		private const int _countLayoutsInDicomFilteredLayoutStoreXml = 13;

		public DicomFilteredAnnotationLayoutStoreTests()
		{ 
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test()
		{
			DicomFilteredAnnotationLayoutStore.Instance.Clear();

			IList<DicomFilteredAnnotationLayout> layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(0, layouts.Count);

			DicomFilteredAnnotationLayoutStore.Instance.Update(CreateLayout("testLayout1", "Dicom.MR", "MR"));
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(1, layouts.Count);

			layouts = new List<DicomFilteredAnnotationLayout>();
			layouts.Clear();
			layouts.Add(CreateLayout("testLayout1", "Dicom.MR", "MR"));
			layouts.Add(CreateLayout("testLayout2", "Dicom.MG", "MG"));
			layouts.Add(CreateLayout("testLayout3", "Dicom.CT", "CT"));
			layouts.Add(CreateLayout("testLayout4", "Dicom.PT", "PT"));

			DicomFilteredAnnotationLayoutStore.Instance.Update(layouts);

			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(4, layouts.Count);

			ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
			using (Stream stream = resolver.OpenResource("DicomFilteredAnnotationLayoutStoreDefaults.xml"))
			{
				StreamReader reader = new StreamReader(stream);
				DicomFilteredAnnotationLayoutStoreSettings.Default.FilteredLayoutSettingsXml = reader.ReadToEnd();
			}

			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(_countLayoutsInDicomFilteredLayoutStoreXml, layouts.Count);

			DicomFilteredAnnotationLayout layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.MR");
			layout = CopyLayout(layout, "Dicom.Filtered.RT");
			layout.Filters[0] = new KeyValuePair<string,string>("Modality", "RT");

			DicomFilteredAnnotationLayoutStore.Instance.Update(layout);
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(_countLayoutsInDicomFilteredLayoutStoreXml + 1, layouts.Count);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.RT");
			Assert.IsNotNull(layout);

			DicomFilteredAnnotationLayoutStore.Instance.RemoveFilteredLayout("Dicom.Filtered.RT");
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(_countLayoutsInDicomFilteredLayoutStoreXml, layouts.Count);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.RT");
			Assert.IsNull(layout);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.MR");
			Assert.IsNotNull(layout);

			layouts = new List<DicomFilteredAnnotationLayout>();
			layouts.Clear();
			layouts.Add(CreateLayout("Dicom.Filtered.RT", "Dicom.RT", "RT"));
			layouts.Add(CreateLayout("Dicom.Filtered.SC", "Dicom.SC", "SC"));
			layouts.Add(CreateLayout("Dicom.Filtered.US", "Dicom.US", "US"));
			layouts.Add(CreateLayout("Dicom.Filtered.ES", "Dicom.ES", "ES"));

			DicomFilteredAnnotationLayoutStore.Instance.Update(layouts);

			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(_countLayoutsInDicomFilteredLayoutStoreXml + 4, layouts.Count);

			DicomFilteredAnnotationLayoutStore.Instance.RemoveFilteredLayout("Dicom.Filtered.RT");
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(_countLayoutsInDicomFilteredLayoutStoreXml + 3, layouts.Count);

			DicomFilteredAnnotationLayoutStore.Instance.RemoveFilteredLayout("Dicom.Filtered.SC");
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(_countLayoutsInDicomFilteredLayoutStoreXml + 2, layouts.Count);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.AllMatch");
			Assert.IsNotNull(layout);
			Assert.AreEqual(0, layout.Filters.Count);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.MR");
			Assert.AreEqual(1, layout.Filters.Count);
		}

		DicomFilteredAnnotationLayout CopyLayout(DicomFilteredAnnotationLayout layout, string newIdentifier)
		{
			DicomFilteredAnnotationLayout newLayout = new DicomFilteredAnnotationLayout(newIdentifier, layout.MatchingLayoutIdentifier);
			foreach (KeyValuePair<string, string> filter in layout.Filters)
				newLayout.Filters.Add(filter);

			return newLayout;
		}

		DicomFilteredAnnotationLayout CreateLayout(string identifier, string matchingLayoutId, string modality)
		{
			DicomFilteredAnnotationLayout newLayout = new DicomFilteredAnnotationLayout(identifier, matchingLayoutId);
			if (modality != "")
				newLayout.Filters.Add(new KeyValuePair<string,string>("Modality", modality));
			
			return newLayout;
		}
	}
}

#endif