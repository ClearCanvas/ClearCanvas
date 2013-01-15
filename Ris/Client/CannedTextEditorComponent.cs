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
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CannedTextEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CannedTextEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextEditorComponent class
	/// </summary>
	[AssociateView(typeof(CannedTextEditorComponentViewExtensionPoint))]
	public class CannedTextEditorComponent : ApplicationComponent
	{
		class DummyItem
		{
			private readonly string _displayString;

			public DummyItem(string displayString)
			{
				_displayString = displayString;
			}

			public override string ToString()
			{
				return _displayString;
			}
		}

		private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);

		private readonly EntityRef _cannedTextRef;
		private readonly List<string> _categoryChoices;

		private CannedTextDetail _cannedTextDetail;
		private CannedTextSummary _cannedTextSummary;

		private List<StaffGroupSummary> _staffGroupChoices;

		private readonly bool _isDuplicate;
		private readonly bool _isNew;
		private readonly bool _canChangeType;
		private bool _isEditingPersonal;

		/// <summary>
		/// Constructor for creating a new canned text.
		/// </summary>
		public CannedTextEditorComponent(List<string> categoryChoices)
		{
			_isNew = true;
			_canChangeType = HasPersonalAdminAuthority && HasGroupAdminAuthority;
			_categoryChoices = categoryChoices;
		}

		/// <summary>
		/// Constructor for editing an existing canned text.
		/// </summary>
		public CannedTextEditorComponent(List<string> categoryChoices, EntityRef cannedTextRef)
		{
			_isNew = false;
			_cannedTextRef = cannedTextRef;
			_categoryChoices = categoryChoices;

			_canChangeType = false;
		}

		/// <summary>
		/// Constructor for duplicating an existing canned text.
		/// </summary>
		public CannedTextEditorComponent(List<string> categoryChoices, EntityRef cannedTextRef, bool duplicate)
			: this(categoryChoices)
		{
			_cannedTextRef = cannedTextRef;
			_isDuplicate = duplicate;
		}

		public override void Start()
		{
			// Insert a blank choice as the first element
			_categoryChoices.Insert(0, "");

			Platform.GetService<ICannedTextService>(
				service =>
				{
					var formDataResponse = service.GetCannedTextEditFormData(new GetCannedTextEditFormDataRequest());
					_staffGroupChoices = formDataResponse.StaffGroups;


					if (_isNew && _isDuplicate == false)
					{
						_cannedTextDetail = new CannedTextDetail();
						_isEditingPersonal = HasPersonalAdminAuthority;
					}
					else
					{
						var response = service.LoadCannedTextForEdit(new LoadCannedTextForEditRequest(_cannedTextRef));
						_cannedTextDetail = response.CannedTextDetail;

						_isEditingPersonal = _cannedTextDetail.IsPersonal;

						if (_isDuplicate)
							this.Name = "";
					}
				});

			// The selected staff groups should only contain entries in the selected group choices
			if (_cannedTextDetail.StaffGroup == null)
			{
				_cannedTextDetail.StaffGroup = CollectionUtils.FirstElement(_staffGroupChoices);
			}
			else
			{
				_cannedTextDetail.StaffGroup = CollectionUtils.SelectFirst(_staffGroupChoices,
						choice => _cannedTextDetail.StaffGroup.StaffGroupRef.Equals(choice.StaffGroupRef, true));
			}

			// add validation rule to ensure the group must be populated when editing group
			this.Validation.Add(new ValidationRule("StaffGroup",
				delegate
				{
					var ok = this.IsEditingPersonal || this.IsEditingGroup && this.StaffGroup != null;
					return new ValidationResult(ok, Desktop.SR.MessageValueRequired);
				}));

			if (CannedTextSettings.Default.RestrictNameToAlphaChars)
			{
				// add validation rule to ensure the name does not contain invalid characters
				this.Validation.Add(new ValidationRule("Name",
					delegate
					{
						if (string.IsNullOrEmpty(this.Name))
							return new ValidationResult(true, "");

						// only allow alphabets and space
						var ok = Regex.IsMatch(this.Name, @"^[A-Za-z ]+$");
						return new ValidationResult(ok, SR.MessageCannedTextNameCanOnlyContainAlphaChars);
					}));
			}

			if (CannedTextSettings.Default.RestrictFieldsToAlphaChars)
			{
				// add validation rule to ensure the name field in the text does not contain invalid characters
				this.Validation.Add(new ValidationRule("Text",
					delegate
					{
						if (string.IsNullOrEmpty(this.Text))
							return new ValidationResult(true, "");

						// look for none alphabets and space within the named field square brackets
						// Patterns explaination
						//		\[				- match beginning bracket
						//			[^\[\]]*	- match any number of non brackets characters
						//			[^a-zA-Z ]	- match at least one alphabets and space characters
						//			[^\[\]]*	- match any number of non brackets characters
						//		\]				- match ending bracket
						var match = Regex.Match(this.Text, @"\[[^\[\]]*[^a-zA-Z ][^\[\]]*\]");
						var message = match.Success ? string.Format(SR.MessageCannedTextNameFieldCanOnlyContainAlphaChars, match.Value) : null;
						return new ValidationResult(!match.Success, message);
					}));
			}

			base.Start();
		}

		/// <summary>
		/// Returns the Canned Text summary for use by the caller of this component
		/// </summary>
		public CannedTextSummary UpdatedCannedTextSummary
		{
			get { return _cannedTextSummary; }
		}

		#region Presentation Model

		public bool IsNew
		{
			get { return _isNew; }
		}

		public bool CanChangeType
		{
			get { return _canChangeType; }
		}

		public bool IsEditingPersonal
		{
			get{ return _isEditingPersonal; }
			set{ _isEditingPersonal = value; }
		}

		public bool IsEditingGroup
		{
			get { return !this.IsEditingPersonal; }
			set { this.IsEditingPersonal = !value; }
		}

		public bool IsReadOnly
		{
			get
			{
				return this.IsEditingPersonal && !HasPersonalAdminAuthority ||
						this.IsEditingGroup && !HasGroupAdminAuthority;
			}
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

		public object NullFilterItem
		{
			get { return _nullFilterItem; }
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _cannedTextDetail.Name; }
			set
			{
				_cannedTextDetail.Name = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Category
		{
			get { return _cannedTextDetail.Category; }
			set
			{
				_cannedTextDetail.Category = value;
				this.Modified = true;
			}
		}

		public bool HasGroupAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.CannedText.Group); }
		}

		public bool HasPersonalAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.CannedText.Personal); }
		}

		[ValidateNotNull]
		public string Text
		{
			get { return _cannedTextDetail.Text; }
			set
			{
				_cannedTextDetail.Text = value;
				this.Modified = true;
			}
		}

		// Dynamic Null Validation when IsEditingGroup
		public StaffGroupSummary StaffGroup
		{
			get { return _cannedTextDetail.StaffGroup; }
			set
			{
				_cannedTextDetail.StaffGroup = value;
				this.Modified = true;
			}
		}

		public IList StaffGroupChoices
		{
			get { return _staffGroupChoices; }
		}

		public string FormatStaffGroup(object item)
		{
			if (!(item is StaffGroupSummary))
				return item.ToString(); // place-holder items

			var staffGroup = (StaffGroupSummary)item;
			return staffGroup.Name;
		}


		public IList CategoryChoices
		{
			get { return _categoryChoices; }
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
				// Depends on the editing mode, remove the unnecessary information
				if (this.IsEditingPersonal)
					_cannedTextDetail.StaffGroup = null;

				// Warn user about possible name conflicts
				if(!WarnAboutNameConflicts())
					return;

				// Commit the changes
				Platform.GetService<ICannedTextService>(
					service =>
					{
						if (_isNew)
						{
							var response = service.AddCannedText(new AddCannedTextRequest(_cannedTextDetail));
							_cannedTextSummary = response.CannedTextSummary;
						}
						else
						{
							var response =
								service.UpdateCannedText(new UpdateCannedTextRequest(_cannedTextRef, _cannedTextDetail));
							_cannedTextSummary = response.CannedTextSummary;
						}
					});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e,
					SR.ExceptionSaveCannedText,
					this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		private bool WarnAboutNameConflicts()
		{
			List<CannedTextSummary> cannedTexts = null;
			Platform.GetService<ICannedTextService>(
				service =>
				{
					var response = service.ListCannedTextForUser(new ListCannedTextForUserRequest { Name = this.Name });
					cannedTexts = response.CannedTexts;
				});

			// If updating an existing canned text, remove the one being edited from the list
			if (!_isNew)
			{
				CollectionUtils.Remove(cannedTexts, c => c.CannedTextRef.Equals(_cannedTextRef, true));
			}

			// check for an exact match
			var exactMatch = CollectionUtils.Contains(cannedTexts, c => Equals(this.Category, c.Category) && Equals(this.StaffGroup, c.StaffGroup));
			if(exactMatch)
			{
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageIdenticalCannedText, MessageBoxActions.Ok);
				return false;
			}

			// Warn user if there are other cannedtext (for which the user has access to) with the same name
			if (cannedTexts.Count > 0)
			{
				var messageBuilder = new StringBuilder();

				messageBuilder.AppendLine(SR.MessageWarningDuplicateCannedTextName);
				messageBuilder.AppendLine();
				foreach (var c in cannedTexts)
				{
					messageBuilder.AppendLine(FormatCannedText(c));
				}

				if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.YesNo))
					return false;
			}
			return true;
		}

		private static string FormatCannedText(CannedTextSummary cannedText)
		{
			return string.Format("{0}, Category: {1}, Owner: {2}", cannedText.Name, cannedText.Category,
								 cannedText.IsPersonal ? SR.ColumnPersonal : cannedText.StaffGroup.Name);
		}
	}
}
