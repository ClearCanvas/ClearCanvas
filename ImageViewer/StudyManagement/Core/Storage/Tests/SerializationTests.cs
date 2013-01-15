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


using NUnit.Framework;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.Tests
{
	[TestFixture]
	public class SerializationTests
	{
		[WorkItemRequestDataContract("b07a3d7a-2909-4ed1-82ce-8a1ab4f30446")]
		class TestRequestA : WorkItemRequest
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

		[WorkItemRequestDataContract("f6c90b95-d631-4b78-b9a1-a786dc23512a")]
		class TestRequestB : WorkItemRequest
		{
		    public override WorkItemConcurrency ConcurrencyType
		    {
		        get { return WorkItemConcurrency.Exclusive;}
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
		public void Test_WorkItemRequest_subclass_roundtrip()
		{
			var requestA = new TestRequestA();
			var requestB = new TestRequestB();

			var a = Serializer.SerializeWorkItemRequest(requestA);
			var b = Serializer.SerializeWorkItemRequest(requestB);

			// ensure that we get instances of the correct sub-classes back, even if we ask for the base-class
            Assert.IsInstanceOf(typeof(TestRequestA), Serializer.DeserializeWorkItemRequest(a));
            Assert.IsInstanceOf(typeof(TestRequestB), Serializer.DeserializeWorkItemRequest(b));
		}
	}
}

#endif