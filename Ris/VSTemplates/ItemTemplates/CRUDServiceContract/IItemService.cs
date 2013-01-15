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
using System.ServiceModel;

namespace $rootnamespace$
{
    /// <summary>
    /// Provides operations to administer $fileinputname$ entities.
    /// </summary>
    [RisServiceProvider]
    [ServiceContract]
    public interface I$fileinputname$AdminService
    {
        /// <summary>
        /// Summary list of all items.
        /// </summary>
        [OperationContract]
        List$fileinputname$sResponse List$fileinputname$s(List$fileinputname$sRequest request);

        /// <summary>
        /// Loads details of specified itemfor editing.
        /// </summary>
        [OperationContract]
        Load$fileinputname$ForEditResponse Load$fileinputname$ForEdit(Load$fileinputname$ForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit an item.
        /// </summary>
        [OperationContract]
        Load$fileinputname$EditorFormDataResponse Load$fileinputname$EditorFormData(Load$fileinputname$EditorFormDataRequest request);

        /// <summary>
        /// Adds a new item.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        Add$fileinputname$Response Add$fileinputname$(Add$fileinputname$Request request);

        /// <summary>
        /// Updates an item.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        Update$fileinputname$Response Update$fileinputname$(Update$fileinputname$Request request);

    }
}
