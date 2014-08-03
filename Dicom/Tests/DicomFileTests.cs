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

using System;
using System.IO;
using ClearCanvas.Dicom.IO;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class DicomFileTests : AbstractTest
	{
		[Test]
		public void ConstructorTests()
		{
			DicomFile file = new DicomFile(null);

			file = new DicomFile("filename");

			file = new DicomFile(null, new DicomAttributeCollection(), new DicomAttributeCollection());
		}

		public void WriteOptionsTest(DicomFile sourceFile, DicomWriteOptions options)
		{
			bool result = sourceFile.Save(options);

			Assert.AreEqual(result, true);

			DicomFile newFile = new DicomFile(sourceFile.Filename);

			DicomReadOptions readOptions = DicomReadOptions.Default;
			newFile.Load(readOptions);

			Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);

			// update a tag, and make sure it shows they're different
			newFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
			Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), false);

			// Now update the original file with the name, and they should be the same again
			sourceFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
			Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);

			// Return the original string value.
			sourceFile.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
		}

		[Test]
		public void MultiframeCreateFileTest()
		{
			DicomFile file = new DicomFile("MultiframeCreateFileTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMultiframeXA(dataSet, 511, 511, 5);

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			DicomWriteOptions writeOptions = DicomWriteOptions.Default;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.None;
			WriteOptionsTest(file, writeOptions);

			// Big Endian Tests
			file.Filename = "MultiframeBigEndianCreateFileTest.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

			writeOptions = DicomWriteOptions.Default;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.None;
			WriteOptionsTest(file, writeOptions);
		}

		[Test]
		public void CreateFileTest()
		{
			DicomFile file = new DicomFile("CreateFileTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMR(dataSet);

			string studyId = file.DataSet[DicomTags.StudyId].GetString(0, "");
			Assert.AreEqual(studyId, "1933");

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			DicomWriteOptions writeOptions = DicomWriteOptions.Default;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.None;
			WriteOptionsTest(file, writeOptions);

			// Big Endian Tests
			file.Filename = "BigEndianCreateFileTest.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

			writeOptions = DicomWriteOptions.Default;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			WriteOptionsTest(file, writeOptions);

			writeOptions = DicomWriteOptions.None;
			WriteOptionsTest(file, writeOptions);
		}

		public void ReadOptionsTest(DicomFile sourceFile, DicomReadOptions options, bool areEqual)
		{
			bool result = sourceFile.Save(DicomWriteOptions.Default);

			Assert.AreEqual(result, true);

			DicomFile newFile = new DicomFile(sourceFile.Filename);

			newFile.Load(options);

			if (areEqual)
				Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);
			else
				Assert.AreNotEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);
		}

		[Test]
		public void FileReadTest()
		{
			DicomFile file = new DicomFile("LittleEndianReadFileTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMR(dataSet);

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			DicomReadOptions readOptions = DicomReadOptions.Default;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.StorePixelDataReferences;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.DoNotStorePixelDataInDataSet;
			ReadOptionsTest(file, readOptions, false);

			readOptions = DicomReadOptions.None;
			ReadOptionsTest(file, readOptions, true);

			// Big Endian Tests
			file.Filename = "BigEndianReadTest.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

			readOptions = DicomReadOptions.Default;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.StorePixelDataReferences;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.DoNotStorePixelDataInDataSet;
			ReadOptionsTest(file, readOptions, false);

			readOptions = DicomReadOptions.None;
			ReadOptionsTest(file, readOptions, true);
		}

		[Test]
		public void MultiframeReadTest()
		{
			DicomFile file = new DicomFile("LittleEndianMultiframeTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMultiframeXA(dataSet, 511, 511, 5);

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			DicomReadOptions readOptions = DicomReadOptions.Default;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.StorePixelDataReferences;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.DoNotStorePixelDataInDataSet;
			ReadOptionsTest(file, readOptions, false);

			readOptions = DicomReadOptions.None;
			ReadOptionsTest(file, readOptions, true);

			// Big Endian Tests
			file.Filename = "BigEndianMultiframeTest.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

			readOptions = DicomReadOptions.Default;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.StorePixelDataReferences;
			ReadOptionsTest(file, readOptions, true);

			readOptions = DicomReadOptions.DoNotStorePixelDataInDataSet;
			ReadOptionsTest(file, readOptions, false);

			readOptions = DicomReadOptions.None;
			ReadOptionsTest(file, readOptions, true);
		}

		private void CheckPixels(string filename)
		{
			DicomReadOptions readOptions = DicomReadOptions.StorePixelDataReferences;
			DicomFile newFile = new DicomFile(filename);

			newFile.Load(readOptions);
			DicomUncompressedPixelData pd = new DicomUncompressedPixelData(newFile.DataSet);

			for (int frame = 0; frame < pd.NumberOfFrames; frame++)
			{
				byte[] data = pd.GetFrame(frame);
				uint pdVal = (uint) frame + 1;
				for (int i = 0; i < pd.UncompressedFrameSize; i++, pdVal++)
				{
					if (data[i] != pdVal%255)
					{
						string val = String.Format("Value bad: frame: {0}, pixel: {1}, val1: {2}, val2: {3}", frame, i, data[i], pdVal%255);
						Console.Write(val);
					}
					Assert.AreEqual(data[i], pdVal%255);
				}
			}
		}

		[Test]
		public void ReadPixelsFromDisk()
		{
			// Check odd dimension file w/ odd frames
			DicomFile file = new DicomFile("LittlePixelTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMultiframeXA(dataSet, 511, 511, 5);

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			bool result = file.Save(DicomWriteOptions.Default);
			Assert.AreEqual(result, true);

			CheckPixels(file.Filename);

			// Big Endian Tests
			file.Filename = "BigEndianMultiframeTest.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

			result = file.Save(DicomWriteOptions.Default);
			Assert.AreEqual(result, true);

			CheckPixels(file.Filename);

			// Now check even size w/ even number of frames

			// Setup the file
			file = new DicomFile("LittlePixelTest.dcm");

			dataSet = file.DataSet;

			SetupMultiframeXA(dataSet, 512, 512, 6);

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			result = file.Save(DicomWriteOptions.Default);
			Assert.AreEqual(result, true);

			CheckPixels(file.Filename);

			// Big Endian Tests
			file.Filename = "BigEndianMultiframeTest.dcm";
			file.TransferSyntax = TransferSyntax.ExplicitVrBigEndian;

			result = file.Save(DicomWriteOptions.Default);
			Assert.AreEqual(result, true);

			CheckPixels(file.Filename);
		}

		[Test]
		public void SimpleFileTest()
		{
			DicomFile file = new DicomFile("LittleEndianReadFileTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMR(dataSet);

			SetupMetaInfo(file);

			dataSet[DicomTags.StudyDescription].SetNullValue();

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			DicomReadOptions readOptions = DicomReadOptions.Default;

			bool result = file.Save(DicomWriteOptions.Default);

			Assert.AreEqual(result, true);

			DicomFile newFile = new DicomFile(file.Filename);

			newFile.Load(readOptions);

			Assert.IsTrue(newFile.DataSet[DicomTags.StudyDescription].IsNull);

			Assert.AreEqual(file.DataSet.Equals(newFile.DataSet), true);

			DicomAttributeCollection dicomAttributeCollection = new DicomAttributeCollection();
			dicomAttributeCollection[DicomTags.PatientId].SetNullValue();
			Assert.IsFalse(dicomAttributeCollection[DicomTags.PatientId].IsEmpty, "Dicom Tag is empty, won't be written in DicomStreamWriter.Write()");
		}

		[Test]
		public void ReadPixelDataReferencesTest()
		{
			DicomFile file = new DicomFile("LittleEndianReadFileTest2.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMR(dataSet);

			SetupMetaInfo(file);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			DicomReadOptions readOptions = DicomReadOptions.StorePixelDataReferences;
			bool result = file.Save(DicomWriteOptions.Default);

			Assert.AreEqual(result, true);

			DicomFile newFile = new DicomFile(file.Filename);

			newFile.Load(readOptions);

			DicomAttribute attrib = newFile.DataSet[DicomTags.PixelData];

			Assert.IsFalse(attrib.IsEmpty);
			Assert.IsFalse(attrib.IsNull);
			Assert.AreEqual(attrib.StreamLength, dataSet[DicomTags.PixelData].StreamLength);

			// Set the pixel data to null and re-read
			dataSet[DicomTags.PixelData].SetNullValue();

			result = file.Save(DicomWriteOptions.Default);
			Assert.AreEqual(result, true);

			newFile = new DicomFile(file.Filename);

			newFile.Load(readOptions);

			attrib = newFile.DataSet[DicomTags.PixelData];

			Assert.IsFalse(attrib.IsEmpty);
			Assert.IsTrue(attrib.IsNull);
			Assert.AreEqual(attrib.StreamLength, dataSet[DicomTags.PixelData].StreamLength);
		}

		[Test]
		public void FixedSequenceLengthTest()
		{
			var file = new DicomFile("FixedSequenceLengthTest.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupMR(dataSet);

			SetupMetaInfo(file);

			dataSet[DicomTags.StudyDescription].SetNullValue();

			// Force another empty tag
			var s = dataSet[DicomTags.ServiceEpisodeId];

			var item = new DicomSequenceItem();
			item[DicomTags.PhotometricInterpretation].AppendString("MONOCHROME1");
			item[DicomTags.Rows].AppendUInt16(256);
			item[DicomTags.Columns].AppendUInt16(256);
			item[DicomTags.BitsAllocated].AppendUInt16(8);
			item[DicomTags.BitsStored].AppendUInt16(8);
			item[DicomTags.HighBit].AppendUInt16(7);
			item[DicomTags.PixelData].Values = new byte[] {0x01, 0x02, 0x03, 0x04, 0x05, 0x06};

			// Force an empty tag
			var m = item[DicomTags.LossyImageCompression];

			var privateTagSQ = new DicomTag(0x00091038, "PrivateSQ", "PrivateSQ", DicomVr.SQvr, true, 1, 10, false);
			dataSet[privateTagSQ].AddSequenceItem(item);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			const DicomReadOptions readOptions = DicomReadOptions.Default;
			const DicomWriteOptions writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;

			bool result = file.Save(writeOptions);

			Assert.AreEqual(result, true);

			var newFile = new DicomFile(file.Filename);

			newFile.Load(readOptions);

			Assert.IsTrue(newFile.DataSet[DicomTags.StudyDescription].IsNull);

			Assert.AreEqual(file.DataSet.Equals(newFile.DataSet), true);

			var dicomAttributeCollection = new DicomAttributeCollection();
			dicomAttributeCollection[DicomTags.PatientId].SetNullValue();
			Assert.IsFalse(dicomAttributeCollection[DicomTags.PatientId].IsEmpty, "Dicom Tag is empty, won't be written in DicomStreamWriter.Write()");
		}

		[Test]
		public void UnseekableNonPart10ReadTest()
		{
			const string msgStartingTest = "Running Test: {0}";

			var sopInstanceUid = DicomUid.GenerateUid().UID;
			var sopClassUid = SopClass.AmbulatoryEcgWaveformStorageUid;

			// create the test data stream
			MemoryStream nonPart10Stream;
			{
				var dataSet = new DicomAttributeCollection();
				dataSet[DicomTags.SopClassUid].SetStringValue(sopClassUid);
				dataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);
				dataSet[DicomTags.BitsAllocated].SetInt32(0, 16);
				nonPart10Stream = CreateNonPart10Stream(dataSet);
			}

			// test without the part 10 option
			Console.WriteLine(msgStartingTest, "no part 10");
			try
			{
				var dcf = new DicomFile();
				dcf.Load(new ReadOnlyUnseekableStream(nonPart10Stream), null, DicomReadOptions.Default & ~DicomReadOptions.ReadNonPart10Files);
				Assert.Fail("Expected DicomException when parsing non-part-10 file without part 10 file flag");
			}
			catch (DicomException)
			{
				// pass!
			}

			// test with default read options
			Console.WriteLine(msgStartingTest, "default options");
			{
				nonPart10Stream.Position = 0;
				var dcf = new DicomFile();
				dcf.Load(new ReadOnlyUnseekableStream(nonPart10Stream), null, DicomReadOptions.Default);

				Assert.AreEqual(sopClassUid, dcf.MediaStorageSopClassUid, "MetaInfo Sop Class UID");
				Assert.AreEqual(sopInstanceUid, dcf.MediaStorageSopInstanceUid, "MetaInfo Sop Instance UID");
				Assert.AreEqual(sopClassUid, dcf.DataSet[DicomTags.SopClassUid].ToString(), "DataSet Sop Class UID");
				Assert.AreEqual(sopInstanceUid, dcf.DataSet[DicomTags.SopInstanceUid].ToString(), "DataSet Sop Instance UID");
				Assert.AreEqual(16, dcf.DataSet[DicomTags.BitsAllocated].GetInt32(0, 0), "DataSet Bits Allocated");
			}
		}

		private static MemoryStream CreateNonPart10Stream(DicomAttributeCollection dataSet)
		{
			var memoryStream = new MemoryStream();
			var streamWriter = new DicomStreamWriter(memoryStream);
			streamWriter.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
			streamWriter.Write(TransferSyntax.ImplicitVrLittleEndian, dataSet, DicomWriteOptions.None);
			memoryStream.Position = 0;
			return memoryStream;
		}

		#region ReadOnlyUnseekableStream

		private class ReadOnlyUnseekableStream : Stream
		{
			private readonly Stream _realStream;

			public ReadOnlyUnseekableStream(Stream realStream)
			{
				_realStream = realStream;
			}

			public override bool CanRead
			{
				get { return true; }
			}

			public override bool CanSeek
			{
				get { return false; }
			}

			public override bool CanWrite
			{
				get { return false; }
			}

			public override long Length
			{
				get { return _realStream.Length; }
			}

			public override long Position
			{
				get { return _realStream.Position; }
				set { throw new NotSupportedException(); }
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return _realStream.Read(buffer, offset, count);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}
		}

		#endregion
	}
}

#endif