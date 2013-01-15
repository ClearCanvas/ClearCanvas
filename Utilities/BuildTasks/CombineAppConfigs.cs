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
using System.IO;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class CombineAppConfigs : Task
	{
		private string[] _sourceFiles;
		private string _outputFile;
		private bool _checkDependency = false;

		public CombineAppConfigs()
		{
			_outputFile = "app.config";
		}
		
		[Required]
		public string OutputFile
		{
			get { return _outputFile; }
			set { _outputFile = value; }
		}
	
		[Required]
		public string[] SourceFiles
		{
			get { return _sourceFiles; }
			set { _sourceFiles = value; }
		}
		
		public bool CheckDependency
		{
			get { return _checkDependency; }
			set { _checkDependency = value; }
		}
				
		public override bool Execute()
		{
			try
			{
				if (ShouldBuildTarget())
				{
					StringBuilder builder = new StringBuilder();
					builder.AppendLine("Combining App Configs:");
					foreach (string sourceFile in _sourceFiles)
						builder.AppendFormat("\t{0}\n", sourceFile);

					builder.AppendFormat("into file: {0}", _outputFile);

					base.Log.LogMessage(builder.ToString());

					Run();
				}
				else
				{
					base.Log.LogMessage("Combining App Configs skipped because output file is already up-to-date");
				}
				
				return true;
			}
			catch(Exception e)
			{
				base.Log.LogErrorFromException(e);
				return false;
			}
		}

		/// <summary>
		/// Allows various conditions to be checked to see whether 
		/// target should actually be built or skipped, such as
		/// check timestamps for dependency
		/// </summary>
		/// <returns></returns>
		private bool ShouldBuildTarget()
		{
			if (CheckDependency)
			{
				// get timestamp of output file
				FileInfo fiOutput = new FileInfo(OutputFile);

				foreach (string sourceFile in SourceFiles)
				{
					FileInfo fiSource = new FileInfo(sourceFile);
					if (fiSource.LastWriteTime > fiOutput.LastWriteTime)
						return true;
				}

				return false;
			}
			else
			{
				// if no dependency check is required, we should
				// always build the target
				return true;
			}
		}

		private void Run()
		{
			XmlDocument newDocument = new XmlDocument();

			string[] mainConfigurationElements = new string[]
				{
					"configSections",
					"connectionStrings",
					"applicationSettings",
					"userSettings",
					"system.serviceModel"
				};

			XmlElement newConfigurationElement = newDocument.CreateElement("configuration");
			newDocument.AppendChild(newConfigurationElement);

			//Append the direct child nodes of 'configuration' to enforce the order.
			foreach (string mainElement in mainConfigurationElements)
			{
				XmlElement newElement = newDocument.CreateElement(mainElement);
				newConfigurationElement.AppendChild(newElement);
			}

			for (int i = 0; i < SourceFiles.Length; ++i)
			{
				string path = Path.GetFullPath(SourceFiles[i]);
				XmlDocument document = new XmlDocument();
				document.Load(path);

				XmlElement configurationElement = (XmlElement)document.SelectSingleNode("configuration");

				foreach (XmlElement element in configurationElement)
				{
					bool isMainElement = false;
					foreach (string mainElement in mainConfigurationElements)
					{
						if (element.Name == mainElement)
						{
							isMainElement = true;
							break;
						}
					}

					bool elementAlreadyExists = (newConfigurationElement.SelectSingleNode(element.Name) != null);

					//Anything that is not one of the main elements, simply replace it.
					if (!isMainElement || !elementAlreadyExists)
						UpdateNode(newConfigurationElement, element, element.Name);
				}

				string elementPath;
				string xPath;

				string[] sectionGroups = new string[]{"applicationSettings", "userSettings"};

				foreach (string sectionGroupName in sectionGroups)
				{
					elementPath = "configSections/sectionGroup";
					string attributeName = "name";

					XmlElement sectionGroup = configurationElement.SelectSingleNode(elementPath + String.Format("[@{0}='{1}']", attributeName, sectionGroupName)) as XmlElement;
					if (sectionGroup != null)
					{
						XmlElement newApplicationSettingsSectionGroup = CreateElement(newConfigurationElement, elementPath, attributeName, sectionGroupName);
						foreach (XmlElement section in sectionGroup)
						{
							xPath = string.Format("section[@name='{0}']", section.GetAttribute("name"));
							UpdateNode(newApplicationSettingsSectionGroup, section, xPath);
						}
					}
				}

				foreach (string sectionGroupName in sectionGroups)
				{
					elementPath = sectionGroupName;
					XmlElement configSection = configurationElement.SelectSingleNode(elementPath) as XmlElement;
					if (configSection != null)
					{
						foreach (XmlElement typeElement in configSection)
						{
							elementPath = string.Format("{0}/{1}", sectionGroupName, typeElement.Name);
							XmlElement newTypeElement = CreateElement(newConfigurationElement, elementPath, null, null);

							var removeChars = new [] {'\t', '\r', '\n'};
							foreach (XmlNode childNode in typeElement)
							{
								if (childNode is XmlElement)
								{
									XmlElement settingElement = (XmlElement)childNode;
									xPath = string.Format("setting[@name='{0}']", settingElement.GetAttribute("name"));
									UpdateNode(newTypeElement, settingElement, xPath);
								}
								else if (childNode is XmlComment)
								{
									bool found = false;
									string v1 = (childNode.Value ?? "").Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");
									foreach (var newElementNode in newTypeElement)
									{
										var commentNode = newElementNode as XmlComment;
										if (commentNode == null)
											continue;

										string v2 = (commentNode.Value ?? "").Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");
										if (v1==v2)
										{
											found = true;
											break;
										}
									}

									if (!found)
										newTypeElement.InsertBefore(newTypeElement.OwnerDocument.ImportNode(childNode, true), newTypeElement.ChildNodes[0]);
								}
							}
						}
					}
				}

				elementPath = "connectionStrings";
				XmlElement connectionStringsElement = configurationElement.SelectSingleNode(elementPath) as XmlElement;
				if (connectionStringsElement != null)
				{
					if (connectionStringsElement.FirstChild == null)
					{
						XmlElement clearElement = document.CreateElement("clear");
						connectionStringsElement.AppendChild(clearElement);
					}
					else if (connectionStringsElement.FirstChild.Name != "clear")
					{
						XmlElement clearElement = document.CreateElement("clear");
						connectionStringsElement.InsertBefore(clearElement, connectionStringsElement.FirstChild);
					}

					XmlElement newConfigurationStrings = CreateElement(newConfigurationElement, elementPath, null, null);
					foreach (XmlElement connectionStringElement in connectionStringsElement)
					{
						xPath = string.Format("service[@name='{0}']", connectionStringElement.GetAttribute("name"));
						UpdateNode(newConfigurationStrings, connectionStringElement, xPath);
					}
				}

				elementPath = "configSections/section";
				foreach (XmlElement sectionNode in configurationElement.SelectNodes(elementPath))
				{
					XmlElement newConfigSection = CreateElement(newConfigurationElement, "configSections", null, null);
					xPath = string.Format("section[@name='{0}']", sectionNode.GetAttribute("name"));
					UpdateNode(newConfigSection, sectionNode, xPath);
				}

				elementPath = "system.serviceModel/services";
				XmlElement serviceModelServicesElement = configurationElement.SelectSingleNode(elementPath) as XmlElement;
				if (serviceModelServicesElement != null)
				{
					XmlElement newServiceModelServiceElement = CreateElement(newConfigurationElement, elementPath, null, null);

					foreach (XmlElement service in serviceModelServicesElement)
					{
						xPath = string.Format("service[@name='{0}']", service.GetAttribute("name"));
						UpdateNode(newServiceModelServiceElement, service, xPath);
					}
				}

				XmlElement serviceModelBehavioursElement = configurationElement.SelectSingleNode("system.serviceModel/behaviors") as XmlElement;
				if (serviceModelBehavioursElement != null)
				{
					foreach (XmlElement behaviours in serviceModelBehavioursElement)
					{
						elementPath = string.Format("system.serviceModel/behaviors/{0}", behaviours.Name);
						XmlElement newServiceModelBehaviourTypeElement = CreateElement(newConfigurationElement, elementPath, null, null);

						foreach (XmlElement behaviour in behaviours)
						{
							xPath = string.Format("behavior[@name='{0}']", behaviour.GetAttribute("name"));
							UpdateNode(newServiceModelBehaviourTypeElement, behaviour, xPath);
						}
					}
				}

				XmlElement serviceModelBindingsElement = configurationElement.SelectSingleNode("system.serviceModel/bindings") as XmlElement;
				if (serviceModelBindingsElement != null)
				{
					foreach (XmlElement bindingType in serviceModelBindingsElement)
					{
						elementPath = string.Format("system.serviceModel/bindings/{0}", bindingType.Name);
						XmlElement newServiceModelBindingTypeElement = CreateElement(newConfigurationElement, elementPath, null, null);

						foreach (XmlElement binding in bindingType)
						{
							xPath = string.Format("binding[@name='{0}']", binding.GetAttribute("name"));
							UpdateNode(newServiceModelBindingTypeElement, binding, xPath);
						}
					}
				}

				elementPath = "system.serviceModel/client";
				XmlElement serviceModelClientElement = configurationElement.SelectSingleNode(elementPath) as XmlElement;
				if (serviceModelClientElement != null)
				{
					XmlElement newServiceModelClientElement = CreateElement(newConfigurationElement, elementPath, null, null);

					foreach (XmlElement client in serviceModelClientElement)
					{
                        xPath = string.Format("endpoint[@name='{0}']", client.GetAttribute("name"));
						UpdateNode(newServiceModelClientElement, client, xPath);
					}
				}
			}

			foreach (string mainElement in mainConfigurationElements)
			{
				XmlElement element = (XmlElement)newConfigurationElement.SelectSingleNode(mainElement);
				if (!element.HasChildNodes)
					newConfigurationElement.RemoveChild(element);
			}

			System.IO.File.Delete(OutputFile);
			newDocument.Save(OutputFile);
		}

		private static XmlElement CreateElement(XmlElement parentElement, string path, string attributeName, string attributeValue)
		{
			string filterPath = path;
			if (!String.IsNullOrEmpty(attributeName))
				filterPath += string.Format("[@{0}='{1}']", attributeName, attributeValue);

			XmlElement existingElement = parentElement.SelectSingleNode(filterPath) as XmlElement;
			if (existingElement != null)
				return existingElement;

			string[] nodeNames = path.Split(new char[] {'/'});

			XmlElement appendNode = parentElement;
			for (int i = 0; i < nodeNames.Length; ++i)
			{
				filterPath = nodeNames[i];
				if (i == nodeNames.Length - 1 && attributeName != null)
					filterPath += string.Format("[@{0}='{1}']", attributeName, attributeValue);

				XmlElement newNode = appendNode.SelectSingleNode(filterPath) as XmlElement;
				if (newNode == null)
				{
					newNode = appendNode.OwnerDocument.CreateElement(nodeNames[i]);
					appendNode.AppendChild(newNode);
				}

				appendNode = newNode;
			}

			if (!String.IsNullOrEmpty(attributeName))
				appendNode.SetAttribute(attributeName, attributeValue);

			return appendNode;
		}

		private static void UpdateNode(XmlNode parentElement, XmlNode updateChild, string path)
		{
			XmlElement existingElement = parentElement.SelectSingleNode(path) as XmlElement;
			if (existingElement != null)
				parentElement.ReplaceChild(parentElement.OwnerDocument.ImportNode(updateChild, true), existingElement);
			else
				parentElement.AppendChild(parentElement.OwnerDocument.ImportNode(updateChild, true));
		}
	}
}