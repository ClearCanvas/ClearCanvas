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

#pragma warning disable 1591, 0168

#if UNIT_TESTS
using System;
using System.Collections.Generic;
using System.Globalization;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
    [TestFixture]
    public class AttributeTests : AbstractTest
    {
		public static readonly CultureInfo CultureInfo = new CultureInfo("en-US");

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

        #region DicomAttributeAE Test
        [Test]
        public void AttributeAETest()
        {
            // ValidateVrValues is turned off by default, enable for sake of tests.
            AttributeAETestSuite test = new AttributeAETestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestGet();
        }

        private class AttributeAETestSuite
        {
            // SetString()
            //      1.     
            public DicomAttributeAE CreateAttribute()
            {
                DicomAttributeAE attr= new DicomAttributeAE(DicomTagDictionary.GetDicomTag(DicomTags.RetrieveAeTitle));
                Assert.AreEqual(0, attr.Count);
                Assert.AreEqual(0, attr.StreamLength);

                return attr;
            }

            public void TestConstructors()
            {
                DicomAttributeAE attrib;
                try
                {
                    attrib= new DicomAttributeAE(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {
                   
                }
               
                attrib = new DicomAttributeAE(DicomTagDictionary.GetDicomTag(DicomTags.RetrieveAeTitle));
                
            }

            public void TestSet()
            {
                DicomAttributeAE attr;

                #region Set String
                attr = CreateAttribute();

                attr.SetString(0, "TestAE");
                Assert.AreEqual(1, attr.Count);
                Assert.AreEqual("TestAE".Length, attr.StreamLength);

                attr.SetString(1, "TestAE2");
                Assert.AreEqual(2, attr.Count);
                Assert.AreEqual("TestAE\\TestAE2".Length, attr.StreamLength);

                try
                {
                    attr.SetString(10, "TestAE3");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual("TestAE\\TestAE2".Length, attr.StreamLength);
                    Assert.AreEqual(2, attr.Count);
                }
                
                // special case: set null or empty string
                attr = CreateAttribute(); 
                attr.SetString(0, null);
                Assert.AreEqual(1, attr.Count);
                Assert.AreEqual(0, attr.StreamLength);

                attr = CreateAttribute();
                attr.SetString(0, "");
                Assert.AreEqual(1, attr.Count);
                Assert.AreEqual(0, attr.StreamLength);

                #endregion

                #region SetStringValue()
                attr = CreateAttribute();

                attr.SetStringValue("TestAE");
                Assert.AreEqual(1, attr.Count);
                Assert.AreEqual("TestAE".Length, attr.StreamLength);

                attr.SetStringValue("TestAE\\TestAE2");
                Assert.AreEqual(2, attr.Count);
                Assert.AreEqual("TestAE\\TestAE2".Length, attr.StreamLength);

                // special case: set null or empty string
                attr = CreateAttribute();
                attr.SetStringValue( null);
                Assert.AreEqual(1, attr.Count);
                Assert.AreEqual(0, attr.StreamLength);

                attr.SetStringValue( "");
                Assert.AreEqual(1, attr.Count);
                Assert.AreEqual(0, attr.StreamLength);

                #endregion

            }

            public void TestGet()
            {
                DicomAttributeAE attr;
                #region Get String
                string stringVal;

                attr = CreateAttribute();

                attr.SetStringValue("TestAE\\TestAE2");
                Assert.AreEqual(2, attr.Count);

                Assert.IsTrue(attr.TryGetString(0, out stringVal) == true);
                Assert.AreEqual("TestAE", stringVal);
                Assert.AreEqual("TestAE", attr.GetString(0, ""));
                
                Assert.IsTrue(attr.TryGetString(1, out stringVal) == true);
                Assert.AreEqual("TestAE2", stringVal);
                Assert.AreEqual("TestAE2", attr.GetString(1, ""));

                // invalid index
                Assert.IsTrue(attr.TryGetString(10, out stringVal) == false);
                Assert.AreEqual("", attr.GetString(10, ""));

                    


                #endregion
            }

            

        }
        #endregion

        #region DicomAttributeAS Test
        public void AttributeASTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeAS attrib = new DicomAttributeAS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeAS attrib = new DicomAttributeAS(DicomTagDictionary.GetDicomTag(DicomTags.PatientsAge));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region DICOMAttributeAT Test
        [Test]
        public void AttributeATTest()
        {

            AttributeATTestSuite test = new AttributeATTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
            
        }
        private class AttributeATTestSuite
        {
            public DicomAttributeAT CreateAttribute()
            {
                DicomAttributeAT attr = new DicomAttributeAT(DicomTagDictionary.GetDicomTag(DicomTags.FrameIncrementPointer));
                Assert.AreEqual(0, attr.Count);
                Assert.AreEqual(0, attr.StreamLength);

                return attr;
            }

            public void TestConstructors()
            {
                
                try
                {
                    DicomAttributeAT attrib = new DicomAttributeAT(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statment should throw exception: invalid tag");
                }
                catch (DicomException)
                {
                    
                }

                CreateAttribute();
                
            }
            /// <summary>
            /// Test Set methods.
            /// Note: To make the test useful and more atomic, the only thing we should only care when calling Set is 
            /// the number of value (Count) is accurate. 
            /// 
            /// 
            /// </summary>
            public void TestSet()
            {
                DicomAttributeAT attrib;
                #region Set from binary


                #region Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetInt16(1, 2);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


                // Special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // Special case: negative number
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(0, -1);
                    Assert.Fail("Above statement should throw exception: AT tag can't store negative numbers");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                
                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetUInt16(1, 2);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


                // Special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }




                #endregion

                #region Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetInt32(1, 1);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


                // Special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // Special case: negative number
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(0, -1);
                    Assert.Fail("Above statement should throw exception: AT tag can't store negative numbers");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                #endregion

                #region UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetUInt32(1, 2);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


                // Special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }




                #endregion

                #region Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetInt64(1, 1);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


                // Special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // Special case: negative number
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(0, -1);
                    Assert.Fail("Above statement should throw exception: AT tag can't store negative numbers");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                #endregion

                #region UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetUInt64(1, 2);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


                // Special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // Special case: range
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, UInt64.MaxValue);
                    Assert.Fail("Above statement should throw exception: invalid value");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }




                #endregion

                
                #endregion

                #region Set from string

                attrib = CreateAttribute();
                attrib.SetString(0, "10002000");
                Assert.IsTrue(attrib.Count == 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.SetString(1, "20002000");
                Assert.IsTrue(attrib.Count == 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                attrib.SetStringValue("10002000\\23001100");
                Assert.IsTrue(attrib.Count == 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                // special case: invalid tag
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "-10002000");
                }
                catch (DicomDataException e)
                {
                    Assert.IsTrue(attrib.Count == 0);
                    Assert.IsTrue(attrib.StreamLength == 0);

                }
                


                #endregion

            }
            
            /// <summary>
            /// Test Append methods.
            /// 
            /// </summary>
            public void TestAppend()
            {

                DicomAttributeAT attrib;

                #region Append binary values

                #region Append Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(1000);
                Assert.AreEqual(attrib.Count , 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendInt16(2000);
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                // special case : negative value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt16(-1000);
                    Assert.Fail("Above statement should throw exception: invalid value");

                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }
                #endregion

                #region Append UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendUInt16(2000);
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);
                #endregion


                #region Append Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendInt32(2000);
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                // special case : negative value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt32(-1000);
                    Assert.Fail("Above statement should throw exception: invalid value");

                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }
                #endregion

                #region Append UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendUInt32(2000);
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);
                #endregion

                #region Append Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendInt64(2000);
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                // special case : negative value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt64(-1000);
                    Assert.Fail("Above statement should throw exception: invalid value");

                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }

                // special case : large value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt64(Int64.MaxValue);
                    Assert.Fail("Above statement should throw exception: invalid value");

                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }
                #endregion

                #region Append UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendUInt64(2000);
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                // special case : large value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt64(UInt64.MaxValue);
                    Assert.Fail("Above statement should throw exception: invalid value");

                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }
                #endregion


                #endregion

                #region Append string
                attrib = CreateAttribute();
                attrib.AppendString("00100020");
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.StreamLength == 4);

                attrib.AppendString("20100020");
                Assert.AreEqual(attrib.Count, 2);
                Assert.IsTrue(attrib.StreamLength == 8);

                // special case: append non numeral string
                attrib = CreateAttribute();
                try
                {

                    attrib.AppendString("Something");
                    Assert.Fail("Above statment should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }
                
                // special case: append null
                attrib = CreateAttribute();
                try
                {
                    
                    attrib.AppendString(null);
                    Assert.Fail("Above statment should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }

                // special case: append null
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("");
                    Assert.Fail("Above statment should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(attrib.Count, 0);
                    Assert.IsTrue(attrib.StreamLength == 0);
                }
                
                #endregion

            }
            /// <summary>
            /// Test Get methods.
            ///
            /// </summary>
            public void TestGet()
            {
                
                DicomAttributeAT attrib;

                #region Get binary values

                #region Get Int16
                Int16 valueInt16;
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16));
                Assert.AreEqual(0x100, valueInt16);
                Assert.AreEqual(0x100, attrib.GetInt16(0, 0));

                Assert.IsTrue(attrib.TryGetInt16(2, out valueInt16)==false);
                Assert.AreEqual(0x0, attrib.GetInt16(2, 0x0));
                

                // Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt16(10, out valueInt16) == false);
                //Assert.AreEqual(0, valueInt16);
                Assert.AreEqual(-1, attrib.GetInt16(10, -1));
                
                // special case: cannot fit into destination
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt16(2, out valueInt16) == false);
                //Assert.AreEqual(0, valueInt16);
                Assert.AreEqual(-1, attrib.GetInt16(2, -1));
                
                #endregion

                #region Get UInt16
                UInt16 valueUInt16;
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16));
                Assert.AreEqual(0x100, valueUInt16);
                Assert.AreEqual(0x100, attrib.GetUInt16(0, 0));

                Assert.IsTrue(attrib.TryGetUInt16(2, out valueUInt16) == false);
                Assert.AreEqual(0x0, attrib.GetUInt16(2, 0x0));
                

                // Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetUInt16(10, out valueUInt16) == false);
                //Assert.AreEqual(0, valueUInt16);
                Assert.AreEqual(99, attrib.GetUInt16(10, 99));

                // special case: cannot fit into destination
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt16(2, out valueUInt16) == false);
                //Assert.AreEqual(0, valueUInt16);
                Assert.AreEqual(99, attrib.GetUInt16(2, 99));

                #endregion

                #region Get Int32
                Int32 valueInt32;
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\87fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32));
                Assert.AreEqual(0x100, valueInt32);
                Assert.AreEqual(0x100, attrib.GetInt32(0, 0));


                // Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt32(10, out valueInt32) == false);
                //Assert.AreEqual(0, valueInt32);
                Assert.AreEqual(-1, attrib.GetInt32(10, -1));

                // special case: cannot fit into destination
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\FFFFFFFE");
                Assert.AreEqual(3, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt32(2, out valueInt32) == false);
                //Assert.AreEqual(0, valueInt32);
                Assert.AreEqual(-1, attrib.GetInt32(2, -1));

                #endregion

                #region Get UInt32
                UInt32 valueUInt32;
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32));
                Assert.AreEqual(0x100, valueUInt32);
                Assert.AreEqual(0x100, attrib.GetUInt32(0, 0));

                Assert.IsTrue(attrib.TryGetUInt32(2, out valueUInt32));
                Assert.AreEqual(0x07fe0010, attrib.GetUInt32(2, 0x0));
                

                // Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetUInt32(10, out valueUInt32) == false);
                //Assert.AreEqual(0, valueUInt32);
                Assert.AreEqual(99, attrib.GetUInt32(10, 99));

                // special case: cannot fit into destination
                // IMPOSSIBLE!

                #endregion

                #region Get Int64
                Int64 valueInt64;
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\07fe0010");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64));
                Assert.AreEqual(0x100, valueInt64);
                Assert.AreEqual(0x100, attrib.GetInt64(0, 0));



                // Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\fffeffe0");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt64(10, out valueInt64) == false);
                //Assert.AreEqual(0, valueInt64);
                Assert.AreEqual(-1, attrib.GetInt64(10, -1));

                // special case: cannot fit into destination
                // IMPOSSIBLE DUE TO IMPLEMENATION OF SetXXXX()

                #endregion

                #region Get UInt64
                UInt64 valueUInt64;
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\fffeffe0");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64));
                Assert.AreEqual(0x100, valueUInt64);
                Assert.AreEqual(0x100, attrib.GetUInt64(0, 0));



                // Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("100\\100000\\fffeffe0");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetUInt64(10, out valueUInt64) == false);
                //Assert.AreEqual(0, valueUInt64);
                Assert.AreEqual(99, attrib.GetUInt64(10, 99));

                // special case: cannot fit into destination
                // IMPOSSIBLE DUE TO IMPLEMENATION OF SetXXXX()

                #endregion

                #endregion

                #region Get string

                attrib = CreateAttribute();
                attrib.SetStringValue("10001010\\fffeffe0");
                Assert.AreEqual(2, attrib.Count);
                
                Assert.AreEqual("10001010", attrib.GetString(0, ""));
                Assert.AreEqual("FFFEFFE0", attrib.GetString(1, ""));
                
                // Special case: invalid index
                // Non-existing element: Get returns default value
                Assert.AreEqual("", attrib.GetString(10, ""));

                // Special case: leadig/trailing spaces when set
                // underlying is not string so spaces should not be preserved
                attrib = CreateAttribute();
                attrib.SetStringValue(" 10001010 \\ fffeffe0 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("10001010", attrib.GetString(0, "")); 
                Assert.AreEqual("FFFEFFE0", attrib.GetString(1, ""));

                #endregion               
            }
        }
        #endregion

        #region DicomAttributeCS Test
        [Test]
        public void AttributeCSTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeCS attrib = new DicomAttributeCS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            try
            {
                DicomAttributeCS attrib = new DicomAttributeCS(DicomTagDictionary.GetDicomTag(DicomTags.ImageType));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region DicomAttributeDA Test
        [Test]
        public void AttributeDATest()
        {
            DicomImplementation.UnitTest = true;

            AttributeDATestSuite test = new AttributeDATestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
            

        }
        private class AttributeDATestSuite
        {
            public DicomAttributeDA CreateAttribute()
            {
                DicomAttributeDA attrib = new DicomAttributeDA(DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepEndDate));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }
            public void TestConstructors()
            {
                try
                {
                    DicomAttributeDA attrib = new DicomAttributeDA(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {
                    
                }

                CreateAttribute();
                    
            }
            public void TestSet()
            {
                DicomImplementation.UnitTest = true;

                DicomAttributeDA attrib;
                #region Set from string

                attrib = CreateAttribute();
                attrib.SetStringValue("20001012");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.StreamLength == "20001012".Length);


                attrib.SetStringValue("20001012\\20910214");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.StreamLength == "20001012\\20910294".Length + 1);

                attrib = CreateAttribute(); 
                attrib.SetString(0, "20001012");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.StreamLength == "20001012".Length);

                attrib.SetString(1, "20001013");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.StreamLength == "20001012\\20910294".Length + 1);
                
                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "something");
                    Assert.Fail("Above statement should fail: invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid date
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "20071240");
                    Assert.Fail("Above statement should fail: invalid date");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: set null
                attrib = CreateAttribute();
                attrib.SetString(0, null);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                attrib.SetString(1, null);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength); // two empty values, separated by "\" and padded to even length


                // special case: set ""
                attrib = CreateAttribute();
                attrib.SetString(0, "");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                attrib.SetString(1, "");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength); // two empty values, separated by "\" and padded to even length
                
                
                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "20001012");
                    Assert.Fail("Above statement should fail: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                

                #endregion

                #region set from date time
				
                attrib = new DicomAttributeDA(DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepEndDate));
				DateTime d1 = DateTime.Parse("2/16/1992", CultureInfo);
                attrib.SetDateTime(0, d1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(DateParser.DicomDateFormat.Length, attrib.StreamLength);

				DateTime d2 = DateTime.Parse("12/02/2000", CultureInfo); 
                attrib.SetDateTime(1, d2);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(DateParser.DicomDateFormat.Length * 2 + "\\".Length + 1, attrib.StreamLength);

                
                
                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeDA attrib;
                #region Append from string
                attrib = CreateAttribute();
                attrib.AppendString("20010804");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(DateParser.DicomDateFormat.Length, attrib.StreamLength);
                
                attrib.AppendString("20010303");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(DateParser.DicomDateFormat.Length * 2 + "\\".Length + 1, attrib.StreamLength);

                //Special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("something");
                    Assert.Fail("Above statement should throw exception: invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                //Special case: invalid date
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("20010232");
                    Assert.Fail("Above statement should throw exception: invalid date");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: append null
                attrib = CreateAttribute();
                attrib.AppendString(null); 
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                // special case: append ""
                attrib = CreateAttribute();
                attrib.AppendString("");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                #endregion

                #region append date time
				DateTime d2 = DateTime.Parse("12/02/2000", CultureInfo);
                attrib = CreateAttribute();
                attrib.AppendDateTime(d2);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(DateParser.DicomDateFormat.Length, attrib.StreamLength);
                #endregion
            }
            

            public void TestGet()
            {
                DicomAttributeDA attrib;
				
                #region Get to string
                string stringValue;
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012\\20190502");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(attrib.GetString(0, ""), "20001012");
                Assert.AreEqual(attrib.GetString(1, ""), "20190502");

                // special case: invalid index
                Assert.AreEqual("", attrib.GetString(10, ""));
                Assert.IsTrue(attrib.TryGetString(10, out stringValue)==false);

                // special case : leading/trailing bytes when set
                attrib = CreateAttribute();
                attrib.SetStringValue(" 20001012 \\ 20190502 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(attrib.GetString(0, ""), " 20001012 "); // trailing/leading spaces are not removed
                Assert.AreEqual(attrib.GetString(1, ""), " 20190502 ");


                #endregion

                #region Get to datetime
                DateTime dt = new DateTime();
                attrib = CreateAttribute();
                attrib.SetString(0, "20001012");
                attrib.SetString(1, "20190502");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("20001012", attrib.GetDateTime(0, dt).ToString("yyyyMMdd"));
                Assert.AreEqual("20190502", attrib.GetDateTime(1, dt).ToString("yyyyMMdd"));

                attrib = CreateAttribute();
                attrib.SetStringValue("20001012\\20190502");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("20001012", attrib.GetDateTime(0, dt).ToString("yyyyMMdd"));
                Assert.AreEqual("20190502", attrib.GetDateTime(1, dt).ToString("yyyyMMdd"));

                attrib = CreateAttribute();
				attrib.SetDateTime(0, DateTime.Parse("10/12/2000", CultureInfo));
				attrib.SetDateTime(1, DateTime.Parse("05/02/2019", CultureInfo));
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("20001012", attrib.GetDateTime(0, dt).ToString("yyyyMMdd"));
                Assert.AreEqual("20190502", attrib.GetDateTime(1, dt).ToString("yyyyMMdd"));


                // special case: invalid index
                attrib = CreateAttribute();
				attrib.SetDateTime(0, DateTime.Parse("10/12/2000", CultureInfo));
                Assert.AreEqual("", attrib.GetString(10, ""));
                Assert.IsTrue(attrib.TryGetString(10, out stringValue) == false);

                // special case : leading/trailing bytes when set
                attrib = CreateAttribute();
                attrib.SetStringValue(" 20001012 \\ 20190502 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("20001012", attrib.GetDateTime(0, dt).ToString("yyyyMMdd"));
                Assert.AreEqual("20190502", attrib.GetDateTime(1, dt).ToString("yyyyMMdd"));

				// special case: get empty, no values
            	string stringVal;
				DateTime dateTimeValue;
				attrib = CreateAttribute();
				Assert.AreEqual(0, attrib.Count);
				Assert.IsTrue(attrib.TryGetString(1, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(1, out dateTimeValue) == false);
				Assert.AreEqual(dateTimeValue, attrib.GetDateTime(1, dateTimeValue));

				// special case: get empty, null value
				attrib = CreateAttribute();
				attrib.SetNullValue();
				Assert.AreEqual(1, attrib.Count);
				Assert.IsTrue(attrib.TryGetString(1, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetString(0, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(1, out dateTimeValue) == false);
				Assert.IsTrue(attrib.TryGetDateTime(0, out dateTimeValue) == false);
				Assert.AreEqual(dateTimeValue, attrib.GetDateTime(1, dateTimeValue));
				Assert.AreEqual(dateTimeValue, attrib.GetDateTime(0, dateTimeValue));

                #endregion
            }

        }
        #endregion

        #region DicomAttributeDT Test
        [Test]
        public void AttributeDTTest()
        {
            AttributeDTTestSuite test = new AttributeDTTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();

        }
        private class AttributeDTTestSuite
        {
            public DicomAttributeDT CreateAttribute()
            {
                DicomAttributeDT attrib = null;
                attrib = new DicomAttributeDT(DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepStartDatetime));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
                    
            }
            public void TestConstructors()
            {
                DicomAttributeDT attrib =null;
                try
                {
                    attrib = new DicomAttributeDT(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {
                    
                }

                CreateAttribute();
                
            }
            public void TestSet()
            {
                DicomAttributeDT attrib;
                #region set string
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013.123456");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("20001012120013.123456".Length + 1, attrib.StreamLength); // padded

                attrib.SetStringValue("20001012122213.123456\\20001012120013.123456");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("20001012122213.123456\\20001012120013.123456".Length+1, attrib.StreamLength); // padded

                // special case: omitted components
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("20001012120013".Length, attrib.StreamLength); 

                attrib = CreateAttribute();
                attrib.SetStringValue("20001012");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("20001012".Length, attrib.StreamLength);

                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("2000");
                }
                catch (DicomDataException e)
                {
                    Assert.Fail("Above statement should succeed; Date/Time values can have trailing null components except for the year.");
                }

                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("200");
                    Assert.Fail("Above statement should fail; Date/Time values can have trailing null components except for the year.");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: null value
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013\\\\20001012");
                Assert.AreEqual(3, attrib.Count);
                Assert.AreEqual("20001012120013\\\\20001012".Length, attrib.StreamLength);

                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("something");
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid date
                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("20011232100000.000000");
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }



                #endregion

                #region Set datetime
				
                attrib = CreateAttribute();
				DateTime d1 = DateTime.Parse("2/16/1992 12:15:12", CultureInfo);
                attrib.SetDateTime(0, d1);
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0); // we'll test the value in TestGet()

				DateTime d2 = DateTime.Parse("12/02/2000 12:15:12", CultureInfo);
                attrib.SetDateTime(1, d2);
                Assert.AreEqual(2, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0); // we'll test the value in TestGet()

                // special case: invalid index
                attrib = CreateAttribute();
				d1 = DateTime.Parse("2/16/1992 12:15:12", CultureInfo);
                try
                {
                    attrib.SetDateTime(10, d1);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength); // we'll test the value in TestGet()
                }

                // special case: setDatetime(null)
                // Compiler does not allow this

                #endregion


            }
            public void TestAppend()
            {
                DicomAttributeDT attrib;

                #region append string
                attrib = CreateAttribute();
                attrib.AppendString("20001012120013.12220");
                Assert.AreEqual(1, attrib.Count);
                
                // special case: omitted components
                attrib = CreateAttribute();
                attrib.AppendString("20001012120013");
                Assert.AreEqual(1, attrib.Count);
                uint x = attrib.StreamLength;
                Assert.Greater(x, 0);

                // special case: invalid format
                try
                {
                    attrib.AppendString("something");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count); // still 2
                    Assert.AreEqual(x, attrib.StreamLength);
                }

                // special case: invalid date
                try
                {
                    attrib.AppendString("20001032120013");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count); // still 2
                    Assert.AreEqual(x, attrib.StreamLength);
                }

                // special case: append null
                attrib.AppendString(null);
                Assert.AreEqual(2, attrib.Count);

                // special case: append ""
                attrib.AppendString("");
                Assert.AreEqual(3, attrib.Count); 
               
                #endregion

                #region append datetime
                attrib = CreateAttribute();
                attrib.AppendDateTime(new DateTime());
                Assert.IsTrue(attrib.Count == 1);
                attrib.AppendDateTime(new DateTime());
                Assert.IsTrue(attrib.Count == 2);

                #endregion

            }

            public void TestGet()
            {
                DicomAttributeDT attrib;
                #region Get string
                string stringVal;
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013.200000\\19991130125000");
                Assert.AreEqual(2, attrib.Count);
                // string are stored as is
                Assert.IsTrue( attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("20001012120013.200000", stringVal);
                Assert.AreEqual(attrib.GetString(0, ""), "20001012120013.200000");
                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("19991130125000", stringVal);
                Assert.AreEqual(attrib.GetString(1, ""), "19991130125000");

                attrib = CreateAttribute();
                attrib.SetString(0, "20001012120013.200000");
                attrib.SetString(1, "19991130125000");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("20001012120013.200000", stringVal);
                Assert.AreEqual(attrib.GetString(0, ""), "20001012120013.200000");
                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("19991130125000", stringVal);
                Assert.AreEqual(attrib.GetString(1, ""), "19991130125000");

                
                attrib = CreateAttribute();
				attrib.SetDateTime(0, DateTime.Parse("08/20/2001 12:15:12", CultureInfo));
                // datetime are converted to string
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("20010820121512.000000", stringVal);
                Assert.AreEqual("20010820121512.000000", attrib.GetString(0, ""));// NOTE: the format can be changed

                //Special case: invalid index
                attrib = CreateAttribute();
				attrib.SetDateTime(0, DateTime.Parse("08/20/2001 12:15:12", CultureInfo));
                Assert.IsTrue(attrib.TryGetString(10, out stringVal) == false);
                Assert.AreEqual("", attrib.GetString(10, ""));

                // special case: get empty
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013.200000\\\\19991130125000");
                Assert.AreEqual(3, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(1, out stringVal) == true);
                Assert.AreEqual("", stringVal);
                Assert.AreEqual("", attrib.GetString(1, ""));

                #endregion

                #region Get datetime
                DateTime dtVal;
                attrib = CreateAttribute();
                attrib.SetString(0, "20001012120013.200000");
                attrib.SetString(1, "19991130125000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("20001012120013.200000", attrib.GetDateTime(0, new DateTime()).ToString("yyyyMMddHHmmss.ffffff"));
                Assert.IsTrue(attrib.TryGetDateTime(0, out dtVal));
                Assert.AreEqual("20001012120013.200000", dtVal.ToString("yyyyMMddHHmmss.ffffff"));
                
                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013.200000\\\\19991130125000");
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetDateTime(10, out dtVal) == false);
                dtVal = new DateTime();
                Assert.AreEqual(dtVal, attrib.GetDateTime(10, dtVal));


                // special case: get empty
                attrib = CreateAttribute();
                attrib.SetStringValue("20001012120013.200000\\\\19991130125000");
                Assert.AreEqual(3, attrib.Count);
                Assert.IsTrue(attrib.TryGetDateTime(1, out dtVal) == false);
                dtVal = new DateTime();
                Assert.AreEqual(dtVal, attrib.GetDateTime(1, dtVal));


				// special case: get empty, no values
				DateTime dateTimeValue;
				attrib = CreateAttribute();
				Assert.AreEqual(0, attrib.Count);
				Assert.IsTrue(attrib.TryGetString(1, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(1, out dateTimeValue) == false);
				Assert.AreEqual(dateTimeValue, attrib.GetDateTime(1, dateTimeValue));

				// special case: get empty, null value
				attrib = CreateAttribute();
            	attrib.SetNullValue();
				Assert.AreEqual(1, attrib.Count);
				Assert.IsTrue(attrib.TryGetString(1, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetString(0, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(1, out dateTimeValue) == false);
				Assert.IsTrue(attrib.TryGetDateTime(0, out dateTimeValue) == false);
				Assert.AreEqual(dateTimeValue, attrib.GetDateTime(1, dateTimeValue));
				Assert.AreEqual(dateTimeValue, attrib.GetDateTime(0, dateTimeValue));


                #endregion

            }
        }
        #endregion

        #region DicomAttributeDS Test
        [Test]
        public void AttributeDSTest()
        {
			DicomImplementation.UnitTest = true;

            AttributeDSTestSuite test = new AttributeDSTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend(); 
            test.TestGet();
            test.TestExtremeValues();

        }
        private class AttributeDSTestSuite
        {
            public DicomAttributeDS CreateAttribute()
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.WindowCenter));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;    
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {
                    
                }
                
            }
            /// <summary>
            /// Test Set methods.
             /// </summary>
            public void TestSet()
            {
                DicomAttributeDS attrib;

                #region Set numbers

                #region Set Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetInt16(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }
                
                #endregion

                #region Set Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetInt32(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetInt64(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetUInt16(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetUInt32(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetUInt64(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #endregion

                #region Set String
                attrib = CreateAttribute();
                attrib.SetString(0, "1000");
                Assert.IsTrue(attrib.Count==1);

                attrib.SetString(0, "2000");
                Assert.IsTrue(attrib.Count== 1);


                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\2000");
                Assert.IsTrue(attrib.Count== 2);

                attrib = CreateAttribute();
                attrib.SetStringValue(
                    @"0.0\6.500000e+02\\6.649999e+02\6.700001e+02\6.649998e+02\6.649999e+02");
                Assert.IsTrue(attrib.Count == 7);

                // special case : invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "1000");
                    Assert.Fail("above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "somenumber");
                    Assert.Fail("above statement should throw exception: invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("1000\\somenumber");
                    Assert.Fail("above statement should throw exception: invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // special case: set null
                attrib = CreateAttribute();
                attrib.SetString(0, null);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                attrib.SetString(1, "");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: null value in the list
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\\\2000");
                Assert.AreEqual(3, attrib.Count);
                Assert.AreEqual("1000\\\\2000".Length, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetString(0, "10.000244140625");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(16, attrib.StreamLength);
               
                #endregion

            }
            /// <summary>
            /// Test Append methods.
            /// 
            /// </summary>
            public void TestAppend()
            {
                DicomAttributeDS attrib;
                #region Append numbers

                #region Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion

                #region Float
                attrib = CreateAttribute();
                attrib.AppendFloat32(1000.0f);
                Assert.AreEqual(1, attrib.Count);
                #endregion

                #region double
                attrib = CreateAttribute();
                attrib.AppendFloat64(1000.0f);
                Assert.AreEqual(1, attrib.Count);
                #endregion

                #endregion

                #region Append String
                attrib = CreateAttribute();
                attrib.AppendString("1000");
                Assert.AreEqual(1, attrib.Count);

                attrib.AppendString("2000");
                Assert.AreEqual(2, attrib.Count);

                attrib.AppendString("2000.0");
                Assert.AreEqual(3, attrib.Count);

                attrib.AppendString("2.0E-10");
                Assert.AreEqual(4, attrib.Count);


                // special case: leading/trailing spaces
                // Underyling data is string, spaces should be preserved
                attrib = CreateAttribute();
                attrib.AppendString(" 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(" 1000 ".Length, attrib.StreamLength);


                // special case: append null
                attrib = CreateAttribute();
                attrib.AppendString(null);
                Assert.AreEqual(1, attrib.Count );
                Assert.AreEqual(0, attrib.StreamLength);


                // special case: append ""
                attrib = CreateAttribute();
                attrib.AppendString("");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                // special case : append invalid format value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("somevalue");
                    Assert.Fail("Above statement should throw exception: invalid value");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }



                #endregion
            }
            /// <summary>
            /// Test Get methods.
            ///
            /// </summary>
            public void TestGet()
            {
                DicomAttributeDS attrib ;
                
                #region GetInt16
                Int16 valueInt16;
                attrib = CreateAttribute();
                attrib.SetInt16(0, 1000);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16));
                Assert.AreEqual(1000, valueInt16);
                Assert.AreEqual(1000, attrib.GetInt16(0, 0));
 
                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16) == false);
                Assert.AreEqual(0, attrib.GetInt16(0, 0));

                // special case: float to Int16
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16) == false);
                Assert.AreEqual(0, attrib.GetInt16(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetInt16(10, out valueInt16) == false);
                Assert.AreEqual(0, attrib.GetInt16(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16));
                Assert.AreEqual(1000, valueInt16);
                Assert.AreEqual(1000, attrib.GetInt16(0, 0));
 
                #endregion

                #region GetInt32
                Int32 valueInt32;
                attrib = CreateAttribute();
                attrib.SetInt32(0, 1000);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32));
                Assert.AreEqual(1000, valueInt32);
                Assert.AreEqual(1000, attrib.GetInt32(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32) == false);
                Assert.AreEqual(0, attrib.GetInt32(0, 0));

                // special case: float to Int32
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32) == false);
                Assert.AreEqual(0, attrib.GetInt32(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetInt32(10, out valueInt32) == false);
                Assert.AreEqual(0, attrib.GetInt32(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32));
                Assert.AreEqual(1000, valueInt32);
                Assert.AreEqual(1000, attrib.GetInt32(0, 0));

                #endregion

                #region GetInt64
                Int64 valueInt64;
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == true);
                Assert.AreEqual(1000, valueInt64);
                Assert.AreEqual(1000, attrib.GetInt64(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == false);
                Assert.AreEqual(0, attrib.GetInt64(0, 0));

                // special case: float to Int64
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == false);
                Assert.AreEqual(0, attrib.GetInt64(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetInt64(10, out valueInt64) == false);
                Assert.AreEqual(0, attrib.GetInt64(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == true);
                Assert.AreEqual(1000, valueInt64);
                Assert.AreEqual(1000, attrib.GetInt64(0, 0));

                #endregion

                #region GetUInt16
                UInt16 valueUInt16;
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == true);
                Assert.AreEqual(1000, valueUInt16);
                Assert.AreEqual(1000, attrib.GetUInt16(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == false);
                Assert.AreEqual(0, attrib.GetUInt16(0, 0));

                // special case: float to UInt16
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == false);
                Assert.AreEqual(0, attrib.GetUInt16(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetUInt16(10, out valueUInt16) == false);
                Assert.AreEqual(0, attrib.GetUInt16(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16));
                Assert.AreEqual(1000, valueUInt16);
                Assert.AreEqual(1000, attrib.GetUInt16(0, 0));

                #endregion

                #region GetUInt32
                UInt32 valueUInt32;
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32));
                Assert.AreEqual(1000, valueUInt32);
                Assert.AreEqual(1000, attrib.GetUInt32(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == false);
                Assert.AreEqual(0, attrib.GetUInt32(0, 0));

                // special case: float to UInt32
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == false);
                Assert.AreEqual(0, attrib.GetUInt32(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetUInt32(10, out valueUInt32) == false);
                Assert.AreEqual(0, attrib.GetUInt32(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == true);
                Assert.AreEqual(1000, valueUInt32);
                Assert.AreEqual(1000, attrib.GetUInt32(0, 0));

                #endregion

                #region GetUInt64
                UInt64 valueUInt64;
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64) == true);
                Assert.AreEqual(1000, valueUInt64);
                Assert.AreEqual(1000, attrib.GetUInt64(0, 0));

                // special case: too big to fit
                // N/A

                // special case: float to UInt64
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64) == false);
                Assert.AreEqual(0, attrib.GetUInt64(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1000.50f);
                Assert.IsTrue(attrib.TryGetUInt64(10, out valueUInt64) == false);
                Assert.AreEqual(0, attrib.GetUInt64(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64) == true);
                Assert.AreEqual(1000, valueUInt64);
                Assert.AreEqual(1000, attrib.GetUInt64(0, 0));

                #endregion

                #region GetFloat
                float valueFloat;
                double valueDouble;
                attrib = CreateAttribute();
                attrib.SetInt16(0,-1000);
                attrib.SetFloat32(1, 2000.0f);
                attrib.SetFloat64(2, 3000.0d);
                Assert.AreEqual(3, attrib.Count);
                
                Assert.IsTrue(attrib.TryGetFloat32(0, out valueFloat) == true);
                Assert.IsTrue(attrib.TryGetFloat64(0, out valueDouble) == true);
                Assert.AreEqual(-1000.0f, attrib.GetFloat32(0, 0.0f));
                Assert.AreEqual(2000.0f, attrib.GetFloat32(1, 0.0f));
                Assert.AreEqual(3000.0f, attrib.GetFloat32(2, 0.0f));

                Assert.AreEqual(-1000.0f, attrib.GetFloat64(0, 0.0f));
                Assert.AreEqual(2000.0f, attrib.GetFloat64(1, 0.0f));
                Assert.AreEqual(3000.0f, attrib.GetFloat64(2, 0.0f));
       

                #endregion

                #region GetString
                attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.WindowCenter));
                attrib.AppendFloat32(1.239f);
                attrib.AppendUInt16(1900);
                attrib.AppendInt16(-120);
                Assert.AreEqual(3, attrib.Count);

				//TODO: this test now fails because the value is not reversible.
				//Assert.AreEqual( "1.239", attrib.GetString(0, "")); 
                Assert.AreEqual("1900", attrib.GetString(1, ""));
                Assert.AreEqual("-120", attrib.GetString(2, ""));

                Assert.AreEqual("", attrib.GetString(10, ""));


                // special case: leading/trailing spaces
                // Underyling data is string, spaces should be preserved 
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(" 1000 ", attrib.GetString(0, ""));
                Assert.AreEqual(" 1000 ", attrib.GetString(0, ""));

                #endregion

            }

            /// <summary>
            /// Test large DS values 
            /// </summary>
            public void TestExtremeValues()
            {
                DicomAttributeDS attrib = CreateAttribute();
                attrib.SetString(0, "1.79769313E+308");
                attrib.SetString(0, "-1.79769313E+308"); 

                attrib.SetString(0, "-1.79769313E-308"); 
                attrib.SetString(0, "-1.79769313E-308"); 
            }
        }
        #endregion

        #region DicomAttributeIS Test
        [Test]
        public void AttributeISTest()
        {
            AttributeISTestSuite test = new AttributeISTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();


        }
        private class AttributeISTestSuite
        {
            public DicomAttributeIS CreateAttribute()
            {
                DicomAttributeIS attrib = new DicomAttributeIS(DicomTagDictionary.GetDicomTag(DicomTags.WedgeNumber));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeIS attrib = new DicomAttributeIS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {

                }

            }
            /// <summary>
            /// Test Set methods.
            /// </summary>
            public void TestSet()
            {
                DicomAttributeIS attrib;

                #region Set numbers

                #region Set Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetInt16(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetInt32(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetInt64(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetUInt16(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetUInt32(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 1000);
                Assert.AreEqual(attrib.Count, 1);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetUInt64(1, 2000);
                Assert.AreEqual(attrib.Count, 2);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 1000);
                    Assert.Fail("above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #endregion

                #region Set String
                attrib = CreateAttribute();
                attrib.SetString(0, "1000");
                Assert.IsTrue(attrib.Count == 1);

                attrib.SetString(0, "2000");
                Assert.IsTrue(attrib.Count == 1);


                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\2000");
                Assert.IsTrue(attrib.Count == 2);

                // special case : invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "1000");
                    Assert.Fail("above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "somenumber");
                    Assert.Fail("above statement should throw exception: invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("1000\\somenumber");
                    Assert.Fail("above statement should throw exception: invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }


                // special case: set null
                attrib = CreateAttribute();
                attrib.SetString(0, null);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                attrib.SetString(1, "");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: null value in the list
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\\\2000");
                Assert.AreEqual(3, attrib.Count);
                Assert.AreEqual("1000\\\\2000".Length, attrib.StreamLength);


                #endregion

            }
            /// <summary>
            /// Test Append methods.
            /// 
            /// </summary>
            public void TestAppend()
            {
                DicomAttributeIS attrib;
                #region Append numbers

                #region Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion
                #region UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                #endregion

                #region Float
                //attrib = CreateAttribute();
                //attrib.AppendFloat32(1000.0f);
                //Assert.AreEqual(1, attrib.Count);
                #endregion

                #region double
                //attrib = CreateAttribute();
                //attrib.AppendFloat64(1000.0f);
                //Assert.AreEqual(1, attrib.Count);
                #endregion

                #endregion

                #region Append String
                attrib = CreateAttribute();
                attrib.AppendString("1000");
                Assert.AreEqual(1, attrib.Count);

                attrib.AppendString("2000");
                Assert.AreEqual(2, attrib.Count);



                // special case: leading/trailing spaces
                // Underyling data is string, spaces should be preserved
                attrib = CreateAttribute();
                attrib.AppendString(" 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(" 1000 ".Length, attrib.StreamLength);


                // special case: append null
                attrib = CreateAttribute();
                attrib.AppendString(null);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                // special case: append ""
                attrib = CreateAttribute();
                attrib.AppendString("");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                // special case : append invalid format value
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("somevalue");
                    Assert.Fail("Above statement should throw exception: invalid value");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }



                #endregion
            }
            /// <summary>
            /// Test Get methods.
            ///
            /// </summary>
            public void TestGet()
            {
                DicomAttributeIS attrib;

                #region GetInt16
                Int16 valueInt16;
                attrib = CreateAttribute();
                attrib.SetInt16(0, 1000);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16) == true);
                Assert.AreEqual(1000, valueInt16);
                Assert.AreEqual(1000, attrib.GetInt16(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16) == false);
                Assert.AreEqual(0, attrib.GetInt16(0, 0));

                // special case: float to Int16
                //attrib = CreateAttribute();
                //attrib.SetFloat64(0, 1000.50f);
                //Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16) == false);
                //Assert.AreEqual(0, attrib.GetInt16(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetInt16(10, out valueInt16) == false);
                Assert.AreEqual(0, attrib.GetInt16(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt16(0, out valueInt16));
                Assert.AreEqual(1000, valueInt16);
                Assert.AreEqual(1000, attrib.GetInt16(0, 0));

                #endregion

                #region GetInt32
                Int32 valueInt32;
                attrib = CreateAttribute();
                attrib.SetInt32(0, 1000);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32) == true);
                Assert.AreEqual(1000, valueInt32);
                Assert.AreEqual(1000, attrib.GetInt32(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32) == false);
                Assert.AreEqual(0, attrib.GetInt32(0, 0));

                // special case: float to Int32
               //attrib = CreateAttribute();
                //attrib.SetFloat64(0, 1000.50f);
                //Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32) == false);
                //Assert.AreEqual(0, attrib.GetInt32(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetInt32(10, out valueInt32) == false);
                Assert.AreEqual(0, attrib.GetInt32(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt32(0, out valueInt32) == true);
                Assert.AreEqual(1000, valueInt32);
                Assert.AreEqual(1000, attrib.GetInt32(0, 0));

                #endregion

                #region GetInt64
                Int64 valueInt64;
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == true);
                Assert.AreEqual(1000, valueInt64);
                Assert.AreEqual(1000, attrib.GetInt64(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == false);
                Assert.AreEqual(0, attrib.GetInt64(0, 0));

                // special case: float to Int64
                //attrib = CreateAttribute();
                //attrib.SetFloat64(0, 1000.50f);
                //Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == false);
                //Assert.AreEqual(0, attrib.GetInt64(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetInt64(10, out valueInt64) == false);
                Assert.AreEqual(0, attrib.GetInt64(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetInt64(0, out valueInt64) == true);
                Assert.AreEqual(1000, valueInt64);
                Assert.AreEqual(1000, attrib.GetInt64(0, 0));

                #endregion

                #region GetUInt16
                UInt16 valueUInt16;
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == true);
                Assert.AreEqual(1000, valueUInt16);
                Assert.AreEqual(1000, attrib.GetUInt16(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == false);
                Assert.AreEqual(0, attrib.GetUInt16(0, 0));

                // special case: float to UInt16
                //attrib = CreateAttribute();
                //attrib.SetFloat64(0, 1000.50f);
                //Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == false);
                //Assert.AreEqual(0, attrib.GetUInt16(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt16(10, out valueUInt16) == false);
                Assert.AreEqual(0, attrib.GetUInt16(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt16(0, out valueUInt16) == true);
                Assert.AreEqual(1000, valueUInt16);
                Assert.AreEqual(1000, attrib.GetUInt16(0, 0));

                #endregion

                #region GetUInt32
                UInt32 valueUInt32;
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == true);
                Assert.AreEqual(1000, valueUInt32);
                Assert.AreEqual(1000, attrib.GetUInt32(0, 0));

                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.SetUInt64(0, UInt64.MaxValue);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == false);
                Assert.AreEqual(0, attrib.GetUInt32(0, 0));

                // special case: float to UInt32
                //attrib = CreateAttribute();
                //attrib.SetFloat64(0, 1000.50f);
                //Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == false);
                //Assert.AreEqual(0, attrib.GetUInt32(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt32(10, out valueUInt32) == false);
                Assert.AreEqual(0, attrib.GetUInt32(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt32(0, out valueUInt32) == true);
                Assert.AreEqual(1000, valueUInt32);
                Assert.AreEqual(1000, attrib.GetUInt32(0, 0));

                #endregion

                #region GetUInt64
                UInt64 valueUInt64;
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64) == true);
                Assert.AreEqual(1000, valueUInt64);
                Assert.AreEqual(1000, attrib.GetUInt64(0, 0));

                // special case: too big to fit
                // N/A

                // special case: float to UInt64
                //attrib = CreateAttribute();
                //attrib.SetFloat64(0, 1000.50f);
                //Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64) == false);
                //Assert.AreEqual(0, attrib.GetUInt64(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetInt64(0, 1000);
                Assert.IsTrue(attrib.TryGetUInt64(10, out valueUInt64) == false);
                Assert.AreEqual(0, attrib.GetUInt64(10, 0));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 1000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUInt64(0, out valueUInt64) == true);
                Assert.AreEqual(1000, valueUInt64);
                Assert.AreEqual(1000, attrib.GetUInt64(0, 0));

                #endregion

                #region GetFloat
                float valueFloat;
                double valueDouble;
                attrib = CreateAttribute();
                attrib.SetInt16(0, -1000);
                attrib.SetInt32(1, 2000);
                attrib.SetInt64(2, 3000);
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetFloat32(0, out valueFloat) == true);
                Assert.IsTrue(attrib.TryGetFloat64(0, out valueDouble) == true);
                Assert.AreEqual(-1000.0f, attrib.GetFloat32(0, 0.0f));
                Assert.AreEqual(2000.0f, attrib.GetFloat32(1, 0.0f));
                Assert.AreEqual(3000.0f, attrib.GetFloat32(2, 0.0f));

                Assert.AreEqual(-1000.0f, attrib.GetFloat64(0, 0.0f));
                Assert.AreEqual(2000.0f, attrib.GetFloat64(1, 0.0f));
                Assert.AreEqual(3000.0f, attrib.GetFloat64(2, 0.0f));


                #endregion

                #region GetString
                string stringVal;
                attrib = CreateAttribute();
                attrib.AppendInt32(1239);
                attrib.AppendUInt16(1900);
                attrib.AppendInt16(-120);
                Assert.AreEqual(3, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1239", stringVal);
                Assert.AreEqual("1239", attrib.GetString(0, ""));
                Assert.AreEqual("1900", attrib.GetString(1, ""));
                Assert.AreEqual("-120", attrib.GetString(2, ""));

                // special case: invalid index
                Assert.IsTrue(attrib.TryGetString(10, out stringVal)==false);
                Assert.AreEqual("", attrib.GetString(10, ""));


                // special case: leading/trailing spaces
                // Underyling data is string, spaces should be preserved 
                attrib = CreateAttribute();
                attrib.SetStringValue(" 1000 \\ -2000 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual(" 1000 ", attrib.GetString(0, ""));
                Assert.AreEqual(" 1000 ", attrib.GetString(0, ""));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual(" -2000 ", attrib.GetString(1, ""));
                Assert.AreEqual(" -2000 ", attrib.GetString(1, ""));

                #endregion

            }
        }
        #endregion

        #region SpecificCharacterSet Test
        [Test]
        public void SpecificCharacterSetTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.WindowCenter));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region DicomUid Test
        [Test]
        public void DicomUidTest()
        {
            DicomUid uid = DicomUid.GenerateUid();
            DicomUid uid2 = DicomUid.GenerateUid();
            Assert.AreNotEqual(uid.UID, uid2.UID);
        }
        #endregion

        #region DICOMAttributeFD Tests
        [Test]
        public void DICOMAttributeFDTest()
        {
            DICOMAttributeFDTestSuite test = new DICOMAttributeFDTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
            
        }
        private class DICOMAttributeFDTestSuite
        {
            public DicomAttributeFD CreateAttribute()
            {
                DicomAttributeFD attrib = new DicomAttributeFD(DicomTagDictionary.GetDicomTag(DicomTags.ReferencePixelPhysicalValueX));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                DicomAttributeFD attrib;
                try
                {
                    attrib = new DicomAttributeFD(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should have failed (different VR)");
                }
                catch (DicomException e)
                {
                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeFD attrib;

                 
                #region Float 32
                attrib = CreateAttribute();
                attrib.SetFloat32(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);
                
                attrib.SetFloat32(1, 0.202020f);
                Assert.AreEqual(8*2, attrib.StreamLength);

                //Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat32(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);
                try
                {
                    attrib.SetFloat32(10, 1000.0f);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(8, attrib.StreamLength);
                }

                #endregion

                #region Double
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);

                attrib.SetFloat64(1, 0.202020f);
                Assert.AreEqual(8 * 2, attrib.StreamLength);

                //Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);
                try
                {
                    attrib.SetFloat64(10, 1000.0f);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(8, attrib.StreamLength);
                }

                #endregion

                #region Set String
                attrib = CreateAttribute();
                attrib.SetString(0, "1000.0292");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);

                attrib.SetString(1, "2000E-29");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8*2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue("1000E02\\-2.202E2");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8 * 2, attrib.StreamLength);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(1, "2000E-29");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: setString(null )
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, null);
                    Assert.Fail("Above statement should throw exception: can't set null value for FD ");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: setStringValue(null)
                attrib = CreateAttribute();
                attrib.SetStringValue("1000E02\\-2.202E2");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8 * 2, attrib.StreamLength);
                attrib.SetStringValue(null);
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                

                // special case: SetString(""); 
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "");
                    Assert.Fail("Above statement should throw exception: can't set empty string for FD ");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: setStringValue("")
                attrib = CreateAttribute();
                attrib.SetStringValue("1000E02\\-2.202E2");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8 * 2, attrib.StreamLength);
                attrib.SetStringValue("");
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                // special case: wrong format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "wrong123");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("1223\\wrong123");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 1223 \\ 1.2E11 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8*2, attrib.StreamLength); 
                // we test the value in Get
                
                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeFD attrib;
                #region Append numbers
                attrib = CreateAttribute();

                /*
                attrib.AppendInt16(Int16.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 1);

                attrib.AppendInt32(Int32.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 2);

                attrib.AppendInt64(Int64.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 3);

                attrib.AppendUInt16(UInt16.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 4);

                attrib.AppendUInt32(UInt32.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 5);

                attrib.AppendUInt64(UInt64.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 6);
                 
                Assert.AreEqual(8 * 6, attrib.StreamLength);
                */

                attrib = CreateAttribute();
                attrib.AppendFloat32(1000.0f);
                Assert.AreEqual(attrib.Count, 1);

                attrib.AppendFloat64(1000.0d);
                Assert.AreEqual(attrib.Count, 2);

                Assert.AreEqual(8 * 2, attrib.StreamLength);
                

               

                #endregion

                #region Append String
                attrib = CreateAttribute();
                attrib.AppendString("1000");
                Assert.AreEqual(1, attrib.Count );
                Assert.AreEqual(8, attrib.StreamLength);

                attrib.AppendString("2000E-10");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(8*2, attrib.StreamLength);

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.AppendString(" 1234.22 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(8, attrib.StreamLength);


               
                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("wrong123");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                // special case: append null
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString(null);
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                try
                {
                    attrib.AppendString("");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
            }

            public void TestGet()
            {
                DicomAttributeFD attrib;

                #region GetInt16
                
                #endregion

                #region GetInt32
                
                #endregion

                #region GetInt64
                
                #endregion

                #region GetUInt16

               
                #endregion

                #region GetUInt32
                
                #endregion

                #region GetUInt64
                
                #endregion

                #region GetFloat32
                float floatVal;
                attrib = CreateAttribute();
                attrib.AppendFloat32(float.MaxValue - 1);
                Assert.AreEqual(1, attrib.Count);


                Assert.IsTrue(attrib.TryGetFloat32(0, out floatVal)==true);
                Assert.AreEqual(float.MaxValue - 1, attrib.GetFloat32(0, 0.0f));
                
                // special case: invalid index
                attrib = CreateAttribute();
                Assert.AreEqual(0, attrib.Count); 
                Assert.IsTrue(attrib.TryGetFloat32(0, out floatVal) == false);
                Assert.AreEqual(0.0f, attrib.GetFloat32(0, 0.0f));
                
                attrib.AppendFloat32(float.MaxValue - 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat32(10, out floatVal) == false);
                Assert.AreEqual(0.0f, attrib.GetFloat32(10, 0.0f));
                
                // special case: too big to fit
                attrib = CreateAttribute();
                attrib.AppendFloat64(double.MaxValue - 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat32(10, out floatVal) == false);
                Assert.AreEqual(0.0f, attrib.GetFloat32(10, 0.0f));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 1.2432 \\ 2.422E+300 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat32(0, out floatVal) == true);
                Assert.AreEqual(1.2432f, attrib.GetFloat32(0, 0.0f));
                Assert.IsTrue(attrib.TryGetFloat32(1, out floatVal) == false); // too big to fit
                Assert.AreEqual(0.0f, attrib.GetFloat32(1, 0.0f));

                #endregion

                #region Get double
                double doubleVal;
                attrib = CreateAttribute();
                attrib.AppendFloat64(double.MaxValue - 1);
                Assert.AreEqual(1, attrib.Count);


                Assert.IsTrue(attrib.TryGetFloat64(0, out doubleVal) == true);
                Assert.AreEqual(double.MaxValue - 1, attrib.GetFloat64(0, 0.0f));

                // special case: invalid index
                attrib = CreateAttribute();
                Assert.AreEqual(0, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat64(0, out doubleVal) == false);
                Assert.AreEqual(0.0, attrib.GetFloat64(0, 0.0f));

                attrib.AppendFloat64(double.MaxValue - 1);
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat64(10, out doubleVal) == false);
                Assert.AreEqual(0.0, attrib.GetFloat64(10, 0.0f));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 1.2432 \\ 2.422E+002 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat64(0, out doubleVal) == true);
                Assert.AreEqual(1.2432, attrib.GetFloat64(0, 0.0f));
                Assert.IsTrue(attrib.TryGetFloat64(1, out doubleVal) == true);
                Assert.AreEqual(2.422E+002, attrib.GetFloat64(1, 0.0f));
                #endregion


                #region GetString
                string stringVal;

                // Value set by SetStringValue()
                attrib = CreateAttribute();
                attrib.SetStringValue("1.2432\\2.422E+002");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal)==true);
                Assert.AreEqual(1.2432, double.Parse(attrib.GetString(0, "")));
                Assert.IsTrue(attrib.TryGetString(1, out stringVal) == true);
                Assert.AreEqual(2.422E+002, double.Parse(attrib.GetString(1, "")));

                // Value set by SetString
                attrib = CreateAttribute();
                attrib.SetString(0, "1.2432");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "2.422E+002");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal) == true);
                Assert.AreEqual(1.2432f, float.Parse(stringVal));
                Assert.AreEqual(1.2432f, float.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal) == true);
                Assert.AreEqual(2.422E+002, double.Parse(stringVal)); 
                Assert.AreEqual(2.422E+002, double.Parse(attrib.GetString(1, "")));

                // value set by set float/double
                attrib = CreateAttribute();
                attrib.SetFloat32(0, (float)1.2432f);
                Assert.AreEqual(1, attrib.Count);
                attrib.SetFloat64(1, 2.422E+002);
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal) == true);
                Assert.AreEqual(1.2432f, float.Parse(stringVal));
                Assert.AreEqual(1.2432f, float.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal) == true);
                Assert.AreEqual(2.422E+002, double.Parse(stringVal));
                Assert.AreEqual(2.422E+002, double.Parse(attrib.GetString(1, "")));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("1.2432\\2.422E+002");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(10, out stringVal) == false);
                Assert.AreEqual("", attrib.GetString(10, ""));

                // special case: leading/trailing spaces
                // underlying data is FD so all leading/trailing spaces should not be preserved
                attrib = CreateAttribute();
                attrib.SetStringValue(" 1.2432 \\ 2.422E+002 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal) == true);
                Assert.AreEqual("1.2432", attrib.GetString(0, ""));

                #endregion

            }

            

        }
        #endregion

        #region DICOMAttributeFL Tests
        [Test]
        public void DICOMAttributeFLTest()
        {
            DICOMAttributeFLTestSuite test = new DICOMAttributeFLTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
            
        }
        private class DICOMAttributeFLTestSuite
        {
            public DicomAttributeFL CreateAttribute()
            {
                DicomAttributeFL attrib = new DicomAttributeFL(DicomTagDictionary.GetDicomTag(DicomTags.DistanceSourceToIsocenter));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                return attrib;
            }
            public void TestConstructors()
            {
                DicomAttributeFL attrib;
                try
                {
                    attrib = new DicomAttributeFL(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should have failed (different VR)");
                }
                catch (DicomException e)
                {
                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeFL attrib;


                #region Float 32
                attrib = CreateAttribute();
                attrib.SetFloat32(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetFloat32(1, 0.202020f);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                //Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat32(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);
                try
                {
                    attrib.SetFloat32(10, 1000.0f);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(4, attrib.StreamLength);
                }

                #endregion

                #region Double
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetFloat64(1, 0.202020f);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                //Special case: invalid index
                attrib = CreateAttribute();
                attrib.SetFloat64(0, 1.2f);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);
                try
                {
                    attrib.SetFloat64(10, 1000.0f);
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(4, attrib.StreamLength);
                }

                // special case: overflow

                attrib = CreateAttribute();
                try
                {
                    attrib.SetFloat64(0, double.MaxValue);
                    Assert.Fail("Above statment should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                #endregion

                #region Set String
                attrib = CreateAttribute();
                attrib.SetString(0, "1000.0292");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.SetString(1, "2000E-29");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue("1000E02\\-2.202E2");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(1, "2000E-29");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: setString(null )
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, null);
                    Assert.Fail("Above statement should throw exception: can't set null value for FL ");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: setStringValue(null)
                attrib = CreateAttribute();
                attrib.SetStringValue("1000E02\\-2.202E2");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);
                attrib.SetStringValue(null);
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);



                // special case: SetString(""); 
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "");
                    Assert.Fail("Above statement should throw exception: can't set empty string for FL ");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: setStringValue("")
                attrib = CreateAttribute();
                attrib.SetStringValue("1000E02\\-2.202E2");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);
                attrib.SetStringValue("");
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                // special case: wrong format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "wrong123");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("1223\\wrong123");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 1223 \\ 1.2E11 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);
                // we test the value in Get


                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetStringValue("1000E+300");
                    Assert.Fail("Above statment should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }
                

                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeFL attrib;
                #region Append numbers
                attrib = CreateAttribute();
                
                /*
                attrib.AppendInt16(Int16.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 1);

                attrib.AppendInt32(Int32.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 2);

                attrib.AppendInt64(Int64.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 3);

                attrib.AppendUInt16(UInt16.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 4);

                attrib.AppendUInt32(UInt32.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 5);

                attrib.AppendUInt64(UInt64.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 6);
                 */

                attrib.AppendFloat32(1000.0f);
                Assert.AreEqual(attrib.Count, 1);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendFloat64(1000.0d);
                Assert.AreEqual(attrib.Count, 2);
                Assert.AreEqual(8, attrib.StreamLength);

                #endregion

                #region Append String
                attrib = CreateAttribute();
                attrib.AppendString("1000");
                Assert.AreEqual(1, attrib.Count );
                Assert.AreEqual(4, attrib.StreamLength);


                attrib.AppendString("2000E-10");
                Assert.AreEqual(2, attrib.Count );
                Assert.AreEqual(8, attrib.StreamLength);


                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.AppendString(" 2000E-10 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);


                // special case: wrong format
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("wrong123");
                    Assert.Fail("Above statement should have failed: wrong format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count );
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                // special case: append null
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString(null);
                    Assert.Fail("Above statement should have failed: cannot append null to FL attribute");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                // special case: append ""
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("");
                    Assert.Fail("Above statement should have failed:cannot append emty string to FL attribute");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
            }

            public void TestGet()
            {
                DicomAttributeFL attrib;

                #region GetInt16

                #endregion

                #region GetInt32

                #endregion

                #region GetInt64

                #endregion

                #region GetUInt16


                #endregion

                #region GetUInt32

                #endregion

                #region GetUInt64

                #endregion

                #region GetFloat32
                float value;
                attrib = CreateAttribute();
                attrib.AppendFloat32(float.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 1);

                Assert.IsTrue(attrib.TryGetFloat32(0, out value));
                Assert.AreEqual(float.MaxValue -1 , value);
                Assert.AreEqual(float.MaxValue - 1, attrib.GetFloat32(0, 0.0f));
                
                // special case: invalid index
                attrib = CreateAttribute();
                attrib.AppendFloat32(float.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.TryGetFloat32(10, out value)==false);
                Assert.AreEqual(0.0f, attrib.GetFloat32(10, 0.0f));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 123 \\ 124.22 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat32(0, out value) == true);
                Assert.AreEqual(123.0f, value);
                Assert.AreEqual(123.0f, attrib.GetFloat32(0, 0.0f));

                Assert.IsTrue(attrib.TryGetFloat32(1, out value) == true);
                Assert.AreEqual(124.22f, value);
                Assert.AreEqual(124.22f, attrib.GetFloat32(1, 0.0f));
                
                #endregion

                #region get Double
                double doubleVal;
                attrib = CreateAttribute();
                attrib.AppendFloat64(1000.0f);
                Assert.AreEqual(attrib.Count, 1);

                Assert.IsTrue(attrib.TryGetFloat64(0, out doubleVal));
                Assert.AreEqual(1000.0, doubleVal);
                Assert.AreEqual(1000.0, attrib.GetFloat64(0, 0.0f));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.AppendFloat64(float.MaxValue - 1);
                Assert.AreEqual(attrib.Count, 1);
                Assert.IsTrue(attrib.TryGetFloat64(10, out doubleVal) == false);
                Assert.AreEqual(0.0, attrib.GetFloat64(10, 0.0f));

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 123 \\ 124.22 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetFloat64(0, out doubleVal) == true);
                Assert.AreEqual(123.0, doubleVal);
                Assert.AreEqual(123.0, attrib.GetFloat64(0, 0.0f));

                Assert.IsTrue(attrib.TryGetFloat64(1, out doubleVal) == true);
                Assert.AreEqual(124.22, doubleVal);
                Assert.AreEqual(124.22, attrib.GetFloat64(1, 0.0f));

                #endregion

            }

            
        }
        #endregion

        #region DICOMAttributeSL Tests
        [Test]
        public void DICOMAttributeSLTest()
        {
            DICOMAttributeSLTestSuite test = new DICOMAttributeSLTestSuite();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
        }
        private class DICOMAttributeSLTestSuite
        {
            public DicomAttributeSL CreateAttribute()
            {
                DicomAttributeSL attrib = new DicomAttributeSL(DicomTagDictionary.GetDicomTag(DicomTags.DisplayedAreaBottomRightHandCorner));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeSL attrib = new DicomAttributeSL(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should have failed (different VR)");
                }
                catch (DicomException e)
                {
                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeSL attrib;

                
                #region Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(0, Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(0, UInt32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(0, UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region Set from string
                attrib = CreateAttribute();
                attrib.SetStringValue("-10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib = CreateAttribute(); 
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);


                attrib = CreateAttribute(); 
                attrib.SetStringValue("10000\\-10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                attrib = CreateAttribute(); 
                attrib.SetString(0, "10000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "-10000");
                Assert.AreEqual(2, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "10000");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "10000000000000000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                try
                {
                    attrib.SetStringValue("10000000000000000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: set null string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count); 
                try
                {
                    attrib.SetString(1, null);
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue(null);
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                
                // special case: set empty string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "");
                    Assert.Fail("Above statement should throw exception. Can't set empty value to SL attribute");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(4 , attrib.StreamLength);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue("");
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                // special case: trailing/leading spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 10000 \\ -20000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);
                


                // special case: invalid format
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "wrongformat");
                    Assert.Fail("Above statement should throw exception. invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(4, attrib.StreamLength);
                }

                
                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeSL attrib;

                #region Append binaries     

                #region Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(-1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendInt16(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                #endregion
                #region Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(-1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendInt32(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                #endregion
                #region Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(-1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendInt64(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt64(Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);


                #endregion
                #region UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                #endregion
                #region UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt64(UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion

                
                #endregion

                #region Append String

                attrib = CreateAttribute();
                attrib.AppendString("-10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendString("10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("100000000000000");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: append null
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString(null);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid format
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("Something");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: leading/trailing spaces
                //
                attrib = CreateAttribute();
                attrib.AppendString(" 12234 ");
            
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);
            
                #endregion
            }

            public void TestGet()
            {
                DicomAttributeSL attrib;

                #region Get to binaries
                attrib = new DicomAttributeSL(DicomTagDictionary.GetDicomTag(DicomTags.DisplayedAreaBottomRightHandCorner));
                
                attrib.SetStringValue("-1000\\2000");
                Assert.AreEqual(2, attrib.Count);

                Assert.AreEqual(-1000, attrib.GetInt16(0, 0));
                Assert.AreEqual(2000, attrib.GetInt16(1, 0));
                Assert.AreEqual(-1000, attrib.GetInt32(0, 0) );
                Assert.AreEqual(2000, attrib.GetInt32(1, 0) );

                Assert.AreEqual(attrib.GetUInt16(0, 0), 0); // can't retrieve -1000 as Uint16
                
                #endregion

                #region Get to string
                string stringVal;
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\ -2000 ");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("-2000", stringVal);
                Assert.AreEqual(-2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.SetString(0, "1000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "-2000");
                Assert.AreEqual(2, attrib.Count);
                
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("-2000", stringVal);
                Assert.AreEqual(-2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.AppendString(" 3000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("3000", stringVal);
                Assert.AreEqual(3000, long.Parse(attrib.GetString(0, "")));
                #endregion

            }

        }
        #endregion


        #region DICOMAttributeSS Tests
        [Test]
        public void DICOMAttributeSSTest()
        {
            DICOMAttributeSSTestSuite test = new DICOMAttributeSSTestSuite();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
        }
        private class DICOMAttributeSSTestSuite
        {
            public DicomAttributeSS CreateAttribute()
            {
                DicomAttributeSS attrib = new DicomAttributeSS(DicomTagDictionary.GetDicomTag(DicomTags.AbstractPriorValue));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeSS attrib = new DicomAttributeSS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should have failed (different VR)");
                }
                catch (DicomException e)
                {
                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeSS attrib;


                #region Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(0, Int32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                #endregion

                #region Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(0, Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(0, UInt16.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(0, UInt32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(0, UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region Set from string
                attrib = CreateAttribute();
                attrib.SetStringValue("-10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);


                attrib = CreateAttribute();
                attrib.SetStringValue("10000\\-10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetString(0, "10000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "-10000");
                Assert.AreEqual(2, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "10000");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "33000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                try
                {
                    attrib.SetStringValue("-33000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: set null string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, null);
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue(null);
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);



                // special case: set empty string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "");
                    Assert.Fail("Above statement should throw exception. Can't set empty value to SS attribute");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(2, attrib.StreamLength);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue("");
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                // special case: trailing/leading spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 10000 \\ -20000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);



                // special case: invalid format
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "wrongformat");
                    Assert.Fail("Above statement should throw exception. invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(2, attrib.StreamLength);
                }


                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeSS attrib;

                #region Append binaries

                #region Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(-1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendInt16(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                #endregion
                #region Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(-1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendInt32(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt32(Int32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(-1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendInt64(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt64(Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt16(UInt16.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt32(UInt32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt64(UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion


                #endregion

                #region Append String

                attrib = CreateAttribute();
                attrib.AppendString("-10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendString("10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("33000");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: append null
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString(null);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid format
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("Something");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: leading/trailing spaces
                //
                attrib = CreateAttribute();
                attrib.AppendString(" 12234 ");

                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                #endregion
            }

            public void TestGet()
            {
                DicomAttributeSS attrib;

                #region Get to binaries
                attrib = CreateAttribute();

                attrib.SetStringValue("-1000\\2000");
                Assert.AreEqual(2, attrib.Count);

                Assert.AreEqual(-1000, attrib.GetInt16(0, 0));
                Assert.AreEqual(2000, attrib.GetInt16(1, 0));
                Assert.AreEqual(-1000, attrib.GetInt32(0, 0));
                Assert.AreEqual(2000, attrib.GetInt32(1, 0));

                Assert.AreEqual(attrib.GetUInt16(0, 0), 0); // can't retrieve -1000 as Uint16

                #endregion

                #region Get to string
                string stringVal;
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\ -2000 ");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("-2000", stringVal);
                Assert.AreEqual(-2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.SetString(0, "1000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "-2000");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("-2000", stringVal);
                Assert.AreEqual(-2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.AppendString(" 3000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("3000", stringVal);
                Assert.AreEqual(3000, long.Parse(attrib.GetString(0, "")));
                #endregion

            }

        }
        #endregion

        #region DicomAttributeTM Test
        [Test]
        public void AttributeTMTest()
        {
            AttributeTMTestSuite test = new AttributeTMTestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();

        }
        private class AttributeTMTestSuite
        {
            public DicomAttributeTM CreateAttribute()
            {
                DicomAttributeTM attrib = new DicomAttributeTM(DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepStartTime));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                
                try
                {
                    DicomAttributeTM attrib = new DicomAttributeTM(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {
                    
                }

                CreateAttribute();

            }

            public void TestSet()
            {
                DicomAttributeTM attrib;
                #region set string
                attrib = CreateAttribute();
                attrib.SetStringValue("120013");
                Assert.AreEqual(1, attrib.Count);

                attrib = CreateAttribute(); 
                attrib.SetStringValue("120013\\120015");
                Assert.AreEqual(2, attrib.Count);

                attrib = CreateAttribute();
                attrib.SetString(0, "120013");
                Assert.AreEqual(1, attrib.Count);

                attrib.SetString(1, "120015");
                Assert.AreEqual(2, attrib.Count);

                // test case: hhmmss.frac format
                attrib = CreateAttribute();
                attrib.SetString(0, "120013.000");
                Assert.AreEqual(1, attrib.Count);

                attrib.SetString(1, "120015.20000");
                Assert.AreEqual(2, attrib.Count);

                // special case: set null value
                attrib = CreateAttribute();
                attrib.SetString(0, null);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);

                // special case: leading/trailing spaces
                attrib = CreateAttribute();
                attrib.SetString(0, " 120013 ");
                Assert.AreEqual(1, attrib.Count);

                attrib = CreateAttribute();
                attrib.SetStringValue(" 120013 \\ 120015 ");
                Assert.AreEqual(2, attrib.Count);


                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "something");
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                }

                // special case: invalid time
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "126700");
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                }


                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "120013");
                    Assert.Fail("Above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                }

                #endregion

                #region Set datetime
                attrib = CreateAttribute();
				DateTime d1 = DateTime.Parse("2/16/1992 12:15:12", CultureInfo);
                attrib.SetDateTime(0, d1);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("HHmmSS".Length, attrib.StreamLength);

                DateTime d2 = DateTime.Parse("12/02/2000 12:15:12", CultureInfo);
                attrib.SetDateTime(1, d2);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("HHmmSS\\HHmmSS".Length + 1, attrib.StreamLength);


                // special case: invalid index
                try
                {
                    attrib.SetDateTime(10, d2);
                    Assert.Fail("Above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                { }


                #endregion


            }
            public void TestAppend()
            {
                DicomAttributeTM attrib;

                #region append string
                attrib = CreateAttribute();
                attrib.AppendString("120040");
                Assert.AreEqual(1, attrib.Count);
                attrib.AppendString("120040");
                Assert.AreEqual(2, attrib.Count);
                attrib.AppendString("120040.2200");
                Assert.AreEqual(3, attrib.Count);

                // special case: append null
                attrib.AppendString(null);
                Assert.AreEqual(4, attrib.Count);
                Assert.IsTrue(attrib.StreamLength % 2 == 0);

                // special case: invalid format
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("something");
                    Assert.Fail("Above statement should throw exception: invalid format");
                }
                catch (DicomDataException e)
                {
                }

                // special case: invalid time
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("120380");
                    Assert.Fail("Above statement should throw exception: invalid time");
                }
                catch (DicomDataException e)
                {
                }

                // special case: append null
                attrib = CreateAttribute(); 
                attrib.AppendString(null);
                Assert.AreEqual(1, attrib.Count);
                

                #endregion

                #region append datetime
                attrib = CreateAttribute();
                attrib.AppendDateTime(new DateTime());
                Assert.IsTrue(attrib.Count == 1);
                attrib.AppendDateTime(new DateTime());
                Assert.IsTrue(attrib.Count == 2);

                #endregion

            }

            public void TestGet()
            {
                DicomAttributeTM attrib;
                #region Get string
                string stringVal;

                attrib = CreateAttribute();
                attrib.SetStringValue("120013.200000 \\ 125000 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("120013.200000 ", stringVal);
                Assert.AreEqual( "120013.200000 ", attrib.GetString(0, ""));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual(" 125000 ", stringVal);
                Assert.AreEqual(" 125000 ", attrib.GetString(1, ""));



                attrib = CreateAttribute();
                attrib.SetString(0, "120013.200000");
                attrib.SetString(1, " 125000 ");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("120013.200000", stringVal);
                Assert.AreEqual("120013.200000", attrib.GetString(0, ""));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual(" 125000 ", stringVal);
                Assert.AreEqual(" 125000 ", attrib.GetString(1, ""));

                // special case: invalid index
                Assert.IsTrue(attrib.TryGetString(10, out stringVal)==false);
                Assert.AreEqual("", attrib.GetString(10, ""));


                attrib = CreateAttribute();
				attrib.SetDateTime(0, DateTime.Parse("08/20/2001 12:15:12", CultureInfo));
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("121512", stringVal);
                Assert.AreEqual("121512", attrib.GetString(0, ""));




                #endregion

                #region Get to datetime
                DateTime dtVal;
                attrib = CreateAttribute();
                attrib.SetStringValue("120013.200000 \\ 125000 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.IsTrue(attrib.TryGetDateTime(0, out dtVal));
                Assert.AreEqual("120013.200000", dtVal.ToString("hhmmss.ffffff"));

                Assert.IsTrue(attrib.TryGetDateTime(1, out dtVal));
                Assert.AreEqual("125000.000000", attrib.GetDateTime(1, dtVal).ToString("hhmmss.ffffff"));

				// special case: get empty, no values
				attrib = CreateAttribute();
				Assert.AreEqual(0, attrib.Count);
				Assert.IsTrue(attrib.TryGetString(1, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(1, out dtVal) == false);
				Assert.AreEqual(dtVal, attrib.GetDateTime(1, dtVal));

				// special case: get empty, null value
				attrib = CreateAttribute();
				attrib.SetNullValue();
				Assert.AreEqual(1, attrib.Count);
				Assert.IsTrue(attrib.TryGetString(1, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetString(0, out stringVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(1, out dtVal) == false);
				Assert.IsTrue(attrib.TryGetDateTime(0, out dtVal) == false);
				Assert.AreEqual(dtVal, attrib.GetDateTime(1, dtVal));
				Assert.AreEqual(dtVal, attrib.GetDateTime(0, dtVal));

                #endregion

            }
        }
        #endregion

        #region DicomAttributeUI Test
        [Test]
        public void AttributeUITest()
        {
            AttributeUITestSuite test = new AttributeUITestSuite();
            test.TestConstructors();
            test.TestSet();
            test.TestAppend();
            test.TestGet();

        }
        private class AttributeUITestSuite
        {
            public DicomAttributeUI CreateAttribute()
            {
                DicomAttributeUI attrib = new DicomAttributeUI(DicomTagDictionary.GetDicomTag(DicomTags.StudyInstanceUid));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeUI attrib = new DicomAttributeUI(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomException)
                {

                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeUI attrib;

                #region set string
                attrib = CreateAttribute();
                attrib.SetStringValue("1.2.455.33.54");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("1.2.455.33.54".Length+1, attrib.StreamLength);

                attrib.SetStringValue("1.2.455.33.54\\1.2.455.33.54.309393");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("1.2.455.33.54\\1.2.455.33.54.309393".Length, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetString(0, "1.2.455.33.54");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("1.2.455.33.54".Length+1, attrib.StreamLength);

                attrib.SetString(1, "1.2.455.33.54.309393");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("1.2.455.33.54\\1.2.455.33.54.309393".Length, attrib.StreamLength);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "1.1.1.1");
                    Assert.Fail("Above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {

                }
                
                // special case: set null
                attrib = CreateAttribute();
                attrib.SetString(0, "1.2.455.33.54.309393");
                Assert.AreEqual(1, attrib.Count); 
                attrib.SetStringValue(null);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                attrib = CreateAttribute(); 
                attrib.SetString(0, "1.2.455.33.54.309393");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, null);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.309393\\".Length + 1, attrib.StreamLength);
 
                
                #endregion

                #region Set dicomui
                attrib = CreateAttribute();
                DicomUid ui = new DicomUid("1.2.455.33.54.309393", "Some Description", UidType.SOPInstance);
                attrib.SetUid(0, ui);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.309393".Length , attrib.StreamLength);


                ui = new DicomUid("1.2.455.33.54.123091823", "Some Description", UidType.SOPClass);
                attrib.SetUid(1, ui);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.309393\\1.2.455.33.54.123091823".Length, attrib.StreamLength);

                // special case: invalid index
                try
                {
                    attrib.SetUid(10, ui);
                    Assert.Fail("Above statement should throw exception");
                }
                catch (IndexOutOfRangeException e)
                {
                }
                #endregion



            }
            public void TestAppend()
            {
                DicomAttributeUI attrib ;

                #region append string
                attrib = CreateAttribute();
                attrib.AppendString("1.2.455.33.54.123091823");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.123091823".Length+1, attrib.StreamLength);
                
                attrib.AppendString("1.2.455.33.54.309393");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.123091823\\1.2.455.33.54.309393".Length, attrib.StreamLength);
                
                // special case: append null
                attrib.AppendString(null);
                Assert.AreEqual(3, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.123091823\\1.2.455.33.54.309393\\".Length+1, attrib.StreamLength);
                

                #endregion

                #region append dicom uid
                attrib = CreateAttribute();
                DicomUid ui = new DicomUid("1.2.455.33.54.123091823", "Some Description", UidType.SOPInstance);
                attrib.AppendUid(ui);
                Assert.AreEqual(1, attrib.Count );
                Assert.AreEqual("1.2.455.33.54.123091823".Length+1, attrib.StreamLength);
                
                ui = new DicomUid("1.2.455.33.54.123123213", "Some Description", UidType.SOPInstance);
                attrib.AppendUid(ui);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual("1.2.455.33.54.123091823\\1.2.455.33.54.123123213".Length+1, attrib.StreamLength);
                
                #endregion

            }

            public void TestGet()
            {
                DicomAttributeUI attrib;
                #region Get string
                string stringVal;

                attrib = CreateAttribute();
                attrib.SetStringValue("1.2.4.5.6.87.8.121\\1.3.54.3.54.5");
                Assert.AreEqual(2, attrib.Count);
                
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1.2.4.5.6.87.8.121", stringVal);
                Assert.AreEqual( "1.2.4.5.6.87.8.121", attrib.GetString(0, ""));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("1.3.54.3.54.5", stringVal);
                Assert.AreEqual("1.3.54.3.54.5", attrib.GetString(1, ""));

                // set using dicomui
                attrib = CreateAttribute();
                DicomUid ui = new DicomUid("1.2.455.33.54.123091823", "Some Description", UidType.SOPInstance);
                attrib.AppendUid(ui);
                Assert.AreEqual(1, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1.2.455.33.54.123091823", stringVal);
                Assert.AreEqual("1.2.455.33.54.123091823", attrib.GetString(0, ""));



                // special case: invalid index
                Assert.IsTrue(attrib.TryGetString(10, out stringVal)==false);
                Assert.AreEqual("", attrib.GetString(10, ""));



                #endregion

                #region Get to dicom uid
                DicomUid uiVal;

                attrib = CreateAttribute();
                attrib.SetString(0, "1.2.840.10008.5.1.4.1.1.6.1");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUid(0, out uiVal));
                Assert.AreEqual(SopClass.UltrasoundImageStorage.DicomUid, attrib.GetUid(0, null));

                // special case: invalid index
                Assert.IsTrue(attrib.TryGetUid(10, out uiVal) == false);
                Assert.AreEqual(null, attrib.GetUid(10, null));

                // special case: set null
                attrib = CreateAttribute();
                attrib.SetString(0, null);
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetUid(0, out uiVal)==true); // empty UI, should return true
                Assert.AreEqual(null, uiVal);
                Assert.AreEqual(null, attrib.GetUid(0, null));

                

                #endregion

            }
        }
        #endregion


        #region DICOMAttributeUL Tests
        [Test]
        public void DICOMAttributeULTest2()
        {
            DICOMAttributeULTestSuite test = new DICOMAttributeULTestSuite();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
        }
        private class DICOMAttributeULTestSuite
        {
            public DicomAttributeUL CreateAttribute()
            {
                DicomAttributeUL attrib = new DicomAttributeUL(DicomTagDictionary.GetDicomTag(DicomTags.SelectorUlValue));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeUL attrib = new DicomAttributeUL(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should have failed (different VR)");
                }
                catch (DicomException e)
                {
                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeUL attrib;


                #region Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(0, -1000); // UL must be >= 0
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                #endregion

                #region Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(0, -1000); // UL must be >= 0
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }
                #endregion

                #region Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(0, -1000); // UL must be >= 0
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }



                #endregion

                #region UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region Set from string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue("20000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);


                attrib = CreateAttribute();
                attrib.SetStringValue("10000\\20000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetString(0, "10000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "20000");
                Assert.AreEqual(2, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "10000");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "-1000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                try
                {
                    attrib.SetStringValue("1000\\-1000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: set null string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, null);
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue(null);
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);



                // special case: set empty string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "");
                    Assert.Fail("Above statement should throw exception. Can't set empty value to UL attribute");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(4, attrib.StreamLength);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue("");
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                // special case: trailing/leading spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 10000 \\ 20000 ");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);



                // special case: invalid format
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "wrongformat");
                    Assert.Fail("Above statement should throw exception. invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(4, attrib.StreamLength);
                }


                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeUL attrib;

                #region Append binaries

                #region Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendInt16(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt16(-1000);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendInt32(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt32(-1000);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendInt64(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt64(Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);


                #endregion
                #region UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                #endregion
                #region UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt64(UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion


                #endregion

                #region Append String

                attrib = CreateAttribute();
                attrib.AppendString("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                attrib.AppendString("10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(4 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("-1000");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: append null
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString(null); // can't append null to UL
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid format
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("Something");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: leading/trailing spaces
                //
                attrib = CreateAttribute();
                attrib.AppendString(" 12234 ");

                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(4, attrib.StreamLength);

                #endregion
            }

            public void TestGet()
            {
                DicomAttributeUL attrib;

                #region Get to binaries
                Int16 Int16Val;
                Int32 Int32Val;
                Int64 Int64Val;
                UInt16 UInt16Val;
                UInt32 UInt32Val;
                UInt64 UInt64Val;

                attrib = CreateAttribute();

                attrib.SetStringValue("1000\\2000");
                Assert.AreEqual(2, attrib.Count);
                
                Assert.IsTrue(attrib.TryGetInt16(0, out Int16Val));
                Assert.AreEqual(1000, Int16Val);
                Assert.AreEqual(1000, attrib.GetInt16(0, 0));

                Assert.IsTrue(attrib.TryGetInt32(0, out Int32Val));
                Assert.AreEqual(1000, Int32Val);
                Assert.AreEqual(1000, attrib.GetInt32(0, 0));

                Assert.IsTrue(attrib.TryGetInt64(0, out Int64Val));
                Assert.AreEqual(1000, Int64Val);
                Assert.AreEqual(1000, attrib.GetInt64(0, 0));

                Assert.IsTrue(attrib.TryGetUInt16(0, out UInt16Val));
                Assert.AreEqual(1000, UInt16Val);
                Assert.AreEqual(1000, attrib.GetUInt16(0, 0));

                Assert.IsTrue(attrib.TryGetUInt32(0, out UInt32Val));
                Assert.AreEqual(1000, UInt32Val);
                Assert.AreEqual(1000, attrib.GetUInt32(0, 0));

                Assert.IsTrue(attrib.TryGetUInt64(0, out UInt64Val));
                Assert.AreEqual(1000, UInt64Val);
                Assert.AreEqual(1000, attrib.GetUInt64(0, 0));

                // special case: invalid index
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\2000");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt16(10, out Int16Val) == false);
                Assert.AreEqual(0, attrib.GetInt16(10, 0));

                Assert.IsTrue(attrib.TryGetInt32(10, out Int32Val) == false);
                Assert.AreEqual(0, attrib.GetInt32(10, 0));

                Assert.IsTrue(attrib.TryGetInt64(10, out Int64Val) == false);
                Assert.AreEqual(0, attrib.GetInt64(10, 0));


                Assert.IsTrue(attrib.TryGetUInt16(10, out UInt16Val) == false);
                Assert.AreEqual(0, attrib.GetUInt16(10, 0));

                Assert.IsTrue(attrib.TryGetUInt32(10, out UInt32Val) == false);
                Assert.AreEqual(0, attrib.GetUInt32(10, 0));

                Assert.IsTrue(attrib.TryGetUInt64(10, out UInt64Val) == false);
                Assert.AreEqual(0, attrib.GetUInt64(10, 0));


                // special case: overflow
                attrib = CreateAttribute();
                attrib.SetStringValue("80000");

                Assert.IsTrue(attrib.TryGetInt16(0, out Int16Val) == false);
                Assert.AreEqual(0, attrib.GetInt16(0, 0));

                Assert.IsTrue(attrib.TryGetInt32(0, out Int32Val) == true);
                Assert.AreEqual(80000, attrib.GetInt32(0, 0));

                Assert.IsTrue(attrib.TryGetInt64(0, out Int64Val) == true);
                Assert.AreEqual(80000, attrib.GetInt64(0, 0));


                Assert.IsTrue(attrib.TryGetUInt16(0, out UInt16Val) == false);
                Assert.AreEqual(0, attrib.GetUInt16(0, 0));

                Assert.IsTrue(attrib.TryGetUInt32(0, out UInt32Val) == true);
                Assert.AreEqual(80000, attrib.GetUInt32(0, 0));

                Assert.IsTrue(attrib.TryGetUInt64(0, out UInt64Val) == true);
                Assert.AreEqual(80000, attrib.GetUInt64(0, 0));

                #endregion

                #region Get to string
                string stringVal;
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\ 2000 ");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("2000", stringVal);
                Assert.AreEqual(2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.SetString(0, "1000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "2000");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("2000", stringVal);
                Assert.AreEqual(2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.AppendString(" 3000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("3000", stringVal);
                Assert.AreEqual(3000, long.Parse(attrib.GetString(0, "")));
                #endregion

            }

        }
        #endregion

        #region DICOMAttributeSS Tests
        [Test]
        public void DICOMAttributeUSTest()
        {
            DICOMAttributeUSTestSuite test = new DICOMAttributeUSTestSuite();
            test.TestSet();
            test.TestAppend();
            test.TestGet();
        }
        private class DICOMAttributeUSTestSuite
        {
            public DicomAttributeUS CreateAttribute()
            {
                DicomAttributeUS attrib = new DicomAttributeUS(DicomTagDictionary.GetDicomTag(DicomTags.SelectorUsValue));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestConstructors()
            {
                try
                {
                    DicomAttributeUS attrib = new DicomAttributeUS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
                    Assert.Fail("Above statement should have failed (different VR)");
                }
                catch (DicomException e)
                {
                }

                CreateAttribute();
            }

            public void TestSet()
            {
                DicomAttributeUS attrib;


                #region Int16
                attrib = CreateAttribute();
                attrib.SetInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt16(0, -100);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region Int32
                attrib = CreateAttribute();
                attrib.SetInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt32(0, Int32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                #endregion

                #region Int64
                attrib = CreateAttribute();
                attrib.SetInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetInt64(0, Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region UInt16
                attrib = CreateAttribute();
                attrib.SetUInt16(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt16(0, UInt16.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt32
                attrib = CreateAttribute();
                attrib.SetUInt32(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt32(0, UInt32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion

                #region UInt64
                attrib = CreateAttribute();
                attrib.SetUInt64(0, 100);
                Assert.AreEqual(1, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(10, 100);
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetUInt64(0, UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                }


                #endregion


                #region Set from string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);


                attrib = CreateAttribute();
                attrib.SetStringValue("10000\\10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetString(0, "10000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "10000");
                Assert.AreEqual(2, attrib.Count);

                // special case: invalid index
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(10, "10000");
                    Assert.Fail("Above statement should throw exception: invalid index");
                }
                catch (IndexOutOfRangeException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.SetString(0, "65537");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                try
                {
                    attrib.SetStringValue("-1000");
                    Assert.Fail("Above statement should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: set null string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, null);
                    Assert.Fail("Above statement should throw exception");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue(null);
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);



                // special case: set empty string
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "");
                    Assert.Fail("Above statement should throw exception. Can't set empty value to SS attribute");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(2, attrib.StreamLength);
                }

                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.Greater(attrib.StreamLength, 0);
                attrib.SetStringValue("");
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);


                // special case: trailing/leading spaces
                attrib = CreateAttribute();
                attrib.SetStringValue(" 10000 \\ 20000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);



                // special case: invalid format
                attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                try
                {
                    attrib.SetString(1, "wrongformat");
                    Assert.Fail("Above statement should throw exception. invalid format");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(1, attrib.Count);
                    Assert.AreEqual(2, attrib.StreamLength);
                }


                #endregion

            }

            public void TestAppend()
            {
                DicomAttributeUS attrib;

                #region Append binaries

                #region Int16
                attrib = CreateAttribute();
                attrib.AppendInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendInt16(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt16(-10);
                    Assert.Fail("Above statemetn should throw exception: overflow");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }



                #endregion
                #region Int32
                attrib = CreateAttribute();
                attrib.AppendInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendInt32(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt32(Int32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region Int64
                attrib = CreateAttribute();
                attrib.AppendInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendInt64(1000);
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendInt64(Int64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion

                #region UInt16
                attrib = CreateAttribute();
                attrib.AppendUInt16(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt16(UInt16.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region UInt32
                attrib = CreateAttribute();
                attrib.AppendUInt32(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt32(UInt32.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion
                #region UInt64
                attrib = CreateAttribute();
                attrib.AppendUInt64(1000);
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendUInt64(UInt64.MaxValue);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);

                }

                #endregion


                #endregion

                #region Append String

                attrib = CreateAttribute();
                attrib.AppendString("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                attrib.AppendString("10000");
                Assert.AreEqual(2, attrib.Count);
                Assert.AreEqual(2 * 2, attrib.StreamLength);

                // special case: overflow
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("67000");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: append null
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString(null);
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: invalid format
                //
                attrib = CreateAttribute();
                try
                {
                    attrib.AppendString("Something");
                }
                catch (DicomDataException e)
                {
                    Assert.AreEqual(0, attrib.Count);
                    Assert.AreEqual(0, attrib.StreamLength);
                }

                // special case: leading/trailing spaces
                //
                attrib = CreateAttribute();
                attrib.AppendString(" 12234 ");

                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(2, attrib.StreamLength);

                #endregion
            }

            public void TestGet()
            {
                DicomAttributeUS attrib;

                #region Get to binaries
                Int16 int16val;
                Int32 int32val;
                Int64 int64val;
                UInt16 Uint16val;
                UInt32 Uint32val;
                UInt64 Uint64val;
                
                attrib = CreateAttribute();

                attrib.SetStringValue("1000\\ 2000 ");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetInt16(0, out int16val));
                Assert.AreEqual(1000, int16val);
                Assert.AreEqual(1000, attrib.GetInt16(0, 0));

                Assert.IsTrue(attrib.TryGetInt16(1, out int16val));
                Assert.AreEqual(2000, int16val);
                Assert.AreEqual(2000, attrib.GetInt16(1, 0));

                Assert.IsTrue(attrib.TryGetInt32(0, out int32val));
                Assert.AreEqual(1000, int32val);
                Assert.AreEqual(1000, attrib.GetInt32(0, 0));

                Assert.IsTrue(attrib.TryGetInt32(1, out int32val));
                Assert.AreEqual(2000, int16val);
                Assert.AreEqual(2000, attrib.GetInt32(1, 0));

                Assert.IsTrue(attrib.TryGetInt64(0, out int64val));
                Assert.AreEqual(1000, int64val);
                Assert.AreEqual(1000, attrib.GetInt64(0, 0));

                Assert.IsTrue(attrib.TryGetInt64(1, out int64val));
                Assert.AreEqual(2000, int64val);
                Assert.AreEqual(2000, attrib.GetInt64(1, 0));


                Assert.IsTrue(attrib.TryGetUInt16(0, out Uint16val));
                Assert.AreEqual(1000, Uint16val);
                Assert.AreEqual(1000, attrib.GetUInt16(0, 0));

                Assert.IsTrue(attrib.TryGetUInt16(1, out Uint16val));
                Assert.AreEqual(2000, Uint16val);
                Assert.AreEqual(2000, attrib.GetUInt16(1, 0));

                Assert.IsTrue(attrib.TryGetUInt32(0, out Uint32val));
                Assert.AreEqual(1000, Uint32val);
                Assert.AreEqual(1000, attrib.GetUInt32(0, 0));

                Assert.IsTrue(attrib.TryGetUInt32(1, out Uint32val));
                Assert.AreEqual(2000, Uint16val);
                Assert.AreEqual(2000, attrib.GetUInt32(1, 0));


                Assert.IsTrue(attrib.TryGetUInt64(0, out Uint64val));
                Assert.AreEqual(1000, Uint64val);
                Assert.AreEqual(1000, attrib.GetUInt64(0, 0));

                Assert.IsTrue(attrib.TryGetUInt64(1, out Uint64val));
                Assert.AreEqual(2000, Uint64val);
                Assert.AreEqual(2000, attrib.GetUInt64(1, 0));

                //special case: invalid index
                Assert.IsTrue(attrib.TryGetInt16(10, out int16val)==false);
                Assert.AreEqual(0, attrib.GetInt16(10, 0));

                Assert.IsTrue(attrib.TryGetInt32(10, out int32val) == false);
                Assert.AreEqual(0, attrib.GetInt32(10, 0));

                Assert.IsTrue(attrib.TryGetInt64(10, out int64val) == false);
                Assert.AreEqual(0, attrib.GetInt64(10, 0));

                Assert.IsTrue(attrib.TryGetUInt16(10, out Uint16val) == false);
                Assert.AreEqual(0, attrib.GetUInt16(10, 0));

                Assert.IsTrue(attrib.TryGetUInt32(10, out Uint32val) == false);
                Assert.AreEqual(0, attrib.GetUInt32(10, 0));

                Assert.IsTrue(attrib.TryGetUInt64(10, out Uint64val) == false);
                Assert.AreEqual(0, attrib.GetUInt64(10, 0));



                #endregion

                #region Get to string
                string stringVal;
                attrib = CreateAttribute();
                attrib.SetStringValue("1000\\ 2000 ");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("2000", stringVal);
                Assert.AreEqual(2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.SetString(0, "1000");
                Assert.AreEqual(1, attrib.Count);
                attrib.SetString(1, "2000");
                Assert.AreEqual(2, attrib.Count);

                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("1000", stringVal);
                Assert.AreEqual(1000, long.Parse(attrib.GetString(0, "")));

                Assert.IsTrue(attrib.TryGetString(1, out stringVal));
                Assert.AreEqual("2000", stringVal);
                Assert.AreEqual(2000, long.Parse(attrib.GetString(1, "")));


                attrib = CreateAttribute();
                attrib.AppendString(" 3000 ");
                Assert.AreEqual(1, attrib.Count);
                Assert.IsTrue(attrib.TryGetString(0, out stringVal));
                Assert.AreEqual("3000", stringVal);
                Assert.AreEqual(3000, long.Parse(attrib.GetString(0, "")));
                #endregion

            }


        }
        #endregion

        #region DICOMAttributeST Tests
        [Test]
        public void DICOMAttributeSTTest()
        {
            DICOMAttributeSTTestSuite test = new DICOMAttributeSTTestSuite();
            test.TestStreamLength();
            test.TestSet();
            
        }

        private class DICOMAttributeSTTestSuite
        {
            public DicomAttributeST CreateAttribute()
            {
                DicomAttributeST attrib = new DicomAttributeST(DicomTagDictionary.GetDicomTag(DicomTags.PartialViewDescription));
                Assert.AreEqual(0, attrib.Count);
                Assert.AreEqual(0, attrib.StreamLength);
                return attrib;
            }

            public void TestStreamLength()
            {
                DicomAttributeST attrib = CreateAttribute();
                Assert.AreEqual(0, attrib.StreamLength);

                // test StreamLength when parent collection is assigned
                DicomAttributeCollection parent = new DicomAttributeCollection();
                parent.SpecificCharacterSet = "ISO_IR 100";
                attrib.ParentCollection = parent;
                Assert.AreEqual(0, attrib.StreamLength);

                attrib.SetNullValue();
                Assert.AreEqual(0, attrib.StreamLength);

                attrib.SetStringValue("TEST");
                Assert.AreEqual(4, attrib.StreamLength);
            }
            

            public void TestSet()
            {
                DicomAttributeST attrib = CreateAttribute();
                attrib.SetStringValue("10000");
                Assert.AreEqual(1, attrib.Count);
                Assert.AreEqual(6, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue("10000\\50694");
                Assert.AreEqual(1, attrib.Count); // ST must be treated as single value
                Assert.AreEqual(12, attrib.StreamLength);

                attrib = CreateAttribute();
                attrib.SetStringValue(null);
                Assert.AreEqual(1, attrib.Count); // ST must be treated as single value
                Assert.AreEqual(0, attrib.StreamLength);


                attrib = CreateAttribute();
                attrib.SetNullValue();
                Assert.AreEqual(1, attrib.Count); // ST must be treated as single value
                Assert.AreEqual(0, attrib.StreamLength);


            }


        }

        #endregion


		[Test]
		public void TestCopy()
		{
			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMR(collection);
			DicomAttributeCollection copy = collection.Copy(true, true, true);
			Assert.AreEqual(collection.Count, copy.Count);

			foreach(DicomAttribute attribute in collection)
			{
				DicomAttribute attribute2 = copy[attribute.Tag];
				Assert.AreEqual(attribute2, attribute);
			}

			Assert.AreEqual(collection, copy);

			collection = new DicomAttributeCollection();
			SetupMultiframeXA(collection, 512, 512, 5);
			copy = collection.Copy(true, true, true);
			Assert.AreEqual(collection.Count, copy.Count);

			foreach (DicomAttribute attribute in collection)
			{
				DicomAttribute attribute2 = copy[attribute.Tag];
				Assert.AreEqual(attribute2, attribute);
			}

			Assert.AreEqual(collection, copy);
		}

    	#region DicomAttribute Empty/Null Tests
		[Test]
		public void TestSetEmptyValue()
		{
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestSetEmpty();

			DicomFile file = new DicomFile();
			base.SetupMultiframeXA(file.DataSet, 16, 16, 3);
			base.SetupMetaInfo(file);

			Assert.IsFalse(file.DataSet[DicomTags.PixelData].IsEmpty);
			file.DataSet[DicomTags.PixelData].SetEmptyValue();
			Assert.IsTrue(file.DataSet[DicomTags.PixelData].IsEmpty);
			Assert.AreEqual(file.DataSet[DicomTags.PixelData].StreamLength, 0);
		}

		[Test]
		public void TestSetNullValue()
		{
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestSetNull();

			DicomFile file = new DicomFile();
			base.SetupMultiframeXA(file.DataSet, 16, 16, 3);
			base.SetupMetaInfo(file);

			Assert.IsFalse(file.DataSet[DicomTags.PixelData].IsNull);
			file.DataSet[DicomTags.PixelData].SetNullValue();
			Assert.IsTrue(file.DataSet[DicomTags.PixelData].IsNull);
			Assert.AreEqual(file.DataSet[DicomTags.PixelData].StreamLength, 0);
		}

    	[Test]
        public void DicomAttributeEmptyTest()
        {
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestEmpty();
		}
		[Test]
		public void DicomAttributeNullTest() {
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestNull();
		}
		[Test]
		public void DicomAttributeNotEmptyOrNullTest() {
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestNotEmptyOrNull();
		}

		[Test]
		public void DicomAttributeSetWhenNullTest()
		{
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestSetWhenNull();
		}

		[Test]
		public void TestEmptyAttributesEqual()
		{
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestEmptyAttributesEqual();
		}

		[Test]
		public void TestNullAttributesEqual()
		{
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestNullAttributesEqual();
		}

		[Test]
		public void TestAttributesEqual()
		{
			DicomAttributeEmptyNullTestSuite test = new DicomAttributeEmptyNullTestSuite();
			test.TestAttributesEqual();
		}

		private class DicomAttributeEmptyNullTestSuite
		{
			private readonly IList<DicomTag> _tags;

			public DicomAttributeEmptyNullTestSuite() {
				uint tag = 0x2101FF00;
				List<DicomTag> tags = new List<DicomTag>();
				foreach (DicomVr vr in DicomVr.GetDicomVrList()) {
					tags.Add(new DicomTag(tag++, "dummy-tag-" + vr.Name, "dummy_tag_" + vr.Name, vr, false, 1, 1, false));
					tags.Add(new DicomTag(tag++, "dummy-mvtag-" + vr.Name, "dummy_mvtag_" + vr.Name, vr, false, 0, 10, false));
				}
				_tags = tags.AsReadOnly();
			}

			public void TestSetEmpty()
			{
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags)
				{
					TrySetValue(0, collection[tag]);
					Assert.IsFalse(collection[tag].IsEmpty);
					Assert.IsFalse(collection[tag].IsNull);
				}

				foreach (DicomAttribute attribute in collection)
				{
					attribute.SetEmptyValue();
					Assert.IsTrue(attribute.IsEmpty);
					Assert.IsFalse(attribute.IsNull);
				}
			}

			public void TestSetNull()
			{
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags)
				{
					TrySetValue(0, collection[tag]);
					Assert.IsFalse(collection[tag].IsEmpty);
					Assert.IsFalse(collection[tag].IsNull);
				}

				foreach (DicomAttribute attribute in collection)
				{
					attribute.SetNullValue();
					Assert.IsTrue(attribute.IsNull);
					Assert.IsFalse(attribute.IsEmpty);
				}
			}

			public void TestEmpty() {
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach(DicomTag tag in _tags) {
					Assert.IsFalse(collection[tag].IsNull, "non-existent tag should not be null: {0} has value \"{1}\"", tag.Name, collection[tag].ToString());
					Assert.IsTrue(collection[tag].IsEmpty, "non-existent tag should be empty: {0} has value \"{1}\"", tag.Name, collection[tag].ToString());
				}
			}

			public void TestNull() {
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags) {
					collection[tag].SetNullValue();
					Assert.IsTrue(collection[tag].IsNull, "tag with null value should be null: {0} has value \"{1}\"", tag.Name, collection[tag].ToString());
					Assert.IsFalse(collection[tag].IsEmpty, "tag with null value should not be empty: {0} has value \"{1}\"", tag.Name, collection[tag].ToString());
				}
			}

			public void TestNotEmptyOrNull() {
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags) {
					TrySetValue(0, collection[tag]);
					Assert.IsFalse(collection[tag].IsNull, "tag with values should not be null: {0} has value \"{1}\"", tag.Name, collection[tag].ToString());
					Assert.IsFalse(collection[tag].IsEmpty, "tag with values should not be empty: {0} has value \"{1}\"", tag.Name, collection[tag].ToString());
				}
			}

			public void TestSetWhenNull() {
				int index = 0;
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags) {
					collection[tag].SetNullValue();
					try {
						TrySetValue(index, collection[tag]);
					} catch(IndexOutOfRangeException) {
						Assert.Fail("Bug #1411: tag with null value could not be set at position {1}: {0}", tag.Name, index);
					}
				}
			}

			public void TestEmptyAttributesEqual()
			{
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags)
				{
					object ignore = collection[tag].Values;
				}

				DicomAttributeCollection copy = collection.Copy(true, true, true);
				foreach(DicomAttribute attribute in collection)
				{
					DicomAttribute copyAttribute = copy[attribute.Tag];
					Assert.AreEqual(attribute, copyAttribute);
				}

				Assert.AreEqual(collection, copy);
			}

			public void TestNullAttributesEqual()
			{
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags)
					collection[tag].SetNullValue();

				DicomAttributeCollection copy = collection.Copy(true, true, true);
				foreach(DicomAttribute attribute in collection)
				{
					DicomAttribute copyAttribute = copy[attribute.Tag];
					Assert.AreEqual(attribute, copyAttribute);
				}

				Assert.AreEqual(collection, copy);
			}

			public void TestAttributesEqual()
			{
				DicomAttributeCollection collection = new DicomAttributeCollection();
				foreach (DicomTag tag in _tags)
					TrySetValue(0, collection[tag]);

				DicomAttributeCollection copy = collection.Copy(true, true, true);
				foreach(DicomAttribute attribute in collection)
				{
					DicomAttribute copyAttribute = copy[attribute.Tag];
					Assert.AreEqual(attribute, copyAttribute);
				}

				Assert.AreEqual(collection, copy);
			}

			private static void TrySetValue(int index, DicomAttribute attrib)
			{
				// one of these statements should be able to put a non-null value on the attribute
				try {
					attrib.SetDateTime(index, DateTime.Now);
				} catch(DicomException) {
					try {
						attrib.SetFloat32(index, 0f);
					} catch (DicomException) {
						try {
							attrib.SetInt16(index, 0);
						} catch (DicomException) {
							try {
								attrib.SetUInt16(index, 0);
							} catch (DicomException) {
								try {
									attrib.SetUid(index, DicomUid.GenerateUid());
								} catch (DicomException) {
									try {
										attrib.SetString(index, "fdsa");
									} catch (DicomException) {
										try {
											attrib.SetString(index, "1");
										} catch (DicomException) {
											try {
												attrib.SetString(index, "11");
											} catch (DicomException) {
												try {
													attrib.SetString(index, "1111");
												} catch (DicomException) {
													try {
														attrib.SetString(index, "111111");
													} catch (DicomException) {
														try {
															attrib.SetString(index, "11111111");
														} catch (DicomException) {
															try {
																attrib.SetStringValue("asdf");
															} catch (DicomException) {
																try {
																	DicomSequenceItem item = new DicomSequenceItem();
																	attrib.AddSequenceItem(item);
																	item[DicomTags.InstanceNumber].SetString(index, "1");
																} catch (DicomException) {
																	Assert.Fail("Test case deficiency: doesn't know how to set an attribute of VR {0}", attrib.Tag.VR.Name);
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion

	}
}

#endif
