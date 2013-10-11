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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Automation.Tests
{
	[TestFixture]
	internal class DataContractTests
	{
		[Test]
		public void TestOpenStudiesRequestContractCompatibility1()
		{
			var referenceObject = new LegacyOpenStudiesRequestCirca20
			                      	{
			                      		ActivateIfAlreadyOpen = false,
			                      		StudiesToOpen = new List<OpenStudyInfo>
			                      		                	{
			                      		                		new OpenStudyInfo("1.234"),
			                      		                		new OpenStudyInfo("56.78"),
			                      		                		new OpenStudyInfo("9.0")
			                      		                	}
			                      	};

			var currentObject = TestRequestContractCompatibility<OpenStudiesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.ActivateIfAlreadyOpen, currentObject.ActivateIfAlreadyOpen);
			AssertAreSequenceEqual(referenceObject.StudiesToOpen, currentObject.StudiesToOpen, (a, b) => a.StudyInstanceUid == b.StudyInstanceUid);
		}

		[Test]
		public void TestOpenStudiesRequestContractCompatibility2()
		{
			var referenceObject = new LegacyOpenStudiesRequestCirca30
			                      	{
			                      		ActivateIfAlreadyOpen = false,
			                      		ReportFaultToUser = true,
			                      		StudiesToOpen = new List<OpenStudyInfo>
			                      		                	{
			                      		                		new OpenStudyInfo("1.234"),
			                      		                		new OpenStudyInfo("56.78"),
			                      		                		new OpenStudyInfo("9.0")
			                      		                	}
			                      	};

			var currentObject = TestRequestContractCompatibility<OpenStudiesRequest>(referenceObject);
			Assert.AreEqual(referenceObject.ActivateIfAlreadyOpen, currentObject.ActivateIfAlreadyOpen);
			Assert.AreEqual(referenceObject.ReportFaultToUser, currentObject.ReportFaultToUser);
			AssertAreSequenceEqual(referenceObject.StudiesToOpen, currentObject.StudiesToOpen, (a, b) => a.StudyInstanceUid == b.StudyInstanceUid);
		}

		[Test]
		public void TestViewerContractCompatibility1()
		{
			var referenceObject = new Viewer
			                      	{
			                      		Identifier = Guid.NewGuid(),
			                      		PrimaryStudyIdentifier = new StudyRootStudyIdentifier
			                      		                         	{
			                      		                         		AccessionNumber = "ACC",
			                      		                         		StudyInstanceUid = "123.456"
			                      		                         	}
			                      	};

			var legacyObject = TestResponseContractCompatibility<LegacyViewerPre90>(referenceObject);
			Assert.AreEqual(referenceObject.Identifier, legacyObject.Identifier);
			Assert.AreEqual(referenceObject.PrimaryStudyInstanceUid, legacyObject.PrimaryStudyInstanceUid);

			var currentObject = TestRequestContractCompatibility<Viewer>(legacyObject);
			Assert.AreEqual(referenceObject.Identifier, currentObject.Identifier);
			Assert.AreEqual(referenceObject.PrimaryStudyInstanceUid, currentObject.PrimaryStudyInstanceUid);
			Assert.IsNotNull(currentObject.PrimaryStudyIdentifier);
			Assert.AreEqual(referenceObject.PrimaryStudyIdentifier.StudyInstanceUid, currentObject.PrimaryStudyIdentifier.StudyInstanceUid);
		}

		/// <summary>
		/// Used for testing backward compatibility of current response contracts sent to legacy clients.
		/// </summary>
		/// <typeparam name="TLegacy">The old data contract definition that the legacy client would have used.</typeparam>
		/// <param name="currentObject">The object to test using the current data contract type.</param>
		/// <param name="dumpSerializedData">Specifies whether or not the serialized data should be dumped to the console even if errors do not occur during serialization.</param>
		/// <returns>The equivalent of <paramref name="currentObject"/> deserialized as a <typeparamref name="TLegacy"/>.</returns>
		private static TLegacy TestResponseContractCompatibility<TLegacy>(object currentObject, bool dumpSerializedData = false)
			where TLegacy : class
		{
			Platform.CheckForNullReference(currentObject, "currentObject");
			Platform.CheckTrue(currentObject.GetType() != typeof (TLegacy), "currentObject cannot be of type TLegacy (otherwise, it wouldn't be a valid test!)");

			var serializedData = string.Empty;
			try
			{
				using (var stream = new MemoryStream())
				{
					var serializer = new DataContractSerializer(currentObject.GetType());
					serializer.WriteObject(stream, currentObject);

					stream.Position = 0;

					serializedData = new StreamReader(stream).ReadToEnd();
					if (dumpSerializedData) Console.WriteLine(serializedData);

					stream.Position = 0;

					var deserializer = new DataContractSerializer(typeof (TLegacy));
					var legacyObject = deserializer.ReadObject(stream) as TLegacy;
					return legacyObject;
				}
			}
			catch (Exception)
			{
				if (!dumpSerializedData && !string.IsNullOrEmpty(serializedData))
				{
					const string msg = "Exception detected. Dumping serialized data...";
					Console.WriteLine(msg);
					Console.WriteLine(serializedData);
				}
				throw;
			}
		}

		/// <summary>
		/// Used for testing backward compatibility of current request contracts received from legacy clients.
		/// </summary>
		/// <typeparam name="TCurrent">The data contract definition that the current client uses.</typeparam>
		/// <param name="legacyObject">The object to test using the legacy data contract type.</param>
		/// <param name="dumpSerializedData">Specifies whether or not the serialized data should be dumped to the console even if errors do not occur during serialization.</param>
		/// <returns>The equivalent of <paramref name="legacyObject"/> deserialized as a <typeparamref name="TCurrent"/>.</returns>
		private static TCurrent TestRequestContractCompatibility<TCurrent>(object legacyObject, bool dumpSerializedData = false)
			where TCurrent : class
		{
			Platform.CheckForNullReference(legacyObject, "legacyObject");
			Platform.CheckTrue(legacyObject.GetType() != typeof (TCurrent), "legacyObject cannot be of type TLegacy (otherwise, it wouldn't be a valid test!)");

			var serializedData = string.Empty;
			try
			{
				using (var stream = new MemoryStream())
				{
					var serializer = new DataContractSerializer(legacyObject.GetType());
					serializer.WriteObject(stream, legacyObject);

					stream.Position = 0;

					serializedData = new StreamReader(stream).ReadToEnd();
					if (dumpSerializedData) Console.WriteLine(serializedData);

					stream.Position = 0;

					var deserializer = new DataContractSerializer(typeof (TCurrent));
					var currentObject = deserializer.ReadObject(stream) as TCurrent;
					return currentObject;
				}
			}
			catch (Exception)
			{
				if (!dumpSerializedData && !string.IsNullOrEmpty(serializedData))
				{
					const string msg = "Exception detected. Dumping serialized data...";
					Console.WriteLine(msg);
					Console.WriteLine(serializedData);
				}
				throw;
			}
		}

		private static void AssertAreSequenceEqual<TLeft, TRight>(IList<TLeft> expected, IList<TRight> actual, Func<TLeft, TRight, bool> comparer, string message = null, params object[] args)
		{
			if (ReferenceEquals(expected, null))
			{
				Assert.IsTrue(ReferenceEquals(actual, null), message, args);
				return;
			}

			Assert.IsFalse(ReferenceEquals(actual, null), message, args);
			Assert.AreEqual(expected.Count, actual.Count);

			for (var n = 0; n < expected.Count; ++n)
				Assert.IsTrue(comparer(expected[n], actual[n]), message, args);
		}
	}
}

#endif