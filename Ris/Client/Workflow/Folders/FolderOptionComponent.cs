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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
    /// <summary>
    /// Extension point for views onto <see cref="FolderOptionComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderOptionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// FolderOptionComponent class
    /// </summary>
    [AssociateView(typeof(FolderOptionComponentViewExtensionPoint))]
    public class FolderOptionComponent : ApplicationComponent
    {
        private int _refreshTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderOptionComponent(int refreshTime)
        {
            _refreshTime = refreshTime;
        }

        public int RefreshTime
        {
            get { return _refreshTime; }
            set 
            { 
                _refreshTime = value;
                this.Modified = true;
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                if (_refreshTime == 0 || _refreshTime > 5000)
                {
                    this.ExitCode = ApplicationComponentExitCode.Accepted;
                    Host.Exit();
                }
                else
                {
                    this.Host.DesktopWindow.ShowMessageBox(SR.MessageEnterNumber0OrOver5000, MessageBoxActions.Ok);
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
