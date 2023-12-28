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
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Samples.Google.Calendar.Mail
{
    /// <summary>
    /// Extension point for views onto <see cref="EmailComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EmailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EmailComponent class
    /// </summary>
    [AssociateView(typeof(EmailComponentViewExtensionPoint))]
    public class EmailComponent : ApplicationComponent
    {
        private string _emailAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailComponent()
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// Gets or sets the email address field.
        /// </summary>
        [ValidateRegex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        /// <summary>
        /// Called when user presses OK.
        /// </summary>
        public void Accept()
        {
            // check for validation errors
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            // exit normally
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        /// <summary>
        /// Called when user presses Cancel.
        /// </summary>
        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }
    }
}
