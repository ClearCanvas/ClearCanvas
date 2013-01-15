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
using System.Reflection;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Model
{
    class QueueSearchableDescriptionAttribute : Attribute
    {

    }

    public abstract class StudyIntegrityQueueDescription
    {
        private const String KEY_VALUE_SEPARATOR = "=";
        private readonly Dictionary<String, Object> _map = new Dictionary<string, object>();
        protected object this[string key]
        {
            get
            {
                if (!_map.ContainsKey(key))
                    return null;
                else
                    return _map[key];
            }
            set
            {
                if (_map.ContainsKey(key))
                    _map[key] = value;
                else
                    _map.Add(key, value);
            }
        }

        public void Parse(String text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            String[] lines = text.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String line in lines)
            {
                int index = line.IndexOf(KEY_VALUE_SEPARATOR);
                String key = line.Substring(0, index);
                String value = line.Substring(index + 1);
                this[key] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (AttributeUtils.HasAttribute<QueueSearchableDescriptionAttribute>(property))
                {
                    text.AppendFormat("{0}{1}{2}", property.Name, KEY_VALUE_SEPARATOR, property.GetValue(this, null));
                    text.Append(Environment.NewLine);
                }
            }

            return text.ToString();
        }

    }

   
    public class ReconcileStudyQueueDescription : StudyIntegrityQueueDescription
    {
        [QueueSearchableDescription]
        public String ExistingPatientName
        {
            get
            {
                return this["ExistingPatientName"] as String;
            }
            set
            {
                this["ExistingPatientName"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ConflictingPatientName
        {
            get
            {
                return this["ConflictingPatientName"] as String;
            }
            set
            {
                this["ConflictingPatientName"] = value;
            }
        }


        [QueueSearchableDescription]
        public String ExistingPatientId
        {
            get
            {
                return this["ExistingPatientId"] as String;
            }
            set
            {
                this["ExistingPatientId"] = value;
            }
        }


        [QueueSearchableDescription]
        public String ConflictingPatientId
        {
            get
            {
                return this["ConflictingPatientId"] as String;
            }
            set
            {
                this["ConflictingPatientId"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ExistingAccessionNumber
        {
            get
            {
                return this["ExistingAccessionNumber"] as String;
            }
            set
            {
                this["ExistingAccessionNumber"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ConflictingAccessionNumber
        {
            get
            {
                return this["ConflictingAccessionNumber"] as String;
            }
            set
            {
                this["ConflictingAccessionNumber"] = value;
            }
        }

    }

}
