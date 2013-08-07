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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class PhoneNumbersEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
	public class PhoneNumberEditorComponent : ApplicationComponent
	{
		private readonly TelephoneDetail _phoneNumber;
		private readonly List<EnumValueInfo> _phoneTypeChoices;
		private readonly bool _phoneTypeEnabled;

		public PhoneNumberEditorComponent(TelephoneDetail phoneNumber, List<EnumValueInfo> phoneTypeChoices)
		{
			_phoneNumber = phoneNumber;
			_phoneTypeChoices = phoneTypeChoices;
			_phoneTypeEnabled = phoneTypeChoices.Count > 1;
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ValidUntil",
				delegate
				{
					// only need to validate the if both ValidFrom and ValidUntil are specified
					if (!_phoneNumber.ValidRangeFrom.HasValue || !_phoneNumber.ValidRangeUntil.HasValue)
						return new ValidationResult(true, "");

					var ok = DateTime.Compare(_phoneNumber.ValidRangeUntil.Value, _phoneNumber.ValidRangeFrom.Value) >= 0;
					return new ValidationResult(ok, SR.MessageValidUntilMustBeLaterOrEqualValidFrom);
				}));

			base.Start();
		}

		public string PhoneNumberMask
		{
			get { return TextFieldMasks.TelephoneNumberLocalMask; }
		}

		public string CountryCode
		{
			get { return _phoneNumber.CountryCode; }
			set
			{
				_phoneNumber.CountryCode = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string AreaCode
		{
			get { return _phoneNumber.AreaCode; }
			set
			{
				_phoneNumber.AreaCode = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Number
		{
			get { return _phoneNumber.Number; }
			set
			{
				_phoneNumber.Number = value;
				this.Modified = true;
			}
		}

		public string Extension
		{
			get { return _phoneNumber.Extension; }
			set
			{
				_phoneNumber.Extension = value;
				this.Modified = true;
			}
		}

		public bool PhoneTypeEnabled
		{
			get { return _phoneTypeEnabled; }
		}

		[ValidateNotNull]
		public EnumValueInfo PhoneType
		{
			get { return _phoneNumber.Type; }
			set
			{
				_phoneNumber.Type = value;
				this.Modified = true;
			}
		}

		public IList PhoneTypeChoices
		{
			get { return _phoneTypeChoices; }
		}

		public DateTime? ValidFrom
		{
			get { return _phoneNumber.ValidRangeFrom; }
			set
			{
				_phoneNumber.ValidRangeFrom = value == null ? value : value.Value.Date;
				this.Modified = true;
			}
		}

		public DateTime? ValidUntil
		{
			get { return _phoneNumber.ValidRangeUntil; }
			set
			{
				_phoneNumber.ValidRangeUntil = value == null ? value : value.Value.Date;
				this.Modified = true;
			}
		}
		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
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
