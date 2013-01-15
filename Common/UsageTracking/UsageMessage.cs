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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Application specific Key/Value pair information for <see cref="UsageMessage"/>.
    /// </summary>
    [DataContract]
    public class UsageApplicationData : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// Key/Value pair for application specific usage data.
        /// </summary>
        [XmlAttribute("Key")]
        [DataMember(IsRequired = true)]
        public string Key { get; set; }

        /// <summary>
        /// Key/Value pair for application specific usage data.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Value { get; set; }

    }

    /// <summary>
    /// The type of usage tracking message.
    /// </summary>
    [DataContract]
    public enum UsageType
    {
        /// <summary>
        /// The message is being sent at startup of the application.
        /// </summary>
        [EnumMember]
        Startup = 1,
        /// <summary>
        /// The message is being sent at shutdown of the application.
        /// </summary>
        [EnumMember]
        Shutdown = 2,
        /// <summary>
        /// The message is being sent
        /// </summary>
        [EnumMember]
        Other = 4
    }

    /// <summary>
    /// A product usage message for usage tracking.
    /// </summary>
    [DataContract]
    public class UsageMessage : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// The type of usage tracking message
        /// </summary>
        [DataMember(IsRequired = true)]
        public UsageType MessageType { get; set; }

        /// <summary>
        /// The timestamp for the usage data.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The product being tracked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Product { get; set; }

        /// <summary>
        /// The component being tracked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Component { get; set; }

        /// <summary>
        /// The edition being tracked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Edition { get; set; }

        /// <summary>
        /// Release type.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Release { get; set; }

        /// <summary>
        /// The version of the product being tracked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Version { get; set; }

        /// <summary>
        /// The operating system version string the product is installed on.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string OS { get; set; }

        /// <summary>
        /// The region/culture information for the system that the product is installed on.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Region { get; set; }

        /// <summary>
        /// License String, if configured, for the product.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string LicenseString { get; set; }

        /// <summary>
        /// Short Description of the License Type, if exists, for the product.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string LicenseType{ get; set; }

        /// <summary>
        /// License Expiry Time, if exists, for the product.
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? LicenseExpiryTimeUTC { get; set; }

        /// <summary>
        /// A unique machine identifier.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string MachineIdentifier { get; set; }

        /// <summary>
        /// A unique machine identifier.
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool Certified { get; set; }

        /// <summary>
        /// If the app is allowed for diagnostic use.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool AllowDiagnosticUse { get; set; }

        
        /// <summary>
        /// A set of application data specific to the <see cref="Product"/>.
        /// </summary>
        [XmlArray("AppData")]
        [XmlArrayItem("UsageApplicationData")]
        [DefaultValue(null)]
        [DataMember(IsRequired = false)]
        public List<UsageApplicationData> AppData { get; set; }
    }
}
