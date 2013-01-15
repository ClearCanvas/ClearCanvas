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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.OrderNotes
{
    /// <summary>
    /// Defines a service contract for working with order notes.
    /// </summary>
    [ServiceContract]
    [RisApplicationService]
    public interface IOrderNoteService
    {
        /// <summary>
        /// Lists staff groups to which the current user belongs.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request);

        /// <summary>
        /// Add the current user to the specified staff groups.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        AddStaffGroupsResponse AddStaffGroups(AddStaffGroupsRequest request);

        /// <summary>
        /// Queries the contents of a specified notebox.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        QueryNoteboxResponse QueryNotebox(QueryNoteboxRequest request);

        /// <summary>
        /// Gets the entire conversation for a specified order.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetConversationResponse GetConversation(GetConversationRequest request);

        /// <summary>
        /// Get conversation editor form data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetConversationEditorFormDataResponse GetConversationEditorFormData(GetConversationEditorFormDataRequest request);

        /// <summary>
        /// Acknowledges and/or posts to the specified order conversation.  Can also be used to
        /// initiate a conversation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        AcknowledgeAndPostResponse AcknowledgeAndPost(AcknowledgeAndPostRequest request);
    }
}
