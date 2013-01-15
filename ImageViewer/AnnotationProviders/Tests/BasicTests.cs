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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Tests
{
	[TestFixture]
	public class BasicTests
	{
		private List<IAnnotationItemProvider> _existingProviders;

		public BasicTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			_existingProviders = new List<IAnnotationItemProvider>();

			object[] types = this.GetType().Assembly.GetTypes();
			foreach (Type type in types)
			{
				object[] attributes = type.GetCustomAttributes(typeof(ExtensionOfAttribute), false);
				foreach (ExtensionOfAttribute extension in attributes)
				{
					if (extension.ExtensionPointClass == typeof(AnnotationItemProviderExtensionPoint))
					{
						IAnnotationItemProvider provider = (IAnnotationItemProvider)Activator.CreateInstance(type);
						_existingProviders.Add(provider);
					}
				}
			}
		}


		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void TestAnnotationItemProviderIdentifier()
		{
			List<string> uniqueIdentifiers = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				string result = provider.GetIdentifier();
				if (uniqueIdentifiers.Contains(result))
					Assert.Fail("non-unique value \"{1}\" for {0}", provider, result);

				uniqueIdentifiers.Add(result);

				if (string.IsNullOrEmpty(result))
					Assert.Fail("null or empty value for {0}", provider);
			}
		}

		[Test]
		public void TestAnnotationItemProviderDisplayName()
		{
			List<string> uniqueDisplayNames = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				string result = provider.GetDisplayName();
				if (uniqueDisplayNames.Contains(result))
					Assert.Fail("non-unique value \"{1}\" for {0}", provider.GetDisplayName(), result);

				uniqueDisplayNames.Add(result);

				if (string.IsNullOrEmpty(result))
					Assert.Fail("null or empty value for {0}", provider);
			}
		}

		[Test]
		public void TestAnnotationItemIdentifiers()
		{
			List<string> uniqueIdentifiers = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetIdentifier();
					if (uniqueIdentifiers.Contains(result))
						Assert.Fail("non-unique value \"{1}\" for {0}", item, result);

					uniqueIdentifiers.Add(result);

					if (string.IsNullOrEmpty(result))
						Assert.Fail("null or empty value for {0}", item);
				}
			}
		}

		[Test]
		public void TestAnnotationItemDisplayNames()
		{
			List<string> uniqueDisplayNames = new List<string>();

			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetDisplayName();
					if (uniqueDisplayNames.Contains(result))
						Assert.Fail("non-unique value \"{1}\" for {0}", item.GetDisplayName(), result);

					uniqueDisplayNames.Add(result);

					if (string.IsNullOrEmpty(result))
						Assert.Fail("null or empty value for {0}", item);
				}
			}
		}

		[Test]
		public void TestAnnotationItemNullImage()
		{
			foreach (IAnnotationItemProvider provider in _existingProviders)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					string result = item.GetAnnotationText(null);
					if (!string.IsNullOrEmpty(result))
						Assert.Fail("non-null and non-empty value for {0}", item);
				}
			}
		}
	}
}

#endif