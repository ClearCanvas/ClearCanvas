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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Automation.Tests
{
	internal abstract class AbstractDataContractTest
	{
		/// <summary>
		/// Used for testing backward compatibility of current response contracts sent to legacy clients.
		/// </summary>
		/// <typeparam name="TLegacy">The old data contract definition that the legacy client would have used.</typeparam>
		/// <param name="currentObject">The object to test using the current data contract type.</param>
		/// <param name="dumpSerializedData">Specifies whether or not the serialized data should be dumped to the console even if errors do not occur during serialization.</param>
		/// <returns>The equivalent of <paramref name="currentObject"/> deserialized as a <typeparamref name="TLegacy"/>.</returns>
		protected static TLegacy TestResponseContractCompatibility<TLegacy>(object currentObject, bool dumpSerializedData = false)
			where TLegacy : class
		{
			Platform.CheckForNullReference(currentObject, "currentObject");
			Platform.CheckTrue(currentObject.GetType() != typeof (TLegacy), "currentObject cannot be of type TLegacy (otherwise, it wouldn't be a valid test!)");
			Platform.CheckTrue((typeof (TLegacy).Namespace ?? string.Empty).StartsWith("ClearCanvas.ImageViewer.Common.Automation.Tests"), "TLegacy doesn't appear to be a legacy data contract defined for the sole purpose of unit tests!");

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
		protected static TCurrent TestRequestContractCompatibility<TCurrent>(object legacyObject, bool dumpSerializedData = false)
			where TCurrent : class
		{
			Platform.CheckForNullReference(legacyObject, "legacyObject");
			Platform.CheckTrue(legacyObject.GetType() != typeof (TCurrent), "legacyObject cannot be of type TCurrent (otherwise, it wouldn't be a valid test!)");
			Platform.CheckFalse((typeof (TCurrent).Namespace ?? string.Empty).StartsWith("ClearCanvas.ImageViewer.Common.Automation.Tests"), "TCurrent appears to be a legacy data contract defined for the sole purpose of unit tests!");

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

		protected static void AssertAreSequenceEqual<TLeft, TRight>(IList<TLeft> expected, IList<TRight> actual, Func<TLeft, TRight, bool> comparer, string message = null, params object[] args)
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