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
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="EnumerationEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EnumerationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EnumerationEditorComponent class
    /// </summary>
    [AssociateView(typeof(EnumerationEditorComponentViewExtensionPoint))]
    public class EnumerationEditorComponent : ApplicationComponent
    {
        private readonly object InsertAtBeginning = new object();

        private readonly EnumValueAdminInfo _enumValue;
		private readonly IList<EnumValueAdminInfo> _otherValues;
		private EnumValueAdminInfo _insertAfter;

        private readonly bool _isNew;
        private readonly string _enumerationClassName;
        
        /// <summary>
        /// Constructor for add.
        /// </summary>
		public EnumerationEditorComponent(string enumerationClassName, IList<EnumValueAdminInfo> allValues)
        {
            _isNew = true;
            _enumerationClassName = enumerationClassName;
			_enumValue = new EnumValueAdminInfo();
            _otherValues = allValues;

            // default insertAfter to last value in list
            _insertAfter = _otherValues.Count == 0 ? null : _otherValues[_otherValues.Count - 1];
        }

        /// <summary>
        /// Constructor for edit.
        /// </summary>
        /// <param name="enumerationName"></param>
        /// <param name="value"></param>
        /// <param name="allValues"></param>
		public EnumerationEditorComponent(string enumerationName, EnumValueAdminInfo value, IList<EnumValueAdminInfo> allValues)
        {
            _isNew = false;
            _enumerationClassName = enumerationName;
            _enumValue = value;
			_otherValues = CollectionUtils.Reject(allValues, delegate(EnumValueAdminInfo v) { return v.Equals(value); });

            int index = allValues.IndexOf(value);
            _insertAfter = index > 0 ? allValues[index - 1] : null;
        }

        public override void Start()
        {
			this.Validation.Add(new ValidationRule("Code",
				delegate 
				{
					bool duplicateCode = CollectionUtils.Contains(_otherValues,
						delegate(EnumValueAdminInfo other) { return other.Code.Equals(_enumValue.Code, StringComparison.InvariantCultureIgnoreCase); });
					return new ValidationResult(!duplicateCode, string.Format("Code {0} is already defined.", _enumValue.Code));
				}));
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

		public EnumValueAdminInfo EnumValue
        {
            get { return _enumValue; }
        }

        #region Presentation Model

        public bool IsCodeReadOnly
        {
            get { return !_isNew; }
        }

        [ValidateNotNull]
        [ValidateRegex(@"^\w+$", Message = "MessageEnumCodeContainsInvalidChars", AllowNull = true)]
        public string Code
        {
            get { return _enumValue.Code; }
            set
            {
                _enumValue.Code = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string DisplayValue
        {
            get { return _enumValue.Value; }
            set
            {
                _enumValue.Value = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _enumValue.Description; }
            set
            {
                _enumValue.Description = value;
                this.Modified = true;
            }
        }

    	public bool IsActive
    	{
			get { return !_enumValue.Deactivated; }
			set { _enumValue.Deactivated = !value; }
    	}

        public IList InsertAfterChoices
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(InsertAtBeginning);
				foreach (EnumValueAdminInfo info in _otherValues)
                {
                    list.Add(info);
                }
                return list;
            }
        }

        public string FormatInsertAfterChoice(object item)
        {
            if (item == InsertAtBeginning)
                return "(Insert at beginning)";

			EnumValueAdminInfo info = (EnumValueAdminInfo)item;
            return string.Format("{0} - {1}", info.Code, info.Value);
        }

        public object InsertAfter
        {
            get { return _insertAfter ?? InsertAtBeginning; }
            set
            {
				EnumValueAdminInfo choice = value == InsertAtBeginning ? null : (EnumValueAdminInfo)value;
                if(!Equals(choice, _insertAfter))
                {
                    _insertAfter = choice;
                    this.Modified = true;
                }
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                Platform.GetService<IEnumerationAdminService>(
                    delegate(IEnumerationAdminService service)
                    {
                        if (_isNew)
                        {
                            service.AddValue(new AddValueRequest(_enumerationClassName, _enumValue, _insertAfter));
                        }
                        else
                        {
                            service.EditValue(new EditValueRequest(_enumerationClassName, _enumValue, _insertAfter));
                        }

                    });

                Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, _isNew ? SR.ExceptionEnumValueAdd : SR.ExceptionEnumValueUpdate, this.Host.DesktopWindow,
                    delegate()
                    {
                        Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        public void Cancel()
        {
            Exit(ApplicationComponentExitCode.None);
        }

        #endregion

    }
}
