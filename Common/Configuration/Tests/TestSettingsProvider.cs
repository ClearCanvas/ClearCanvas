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

#if	UNIT_TESTS
#pragma warning disable 1591

using System;
using System.Configuration;

namespace ClearCanvas.Common.Configuration.Tests
{
	public class TestSettingsProvider : SettingsProvider, IApplicationSettingsProvider, ISharedApplicationSettingsProvider
	{
		public TestSettingsProvider()
		{
			ApplicationName = "TestSettingsProvider";
		}

		public override string Name
		{
			get { return "TestSettingsProvider"; }
		}

		public override string ApplicationName { get; set; }

		public override string Description
		{
			get{ return "Test settings provider"; }
		}

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			if (String.IsNullOrEmpty(name))
				name = Name;

			base.Initialize(name, config);
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
		{
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

			foreach (SettingsProperty property in collection)
			{
				if (SettingsPropertyExtensions.IsUserScoped(property))
				{
					SettingsPropertyValue value = SimpleSettingsStore.Instance.CurrentUserValues[property.Name] ?? new SettingsPropertyValue(property);
					values.Add(value);
				}
				else
				{
					SettingsPropertyValue value = SimpleSettingsStore.Instance.CurrentSharedValues[property.Name] ?? new SettingsPropertyValue(property);
					values.Add(value);
				}
			}

			return values;
		}

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
		{
			foreach (SettingsPropertyValue value in collection)
			{
				SettingsPropertyValue existing = SimpleSettingsStore.Instance.CurrentUserValues[value.Name];
				if (existing == null)
					SimpleSettingsStore.Instance.CurrentUserValues.Add(existing = new SettingsPropertyValue(value.Property));

				existing.PropertyValue = value.PropertyValue;
			}
		}

		#region IApplicationSettingsProvider Members

		public virtual SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			return SimpleSettingsStore.Instance.PreviousUserValues[property.Name];
		}

		public virtual void Reset(SettingsContext context)
		{
		}

		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			foreach (SettingsProperty property in properties)
			{
				if (!SettingsPropertyExtensions.IsUserScoped(property))
					continue;

				SettingsPropertyValue previousValue = SimpleSettingsStore.Instance.PreviousUserValues[property.Name];
				if (previousValue == null)
					continue;

				SettingsPropertyValue currentValue = SimpleSettingsStore.Instance.CurrentUserValues[property.Name];
				if (currentValue == null)
					continue;

				currentValue.PropertyValue = previousValue.PropertyValue;
			}
		}

		#endregion

		#region ISharedApplicationSettingsProvider Members

		public bool CanUpgradeSharedPropertyValues(SettingsContext context)
		{
			return true; //just let them get overwritten.
		}

		public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
		{
			foreach (SettingsProperty property in properties)
			{
				SettingsPropertyValue previousValue = SimpleSettingsStore.Instance.PreviousSharedValues[property.Name];
				if (previousValue == null)
					continue;

				SettingsPropertyValue currentValue = SimpleSettingsStore.Instance.CurrentSharedValues[property.Name];
				if (currentValue == null)
					continue;

				currentValue.PropertyValue = previousValue.PropertyValue;
			}
		}

		public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
		{
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
			foreach (SettingsProperty property in properties)
			{
				SettingsPropertyValue value = SimpleSettingsStore.Instance.PreviousSharedValues[property.Name];
				if (value != null)
					values.Add(value);
			}

			return values;
		}

		public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
			foreach (SettingsProperty property in properties)
			{
				SettingsPropertyValue value = SimpleSettingsStore.Instance.CurrentSharedValues[property.Name];
				if (value != null)
					values.Add(value);
			}

			return values;
		}

		public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			foreach (SettingsPropertyValue value in values)
			{
				SettingsPropertyValue existing = SimpleSettingsStore.Instance.CurrentSharedValues[value.Name];
				if (existing == null)
					SimpleSettingsStore.Instance.CurrentSharedValues.Add(existing = new SettingsPropertyValue(value.Property));

				existing.PropertyValue = value.PropertyValue;
			}
		}

		#endregion
	}
}

#endif