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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Manages an XML document containing custom validation rules.
	/// </summary>
	public class XmlValidationManager
	{
		private static readonly XmlValidationManager _instance = new XmlValidationManager();

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static XmlValidationManager Instance { get { return _instance; } }

		const string TagValidation = "validation";
		const string TagValidationRuleset = "validation-ruleset";
		const string TagValidationRule = "validation-rule";
		const string AttrClass = "class";

		private readonly ISettingsStore _settingsStore;

		/// <summary>
		/// Constructor
		/// </summary>
		private XmlValidationManager()
		{
			try
			{
				_settingsStore = SettingsStore.Create();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#region Public API

		/// <summary>
		/// Gets a value indicating whether custom validation rules are supported.
		/// </summary>
		public bool IsSupported
		{
			//TODO (CR Sept 2010): If ValidationRulesSettings used ApplicationSettingsExtensions to save, we wouldn't need this.
			get { return _settingsStore != null; }
		}

		/// <summary>
		/// Gets the custom rules for the specified application component class, as a set of XML elements where each element represents a rule.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public IEnumerable<XmlElement> GetRules(Type componentClass)
		{
			CheckSupported();

			// find node for the specified class
			var rulesNode = FindRulesNode(componentClass);
			if (rulesNode != null)
			{
				// don't allow modification of our document
				var copy = (XmlElement)rulesNode.CloneNode(true);

				return new TypeSafeEnumerableWrapper<XmlElement>(copy.GetElementsByTagName(TagValidationRule));
			}

			// if not exist, return an empty list
			return new XmlElement[0];
		}

		/// <summary>
		/// Sets the custom rules for the specified application component class.  The rules are child elements of the specified parent node.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <param name="parentNode"></param>
		public void SetRules(Type componentClass, XmlElement parentNode)
		{
			CheckSupported();

			// find node for specified class
			// if not exist, create
			var rulesNode = FindRulesNode(componentClass) ?? CreateRulesNode(componentClass);

			// set inner XML from specified node
			rulesNode.InnerXml = parentNode.InnerXml;
		}

		/// <summary>
		/// Saves all changes made to the document via <see cref="SetRules"/>.
		/// </summary>
		public void Save()
		{
			CheckSupported();
			ValidationRulesSettings.Default.Save(_settingsStore);
		}

		#endregion

		private void CheckSupported()
		{
			if (!IsSupported)
				throw new NotSupportedException("XML validation rules are not supported because there is no configuration store.");
		}

		private static XmlElement FindRulesNode(Type componentClass)
		{

			return (XmlElement)ValidationRulesSettings.Default.RulesDocument.SelectSingleNode(
				string.Format("/{0}/{1}[@{2}='{3}']", TagValidation, TagValidationRuleset, AttrClass, componentClass.FullName));
		}

		private static XmlElement CreateRulesNode(Type componentClass)
		{
			var rulesDoc = ValidationRulesSettings.Default.RulesDocument;
			var rulesNode = rulesDoc.CreateElement(TagValidationRuleset);
			rulesNode.SetAttribute(AttrClass, componentClass.FullName);
			rulesDoc.DocumentElement.AppendChild(rulesNode);
			return rulesNode;
		}
	}
}
