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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.EnumerationAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IEnumerationAdminService))]
	public class EnumerationAdminService : ApplicationServiceBase, IEnumerationAdminService
	{
		#region IEnumerationAdminService Members

		[ReadOperation]
		public ListEnumerationsResponse ListEnumerations(ListEnumerationsRequest request)
		{
			var broker = PersistenceContext.GetBroker<IMetadataBroker>();
			var enumClasses = broker.ListEnumValueClasses();

			var enumerations = CollectionUtils.Map<Type, EnumerationSummary, List<EnumerationSummary>>(enumClasses,
				enumClass => new EnumerationSummary(enumClass.AssemblyQualifiedName, enumClass.Name, IsSoftEnum(enumClass)));

			return new ListEnumerationsResponse(enumerations);
		}

		[ReadOperation]
		public ListEnumerationValuesResponse ListEnumerationValues(ListEnumerationValuesRequest request)
		{
			var enumBroker = PersistenceContext.GetBroker<IEnumBroker>();
			var enumValues = enumBroker.Load(GetEnumClass(request.AssemblyQualifiedClassName), request.IncludeDeactivated);
			return new ListEnumerationValuesResponse(
				CollectionUtils.Map(enumValues, 
				(EnumValue value) => new EnumValueAdminInfo(value.Code, value.Value, value.Description, value.Deactivated)));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Enumeration)]
		public AddValueResponse AddValue(AddValueRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Value, "Value");

			var enumClass = GetEnumClass(request.AssemblyQualifiedClassName);

			// compute a display order for the new value
			var displayOrder = ComputeDisplayOrderValue(enumClass, request.Value.Code,
				request.InsertAfter == null ? null : request.InsertAfter.Code);

			// add the new value
			var broker = PersistenceContext.GetBroker<IEnumBroker>();
			broker.AddValue(enumClass, request.Value.Code, request.Value.Value, request.Value.Description, displayOrder,
				IsSoftEnum(enumClass) ? request.Value.Deactivated : false);

			return new AddValueResponse();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Enumeration)]
		public EditValueResponse EditValue(EditValueRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Value, "Value");

			var enumClass = GetEnumClass(request.AssemblyQualifiedClassName);

			// compute display order value
			var displayOrder = ComputeDisplayOrderValue(enumClass, request.Value.Code,
				request.InsertAfter == null ? null : request.InsertAfter.Code);

			var broker = PersistenceContext.GetBroker<IEnumBroker>();
			broker.UpdateValue(enumClass, request.Value.Code, request.Value.Value, request.Value.Description, displayOrder,
				IsSoftEnum(enumClass) ? request.Value.Deactivated : false);

			return new EditValueResponse();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Enumeration)]
		public RemoveValueResponse RemoveValue(RemoveValueRequest request)
		{
			Type enumClass = null;
			
			try
			{
				enumClass = GetEnumClass(request.AssemblyQualifiedClassName);

			// Client side should enforce this.  But just in case it does not.
			if (IsSoftEnum(enumClass) == false)
				throw new RequestValidationException(SR.ExceptionUnableToDeleteHardEnumeration);

				var broker = PersistenceContext.GetBroker<IEnumBroker>();
				broker.RemoveValue(enumClass, request.Value.Code);
				PersistenceContext.SynchState();
				return new RemoveValueResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(enumClass)));
			}
		}

		#endregion

		private static Type GetEnumClass(string enumerationName)
		{
			var enumClass = Type.GetType(enumerationName);
			if (enumClass == null)
				throw new RequestValidationException(SR.InvalidRequest_InvalidEnumerationName);

			return enumClass;
		}

		private static bool IsSoftEnum(Type enumClass)
		{
			return !CollectionUtils.Contains(enumClass.Assembly.GetTypes(),
				t => t.IsEnum && AttributeUtils.HasAttribute<EnumValueClassAttribute>(t, false, attr => attr.EnumValueClass.Equals(enumClass)));
		}

		private float ComputeDisplayOrderValue(Type enumValueClass, string code, string insertAfterCode)
		{
			var broker = PersistenceContext.GetBroker<IEnumBroker>();

			// get insertAfter value, which may be null if the value is to be inserted at the beginning
			var insertAfter = insertAfterCode == null ? null : broker.Find(enumValueClass, insertAfterCode);
			if (insertAfter != null && insertAfter.Code == code)
				throw new RequestValidationException(SR.InvalidRequest_EnumerationValueCannotInsertAfterSelf);

			// get the insertBefore value (the value immediately following insertAfter)
			// this may be null if insertAfter is the last value in the set
			var values = broker.Load(enumValueClass, true);
			var insertAfterIndex = insertAfter == null ? -1 : values.IndexOf(insertAfter);
			var insertBefore = (insertAfterIndex + 1 == values.Count) ? null : values[insertAfterIndex + 1];

			// if the insertBefore value is the same as the value being edited, then there is no change in displayOrder
			if (insertBefore != null && insertBefore.Code == code)
				return insertBefore.DisplayOrder;

			// otherwise compute a new display order value
			var lower = insertAfter == null ? 0 : insertAfter.DisplayOrder;
			return insertBefore == null ? lower + 1 : (lower + insertBefore.DisplayOrder) / 2;
		}
	}
}
