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
using System.IO;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
    /// <summary>
    /// Summary description for ServerRuleSamples
    /// </summary>
    [WebService(Namespace = "http://www.clearcanvas.ca/ImageServer/ServerRuleSamples.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ServerRuleSamples : WebService
    {
        [WebMethod]
        public string GetXml(string type)
        {
            try
            {
                XmlDocument doc = null;

                string inputString = Server.HtmlEncode(type);
                if (String.IsNullOrEmpty(inputString))
                    inputString = string.Empty;

                var ep = new SampleRuleExtensionPoint();
                object[] extensions = ep.CreateExtensions();

                foreach (ISampleRule extension in extensions)
                {
                    if (extension.Name.Equals(inputString))
                    {
                        doc = extension.Rule;
                        break;
                    }
                }

                if (doc == null)
                {
                    doc = new XmlDocument();
                    XmlNode node = doc.CreateElement("rule");
                    doc.AppendChild(node);
                    XmlElement conditionNode = doc.CreateElement("condition");
                    node.AppendChild(conditionNode);
                    conditionNode.SetAttribute("expressionLanguage", "dicom");
                    XmlNode actionNode = doc.CreateElement("action");
                    node.AppendChild(actionNode);
                }

                var sw = new StringWriter();

                var xmlSettings = new XmlWriterSettings
                                      {
                                          Encoding = Encoding.UTF8,
                                          ConformanceLevel = ConformanceLevel.Fragment,
                                          Indent = true,
                                          NewLineOnAttributes = false,
                                          CheckCharacters = true,
                                          IndentChars = "  "
                                      };

                XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

                if (tw != null)
                {
                    doc.WriteTo(tw);
                    tw.Close();
                }

                return sw.ToString();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception processing server rule of type: {0}", type);
                throw;
            }
        }
    }
}