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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Command
{
	/// <summary>
	/// A ServerCommand derived class for creating a directory.
	/// </summary>
	public class CreateDirectoryCommand : CommandBase
	{
		#region Private Members
		protected string _directory;
        protected bool _created = false;
		#endregion

        private GetDirectoryDelegateMethod GetDirectoryDelegate;
        public delegate string GetDirectoryDelegateMethod();

		public CreateDirectoryCommand(string directory)
			: base("Create Directory", true)
		{
			Platform.CheckForNullReference(directory, "Directory name");

			_directory = directory;
		}

        public CreateDirectoryCommand(GetDirectoryDelegateMethod getDirectoryDelegate)
            : base("Create Directory", true)
        {
            Platform.CheckForNullReference(getDirectoryDelegate, "getDirectoryDelegate");
            GetDirectoryDelegate = getDirectoryDelegate;
        }

		protected override void OnExecute(CommandProcessor theProcessor)
		{
            if (String.IsNullOrEmpty(_directory) && GetDirectoryDelegate!=null)
            {
                _directory = GetDirectoryDelegate();
            }

			if (Directory.Exists(_directory))
			{
				_created = false;
				return;
			}

			try
			{
			    Directory.CreateDirectory(_directory);
			}
            catch(UnauthorizedAccessException)
            {
                //alert the system admin
                //ServerPlatform.Alert(AlertCategory.System, AlertLevel.Critical, "Filesystem", 
                //                        AlertTypeCodes.NoPermission, null, TimeSpan.Zero,
                //                     "Unauthorized access to {0} from {1}", _directory, ServerPlatform.HostId);
                throw;
            }

			_created = true;
		}

		protected override void OnUndo()
		{
			if (_created)
			{
				DirectoryUtility.DeleteIfExists(_directory);
				_created = false;
			}
		}
	}

}