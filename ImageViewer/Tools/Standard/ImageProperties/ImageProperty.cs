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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	public interface IImageProperty
	{
		string Identifier { get; }

		string Category { get; }
		string Name { get; }
		string Description { get; }
		bool IsEmpty { get; }
		object Value { get; }
		Type ValueType { get; }
	}

	public class ImageProperty : IImageProperty
	{
		public ImageProperty()
		{}

		public ImageProperty(string identifier, string category, string name, string description, object value)
		{
			Identifier = identifier;
			Category = category;
			Name = name;
			Description = description;
			Value = value;
		}

		#region IImageProperty Members

		public string Identifier { get; set; }

		public string Category { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public object Value { get; set; }

		public Type ValueType
		{
			get
			{
				if (Value == null)
					return typeof(string);
				else
					return Value.GetType();
			}
		}

		public virtual bool IsEmpty
		{
			get 
			{
				if (Value == null)
					return true;
				if (Value is string)
					return ((string) Value).Length == 0;

				return false;
			}	
		}

		#endregion

		public static ImageProperty Create(DicomAttribute attribute, string category, string name, string description, string separator)
		{
			// always use the hex value as the identifier, so that private and unknown tags aren't all mapped to the same identifier
			string identifier = attribute.Tag.HexString;

			if (category == null)
				category = string.Empty;

			if (string.IsNullOrEmpty(name))
				name = GetTagName(attribute.Tag);

			if (string.IsNullOrEmpty(description))
				description = GetTagDescription(attribute.Tag);

			if (attribute.IsNull || attribute.IsEmpty)
				return new ImageProperty(identifier, category, name, description, string.Empty);

			if (String.IsNullOrEmpty(separator))
				separator = ", ";

			object value;
			if (attribute.Tag.VR.Name == DicomVr.DAvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string dateString)
				                                	{
				                                		DateTime? date = DateParser.Parse(dateString);
				                                		if (!date.HasValue)
				                                			return null;
				                                		else
				                                			return Format.Date(date.Value);
				                                	}, true);
			}
			else if (attribute.Tag.VR.Name == DicomVr.TMvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string timeString)
				                                	{
				                                		DateTime? time = TimeParser.Parse(timeString);
				                                		if (!time.HasValue)
				                                			return null;
				                                		else
				                                			return Format.Time(time.Value);
				                                	}, true);
			}
			else if (attribute.Tag.VR.Name == DicomVr.DTvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string dateTimeString)
				                                	{
				                                		DateTime? dateTime = DateTimeParser.Parse(dateTimeString);
				                                		if (!dateTime.HasValue)
				                                			return null;
				                                		else
				                                			return Format.Time(dateTime.Value);
				                                	}, true);
			}
			else if (attribute.Tag.VR.Name == DicomVr.PNvr.Name)
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator,
				                                delegate(string nameString)
				                                	{
				                                		PersonName personName = new PersonName(nameString ?? "");
				                                		return personName.FormattedName;
				                                	}, true);
			}
			else if (attribute.Tag.VR == DicomVr.SQvr)
			{
				value = string.Empty;

				var values = attribute.Values as DicomSequenceItem[];
				if (values != null && values.Length > 0)
				{
					// handle simple use case by listing only the attributes of the first sequence item
					// since user can always use DICOM editor for more complex use cases
					var subproperties = new List<IImageProperty>();
					foreach (var subattribute in values[0])
						subproperties.Add(Create(subattribute, string.Empty, null, null, null));
					subproperties.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase));
					value = subproperties.ToArray();
				}
			}
			else if (attribute.Tag.VR.IsTextVR && attribute.GetValueType() == typeof(string[]))
			{
				value = StringUtilities.Combine(attribute.Values as string[], separator, true);
			}
			else
			{
				value = attribute.ToString();
			}

			return new ImageProperty(identifier, category, name, description, value);
		}

		private static readonly IResourceResolver _resolver = new ResourceResolver(typeof (ImageProperty), false);

		private static string GetTagName(DicomTag dicomTag)
		{
			var lookup = string.Format(@"Name{0}", dicomTag.VariableName);
			var resolved = _resolver.LocalizeString(lookup);
			return lookup == resolved ? dicomTag.Name : resolved;
		}

		private static string GetTagDescription(DicomTag dicomTag)
		{
			var lookup = string.Format(@"Description{0}", dicomTag.VariableName);
			var resolved = _resolver.LocalizeString(lookup);
			return lookup == resolved ? string.Empty : resolved;
		}
	}
}