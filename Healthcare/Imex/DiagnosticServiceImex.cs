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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("DiagnosticService")]
	public class DiagnosticServiceImex : XmlEntityImex<DiagnosticService, DiagnosticServiceImex.DiagnosticServiceData>
	{
		[DataContract]
		public class DiagnosticServiceData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public List<ProcedureTypeData> ProcedureTypes;

		}

		[DataContract]
		public class ProcedureTypeData
		{
			[DataMember]
			public string Id;
		}

		#region Overrides

		protected override IList<DiagnosticService> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new DiagnosticServiceSearchCriteria();
			where.Id.SortAsc(0);
			return context.GetBroker<IDiagnosticServiceBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override DiagnosticServiceData Export(DiagnosticService entity, IReadContext context)
		{
			var data = new DiagnosticServiceData
				{
					Deactivated = entity.Deactivated,
					Id = entity.Id,
					Name = entity.Name,
					ProcedureTypes = CollectionUtils.Map<ProcedureType, ProcedureTypeData>(entity.ProcedureTypes, pt => new ProcedureTypeData { Id = pt.Id })
				};

			return data;
		}

		protected override void Import(DiagnosticServiceData data, IUpdateContext context)
		{
			var ds = GetDiagnosticService(data.Id, data.Name, context);
			ds.Deactivated = data.Deactivated;
			ds.Name = data.Name;

			if (data.ProcedureTypes != null)
			{
				foreach (var s in data.ProcedureTypes)
				{
					var where = new ProcedureTypeSearchCriteria();
					where.Id.EqualTo(s.Id);
					var pt = CollectionUtils.FirstElement(context.GetBroker<IProcedureTypeBroker>().Find(where));
					if (pt != null)
						ds.ProcedureTypes.Add(pt);
				}
			}
		}

		#endregion


		private static DiagnosticService GetDiagnosticService(string id, string name, IPersistenceContext context)
		{
			DiagnosticService ds;
			try
			{
				// see if already exists in db
				var where = new DiagnosticServiceSearchCriteria();
				where.Id.EqualTo(id);
				ds = context.GetBroker<IDiagnosticServiceBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				ds = new DiagnosticService(id, name);
				context.Lock(ds, DirtyState.New);
			}

			return ds;
		}
	}
}
