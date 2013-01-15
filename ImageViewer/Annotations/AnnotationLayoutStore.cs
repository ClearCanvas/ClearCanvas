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
using System.Drawing;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal sealed class AnnotationLayoutStore
	{
		private static readonly AnnotationLayoutStore _instance = new AnnotationLayoutStore();

		private readonly object _syncLock = new object();
		private event EventHandler _storeChanged;

		private AnnotationLayoutStore()
		{
			AnnotationLayoutStoreSettings.Default.PropertyChanged += 
			delegate
			{
				EventsHelper.Fire(_storeChanged, this, EventArgs.Empty);
			};
		}

		public event EventHandler StoreChanged
		{
			add { _storeChanged += value; }
			remove { _storeChanged -= value; }
		}

		public static AnnotationLayoutStore Instance
		{
			get{ return _instance; }
		}

		private XmlDocument Document
		{
			get { return AnnotationLayoutStoreSettings.Default.LayoutSettingsXml; }	
			set
			{
				AnnotationLayoutStoreSettings.Default.LayoutSettingsXml = value;
				AnnotationLayoutStoreSettings.Default.Save();
			}
		}
#if UNIT_TESTS

		/// <summary>
		/// Resets the <see cref="AnnotationLayoutStore"/> to the default layout configuration.
		/// </summary>
		public void Reset()
		{
			lock (_syncLock)
			{

				ResetSettings();
				Initialize(true);
			}
		}

#endif

		public void Clear()
		{
			lock (_syncLock)
			{
				XmlDocument document = new XmlDocument();
				XmlElement root = document.CreateElement("annotation-configuration");
				document.AppendChild(root);
				root.AppendChild(document.CreateElement("annotation-layouts"));
				Document = document;
			}
		}

		public void RemoveLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");

			lock (_syncLock)
			{
				Initialize(false);

				string xPath = "annotation-configuration/annotation-layouts";
				XmlElement layoutsNode = (XmlElement)Document.SelectSingleNode(xPath);
				if (layoutsNode == null)
					throw new InvalidDataException(
						String.Format(SR.ExceptionInvalidAnnotationLayoutXml, "'annotation-layouts' node does not exist"));

				xPath = String.Format("annotation-layout[@id='{0}']", identifier);
				XmlNodeList matchingNodes = layoutsNode.SelectNodes(xPath);
				foreach (XmlElement matchingNode in matchingNodes)
					layoutsNode.RemoveChild(matchingNode);

				SaveSettings();
			}
		}

		public IList<StoredAnnotationLayout> GetLayouts(IEnumerable<IAnnotationItem> availableAnnotationItems)
		{
			lock (_syncLock)
			{
				Initialize(false);

				string xPath = "annotation-configuration/annotation-layouts/annotation-layout";
				XmlNodeList layoutNodes = Document.SelectNodes(xPath);

				StoredAnnotationLayoutDeserializer deserializer = new StoredAnnotationLayoutDeserializer(availableAnnotationItems);
				List<StoredAnnotationLayout> layouts = new List<StoredAnnotationLayout>();

				foreach (XmlElement layoutNode in layoutNodes)
                    layouts.Add(deserializer.DeserializeLayout(layoutNode));

				return layouts;
			}
		}

		public StoredAnnotationLayout GetLayout(string identifier, IEnumerable<IAnnotationItem> availableAnnotationItems)
		{
			if (String.IsNullOrEmpty(identifier))
				return null;

            string xPath = String.Format("annotation-configuration/annotation-layouts/annotation-layout[@id='{0}']", identifier);
            
            lock (_syncLock)
			{
				Initialize(false);
				XmlElement layoutNode = Document.SelectSingleNode(xPath) as XmlElement;
				if (layoutNode == null)
					return null;

				return new StoredAnnotationLayoutDeserializer(availableAnnotationItems).DeserializeLayout(layoutNode);
			}
		}

		public void Update(StoredAnnotationLayout layout)
		{
			Platform.CheckForNullReference(layout, "layout");
			Platform.CheckForEmptyString(layout.Identifier, "layout.Identifier");

			lock (_syncLock)
			{
				Initialize(false);

				try
				{
					new StoredAnnotationLayoutSerializer().SerializeLayout(layout);
					SaveSettings();
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
					Initialize(true);
				}
			}
		}

		public void Update(IEnumerable<StoredAnnotationLayout> layouts)
		{
			lock (_syncLock)
			{
				Initialize(false);

				try
				{
					StoredAnnotationLayoutSerializer serializer = new StoredAnnotationLayoutSerializer();
					foreach (StoredAnnotationLayout layout in layouts)
					{
						Platform.CheckForNullReference(layout, "layout");
						Platform.CheckForEmptyString(layout.Identifier, "layout.Identifier");

                        serializer.SerializeLayout(layout);
					}

					SaveSettings();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
					Initialize(true);
				}
			}
		}

		private void SaveSettings()
		{
			AnnotationLayoutStoreSettings.Default.Save();

			if (_storeChanged != null)
				_storeChanged(this, EventArgs.Empty);
		}

		private void ResetSettings()
		{
			AnnotationLayoutStoreSettings.Default.Reset();
			AnnotationLayoutStoreSettings.Default.Save();

			if (_storeChanged != null)
				_storeChanged(this, EventArgs.Empty);
		}

		private void Initialize(bool reloadSettings)
		{
			lock (_syncLock)
			{
				if (Document != null && !reloadSettings)
					return;

				AnnotationLayoutStoreSettings.Default.Reload();
				if (Document != null)
					return;

				try
				{
					XmlDocument document = new XmlDocument();
					ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
					using (Stream stream = resolver.OpenResource("AnnotationLayoutStoreDefaults.xml"))
					{
						document.Load(stream);
						Document = document;
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
					Clear();
				}
			}
		}

		private class StoredAnnotationLayoutDeserializer
		{
			private readonly IEnumerable<IAnnotationItem> _availableAnnotationItems;
			
			public StoredAnnotationLayoutDeserializer(IEnumerable<IAnnotationItem> availableAnnotationItems)
			{
				Platform.CheckForNullReference(availableAnnotationItems, "availableAnnotationItems");

				_availableAnnotationItems = availableAnnotationItems;
			}

			public StoredAnnotationLayout DeserializeLayout(XmlElement layoutNode)
			{
				Platform.CheckForNullReference(layoutNode, "layoutNode");

				StoredAnnotationLayout layout = new StoredAnnotationLayout(layoutNode.GetAttribute("id"));
				XmlNodeList annotationBoxGroupNodes = layoutNode.SelectNodes("annotation-box-groups/annotation-box-group");
				if (annotationBoxGroupNodes != null)
					DeserializeAnnotationBoxGroups(layout, annotationBoxGroupNodes);

				return layout;
			}

			private void DeserializeAnnotationBoxGroups(StoredAnnotationLayout layout, XmlNodeList groupNodes)
			{
				foreach (XmlElement groupNode in groupNodes)
				{
					string newGroupId = groupNode.GetAttribute("id");
					StoredAnnotationBoxGroup newGroup = new StoredAnnotationBoxGroup(newGroupId);

					XmlElement defaultBoxSettingsNode = (XmlElement)groupNode.SelectSingleNode("default-box-settings");

					if (defaultBoxSettingsNode != null)
						DeserializeBoxSettings(newGroup.DefaultBoxSettings, defaultBoxSettingsNode);

					XmlNodeList annotationBoxNodes = groupNode.SelectNodes("annotation-boxes/annotation-box");
					if (annotationBoxNodes != null)
						DeserializeAnnotationBoxes(newGroup, annotationBoxNodes);

					layout.AnnotationBoxGroups.Add(newGroup);
				}
			}

			private void DeserializeAnnotationBoxes(StoredAnnotationBoxGroup group, XmlNodeList annotationBoxNodes)
			{
				foreach (XmlElement annotationBoxNode in annotationBoxNodes)
				{
					string normalizedRectangleString = annotationBoxNode.GetAttribute("normalized-rectangle");

					RectangleF normalizedRectangle;
					if (!DeserializeNormalizedRectangle(normalizedRectangleString, out normalizedRectangle))
						continue;

					XmlElement boxSettingsNode = (XmlElement)annotationBoxNode.SelectSingleNode("box-settings");

					AnnotationBox newBox = group.DefaultBoxSettings.Clone();
					newBox.NormalizedRectangle = normalizedRectangle;

					if (boxSettingsNode != null)
						DeserializeBoxSettings(newBox, boxSettingsNode);

					string annotationItemIdentifier = annotationBoxNode.GetAttribute("annotation-item-id");
					foreach (IAnnotationItem item in _availableAnnotationItems)
					{
						if (item.GetIdentifier() == annotationItemIdentifier)
						{
							newBox.AnnotationItem = item;
							break;
						}
					}

					group.AnnotationBoxes.Add(newBox);
				}
			}

			private void DeserializeBoxSettings(AnnotationBox boxSettings, XmlElement boxSettingsNode)
			{
				string font = boxSettingsNode.GetAttribute("font");
				string color = boxSettingsNode.GetAttribute("color");
				string italics = boxSettingsNode.GetAttribute("italics");
				string bold = boxSettingsNode.GetAttribute("bold");
				string numberOfLines = boxSettingsNode.GetAttribute("number-of-lines");
				string truncation = boxSettingsNode.GetAttribute("truncation");
				string justification = boxSettingsNode.GetAttribute("justification");
				string verticalAlignment = boxSettingsNode.GetAttribute("vertical-alignment");
				string fitWidth = boxSettingsNode.GetAttribute("fit-width");
				string alwaysVisible = boxSettingsNode.GetAttribute("always-visible");

				if (!String.IsNullOrEmpty(font))
					boxSettings.Font = font;
				if (!String.IsNullOrEmpty(color))
					boxSettings.Color = color;
				if (!String.IsNullOrEmpty(italics))
					boxSettings.Italics = (String.Compare("true", italics, true) == 0);
				if (!String.IsNullOrEmpty(bold))
					boxSettings.Bold = (String.Compare("true", bold, true) == 0);
				if (!String.IsNullOrEmpty(numberOfLines))
				{
					byte result;
					if (!byte.TryParse(numberOfLines, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result))
						result = 1;

					boxSettings.NumberOfLines = result;
				}

				if (!String.IsNullOrEmpty(fitWidth))
					boxSettings.FitWidth = (String.Compare("true", fitWidth) == 0);

				if (!String.IsNullOrEmpty(alwaysVisible))
					boxSettings.AlwaysVisible = (String.Compare("true", alwaysVisible, true) == 0);

				if (!String.IsNullOrEmpty(truncation))
				{
					AnnotationBox.TruncationBehaviour fromString = boxSettings.Truncation;
					EnumConverter converter = new EnumConverter(typeof(AnnotationBox.TruncationBehaviour));
					if (converter.IsValid(truncation))
						boxSettings.Truncation = (AnnotationBox.TruncationBehaviour)converter.ConvertFromString(truncation);
				}

				if (!String.IsNullOrEmpty(justification))
				{
					AnnotationBox.JustificationBehaviour fromString = boxSettings.Justification;
					EnumConverter converter = new EnumConverter(typeof(AnnotationBox.JustificationBehaviour));
					if (converter.IsValid(justification))
						boxSettings.Justification = (AnnotationBox.JustificationBehaviour)converter.ConvertFromString(justification);
				}

				if (!String.IsNullOrEmpty(verticalAlignment))
				{
					AnnotationBox.VerticalAlignmentBehaviour fromString = boxSettings.VerticalAlignment;
					EnumConverter converter = new EnumConverter(typeof(AnnotationBox.VerticalAlignmentBehaviour));
					if (converter.IsValid(verticalAlignment))
						boxSettings.VerticalAlignment = (AnnotationBox.VerticalAlignmentBehaviour)converter.ConvertFromString(verticalAlignment);
				}

				XmlElement configurationSettings = (XmlElement)boxSettingsNode.SelectSingleNode("configuration-settings");
				if (configurationSettings != null)
				{
					string showLabel = configurationSettings.GetAttribute("show-label");
					string showLabelIfEmpty = configurationSettings.GetAttribute("show-label-if-empty");
					if (!String.IsNullOrEmpty(showLabel))
						boxSettings.ConfigurationOptions.ShowLabel = (String.Compare("true", showLabel, true) == 0);
					if (!String.IsNullOrEmpty(showLabelIfEmpty))
						boxSettings.ConfigurationOptions.ShowLabelIfValueEmpty = (String.Compare("true", showLabelIfEmpty, true) == 0);
				}
			}

			private bool DeserializeNormalizedRectangle(string normalizedRectangleString, out RectangleF normalizedRectangle)
			{
				normalizedRectangle = new RectangleF();
				
				string[] rectangleComponents = normalizedRectangleString.Split('\\');
				if (rectangleComponents.Length != 4)
					return false;

				float left, right, top, bottom;
				if (!float.TryParse(rectangleComponents[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out left))
					return false;
				if (!float.TryParse(rectangleComponents[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out top))
					return false;
				if (!float.TryParse(rectangleComponents[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out right))
					return false;
				if (!float.TryParse(rectangleComponents[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out bottom))
					return false;

				if (left >= right)
					return false;
				if (top >= bottom)
					return false;
				if (left < 0F || left > 1.0F)
					return false;
				if (top < 0F || top > 1.0F)
					return false;
				if (right < 0F || right > 1.0F)
					return false;
				if (bottom < 0F || bottom > 1.0F)
					return false;

				normalizedRectangle = RectangleF.FromLTRB(left, top, right, bottom);
				return RectangleUtilities.IsRectangleNormalized(normalizedRectangle);
			}
		}

		private class StoredAnnotationLayoutSerializer
		{
			public StoredAnnotationLayoutSerializer()
			{
			}

			private XmlDocument Document
			{
				get { return _instance.Document; }
			}

			public void SerializeLayout(StoredAnnotationLayout layout)
			{
				string xPath = "annotation-configuration/annotation-layouts";
				XmlElement layoutsNode = (XmlElement)this.Document.SelectSingleNode(xPath);
				if (layoutsNode == null)
					throw new InvalidDataException(String.Format(SR.ExceptionInvalidAnnotationLayoutXml, "'annotation-layouts' node does not exist"));

				XmlElement newLayoutNode = this.Document.CreateElement("annotation-layout");
				newLayoutNode.SetAttribute("id", layout.Identifier);

				XmlElement groupsNode = this.Document.CreateElement("annotation-box-groups");
				newLayoutNode.AppendChild(groupsNode);

				SerializeAnnotationBoxGroups(groupsNode, layout.AnnotationBoxGroups);

				xPath = String.Format("annotation-layout[@id='{0}']", layout.Identifier);
				XmlElement existingLayoutNode = (XmlElement)layoutsNode.SelectSingleNode(xPath);

				if (existingLayoutNode != null)
					layoutsNode.ReplaceChild(newLayoutNode, existingLayoutNode);
				else
					layoutsNode.AppendChild(newLayoutNode);
			}

			private void SerializeAnnotationBoxGroups(XmlElement groupsNode, IEnumerable<StoredAnnotationBoxGroup> annotationBoxGroups)
			{
				foreach (StoredAnnotationBoxGroup group in annotationBoxGroups)
				{
					XmlElement groupNode = this.Document.CreateElement("annotation-box-group");
					groupsNode.AppendChild(groupNode);

					groupNode.SetAttribute("id", group.Identifier);

					XmlElement defaultBoxSettingsNode = this.Document.CreateElement("default-box-settings");
					SerializeAnnotationBoxSettings(group.DefaultBoxSettings, new AnnotationBox(), defaultBoxSettingsNode);

					if (defaultBoxSettingsNode.ChildNodes.Count > 0 || defaultBoxSettingsNode.Attributes.Count > 0) 
						groupNode.AppendChild(defaultBoxSettingsNode);

					SerializeAnnotationBoxes(group.AnnotationBoxes, group.DefaultBoxSettings, groupNode);
				}
			}

			private void SerializeAnnotationBoxes(IList<AnnotationBox> annotationBoxes, AnnotationBox defaultBoxSettings, XmlElement groupNode)
			{
				XmlElement boxesNode = this.Document.CreateElement("annotation-boxes");
				groupNode.AppendChild(boxesNode);

				foreach (AnnotationBox box in annotationBoxes)
				{
					XmlElement boxNode = this.Document.CreateElement("annotation-box");
					boxesNode.AppendChild(boxNode);

					string normalizedRectangle = String.Format("{0:F6}\\{1:F6}\\{2:F6}\\{3:F6}", box.NormalizedRectangle.Left, box.NormalizedRectangle.Top, box.NormalizedRectangle.Right, box.NormalizedRectangle.Bottom);

					boxNode.SetAttribute("normalized-rectangle", normalizedRectangle);
					boxNode.SetAttribute("annotation-item-id", (box.AnnotationItem == null) ? "" : box.AnnotationItem.GetIdentifier());

					XmlElement settingsNode = this.Document.CreateElement("box-settings");
					SerializeAnnotationBoxSettings(box, defaultBoxSettings, settingsNode);

					if (settingsNode.ChildNodes.Count > 0 || settingsNode.Attributes.Count > 0)
						boxNode.AppendChild(settingsNode);
				}
			}

			private void SerializeAnnotationBoxSettings(AnnotationBox annotationBox, AnnotationBox defaultSettings, XmlElement boxSettingsNode)
			{
				//only save values that are different from the defaults.
				if (annotationBox.Bold != defaultSettings.Bold)
					boxSettingsNode.SetAttribute("bold", annotationBox.Bold ? "true" : "false");
				if (annotationBox.Italics != defaultSettings.Italics)
					boxSettingsNode.SetAttribute("italics", annotationBox.Italics ? "true" : "false");
				if (annotationBox.Font != defaultSettings.Font)
					boxSettingsNode.SetAttribute("font", annotationBox.Font);
				if (annotationBox.Color != defaultSettings.Color)
					boxSettingsNode.SetAttribute("color", annotationBox.Color);
				if (annotationBox.NumberOfLines != defaultSettings.NumberOfLines)
					boxSettingsNode.SetAttribute("number-of-lines", annotationBox.NumberOfLines.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (annotationBox.Truncation != defaultSettings.Truncation)
					boxSettingsNode.SetAttribute("truncation", annotationBox.Truncation.ToString());
				if (annotationBox.Justification != defaultSettings.Justification)
					boxSettingsNode.SetAttribute("justification", annotationBox.Justification.ToString());
				if (annotationBox.VerticalAlignment != defaultSettings.VerticalAlignment)
					boxSettingsNode.SetAttribute("vertical-alignment", annotationBox.VerticalAlignment.ToString());
				if (annotationBox.FitWidth != defaultSettings.FitWidth)
					boxSettingsNode.SetAttribute("fit-width", annotationBox.FitWidth ? "true" : "false");

				XmlElement configurationSettingsNode = this.Document.CreateElement("configuration-settings");
				if (annotationBox.ConfigurationOptions.ShowLabel != defaultSettings.ConfigurationOptions.ShowLabel)
					configurationSettingsNode.SetAttribute("show-label", annotationBox.ConfigurationOptions.ShowLabel ? "true" : "false");

				if (annotationBox.ConfigurationOptions.ShowLabelIfValueEmpty != defaultSettings.ConfigurationOptions.ShowLabelIfValueEmpty)
					configurationSettingsNode.SetAttribute("show-label-if-empty", annotationBox.ConfigurationOptions.ShowLabelIfValueEmpty ? "true" : "false");

				if (configurationSettingsNode.Attributes.Count > 0)
					boxSettingsNode.AppendChild(configurationSettingsNode);
			}
		}
	}
}