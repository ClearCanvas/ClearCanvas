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
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Tests
{
	[TestFixture]
	public class AnnotationLayoutStoreTests
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test()
		{
			AnnotationLayoutStore.Instance.Clear();
			try
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				IList<StoredAnnotationLayout> layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(0, layouts.Count);

				AnnotationLayoutStore.Instance.Update(CreateLayout("testLayout1"));
				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(1, layouts.Count);

				layouts = new List<StoredAnnotationLayout>();
				layouts.Clear();
				layouts.Add(CreateLayout("testLayout1"));
				layouts.Add(CreateLayout("testLayout2"));
				layouts.Add(CreateLayout("testLayout3"));
				layouts.Add(CreateLayout("testLayout4"));

				AnnotationLayoutStore.Instance.Update(layouts);

				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(4, layouts.Count);

				ResourceResolver resolver = new ResourceResolver(GetType().Assembly);
				using (Stream stream = resolver.OpenResource("AnnotationLayoutStoreDefaults.xml"))
				{
					AnnotationLayoutStoreSettings.Default.LayoutSettingsXml = new XmlDocument();
					AnnotationLayoutStoreSettings.Default.LayoutSettingsXml.Load(stream);
				}

				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				int xmlLayoutCount = layouts.Count;

				StoredAnnotationLayout layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
				layout = CopyLayout(layout, "Dicom.OT.Copied");

				AnnotationLayoutStore.Instance.Update(layout);
				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(xmlLayoutCount + 1, layouts.Count);

				layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT.Copied", annotationItems);
				Assert.IsNotNull(layout);

				AnnotationLayoutStore.Instance.RemoveLayout("Dicom.OT.Copied");
				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(xmlLayoutCount, layouts.Count);

				layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT.Copied", annotationItems);
				Assert.IsNull(layout);

				layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
				Assert.IsNotNull(layout);

				layouts = new List<StoredAnnotationLayout>();
				layouts.Clear();
				layouts.Add(CreateLayout("testLayout1"));
				layouts.Add(CreateLayout("testLayout2"));
				layouts.Add(CreateLayout("testLayout3"));
				layouts.Add(CreateLayout("testLayout4"));

				AnnotationLayoutStore.Instance.Update(layouts);

				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(xmlLayoutCount + 4, layouts.Count);

				AnnotationLayoutStore.Instance.RemoveLayout("testLayout1");
				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(xmlLayoutCount + 3, layouts.Count);

				AnnotationLayoutStore.Instance.RemoveLayout("testLayout2");
				layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
				Assert.AreEqual(xmlLayoutCount + 2, layouts.Count);

				layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
				Assert.IsNotNull(layout);

				layout = AnnotationLayoutStore.Instance.GetLayout("testLayout3", annotationItems);
				Assert.AreEqual(1, layout.AnnotationBoxGroups.Count);

				layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
				layout = CopyLayout(layout, "testLayout3");
				AnnotationLayoutStore.Instance.Update(layout);
				layout = AnnotationLayoutStore.Instance.GetLayout("testLayout3", annotationItems);
				Assert.AreEqual(5, layout.AnnotationBoxGroups.Count);
			}
			finally
			{
				// reset all the changes we made in this test
				AnnotationLayoutStore.Instance.Reset();
			}
		}

		[Test]
		public void TestDefaultLayout()
		{
			var resourceResolver = new ResourceResolver(typeof (AnnotationLayoutStoreTests).Assembly);

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(resourceResolver.OpenResource("AnnotationLayoutStoreDefaults.xml"));

			var layouts = xmlDocument.SelectNodes("/annotation-configuration/annotation-layouts/annotation-layout");
			Assert.IsNotNull(layouts, "/annotation-configuration/annotation-layouts/annotation-layout");

			foreach (var layout in layouts.OfType<XmlElement>())
			{
				var layoutId = layout.GetAttribute("id");
				Assert.IsNotEmpty(layoutId, "/annotation-configuration/annotation-layouts/annotation-layout/@id");

				var groups = layout.SelectNodes("annotation-box-groups/annotation-box-group");
				Assert.IsNotNull(groups, "[{0}]: annotation-box-groups/annotation-box-group", layoutId);

				foreach (var group in groups.OfType<XmlElement>())
				{
					var groupId = group.GetAttribute("id");
					Assert.IsNotEmpty(groupId, "[{0}]: annotation-box-groups/annotation-box-group/@id", layoutId);

					var boxes = group.SelectNodes("annotation-boxes/annotation-box");
					Assert.IsNotNull(boxes, "[{0}/{1}]: annotation-boxes/annotation-box", layoutId, groupId);

					var rectangles = new List<RectangleF>();
					foreach (var box in boxes.OfType<XmlElement>())
					{
						var rectangleString = box.GetAttribute("normalized-rectangle");
						Assert.IsNotEmpty(rectangleString, "[{0}/{1}]: annotation-boxes/annotation-box/@normalized-rectangle", layoutId, groupId);

						var rectangleStringParts = rectangleString.Split('\\');
						Assert.IsTrue(rectangleStringParts.Length == 4, "[{0}/{1}]: '{2}' does not have 4 elements", layoutId, groupId, rectangleString);

						var rectangleComponents = rectangleStringParts.Select(s => AssertParseOrFail(s, "[{0}/{1}]: '{2}' failed to parse as 4 valid values between 0 and 1", layoutId, groupId, rectangleString)).ToArray();

						Assert.IsTrue(rectangleComponents.All(d => d >= 0 && d <= 1), "[{0}/{1}]: '{2}' failed to parse as 4 valid values between 0 and 1", layoutId, groupId, rectangleString);

						var rectangle = RectangleF.FromLTRB(rectangleComponents[0], rectangleComponents[1], rectangleComponents[2], rectangleComponents[3]);

						Assert.IsTrue(rectangle.Width > 0 && rectangle.Height > 0, "[{0}/{1}]: '{2}' should define a rectangle with positive width and height", layoutId, groupId, rectangleString);

						rectangles.Add(rectangle);
					}

					// skip overlap tests for all directional marker groups
					if (groupId == "DirectionalMarkers") continue;

					rectangles.Sort((x, y) => x.Top.CompareTo(y.Top));
					for (var n = 1; n < rectangles.Count; ++n)
					{
						Assert.IsTrue(rectangles[n].Top >= rectangles[n - 1].Bottom, "[{0}/{1}]: Found a vertically overlapping rectangle", layoutId, groupId);
					}
				}
			}
		}

		// [Test]
		internal void ExportDefaultLayoutConfigurationData()
		{
			// this method exports the default layout in EnterpriseServer format

			var configBodyData = new StringBuilder();
			using (var configBodyDataWriter = new StringWriter(configBodyData))
			{
				var configBody = new XmlDocument();
				{
					var sbValue = new StringBuilder();
					using (var writer = new StringWriter(sbValue))
					{
						var resourceResolver = new ResourceResolver(typeof (AnnotationLayoutStoreTests).Assembly);
						var defaultLayout = new XmlDocument();
						defaultLayout.Load(resourceResolver.OpenResource("AnnotationLayoutStoreDefaults.xml"));
						defaultLayout.Save(writer);
					}

					var cdata = configBody.CreateCDataSection(sbValue.ToString());

					var value = configBody.CreateElement("value");
					value.AppendChild(cdata);

					var setting = configBody.CreateElement("setting");
					setting.SetAttribute("name", "LayoutSettingsXml");
					setting.AppendChild(value);

					var settings = configBody.CreateElement("settings");
					settings.AppendChild(setting);

					configBody.AppendChild(settings);
				}
				configBody.Save(configBodyDataWriter);
			}

			var configDocument = new XmlDocument();

			var nameElement = configDocument.CreateElement("Name");
			nameElement.InnerText = typeof (AnnotationLayoutStoreSettings).FullName ?? string.Empty;

			var versionElement = configDocument.CreateElement("Version");
			versionElement.InnerText = string.Format("{0:00000}.{1:00000}", ProductInformation.Version.Major, ProductInformation.Version.Minor);

			var bodyElement = configDocument.CreateElement("Body");
			bodyElement.InnerText = configBodyData.ToString();

			var configItem = configDocument.CreateElement("item");
			configItem.AppendChild(nameElement);
			configItem.AppendChild(versionElement);
			configItem.AppendChild(bodyElement);

			var rootConfigurations = configDocument.CreateElement("Configurations");
			rootConfigurations.SetAttribute("type", "array");
			rootConfigurations.AppendChild(configItem);

			configDocument.AppendChild(rootConfigurations);
			configDocument.Save(string.Format("{0}.xml", typeof (AnnotationLayoutStoreSettings).FullName));
		}

		private static float AssertParseOrFail(string s, string message = null, params object[] args)
		{
			float v;
			if (float.TryParse(s, out v))
				return v;
			Assert.Fail(message, args);

			// ReSharper disable HeuristicUnreachableCode
			return 0;
			// ReSharper restore HeuristicUnreachableCode
		}

		private static StoredAnnotationLayout CopyLayout(StoredAnnotationLayout layout, string newIdentifier)
		{
			StoredAnnotationLayout newLayout = new StoredAnnotationLayout(newIdentifier);
			foreach (StoredAnnotationBoxGroup group in layout.AnnotationBoxGroups)
				newLayout.AnnotationBoxGroups.Add(group);

			return newLayout;
		}

		private static StoredAnnotationLayout CreateLayout(string identifier)
		{
			StoredAnnotationLayout layout = new StoredAnnotationLayout(identifier);
			layout.AnnotationBoxGroups.Add(new StoredAnnotationBoxGroup("group1"));
			layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(
				new AnnotationBox(new RectangleF(0.0F, 0.0F, 0.5F, 0.1F), AnnotationLayoutFactory.GetAnnotationItem("Dicom.GeneralStudy.StudyDescription")));
			layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(
				new AnnotationBox(new RectangleF(0.0F, 0.1F, 0.5F, 0.2F), AnnotationLayoutFactory.GetAnnotationItem("Dicom.GeneralStudy.StudyDescription")));

			return layout;
		}
	}
}

#endif