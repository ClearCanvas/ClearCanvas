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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Model;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.FileSystems)]
    public partial class Default : BasePage
    {
        #region Private members

        // the controller used for database interaction
        private FileSystemsConfigurationController _controller = new FileSystemsConfigurationController();

        #endregion Private members

        #region Protected methods

        /// <summary>
        /// Set up the event handlers for child controls.
        /// </summary>
        protected void SetupEventHandlers()
        {
            AddEditFileSystemDialog1.OKClicked += delegate(Filesystem fs)
                                                      {
                                                          if (AddEditFileSystemDialog1.EditMode)
                                                          {
                                                              // Commit the new FileSystems into database
                                                              if (_controller.UpdateFileSystem(fs))
                                                              {
                                                                  FileSystemsPanel1.UpdateUI();
                                                              }
                                                          }
                                                          else
                                                          {
                                                              // Commit the new FileSystems into database
                                                              if (_controller.AddFileSystem(fs))
                                                              {
                                                                  FileSystemsPanel1.UpdateUI();
                                                              }
                                                          }
                                                      };
        }


        /// <summary>
        /// Retrieves the Filesystems to be rendered in the page.
        /// </summary>
        /// <returns></returns>
        private IList<Filesystem> GetFilesystems()
        {
            // TODO We may want to add context or user preference here to specify which partitions to load

            IList<Filesystem> list = _controller.GetAllFileSystems();
            return list;
        }

        protected override void OnInit(EventArgs e)
        {
            FileSystemsPanel1.EnclosingPage = this;

            base.OnInit(e);

            _controller = new FileSystemsConfigurationController();

            SetupControls();
            SetupEventHandlers();
        }

        protected void SetupControls()
        {
            FileSystemsPanel1.FileSystems = GetFilesystems();
            AddEditFileSystemDialog1.FilesystemTiers = _controller.GetFileSystemTiers();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.FileSystemsPageTitle);
        }

        #endregion  Protected methods

        #region Public Methods

        public void OnAddFileSystem()
        {
            AddEditFileSystemDialog1.EditMode = false;
            AddEditFileSystemDialog1.FileSystem = null;
            AddEditFileSystemDialog1.Show(true);
        }

        public void OnEditFileSystem(FileSystemsConfigurationController controller, Filesystem fs)
        {
            AddEditFileSystemDialog1.EditMode = true;
            AddEditFileSystemDialog1.FileSystem = fs;
            AddEditFileSystemDialog1.Show(true);
        }

        #endregion
    }
}
