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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public sealed class SearchResultColumnOptionCollection : IEnumerable<KeyValuePair<string, SearchResultColumnOption>>, IXmlSerializable
	{
		private const string _itemTag = @"Column";
		private const string _keyAttribute = @"Key";

		private event EventHandler _collectionChanged;
		private readonly Dictionary<string, SearchResultColumnOption> _entries = new Dictionary<string, SearchResultColumnOption>();

		public SearchResultColumnOptionCollection() {}

		public SearchResultColumnOptionCollection(IEnumerable<KeyValuePair<string, SearchResultColumnOption>> source)
			: this()
		{
			if (source == null)
				return;

			foreach (var entry in source)
				SetEntry(entry.Key, new SearchResultColumnOption(entry.Value));
		}

		internal int Count
		{
			get { return _entries.Count; }
		}

		public SearchResultColumnOption this[string columnKey]
		{
			get { return GetEntry(columnKey, true); }
			set { SetEntry(columnKey, value); }
		}

		public event EventHandler CollectionChanged
		{
			add { _collectionChanged += value; }
			remove { _collectionChanged -= value; }
		}

		private void Clear()
		{
			foreach (var entry in _entries)
				entry.Value.PropertyChanged -= HandleEntryPropertyChanged;
			_entries.Clear();
		}

		public bool Contains(string columnKey)
		{
			return _entries.ContainsKey(columnKey);
		}

		private void SetEntry(string columnKey, SearchResultColumnOption value)
		{
			SearchResultColumnOption oldValue;
			if (_entries.TryGetValue(columnKey, out oldValue) && oldValue != null)
				oldValue.PropertyChanged -= HandleEntryPropertyChanged;

			_entries[columnKey] = value;

			if (value != null)
				value.PropertyChanged += HandleEntryPropertyChanged;
		}

		private SearchResultColumnOption GetEntry(string columnKey, bool createIfNotExists)
		{
			if (_entries.ContainsKey(columnKey))
				return _entries[columnKey];

			SearchResultColumnOption entry = null;
			if (createIfNotExists)
				SetEntry(columnKey, entry = new SearchResultColumnOption());
			return entry;
		}

		public SearchResultColumnOption GetEntryOrDefault(string columnKey)
		{
			var behavior = GetEntry(columnKey, false);
			return behavior ?? this[string.Empty];
		}

		private void HandleEntryPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			EventsHelper.Fire(_collectionChanged, this, new EventArgs());
		}

		public IEnumerator<KeyValuePair<string, SearchResultColumnOption>> GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region Search Result Helpers

		internal void ApplyColumnSettings(SearchResult searchResult)
		{
			if (searchResult == null)
				return;

			foreach (var column in searchResult.StudyTable.Columns)
			{
				if (!Contains(column.Name))
					continue;

				var option = this[column.Name];
				column.Visible = option.Visible;
			}
		}

		#endregion

		#region Implementation of IXmlSerializable

		private static readonly XmlSerializer _entrySerializer = new XmlSerializer(typeof (SearchResultColumnOption));

		public void ReadXml(XmlReader reader)
		{
			var list = new List<KeyValuePair<string, SearchResultColumnOption>>();

			reader.MoveToContent();
			var empty = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!empty)
			{
				while (reader.MoveToContent() == XmlNodeType.Element)
				{
					if (reader.Name == _itemTag && !reader.IsEmptyElement)
					{
						var modality = reader.GetAttribute(_keyAttribute);
						reader.ReadStartElement();
						reader.MoveToContent();
						var option = _entrySerializer.Deserialize(reader) as SearchResultColumnOption;
						reader.MoveToContent();
						reader.ReadEndElement();

						if (option != null)
							list.Add(new KeyValuePair<string, SearchResultColumnOption>(modality, option));
					}
					else
					{
						// consume the bad element and skip to next sibling or the parent end element tag
						reader.ReadOuterXml();
					}
				}
				reader.MoveToContent();
				reader.ReadEndElement();
			}

			Clear();
			foreach (var pair in list)
				SetEntry(pair.Key, pair.Value);
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (var entry in this)
			{
				writer.WriteStartElement(_itemTag);
				writer.WriteAttributeString(_keyAttribute, entry.Key);
				_entrySerializer.Serialize(writer, entry.Value);
				writer.WriteEndElement();
			}
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion
	}
}