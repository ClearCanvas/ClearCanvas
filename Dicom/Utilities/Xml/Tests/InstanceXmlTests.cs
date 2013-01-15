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

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Xml.Tests
{
	[TestFixture]
	public class InstanceXmlTests
	{
		[Test]
		public void TestXmlEntityBasicEscaping()
		{
			Assert.AreEqual(string.Empty, InstanceXml.TestXmlEscapeString(null));
			Assert.AreEqual(string.Empty, InstanceXml.TestXmlEscapeString(string.Empty));
			Assert.AreEqual("the 123,4567,890 jumpy foxes lazed over the quick brown dog", InstanceXml.TestXmlEscapeString("the 123,4567,890 jumpy foxes lazed over the quick brown dog"));
			Assert.AreEqual("&amp;amp;&quot;&lt;&apos;&gt;", InstanceXml.TestXmlEscapeString("&amp;\"<'>"));
			Assert.AreEqual("&lt;a href=&quot;http://garba/&quot;&gt;tom&amp;jerry&apos;s&lt;/a&gt;", InstanceXml.TestXmlEscapeString("<a href=\"http://garba/\">tom&jerry's</a>"));
		}

		[Test]
		public void TestXmlEntityBasicUnescaping()
		{
			Assert.AreEqual(string.Empty, InstanceXml.TestXmlUnescapeString(null));
			Assert.AreEqual(string.Empty, InstanceXml.TestXmlUnescapeString(string.Empty));
			Assert.AreEqual("the 123,4567,890 jumpy foxes lazed over the quick brown dog", InstanceXml.TestXmlUnescapeString("the 123,4567,890 jumpy foxes lazed over the quick brown dog"));
			Assert.AreEqual("&amp;\"<'>", InstanceXml.TestXmlUnescapeString("&amp;amp;&quot;&lt;&apos;&gt;"));
			Assert.AreEqual("<a href=\"http://garba/\">tom&jerry's</a>", InstanceXml.TestXmlUnescapeString("&lt;a href=&quot;http://garba/&quot;&gt;tom&amp;jerry&apos;s&lt;/a&gt;"));
		}

		[Test]
		public void TestXmlEntityUnescapingSpecialFormattedEntities()
		{
			// these all say the same thing
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#27;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#x1B;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#X1B;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#x1b;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#X1B;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#x01B;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#x001B;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#x0001B;"));
			Assert.AreEqual("\u001B", InstanceXml.TestXmlUnescapeString("&#x00001B;"));
		}

		[Test]
		public void TestXmlEntityBasicRoundtripEscaping()
		{
			Trace.WriteLine("Testing x == Unescape(Escape(x)) (basic input)");
			ValidateRoundtripEscapeUnescape(string.Empty);
			ValidateRoundtripEscapeUnescape("the 123,4567,890 jumpy foxes lazed over the quick brown dog");
			ValidateRoundtripEscapeUnescape("&amp;\"<'>");
			ValidateRoundtripEscapeUnescape("<a href=\"http://garba/\">tom&jerry's</a>");
			ValidateRoundtripEscapeUnescape("&lt;a href=&quot;http://garba/&quot;&gt;tom&amp;jerry&apos;s&lt;/a&gt;");
			ValidateRoundtripEscapeUnescape("<a href=\"http://garba/\">&lt;a href=&quot;http://garba/&quot;&gt;tom&amp;jerry&apos;s&lt;/a&gt;</a>");
		}

		[Test]
		public void TestXmlEntityComplexRoundtripEscaping()
		{
			Trace.WriteLine("Testing x == Unescape(Escape(x)) (complex input)");
			Trace.WriteLine(" * US-ASCII Characters (0-127)");
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(0, 128));
			Trace.WriteLine(" * Extended ASCII Characters (128-255)");
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(128, 256));
			Trace.WriteLine(" * Unicode Basic Multilingual Plane");
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(256, 0x4000));
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(0x4000, 0x8000));
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(0x8000, 0xC000));
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(0xC000, 0xD800));
			ValidateRoundtripEscapeUnescape(GenerateBinaryString(0xE000, 0x10000));
			Trace.WriteLine(" * Unicode UTF-16 Surrogate Pairs");
			ValidateRoundtripEscapeUnescape(GenerateSurrogatesBinaryString());
		}

		[Test]
		public void TestXmlEntityBasicRoundtripUnescaping()
		{
			Trace.WriteLine("Testing x == Escape(Unescape(x)) (basic input)");
			ValidateRoundtripUnescapeEscape(string.Empty);
			ValidateRoundtripUnescapeEscape("&amp;amp;&quot;&lt;&apos;&gt;");
			ValidateRoundtripUnescapeEscape("&lt;a href=&quot;http://garba/&quot;&gt;tom&amp;jerry&apos;s&lt;/a&gt;");
		}

		[Test]
		public void TestXmlEntityComplexRoundtripUnescaping()
		{
			Trace.WriteLine("Testing x == Escape(Unescape(x)) (complex input)");
			Trace.WriteLine(" * US-ASCII Characters (0-127)");
			ValidateRoundtripUnescapeEscape(GenerateEntityString(0, 128));
			Trace.WriteLine(" * Extended ASCII Characters (128-255)");
			ValidateRoundtripUnescapeEscape(GenerateEntityString(128, 256));
			Trace.WriteLine(" * Unicode Basic Multilingual Plane");
			ValidateRoundtripUnescapeEscape(GenerateEntityString(256, 0x4000));
			ValidateRoundtripUnescapeEscape(GenerateEntityString(0x4000, 0x8000));
			ValidateRoundtripUnescapeEscape(GenerateEntityString(0x8000, 0xC000));
			ValidateRoundtripUnescapeEscape(GenerateEntityString(0xC000, 0xD800));
			ValidateRoundtripUnescapeEscape(GenerateEntityString(0xE000, 0x10000));
			Trace.WriteLine(" * Unicode UTF-16 Surrogate Pairs");
			ValidateRoundtripUnescapeEscape(GenerateSurrogatesEntityString());
		}

		[Test]
		public void TestValidXmlOutput()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				// generate the dataset
				DicomAttributeCollection dataset = new DicomAttributeCollection();
				{
					Trace.WriteLine("Generating dataset using Unicode data");
					Trace.WriteLine(" * US-ASCII Characters (0-127)");
					// the line feed characters /r and /n are deliberately excluded because
					// automatic line end handling would otherwise mangle them in the DicomAttribute
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0, 0xA)));
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0xB, 0xD)));
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0xE, 128)));
					Trace.WriteLine(" * Extended ASCII Characters (128-255)");
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(128, 256)));
					Trace.WriteLine(" * Unicode Basic Multilingual Plane");
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(256, 0x4000)));
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0x4000, 0x8000)));
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0x8000, 0xC000)));
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0xC000, 0xD800)));
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateBinaryString(0xE000, 0x10000)));
					Trace.WriteLine(" * Unicode UTF-16 Surrogate Pairs");
					dataset[DicomTags.FailedAttributesSequence].AddSequenceItem(CreateSequenceItem(GenerateSurrogatesBinaryString()));
				}

				// generate the InstanceXml document and dump into the MemoryStream
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
					InstanceXml instanceXml = new InstanceXml(dataset, SopClass.RawDataStorage, TransferSyntax.ExplicitVrLittleEndian);
					Assert.IsTrue(instanceXml[DicomTags.FailedAttributesSequence].Count > 0);
					XmlElement xmlRoot = xmlDocument.CreateElement("test");
					xmlDocument.AppendChild(xmlRoot);
					XmlElement xmlElement = instanceXml.GetMemento(xmlDocument, new StudyXmlOutputSettings());
					xmlRoot.AppendChild(xmlElement);
					XmlWriter xmlWriter = XmlWriter.Create(ms);
					xmlDocument.WriteTo(xmlWriter);
					xmlWriter.Close();
					Assert.IsTrue(ms.Length > 0);
					Trace.WriteLine(string.Format("XML fragment length: {0}", ms.Length));
				}

				// write the xml to a file
				ms.Seek(0, SeekOrigin.Begin);
				using (FileStream fs = File.OpenWrite("InstanceXmlTests.TestValidXmlOutput.Result.xml"))
				{
					ms.WriteTo(fs);
					fs.Close();
				}

				// parse and validate the xml using .NET
				ms.Seek(0, SeekOrigin.Begin);
				{
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.CheckCharacters = true;
					xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
					XmlReader xmlReader = XmlReader.Create(ms, xmlReaderSettings);
					while (xmlReader.Read()) {}
					xmlReader.Close();
				}

				// read it back as an InstanceXml
				ms.Seek(0, SeekOrigin.Begin);
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(ms);
					InstanceXml instanceXml = new InstanceXml(xmlDocument.GetElementsByTagName("test")[0].FirstChild, new DicomAttributeCollection());
					Assert.IsTrue(instanceXml[DicomTags.FailedAttributesSequence].Count > 0);

					int i = 0;
					Trace.WriteLine("Validating decoded dataset for correct Unicode data");
					Trace.WriteLine(" * US-ASCII Characters (0-127)");
					// the line feed characters /r and /n are deliberately excluded because
					// automatic line end handling would otherwise mangle them in the DicomAttribute
					Assert.AreEqual(GenerateBinaryString(0, 0xA), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Assert.AreEqual(GenerateBinaryString(0xB, 0xD), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Assert.AreEqual(GenerateBinaryString(0xE, 128), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Trace.WriteLine(" * Extended ASCII Characters (128-255)");
					Assert.AreEqual(GenerateBinaryString(128, 256), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Trace.WriteLine(" * Unicode Basic Multilingual Plane");
					Assert.AreEqual(GenerateBinaryString(256, 0x4000), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Assert.AreEqual(GenerateBinaryString(0x4000, 0x8000), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Assert.AreEqual(GenerateBinaryString(0x8000, 0xC000), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Assert.AreEqual(GenerateBinaryString(0xC000, 0xD800), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Assert.AreEqual(GenerateBinaryString(0xE000, 0x10000), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
					Trace.WriteLine(" * Unicode UTF-16 Surrogate Pairs");
					Assert.AreEqual(GenerateSurrogatesBinaryString(), ReadSequenceItem(instanceXml[DicomTags.FailedAttributesSequence], i++));
				}

				ms.Close();
			}
		}

		private static DicomSequenceItem CreateSequenceItem(string unlimitedTextData)
		{
			DicomSequenceItem sqItem = new DicomSequenceItem();
			sqItem[DicomTags.TextValue].SetStringValue(unlimitedTextData);
			return sqItem;
		}

		private static string ReadSequenceItem(DicomAttribute attribute, int index)
		{
			if (attribute is DicomAttributeSQ)
				return (((DicomAttributeSQ) attribute)[index])[DicomTags.TextValue].ToString();
			return null;
		}

		private static string GenerateBinaryString(int start, int stop)
		{
			StringBuilder sb = new StringBuilder();
			for (int n = start; n < stop; n++)
				sb.Append((char) n);
			return sb.ToString();
		}

		private static string GenerateSurrogatesBinaryString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 0x0400; i++)
			{
				int lWord = 0xD800 + i;
				int uWord = 0xDC00 + i;
				sb.Append(GenerateBinaryString(lWord, lWord + 1));
				sb.Append(GenerateBinaryString(uWord, uWord + 1));
			}
			return sb.ToString();
		}

		private static string GenerateEntityString(int start, int stop)
		{
			StringBuilder sb = new StringBuilder();
			for (int n = start; n < stop; n++)
			{
				// \x9\xA\xD\x20-\xFFFD
				if (n == '"')
					sb.Append("&quot;");
				else if (n == '>')
					sb.Append("&gt;");
				else if (n == '<')
					sb.Append("&lt;");
				else if (n == '\'')
					sb.Append("&apos;");
				else if (n == '&')
					sb.Append("&amp;");
				else if (n == 0x9 || n == 0xA || n == 0xD || (n >= 0x20 && n <= 0xFFFD))
					sb.Append((char) n);
				else
					sb.AppendFormat("&#x{0};", n.ToString("X"));
			}
			return sb.ToString();
		}

		private static string GenerateSurrogatesEntityString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 0x0400; i++)
			{
				int lWord = 0xD800 + i;
				int uWord = 0xDC00 + i;
				sb.Append(GenerateEntityString(lWord, lWord + 1));
				sb.Append(GenerateEntityString(uWord, uWord + 1));
			}
			return sb.ToString();
		}

		private static void ValidateRoundtripEscapeUnescape(string data)
		{
			string escaped = InstanceXml.TestXmlEscapeString(data);
			string unescaped = InstanceXml.TestXmlUnescapeString(escaped);
			Assert.AreEqual(unescaped, data);
			Trace.WriteLine("Success - Escaped string was " + escaped);
		}

		private static void ValidateRoundtripUnescapeEscape(string data)
		{
			string unescaped = InstanceXml.TestXmlUnescapeString(data);
			string escaped = InstanceXml.TestXmlEscapeString(unescaped);
			Assert.AreEqual(escaped, data);
			Trace.WriteLine("Success - Unescaped string was " + unescaped);
		}
	}
}

#endif