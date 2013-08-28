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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DicomEditorCreateToolComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomEditorCreateToolComponent class
    /// </summary>
    [AssociateView(typeof(DicomEditorCreateToolComponentViewExtensionPoint))]
    public class DicomEditorCreateToolComponent : ApplicationComponent
    {
        private ushort _group;
        private ushort _element;

        public uint TagId
        {
            get
            {
                var tag = DicomTagDictionary.GetDicomTag(_group, _element);
                if (tag == null)
                    return 0;

                return tag.TagValue;
            }
        }

        public string Group
        {
            get { return String.Format("{0:x4}", _group); }
            set
            {
                if (!ushort.TryParse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out _group))
                    _group = 0;

                UpdateDialog();
            }
        }

        public string Element
        {
            get { return String.Format("{0:x4}", _element); }
            set 
            {
                if (!ushort.TryParse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out _element))
                    _element = 0;
                
                UpdateDialog();
            }
        }

        public string TagName { get; private set; }

        public string Vr { get; private set; }

        //This component used to allow the user to enter the VR, but it was unused and, therefore misleading.
        //Also, it used to allow tags to be entered/accepted that aren't in the DICOM tag dictionary, but the
        //DicomEditorComponent itself can't do that, and it would cause the application to crash. One of these
        //days, we should write a more complete DICOM Editor.
        public bool VrEnabled { get { return false; } }

        public string Value { get; set; }

        public void Accept()
        {
            if (!AcceptEnabled)
                return;

            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();            
        }

        public bool AcceptEnabled { get; private set; }

        public void Cancel()
        {
            this.Host.Exit();
        }

        private void UpdateDialog()
        {       
            DicomTag entry = DicomTagDictionary.GetDicomTag(_group, _element);
            if (entry != null)
            {
                TagName = entry.Name;
                Vr = entry.VR.ToString();
            }
            else
            {
                TagName = "Unknown";
                Vr = "";
            }
      
            AcceptEnabled = AllowTagAddition();

            NotifyPropertyChanged("Group");
            NotifyPropertyChanged("Element");

            NotifyPropertyChanged("TagName");
            NotifyPropertyChanged("Vr");
            NotifyPropertyChanged("AcceptEnabled");
        }

        private bool AllowTagAddition()
        {
            var isInDicomTagDictionary = TagId > 0;
            if (!isInDicomTagDictionary)
                return false; 

            ICollection<string> badGroups = new [] { "0000", "0001", "0003" };

            // if the group number is odd, then it's a private tag and we 
            // cannot handle private tags yet at this point, so we fail out
            // the validation

            var isbad = badGroups.Contains(Group) || Element == "0000";
            return !isbad;
        }
    }
}
