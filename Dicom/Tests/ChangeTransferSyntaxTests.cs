#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Codec.Tests;
using NUnit.Framework;

// ReSharper disable LocalizableElement

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	internal class ChangeTransferSyntaxTests
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			TransferSyntax.RegisterTransferSyntax(NullDicomCodec.TransferSyntax);
			TransferSyntax.RegisterTransferSyntax(GZipDicomCodec.TransferSyntax);
		}

		[TestFixtureTearDown]
		public void Deinitialize()
		{
			TransferSyntax.UnregisterTransferSyntax(NullDicomCodec.TransferSyntax);
			TransferSyntax.UnregisterTransferSyntax(GZipDicomCodec.TransferSyntax);
		}

		[Test]
		public void TestRoundtripNull()
		{
			TestRoundtripFile<NullDicomCodec>("File based compression and decompression");
			TestRoundtripMemory<NullDicomCodec>("Memory based compression and decompression");
			TestRoundtripFileToMemory<NullDicomCodec>("File based compression and memory based decompression");
			TestRoundtripMemoryToFile<NullDicomCodec>("Memory based compression and file based decompression");
		}

		[Test]
		public void TestRoundtripGZip()
		{
			TestRoundtripFile<GZipDicomCodec>("File based compression and decompression");
			TestRoundtripMemory<GZipDicomCodec>("Memory based compression and decompression");
			TestRoundtripFileToMemory<GZipDicomCodec>("File based compression and memory based decompression");
			TestRoundtripMemoryToFile<GZipDicomCodec>("Memory based compression and file based decompression");
		}

		[Test]
		public void TestDirectRecompression()
		{
			TestRoundtripMemory<GZipDicomCodec, NullDicomCodec>("Direct recompression");
		}

		private static void TestRoundtripMemory<T1, T2>(string message, params object[] args)
			where T1 : IDicomCodec, new()
			where T2 : IDicomCodec, new()
		{
			const string filenameReference = "reference.dcm";

			var codec1 = new T1();
			var codec2 = new T2();

			// create an image
			var instance = DicomImageTestHelper.CreateDicomImage();
			instance.Save(filenameReference);

			// compress the image to 1
			instance.ChangeTransferSyntax(codec1.CodecTransferSyntax, codec1, null);

			// recompress the image to 2
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec1, null);
			instance.ChangeTransferSyntax(codec2.CodecTransferSyntax, codec2, null);

			// recompress the image to 1
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec2, null);
			instance.ChangeTransferSyntax(codec1.CodecTransferSyntax, codec1, null);

			// recompress the image to 2
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec1, null);
			instance.ChangeTransferSyntax(codec2.CodecTransferSyntax, codec2, null);

			// decompress the image
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec2, null);

			// compare results
			var reference = new DicomFile();
			reference.Load(DicomReadOptions.Default, filenameReference);
			AssertEquals(reference, instance, message, args);
		}

		private static void TestRoundtripMemory<T>(string message, params object[] args)
			where T : IDicomCodec, new()
		{
			const string filenameReference = "reference.dcm";
			var codec = new T();

			// create an image
			var instance = DicomImageTestHelper.CreateDicomImage();
			instance.Save(filenameReference);

			// compress the image
			instance.ChangeTransferSyntax(codec.CodecTransferSyntax, codec, null);

			// decompress the image
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec, null);

			// compare results
			var reference = new DicomFile();
			reference.Load(DicomReadOptions.Default, filenameReference);
			AssertEquals(reference, instance, message, args);
		}

		private static void TestRoundtripMemoryToFile<T>(string message, params object[] args)
			where T : IDicomCodec, new()
		{
			var filenameReference = string.Format("reference_{0}.dcm", typeof (T).Name);
			var filenameDecompressed = string.Format("decompressed_{0}.dcm", typeof (T).Name);
			var codec = new T();

			// create an image
			var instance = DicomImageTestHelper.CreateDicomImage();
			instance.Save(filenameReference);

			// compress the image
			instance.ChangeTransferSyntax(codec.CodecTransferSyntax, codec, null);

			// decompress the image
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec, null);
			instance.Save(filenameDecompressed);

			// compare results
			var reference = new DicomFile();
			reference.Load(DicomReadOptions.Default, filenameReference);
			var actual = new DicomFile();
			actual.Load(DicomReadOptions.Default, filenameDecompressed);
			AssertEquals(reference, actual, message, args);
		}

		private static void TestRoundtripFile<T>(string message, params object[] args)
			where T : IDicomCodec, new()
		{
			var filenameReference = string.Format("reference_{0}.dcm", typeof (T).Name);
			var filenameCompressed = string.Format("compressed_{0}.dcm", typeof (T).Name);
			var codec = new T();

			// create an image
			var original = DicomImageTestHelper.CreateDicomImage();
			original.Save(filenameReference);

			// compress the image
			var compressed = new DicomFile();
			compressed.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences, filenameReference);
			compressed.ChangeTransferSyntax(codec.CodecTransferSyntax, codec, null);
			compressed.Save(filenameCompressed);

			// decompress the image
			var decompressed = new DicomFile();
			decompressed.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences, filenameCompressed);
			decompressed.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec, null);

			// compare results
			AssertEquals(original, decompressed, message, args);
		}

		private static void TestRoundtripFileToMemory<T>(string message, params object[] args)
			where T : IDicomCodec, new()
		{
			var filenameReference = string.Format("reference_{0}.dcm", typeof (T).Name);
			var codec = new T();

			// create an image
			var reference = DicomImageTestHelper.CreateDicomImage();
			reference.Save(filenameReference);

			// compress the image
			var instance = new DicomFile();
			instance.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences, filenameReference);
			instance.ChangeTransferSyntax(codec.CodecTransferSyntax, codec, null);

			// decompress the image
			instance.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian, codec, null);

			// compare results
			AssertEquals(reference, instance, message, args);
		}

		private static void AssertEquals(DicomFile expected, DicomFile actual, string message, params object[] args)
		{
			var failures = new List<DicomAttributeComparisonResult>();
			if (!expected.DataSet.Equals(actual.DataSet, ref failures))
			{
				foreach (var result in failures)
					Console.WriteLine("{0} {1}: {2}", result.ResultType, result.TagName, result.Details);
				Assert.Fail(message, args);
			}
		}
	}
}

// ReSharper restore LocalizableElement

#endif