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
using System.Runtime.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// List of predefined alert type codes
    /// </summary>
    public class AlertTypeCodes
    {
        public const int Starting = 0;
        public const int Started = 1;
        public const int UnableToStart = 2;
        public const int Stopping = 10;
        public const int Stopped = 11;
        public const int UnableToStop = 12;

        public const int NoPermission = 13;

        public const int InvalidConfiguration = 20;

        public const int UnableToProcess = 30;

        public const int NoResources = 40;
        public const int LowResources = 41;


    }

    /// <summary>
    /// Represents the category of an <see cref="Alert"/>
    /// </summary>
    public enum AlertCategory
    {
        /// <summary>
        /// System alert
        /// </summary>
        System,

        /// <summary>
        /// Application alert
        /// </summary>
        Application,

        /// <summary>
        /// Security alert
        /// </summary>
        Security,

        /// <summary>
        /// User alert
        /// </summary>
        User,

        Unknown
    }

    /// <summary>
    /// Represents the level of an <see cref="Alert"/>
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// Alerts carrying information
        /// </summary>
        Informational,

        /// <summary>
        /// Alerts carrying warning message
        /// </summary>
        Warning,

        /// <summary>
        /// Alerts carrying error message
        /// </summary>
        Error,

        /// <summary>
        /// Alerts carrying critical information message
        /// </summary>
        Critical
    }

    /// <summary>
    /// Represents the source of an <see cref="Alert"/>
    /// </summary>
    [DataContract]
    public class AlertSource : IEquatable<AlertSource>
    {
        #region Private Members
        private string _name;
        private string _host;
        #endregion

        #region Constructors

        public AlertSource()
        {
        }


        /// <summary>
        /// Creates an instance of <see cref="Alert"/> for the specified source.
        /// </summary>
        /// <param name="name">Name of the source associated with alerts</param>
        /// <param name="host">The host for the alert.</param>
        public AlertSource(string name, string host)
        {
            _name = name;
            _host = host;
        }

        #endregion

        [DataMember]
        /// <summary>
        /// Name of the source.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets a string that represents the host machine of the source
        /// </summary>
        [DataMember]
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        #region IEquatable<AlertSource> Members

        public bool Equals(AlertSource other)
        {
            return Name == other.Name && Host == other.Host;
        }

        #endregion
    }

    /// <summary>
    /// Represents an alert.
    /// </summary>
    [DataContract]
    public class Alert : IEquatable<Alert>
    {
        #region Public Static Properties
        /// <summary>
        /// 'Null' alert.
        /// </summary>
        #endregion

        #region Private Members
        private AlertSource _source;
        private AlertCategory _category;
        private AlertLevel _level;
        private DateTime _timeStamp;
        private DateTime _expirationTime;
        private int _code;
        private String _message;
        private object _data;

        #endregion

        #region Constructors

        public Alert()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="Alert"/> of specified category and level for the specified source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="category"></param>
        /// <param name="level"></param>
        public Alert(AlertSource source, AlertCategory category, AlertLevel level)
        {
            _source = source;
            _timeStamp = Platform.Time;
            _expirationTime = Platform.Time; 
            _category = category;
            _level = level;
            _code = 0;
            _data = null;
            _message = null;
        }

        #endregion

        /// <summary>
        /// Sets or gets the source of the alert.
        /// </summary>
        [DataMember]
        public AlertSource Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Gets the timestamp when the alert was created
        /// </summary>
        [DataMember]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }



        /// <summary>
        /// Gets or sets the data associated with the alert
        /// </summary>
        [DataMember]
        public object ContextData
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Gets or sets the alert category
        /// </summary>
        [DataMember]
        public AlertCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// Gets or sets the alert level
        /// </summary>
        [DataMember]
        public AlertLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        /// Gets or sets the alert code
        /// </summary>
        /// <remarks>
        /// <seealso cref="AlertTypeCodes"/> for predefined codes
        /// </remarks>
        [DataMember]
        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Gets or sets the expiration time of the alert.
        /// </summary>
        [DataMember]
        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { _expirationTime = value; }
        }

        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #region IEquatable<Alert> Members

        public bool Equals(Alert other)
        {
            if (! (Source.Equals(other.Source) &&
                   Category.Equals(other.Category) &&
                   Level.Equals(other.Level) &&
                   Code.Equals(other.Code)))
            return false;

            if (ContextData == null)
                return other.ContextData == null;
            
            return ContextData.Equals(other.ContextData);
        }

        #endregion
    }
}