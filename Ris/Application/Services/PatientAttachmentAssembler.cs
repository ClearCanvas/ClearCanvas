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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientAttachmentAssembler
    {
		class PatientAttachmentSynchronizeHelper : CollectionSynchronizeHelper<PatientAttachment, AttachmentSummary>
        {
            private readonly PatientAttachmentAssembler _assembler;
            private readonly IPersistenceContext _context;
            private readonly Staff _currentUserStaff;

            public PatientAttachmentSynchronizeHelper(PatientAttachmentAssembler assembler, Staff currentUserStaff, IPersistenceContext context)
                : base(true, true)
            {
                _assembler = assembler;
                _context = context;
                _currentUserStaff = currentUserStaff;
            }

			protected override bool CompareItems(PatientAttachment domainItem, AttachmentSummary sourceItem)
            {
                return Equals(domainItem.Document.GetRef(), sourceItem.Document.DocumentRef);
            }

			protected override void AddItem(AttachmentSummary sourceItem, ICollection<PatientAttachment> domainList)
            {
                PatientAttachment attachment = _assembler.CreatePatientAttachment(sourceItem, _currentUserStaff, _context);
                attachment.Document.Attach();
                domainList.Add(attachment);
            }

			protected override void UpdateItem(PatientAttachment domainItem, AttachmentSummary sourceItem, ICollection<PatientAttachment> domainList)
            {
                _assembler.UpdatePatientAttachment(domainItem, sourceItem, _context);
            }

            protected override void RemoveItem(PatientAttachment domainItem, ICollection<PatientAttachment> domainList)
            {
                domainList.Remove(domainItem);
                domainItem.Document.Detach();
            }
        }

		public void Synchronize(IList<PatientAttachment> domainList, IList<AttachmentSummary> sourceList, Staff currentUserStaff, IPersistenceContext context)
        {
            PatientAttachmentSynchronizeHelper synchronizer = new PatientAttachmentSynchronizeHelper(this, currentUserStaff, context);
            synchronizer.Synchronize(domainList, sourceList);
        }

		public AttachmentSummary CreatePatientAttachmentSummary(PatientAttachment attachment, IPersistenceContext context)
        {
            AttachedDocumentAssembler attachedDocAssembler = new AttachedDocumentAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();

			return new AttachmentSummary(
                EnumUtils.GetEnumValueInfo(attachment.Category),
                staffAssembler.CreateStaffSummary(attachment.AttachedBy, context),
                attachment.AttachedTime,
                attachedDocAssembler.CreateAttachedDocumentSummary(attachment.Document));
        }

		public PatientAttachment CreatePatientAttachment(AttachmentSummary summary, Staff currentUserStaff, IPersistenceContext context)
        {
            return new PatientAttachment(
                EnumUtils.GetEnumValue<PatientAttachmentCategoryEnum>(summary.Category, context),
                summary.AttachedBy == null ? currentUserStaff : context.Load<Staff>(summary.AttachedBy.StaffRef),
                Platform.Time,
                context.Load<AttachedDocument>(summary.Document.DocumentRef));
        }

		public void UpdatePatientAttachment(PatientAttachment attachment, AttachmentSummary summary, IPersistenceContext context)
        {
            AttachedDocumentAssembler attachedDocAssembler = new AttachedDocumentAssembler();
            attachment.Category = EnumUtils.GetEnumValue<PatientAttachmentCategoryEnum>(summary.Category, context);
            attachedDocAssembler.UpdateAttachedDocumentSummary(attachment.Document, summary.Document);
        }
    }
}
