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
using ClearCanvas.Dicom.IO;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class SequenceTests : AbstractTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			DicomImplementation.UnitTest = true;
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			DicomImplementation.UnitTest = false;
		}

		[Test]
		public void TestSequenceImplicitVr()
		{
			var syntax = TransferSyntax.ImplicitVrLittleEndian;
			var sq = new DicomAttributeSQ(DicomTags.IconImageSequence);

			// note: we handle an empty sequence (0 items) by explicitly encoding zero length regardless of options

			// sq_length = tag(4) + len(4)
			var options = DicomWriteOptions.None;
			Assert.AreEqual(8, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + len(4)
			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(8, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + len(4)
			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(8, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + len(4)
			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(8, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);
		}

		[Test]
		public void TestSequenceImplicitVr2()
		{
			var syntax = TransferSyntax.ImplicitVrLittleEndian;
			var sq = new DicomAttributeSQ(DicomTags.IconImageSequence);

			var item1 = new DicomSequenceItem();
			item1[DicomTags.AccessionNumber].SetStringValue("ABCDEFG");
			sq.AddSequenceItem(item1);

			var item2 = new DicomSequenceItem();
			item2[DicomTags.AccessionNumber].SetStringValue("XYZ");
			sq.AddSequenceItem(item2);

			// note: both sequence and item delimiter tags are calculated as part of the sequence - the item considers only the total length of its contents

			// item1_length = contents(16)
			Assert.AreEqual(16, item1[DicomTags.AccessionNumber].CalculateWriteLength(syntax, DicomWriteOptions.None), "Attr1 Length");
			Assert.AreEqual(16, item1.CalculateWriteLength(syntax, DicomWriteOptions.None), "Item1 Length");

			// item2_length = contents(12)
			Assert.AreEqual(12, item2[DicomTags.AccessionNumber].CalculateWriteLength(syntax, DicomWriteOptions.None), "Attr2 Length");
			Assert.AreEqual(12, item2.CalculateWriteLength(syntax, DicomWriteOptions.None), "Item2 Length");

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + item1_length(16) + item_delimiter_tag(4) + item_delimiter_len(4)
			//                             + item_tag(4) + item_len(4) + item2_length(12) + item_delimiter_tag(4) + item_delimiter_len(4)
			//                             + sq_delimiter_tag(4) + sq_delimiter_len(4)
			var options = DicomWriteOptions.None;
			Assert.AreEqual(76, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + item1_length(16) + item_delimiter_tag(4) + item_delimiter_len(4)
			//                             + item_tag(4) + item_len(4) + item2_length(12) + item_delimiter_tag(4) + item_delimiter_len(4)
			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(68, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + item1_length(16)
			//                             + item_tag(4) + item_len(4) + item2_length(12)
			//                             + sq_delimiter_tag(4) + sq_delimiter_len(4)
			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(60, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + item1_length(16)
			//                             + item_tag(4) + item_len(4) + item2_length(12)
			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(52, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);
		}

		[Test]
		public void TestSequenceExplicitVr()
		{
			var syntax = TransferSyntax.ExplicitVrLittleEndian;
			var sq = new DicomAttributeSQ(DicomTags.IconImageSequence);

			// note: we handle an empty sequence (0 items) by explicitly encoding zero length regardless of options

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4)
			var options = DicomWriteOptions.None;
			Assert.AreEqual(12, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4)
			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(12, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4)
			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(12, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4)
			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(12, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);
		}

		[Test]
		public void TestSequenceExplicitVr2()
		{
			var syntax = TransferSyntax.ExplicitVrLittleEndian;
			var sq = new DicomAttributeSQ(DicomTags.IconImageSequence);

			var item1 = new DicomSequenceItem();
			item1[DicomTags.AccessionNumber].SetStringValue("ABCDEFG");
			sq.AddSequenceItem(item1);

			var item2 = new DicomSequenceItem();
			item2[DicomTags.AccessionNumber].SetStringValue("XYZ");
			sq.AddSequenceItem(item2);

			// note: both sequence and item delimiter tags are calculated as part of the sequence - the item considers only the total length of its contents

			// item1_length = contents(16)
			Assert.AreEqual(16, item1[DicomTags.AccessionNumber].CalculateWriteLength(syntax, DicomWriteOptions.None), "Attr1 Length");
			Assert.AreEqual(16, item1.CalculateWriteLength(syntax, DicomWriteOptions.None), "Item1 Length");

			// item2_length = contents(12)
			Assert.AreEqual(12, item2[DicomTags.AccessionNumber].CalculateWriteLength(syntax, DicomWriteOptions.None), "Attr2 Length");
			Assert.AreEqual(12, item2.CalculateWriteLength(syntax, DicomWriteOptions.None), "Item2 Length");

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + item1_length(16) + item_delimiter_tag(4) + item_delimiter_len(4)
			//                                                   + item_tag(4) + item_len(4) + item2_length(12) + item_delimiter_tag(4) + item_delimiter_len(4)
			//                                                   + sq_delimiter_tag(4) + sq_delimiter_len(4)
			var options = DicomWriteOptions.None;
			Assert.AreEqual(80, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + item1_length(16) + item_delimiter_tag(4) + item_delimiter_len(4)
			//                                                   + item_tag(4) + item_len(4) + item2_length(12) + item_delimiter_tag(4) + item_delimiter_len(4)
			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(72, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + item1_length(16)
			//                                                   + item_tag(4) + item_len(4) + item2_length(12)
			//                                                   + sq_delimiter_tag(4) + sq_delimiter_len(4)
			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(64, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + item1_length(16)
			//                                                   + item_tag(4) + item_len(4) + item2_length(12)
			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(56, sq.CalculateWriteLength(syntax, options), "SQ Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsImplicitVr()
		{
			var syntax = TransferSyntax.ImplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			// note: an empty fragment sequence still has one item in it representing the empty offset table
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + offset_table(0)
			//                             + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.None;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsImplicitVr2()
		{
			var syntax = TransferSyntax.ImplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			var frag1 = new DicomFragment(new ByteBuffer(new byte[16]));
			sq.AddFragment(frag1);

			var frag2 = new DicomFragment(new ByteBuffer(new byte[12]));
			sq.AddFragment(frag2);

			// note: both sequence and item delimiter tags are calculated as part of the sequence - the item considers only the total length of its contents
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// frag1_length = 16
			Assert.AreEqual(16, frag1.Length, "Frag1 Length");

			// frag2_length = 12
			Assert.AreEqual(12, frag2.Length, "Frag2 Length");

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + offset_table(0)
			//                             + item_tag(4) + item_len(4) + frag1_length(16)
			//                             + item_tag(4) + item_len(4) + frag2_length(12)
			//                             + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.None;
			Assert.AreEqual(68, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(68, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(68, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(68, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsExplicitVr()
		{
			var syntax = TransferSyntax.ExplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			// note: an empty fragment sequence still has one item in it representing the empty offset table
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + offset_table(0)
			//                                                   + sq_delimiter_tag(4) + sq_delimiter_len(4)
			var options = DicomWriteOptions.None;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsExplicitVr2()
		{
			var syntax = TransferSyntax.ExplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			var frag1 = new DicomFragment(new ByteBuffer(new byte[16]));
			sq.AddFragment(frag1);

			var frag2 = new DicomFragment(new ByteBuffer(new byte[12]));
			sq.AddFragment(frag2);

			// note: both sequence and item delimiter tags are calculated as part of the sequence - the item considers only the total length of its contents
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// frag1_length = 16
			Assert.AreEqual(16, frag1.Length, "Frag1 Length");

			// frag2_length = 12
			Assert.AreEqual(12, frag2.Length, "Frag2 Length");

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + offset_table(0)
			//                                                   + item_tag(4) + item_len(4) + frag1_length(16)
			//                                                   + item_tag(4) + item_len(4) + frag2_length(12)
			//                                                   + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.None;
			Assert.AreEqual(72, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(72, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(72, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(72, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsWithOffsetTableImplicitVr()
		{
			var syntax = TransferSyntax.ImplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			sq.SetOffsetTable(new List<uint>());

			// note: an empty fragment sequence still has one item in it representing the empty offset table
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + offset_table(0)
			//                             + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.WriteFragmentOffsetTable;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(24, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsWithOffsetTableImplicitVr2()
		{
			var syntax = TransferSyntax.ImplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			var frag1 = new DicomFragment(new ByteBuffer(new byte[16]));
			sq.AddFragment(frag1);

			var frag2 = new DicomFragment(new ByteBuffer(new byte[12]));
			sq.AddFragment(frag2);

			// offset_table = 8
			sq.SetOffsetTable(new List<uint> {0, 24});

			// note: both sequence and item delimiter tags are calculated as part of the sequence - the item considers only the total length of its contents
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// frag1_length = 16
			Assert.AreEqual(16, frag1.Length, "Frag1 Length");

			// frag2_length = 12
			Assert.AreEqual(12, frag2.Length, "Frag2 Length");

			// sq_length = tag(4) + len(4) + item_tag(4) + item_len(4) + offset_table(8)
			//                             + item_tag(4) + item_len(4) + frag1_length(16)
			//                             + item_tag(4) + item_len(4) + frag2_length(12)
			//                             + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.WriteFragmentOffsetTable;
			Assert.AreEqual(76, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(76, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(76, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(76, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsWithOffsetTableExplicitVr()
		{
			var syntax = TransferSyntax.ExplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			sq.SetOffsetTable(new List<uint>());

			// note: an empty fragment sequence still has one item in it representing the empty offset table
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + offset_table(0)
			//                                                   + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.WriteFragmentOffsetTable;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(28, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}

		[Test]
		public void TestFragmentsWithOffsetTableExplicitVr2()
		{
			var syntax = TransferSyntax.ExplicitVrLittleEndian;
			var sq = new DicomFragmentSequence(DicomTags.PixelData);

			var frag1 = new DicomFragment(new ByteBuffer(new byte[16]));
			sq.AddFragment(frag1);

			var frag2 = new DicomFragment(new ByteBuffer(new byte[12]));
			sq.AddFragment(frag2);

			// offset_table = 8
			sq.SetOffsetTable(new List<uint> {0, 24});

			// note: both sequence and item delimiter tags are calculated as part of the sequence - the item considers only the total length of its contents
			// furthermore, fragment sequences are *always* undefined sequence length and explicit item (fragment) length

			// frag1_length = 16
			Assert.AreEqual(16, frag1.Length, "Frag1 Length");

			// frag2_length = 12
			Assert.AreEqual(12, frag2.Length, "Frag2 Length");

			// sq_length = tag(4) + vr(2) + reserved(2) + len(4) + item_tag(4) + item_len(4) + offset_table(8)
			//                                                   + item_tag(4) + item_len(4) + frag1_length(16)
			//                                                   + item_tag(4) + item_len(4) + frag2_length(12)
			//                                                   + sq_delimiter_tag(4) + sq_delimiter_len(4)

			var options = DicomWriteOptions.WriteFragmentOffsetTable;
			Assert.AreEqual(80, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence;
			Assert.AreEqual(80, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(80, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);

			options = DicomWriteOptions.WriteFragmentOffsetTable | DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
			Assert.AreEqual(80, sq.CalculateWriteLength(syntax, options), "Fragment Sequence Length (Options={0})", options);
		}
	}
}

#endif