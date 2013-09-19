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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod.Macros;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Tests
{
	[TestFixture]
	internal class FunctionalGroupMacroTests
	{
		[Test]
		public void ListFunctionalGroups()
		{
			const string s = "> {0}";
			foreach (var functionalGroupType in GetFunctionalGroupTypes())
			{
				Console.WriteLine(functionalGroupType.Name);

				var functionalGroup = (FunctionalGroupMacro) Activator.CreateInstance(functionalGroupType);
				foreach (var tag in functionalGroup.DefinedTags.Select(DicomTagDictionary.GetDicomTag))
				{
					Console.WriteLine(tag.ToString());

					foreach (var nestedTag in functionalGroup.NestedTags.Select(DicomTagDictionary.GetDicomTag))
					{
						Console.WriteLine(s, nestedTag);
					}
				}

				Console.WriteLine();
			}
		}

		[Test]
		public void ListSingletonTagsByFunctionalGroup()
		{
			ListTagsByFunctionalGroup(true);
		}

		[Test]
		public void ListAllTagsByFunctionalGroup()
		{
			ListTagsByFunctionalGroup(false);
		}

		private static void ListTagsByFunctionalGroup(bool excludeMultiItemSequences)
		{
			var tagFunctionalGroupMap = CreateFunctionalGroups().SelectMany(f => f.NestedTags.Select(g => new {Tag = g, FunctionalGroup = f}));
			if (excludeMultiItemSequences) tagFunctionalGroupMap = tagFunctionalGroupMap.Where(t => !t.FunctionalGroup.CanHaveMultipleItems);
			foreach (var groupResults in tagFunctionalGroupMap.GroupBy(f => f.Tag))
			{
				if (groupResults.Count() > 1)
				{
					Console.WriteLine(DicomTagDictionary.GetDicomTag(groupResults.Key));
					foreach (var pair in groupResults)
					{
						const string format = "- {0}";
						Console.WriteLine(format, pair.FunctionalGroup.GetType().Name);
					}
					Console.WriteLine();
				}
			}
		}

		[Test]
		public void TestFunctionalGroupSequenceTags()
		{
			foreach (var functionalGroup in CreateFunctionalGroups())
			{
				var fgName = functionalGroup.GetType().Name;

				var tags = functionalGroup.DefinedTags.Select(DicomTagDictionary.GetDicomTag).ToList();
				Assert.AreEqual(1, tags.Count(), "Root Tag in functional group should be the only one: {0}", fgName);

				var rootTag = tags.Single();
				Assert.AreEqual(DicomVr.SQvr, rootTag.VR, "Root Tag in functional group should be SQ: {0} in {1}", rootTag, fgName);

				var sqProperty = functionalGroup.GetType().GetProperty(rootTag.VariableName.Replace("3d", "3D"));
				if (functionalGroup.CanHaveMultipleItems)
				{
					Assert.IsTrue(sqProperty.PropertyType.IsArray, "Sequence property for multiple item functional group should be an array: {0}", fgName);

					var elementType = sqProperty.PropertyType.GetElementType();
					Assert.IsTrue(typeof (IodBase).IsAssignableFrom(elementType) || typeof (IIodMacro).IsAssignableFrom(elementType),
					              "Sequence property array element type should be some subclass of IodBase: {0}", fgName);
				}
				else
				{
					Assert.IsFalse(sqProperty.PropertyType.IsArray, "Sequence property for single item functional group should NOT be an array: {0}", fgName);

					var propertyType = sqProperty.PropertyType;
					Assert.IsTrue(typeof (IodBase).IsAssignableFrom(propertyType) || typeof (IIodMacro).IsAssignableFrom(propertyType),
					              "Sequence property type should be some subclass of IodBase: {0}", fgName);
				}

				var nestedTags = functionalGroup.NestedTags.ToList();
				Assert.IsNotEmpty(nestedTags, "Nested tags in sequence should be defined: {0}", fgName);
			}
		}

		private static IEnumerable<FunctionalGroupMacro> CreateFunctionalGroups()
		{
			return GetFunctionalGroupTypes().Select(t => (FunctionalGroupMacro) Activator.CreateInstance(t));
		}

		private static IEnumerable<Type> GetFunctionalGroupTypes()
		{
			var baseType = typeof (FunctionalGroupMacro);
			return baseType.Assembly.GetTypes().Where(baseType.IsAssignableFrom).Where(t => !t.IsAbstract).OrderBy(t => t.Name);
		}
	}
}