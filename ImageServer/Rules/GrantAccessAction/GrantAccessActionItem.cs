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
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom.Utilities.Rules;

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
    /// <summary>
    /// Class for implementing auto-route action as specified by <see cref="IActionItem{T}"/>
    /// </summary>
    public class GrantAccessActionItem : ActionItemBase<ServerActionContext>
    {
        readonly private string _oid;
        #region Constructors

        public GrantAccessActionItem(string oid)
            : base("GrantAcccess Action")
        {
            _oid = oid;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(ServerActionContext context)
        {
            InsertStudyDataAccessCommand command;

            command = new InsertStudyDataAccessCommand(context, new Guid(_oid));

            if (context.CommandProcessor != null)
                context.CommandProcessor.AddCommand(command);
            else
            {
                try
                {
                    command.Execute(context.CommandProcessor);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when inserting grant-access request");

                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
