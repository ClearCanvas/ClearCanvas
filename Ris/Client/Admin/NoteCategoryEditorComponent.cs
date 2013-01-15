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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="NoteCategoryEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteCategoryEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteCategoryEditorComponent class
    /// </summary>
    [AssociateView(typeof(NoteCategoryEditorComponentViewExtensionPoint))]
    public class NoteCategoryEditorComponent : ApplicationComponent
    {
        private List<EnumValueInfo> _severityChoices;

        private PatientNoteCategoryDetail _noteCategoryDetail;
        private EntityRef _noteCategoryRef;
        private bool _isNew;

        private PatientNoteCategorySummary _noteCategorySummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategoryEditorComponent()
        {
            _isNew = true;
        }

        public NoteCategoryEditorComponent(EntityRef noteCategoryRef)
        {
            _isNew = false;
            _noteCategoryRef = noteCategoryRef;
        }

        public PatientNoteCategorySummary NoteCategorySummary
        {
            get { return _noteCategorySummary; }
        }

        public override void Start()
        {
            if (_isNew)
            {
                _noteCategoryDetail = new PatientNoteCategoryDetail();
            }
            else
            {
                Platform.GetService<INoteCategoryAdminService>(
                    delegate(INoteCategoryAdminService service)
                    {
                        LoadNoteCategoryForEditResponse response = service.LoadNoteCategoryForEdit(new LoadNoteCategoryForEditRequest(_noteCategoryRef));
                        _noteCategoryRef = response.NoteCategoryRef;
                        _noteCategoryDetail = response.NoteCategoryDetail;
                    });
            }

            Platform.GetService<INoteCategoryAdminService>(
                delegate(INoteCategoryAdminService service)
                {
                    GetNoteCategoryEditFormDataResponse response = service.GetNoteCategoryEditFormData(new GetNoteCategoryEditFormDataRequest());
                    _severityChoices = response.SeverityChoices;

                    if (_isNew && _noteCategoryDetail.Severity == null && response.SeverityChoices.Count > 0)
                    {
                        _noteCategoryDetail.Severity = response.SeverityChoices[0];
                    }
                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public PatientNoteCategoryDetail NoteCategoryDetail
        {
            get { return _noteCategoryDetail; }
            set { _noteCategoryDetail = value; }
        }

        #region Presentation Model

        public IList SeverityChoices
        {
            get { return _severityChoices; }
        }

        [ValidateNotNull]
        public EnumValueInfo Severity
        {
            get { return _noteCategoryDetail.Severity; }
            set
            {
            	_noteCategoryDetail.Severity = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Category
        {
            get { return _noteCategoryDetail.Category; }
            set 
            {
                _noteCategoryDetail.Category = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _noteCategoryDetail.Description; }
            set 
            {
                _noteCategoryDetail.Description = value;
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
                    ExceptionHandler.Report(e, SR.ExceptionSaveNoteCategory, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
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

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<INoteCategoryAdminService>(
                delegate(INoteCategoryAdminService service)
                {
                    if (_isNew)
                    {
                        AddNoteCategoryResponse response = service.AddNoteCategory(new AddNoteCategoryRequest(_noteCategoryDetail));
                        _noteCategoryRef = response.NoteCategory.NoteCategoryRef;
                        _noteCategorySummary = response.NoteCategory;
                    }
                    else
                    {
                        UpdateNoteCategoryResponse response = service.UpdateNoteCategory(new UpdateNoteCategoryRequest(_noteCategoryDetail));
                        _noteCategoryRef = response.NoteCategory.NoteCategoryRef;
                        _noteCategorySummary = response.NoteCategory;
                    }
                });
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
