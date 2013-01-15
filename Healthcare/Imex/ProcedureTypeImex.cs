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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("ProcedureType")]
	public class ProcedureTypeImex : XmlEntityImex<ProcedureType, ProcedureTypeImex.ProcedureTypeData>
	{
		[DataContract]
		public class ProcedureTypeData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string DefaultModality;

			[DataMember]
			public string BaseTypeId;

			[DataMember]
			public XmlDocument PlanXml;

			[DataMember]
			public int DefaultDuration;
		}



		#region Overrides

		protected override IList<ProcedureType> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<IProcedureTypeBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override ProcedureTypeData Export(ProcedureType entity, IReadContext context)
		{
			var data = new ProcedureTypeData
						{
							Deactivated = entity.Deactivated, 
							Id = entity.Id, 
							Name = entity.Name,
							DefaultDuration = entity.DefaultDuration
						};

			if(entity.Plan.IsDefault)
			{
				data.DefaultModality = entity.Plan.DefaultModality.Id;
			}
			else
			{
				data.PlanXml = entity.Plan.AsXml();
				if (entity.BaseType != null)
				{
					data.BaseTypeId = entity.BaseType.Id;
				}
			}

			return data;
		}

		protected override void Import(ProcedureTypeData data, IUpdateContext context)
		{
			var pt = LoadOrCreateProcedureType(data.Id, data.Name, context);
			pt.Deactivated = data.Deactivated;
			pt.DefaultDuration = data.DefaultDuration;
			if (!string.IsNullOrEmpty(data.DefaultModality))
			{
				pt.Plan = ProcedurePlan.CreateDefaultPlan(data.Name, GetModality(data.DefaultModality, context));
			}
			else
			{
				pt.Plan = new ProcedurePlan(data.PlanXml);
				if (!string.IsNullOrEmpty(data.BaseTypeId))
				{
					pt.BaseType = LoadOrCreateProcedureType(data.BaseTypeId, data.BaseTypeId, context);
				}
			}
		}

		#endregion

		private static ProcedureType LoadOrCreateProcedureType(string id, string name, IPersistenceContext context)
		{
			ProcedureType pt;
			try
			{
				// see if already exists in db
				var where = new ProcedureTypeSearchCriteria();
				where.Id.EqualTo(id);
				pt = context.GetBroker<IProcedureTypeBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				pt = new ProcedureType(id, name);
				context.Lock(pt, DirtyState.New);
			}

			return pt;
		}

		private static Modality GetModality(string id, IPersistenceContext context)
		{
			var where = new ModalitySearchCriteria();
			where.Id.EqualTo(id);
			return context.GetBroker<IModalityBroker>().FindOne(where);
		}
	}
}
