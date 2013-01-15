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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Externals.General;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Externals.Tests
{
	[TestFixture]
	public class ExternalCollectionTest
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new UnitTestExtensionFactory
			                             	{
			                             		{typeof (ExternalFactoryExtensionPoint), typeof (CommandLineExternalDefinitionFactory)},
			                             		{typeof (ExternalFactoryExtensionPoint), typeof (MockExternal.ExternalFactory)},
			                             		{typeof (ExternalFactoryExtensionPoint), typeof (MockXmlSerializableExternal.ExternalFactory)},
			                             		{typeof (ExternalFactoryExtensionPoint), typeof (MockBrokenExternal.ExternalFactory)}
			                             	});
		}

		[Test(Description = "Tests XML serialization of null reference")]
		public void TestNullXmlSerialization()
		{
			var xmlData = ExternalCollection.Serialize(null);
			try
			{
				Assert.IsEmpty(xmlData, "Serialization of null should produce empty string output");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test(Description = "Tests XML serialization of empty collection")]
		public void TestEmptyXmlSerialization()
		{
			var xmlData = ExternalCollection.Serialize(new ExternalCollection());
			try
			{
				var xmlDoc = LoadXml(xmlData);
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection", "Serialization of empty collection should produce empty element output");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test(Description = "Tests XML serialization")]
		public void TestXmlSerialization()
		{
			var external1 = new CommandLineExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Command = @"C:\Temp\CommandA.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "", Username = "", Domain = "", AllowMultiValueFields = false};
			var external2 = new CommandLineExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Command = @"\ComputerA\ShareB\CommandC.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "\"$FILENAMEONLY$\"", Username = "\u5305\u9752\u5929", Domain = "\u958B\u5C01\u5E9C", AllowMultiValueFields = true, MultiValueFieldSeparator = "\" \""};
			var external3 = new CommandLineExternal {Name = "external3"};
			var xmlData = ExternalCollection.Serialize(new ExternalCollection {external1, external2, external3});
			try
			{
				var xmlDoc = LoadXml(xmlData);

				AssertXmlNodeValue(typeof (CommandLineExternal).FullName, xmlDoc, "/ExternalCollection/External[1]/@Type", "Type should be namespace qualified type name");
				AssertXmlNodeValue("external1", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Name", "external1");
				AssertXmlNodeValue("Label1", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Label", "external1");
				AssertXmlNodeValue("true", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Enabled", "external1");
				AssertXmlNodeValue("Normal", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/WindowStyle", "external1");
				AssertXmlNodeValue("C:\\Temp\\CommandA.cmd", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Command", "external1");
				AssertXmlNodeValue("$DIRECTORY$", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/WorkingDirectory", "external1");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Arguments", "external1");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Username", "external1");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/Domain", "external1");
				AssertXmlNodeValue("false", xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/AllowMultiValueFields", "external1");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[1]/CommandLineExternal/MultiValueFieldSeparator", "external1");

				AssertXmlNodeValue(typeof (CommandLineExternal).FullName, xmlDoc, "/ExternalCollection/External[2]/@Type", "Type should be namespace qualified type name");
				AssertXmlNodeValue("external2", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Name", "external2");
				AssertXmlNodeValue("Label2", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Label", "external2");
				AssertXmlNodeValue("false", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Enabled", "external2");
				AssertXmlNodeValue("Hidden", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/WindowStyle", "external2");
				AssertXmlNodeValue("\\ComputerA\\ShareB\\CommandC.cmd", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Command", "external2");
				AssertXmlNodeValue("$DIRECTORY$", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/WorkingDirectory", "external2");
				AssertXmlNodeValue("\"$FILENAMEONLY$\"", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Arguments", "external2");
				AssertXmlNodeValue("\u5305\u9752\u5929", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Username", "external2");
				AssertXmlNodeValue("\u958B\u5C01\u5E9C", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/Domain", "external2");
				AssertXmlNodeValue("true", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/AllowMultiValueFields", "external2");
				AssertXmlNodeValue("\" \"", xmlDoc, "/ExternalCollection/External[2]/CommandLineExternal/MultiValueFieldSeparator", "external2");

				AssertXmlNodeValue(typeof (CommandLineExternal).FullName, xmlDoc, "/ExternalCollection/External[3]/@Type", "Type should be namespace qualified type name");
				AssertXmlNodeValue("external3", xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Name", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Label", "external3");
				AssertXmlNodeValue("true", xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Enabled", "external3");
				AssertXmlNodeValue("Normal", xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/WindowStyle", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Command", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/WorkingDirectory", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Arguments", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Username", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/Domain", "external3");
				AssertXmlNodeValue("true", xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/AllowMultiValueFields", "external3");
				AssertXmlNodeEmpty(xmlDoc, "/ExternalCollection/External[3]/CommandLineExternal/MultiValueFieldSeparator", "external3");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test(Description = "Tests that ExternalCollection is capable of serializing IExternal types that do and do not implement IXmlSerializable")]
		public void TestSupportedXmlSerializationModes()
		{
			var external1 = new MockExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Data = "Data1"};
			var external2 = new MockXmlSerializableExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Data = "Data2"};
			Assert.IsFalse(typeof (IXmlSerializable).IsAssignableFrom(external1.GetType()), "One of the two externals should not be IXmlSerializable");
			Assert.IsTrue(typeof (IXmlSerializable).IsAssignableFrom(external2.GetType()), "One of the two externals should be IXmlSerializable");

			var xmlData = ExternalCollection.Serialize(new ExternalCollection {external1, external2});
			try
			{
				var xmlDoc = LoadXml(xmlData);

				AssertXmlNodeValue(typeof (MockExternal).FullName, xmlDoc, "/ExternalCollection/External[1]/@Type", "Type should be namespace qualified type name");
				AssertXmlNodeValue("external1", xmlDoc, "/ExternalCollection/External[1]/*/Name", "external1");
				AssertXmlNodeValue("Label1", xmlDoc, "/ExternalCollection/External[1]/*/Label", "external1");
				AssertXmlNodeValueIgnoreCase("true", xmlDoc, "/ExternalCollection/External[1]/*/Enabled", "external1"); // ignore case - the default XML serializer is case insensitive for bools
				AssertXmlNodeValue("Normal", xmlDoc, "/ExternalCollection/External[1]/*/WindowStyle", "external1");
				AssertXmlNodeValue("Data1", xmlDoc, "/ExternalCollection/External[1]/*/Data", "external1");

				AssertXmlNodeValue(typeof (MockXmlSerializableExternal).FullName, xmlDoc, "/ExternalCollection/External[2]/@Type", "Type should be namespace qualified type name");
				AssertXmlNodeValue("external2", xmlDoc, "/ExternalCollection/External[2]/*/@Name", "external2");
				AssertXmlNodeValue("Label2", xmlDoc, "/ExternalCollection/External[2]/*/@Label", "external2");
				AssertXmlNodeValue("False", xmlDoc, "/ExternalCollection/External[2]/*/@Enabled", "external2");
				AssertXmlNodeValue("Hidden", xmlDoc, "/ExternalCollection/External[2]/*/@WindowStyle", "external2");
				AssertXmlNodeValue("Data2", xmlDoc, "/ExternalCollection/External[2]/*/Data", "external2");
			}
			catch (Exception)
			{
				Trace.WriteLine("XML Data Dump");
				Trace.WriteLine(xmlData);
				throw;
			}
		}

		[Test(Description = "Tests XML deserialization of empty string")]
		public void TestNullXmlDeserialization()
		{
			var collection = ExternalCollection.Deserialize(string.Empty);
			Assert.IsNull(collection, "Deserialization of null should produce null output");
		}

		[Test(Description = "Tests XML deserialization of empty collection from empty element tag")]
		public void TestEmptyElementXmlDeserialization()
		{
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection />";

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization of empty element tag should produce empty collection");
			Assert.AreEqual(0, collection.Count, "Deserialization of empty element tag should produce empty collection");
		}

		[Test(Description = "Tests XML deserialization of empty collection from full element tag with no descendants")]
		public void TestEmptyCollectionXmlDeserialization()
		{
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "</ExternalCollection>";

			var collection2 = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection2, "Deserialization of full element tag with no descendants should produce empty collection");
			Assert.AreEqual(0, collection2.Count, "Deserialization of full element tag with no descendants should produce empty collection");
		}

		[Test(Description = "Tests XML deserialization")]
		public void TestXmlDeserialization()
		{
			// if you encounter "unspecified errors" during compile, reformat the test data - csc has a maximum line/statement length (particularly important during pdb generation)
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal\">\r\n"
			                       + "    <CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "      <Name>external1</Name>\r\n"
			                       + "      <Label>Label1</Label>\r\n"
			                       + "      <Enabled>true</Enabled>\r\n"
			                       + "      <WindowStyle>Normal</WindowStyle>\r\n"
			                       + "      <Command>C:\\Temp\\CommandA.cmd</Command>\r\n"
			                       + "      <WorkingDirectory>$DIRECTORY$</WorkingDirectory>\r\n"
			                       + "      <Arguments />\r\n"
			                       + "      <Username />\r\n"
			                       + "      <Domain />\r\n"
			                       + "      <AllowMultiValueFields>false</AllowMultiValueFields>\r\n"
			                       + "    </CommandLineExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal\">\r\n"
			                       + "    <CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "      <Name>external2</Name>\r\n"
			                       + "      <Label>Label2</Label>\r\n"
			                       + "      <Enabled>false</Enabled>\r\n"
			                       + "      <WindowStyle>Hidden</WindowStyle>\r\n"
			                       + "      <Command>\\ComputerA\\ShareB\\CommandC.cmd</Command>\r\n"
			                       + "      <WorkingDirectory>$DIRECTORY$</WorkingDirectory>\r\n"
			                       + "      <Arguments>\"$FILENAMEONLY$\"</Arguments>\r\n"
			                       + "      <Username>\u5305\u9752\u5929</Username>\r\n"
			                       + "      <Domain>\u958B\u5C01\u5E9C</Domain>\r\n"
			                       + "      <AllowMultiValueFields>true</AllowMultiValueFields>\r\n"
			                       + "      <MultiValueFieldSeparator>\" \"</MultiValueFieldSeparator>\r\n"
			                       + "    </CommandLineExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal\">\r\n"
			                       + "    <CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "      <Name>external3</Name>\r\n"
			                       + "    </CommandLineExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "</ExternalCollection>\r\n";

			var expectedExternal1 = new CommandLineExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Command = @"C:\Temp\CommandA.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "", Username = "", Domain = "", AllowMultiValueFields = false};
			var expectedExternal2 = new CommandLineExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Command = @"\ComputerA\ShareB\CommandC.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "\"$FILENAMEONLY$\"", Username = "\u5305\u9752\u5929", Domain = "\u958B\u5C01\u5E9C", AllowMultiValueFields = true, MultiValueFieldSeparator = "\" \""};
			var expectedExternal3 = new CommandLineExternal {Name = "external3"};

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization returned null");
			Assert.AreEqual(3, collection.Count, "Deserialization returned collection with wrong number of entries");

			var external1 = CollectionUtils.SelectFirst(collection, e => e.Name == "external1");
			Assert.IsNotNull(external1, "Failed to deserialize external1");
            Assert.IsInstanceOf(typeof(CommandLineExternal), external1, "external1: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal1, (CommandLineExternal) external1, "external1");

			var external2 = CollectionUtils.SelectFirst(collection, e => e.Name == "external2");
			Assert.IsNotNull(external2, "Failed to deserialize external2");
            Assert.IsInstanceOf(typeof(CommandLineExternal), external2, "external2: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal2, (CommandLineExternal) external2, "external2");

			var external3 = CollectionUtils.SelectFirst(collection, e => e.Name == "external3");
			Assert.IsNotNull(external3, "Failed to deserialize external3");
            Assert.IsInstanceOf(typeof(CommandLineExternal), external3, "external3: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal3, (CommandLineExternal) external3, "external3");
		}

		[Test(Description = "Tests XML deserialization in legacy XML format")]
		public void TestLegacyXmlDeserialization()
		{
			// if you encounter "unspecified errors" during compile, reformat the test data - csc has a maximum line/statement length (particularly important during pdb generation)
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "  <Name>external1</Name>\r\n"
			                       + "  <Label>Label1</Label>\r\n"
			                       + "  <Enabled>true</Enabled>\r\n"
			                       + "  <WindowStyle>Normal</WindowStyle>\r\n"
			                       + "  <Command>C:\\Temp\\CommandA.cmd</Command>\r\n"
			                       + "  <WorkingDirectory>$DIRECTORY$</WorkingDirectory>\r\n"
			                       + "  <Arguments />\r\n"
			                       + "  <Username />\r\n"
			                       + "  <Domain />\r\n"
			                       + "  <AllowMultiValueFields>false</AllowMultiValueFields>\r\n"
			                       + "</CommandLineExternal>]]></IExternal>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "  <Name>external2</Name>\r\n"
			                       + "  <Label>Label2</Label>\r\n"
			                       + "  <Enabled>false</Enabled>\r\n"
			                       + "  <WindowStyle>Hidden</WindowStyle>\r\n"
			                       + "  <Command>\\ComputerA\\ShareB\\CommandC.cmd</Command>\r\n"
			                       + "  <WorkingDirectory>$DIRECTORY$</WorkingDirectory>"
			                       + "  <Arguments>\"$FILENAMEONLY$\"</Arguments>"
			                       + "  <Username>\u5305\u9752\u5929</Username>"
			                       + "  <Domain>\u958B\u5C01\u5E9C</Domain>"
			                       + "  <AllowMultiValueFields>true</AllowMultiValueFields>\r\n"
			                       + "  <MultiValueFieldSeparator>\" \"</MultiValueFieldSeparator>\r\n"
			                       + "</CommandLineExternal>]]></IExternal>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.General.CommandLineExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<CommandLineExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "  <Name>external3</Name>\r\n"
			                       + "</CommandLineExternal>]]></IExternal>\r\n"
			                       + "</ExternalCollection>";

			var expectedExternal1 = new CommandLineExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Command = @"C:\Temp\CommandA.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "", Username = "", Domain = "", AllowMultiValueFields = false};
			var expectedExternal2 = new CommandLineExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Command = @"\ComputerA\ShareB\CommandC.cmd", WorkingDirectory = "$DIRECTORY$", Arguments = "\"$FILENAMEONLY$\"", Username = "\u5305\u9752\u5929", Domain = "\u958B\u5C01\u5E9C", AllowMultiValueFields = true, MultiValueFieldSeparator = "\" \""};
			var expectedExternal3 = new CommandLineExternal {Name = "external3"};

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization returned null");
			Assert.AreEqual(3, collection.Count, "Deserialization returned collection with wrong number of entries");

			var external1 = CollectionUtils.SelectFirst(collection, e => e.Name == "external1");
			Assert.IsNotNull(external1, "Failed to deserialize external1");
            Assert.IsInstanceOf(typeof(CommandLineExternal), external1, "external1: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal1, (CommandLineExternal) external1, "external1");

			var external2 = CollectionUtils.SelectFirst(collection, e => e.Name == "external2");
			Assert.IsNotNull(external2, "Failed to deserialize external2");
            Assert.IsInstanceOf(typeof(CommandLineExternal), external2, "external2: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal2, (CommandLineExternal) external2, "external2");

			var external3 = CollectionUtils.SelectFirst(collection, e => e.Name == "external3");
			Assert.IsNotNull(external3, "Failed to deserialize external3");
            Assert.IsInstanceOf(typeof(CommandLineExternal), external3, "external3: Wrong concrete implementation of IExternal");
			AssertCommandLineExternal(expectedExternal3, (CommandLineExternal) external3, "external3");
		}

		[Test(Description = "Tests XML deserialization for fault tolerance (recover where possible from bad XML in individual external definitions)")]
		public void TestXmlDeserializationFaultTolerance()
		{
			// if you encounter "unspecified errors" during compile, reformat the test data - csc has a maximum line/statement length (particularly important during pdb generation)
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "  <Junk>\r\n"
			                       + "    <Junk2>\r\n"
			                       + "      <Junk3>\r\n"
			                       + "        <Junk4 />\r\n"
			                       + "      </Junk3>\r\n"
			                       + "    </Junk2>\r\n"
			                       + "  </Junk>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external1\" Label=\"Label1\" Enabled=\"True\" WindowStyle=\"Normal\">\r\n"
			                       + "      <Data>Data1</Data>\r\n"
			                       + "    </MockXmlSerializableExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockBrokenExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "    <badgarbled xml\"\">>>> ]]>\r\n"
			                       + "  </IExternal>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external3\" Label=\"\" Enabled=\"True\" WindowStyle=\"Normal\" />\r\n"
			                       + "  </External>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockBrokenExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\">\r\n"
			                       + "  </IExternal>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external4\" Label=\"\" Enabled=\"True\" WindowStyle=\"Normal\" />\r\n"
			                       + "  </External>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockBrokenExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "    <Junk>\r\n"
			                       + "      <Junk2 />\r\n"
			                       + "    </Junk>]]>\r\n"
			                       + "  </IExternal>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external5\" Label=\"\" Enabled=\"True\" WindowStyle=\"Normal\" />\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockBrokenExternal\">\r\n"
			                       + "    <MockBrokenExternal Name=\"bad1\" Label=\"dfdsfds\" Enabled=\"True\" WindowStyle=\"Normal\">\r\n"
			                       + "    </MockBrokenExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external6\" Label=\"\" Enabled=\"True\" WindowStyle=\"Normal\" />\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockBrokenExternal\" />\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external7\" Label=\"\" Enabled=\"True\" WindowStyle=\"Normal\" />\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockBrokenExternal\">\r\n"
			                       + "    <MockBrokenExternal Name=\"bad1\" Label=\"dfdsfds\" Enabled=\"True\" WindowStyle=\"Normal\" />\r\n"
			                       + "  </External>\r\n"
			                       + "  <IExternal Concrete-Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal, ClearCanvas.ImageViewer.Externals, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"><![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external2\" Label=\"Label2\" Enabled=\"False\" WindowStyle=\"Hidden\">\r\n"
			                       + "      <Data>Data2</Data>\r\n"
			                       + "    </MockXmlSerializableExternal>]]>\r\n"
			                       + "  </IExternal>\r\n"
			                       + "</ExternalCollection>";

			var expectedExternal1 = new MockXmlSerializableExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Data = "Data1"};
			var expectedExternal2 = new MockXmlSerializableExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Data = "Data2"};
			var expectedExternal3 = new MockXmlSerializableExternal {Name = "external3", Label = ""};
			var expectedExternal4 = new MockXmlSerializableExternal {Name = "external4", Label = ""};
			var expectedExternal5 = new MockXmlSerializableExternal {Name = "external5", Label = ""};
			var expectedExternal6 = new MockXmlSerializableExternal {Name = "external6", Label = ""};
			var expectedExternal7 = new MockXmlSerializableExternal {Name = "external7", Label = ""};

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization returned null");
			Assert.AreEqual(7, collection.Count, "Deserialization returned collection with wrong number of entries");

			var external1 = CollectionUtils.SelectFirst(collection, e => e.Name == "external1");
			Assert.IsNotNull(external1, "Failed to deserialize external1");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external1, "external1: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal1, (MockXmlSerializableExternal) external1, "external1");

			var external2 = CollectionUtils.SelectFirst(collection, e => e.Name == "external2");
			Assert.IsNotNull(external2, "Failed to deserialize external2");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external2, "external2: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal2, (MockXmlSerializableExternal) external2, "external2");

			var external3 = CollectionUtils.SelectFirst(collection, e => e.Name == "external3");
			Assert.IsNotNull(external3, "Failed to deserialize external3");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external3, "external3: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal3, (MockXmlSerializableExternal) external3, "external3");

			var external4 = CollectionUtils.SelectFirst(collection, e => e.Name == "external4");
			Assert.IsNotNull(external4, "Failed to deserialize external4");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external4, "external4: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal4, (MockXmlSerializableExternal) external4, "external4");

			var external5 = CollectionUtils.SelectFirst(collection, e => e.Name == "external5");
			Assert.IsNotNull(external5, "Failed to deserialize external5");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external5, "external5: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal5, (MockXmlSerializableExternal) external5, "external5");

			var external6 = CollectionUtils.SelectFirst(collection, e => e.Name == "external6");
			Assert.IsNotNull(external6, "Failed to deserialize external6");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external6, "external6: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal6, (MockXmlSerializableExternal) external6, "external6");

			var external7 = CollectionUtils.SelectFirst(collection, e => e.Name == "external7");
			Assert.IsNotNull(external7, "Failed to deserialize external7");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external7, "external7: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal7, (MockXmlSerializableExternal) external7, "external7");
		}

		[Test(Description = "Tests that ExternalCollection is capable of deserializing IExternal types that do and do not implement IXmlSerializable")]
		public void TestSupportedXmlDeserializationModes()
		{
			const string testXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"
			                       + "<ExternalCollection>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockExternal\">\r\n"
			                       + "    <MockExternal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
			                       + "      <Name>external1</Name>\r\n"
			                       + "      <Label>Label1</Label>\r\n"
			                       + "      <Enabled>true</Enabled>\r\n"
			                       + "      <WindowStyle>Normal</WindowStyle>\r\n"
			                       + "      <Data>Data1</Data>\r\n"
			                       + "    </MockExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "  <External Type=\"ClearCanvas.ImageViewer.Externals.Tests.MockXmlSerializableExternal\">\r\n"
			                       + "    <MockXmlSerializableExternal Name=\"external2\" Label=\"Label2\" Enabled=\"False\" WindowStyle=\"Hidden\">\r\n"
			                       + "      <Data>Data2</Data>\r\n"
			                       + "    </MockXmlSerializableExternal>\r\n"
			                       + "  </External>\r\n"
			                       + "</ExternalCollection>";

			var expectedExternal1 = new MockExternal {Name = "external1", Label = "Label1", Enabled = true, WindowStyle = WindowStyle.Normal, Data = "Data1"};
			var expectedExternal2 = new MockXmlSerializableExternal {Name = "external2", Label = "Label2", Enabled = false, WindowStyle = WindowStyle.Hidden, Data = "Data2"};
			Assert.IsFalse(typeof (IXmlSerializable).IsAssignableFrom(expectedExternal1.GetType()), "One of the two externals should not be IXmlSerializable");
			Assert.IsTrue(typeof (IXmlSerializable).IsAssignableFrom(expectedExternal2.GetType()), "One of the two externals should be IXmlSerializable");

			var collection = ExternalCollection.Deserialize(testXml);
			Assert.IsNotNull(collection, "Deserialization returned null");
			Assert.AreEqual(2, collection.Count, "Deserialization returned collection with wrong number of entries");

			var external1 = CollectionUtils.SelectFirst(collection, e => e.Name == "external1");
			Assert.IsNotNull(external1, "Failed to deserialize external1");
            Assert.IsInstanceOf(typeof(MockExternal), external1, "external1: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal1, (MockExternal) external1, "external1");

			var external2 = CollectionUtils.SelectFirst(collection, e => e.Name == "external2");
			Assert.IsNotNull(external2, "Failed to deserialize external2");
            Assert.IsInstanceOf(typeof(MockXmlSerializableExternal), external2, "external2: Wrong concrete implementation of IExternal");
			AssertMockExternal(expectedExternal2, (MockXmlSerializableExternal) external2, "external2");
		}

		private static void AssertCommandLineExternal(CommandLineExternal expectedExternal, CommandLineExternal actualExternal, string message)
		{
			Assert.AreEqual(expectedExternal.Label, actualExternal.Label, "{0} (Wrong Label)", message);
			Assert.AreEqual(expectedExternal.Enabled, actualExternal.Enabled, "{0} (Wrong Enabled)", message);
			Assert.AreEqual(expectedExternal.WindowStyle, actualExternal.WindowStyle, "{0} (Wrong WindowStyle)", message);
			Assert.AreEqual(expectedExternal.Command, actualExternal.Command, "{0} (Wrong Command)", message);
			Assert.AreEqual(expectedExternal.WorkingDirectory, actualExternal.WorkingDirectory, "{0} (Wrong WorkingDirectory)", message);
			Assert.AreEqual(expectedExternal.Arguments, actualExternal.Arguments, "{0} (Wrong Arguments)", message);
			Assert.AreEqual(expectedExternal.Username, actualExternal.Username, "{0} (Wrong Username)", message);
			Assert.AreEqual(expectedExternal.Domain, actualExternal.Domain, "{0} (Wrong Domain)", message);
			Assert.AreEqual(expectedExternal.AllowMultiValueFields, actualExternal.AllowMultiValueFields, "{0} (Wrong AllowMultiValueFields)", message);
			Assert.AreEqual(expectedExternal.MultiValueFieldSeparator, actualExternal.MultiValueFieldSeparator, "{0} (Wrong MultiValueFieldSeparator)", message);
		}

		private static void AssertMockExternal(IMockExternal expectedExternal, IMockExternal actualExternal, string message)
		{
			Assert.AreEqual(expectedExternal.Label, actualExternal.Label, "{0} (Wrong Label)", message);
			Assert.AreEqual(expectedExternal.Enabled, actualExternal.Enabled, "{0} (Wrong Enabled)", message);
			Assert.AreEqual(expectedExternal.WindowStyle, actualExternal.WindowStyle, "{0} (Wrong WindowStyle)", message);
			Assert.AreEqual(expectedExternal.Data, actualExternal.Data, "{0} (Wrong Data)", message);
		}

		private static void AssertXmlNodeValue(string expected, XmlNode root, string xPath, string message)
		{
			var node = root.SelectSingleNode(xPath);
			Assert.IsNotNull(node, "{0} ({1} Not Found)", message, xPath);
			Assert.AreEqual(expected, node.InnerText, "{0} ({1} Wrong Value)", message, xPath);
		}

		private static void AssertXmlNodeValueIgnoreCase(string expected, XmlNode root, string xPath, string message)
		{
			var node = root.SelectSingleNode(xPath);
			Assert.IsNotNull(node, "{0} ({1} Not Found)", message, xPath);
			var actual = node.InnerText;
			if (actual != null)
				actual = actual.ToLowerInvariant();
			Assert.AreEqual(expected.ToLowerInvariant(), actual, "{0} ({1} Wrong Value)", message, xPath);
		}

		private static void AssertXmlNodeEmpty(XmlNode root, string xPath, string message)
		{
			var node = root.SelectSingleNode(xPath);
			if (node != null && !string.IsNullOrEmpty(node.InnerText))
				Assert.AreEqual(string.Empty, node.InnerText, "{0} ({1} Should Be Empty)", message, xPath);
		}

		private static XmlDocument LoadXml(string xml)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument;
		}
	}
}

#endif