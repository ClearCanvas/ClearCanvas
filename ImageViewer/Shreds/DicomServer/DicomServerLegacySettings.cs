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
using System.Configuration;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.Server.ShredHost;
using System.IO;
using ClearCanvas.Common.Utilities;
using System.Xml;
using ClearCanvas.Common.Configuration;
using LocalDicomServer = ClearCanvas.ImageViewer.Common.DicomServer.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	#region Custom Configuration classes

	internal class SopClassLegacyConfigurationElement : ConfigurationElement
	{
		public SopClassLegacyConfigurationElement()
		{
		}

		public SopClassLegacyConfigurationElement(string uid, string description)
		{
			Uid = uid;
			Description = description;
		}

		[ConfigurationProperty("Uid", IsKey = true, IsRequired = true)]
		public string Uid
		{
			get { return this["Uid"] as string; }
			set { this["Uid"] = value; }
		}

		[ConfigurationProperty("Description", IsKey = false, IsRequired = false)]
		public string Description
		{
			get { return this["Description"] as string; }
			set { this["Description"] = value; }
		}
	}

	internal class ImageSopClassLegacyConfigurationElementCollection : SopClassLegacyConfigurationElementCollection
	{
		public ImageSopClassLegacyConfigurationElementCollection()
		{
		}

		protected override void InitializeDefault()
		{
			base.Initialize("ImageSopClasses.xml");
		}

		internal static ImageSopClassLegacyConfigurationElementCollection Create()
		{
			ImageSopClassLegacyConfigurationElementCollection config = new ImageSopClassLegacyConfigurationElementCollection();
			config.Initialize("ImageSopClasses.xml");
			return config;
		}
	}

	internal class NonImageSopClassLegacyConfigurationElementCollection : SopClassLegacyConfigurationElementCollection
	{
		public NonImageSopClassLegacyConfigurationElementCollection()
		{
		}

		protected override void InitializeDefault()
		{
			base.Initialize("NonImageSopClasses.xml");
		}

		internal static NonImageSopClassLegacyConfigurationElementCollection Create()
		{
			NonImageSopClassLegacyConfigurationElementCollection config = new NonImageSopClassLegacyConfigurationElementCollection();
			config.Initialize("NonImageSopClasses.xml");
			return config;
		}
	}

	internal abstract class SopClassLegacyConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new SopClassLegacyConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SopClassLegacyConfigurationElement)element).Uid;
		}

		protected void Initialize(string xmlResourceName)
		{
			using (Stream stream = new ResourceResolver(typeof(SopClassLegacyConfigurationElementCollection).Assembly).OpenResource(xmlResourceName))
			{
				XmlDocument document = new XmlDocument();
				document.Load(stream);
				foreach (XmlElement sopClass in document.SelectNodes("//sop-class"))
					Add(sopClass.Attributes["uid"].InnerText, sopClass.Attributes["description"].InnerText);

				stream.Close();
			}
		}

		public void Add(string uid, string description)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);

			base.BaseAdd(new SopClassLegacyConfigurationElement(uid, description), false);
		}

		public void Remove(string uid)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);
		}
	}

	internal class TransferSyntaxLegacyConfigurationElement : ConfigurationElement
	{
		public TransferSyntaxLegacyConfigurationElement()
		{
		}

		public TransferSyntaxLegacyConfigurationElement(string uid, string description)
		{
			Uid = uid;
			Description = description;
		}

		[ConfigurationProperty("Uid", IsKey = true, IsRequired = true)]
		public string Uid
		{
			get { return this["Uid"] as string; }
			set { this["Uid"] = value; }
		}

		[ConfigurationProperty("Description", IsKey = false, IsRequired = false)]
		public string Description
		{
			get { return this["Description"] as string; }
			set { this["Description"] = value; }
		}
	}

	internal class TransferSyntaxLegacyConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TransferSyntaxLegacyConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TransferSyntaxLegacyConfigurationElement)element).Uid;
		}

		protected override void InitializeDefault()
		{
			using (Stream stream = new ResourceResolver(typeof(SopClassLegacyConfigurationElementCollection).Assembly).OpenResource("TransferSyntaxes.xml"))
			{
				XmlDocument document = new XmlDocument();
				document.Load(stream);
				foreach (XmlElement transferSyntax in document.SelectNodes("//transfer-syntax"))
					Add(transferSyntax.Attributes["uid"].InnerText, transferSyntax.Attributes["description"].InnerText);
			}
		}

		public void Add(string uid, string description)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);

			base.BaseAdd(new TransferSyntaxLegacyConfigurationElement(uid, description), false);
		}

		public void Remove(string uid)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);
		}

		public static TransferSyntaxLegacyConfigurationElementCollection Create()
		{
			TransferSyntaxLegacyConfigurationElementCollection config = new TransferSyntaxLegacyConfigurationElementCollection();
			config.InitializeDefault();
			return config;
		}
	}

	#endregion

	[Obsolete("Use DicomServerSettings")]
	[LegacyShredConfigSection(@"DicomServerSettings")]
	internal class DicomServerLegacySettings : ShredConfigSection, IMigrateSettings, IMigrateLegacyShredConfigSection
    {
        private static DicomServerLegacySettings _instance;

		private DicomServerLegacySettings()
        {
        }

        public static string SettingName
        {
            get { return "DicomServerSettings"; }
        }

        public static DicomServerLegacySettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ShredConfigManager.GetConfigSection(DicomServerLegacySettings.SettingName) as DicomServerLegacySettings;
                    if (_instance == null)
                    {
                        _instance = new DicomServerLegacySettings();
                        ShredConfigManager.UpdateConfigSection(DicomServerLegacySettings.SettingName, _instance);
                    }
                }

                return _instance;
            }
        }

        public static void Save()
        {
            ShredConfigManager.UpdateConfigSection(DicomServerLegacySettings.SettingName, _instance);
        }

        #region Public Properties

        [ConfigurationProperty("HostName", DefaultValue = "localhost")]
        public string HostName
        {
            get { return (string)this["HostName"]; }
            set { this["HostName"] = value; }
        }

        [ConfigurationProperty("AETitle", DefaultValue = "CLEARCANVAS")]
        public string AETitle
        {
            get { return (string)this["AETitle"]; }
            set { this["AETitle"] = value; }
        }

        [ConfigurationProperty("Port", DefaultValue = "104")]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("InterimStorageDirectory", DefaultValue = ".\\dicom_interim")]
        public string InterimStorageDirectory
        {
            get { return (string)this["InterimStorageDirectory"]; }
            set { this["InterimStorageDirectory"] = value; }
        }

		[ConfigurationProperty("AllowUnknownCaller", DefaultValue = true)]
		public bool AllowUnknownCaller
		{
			get { return (bool)this["AllowUnknownCaller"]; }
			set { this["AllowUnknownCaller"] = value; }
		}

		[ConfigurationProperty("ImageStorageSopClasses")]
		public ImageSopClassLegacyConfigurationElementCollection ImageStorageSopClasses
		{
			get { return (ImageSopClassLegacyConfigurationElementCollection)this["ImageStorageSopClasses"]; }
			set { this["ImageStorageSopClasses"] = value; }
		}

		[ConfigurationProperty("NonImageStorageSopClasses")]
		public NonImageSopClassLegacyConfigurationElementCollection NonImageStorageSopClasses
		{
			get { return (NonImageSopClassLegacyConfigurationElementCollection)this["NonImageStorageSopClasses"]; }
			set { this["NonImageStorageSopClasses"] = value; }
		}

		[ConfigurationProperty("StorageTransferSyntaxes")]
		public TransferSyntaxLegacyConfigurationElementCollection StorageTransferSyntaxes
		{
			get { return (TransferSyntaxLegacyConfigurationElementCollection)this["StorageTransferSyntaxes"]; }
			set { this["StorageTransferSyntaxes"] = value; }
		}

        [ConfigurationProperty("QueryResponsesInUtf8", DefaultValue = false)]
        public bool QueryResponsesInUtf8
        {
            get { return (bool)this["QueryResponsesInUtf8"]; }
            set { this["QueryResponsesInUtf8"] = value; }
        }

		#endregion

        public override object Clone()
        {
            DicomServerLegacySettings clone = new DicomServerLegacySettings();

            clone.HostName = _instance.HostName;
            clone.AETitle = _instance.AETitle;
            clone.Port = _instance.Port;
			clone.AllowUnknownCaller = _instance.AllowUnknownCaller;
            
			clone.InterimStorageDirectory = _instance.InterimStorageDirectory;
			clone.StorageTransferSyntaxes = _instance.StorageTransferSyntaxes;
			clone.ImageStorageSopClasses = _instance.ImageStorageSopClasses;
			clone.NonImageStorageSopClasses = _instance.NonImageStorageSopClasses;
            clone.QueryResponsesInUtf8 = _instance.QueryResponsesInUtf8;

            return clone;
        }

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			switch (migrationValues.PropertyName)
			{
				case "HostName":
				case "AETitle":
				case "Port":
				case "InterimStorageDirectory":
				case "AllowUnknownCaller":
                case "QueryResponsesInUtf8":
                    migrationValues.CurrentValue = migrationValues.PreviousValue;
					break;
                default: //Don't migrate the storage sop classes or transfer syntaxes
					break;
    }
		}

		#endregion

		void IMigrateLegacyShredConfigSection.Migrate()
		{
            LocalDicomServer.UpdateConfiguration(new DicomServerConfiguration { AETitle = AETitle, HostName = HostName, Port = Port });
            LocalDicomServer.UpdateExtendedConfiguration(new DicomServerExtendedConfiguration
                                                             {
                                                                 AllowUnknownCaller = AllowUnknownCaller,
                                                                 QueryResponsesInUtf8 = QueryResponsesInUtf8
                                                             });
		}
	}
}
