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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using System.Collections;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReasonSelectionComponentBase"/>
	/// </summary>
	[ExtensionPoint]
	public class ReasonSelectionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProtocolReasonComponent class
	/// </summary>
	[AssociateView(typeof(ReasonSelectionComponentViewExtensionPoint))]
	public abstract class ReasonSelectionComponentBase : ApplicationComponent
	{
		private EnumValueInfo _selectedReason;
		private List<EnumValueInfo> _availableReasons;
		private string _otherReason;
		private ICannedTextLookupHandler _cannedTextLookupHandler;

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);
			_availableReasons = GetReasonChoices();

			base.Start();
		}

		#region PresentationModel

		protected abstract List<EnumValueInfo> GetReasonChoices();

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public EnumValueInfo Reason
		{
			get { return _selectedReason; }
		}

		public EnumValueInfo SelectedReasonChoice
		{
			get { return _selectedReason; }
			set
			{
				_selectedReason = value;
			}
		}

		public IList ReasonChoices
		{
			get { return _availableReasons; }
		}

		public string OtherReason
		{
			get { return _otherReason; }
			set { _otherReason = value; }
		}

		public bool AcceptEnabled
		{
			get { return _selectedReason != null; }
		}

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
