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

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client
{
	public abstract class OrderDetailViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class OrderContext : DataContractBase
		{
			public OrderContext(EntityRef orderRef)
			{
				this.OrderRef = orderRef;
			}

			[DataMember]
			public EntityRef OrderRef;
		}

        protected DataContractBase _context;

		public OrderDetailViewComponent()
			: this(null)
		{
		}

		public OrderDetailViewComponent(EntityRef orderRef)
		{
			_context = orderRef == null ? null : new OrderContext(orderRef);
		}

		public override void Start()
		{
			SetUrl(this.PageUrl);
			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected abstract string PageUrl { get; }

        public virtual DataContractBase Context
		{
			get { return _context; }
			set
			{
				_context = value;
				if(this.IsStarted)
				{
					NotifyAllPropertiesChanged();
				}
			}
		}
	}
}
