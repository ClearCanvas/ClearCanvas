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
using System.Globalization;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ExtensionOf(typeof(ImagePropertyProviderExtensionPoint))]
	internal class StandardImagePropertyProvider : IImagePropertyProvider
	{
		private static readonly IResourceResolver _resolver = new ResourceResolver(typeof(StandardImagePropertyProvider), false);

		public StandardImagePropertyProvider()
		{
		}

		private static XmlDocument LoadDocumentFromResources()
		{
			XmlDocument document = new XmlDocument();
			Stream xmlStream = _resolver.OpenResource("StandardImageProperties.xml");
			document.Load(xmlStream);
			xmlStream.Close();
			return document;
		}

		#region IImageInformation Members

		public IImageProperty[] GetProperties(IPresentationImage image)
		{
			List<IImageProperty> properties = new List<IImageProperty>();

			if (image == null || !(image is IImageSopProvider))
				return properties.ToArray();

			IDicomAttributeProvider dataSource = ((IImageSopProvider) image).Frame;

			try
			{
				XmlDocument document = ImagePropertiesSettings.Default.StandardImagePropertiesXml;
				if (document == null)
				{
					Platform.Log(LogLevel.Debug, "StandardImagePropertiesXml setting document is null.");
					document = LoadDocumentFromResources();
				}

				XmlNodeList groupNodes = document.SelectNodes("//standard-image-properties/image-property-group");
				if (groupNodes == null || groupNodes.Count == 0)
				{
					Platform.Log(LogLevel.Debug, "StandardImagePropertiesXml setting document is empty or incorrectly formatted.");

					document = LoadDocumentFromResources();
					groupNodes = document.SelectNodes("//standard-image-properties/image-property-group");
				}

				if (groupNodes == null || groupNodes.Count == 0)
				{
					Platform.Log(LogLevel.Debug, "StandardImagePropertiesXml setting document is empty or incorrectly formatted.");
					return properties.ToArray();
				}

				foreach (XmlElement groupNode in groupNodes)
				{
					string category = "";
					XmlAttribute categoryAttribute = groupNode.Attributes["name"];
					if (categoryAttribute != null)
						category = LookupCategory(categoryAttribute.Value);

					XmlNodeList propertyNodes = groupNode.SelectNodes("image-property");
					if (propertyNodes == null)
					{
						Platform.Log(LogLevel.Debug, "image-property-group element does not define any image-property elements");
						continue;
					}

					foreach (XmlElement propertyNode in propertyNodes)
					{
						string tagVariableName = null;
						XmlAttribute tagVariableNameAttribute = propertyNode.Attributes["tag-variable-name"];
						if (tagVariableNameAttribute != null)
							tagVariableName = tagVariableNameAttribute.Value;
                        
						if (String.IsNullOrEmpty(tagVariableName))
						{
							Platform.Log(LogLevel.Debug, "tag-variable-name attribute is empty");	
							continue;
						}

						var tag = LookupDicomTag(tagVariableName);
						if (tag == null)
						{
							Platform.Log(LogLevel.Debug, "tag-variable-name doesn't match a valid DicomTag");	
							continue;
						}

						string tagName = null;
						XmlAttribute attribute = propertyNode.Attributes["label"];
						if (attribute != null)
							tagName = attribute.Value;

						string description = null;
						attribute = propertyNode.Attributes["description"];
						if (attribute != null)
							description = attribute.Value;

						string separator = null;
						attribute = propertyNode.Attributes["separator"];
						if (attribute != null)
							separator = attribute.Value;

						try
						{
							ImageProperty property = ImageProperty.Create(dataSource[tag], category, tagName, description, separator);
							properties.Add(property);
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Debug, e, "Failed to create image property '{0}'.", tagName);
						}
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Failed to read in image properties xml.");
			}

			return properties.ToArray();
		}

		private static DicomTag LookupDicomTag(string tag)
		{
			uint tagValue;
			if (uint.TryParse(tag, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out tagValue))
			{
				// very simple support for private tags, since the toolkit won't try to read it anyway
				if (DicomTag.IsPrivateGroup((ushort) (0x00FFFF & (tagValue >> 16))))
					return new DicomTag(tagValue, "Private Tag", "PrivateTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
				return DicomTagDictionary.GetDicomTag(tagValue);
			}
			return DicomTagDictionary.GetDicomTag(tag);
		}

		private static string LookupCategory(string category)
		{
			if (String.IsNullOrEmpty(category))
				return "";

			string lookup = String.Format("Category{0}", category);
			string resolved = _resolver.LocalizeString(lookup);
			if (lookup == resolved)
				return category;
			else
				return resolved;
		}

		#endregion
	}
}