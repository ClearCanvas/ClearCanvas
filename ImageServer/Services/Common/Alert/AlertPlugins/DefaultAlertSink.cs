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
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Common.Alert.AlertPlugins
{
   
    /// <summary>
    /// Represents an alert service extension that stores <see cref="ClearCanvas.ImageServer.Common.Alert"/> in the database or log file, whichever available.
    /// </summary>
    [ExtensionOf(typeof(AlertServiceExtensionPoint))]
    class DefaultAlertSink : IAlertServiceExtension
    {
        #region IAlertServiceExtension Members

        private bool _databaseEnabled = true;

        public bool DatabaseEnabled
        {
            get { return _databaseEnabled; }
            set { _databaseEnabled = value; }
        }

        public void OnAlert(ImageServer.Common.Alert alert)
        {
            AlertFilter filter = new AlertFilter(AlertCache.Instance);
            if (!filter.Filter(alert))
            {
                AlertCache.Instance.Add(alert);
                if (DatabaseEnabled)
                    WriteToDatabase(alert);
                else
                    WriteToLog(alert);
            }

        }

        #endregion

        #region Private Methods

        private static XmlDocument CreateXmlContent(ImageServer.Common.Alert alert)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode docElement = doc.CreateElement("Contents");
            doc.AppendChild(docElement);

            XmlNode messageNode = doc.CreateElement("Message");
            messageNode.AppendChild(doc.CreateTextNode(alert.Message));

            docElement.AppendChild(messageNode);

            if (alert.ContextData != null)
            {
                XmlNode contextContainerNode = doc.CreateElement("Context");
                XmlNode contextNode = doc.ImportNode(XmlUtils.Serialize(alert.ContextData, false), true);

                contextContainerNode.AppendChild(contextNode);
                docElement.AppendChild(contextContainerNode);
            }

            return doc;
        }

        private static void WriteToLog(ImageServer.Common.Alert alert)
        {
            XmlDocument doc = CreateXmlContent(alert);

            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.OmitXmlDeclaration = true;
                settings.Encoding = Encoding.UTF8;

                XmlWriter writer = XmlWriter.Create(sw, settings);
                doc.WriteTo(writer);
                writer.Flush();

                String log = String.Format("ALERT: {0} : {1}", alert.Source.Name, sw);
                switch (alert.Level)
                {
                    case AlertLevel.Critical:
                    case AlertLevel.Error:
                        Platform.Log(LogLevel.Error, log);
                        break;

                    case AlertLevel.Informational:
                        Platform.Log(LogLevel.Info, log);
                        break;
                    case AlertLevel.Warning:
                        Platform.Log(LogLevel.Warn, log);
                        break;
                    default:
                        Platform.Log(LogLevel.Info, log);
                        break;
                }
            }

        }

        private static void WriteToDatabase(ImageServer.Common.Alert alert)
        {
            XmlDocument doc = CreateXmlContent(alert);

            AlertUpdateColumns columns = new AlertUpdateColumns();

            columns.AlertCategoryEnum = AlertCategoryEnum.GetEnum(alert.Category.ToString());
            columns.AlertLevelEnum = AlertLevelEnum.GetEnum(alert.Level.ToString());
            columns.Component = alert.Source.Name;
            columns.Content = doc;
            columns.InsertTime = Platform.Time;
            columns.Source = alert.Source.Host;
            columns.TypeCode = alert.Code;

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IAlertEntityBroker alertBroker = ctx.GetBroker<IAlertEntityBroker>();
                alertBroker.Insert(columns);
                ctx.Commit();
            }
        }
        #endregion
    }

    /// <summary>
    /// Represent an alert cache
    /// </summary>
    internal class AlertCache
    {
        #region Private members
        private readonly Cache _cache = HttpRuntime.Cache;
        private readonly List<ImageServer.Common.Alert> _listAlerts = new List<ImageServer.Common.Alert>();
        private readonly object _syncLock = new object();
        #endregion

        #region Private Static Members
        static private AlertCache _instance;
        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets an instance of <see cref="AlertCache"/>
        /// </summary>
        static public AlertCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AlertCache();
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// ***** Internal use only. Use AlertCache.Instance instead ****
        /// </summary>
        private AlertCache()
        { }

        #endregion

        #region Private Methods
        static private string ResolveKey(ImageServer.Common.Alert alert)
        {
            Platform.CheckForNullReference(alert, "alert");
            Platform.CheckForNullReference(alert.Source, "alert.Source");
            string key = String.Format("{0}/{1}/{2}/{3}",
                                       alert.Source.Host, alert.Source.Name, alert.Code, alert.ContextData);

            return key;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an alert into the cache.
        /// </summary>
        /// <param name="alert"></param>
        public void Add(ImageServer.Common.Alert alert)
        {
            lock (_syncLock)
            {
                _listAlerts.Add(alert);
            }
            _cache.Add(ResolveKey(alert), alert, null, alert.ExpirationTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal,
                    delegate(string key, Object value, CacheItemRemovedReason reason)
                    {
                        // Discovered an exception here when debugging that may have caused the service to 
                        // crash. This delegate was called, however, the alert was not in the cache
                        lock (_syncLock)
                        {
                            if (_listAlerts.Contains((ImageServer.Common.Alert)value))
                                _listAlerts.Remove((ImageServer.Common.Alert)value);
                        }
                    });
        }

        /// <summary>
        /// Gets a value indicating whether the specified alert or another alert that represents the same event is already in the cache.
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        public bool Contains(ImageServer.Common.Alert alert)
        {
            lock (_syncLock)
                return _listAlerts.Contains(alert);
        }

        #endregion

    }


    /// <summary>
    /// Represents an alert filter
    /// </summary>
    internal class AlertFilter
    {
        #region Private Members
        private readonly AlertCache _cache;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="AlertFilter"/> backed by the specified the alert cache.
        /// </summary>
        /// <param name="cache"></param>
        public AlertFilter(AlertCache cache)
        {
            _cache = cache;
        }
        #endregion

        #region Public Methods
        public bool Filter(ImageServer.Common.Alert alert)
        {
            return _cache.Contains(alert);
        }

        #endregion
    }

}