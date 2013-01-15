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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public class OrderAdditionalInfoComponent : DHtmlComponent
	{
		[DataContract]
		public class HealthcareContext : DataContractBase
		{
			public HealthcareContext(EntityRef orderRef)
			{
				this.OrderRef = orderRef;
			}

			[DataMember]
			public EntityRef OrderRef;
		}


		private IDictionary<string, string> _orderExtendedProperties;
		private HealthcareContext _healthcareContext;
		private readonly bool _readOnly;

		public OrderAdditionalInfoComponent()
			: this(false)
		{
		}

		public OrderAdditionalInfoComponent(bool readOnly)
		{
			_orderExtendedProperties = new Dictionary<string, string>();
			_readOnly = readOnly;
		}

		/// <summary>
		/// Gets or sets the dictionary of order extended properties that this component will
		/// use to store data.
		/// </summary>
		public IDictionary<string, string> OrderExtendedProperties
		{
			get { return _orderExtendedProperties; }
			set
			{
				_orderExtendedProperties = value;

				// refresh the page
				SetUrl(WebResourcesSettings.Default.OrderAdditionalInfoPageUrl);
			}
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.OrderAdditionalInfoPageUrl);
			base.Start();
		}

		protected override IDictionary<string, string> TagData
		{
			get
			{
				return _orderExtendedProperties;
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _healthcareContext;
		}

		public HealthcareContext Context
		{
			get { return _healthcareContext; }
			set
			{
				_healthcareContext = value;
				NotifyAllPropertiesChanged();
			}
		}

		protected override string GetTag(string tag)
		{
			if (string.Equals("ReadOnly", tag))
			{
				return _readOnly ? "true" : "false";
			}
			return base.GetTag(tag);
		}
	}
}
