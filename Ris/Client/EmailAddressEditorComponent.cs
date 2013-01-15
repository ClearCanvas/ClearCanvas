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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="EmailAddressEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class EmailAddressEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// EmailAddressEditorComponent class
	/// </summary>
	[AssociateView(typeof(EmailAddressEditorComponentViewExtensionPoint))]
	public class EmailAddressEditorComponent : ApplicationComponent
	{
		private readonly EmailAddressDetail _emailAddress;

		/// <summary>
		/// Constructor
		/// </summary>
		public EmailAddressEditorComponent(EmailAddressDetail emailAddress)
		{
			_emailAddress = emailAddress;
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ValidUntil",
				delegate
				{
					// only need to validate the if both ValidFrom and ValidUntil are specified
					if (!_emailAddress.ValidRangeFrom.HasValue || !_emailAddress.ValidRangeUntil.HasValue)
						return new ValidationResult(true, "");

					var ok = DateTime.Compare(_emailAddress.ValidRangeUntil.Value, _emailAddress.ValidRangeFrom.Value) >= 0;
					return new ValidationResult(ok, SR.MessageValidUntilMustBeLaterOrEqualValidFrom);
				}));
			
			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		[ValidateRegex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", AllowNull = true)]
		public string Address
		{
			get { return _emailAddress.Address; }
			set
			{
				_emailAddress.Address = value;
				this.Modified = true;
			}
		}


		public DateTime? ValidFrom
		{
			get { return _emailAddress.ValidRangeFrom; }
			set
			{
				_emailAddress.ValidRangeFrom = value == null ? value : value.Value.Date;
				this.Modified = true;
			}
		}

		public DateTime? ValidUntil
		{
			get { return _emailAddress.ValidRangeUntil; }
			set
			{
				_emailAddress.ValidRangeUntil = value == null ? value : value.Value.Date;
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
				this.ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
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

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		#endregion
	}
}
