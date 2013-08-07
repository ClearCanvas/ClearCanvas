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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="UnmergeOrderComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class UnmergeOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// UnmergeOrderComponent class
	/// </summary>
	[AssociateView(typeof(UnmergeOrderComponentViewExtensionPoint))]
	public class UnmergeOrderComponent : ApplicationComponent
	{
		private EnumValueInfo _selectedReason;
		private List<EnumValueInfo> _cancelReasonChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public UnmergeOrderComponent()
		{
		}

		public override void Start()
		{
			Platform.GetService<IOrderEntryService>(
				service =>
				{
					_cancelReasonChoices = service.GetCancelOrderFormData(new GetCancelOrderFormDataRequest()).CancelReasonChoices;
				});

			base.Start();
		}

		#region Presentation Model

		public IList ReasonChoices
		{
			get { return _cancelReasonChoices; }
		}

		public EnumValueInfo SelectedReason
		{
			get { return _selectedReason; }
			set { _selectedReason = value; }
		}

		#endregion

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Unmerge()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public bool AcceptEnabled
		{
			get { return _selectedReason != null; }
		}
	}
}
