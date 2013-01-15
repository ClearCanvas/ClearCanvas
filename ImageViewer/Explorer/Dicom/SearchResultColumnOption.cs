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

using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public sealed class SearchResultColumnOption : INotifyPropertyChanged, IXmlSerializable
	{
		private const string _propertyVisible = @"Visible";

		private event PropertyChangedEventHandler _propertyChanged;
		private bool _visible = true;

		public SearchResultColumnOption() {}

		public SearchResultColumnOption(SearchResultColumnOption source)
		{
			if (source == null)
				return;

			Visible = source.Visible;
		}

		/// <summary>
		/// Configures whether or not the column is visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					NotifyPropertyChanged(_propertyVisible);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#region Implementation of IXmlSerializable

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var empty = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!empty)
			{
				while (reader.MoveToContent() == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case _propertyVisible:
							Visible = ReadXmlBooleanAttribute(reader, false);
							break;
						default:
							// consume the bad element and skip to next sibling or the parent end element tag
							reader.ReadOuterXml();
							break;
					}
				}
				reader.MoveToContent();
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			WriteXmlBooleanAttribute(writer, _propertyVisible, Visible);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		private static bool ReadXmlBooleanAttribute(XmlReader reader, bool defaultValue)
		{
			bool result;
			return bool.TryParse(reader.ReadElementString(), out result) ? result : defaultValue;
		}

		private static void WriteXmlBooleanAttribute(XmlWriter writer, string name, bool value)
		{
			writer.WriteElementString(name, value.ToString(CultureInfo.InvariantCulture));
		}

		private static float? ReadXmlFloatAttribute(XmlReader reader, float? defaultValue)
		{
			float result;
			return float.TryParse(reader.ReadElementString(), out result) ? result : defaultValue;
		}

		private static void WriteXmlFloatAttribute(XmlWriter writer, string name, float? value)
		{
			writer.WriteElementString(name, value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
		}

		#endregion
	}
}