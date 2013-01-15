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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Server.ShredHostClientUI.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ShredHostClientComponent"/>
    /// </summary>
    public partial class ShredHostClientComponentControl : ApplicationComponentUserControl
    {
        private ShredHostClientComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ShredHostClientComponentControl(ShredHostClientComponent component)
            :base(component)
        {
            InitializeComponent();  

            _component = component;

            // TODO add .NET databindings to _component

            // create a binding that will show specific text based on the boolean value
            // of whether the shred host is running
            Binding binding = new Binding("Text", _component, "IsShredHostRunning");
            binding.Parse += new ConvertEventHandler(IsShredHostRunningParse);
            binding.Format += new ConvertEventHandler(IsShredHostRunningFormat);
            _shredHostGroupBox.DataBindings.Add(binding);

            Binding binding2 = new Binding("Text", _component, "IsShredHostRunning");
            binding2.Parse += new ConvertEventHandler(IsShredHostToggleButtonParse);
            binding2.Format += new ConvertEventHandler(IsShredHostToggleButtonFormat);
            _toggleButton.DataBindings.Add(binding2);

            _shredCollectionTable.Table = _component.ShredCollection;
            _shredCollectionTable.SelectionChanged += new EventHandler(OnShredTableSelectionChanged);
            _shredCollectionTable.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            _shredCollectionTable.ToolbarModel = _component.ToolbarModel;
            _shredCollectionTable.MenuModel = _component.ContextMenuModel;

            _toggleButton.Click += delegate(object source, EventArgs args)
            {
                _component.Toggle();
            };
        }

        void IsShredHostToggleButtonFormat(object sender, ConvertEventArgs e)
        {
            // The method converts only to string type. Test this using the DesiredType.
            if (e.DesiredType != typeof(string)) return;

            // Use the ToString method to format the value as currency ("c").
            if (true == ((bool)e.Value))
                e.Value = (string)"Stop";
            else
                e.Value = (string)"Start";
        }

        void IsShredHostToggleButtonParse(object sender, ConvertEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IsShredHostRunningFormat(object sender, ConvertEventArgs e)
        {
            // The method converts only to string type. Test this using the DesiredType.
            if (e.DesiredType != typeof(string)) return;

            // Use the ToString method to format the value as currency ("c").
            if (true == ((bool)e.Value))
                e.Value = (string)"ShredHost is Running";
            else
                e.Value = (string)"ShredHost is Stopped";
        }

        void IsShredHostRunningParse(object sender, ConvertEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void OnShredTableSelectionChanged(object source, EventArgs args)
        {
            _component.TableSelection = _shredCollectionTable.Selection;
        }
    }
}
