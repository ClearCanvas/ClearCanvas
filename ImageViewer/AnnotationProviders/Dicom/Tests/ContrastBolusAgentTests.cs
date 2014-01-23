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

#if UNIT_TESTS

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.FunctionalGroups;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Tests
{
	[TestFixture]
	internal class ContrastBolusAgentTests
	{
		private static readonly IDictionary<string, IAnnotationItem> _annotationItems = CreateAnnotationItems();
		private const string _cbAgent = "Dicom.ContrastBolus.Agent";

		[Test]
		public void TestNil()
		{
			var dataset = TestDataSource.CreateImageSopDataSource();

			using (var sop = (ImageSop) Sop.Create(dataset))
			using (var images = new DisposableList<IPresentationImage>(PresentationImageFactory.Create(sop)))
			{
				Assert.IsEmpty(_annotationItems[_cbAgent].GetAnnotationText(images[0]));
			}
		}

		[Test]
		public void TestAgentDescriptorOnly()
		{
			var dataset = TestDataSource.CreateImageSopDataSource();
			dataset[DicomTags.ContrastBolusAgent].SetStringValue(@"Contrastinol and Bolusinate");

			using (var sop = (ImageSop) Sop.Create(dataset))
			using (var images = new DisposableList<IPresentationImage>(PresentationImageFactory.Create(sop)))
			{
				Assert.AreEqual(@"Contrastinol and Bolusinate", _annotationItems[_cbAgent].GetAnnotationText(images[0]));
			}
		}

		[Test]
		public void TestAgentCodeSequence()
		{
			var agent1 = new CodeSequenceMacro {CodeMeaning = "Contrastinol", CodeValue = "123", CodingSchemeDesignator = "ABC"};
			var agent2 = new CodeSequenceMacro {CodeMeaning = "Bolusinate", CodeValue = "456", CodingSchemeDesignator = "DEF"};

			var dataset = TestDataSource.CreateImageSopDataSource();
			dataset[DicomTags.ContrastBolusAgent].SetStringValue(@"Contrastinol and Bolusinate");
			dataset[DicomTags.ContrastBolusAgentSequence].AddSequenceItem(agent1.DicomSequenceItem);

			using (var sop = (ImageSop) Sop.Create(dataset))
			using (var images = new DisposableList<IPresentationImage>(PresentationImageFactory.Create(sop)))
			{
				Assert.AreEqual(@"Contrastinol", _annotationItems[_cbAgent].GetAnnotationText(images[0]));

				dataset[DicomTags.ContrastBolusAgentSequence].AddSequenceItem(agent2.DicomSequenceItem);
				Assert.AreEqual(@"Contrastinol\Bolusinate", _annotationItems[_cbAgent].GetAnnotationText(images[0]));

				agent1.CodeMeaning = string.Empty;
				Assert.AreEqual(@"123 (ABC)\Bolusinate", _annotationItems[_cbAgent].GetAnnotationText(images[0]));

				agent2.CodeMeaning = string.Empty;
				Assert.AreEqual(@"Contrastinol and Bolusinate", _annotationItems[_cbAgent].GetAnnotationText(images[0]));

				dataset[DicomTags.ContrastBolusAgent].SetNullValue();
				Assert.AreEqual(@"123 (ABC)\456 (DEF)", _annotationItems[_cbAgent].GetAnnotationText(images[0]));
			}
		}

		[Test]
		public void TestEnhancedAgentCodeSequence()
		{
			var agent1 = new CodeSequenceMacro {CodeMeaning = "Contrastinol", CodeValue = "123", CodingSchemeDesignator = "ABC"};
			var agent2 = new CodeSequenceMacro {CodeMeaning = "Bolusinate", CodeValue = "456", CodingSchemeDesignator = "DEF"};
			var agent3 = new CodeSequenceMacro {CodeMeaning = "Dilithium", CodeValue = "789", CodingSchemeDesignator = "GHI"};

			var usageFrame1 = new ContrastBolusUsageFunctionalGroup
			                  	{
			                  		ContrastBolusUsageSequence = new[]
			                  		                             	{
			                  		                             		new ContrastBolusUsageSequenceItem {ContrastBolusAgentNumber = 1},
			                  		                             		new ContrastBolusUsageSequenceItem {ContrastBolusAgentNumber = 3}
			                  		                             	}
			                  	};
			var usageFrame2 = new ContrastBolusUsageFunctionalGroup
			                  	{
			                  		ContrastBolusUsageSequence = new[]
			                  		                             	{
			                  		                             		new ContrastBolusUsageSequenceItem {ContrastBolusAgentNumber = 2}
			                  		                             	}
			                  	};
			var usageFrame3 = new ContrastBolusUsageFunctionalGroup(new DicomSequenceItem())
			                  	{
			                  		ContrastBolusUsageSequence = new[]
			                  		                             	{
			                  		                             		new ContrastBolusUsageSequenceItem {ContrastBolusAgentNumber = 999},
			                  		                             		new ContrastBolusUsageSequenceItem {ContrastBolusAgentNumber = 2}
			                  		                             	}
			                  	};
			var usageFrame4 = new ContrastBolusUsageFunctionalGroup(new DicomSequenceItem());

			var dataset = TestDataSource.CreateImageSopDataSource(4);
			dataset[DicomTags.ContrastBolusAgent].SetStringValue(@"Contrastinol and Bolusinate");
			dataset[DicomTags.ContrastBolusAgentSequence].AddSequenceItem(agent1.DicomSequenceItem);
			dataset[DicomTags.ContrastBolusAgentSequence].AddSequenceItem(agent2.DicomSequenceItem);
			dataset[DicomTags.ContrastBolusAgentSequence].AddSequenceItem(agent3.DicomSequenceItem);
			dataset[DicomTags.PerFrameFunctionalGroupsSequence].AddSequenceItem(usageFrame1.DicomSequenceItem);
			dataset[DicomTags.PerFrameFunctionalGroupsSequence].AddSequenceItem(usageFrame2.DicomSequenceItem);
			dataset[DicomTags.PerFrameFunctionalGroupsSequence].AddSequenceItem(usageFrame3.DicomSequenceItem);
			dataset[DicomTags.PerFrameFunctionalGroupsSequence].AddSequenceItem(usageFrame4.DicomSequenceItem);

			agent1.DicomSequenceItem[DicomTags.ContrastBolusAgentNumber].SetInt32(0, 1);
			agent2.DicomSequenceItem[DicomTags.ContrastBolusAgentNumber].SetInt32(0, 2);
			agent3.DicomSequenceItem[DicomTags.ContrastBolusAgentNumber].SetInt32(0, 3);

			using (var sop = (ImageSop) Sop.Create(dataset))
			using (var images = new DisposableList<IPresentationImage>(PresentationImageFactory.Create(sop)))
			{
				Assert.AreEqual(@"Contrastinol\Dilithium", _annotationItems[_cbAgent].GetAnnotationText(images[0]), "Frame 1");
				Assert.AreEqual(@"Bolusinate", _annotationItems[_cbAgent].GetAnnotationText(images[1]), "Frame 2");
				Assert.AreEqual(@"Bolusinate", _annotationItems[_cbAgent].GetAnnotationText(images[2]), "Frame 3");
				Assert.IsEmpty(_annotationItems[_cbAgent].GetAnnotationText(images[3]), "Frame 4");

				agent1.CodeMeaning = string.Empty;
				agent2.CodeMeaning = string.Empty;
				Assert.AreEqual(@"123 (ABC)\Dilithium", _annotationItems[_cbAgent].GetAnnotationText(images[0]), "Frame 1");
				Assert.AreEqual(@"456 (DEF)", _annotationItems[_cbAgent].GetAnnotationText(images[1]), "Frame 2");
				Assert.AreEqual(@"456 (DEF)", _annotationItems[_cbAgent].GetAnnotationText(images[2]), "Frame 3");
				Assert.IsEmpty(_annotationItems[_cbAgent].GetAnnotationText(images[3]), "Frame 4");

				agent3.CodeMeaning = string.Empty;
				Assert.AreEqual(@"123 (ABC)\789 (GHI)", _annotationItems[_cbAgent].GetAnnotationText(images[0]));
				Assert.AreEqual(@"456 (DEF)", _annotationItems[_cbAgent].GetAnnotationText(images[1]), "Frame 2");
				Assert.AreEqual(@"456 (DEF)", _annotationItems[_cbAgent].GetAnnotationText(images[2]), "Frame 3");
				Assert.IsEmpty(_annotationItems[_cbAgent].GetAnnotationText(images[3]), "Frame 4");
			}
		}

		private static IDictionary<string, IAnnotationItem> CreateAnnotationItems()
		{
			return new ContrastBolusAnnotationItemProvider().GetAnnotationItems().ToDictionary(x => x.GetIdentifier());
		}
	}
}

#endif