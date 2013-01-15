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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    internal class TestObject<T> where T :class 
    {
        public T TheProperty;
    }

    internal class TestStringPropertyFilter : StringDicomPropertyFilter<TestObject<string>>
    {
        public bool CalledAddEqualsToQuery;
        public bool CalledAddLikeToQuery;
        public bool CalledFilterResults;
        public bool CalledAddToResults;
        public string EqualsCriterion = "";
        public string LikeCriterion = "";

        public TestStringPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
            : base(path, criteria)
        {
        }

        public TestStringPropertyFilter(DicomTag tag, DicomAttributeCollection criteria)
            : base(tag, criteria)
        {
        }

        public TestStringPropertyFilter(uint tag, DicomAttributeCollection criteria) : base(tag, criteria)
        {
        }

        public void Reset()
        {
            CalledAddEqualsToQuery = false;
            CalledAddLikeToQuery = false;
            CalledFilterResults = false;
            CalledAddToResults = false;
            EqualsCriterion = LikeCriterion = "";
        }

        protected override System.Linq.IQueryable<TestObject<string>> AddEqualsToQuery(System.Linq.IQueryable<TestObject<string>> query, string criterion)
        {
            CalledAddEqualsToQuery = true;
            EqualsCriterion = criterion;
            return query;
        }

        protected override System.Linq.IQueryable<TestObject<string>> AddLikeToQuery(System.Linq.IQueryable<TestObject<string>> query, string criterion)
        {
            CalledAddLikeToQuery = true;
            LikeCriterion = criterion;
            return query;
        }

        protected override System.Collections.Generic.IEnumerable<TestObject<string>> FilterResults(System.Collections.Generic.IEnumerable<TestObject<string>> results)
        {
            CalledFilterResults = true;
            return base.FilterResults(results);
        }

        protected override void AddValueToResult(TestObject<string> item, DicomAttribute resultAttribute)
        {
            CalledAddToResults = true;
            resultAttribute.SetStringValue(item.TheProperty);
        }
    }

    [TestFixture]
    public class PropertyFilterTests
    {
        [Test]
        public void TestBasicString_NoCriteria()
        {
            var testObjects = new[] {new TestObject<string> {TheProperty = "test"}};
            var criteria = new DicomAttributeCollection();
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IMultiValuedPropertyFilter<TestObject<string>>) filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsTrue(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsFalse(filter.ShouldAddToQuery);
            Assert.IsFalse(filter.ShouldAddToResult);
            Assert.IsTrue(iFilter.IsWildcardCriterionAllowed);

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsFalse(filter.CalledAddToResults);
        }

        [Test]
        public void TestBasicString_NullCriteria()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetNullValue();
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IMultiValuedPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsTrue(filter.IsCriterionNull);
            Assert.IsFalse(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(iFilter.IsWildcardCriterionAllowed);
            //Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));

            filter.Reset();
            criteria[DicomTags.PatientId].SetStringValue("");
            result[DicomTags.PatientId].SetEmptyValue();

            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsTrue(filter.IsCriterionNull);
            Assert.IsFalse(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(iFilter.IsWildcardCriterionAllowed);

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestBasicString_SimpleCriteria()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetStringValue("test");
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IMultiValuedPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsTrue(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(iFilter.IsWildcardCriterionAllowed);
            //Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.IsTrue(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            //Only if enabled.
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestWildCardCriteria_WildcardAllowed()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetStringValue("test*");
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IMultiValuedPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsTrue(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(iFilter.IsWildcardCriterionAllowed);
            Assert.IsTrue(iFilter.IsWildcardCriterion(iFilter.CriterionValues[0]));

            Assert.AreEqual(filter.EqualsCriterion ?? "", "");
            Assert.AreEqual(filter.LikeCriterion, "test%");

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsTrue(filter.CalledAddLikeToQuery);
            //Only if enabled.
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestWildCardCriteria_WildcardNotAllowed()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.StudyInstanceUid].SetStringValue("test*");
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.StudyInstanceUid, criteria);
            var iFilter = (IMultiValuedPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsTrue(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsFalse(iFilter.IsWildcardCriterionAllowed);
            //Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.AreEqual(filter.EqualsCriterion, "test*");
            Assert.AreEqual(filter.LikeCriterion ?? "", "");

            Assert.IsTrue(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            //Only if enabled.
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.StudyInstanceUid].GetString(0, ""));
        }
    }
}

#endif