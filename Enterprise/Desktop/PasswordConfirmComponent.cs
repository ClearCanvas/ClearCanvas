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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Enterprise.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="PasswordConfirmComponent"/>.
    /// </summary>
    [ExtensionPoint]
    public sealed class PasswordConfirmComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PasswordConfirmComponent class.
    /// </summary>
    [AssociateView(typeof(PasswordConfirmComponentViewExtensionPoint))]
    public class PasswordConfirmComponent : ApplicationComponent
    {
        private string _description;
        private string _password;

        #region Constructor

        public PasswordConfirmComponent()
        {
            Description = SR.DescriptionDataAccessGroupChange;
        }

        #endregion

        #region Presentation Model

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                Modified = true;
            }
        }

        [ValidateNotNull]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                Modified = true;
            }
        }


        public void Accept()
        {
            if (HasValidationErrors)
            {
                ShowValidation(true);
                return;
            }
            Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }
  
        #endregion
    }
}
