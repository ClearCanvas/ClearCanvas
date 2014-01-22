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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class OverlaySelectionSetting : IOverlaySelection
	{
		private readonly IOverlayManager _manager;
		private bool _isSelected;

		internal OverlaySelectionSetting(IOverlayManager manager)
		{
			_manager = manager;
		}

		#region Implementation of IOverlaySelectionOption

		public string Name
		{
			get { return _manager.Name; }
		}

		public string DisplayName
		{
			get
			{
				var resourceResolver = _manager.ResourceResolver;
				return resourceResolver != null ? resourceResolver.LocalizeString(_manager.DisplayName) : _manager.DisplayName;
			}
		}

		public bool IsConfigurable
		{
			get { return !_manager.IsImportant; }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected == value) return;
				_isSelected = value;
				EventsHelper.Fire(IsSelectedChanged, this, EventArgs.Empty);
			}
		}

		#endregion

		public event EventHandler IsSelectedChanged;
	}

	public class StoredDisplaySetCreationSetting : INotifyPropertyChanged, IModalityDisplaySetCreationOptions
	{
		private readonly string _modality;

		private bool _createSingleImageDisplaySets;
		private bool _createAllImagesDisplaySet;
		private bool _showOriginalSeries;
		private bool _splitMultiEchoSeries;
		private bool _showOriginalMultiEchoSeries;
		private bool _splitMultiStackSeries;
		private bool _showOriginalMultiStackSeries;

		private bool _splitMixedMultiframes;
		private bool _showOriginalMixedMultiframeSeries;

		private bool _showGrayscaleInverted;

		private event PropertyChangedEventHandler _propertyChanged;

		internal StoredDisplaySetCreationSetting(string modality)
		{
			_modality = modality;
			SetDefaults();
		}

		private void SetDefaults()
		{
			CreateAllImagesDisplaySet = false;

			if (CreateSingleImageDisplaySetsEnabled)
			{
				CreateSingleImageDisplaySets = true;
				ShowOriginalSeries = false;
			}
			else
			{
				ShowOriginalSeries = true;
			}

			if (SplitMixedMultiframesEnabled)
			{
				SplitMixedMultiframes = true;
				ShowOriginalMixedMultiframeSeries = false;
			}

			if (SplitMultiEchoSeriesEnabled)
			{
				SplitMultiEchoSeries = true;
				ShowOriginalMultiEchoSeries = false;
			}

			if (SplitMultiStackSeriesEnabled)
			{
				SplitMultiStackSeries = true;
				ShowOriginalMultiStackSeries = false;
			}

			OverlaySelections = OverlayHelper.OverlayManagers.Select(
				s => new OverlaySelectionSetting(s) {IsSelected = s.IsSelectedByDefault(_modality)}).ToList();
		}

		public string Modality
		{
			get { return _modality; }
		}

		public bool CreateSingleImageDisplaySets
		{
			get { return _createSingleImageDisplaySets; }
			set
			{
				if (_createSingleImageDisplaySets != value)
				{
					_createSingleImageDisplaySets = value;
					NotifyPropertyChanged("CreateSingleImageDisplaySets");
					if (!value && !_createAllImagesDisplaySet)
					{
						//Check show original if both other options are now false.
						ShowOriginalSeries = true;
					}
					else if (!_createAllImagesDisplaySet)
					{
						//Uncheck show original if one option just became true.
						ShowOriginalSeries = false;
					}

					NotifyPropertyChanged("ShowOriginalSeriesEnabled");
				}
			}
		}

		public bool CreateSingleImageDisplaySetsEnabled
		{
			get { return DisplaySetCreationSettings.DefaultInstance.GetSingleImageModalities().Contains(_modality); }
		}

		public bool CreateAllImagesDisplaySet
		{
			get { return _createAllImagesDisplaySet; }
			set
			{
				if (_createAllImagesDisplaySet != value)
				{
					_createAllImagesDisplaySet = value;
					NotifyPropertyChanged("CreateAllImagesDisplaySet");

					if (!value && !_createSingleImageDisplaySets)
					{
						//Check show original if both other options are now false.
						ShowOriginalSeries = true;
					}
					else if (!_createSingleImageDisplaySets)
					{
						//Uncheck show original if one option just became true.
						ShowOriginalSeries = false;
					}

					NotifyPropertyChanged("ShowOriginalSeriesEnabled");
				}
			}
		}

		public bool CreateAllImagesDisplaySetEnabled
		{
			get { return DisplaySetCreationSettings.DefaultInstance.GetAllImagesModalities().Contains(_modality); }
		}

		public bool ShowOriginalSeries
		{
			get { return _showOriginalSeries; }
			set
			{
				if (_showOriginalSeries != value)
				{
					_showOriginalSeries = value;
					NotifyPropertyChanged("ShowOriginalSeries");
				}
			}
		}

		public bool ShowOriginalSeriesEnabled
		{
			get
			{
				return (CreateAllImagesDisplaySetEnabled || CreateSingleImageDisplaySetsEnabled)
				       && (CreateAllImagesDisplaySet || CreateSingleImageDisplaySets);
			}
		}

		public bool SplitMultiEchoSeries
		{
			get { return _splitMultiEchoSeries; }
			set
			{
				if (_splitMultiEchoSeries != value)
				{
					_splitMultiEchoSeries = value;
					NotifyPropertyChanged("SplitMultiEchoSeries");
					NotifyPropertyChanged("ShowOriginalMultiEchoSeriesEnabled");
				}
			}
		}

		public bool SplitMultiEchoSeriesEnabled
		{
			get { return _modality == "MR"; }
		}

		public bool ShowOriginalMultiEchoSeries
		{
			get { return _showOriginalMultiEchoSeries; }
			set
			{
				if (_showOriginalMultiEchoSeries != value)
				{
					_showOriginalMultiEchoSeries = value;
					NotifyPropertyChanged("ShowOriginalMultiEchoSeries");
				}
			}
		}

		public bool ShowOriginalMultiEchoSeriesEnabled
		{
			get { return SplitMultiEchoSeries && SplitMultiEchoSeriesEnabled; }
		}

		public bool SplitMultiStackSeries
		{
			get { return _splitMultiStackSeries; }
			set
			{
				if (_splitMultiStackSeries != value)
				{
					_splitMultiStackSeries = value;
					NotifyPropertyChanged("SplitMultiStackSeries");
					NotifyPropertyChanged("ShowOriginalMultiStackSeriesEnabled");
				}
			}
		}

		public bool SplitMultiStackSeriesEnabled
		{
			get { return SplitMixedMultiframesEnabled; }
		}

		public bool ShowOriginalMultiStackSeries
		{
			get { return _showOriginalMultiStackSeries; }
			set
			{
				if (_showOriginalMultiStackSeries != value)
				{
					_showOriginalMultiStackSeries = value;
					NotifyPropertyChanged("ShowOriginalMultiStackSeries");
				}
			}
		}

		public bool ShowOriginalMultiStackSeriesEnabled
		{
			get { return SplitMultiStackSeries && SplitMultiStackSeriesEnabled; }
		}

		public bool SplitMixedMultiframes
		{
			get { return _splitMixedMultiframes; }
			set
			{
				if (_splitMixedMultiframes != value)
				{
					_splitMixedMultiframes = value;
					NotifyPropertyChanged("SplitMixedMultiframes");
					NotifyPropertyChanged("ShowOriginalMixedMultiframeSeriesEnabled");
				}
			}
		}

		public bool SplitMixedMultiframesEnabled
		{
			get { return DisplaySetCreationSettings.DefaultInstance.GetMixedMultiframeModalities().Contains(_modality); }
		}

		public bool ShowOriginalMixedMultiframeSeries
		{
			get { return _showOriginalMixedMultiframeSeries; }
			set
			{
				if (_showOriginalMixedMultiframeSeries != value)
				{
					_showOriginalMixedMultiframeSeries = value;
					NotifyPropertyChanged("ShowOriginalMixedMultiframeSeries");
				}
			}
		}

		public bool ShowOriginalMixedMultiframeSeriesEnabled
		{
			get { return SplitMixedMultiframes && SplitMixedMultiframesEnabled; }
		}

		public bool ShowGrayscaleInverted
		{
			get { return _showGrayscaleInverted; }
			set
			{
				if (_showGrayscaleInverted == value)
					return;

				_showGrayscaleInverted = value;
				NotifyPropertyChanged("ShowGrayscaleInverted");
			}
		}

		public bool ShowGrayscaleInvertedEnabled
		{
			get { return true; }
		}

		public List<OverlaySelectionSetting> OverlaySelections { get; private set; }

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		#endregion
	}

	[SettingsGroupDescription("Stores user options for how display sets are created.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	public sealed partial class DisplaySetCreationSettings : IMigrateSettings
	{
		private DisplaySetCreationSettings() {}

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			switch (migrationValues.PropertyName)
			{
				case "DisplaySetCreationSettingsXml":
					var previousDocument = migrationValues.PreviousValue as XmlDocument;
					if (previousDocument != null && !string.IsNullOrEmpty(previousDocument.InnerXml))
						migrationValues.CurrentValue = migrationValues.PreviousValue;
					break;
				case "SingleImageModalities":
				case "MixedMultiframeModalities":
					migrationValues.CurrentValue = CombineModalities(
						migrationValues.CurrentValue as string,
						migrationValues.PreviousValue as string);
					break;
				case "AllImagesModalities":
					migrationValues.CurrentValue = CombineModalities(
						migrationValues.CurrentValue as string,
						migrationValues.PreviousValue as string);
					break;
				default:
					break;
			}
		}

		#endregion

		private static string CombineModalities(string modalities1, string modalities2)
		{
			var combined = new SortedDictionary<string, string>();
			foreach (string modality in GetModalities(modalities1 ?? ""))
				combined[modality] = modality;
			foreach (string modality in GetModalities(modalities2 ?? ""))
				combined[modality] = modality;

			return StringUtilities.Combine(combined.Values, ",");
		}

		private static List<string> GetModalities(string modalities)
		{
			return CollectionUtils.Map(modalities.Split(','), (string s) => s.Trim());
		}

		public List<string> GetSingleImageModalities()
		{
			return GetModalities(SingleImageModalities);
		}

		public List<string> GetAllImagesModalities()
		{
			return GetModalities(AllImagesModalities);
		}

		public List<string> GetMixedMultiframeModalities()
		{
			return GetModalities(MixedMultiframeModalities);
		}

		public List<StoredDisplaySetCreationSetting> GetStoredSettings()
		{
			XmlDocument document = this.DisplaySetCreationSettingsXml;
			if (document == null)
			{
				document = new XmlDocument();
				Stream stream = new ResourceResolver(this.GetType(), false).OpenResource("DisplaySetCreationSettingsDefaults.xml");
				document.Load(stream);
				stream.Close();
			}

			XmlNodeList settingsNodes = document.SelectNodes("//display-set-creation-settings/setting");
			if (settingsNodes == null || settingsNodes.Count == 0)
			{
				document = new XmlDocument();
				Stream stream = new ResourceResolver(this.GetType(), false).OpenResource("DisplaySetCreationSettingsDefaults.xml");
				document.Load(stream);
				stream.Close();
				settingsNodes = document.SelectNodes("//display-set-creation-settings/setting");
			}

			List<string> missingModalities = new List<string>(StandardModalities.Modalities);
			List<StoredDisplaySetCreationSetting> storedDisplaySetSettings = new List<StoredDisplaySetCreationSetting>();

			foreach (XmlElement settingsNode in settingsNodes)
			{
				XmlAttribute attribute = settingsNode.Attributes["modality"];
				string modality = "";
				if (attribute != null)
				{
					modality = attribute.Value ?? "";
					missingModalities.Remove(modality);
				}

				if (!String.IsNullOrEmpty(modality))
				{
					XmlNodeList optionNodes = settingsNode.SelectNodes("options/option");
					StoredDisplaySetCreationSetting setting = new StoredDisplaySetCreationSetting(modality);
					SetOptions(setting, optionNodes);
					storedDisplaySetSettings.Add(setting);

					XmlNodeList overlaySelectionOptions = settingsNode.SelectNodes("overlay-selection-options/overlay-selection-option");
					if (overlaySelectionOptions != null)
						SetOverlaySelectionOptions(setting, overlaySelectionOptions);

					XmlNode presentationIntentNode = settingsNode.SelectSingleNode("presentation-intent");
					if (presentationIntentNode != null)
					{
						attribute = presentationIntentNode.Attributes["show-grayscale-inverted"];
						if (attribute != null)
							setting.ShowGrayscaleInverted = (attribute.Value == "True");
					}
				}
			}

			foreach (string missingModality in missingModalities)
				storedDisplaySetSettings.Add(new StoredDisplaySetCreationSetting(missingModality));

			return storedDisplaySetSettings;
		}

		private static void SetOptions(StoredDisplaySetCreationSetting setting, XmlNodeList optionNodes)
		{
			if (optionNodes != null)
			{
				foreach (XmlNode optionNode in optionNodes)
				{
					XmlAttribute identifierAttribute = optionNode.Attributes["identifier"];
					if (identifierAttribute != null)
					{
						XmlAttribute valueAttribute;
						XmlAttribute showOriginalAttribute;

						switch (identifierAttribute.Value)
						{
							case "CreateSingleImageDisplaySets":
								if (setting.CreateSingleImageDisplaySetsEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										setting.CreateSingleImageDisplaySets = valueAttribute.Value == "True";
								}
								break;
							case "CreateAllImagesDisplaySet":
								if (setting.CreateAllImagesDisplaySetEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										setting.CreateAllImagesDisplaySet = valueAttribute.Value == "True";
								}
								break;
							case "ShowOriginalSeries":
								if (setting.ShowOriginalSeriesEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										setting.ShowOriginalSeries = valueAttribute.Value == "True";
								}
								break;
							case "SplitEchos":
								if (setting.SplitMultiEchoSeriesEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										setting.SplitMultiEchoSeries = (valueAttribute.Value == "True");

									showOriginalAttribute = optionNode.Attributes["show-original"];
									if (showOriginalAttribute != null)
										setting.ShowOriginalMultiEchoSeries = showOriginalAttribute.Value == "True";
								}
								break;
							case "SplitStacks":
								if (setting.SplitMultiStackSeriesEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										setting.SplitMultiStackSeries = (valueAttribute.Value == "True");

									showOriginalAttribute = optionNode.Attributes["show-original"];
									if (showOriginalAttribute != null)
										setting.ShowOriginalMultiStackSeries = showOriginalAttribute.Value == "True";
								}
								break;
							case "SplitMixedMultiframes":
								if (setting.SplitMixedMultiframesEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										setting.SplitMixedMultiframes = (valueAttribute.Value == "True");

									showOriginalAttribute = optionNode.Attributes["show-original"];
									if (showOriginalAttribute != null)
										setting.ShowOriginalMixedMultiframeSeries = showOriginalAttribute.Value == "True";
								}
								break;

							default:
								break;
						}
					}
				}
			}
		}

		private void SetOverlaySelectionOptions(StoredDisplaySetCreationSetting setting, XmlNodeList overlaySelectionOptions)
		{
			if (overlaySelectionOptions == null)
				return;

			foreach (XmlNode overlaySelectionOption in overlaySelectionOptions)
			{
				var overlayName = overlaySelectionOption.Attributes["name"];
				if (overlayName == null)
					continue;

				var option = setting.OverlaySelections.FirstOrDefault(o => o.Name == overlayName.Value);
				if (option == null)
					continue;

				var isSelected = overlaySelectionOption.Attributes["selected"];
				option.IsSelected = isSelected != null && isSelected.Value == "True";
			}
		}

		public void Save(IEnumerable<StoredDisplaySetCreationSetting> storedSettings)
		{
			var document = new XmlDocument();
			var root = document.CreateElement("display-set-creation-settings");
			document.AppendChild(root);

			foreach (StoredDisplaySetCreationSetting storedSetting in storedSettings)
			{
				XmlElement settingElement = document.CreateElement("setting");
				settingElement.SetAttribute("modality", storedSetting.Modality);

				XmlElement optionsElement = document.CreateElement("options");
				settingElement.AppendChild(optionsElement);

				if (storedSetting.CreateSingleImageDisplaySetsEnabled)
				{
					XmlElement createSingleImageDisplaySetsElement = document.CreateElement("option");
					createSingleImageDisplaySetsElement.SetAttribute("identifier", "CreateSingleImageDisplaySets");
					createSingleImageDisplaySetsElement.SetAttribute("value", storedSetting.CreateSingleImageDisplaySets ? "True" : "False");
					optionsElement.AppendChild(createSingleImageDisplaySetsElement);
				}

				if (storedSetting.CreateAllImagesDisplaySetEnabled)
				{
					XmlElement createAllImagesDisplaySetElement = document.CreateElement("option");
					createAllImagesDisplaySetElement.SetAttribute("identifier", "CreateAllImagesDisplaySet");
					createAllImagesDisplaySetElement.SetAttribute("value", storedSetting.CreateAllImagesDisplaySet ? "True" : "False");
					optionsElement.AppendChild(createAllImagesDisplaySetElement);
				}

				if (storedSetting.ShowOriginalSeriesEnabled)
				{
					XmlElement showOriginalSeriesElement = document.CreateElement("option");
					showOriginalSeriesElement.SetAttribute("identifier", "ShowOriginalSeries");
					showOriginalSeriesElement.SetAttribute("value", storedSetting.ShowOriginalSeries ? "True" : "False");
					optionsElement.AppendChild(showOriginalSeriesElement);
				}

				if (storedSetting.SplitMultiEchoSeriesEnabled)
				{
					XmlElement splitEchosElement = document.CreateElement("option");
					splitEchosElement.SetAttribute("identifier", "SplitEchos");
					splitEchosElement.SetAttribute("value", storedSetting.SplitMultiEchoSeries ? "True" : "False");
					splitEchosElement.SetAttribute("show-original", storedSetting.ShowOriginalMultiEchoSeries ? "True" : "False");
					optionsElement.AppendChild(splitEchosElement);
				}

				if (storedSetting.SplitMultiStackSeriesEnabled)
				{
					XmlElement splitStacksElement = document.CreateElement("option");
					splitStacksElement.SetAttribute("identifier", "SplitStacks");
					splitStacksElement.SetAttribute("value", storedSetting.SplitMultiStackSeries ? "True" : "False");
					splitStacksElement.SetAttribute("show-original", storedSetting.ShowOriginalMultiStackSeries ? "True" : "False");
					optionsElement.AppendChild(splitStacksElement);
				}

				if (storedSetting.SplitMixedMultiframesEnabled)
				{
					XmlElement splitMultiframesElement = document.CreateElement("option");
					splitMultiframesElement.SetAttribute("identifier", "SplitMixedMultiframes");
					splitMultiframesElement.SetAttribute("value", storedSetting.SplitMixedMultiframes ? "True" : "False");
					splitMultiframesElement.SetAttribute("show-original", storedSetting.ShowOriginalMixedMultiframeSeries ? "True" : "False");
					optionsElement.AppendChild(splitMultiframesElement);
				}

				if (storedSetting.OverlaySelections != null && storedSetting.OverlaySelections.Count > 0)
				{
					XmlElement overlaySelectionOptionsElement = document.CreateElement("overlay-selection-options");
					settingElement.AppendChild(overlaySelectionOptionsElement);
					foreach (var overlaySelectionOption in storedSetting.OverlaySelections)
					{
						XmlElement overlaySelectionOptionElement = document.CreateElement("overlay-selection-option");
						overlaySelectionOptionsElement.AppendChild(overlaySelectionOptionElement);
						overlaySelectionOptionElement.SetAttribute("name", overlaySelectionOption.Name);
						overlaySelectionOptionElement.SetAttribute("selected", overlaySelectionOption.IsSelected ? "True" : "False");
					}
				}

				if (storedSetting.ShowGrayscaleInverted)
				{
					XmlElement presentationIntentElement = document.CreateElement("presentation-intent");
					presentationIntentElement.SetAttribute("show-grayscale-inverted", "True");
					settingElement.AppendChild(presentationIntentElement);
				}

				root.AppendChild(settingElement);
			}

			this.DisplaySetCreationSettingsXml = document;
			this.Save();
		}

		//TODO (Phoenix5): #10730 - remove this when it's fixed.

		#region WebStation Settings Hack

		[ThreadStatic]
		private static DisplaySetCreationSettings _webDefault;

		public static DisplaySetCreationSettings DefaultInstance
		{
			get
			{
				if (Application.GuiToolkitID == GuiToolkitID.Web)
					return _webDefault ?? (_webDefault = new DisplaySetCreationSettings());

				return Default;
			}
		}

		#endregion
	}
}