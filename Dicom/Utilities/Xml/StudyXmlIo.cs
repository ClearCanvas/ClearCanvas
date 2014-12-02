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

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Xml
{
	/// <summary>
	/// Class used in conjunction with <see cref="StudyXml"/> for reading and writing an XML representation of a Study.
	/// </summary>
	public static class StudyXmlIo
	{
		public static void Write(StudyXmlMemento theMemento, Stream theStream)
		{
			if (theMemento.RootNode != null)
			{
				var sw = new StreamWriter(theStream, Encoding.UTF8);
				sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				theMemento.RootNode.WriteTo(sw);
				sw.Flush();
			}
			else
			{
				var xmlSettings = new XmlWriterSettings
					{
						Encoding = Encoding.UTF8,
						ConformanceLevel = ConformanceLevel.Document,
						Indent = false,
						NewLineOnAttributes = false,
						CheckCharacters = true,
						IndentChars = string.Empty
					};


				XmlWriter tw = XmlWriter.Create(theStream, xmlSettings);
				theMemento.Document.WriteTo(tw);
				tw.Flush();
				tw.Close();
			}
		}

		public static void Write(StudyXmlMemento theMemento, string filename)
        {
			if (theMemento.RootNode != null)
			{
				using (var fs = FileStreamOpener.OpenForSoleUpdate(filename, FileMode.CreateNew))
				{
					Write(theMemento, fs);
				}
			}
			else
			{
				var xmlSettings = new XmlWriterSettings
					{
						Encoding = Encoding.UTF8,
						ConformanceLevel = ConformanceLevel.Document,
						Indent = false,
						NewLineOnAttributes = false,
						CheckCharacters = true,
						IndentChars = string.Empty
					};

				XmlWriter tw = XmlWriter.Create(filename, xmlSettings);
				theMemento.Document.WriteTo(tw);
				tw.Flush();
				tw.Close();
			}
        }

		public static void WriteGzip(StudyXmlMemento theMemento, Stream theStream)
		{
			using (var compressedZipStream = new GZipStream(theStream, CompressionMode.Compress, true))
			{
				Write(theMemento, compressedZipStream);

				// Close the stream.
				compressedZipStream.Flush();
				compressedZipStream.Close();
			}

			// Force a flush
			theStream.Flush();
		}

		public static void WriteXmlAndGzip(StudyXmlMemento theMemento, Stream theXmlStream, Stream theGzipStream)
		{
			// Write to a memory stream, then flush to disk and to gzip file
			var ms = new LargeMemoryStream();

			Write(theMemento, ms);

			using (var compressedZipStream = new GZipStream(theGzipStream, CompressionMode.Compress, true))
			{
				ms.Seek(0, SeekOrigin.Begin);
				ms.WriteTo(compressedZipStream);

				// Close the stream.
				compressedZipStream.Flush();
				compressedZipStream.Close();
			}

			ms.Seek(0, SeekOrigin.Begin); 
			ms.WriteTo(theXmlStream);

			// Force a flush.
			theXmlStream.Flush();
			theGzipStream.Flush();
		}

		public static void ReadGzip(StudyXmlMemento theMemento, Stream theStream)
		{
			using (var zipStream = new GZipStream(theStream, CompressionMode.Decompress))
			{
				if (theMemento.Document == null)
					theMemento.Document = new XmlDocument();

				theMemento.Document.Load(zipStream);
			}
		}

		public static void Read(StudyXmlMemento theMemento, Stream theStream)
		{
			if (theMemento.Document == null)
				theMemento.Document = new XmlDocument();
			theMemento.Document.Load(theStream);
		}
	}
}