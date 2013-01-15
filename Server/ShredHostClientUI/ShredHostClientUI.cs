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

namespace ClearCanvas.Server.ShredHostClientUI
{
    public class ShredHostClientUI : INotifyPropertyChanged, IDisposable
    {
        public ShredHostClientUI()
        {

            _shredHostProxy = new ShredHostClient();
            _shredCollection = new ShredCollection();
            
            WcfDataShred[] shreds = _shredHostProxy.GetShreds();
            foreach (WcfDataShred shred in shreds)
            {
                _shredCollection.Add(new Shred(shred._id, shred._name, shred._description, shred._isRunning));
            }

        }


        public void Toggle()
        {

        }

        #region Properties

        private ShredCollection _shredCollection;
        private bool _isShredHostRunning;

        public bool IsShredHostRunning
        {
            get { return _isShredHostRunning; }
            set 
            { 
                _isShredHostRunning = value;
                NotifyPropertyChanged("IsShredHostRunning");
            }
        }

        public ShredCollection ShredCollection
        {
            get { return _shredCollection; }
            private set { _shredCollection = value; }
        }


        #endregion

        #region Private fields
        ShredHostClient _shredHostProxy;
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

        #region IDisposable Members

        public void Dispose()
        {
            _shredHostProxy.Close();
        }

        #endregion
    }
}
