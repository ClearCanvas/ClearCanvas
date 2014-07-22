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

using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Utilities.Xml;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
    internal class TestInstance : XmlSopDataSource
    {
        private readonly DicomFile _real;

        public TestInstance(InstanceXml instanceXml, DicomFile real) : base(instanceXml)
        {
            _real = real;
        }

        protected override DicomFile GetFullHeader()
        {
            return _real;
        }
    }

    [TestFixture]
    public class XmlSopDataSourceTests : AbstractTest
    {
        private InstanceXml GetInstanceXml(StudyXmlOutputSettings outputSettings, out DicomFile real)
        {
            var xml = new StudyXml();
            real = new DicomFile();
            SetupMR(real.DataSet);
            real.MediaStorageSopClassUid = real.DataSet[DicomTags.SopClassUid].ToString();
            real.MetaInfo[DicomTags.SopClassUid].SetString(0, real.DataSet[DicomTags.SopClassUid].ToString());
            
            var bytes = new Byte[2048];
            real.DataSet[DicomTags.RedPaletteColorLookupTableData].Values = bytes;

            var privateTag = new DicomTag(0x00111301, "Private Tag", "PrivateTag", DicomVr.CSvr, false, 1, 1, false);
            real.DataSet[privateTag].SetString(0, "Private");

            var sequences = (DicomSequenceItem[])real.DataSet[DicomTags.RequestAttributesSequence].Values;
            var firstItem = sequences.First();
            firstItem[DicomTags.RedPaletteColorLookupTableData].Values = bytes;

            var attr = real.DataSet[DicomTags.ReferencedStudySequence];
            var sequenceItem = new DicomSequenceItem();
            attr.AddSequenceItem(sequenceItem);
            sequenceItem[privateTag].SetString(0, "Private");

            xml.AddFile(real);
            
            var memento = xml.GetMemento(outputSettings ?? new StudyXmlOutputSettings
            {
                IncludeLargeTags = StudyXmlTagInclusion.IncludeTagExclusion,
                IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag,
                MaxTagLength = 1024
            });

            xml = new StudyXml();
            xml.SetMemento(memento);
            return xml.First().First();
        }

        [Test]
        public void Test_Excluded_BigBinary()
        {
            DicomFile real;
            var instanceXml = GetInstanceXml(null, out real);
            using (var instance = new TestInstance(instanceXml, real))
            {
                Assert.AreEqual("MR", instance[DicomTags.Modality].ToString());
                Assert.AreEqual(false, instance._fullHeaderLoaded);

                var bytes = (byte[]) instance[DicomTags.RedPaletteColorLookupTableData].Values;
                Assert.AreEqual(2048, bytes.Length);
                Assert.AreEqual(true, instance._fullHeaderLoaded);
            }
        }

        [Test]
        public void Test_Excluded_Private()
        {
            DicomFile real;
            var instanceXml = GetInstanceXml(null, out real);
            using (var instance = new TestInstance(instanceXml, real))
            {
                Assert.AreEqual("MR", instance[DicomTags.Modality].ToString());
                Assert.AreEqual(false, instance._fullHeaderLoaded);

                var privateAttribute = instance[0x00111301];
                Assert.AreEqual(true, instance._fullHeaderLoaded);
                Assert.AreEqual("Private", privateAttribute.ToString());
            }
        }

        [Test]
        public void Test_Excluded_SequenceAttribute()
        {
            DicomFile real;
            var instanceXml = GetInstanceXml(null, out real);
            using (var instance = new TestInstance(instanceXml, real))
            {
                Assert.AreEqual("MR", instance[DicomTags.Modality].ToString());
                Assert.AreEqual(false, instance._fullHeaderLoaded);

                var sequences = (DicomSequenceItem[])instance[DicomTags.RequestAttributesSequence].Values;
                var sequenceItem = sequences.First();
                var bytes = (byte[])sequenceItem[DicomTags.RedPaletteColorLookupTableData].Values;
                Assert.AreEqual(2048, bytes.Length);
                Assert.AreEqual(true, instance._fullHeaderLoaded);
            }
        }

        [Test]
        [Ignore("See #12459.")]
        public void Test_Excluded_PrivateSequenceAttribute()
        {
            DicomFile real;
            var outputSettings = new StudyXmlOutputSettings
            {
                //If this were enabled, it would work.
                //IncludePrivateValues = StudyXmlTagInclusion.IncludeTagExclusion
            };

            var instanceXml = GetInstanceXml(outputSettings, out real);
            using (var instance = new TestInstance(instanceXml, real))
            {
                Assert.AreEqual("MR", instance[DicomTags.Modality].ToString());
                Assert.AreEqual(false, instance._fullHeaderLoaded);

                var sequences = (DicomSequenceItem[])instance[DicomTags.ReferencedStudySequence].Values;
                Assert.AreEqual(true, instance._fullHeaderLoaded);

                var sequenceItem = sequences.First();
                var privateAttribute = sequenceItem[0x00111301];
                Assert.AreEqual("Private", privateAttribute.ToString());
            }
        }
    }
}

#endif