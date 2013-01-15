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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolCodeEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolCodeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolCodeEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolCodeEditorComponentViewExtensionPoint))]
    public class ProtocolCodeEditorComponent : ApplicationComponent
    {
    	private readonly EntityRef _protocolCodeRef;
    	private readonly bool _isNew;
    	private ProtocolCodeDetail _detail;
		private ProtocolCodeSummary _protocolCode;

		/// <summary>
		/// Constructor for adding new protocol code.
		/// </summary>
		public ProtocolCodeEditorComponent()
		{
			_isNew = true;
		}

		/// <summary>
		/// Constructor for editing existing code.
		/// </summary>
		/// <param name="protocolCodeRef"></param>
		public ProtocolCodeEditorComponent(EntityRef protocolCodeRef)
		{
			_protocolCodeRef = protocolCodeRef;
		}

		public ProtocolCodeSummary ProtocolCode
		{
			get { return _protocolCode; }
		}

		public override void Start()
		{
			if(_isNew)
			{
				_detail = new ProtocolCodeDetail();
			}
			else
			{
				Platform.GetService<IProtocolAdminService>(
					delegate(IProtocolAdminService service)
					{
						LoadProtocolCodeForEditResponse response = service.LoadProtocolCodeForEdit(
							new LoadProtocolCodeForEditRequest(_protocolCodeRef));
						_detail = response.ProtocolCode;
					});
			}


			base.Start();
		}

        #region Presentation Model

        [ValidateNotNull]
        public string Name
        {
            get { return _detail.Name; }
            set
            {
				_detail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
			get { return _detail.Description; }
            set
            {
				_detail.Description = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveProtocolCode, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                {
					if(_isNew)
					{
						AddProtocolCodeResponse response = service.AddProtocolCode(new AddProtocolCodeRequest(_detail));
						_protocolCode = response.ProtocolCode;
					}
					else
					{
						UpdateProtocolCodeResponse response = service.UpdateProtocolCode(new UpdateProtocolCodeRequest(_detail));
						_protocolCode = response.ProtocolCode;
					}
                });
        }
    }
}
