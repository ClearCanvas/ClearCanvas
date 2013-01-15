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
using System.Text;
using System.ComponentModel;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHost
{
    public partial class WcfDataShred
    {
        public WcfDataShred(int id, string name, string description, bool isRunning)
        {
            this._idField = id;
            this._nameField = name;
            this._descriptionField = description;
            this._isRunningField = isRunning;
        }
    }
}

namespace ClearCanvas.Server.ShredHostClientUI
{
    public class Shred : INotifyPropertyChanged
    {
        public Shred() : this(0, "defaultName", "defaultDescription", false)
        {

        }

        public Shred(int id, string name, string description, bool isRunning)
        {
            _id = id;
            _name = name;
            _description = description;
            _isRunning = isRunning;
        }

        public WcfDataShred GetWcfDataShred()
        {
            return new WcfDataShred(this.Id, this.Name, this.Description, this.IsRunning);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            Shred otherShred = obj as Shred;
            if (null == otherShred)
                return false;
            else if (this == otherShred)
                return true;
            else if (this.Id == otherShred.Id)
                return true;
            else
                return false;
        }

        #region Properties
        private string _name;
        private bool _isRunning;
        private string _description;
        private int _id;

        public int Id
        {
            get { return _id; }
            set 
            { 
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
	
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            { 
                _isRunning = value;
                NotifyPropertyChanged("IsRunning");
            }
        }
	
        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

}
