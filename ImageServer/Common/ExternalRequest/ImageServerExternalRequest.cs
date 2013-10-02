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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageServer.Common.ExternalRequest
{
    [DataContract(Namespace = ImageServerExternalRequestNamespace.Value)]
    [ImageServerExternalRequestType("42C5B5E3-8874-4399-972C-35878C579D89")]
    public abstract class ImageServerExternalRequest : DataContractBase, IEquatable<ImageServerExternalRequest>
    {        
        /// <summary>
        /// Specifies if the operation must run synchronously or asynchronously
        /// </summary>        
        public abstract ExecutionModeEnum ExecutionMode { get; }

        /// <summary>
        /// A string uniquely identifying the request type
        /// </summary>
        public abstract string ExternalRequestType { get; }

        /// <summary>
        /// A localized string description of the request type
        /// </summary>
        public abstract string RequestTypeString { get; }

        /// <summary>
        /// A localized string description of the request
        /// </summary>
        public abstract string RequestDescription { get; }

        /// <summary>
        /// A string uniquely identifying the operation.  Can be null.
        /// </summary>
        /// <remarks>
        /// The OperationToken is passed in notification messages if they resulted from a request.
        /// </remarks>
        [DataMember]
        public string OperationToken { get; set; }

		/// <summary>
		/// The user requesting the operation.  Can be null.
		/// </summary>
		[DataMember]
		public string User { get; set; }

        public bool Equals(ImageServerExternalRequest other)
        {
            if (other == null)
                return false;

            if (ExecutionMode != other.ExecutionMode)
                return false;
            if (!string.Equals(ExternalRequestType, other.ExternalRequestType))
                return false;
            if (!string.Equals(OperationToken, other.OperationToken))
                return false;

            return true;
        }
    }
}
