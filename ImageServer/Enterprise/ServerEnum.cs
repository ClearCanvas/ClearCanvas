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
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// A specialized <see cref="ServerEntity"/> that represents an enumerated value.
    /// </summary>
    [Serializable]
    public abstract partial class ServerEnum : ServerEntity
    {
        
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the <see cref="ServerEntity"/>.</param>
        public ServerEnum(String name)
            : base(name)
        {
        }

        #endregion

        #region Private Members

        private short _enumValue;
        private string _lookup;
        private string _description;
        private string _longDescription;

        #endregion

        #region Public Properties

        /// <summary>
        /// The enumerated value itself.
        /// </summary>
        [EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "Enum")]
        public short Enum
        {
            get { return _enumValue; }
            set { _enumValue = value; }
        }

        /// <summary>
        /// A lookup string.
        /// </summary>
        [EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "Lookup")]
        public string Lookup
        {
            get { return _lookup; }
            set { _lookup = value; }
        }

        /// <summary>
        /// A short description of the enumerated value.
        /// </summary>
        [EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "Description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// A long description of the enumerated value.
        /// </summary>
        [EntityFieldDatabaseMappingAttribute(TableName = "", ColumnName = "LongDescription")]
        public string LongDescription
        {
            get { return _longDescription; }
            set { _longDescription = value; }
        }

        #endregion

        #region Public Abstract Methods

        public abstract void SetEnum(short val);

        #endregion

        #region Public Overrides

        public override int GetHashCode()
        {
            return Enum;
        }

        public override bool Equals(object obj)
        {
            ServerEnum e = obj as ServerEnum;
            if (e == null)
                return false;

            // Must be in the inheritance hierarchy of each other to be equal
            // eg, Status enum can't be equal to Type enum.
            if (GetType().IsAssignableFrom(obj.GetType()) ||
                obj.GetType().IsAssignableFrom(GetType()))
            {
                return e.Enum == Enum;
            }
            return false;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(ServerEnum t1, ServerEnum t2)
        {
            if ((object)t1 == null && (object)t2 == null)
                return true;
            if ((object)t1 == null || (object)t2 == null)
                return false;
            return t1.Equals(t2);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(ServerEnum t1, ServerEnum t2)
        {
            if ((object)t1 == null && (object)t2 == null)
                return false;
            if ((object)t1 == null || (object)t2 == null)
                return true;
            return !t1.Equals(t2);
        }

        #endregion
    }

    public class ServerEnumExtensionPoint : ExtensionPoint<IServerEnumDescriptionTranslator>
    { }

    /// <summary>
    /// Localization Support implementation for ServerEnum
    /// </summary>
    public abstract partial class ServerEnum
    {
        private static readonly IServerEnumDescriptionTranslator _descriptionTranslator;

        static ServerEnum()
        {
            try
            {
                _descriptionTranslator =
                    new ServerEnumExtensionPoint().CreateExtension() as IServerEnumDescriptionTranslator;
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Warn, "Unable to instantiate ServerEnum Description transatlor: {0}", ex.Message);

                Platform.Log(LogLevel.Warn, "Use default server enum description translator");
                _descriptionTranslator = new DefaultServerEnumDescriptionTranslator();
            }
        }

        public string LocalizedDescription
        {
            get
            {
                return _descriptionTranslator != null ? _descriptionTranslator.GetLocalizedDescription(this) : Description;
            }
        }

        public string LocalizedLongDescription
        {
            get
            {
                return _descriptionTranslator != null ? _descriptionTranslator.GetLocalizedLongDescription(this) : Description;
            }
        }

        public override string ToString()
        {
            return _descriptionTranslator != null ? _descriptionTranslator.GetLocalizedDescription(this) : Description;
        }
    }

    internal class DefaultServerEnumDescriptionTranslator : IServerEnumDescriptionTranslator
    {
        public string GetLocalizedDescription(ServerEnum serverEnum)
        {
            return serverEnum.Description;
        }

        public string GetLocalizedLongDescription(ServerEnum serverEnum)
        {
            return serverEnum.LongDescription;
        }
    }

    public interface IServerEnumDescriptionTranslator
    {
        string GetLocalizedDescription(ServerEnum serverEnum);
        string GetLocalizedLongDescription(ServerEnum serverEnum);
    }
}
