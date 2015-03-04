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
using ClearCanvas.Dicom.Codec.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class IconImageSequenceTests : AbstractTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			DicomImplementation.UnitTest = true;
			NullDicomCodec.Register();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			NullDicomCodec.Unregister();
			DicomImplementation.UnitTest = false;
		}

		private void SetupImageWithIconSequence(DicomFile file, bool explicitVr)
		{
			DicomAttributeCollection dataSet = file.DataSet;

			SetupSecondaryCapture(dataSet);
			CreateIconImageSequence(dataSet);

			SetupMetaInfo(file);

			file.TransferSyntax = explicitVr ? TransferSyntax.ExplicitVrLittleEndian : TransferSyntax.ImplicitVrLittleEndian;
		}

		private void SetupEncapsulatedImageWithIconSequence(DicomFile file, bool encapsulateIconPixelData)
		{
			var codec = new NullDicomCodec();

			DicomAttributeCollection dataSet = file.DataSet;

			SetupSecondaryCapture(dataSet);

			SetupMetaInfo(file);

			file.TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
			file.ChangeTransferSyntax(codec.CodecTransferSyntax);

			// explicitly create the icon sequence as either encapsulated or native, so that we are not depending on correct behaviour elsewhere...
			CreateIconImageSequence(dataSet);

			if (encapsulateIconPixelData)
			{
				var dataset = ((DicomAttributeSQ) dataSet[DicomTags.IconImageSequence])[0];
				var pixelData = dataset[DicomTags.PixelData];

				var pd = new DicomUncompressedPixelData(dataset);
				using (var pixelStream = ((DicomAttributeBinary) pixelData).AsStream())
				{
					//Before compression, make the pixel data more "typical", so it's harder to mess up the codecs.
					//NOTE: Could combine mask and align into one method so we're not iterating twice, but I prefer having the methods separate.
					if (DicomUncompressedPixelData.RightAlign(pixelStream, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
					{
						var newHighBit = (ushort) (pd.HighBit - pd.LowBit);

						pd.HighBit = newHighBit; //correct high bit after right-aligning.
						dataset[DicomTags.HighBit].SetUInt16(0, newHighBit);
					}
					DicomUncompressedPixelData.ZeroUnusedBits(pixelStream, pd.BitsAllocated, pd.BitsStored, pd.HighBit);
				}

				// Set transfer syntax before compression, the codecs need it.
				var fragments = new DicomCompressedPixelData(pd) {TransferSyntax = codec.CodecTransferSyntax};
				codec.Encode(pd, fragments, null);
				fragments.UpdateAttributeCollection(dataset);
			}
		}

		private void ReadWriteOptionsTest(DicomFile sourceFile, DicomReadOptions readOptions, DicomWriteOptions options)
		{
			var sourceIconImageSq = (DicomAttributeSQ) sourceFile.DataSet[DicomTags.IconImageSequence];
			Assert.IsTrue(!sourceIconImageSq.IsEmpty && !sourceIconImageSq.IsNull && sourceIconImageSq.Count == 1, "File doesn't have icon image sequence");
			Assert.IsTrue(sourceFile.Save(options), "Failed to write file");

			var newFile = new DicomFile(sourceFile.Filename);

			newFile.Load(readOptions);

			var resultList = new List<DicomAttributeComparisonResult>();
			var result = sourceFile.DataSet.Equals(newFile.DataSet, ref resultList);
			string resultMessage = resultList.Count > 0 ? resultList.First().ToString() : string.Empty;
			Assert.IsTrue(result, resultMessage);

			var newIconImageSq = (DicomAttributeSQ) newFile.DataSet[DicomTags.IconImageSequence];
			Assert.IsTrue(!newIconImageSq.IsEmpty && !newIconImageSq.IsNull && newIconImageSq.Count == 1, "New file doesn't have icon image sequence");

			// update a tag, and make sure it shows they're different
			newFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
			Assert.IsFalse(sourceFile.DataSet.Equals(newFile.DataSet, ref resultList));

			// Now update the original file with the name, and they should be the same again
			sourceFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
			Assert.IsTrue(sourceFile.DataSet.Equals(newFile.DataSet));

			// Return the original string value.
			sourceFile.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
		}

		[Test]
		public void TestRoundtripImplicit()
		{
			var file = new DicomFile("TestRoundtripImplicit.dcm");

			SetupImageWithIconSequence(file, false);

			const DicomReadOptions readOptions = DicomReadOptions.Default;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripImplicitFileReferences()
		{
			var file = new DicomFile("TestRoundtripImplicitFileReferences.dcm");

			SetupImageWithIconSequence(file, false);

			const DicomReadOptions readOptions = DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripExplicit()
		{
			var file = new DicomFile("TestRoundtripExplicit.dcm");

			SetupImageWithIconSequence(file, true);

			const DicomReadOptions readOptions = DicomReadOptions.Default;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripExplicitFileReferences()
		{
			var file = new DicomFile("TestRoundtripExplicitFileReferences.dcm");

			SetupImageWithIconSequence(file, true);

			const DicomReadOptions readOptions = DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripEncapsulatedImageNativeIcon()
		{
			var file = new DicomFile("TestRoundtripEncapsulatedImageNativeIcon.dcm");

			SetupEncapsulatedImageWithIconSequence(file, false);

			const DicomReadOptions readOptions = DicomReadOptions.Default;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripEncapsulatedImageNativeIconFileReferences()
		{
			var file = new DicomFile("TestRoundtripEncapsulatedImageNativeIconFileReferences.dcm");

			SetupEncapsulatedImageWithIconSequence(file, false);

			const DicomReadOptions readOptions = DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripEncapsulatedImageEncapsulatedIcon()
		{
			var file = new DicomFile("TestRoundtripEncapsulatedImageEncapsulatedIcon.dcm");

			SetupEncapsulatedImageWithIconSequence(file, true);

			const DicomReadOptions readOptions = DicomReadOptions.Default;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}

		[Test]
		public void TestRoundtripEncapsulatedImageEncapsulatedIconFileReferences()
		{
			var file = new DicomFile("TestRoundtripEncapsulatedImageEncapsulatedIconFileReferences.dcm");

			SetupEncapsulatedImageWithIconSequence(file, true);

			const DicomReadOptions readOptions = DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences;

			var writeOptions = DicomWriteOptions.WriteFragmentOffsetTable;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			ReadWriteOptionsTest(file, readOptions, writeOptions);

			writeOptions = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem | DicomWriteOptions.ExplicitLengthSequence;
			ReadWriteOptionsTest(file, readOptions, writeOptions);
		}
	}
}

#endif