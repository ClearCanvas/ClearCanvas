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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
	/// <summary>
	/// A context used when applying rules and actions within the ImageServer.
	/// </summary>
	/// <remarks>
	/// This class is used to pass information to rules and to the action procesor
	/// when applying rules within the ImageServer.  It should contain enough
	/// information to apply a given Action for a rule.
	/// </remarks>
	/// <seealso cref="ServerRulesEngine"/>
	public class ServerActionContext : ActionContext
	{
		#region Constructors

		public ServerActionContext(DicomMessageBase msg, ServerEntityKey filesystemKey,
		                           ServerPartition partition, ServerEntityKey studyLocationKey)
		{
			Message = msg;
			ServerPartitionKey = partition.Key;
			StudyLocationKey = studyLocationKey;
			FilesystemKey = filesystemKey;
			ServerPartition = partition;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The Server Rules Engine
		/// </summary>
		public IServerRulesEngine RuleEngine { get; set; }

		/// <summary>
		/// The partition of the object.
		/// </summary>
		public ServerEntityKey ServerPartitionKey { get; private set; }

		/// <summary>
		/// The key of the filesystem being worked with..
		/// </summary>
		public ServerEntityKey FilesystemKey { get; private set; }

		/// <summary>
		/// The study location key.
		/// </summary>
		public ServerEntityKey StudyLocationKey { get; private set; }

		/// <summary>
		/// The server partition itself.
		/// </summary>
		public ServerPartition ServerPartition { get; private set; }

		#endregion

	}
}
