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

namespace ClearCanvas.Dicom.Utilities.Xml
{
	/// <summary>
	/// Class used in conjunction with <see cref="StudyXml"/> for reading and writing an XML representation of a Study.
	/// </summary>
	public static class StudyXmlIo
	{
		public static void Write(XmlDocument theDoc, Stream theStream)
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
			theDoc.WriteTo(tw);
			tw.Flush();
			tw.Close();
		}

        public static void Write(XmlDocument theDoc, string filename)
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
            theDoc.WriteTo(tw);
            tw.Flush();
            tw.Close();
        }

		public static void WriteGzip(XmlDocument theDoc, Stream theStream)
		{
			var ms = new MemoryStream();
		    var xmlSettings = new XmlWriterSettings
		                          {
		                              Encoding = Encoding.UTF8,
		                              ConformanceLevel = ConformanceLevel.Document,
		                              Indent = false,
		                              NewLineOnAttributes = false,
		                              CheckCharacters = true,
		                              IndentChars = string.Empty
		                          };


		    XmlWriter tw = XmlWriter.Create(ms, xmlSettings);

			theDoc.WriteTo(tw);
			tw.Flush();
			tw.Close();

			byte[] buffer = ms.GetBuffer();

			var compressedzipStream = new GZipStream(theStream, CompressionMode.Compress, true);
			compressedzipStream.Write(buffer, 0, buffer.Length);
			// Close the stream.
			compressedzipStream.Flush();
			compressedzipStream.Close();

			// Force a flush
			theStream.Flush();
		}

		public static void WriteXmlAndGzip(XmlDocument theDoc, Stream theXmlStream, Stream theGzipStream)
		{
			// Write to a memory stream, then flush to disk and to gzip file
			MemoryStream ms = new MemoryStream();
			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.Encoding = Encoding.UTF8;
			xmlSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlSettings.Indent = false;
			xmlSettings.NewLineOnAttributes = false;
			xmlSettings.CheckCharacters = true;
			xmlSettings.IndentChars = "";

			XmlWriter tw = XmlWriter.Create(ms, xmlSettings);

			theDoc.WriteTo(tw);

			tw.Flush();
			tw.Close();

			byte[] buffer = ms.GetBuffer();
			
			GZipStream compressedzipStream = new GZipStream(theGzipStream, CompressionMode.Compress, true);
			compressedzipStream.Write(buffer, 0, (int)ms.Length);

			// Close the stream.
			compressedzipStream.Flush();
			compressedzipStream.Close();

			theXmlStream.Write(buffer, 0, (int)ms.Length);

			// Force a flush.
			theXmlStream.Flush();
			theGzipStream.Flush();
		}

		public static void ReadGzip(XmlDocument theDoc, Stream theStream)
		{
			GZipStream zipStream = new GZipStream(theStream, CompressionMode.Decompress);

			theDoc.Load(zipStream);

			return;
		}

		public static void Read(XmlDocument theDoc, Stream theStream)
		{
			theDoc.Load(theStream);
		}
	}
}