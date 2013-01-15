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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ProtocolAssembler
	{
		public ProtocolDetail CreateProtocolDetail(Protocol protocol, IPersistenceContext context)
		{
			var detail = new ProtocolDetail();
			var staffAssembler = new StaffAssembler();

			detail.ProtocolRef = protocol.GetRef();
			detail.Author = protocol.Author != null ? staffAssembler.CreateStaffSummary(protocol.Author, context) : null;
			detail.Supervisor = protocol.Supervisor != null ? staffAssembler.CreateStaffSummary(protocol.Supervisor, context) : null;
			detail.Status = EnumUtils.GetEnumValueInfo(protocol.Status, context);
			detail.Urgency = EnumUtils.GetEnumValueInfo(protocol.Urgency);
			detail.RejectReason = EnumUtils.GetEnumValueInfo(protocol.RejectReason);

			detail.Codes = protocol.Codes == null
				? new List<ProtocolCodeSummary>()
				: CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(protocol.Codes, CreateProtocolCodeSummary);

			var procedureAssembler = new ProcedureAssembler();
			detail.Procedures = protocol.Procedures == null
				? new List<ProcedureDetail>()
				: CollectionUtils.Map<Procedure, ProcedureDetail>(
					protocol.Procedures,
					procedure => procedureAssembler.CreateProcedureDetail(procedure, delegate { return false; }, false, context));

			return detail;
		}

		public ProtocolCodeDetail CreateProtocolCodeDetail(ProtocolCode pc)
		{
			return new ProtocolCodeDetail(pc.GetRef(), pc.Name, pc.Description, pc.Deactivated);
		}

		public ProtocolCodeSummary CreateProtocolCodeSummary(ProtocolCode code)
		{
			return new ProtocolCodeSummary(code.GetRef(), code.Name, code.Description, code.Deactivated);
		}

		public void UpdateProtocol(Protocol protocol, ProtocolDetail detail, IPersistenceContext context)
		{
			protocol.Urgency = EnumUtils.GetEnumValue<ProtocolUrgencyEnum>(detail.Urgency, context);

			// if detail specifies a supervisor use it otherwise retain previous supervisor (null or valued)
			protocol.Supervisor = detail.Supervisor != null ? context.Load<Staff>(detail.Supervisor.StaffRef) : protocol.Supervisor;

			protocol.Codes.Clear();
			foreach (var codeSummary in detail.Codes)
			{
				var code = context.Load<ProtocolCode>(codeSummary.ProtocolCodeRef, EntityLoadFlags.Proxy);
				protocol.Codes.Add(code);
			}
		}

		public ProtocolGroupSummary CreateProtocolGroupSummary(ProtocolGroup group)
		{
			return new ProtocolGroupSummary(group.GetRef(), group.Name, group.Description);
		}

		public ProtocolGroupDetail CreateProtocolGroupDetail(ProtocolGroup group, bool includeDeactivatedCodes, IPersistenceContext context)
		{
			var protocolCodes = includeDeactivatedCodes
				? group.Codes
				: CollectionUtils.Select(group.Codes, code => !code.Deactivated);

			var protocolCodeSummaries = CollectionUtils.Map<ProtocolCode, ProtocolCodeSummary>(
				protocolCodes,
				CreateProtocolCodeSummary);

			var assembler = new ProcedureTypeGroupAssembler();
			var groups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
				group.ReadingGroups,
				readingGroup => assembler.GetProcedureTypeGroupSummary(readingGroup, context));

			return new ProtocolGroupDetail(group.Name, group.Description, protocolCodeSummaries, groups);
		}
	}
}
