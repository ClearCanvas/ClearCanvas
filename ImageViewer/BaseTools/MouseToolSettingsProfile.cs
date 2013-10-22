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
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	public class MouseToolSettingsProfile : IXmlSerializable
	{
		private readonly Dictionary<string, Setting> _entries;
		private readonly Dictionary<string, string> _toolActivationActionIdMap;

		public MouseToolSettingsProfile()
		{
			_toolActivationActionIdMap = new Dictionary<string, string>();
			_entries = new Dictionary<string, Setting>();
		}

		private MouseToolSettingsProfile(MouseToolSettingsProfile original)
		{
			_toolActivationActionIdMap = new Dictionary<string, string>(original._toolActivationActionIdMap);
			_entries = new Dictionary<string, Setting>(original._entries.Count);
			foreach (KeyValuePair<string, Setting> entry in original._entries)
				_entries.Add(entry.Key, new Setting(entry.Value));
		}

		public bool HasEntry(Type mouseImageViewerToolType)
		{
			Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
			string key = mouseImageViewerToolType.FullName;
			return this.HasEntryCore(key);
		}

		protected virtual bool HasEntryCore(string key)
		{
			return _entries.ContainsKey(key);
		}

		public virtual bool HideButtonsOverlay
		{
			get { return false; }
		}

		public Setting this[Type mouseImageViewerToolType]
		{
			get
			{
				Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
				Platform.CheckTrue(typeof (MouseImageViewerTool).IsAssignableFrom(mouseImageViewerToolType), "mouseImageViewerToolType should be a MouseImageViewerTool type.");
				string key = mouseImageViewerToolType.FullName;
				return GetSettingCore(key);
			}
		}

		protected virtual Setting GetSettingCore(string key)
		{
			if (!_entries.ContainsKey(key))
				_entries.Add(key, new Setting());
			return _entries[key];
		}

		//TODO (CR Sept 2010): name of these methods is awkward, maybe because it's ActivationAction.  Can we use SelectAction instead?
		public bool IsRegisteredMouseToolActivationAction(string actionId)
		{
			Platform.CheckForEmptyString(actionId, "toolActivationActionId");
			return _toolActivationActionIdMap.ContainsKey(actionId);
		}

		public bool HasEntryByActivationActionId(string toolActivationActionId)
		{
			Platform.CheckForEmptyString(toolActivationActionId, "toolActivationActionId");
			if (!IsRegisteredMouseToolActivationAction(toolActivationActionId))
				return false;
			string key = _toolActivationActionIdMap[toolActivationActionId];
			return this.HasEntryCore(key);
		}

		//TODO (CR Sept 2010): GetSetting instead of GetEntry?
		public Setting GetEntryByActivationActionId(string toolActivationActionId)
		{
			Platform.CheckForEmptyString(toolActivationActionId, "toolActivationActionId");
			Platform.CheckTrue(IsRegisteredMouseToolActivationAction(toolActivationActionId), "hat power is unknown");
			string key = _toolActivationActionIdMap[toolActivationActionId];
			return GetSettingCore(key);
		}

		internal void RegisterActivationActionId(string actionId, Type mouseImageViewerToolType)
		{
			Platform.CheckForEmptyString(actionId, "actionId");
			Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
			Platform.CheckTrue(typeof(MouseImageViewerTool).IsAssignableFrom(mouseImageViewerToolType), "mouseImageViewerToolType should be a MouseImageViewerTool type.");
			string typeName = mouseImageViewerToolType.FullName;

			Platform.CheckFalse(_toolActivationActionIdMap.ContainsKey(actionId) && _toolActivationActionIdMap[actionId] != typeName, "The given actionId has already been registered to a different tool type!");
			if (!_toolActivationActionIdMap.ContainsKey(actionId))
				_toolActivationActionIdMap.Add(actionId, typeName);
		}

		public MouseToolSettingsProfile Clone()
		{
			return new MouseToolSettingsProfile(this);
		}

		#region Static Profile Access

		private static event EventHandler _currentProfileChanged;

        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        [ThreadStatic]
		private static MouseToolSettingsProfile _profile;

		public static MouseToolSettingsProfile Current
		{
			get
			{
				if (_profile == null)
				{
                    _profile = MouseToolSettings.DefaultInstance.DefaultProfile;
					if (_profile == null)
						_profile = new MouseToolSettingsProfile();
				}
				return _profile;
			}
			set
			{
				Platform.CheckForNullReference(value, "value");
				if (_profile != value)
				{
					_profile = value;
					EventsHelper.Fire(_currentProfileChanged, null, EventArgs.Empty);
				}
			}
		}

		public static event EventHandler CurrentProfileChanged
		{
			add { _currentProfileChanged += value; }
			remove { _currentProfileChanged -= value; }
		}

		//TODO (CR Sept 2010): why not just do this when Current is set?
		public static void SaveCurrentAsDefault()
		{
			if (_profile != null)
			{
				try
				{
                    MouseToolSettings settings = MouseToolSettings.DefaultInstance;
					settings.DefaultProfile = _profile;
					settings.Save();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "An exception was encountered while writing the mouse tool settings profile.");
				}
			}
		}

        //For unit tests.
        internal static void Reset()
        {
            _profile = null;
        }

	    #endregion

		#region IXmlSerializable Members

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			_entries.Clear();

			reader.MoveToContent();

			bool isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!isEmptyElement)
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.Name == typeof (Setting).Name)
				{
					string key = reader.GetAttribute("Type");
					Setting value = new Setting();
					((IXmlSerializable) value).ReadXml(reader);
					_entries.Add(key, value);
				}

				reader.ReadEndElement();
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (KeyValuePair<string, Setting> settingEntry in _entries)
			{
				if (settingEntry.Value.IsEmpty)
					continue;
				writer.WriteStartElement(typeof (Setting).Name);
				writer.WriteAttributeString("Type", settingEntry.Key);
				((IXmlSerializable) settingEntry.Value).WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion

		#region Setting Class

		public class Setting : IXmlSerializable
		{
			private ModifierFlags? _defaultMouseButtonModifiers = null;
			private XMouseButtons? _defaultMouseButton = null;
			private XMouseButtons? _mouseButton = null;
			private bool? _initiallyActive = null;

			public Setting() {}

			internal Setting(Setting original)
			{
				_defaultMouseButtonModifiers = original._defaultMouseButtonModifiers;
				_defaultMouseButton = original._defaultMouseButton;
				_mouseButton = original._mouseButton;
				_initiallyActive = original._initiallyActive;
			}

			public bool? InitiallyActive
			{
				get { return _initiallyActive; }
				set { _initiallyActive = value; }
			}

			public XMouseButtons? MouseButton
			{
				get { return _mouseButton; }
				set { _mouseButton = value; }
			}

			public XMouseButtons? DefaultMouseButton
			{
				get { return _defaultMouseButton; }
				set { _defaultMouseButton = value; }
			}

			public ModifierFlags? DefaultMouseButtonModifiers
			{
				get { return _defaultMouseButtonModifiers; }
				set { _defaultMouseButtonModifiers = value; }
			}

			public bool IsEmpty
			{
				get
				{
					return !(_initiallyActive.HasValue
					         || _mouseButton.HasValue
					         || _defaultMouseButton.HasValue
					         || _defaultMouseButtonModifiers.HasValue);
				}
			}

			#region IXmlSerializable Members

			XmlSchema IXmlSerializable.GetSchema()
			{
				return null;
			}

			void IXmlSerializable.ReadXml(XmlReader reader)
			{
				_initiallyActive = null;
				_mouseButton = null;
				_defaultMouseButton = null;
				_defaultMouseButtonModifiers = null;

				reader.MoveToContent();

				bool isEmpty = reader.IsEmptyElement;
				reader.ReadStartElement();
				if (!isEmpty)
				{
					while (reader.MoveToContent() == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "InitiallyActive":
								_initiallyActive = Parse<bool>(reader.ReadElementString());
								break;
							case "MouseButton":
								_mouseButton = Parse<XMouseButtons>(reader.ReadElementString());
								break;
							case "DefaultMouseButton":
								_defaultMouseButton = Parse<XMouseButtons>(reader.ReadElementString());
								break;
							case "DefaultMouseButtonModifiers":
								_defaultMouseButtonModifiers = Parse<ModifierFlags>(reader.ReadElementString());
								break;
						}
					}

					reader.ReadEndElement();
				}
			}

			void IXmlSerializable.WriteXml(XmlWriter writer)
			{
				if (_initiallyActive.HasValue)
					writer.WriteElementString("InitiallyActive", Format(_initiallyActive.Value));

				if (_mouseButton.HasValue)
					writer.WriteElementString("MouseButton", Format(_mouseButton.Value));

				if (_defaultMouseButton.HasValue)
					writer.WriteElementString("DefaultMouseButton", Format(_defaultMouseButton.Value));

				if (_defaultMouseButtonModifiers.HasValue)
					writer.WriteElementString("DefaultMouseButtonModifiers", Format(_defaultMouseButtonModifiers.Value));
			}

			private static string Format<T>(T value) where T : struct
			{
				// for enum types, format using constant labels and standard flags formatting.
				if (typeof (T).IsEnum)
					return Enum.Format(typeof (T), value, "g");
				return TypeDescriptor.GetConverter(typeof (T)).ConvertToInvariantString(value);
			}

			private static T? Parse<T>(string value) where T : struct
			{
				// a serialized null value indicates the type default, not a T? null (which actually indicates that it was not serialized at all)
				if (string.IsNullOrEmpty(value))
					return default(T);

				try
				{
					// for enum types, parse using constant labels and standard flags formatting.
					if (typeof (T).IsEnum)
						return (T) Enum.Parse(typeof (T), value);
					return (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFromInvariantString(value);
				}
				catch (Exception)
				{
					return null;
				}
			}

			#endregion
		}

		#endregion
	}
}