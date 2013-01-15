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
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("CannedText")]
	public class CannedTextImex : XmlEntityImex<CannedText, CannedTextImex.CannedTextData>
	{
		[DataContract]
		public class CannedTextData
		{
			[DataMember]
			public string Name;

			[DataMember]
			public string Category;

			[DataMember]
			public string StaffId;

			[DataMember]
			public string StaffGroupName;

			[DataMember]
			public string Text;
		}


		#region Overrides

		protected override IList<CannedText> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			CannedTextSearchCriteria where = new CannedTextSearchCriteria();
			where.Name.SortAsc(0);

			return context.GetBroker<ICannedTextBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override CannedTextData Export(CannedText entity, IReadContext context)
		{
			CannedTextData data = new CannedTextData();
			data.Name = entity.Name;
			data.Category = entity.Category;
			data.Text = entity.Text;
			data.StaffId = entity.Staff == null ? null : entity.Staff.Id;
			data.StaffGroupName = entity.StaffGroup == null ? null : entity.StaffGroup.Name;
			return data;
		}

		protected override void Import(CannedTextData data, IUpdateContext context)
		{
			CreateOrUpdateCannedText(data.Name, data.Category, data.StaffId, data.StaffGroupName, data.Text, context);
		}

		#endregion

		private static void CreateOrUpdateCannedText(
			string name, 
			string category, 
			string staffId, 
			string staffGroupName, 
			string text,
			IPersistenceContext context)
		{
			try
			{
				// At least one of these should be populated
				if (staffId == null && staffGroupName == null)
					throw new Exception("A canned text has a staff or a staff group.  They cannot both be empty");

				if (staffId != null && staffGroupName != null)
					throw new Exception("A canned text has a staff or a staff group.  They cannot both exist");

				CannedTextSearchCriteria criteria = new CannedTextSearchCriteria();

				// We must search all these criteria because the combination of them form a unique key
				criteria.Name.EqualTo(name);
				criteria.Category.EqualTo(category);
				if (!string.IsNullOrEmpty(staffId))
					criteria.Staff.Id.EqualTo(staffId);
				if (!string.IsNullOrEmpty(staffGroupName))
					criteria.StaffGroup.Name.EqualTo(staffGroupName);

				ICannedTextBroker broker = context.GetBroker<ICannedTextBroker>();
				CannedText cannedText = broker.FindOne(criteria);

				cannedText.Text = text;
			}
			catch (EntityNotFoundException)
			{
				Staff staff = FindStaff(staffId, context);
				StaffGroup staffGroup = FindStaffGroup(staffGroupName, context);

				if (!string.IsNullOrEmpty(staffId) && staff == null)
					throw new Exception("The requested staff does not exist.");

				if (!string.IsNullOrEmpty(staffGroupName) && staffGroup == null)
					throw new Exception("The requested staff group does not exist.");

				CannedText cannedText = new CannedText();
				cannedText.Name = name;
				cannedText.Category = category;
				cannedText.Staff = staff;
				cannedText.StaffGroup = staffGroup;
				cannedText.Text = text;
				context.Lock(cannedText, DirtyState.New);
			}
		}

		private static Staff FindStaff(string staffId, IPersistenceContext context)
		{
			try
			{
				if (string.IsNullOrEmpty(staffId))
					return null;

				StaffSearchCriteria criteria = new StaffSearchCriteria();
				criteria.Id.EqualTo(staffId);

				IStaffBroker broker = context.GetBroker<IStaffBroker>();
				return broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				return null;
			}
		}

		private static StaffGroup FindStaffGroup(string staffGroupName, IPersistenceContext context)
		{
			try
			{
				if (string.IsNullOrEmpty(staffGroupName))
					return null;

				StaffGroupSearchCriteria criteria = new StaffGroupSearchCriteria();
				criteria.Name.EqualTo(staffGroupName);

				IStaffGroupBroker broker = context.GetBroker<IStaffGroupBroker>();
				return broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				return null;
			}
		}
	}
}
