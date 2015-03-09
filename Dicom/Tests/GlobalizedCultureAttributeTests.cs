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

#if UNIT_TESTS && !__MonoCS__

// ReSharper disable InconsistentNaming

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	public abstract class GlobalizedCultureAttributeTest
	{
		private readonly CultureInfo _testCultureInfo;
		private readonly CultureInfo _originalCultureInfo;

		protected GlobalizedCultureAttributeTest(CultureInfo cultureInfo)
		{
			_originalCultureInfo = Thread.CurrentThread.CurrentCulture;
			_testCultureInfo = cultureInfo;

			Trace.WriteLine(string.Format("Using Culture \"{0}\"", _testCultureInfo));
			Trace.WriteLine(string.Format("\tPositive: {0}", (123456789.0987654321).ToString("n", _testCultureInfo)));
			Trace.WriteLine(string.Format("\tNegative: {0}", (-123456789.0987654321).ToString("n", _testCultureInfo)));
			Trace.WriteLine(string.Format("\tDateTime: {0}", DateTime.Now.ToString("F", _testCultureInfo)));
		}

		protected void SetCurrentCulture()
		{
			Thread.CurrentThread.CurrentCulture = _testCultureInfo;
		}

		protected void ResetCurrentCulture()
		{
			Thread.CurrentThread.CurrentCulture = _originalCultureInfo;
		}

		#region Binary Numeric Attributes

		[Test]
		public void TestAttributeSL()
		{
			DicomVr vr = DicomVr.SLvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "-1", -1);
				SetAndAssertInt16(vr, "32767", 32767);
				SetAndAssertInt16(vr, "-32768", -32768);

				SetAndAssertInt32(vr, "0", 0);
				SetAndAssertInt32(vr, "1", 1);
				SetAndAssertInt32(vr, "-1", -1);
				SetAndAssertInt32(vr, "2147483647", 2147483647);
				SetAndAssertInt32(vr, "-2147483648", -2147483648);

				SetAndAssertInt64(vr, "0", 0);
				SetAndAssertInt64(vr, "1", 1);
				SetAndAssertInt64(vr, "-1", -1);
				SetAndAssertInt64(vr, "2147483647", 2147483647);
				SetAndAssertInt64(vr, "-2147483648", -2147483648);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "65535", 65535);

				SetAndAssertUInt32(vr, "0", 0);
				SetAndAssertUInt32(vr, "1", 1);
				SetAndAssertUInt32(vr, "2147483647", 2147483647);

				SetAndAssertUInt64(vr, "0", 0);
				SetAndAssertUInt64(vr, "1", 1);
				SetAndAssertUInt64(vr, "2147483647", 2147483647);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeSS()
		{
			DicomVr vr = DicomVr.SSvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "-1", -1);
				SetAndAssertInt16(vr, "32767", 32767);
				SetAndAssertInt16(vr, "-32768", -32768);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "32767", 32767);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeAT()
		{
			DicomVr vr = DicomVr.ATvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "7FFF", 0x7FFF);

				SetAndAssertInt32(vr, "0", 0);
				SetAndAssertInt32(vr, "1", 1);
				SetAndAssertInt32(vr, "7FFFFFFF", 0x7FFFFFFF);

				SetAndAssertInt64(vr, "0", 0);
				SetAndAssertInt64(vr, "1", 1);
				SetAndAssertInt64(vr, "FFFFFFFF", 0xFFFFFFFF);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "FFFF", 0xFFFF);

				SetAndAssertUInt32(vr, "0", 0);
				SetAndAssertUInt32(vr, "1", 1);
				SetAndAssertUInt32(vr, "FFFFFFFF", 0xFFFFFFFF);

				SetAndAssertUInt64(vr, "0", 0);
				SetAndAssertUInt64(vr, "1", 1);
				SetAndAssertUInt64(vr, "FFFFFFFF", 0xFFFFFFFF);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeUL()
		{
			DicomVr vr = DicomVr.ULvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "32767", 32767);

				SetAndAssertInt32(vr, "0", 0);
				SetAndAssertInt32(vr, "1", 1);
				SetAndAssertInt32(vr, "2147483647", 2147483647);

				SetAndAssertInt64(vr, "0", 0);
				SetAndAssertInt64(vr, "1", 1);
				SetAndAssertInt64(vr, "4294967295", 4294967295);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "65535", 65535);

				SetAndAssertUInt32(vr, "0", 0);
				SetAndAssertUInt32(vr, "1", 1);
				SetAndAssertUInt32(vr, "4294967295", 4294967295);

				SetAndAssertUInt64(vr, "0", 0);
				SetAndAssertUInt64(vr, "1", 1);
				SetAndAssertUInt64(vr, "4294967295", 4294967295);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeUS()
		{
			DicomVr vr = DicomVr.USvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "32767", 32767);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "65535", 65535);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeFL()
		{
			DicomVr vr = DicomVr.FLvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertFloat32(vr, "0", 0.00000000f);
				SetAndAssertFloat32(vr, "2.048", 2.048f);
				SetAndAssertFloat32(vr, "-2.048", -2.048f);
				SetAndAssertFloat32(vr, "12345.6", 12345.6f);
				SetAndAssertFloat32(vr, "-12345.6", -12345.6f);
				SetAndAssertFloat32(vr, "1.797693E+38", 1.797693e38f);
				SetAndAssertFloat32(vr, "-1.797693E+38", -1.797693e38f);
				SetAndAssertFloat32(vr, "1.797693E-38", 1.797693e-38f);
				SetAndAssertFloat32(vr, "-1.797693E-38", -1.797693e-38f);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeFD()
		{
			DicomVr vr = DicomVr.FDvr;

			SetCurrentCulture();
			try
			{
				//there seems to invariably be some precision loss with floats
				//SetAndAssertFloat32(vr, "0", 0.00000000f);
				//SetAndAssertFloat32(vr, "2.048", 2.048f);
				//SetAndAssertFloat32(vr, "-2.048", -2.048f);
				//SetAndAssertFloat32(vr, "12345.6", 12345.6f);
				//SetAndAssertFloat32(vr, "-12345.6", -12345.6f);
				//SetAndAssertFloat32(vr, "1.797693E+38", 1.797693e38f);
				//SetAndAssertFloat32(vr, "-1.797693E+38", -1.797693e38f);
				//SetAndAssertFloat32(vr, "1.797693E-38", 1.797693e-38f);
				//SetAndAssertFloat32(vr, "-1.797693E-38", -1.797693e-38f);

				SetAndAssertFloat64(vr, "0", 0.00000000);
				SetAndAssertFloat64(vr, "12.3456789", 12.3456789);
				SetAndAssertFloat64(vr, "-12.3456789", -12.3456789);
				SetAndAssertFloat64(vr, "12345.6789", 12345.6789);
				SetAndAssertFloat64(vr, "-12345.6789", -12345.6789);
				SetAndAssertFloat64(vr, "1.7976931349E+38", 1.7976931349e38);
				SetAndAssertFloat64(vr, "-1.797693135E+38", -1.797693135e38);
				SetAndAssertFloat64(vr, "1.7976931349E-38", 1.7976931349e-38);
				SetAndAssertFloat64(vr, "-1.797693135E-38", -1.797693135e-38);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		#endregion

		#region Text-Based Numberic Attributes

		[Test]
		public void TestAttributeIS()
		{
			DicomVr vr = DicomVr.ISvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "-1", -1);
				SetAndAssertInt16(vr, "32767", 32767);
				SetAndAssertInt16(vr, "-32768", -32768);

				SetAndAssertInt32(vr, "0", 0);
				SetAndAssertInt32(vr, "1", 1);
				SetAndAssertInt32(vr, "-1", -1);
				SetAndAssertInt32(vr, "2147483647", 2147483647);
				SetAndAssertInt32(vr, "-2147483648", -2147483648);

				SetAndAssertInt64(vr, "0", 0);
				SetAndAssertInt64(vr, "1", 1);
				SetAndAssertInt64(vr, "-1", -1);
				SetAndAssertInt64(vr, "2147483647", 2147483647);
				SetAndAssertInt64(vr, "-2147483648", -2147483648);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "65535", 65535);

				SetAndAssertUInt32(vr, "0", 0);
				SetAndAssertUInt32(vr, "1", 1);
				SetAndAssertUInt32(vr, "2147483647", 2147483647);

				SetAndAssertUInt64(vr, "0", 0);
				SetAndAssertUInt64(vr, "1", 1);
				SetAndAssertUInt64(vr, "2147483647", 2147483647);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		[Test]
		public void TestAttributeDS()
		{
			DicomVr vr = DicomVr.DSvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertInt16(vr, "0", 0);
				SetAndAssertInt16(vr, "1", 1);
				SetAndAssertInt16(vr, "-1", -1);
				SetAndAssertInt16(vr, "32767", 32767);
				SetAndAssertInt16(vr, "-32768", -32768);

				SetAndAssertInt32(vr, "0", 0);
				SetAndAssertInt32(vr, "1", 1);
				SetAndAssertInt32(vr, "-1", -1);
				SetAndAssertInt32(vr, "2147483647", 2147483647);
				SetAndAssertInt32(vr, "-2147483648", -2147483648);

				SetAndAssertInt64(vr, "0", 0);
				SetAndAssertInt64(vr, "1", 1);
				SetAndAssertInt64(vr, "-1", -1);
				SetAndAssertInt64(vr, "2147483647", 2147483647);
				SetAndAssertInt64(vr, "-2147483648", -2147483648);

				SetAndAssertUInt16(vr, "0", 0);
				SetAndAssertUInt16(vr, "1", 1);
				SetAndAssertUInt16(vr, "65535", 65535);

				SetAndAssertUInt32(vr, "0", 0);
				SetAndAssertUInt32(vr, "1", 1);
				SetAndAssertUInt32(vr, "2147483647", 2147483647);

				SetAndAssertUInt64(vr, "0", 0);
				SetAndAssertUInt64(vr, "1", 1);
				SetAndAssertUInt64(vr, "2147483647", 2147483647);

				//there seems to invariably be some precision loss with floats
				//SetAndAssertFloat32(vr, "0", 0.00000000f);
				//SetAndAssertFloat32(vr, "2.048", 2.048f);
				//SetAndAssertFloat32(vr, "-2.048", -2.048f);
				//SetAndAssertFloat32(vr, "12345.6", 12345.6f);
				//SetAndAssertFloat32(vr, "-12345.6", -12345.6f);
				//SetAndAssertFloat32(vr, "1.797693E+38", 1.797693e38f);
				//SetAndAssertFloat32(vr, "-1.797693E+38", -1.797693e38f);
				//SetAndAssertFloat32(vr, "1.797693E-38", 1.797693e-38f);
				//SetAndAssertFloat32(vr, "-1.797693E-38", -1.797693e-38f);

				SetAndAssertFloat64(vr, "0", 0.00000000);
				SetAndAssertFloat64(vr, "12.3456789", 12.3456789);
				SetAndAssertFloat64(vr, "-12.3456789", -12.3456789);
				SetAndAssertFloat64(vr, "12345.6789", 12345.6789);
				SetAndAssertFloat64(vr, "-12345.6789", -12345.6789);
				SetAndAssertFloat64(vr, "1.7976931349E+38", 1.7976931349e38);
				SetAndAssertFloat64(vr, "-1.797693135E+38", -1.797693135e38);
				SetAndAssertFloat64(vr, "1.7976931349E-38", 1.7976931349e-38);
				SetAndAssertFloat64(vr, "-1.797693135E-38", -1.797693135e-38);
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		#endregion

		#region Date/Time Attributes

		[Test]
		public void TestAttributeDT()
		{
			DicomVr vr = DicomVr.DTvr;

			SetCurrentCulture();
			try
			{
				SetAndAssertDateTime(vr, @"20090101000000+0000", new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Utc));
				SetAndAssertDateTime(vr, @"20080101000000-0500", new DateTime(2007, 12, 31, 19, 0, 0, DateTimeKind.Utc));
				SetAndAssertDateTime(vr, @"20070101000000+0500", new DateTime(2007, 1, 1, 5, 0, 0, DateTimeKind.Utc));
				SetAndAssertDateTime(vr, @"20060101000000-0000", new DateTime(2006, 1, 1, 0, 0, 0, DateTimeKind.Utc));

				SetAndAssertDateTime(vr, new DateTime(2009, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc), "20090102030405.006000");
				SetAndAssertDateTime(vr, new DateTime(2009, 1, 2, 3, 4, 5, 6, DateTimeKind.Local), "20090102030405.006000");
			}
			finally
			{
				ResetCurrentCulture();
			}
		}

		#endregion

		#region Static Helpers

		private static void SetAndAssertInt16(DicomVr vr, string szValue, short sValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (short) vs dataset[tag].ToString()");
			Assert.AreEqual(sValue, dataset[tag].GetInt16(0, -123), "SetStringValue (short) vs dataset[tag].GetInt16(...)");

			dataset[tag].SetInt16(0, sValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as short)", vr.Name, sValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetInt16 vs dataset[tag].ToString()");
			Assert.AreEqual(sValue, dataset[tag].GetInt16(0, -123), "SetInt16 vs dataset[tag].GetInt16(...)");

			dataset[tag].AppendInt16(sValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as short)", vr.Name, sValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendInt16 vs dataset[tag].ToString()");
			Assert.AreEqual(sValue, dataset[tag].GetInt16(1, -123), "AppendInt16 vs dataset[tag].GetInt16(...)");
		}

		private static void SetAndAssertInt32(DicomVr vr, string szValue, int iValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (int) vs dataset[tag].ToString()");
			Assert.AreEqual(iValue, dataset[tag].GetInt32(0, -123), "SetStringValue (int) vs dataset[tag].GetInt32(...)");

			dataset[tag].SetInt32(0, iValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as int)", vr.Name, iValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetInt32 vs dataset[tag].ToString()");
			Assert.AreEqual(iValue, dataset[tag].GetInt32(0, -123), "SetInt32 vs dataset[tag].GetInt32(...)");

			dataset[tag].AppendInt32(iValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as int)", vr.Name, iValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendInt32 vs dataset[tag].ToString()");
			Assert.AreEqual(iValue, dataset[tag].GetInt32(1, -123), "AppendInt32 vs dataset[tag].GetInt32(...)");
		}

		private static void SetAndAssertInt64(DicomVr vr, string szValue, long lValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (long) vs dataset[tag].ToString()");
			Assert.AreEqual(lValue, dataset[tag].GetInt64(0, -123), "SetStringValue (long) vs dataset[tag].GetInt64(...)");

			dataset[tag].SetInt64(0, lValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as long)", vr.Name, lValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetInt64 vs dataset[tag].ToString()");
			Assert.AreEqual(lValue, dataset[tag].GetInt64(0, -123), "SetInt64 vs dataset[tag].GetInt64(...)");

			dataset[tag].AppendInt64(lValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as long)", vr.Name, lValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendInt64 vs dataset[tag].ToString()");
			Assert.AreEqual(lValue, dataset[tag].GetInt64(1, -123), "AppendInt64 vs dataset[tag].GetInt64(...)");
		}

		private static void SetAndAssertUInt16(DicomVr vr, string szValue, ushort usValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (ushort) vs dataset[tag].ToString()");
			Assert.AreEqual(usValue, dataset[tag].GetUInt16(0, 123), "SetStringValue (ushort) vs dataset[tag].GetUInt16(...)");

			dataset[tag].SetUInt16(0, usValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as ushort)", vr.Name, usValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetUInt16 vs dataset[tag].ToString()");
			Assert.AreEqual(usValue, dataset[tag].GetUInt16(0, 123), "SetUInt16 vs dataset[tag].GetUInt16(...)");

			dataset[tag].AppendUInt16(usValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as ushort)", vr.Name, usValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendUInt16 vs dataset[tag].ToString()");
			Assert.AreEqual(usValue, dataset[tag].GetUInt16(1, 123), "AppendUInt16 vs dataset[tag].GetUInt16(...)");
		}

		private static void SetAndAssertUInt32(DicomVr vr, string szValue, uint uiValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (uint) vs dataset[tag].ToString()");
			Assert.AreEqual(uiValue, dataset[tag].GetUInt32(0, 123), "SetStringValue (uint) vs dataset[tag].GetUInt32(...)");

			dataset[tag].SetUInt32(0, uiValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as uint)", vr.Name, uiValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetUInt32 vs dataset[tag].ToString()");
			Assert.AreEqual(uiValue, dataset[tag].GetUInt32(0, 123), "SetUInt32 vs dataset[tag].GetUInt32(...)");

			dataset[tag].AppendUInt32(uiValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as uint)", vr.Name, uiValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendUInt32 vs dataset[tag].ToString()");
			Assert.AreEqual(uiValue, dataset[tag].GetUInt32(1, 123), "AppendUInt32 vs dataset[tag].GetUInt32(...)");
		}

		private static void SetAndAssertUInt64(DicomVr vr, string szValue, ulong ulValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (ulong) vs dataset[tag].ToString()");
			Assert.AreEqual(ulValue, dataset[tag].GetUInt64(0, 123), "SetStringValue (ulong) vs dataset[tag].GetUInt64(...)");

			dataset[tag].SetUInt64(0, ulValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as ulong)", vr.Name, ulValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetUInt64 vs dataset[tag].ToString()");
			Assert.AreEqual(ulValue, dataset[tag].GetUInt64(0, 123), "SetUInt64 vs dataset[tag].GetUInt64(...)");

			dataset[tag].AppendUInt64(ulValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as ulong)", vr.Name, ulValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendUInt64 vs dataset[tag].ToString()");
			Assert.AreEqual(ulValue, dataset[tag].GetUInt64(1, 123), "AppendUInt64 vs dataset[tag].GetUInt64(...)");
		}

		private static void SetAndAssertFloat32(DicomVr vr, string szValue, float fValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (float) vs dataset[tag].ToString()");
			Assert.AreEqual(fValue, dataset[tag].GetFloat32(0, -123), "SetStringValue (float) vs dataset[tag].GetFloat32(...)");

			dataset[tag].SetFloat32(0, fValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as float)", vr.Name, fValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetFloat32 vs dataset[tag].ToString()");
			Assert.AreEqual(fValue, dataset[tag].GetFloat32(0, -123), "SetFloat32 vs dataset[tag].GetFloat32(...)");

			dataset[tag].AppendFloat32(fValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as float)", vr.Name, fValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendFloat32 vs dataset[tag].ToString()");
			Assert.AreEqual(fValue, dataset[tag].GetFloat32(1, -123), "AppendFloat32 vs dataset[tag].GetFloat32(...)");
		}

		private static void SetAndAssertFloat64(DicomVr vr, string szValue, double dValue)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(szValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, szValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetStringValue (double) vs dataset[tag].ToString()");
			Assert.AreEqual(dValue, dataset[tag].GetFloat64(0, -123), "SetStringValue (double) vs dataset[tag].GetFloat64(...)");

			dataset[tag].SetFloat64(0, dValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as double)", vr.Name, dValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue, dataset[tag].ToString(), "SetFloat64 vs dataset[tag].ToString()");
			Assert.AreEqual(dValue, dataset[tag].GetFloat64(0, -123), "SetFloat64 vs dataset[tag].GetFloat64(...)");

			dataset[tag].AppendFloat64(dValue);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (append as double)", vr.Name, dValue));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(szValue + @"\" + szValue, dataset[tag].ToString(), "AppendFloat64 vs dataset[tag].ToString()");
			Assert.AreEqual(dValue, dataset[tag].GetFloat64(1, -123), "AppendFloat64 vs dataset[tag].GetFloat64(...)");
		}

		private static void SetAndAssertDateTime(DicomVr vr, string value, DateTime expected)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetStringValue(value);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as string)", vr.Name, value));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(value, dataset[tag].ToString());
			Assert.AreEqual(expected, dataset[tag].GetDateTime(0));
		}

		private static void SetAndAssertDateTime(DicomVr vr, DateTime value, string expected)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			DicomTag tag = CreateTag(1, vr);
			dataset[tag].SetDateTime(0, value);
			Trace.WriteLine(string.Format("Testing {0} attribute with {1} (set as DateTime)", vr.Name, value));
			Trace.WriteLine(string.Format("Attribute Value is: \"{0}\"", dataset[tag].ToString()));
			Assert.AreEqual(value, dataset[tag].GetDateTime(0));
			Assert.AreEqual(expected, dataset[tag].ToString());
		}

		private static DicomTag CreateTag(int id, DicomVr vr)
		{
			uint tagValue = (uint) id;
			tagValue = ((((tagValue >> 16) << 1) + 0x11) << 16) + (tagValue & 0x0000ffff); // this generates a serial private tag
			return new DicomTag(tagValue, string.Format("({0:x4},{1:x4})", (tagValue >> 16), (tagValue & 0x0000ffff)), string.Format("tag{0:x8}", tagValue), vr, true, 0, 10, false);
		}

		#endregion
	}

	[TestFixture]
	public class InvariantCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: control (read: if this doesn't pass, there's something wrong)
		public InvariantCultureAttributeTest() : base(CultureInfo.InvariantCulture) {}
	}

	[TestFixture]
	public class GermanCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: known issues with decimal formatting
		public GermanCultureAttributeTest() : base(CultureInfo.GetCultureInfo("de-DE")) {}
	}

	[TestFixture]
	public class HungarianCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: known issues with decimal formatting
		public HungarianCultureAttributeTest() : base(CultureInfo.GetCultureInfo("hu-HU")) {}
	}

	[TestFixture]
	public class FrenchCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: possible issues with decimal formatting
		public FrenchCultureAttributeTest() : base(CultureInfo.GetCultureInfo("fr-FR")) {}
	}

	[TestFixture]
	public class BrazilianPortugueseCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: known issues with decimal formatting
		public BrazilianPortugueseCultureAttributeTest() : base(CultureInfo.GetCultureInfo("pt-BR")) {}
	}

	[TestFixture]
	public class ChineseCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: known issues, although probably Workstation specific
		public ChineseCultureAttributeTest() : base(CultureInfo.GetCultureInfo("zh-CN")) {}
	}

	[TestFixture]
	public class JapaneseCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: past known issues
		public JapaneseCultureAttributeTest() : base(CultureInfo.GetCultureInfo("ja-JP")) {}
	}

	[TestFixture]
	public class AmericanEnglishCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: control (read: if this doesn't pass, there's something wrong)
		public AmericanEnglishCultureAttributeTest() : base(CultureInfo.GetCultureInfo("en-US")) {}
	}

	[TestFixture]
	public class CanadianEnglishCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: control (read: if this doesn't pass, there's something wrong)
		public CanadianEnglishCultureAttributeTest() : base(CultureInfo.GetCultureInfo("en-CA")) {}
	}

	[TestFixture]
	public class CanadianFrenchCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: possible issues with decimal formatting
		public CanadianFrenchCultureAttributeTest() : base(CultureInfo.GetCultureInfo("fr-CA")) {}
	}

	[TestFixture]
	public class BackwardsLandCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: to fully exercise boundary condition culture infos
		public BackwardsLandCultureAttributeTest() : base(CreateCultureInfo()) {}

		private const string _cultureName = "x-cc-test-bckwrds-lnd";

		private static CultureInfo CreateCultureInfo()
		{
			try
			{
				return new CultureInfo(_cultureName);
			}
			catch (ArgumentException)
			{
				try
				{
					CultureAndRegionInfoBuilder cultureAndRegionInfoBuilder = new CultureAndRegionInfoBuilder(_cultureName, CultureAndRegionModifiers.None);
					cultureAndRegionInfoBuilder.LoadDataFromCultureInfo(CultureInfo.InvariantCulture);
					cultureAndRegionInfoBuilder.LoadDataFromRegionInfo(new RegionInfo("US"));
					cultureAndRegionInfoBuilder.ThreeLetterWindowsRegionName = "bwl";
					cultureAndRegionInfoBuilder.ThreeLetterISORegionName = "bwl";
					cultureAndRegionInfoBuilder.TwoLetterISORegionName = "bl";
					cultureAndRegionInfoBuilder.RegionEnglishName = "Backwards Land";
					cultureAndRegionInfoBuilder.RegionNativeName = "Wark";
					cultureAndRegionInfoBuilder.NumberFormat.NegativeSign = "+";
					cultureAndRegionInfoBuilder.NumberFormat.PositiveSign = "-";
					cultureAndRegionInfoBuilder.NumberFormat.NumberDecimalSeparator = ";";
					cultureAndRegionInfoBuilder.NumberFormat.NumberGroupSeparator = ":";
					cultureAndRegionInfoBuilder.NumberFormat.NumberGroupSizes = new int[] {2, 3, 4, 5};
					cultureAndRegionInfoBuilder.NumberFormat.CurrencyDecimalSeparator = ";";
					cultureAndRegionInfoBuilder.NumberFormat.CurrencyGroupSeparator = ":";
					cultureAndRegionInfoBuilder.NumberFormat.CurrencyGroupSizes = new int[] {2, 3, 4, 5};
					cultureAndRegionInfoBuilder.NumberFormat.PercentDecimalSeparator = ";";
					cultureAndRegionInfoBuilder.NumberFormat.PercentGroupSeparator = ":";
					cultureAndRegionInfoBuilder.NumberFormat.PercentGroupSizes = new int[] {2, 3, 4, 5};
					cultureAndRegionInfoBuilder.Register();
					return new CultureInfo(_cultureName);
				}
				catch (UnauthorizedAccessException)
				{
					Assert.Ignore("Unable to register custom culture definition. You may need to run NUnit as an administrator.");
					return CultureInfo.InvariantCulture;
				}
			}
		}

		~BackwardsLandCultureAttributeTest()
		{
			try
			{
				CultureAndRegionInfoBuilder.Unregister(_cultureName);
			}
			catch (ArgumentException) {}
		}
	}

	[TestFixture]
	public class CandyMountainCultureAttributeTest : GlobalizedCultureAttributeTest
	{
		// Justification: to fully exercise boundary condition culture infos
		public CandyMountainCultureAttributeTest() : base(CreateCultureInfo()) {}

		private const string _cultureName = "x_cc";

		private static CultureInfo CreateCultureInfo()
		{
			try
			{
				return new CultureInfo(_cultureName);
			}
			catch (ArgumentException)
			{
				try
				{
					CultureAndRegionInfoBuilder cultureAndRegionInfoBuilder = new CultureAndRegionInfoBuilder(_cultureName, CultureAndRegionModifiers.None);
					cultureAndRegionInfoBuilder.LoadDataFromCultureInfo(CultureInfo.InvariantCulture);
					cultureAndRegionInfoBuilder.LoadDataFromRegionInfo(new RegionInfo("US"));
					cultureAndRegionInfoBuilder.ThreeLetterWindowsRegionName = "cmt";
					cultureAndRegionInfoBuilder.ThreeLetterISORegionName = "cmt";
					cultureAndRegionInfoBuilder.TwoLetterISORegionName = "mc";
					cultureAndRegionInfoBuilder.RegionEnglishName = "Candy Mountain";
					cultureAndRegionInfoBuilder.RegionNativeName = "Come inside the cave, Charlie";
					cultureAndRegionInfoBuilder.NumberFormat.NegativeSign = "++--";
					cultureAndRegionInfoBuilder.NumberFormat.PositiveSign = "--++";
					cultureAndRegionInfoBuilder.NumberFormat.NumberDecimalSeparator = ";";
					cultureAndRegionInfoBuilder.NumberFormat.NumberGroupSeparator = ":";
					cultureAndRegionInfoBuilder.NumberFormat.NumberGroupSizes = new int[] {2, 3, 4, 5};
					cultureAndRegionInfoBuilder.NumberFormat.CurrencyDecimalSeparator = ";";
					cultureAndRegionInfoBuilder.NumberFormat.CurrencyGroupSeparator = ":";
					cultureAndRegionInfoBuilder.NumberFormat.CurrencyGroupSizes = new int[] {2, 3, 4, 5};
					cultureAndRegionInfoBuilder.NumberFormat.PercentDecimalSeparator = ";";
					cultureAndRegionInfoBuilder.NumberFormat.PercentGroupSeparator = ":";
					cultureAndRegionInfoBuilder.NumberFormat.PercentGroupSizes = new int[] {2, 3, 4, 5};
					cultureAndRegionInfoBuilder.Register();
					return new CultureInfo(_cultureName);
				}
				catch (UnauthorizedAccessException)
				{
					Assert.Ignore("Unable to register custom culture definition. You may need to run NUnit as an administrator.");
					return CultureInfo.InvariantCulture;
				}
			}
		}

		~CandyMountainCultureAttributeTest()
		{
			try
			{
				CultureAndRegionInfoBuilder.Unregister(_cultureName);
			}
			catch (ArgumentException) {}
		}
	}
}

// ReSharper restore InconsistentNaming

#endif