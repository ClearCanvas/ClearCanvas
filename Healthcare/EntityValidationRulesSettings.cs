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

using System.Configuration;
using System.Xml;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(EntityValidationRuleSetSourceExtensionPoint))]
	public class HealthcareXmlValidationRuleSetSource : XmlValidationRuleSetSource
	{
		protected override XmlDocument RuleSetDocument
		{
			get
			{
				var settings = new EntityValidationRulesSettings();
				var doc = new XmlDocument();
				doc.LoadXml(settings.CustomRulesXml);
				return doc;
			}
		}
	}

	[SettingsGroupDescription("Defines custom entity validation rules for the Healthcare model.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class EntityValidationRulesSettings
	{
		///<summary>
		/// Public constructor.
		///</summary>
		public EntityValidationRulesSettings()
		{
		}
	}
}
