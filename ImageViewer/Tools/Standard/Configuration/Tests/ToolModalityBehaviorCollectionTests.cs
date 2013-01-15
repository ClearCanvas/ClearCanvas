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

using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration.Tests
{
	[TestFixture]
	public class ToolModalityBehaviorCollectionTests
	{
		private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof (ToolModalityBehaviorCollection));

		public void GenerateDefault()
		{
			var singlePlaneModalities = new[] {@"CR", @"DX", @"MG", @"KO"};
			var collection = new ToolModalityBehaviorCollection();
			foreach (var modality in singlePlaneModalities)
			{
				collection[modality] = new ToolModalityBehavior
				                       	{
				                       		SelectedImageWindowLevelTool = true,
				                       		SelectedImageWindowLevelPresetsTool = true,
				                       		SelectedImageInvertTool = true,
				                       		SelectedImageZoomTool = true,
				                       		SelectedImagePanTool = true,
				                       		SelectedImageFlipTool = true,
				                       		SelectedImageRotateTool = true,
				                       		SelectedImageResetTool = true
				                       	};
			}

			using (var stream = File.Open(typeof (ToolModalityBehaviorCollection).FullName + @".default.xml", FileMode.Create))
			{
				_xmlSerializer.Serialize(stream, collection);
			}
		}

		[Test]
		public void TestDeserialization()
		{
			ToolModalityBehaviorCollection collection;
			using (var stream = new MemoryStream())
			using (var writer = new StreamWriter(stream))
			{
				writer.WriteLine(@"<?xml version=""1.0""?>");
				writer.WriteLine(@"<ToolModalityBehaviorCollection>");
				writer.WriteLine(@"  <UnknownElement Move=""Zig""><xa/></UnknownElement>");
				writer.WriteLine(@"  <UnknownElement/>");
				writer.WriteLine(@"  <!--commentary-->");
				writer.WriteLine(@"  <Entry Modality=""CT"">");
				writer.WriteLine(@"   <!--commentary-->");
				writer.WriteLine(@"   <ToolModalityBehavior>");
				writer.WriteLine(@"     <UnknownProperty/>");
				writer.WriteLine(@"     <UnknownProperty Cats=""Base"">2101</UnknownProperty>");
				writer.WriteLine(@"     <SelectedImageWindowLevelTool>False</SelectedImageWindowLevelTool>");
				writer.WriteLine(@"     <SelectedImageResetTool>false</SelectedImageResetTool>");
				writer.WriteLine(@"     <!--commentary-->");
				writer.WriteLine(@"     <SelectedImageWindowLevelPresetsTool>True</SelectedImageWindowLevelPresetsTool>");
				writer.WriteLine(@"     <UnknownProperty><fdsa /></UnknownProperty>");
				writer.WriteLine(@"     <SelectedImagePanTool>true</SelectedImagePanTool>");
				writer.WriteLine(@"     <SelectedImageFlipTool>True</SelectedImageFlipTool>");
				writer.WriteLine(@"    <UnknownElement></UnknownElement>");
				writer.WriteLine(@"   </ToolModalityBehavior></Entry>");
				writer.WriteLine(@"  <Entry Modality=""DX""><ToolModalityBehavior/></Entry>");
				writer.WriteLine(@"  <Entry Modality=""XA""/>");
				writer.WriteLine(@"</ToolModalityBehaviorCollection>");
				writer.Flush();

				stream.Seek(0, SeekOrigin.Begin);
				collection = _xmlSerializer.Deserialize(stream) as ToolModalityBehaviorCollection;
			}

			Assert.IsNotNull(collection);
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual(true, collection.Contains(@"CT"));
			Assert.AreEqual(true, collection.Contains(@"ct"));
			Assert.AreEqual(true, collection.Contains(@"DX"));
			Assert.AreEqual(false, collection.Contains(@"XA"));
			Assert.AreEqual(false, collection.Contains(@"MG"));

			Assert.AreEqual(true, collection[@"CT"].SelectedImageFlipTool);
			Assert.AreEqual(true, collection[@"CT"].SelectedImagePanTool);
			Assert.AreEqual(true, collection[@"CT"].SelectedImageWindowLevelPresetsTool);
			Assert.AreEqual(false, collection[@"CT"].SelectedImageWindowLevelTool);
			Assert.AreEqual(false, collection[@"CT"].SelectedImageResetTool);
			Assert.AreEqual(false, collection[@"CT"].SelectedImageInvertTool);
		}

		[Test]
		public void TestRoundtripSerialization()
		{
			var collection = new ToolModalityBehaviorCollection();
			collection[@"CT"] = new ToolModalityBehavior {SelectedImageFlipTool = true, SelectedImagePanTool = true, SelectedImageWindowLevelPresetsTool = true};
			collection[@"DX"] = new ToolModalityBehavior {SelectedImageResetTool = true, SelectedImageInvertTool = true, SelectedImageWindowLevelTool = true};
			collection[null] = new ToolModalityBehavior {SelectedImageRotateTool = true, SelectedImageZoomTool = true};

			using (var stream = new MemoryStream())
			{
				_xmlSerializer.Serialize(stream, collection);

				stream.Seek(0, SeekOrigin.Begin);
				collection = _xmlSerializer.Deserialize(stream) as ToolModalityBehaviorCollection;
			}

			Assert.IsNotNull(collection);
			Assert.AreEqual(3, collection.Count);
			Assert.AreEqual(true, collection.Contains(@"CT"));
			Assert.AreEqual(true, collection.Contains(@"ct"));
			Assert.AreEqual(true, collection.Contains(@"DX"));
			Assert.AreEqual(true, collection.Contains(@""));
			Assert.AreEqual(true, collection.Contains(null));
			Assert.AreEqual(false, collection.Contains(@"XA"));

			Assert.AreEqual(true, collection[@"CT"].SelectedImageFlipTool);
			Assert.AreEqual(true, collection[@"CT"].SelectedImagePanTool);
			Assert.AreEqual(true, collection[@"CT"].SelectedImageWindowLevelPresetsTool);
			Assert.AreEqual(false, collection[@"CT"].SelectedImageResetTool);
			Assert.AreEqual(false, collection[@"CT"].SelectedImageInvertTool);

			Assert.AreEqual(true, collection[@"DX"].SelectedImageResetTool);
			Assert.AreEqual(true, collection[@"DX"].SelectedImageInvertTool);
			Assert.AreEqual(true, collection[@"DX"].SelectedImageWindowLevelTool);
			Assert.AreEqual(false, collection[@"DX"].SelectedImagePanTool);
			Assert.AreEqual(false, collection[@"DX"].SelectedImageZoomTool);

			Assert.AreEqual(true, collection[null].SelectedImageRotateTool);
			Assert.AreEqual(true, collection[@""].SelectedImageZoomTool);
			Assert.AreEqual(false, collection[null].SelectedImageWindowLevelPresetsTool);
			Assert.AreEqual(false, collection[@""].SelectedImageWindowLevelTool);
			Assert.AreEqual(false, collection[@""].SelectedImageFlipTool);
		}
	}
}

#endif