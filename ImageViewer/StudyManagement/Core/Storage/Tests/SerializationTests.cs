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

using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.Common.WorkItem;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.Tests
{
	[TestFixture]
	public class SerializationTests
	{
		#region WorkItemRequest

		[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
		[WorkItemRequestDataContract("b07a3d7a-2909-4ed1-82ce-8a1ab4f30446")]
		private class TestRequestA : WorkItemRequest
		{
			public override WorkItemConcurrency ConcurrencyType
			{
				get { return WorkItemConcurrency.Exclusive; }
			}

			public override string ActivityDescription
			{
				get { return string.Empty; }
			}

			public override string ActivityTypeString
			{
				get { return string.Empty; }
			}
		}

		[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
		[WorkItemRequestDataContract("f6c90b95-d631-4b78-b9a1-a786dc23512a")]
		private class TestRequestB : WorkItemRequest
		{
			public override WorkItemConcurrency ConcurrencyType
			{
				get { return WorkItemConcurrency.Exclusive; }
			}

			public override string ActivityDescription
			{
				get { return string.Empty; }
			}

			public override string ActivityTypeString
			{
				get { return string.Empty; }
			}
		}

		[Test]
		public void Test_WorkItemRequest_serialize_null()
		{
			var a = Serializer.SerializeWorkItemRequest(null);
			Assert.IsNull(a);
		}

		[Test]
		public void Test_WorkItemRequest_deserialize_null()
		{
			var a = Serializer.DeserializeWorkItemRequest(null);
			Assert.IsNull(a);
		}

		[Test]
		public void Test_WorkItemRequest_Roundtrip_Null()
		{
			var a = Serializer.SerializeWorkItemRequest(null);
			var b = Serializer.DeserializeWorkItemRequest(a);
			Assert.IsNull(b);
		}

		[Test]
		public void Test_WorkItemRequest_subclass_roundtrip()
		{
			var requestA = new TestRequestA();
			var requestB = new TestRequestB();

			var a = Serializer.SerializeWorkItemRequest(requestA);
			var b = Serializer.SerializeWorkItemRequest(requestB);

			// ensure that we get instances of the correct sub-classes back, even if we ask for the base-class
			Assert.IsInstanceOf(typeof (TestRequestA), Serializer.DeserializeWorkItemRequest(a));
			Assert.IsInstanceOf(typeof (TestRequestB), Serializer.DeserializeWorkItemRequest(b));
		}

		#endregion

		#region WorkItemProgress

		[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
		[WorkItemProgressDataContract("{A5D1DB11-43EC-45D3-82CF-41597B3A8286}")]
		private class TestProgressA : WorkItemProgress {}

		[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
		[WorkItemProgressDataContract("{FA5F4689-B8BE-45FA-A67B-A39D82C2C455}")]
		private class TestProgressB : WorkItemProgress {}

		[Test]
		public void Test_WorkItemProgress_serialize_null()
		{
			var a = Serializer.SerializeWorkItemProgress(null);
			Assert.IsNull(a);
		}

		[Test]
		public void Test_WorkItemProgress_deserialize_null()
		{
			var a = Serializer.DeserializeWorkItemProgress(null);
			Assert.IsNull(a);
		}

		[Test]
		public void Test_WorkItemProgress_Roundtrip_Null()
		{
			var a = Serializer.SerializeWorkItemProgress(null);
			var b = Serializer.DeserializeWorkItemProgress(a);
			Assert.IsNull(b);
		}

		[Test]
		public void Test_WorkItemProgress_subclass_roundtrip()
		{
			var requestA = new TestProgressA();
			var requestB = new TestProgressB();

			var a = Serializer.SerializeWorkItemProgress(requestA);
			var b = Serializer.SerializeWorkItemProgress(requestB);

			// ensure that we get instances of the correct sub-classes back, even if we ask for the base-class
			Assert.IsInstanceOf(typeof (TestProgressA), Serializer.DeserializeWorkItemProgress(a));
			Assert.IsInstanceOf(typeof (TestProgressB), Serializer.DeserializeWorkItemProgress(b));
		}

		#endregion
	}
}

#endif