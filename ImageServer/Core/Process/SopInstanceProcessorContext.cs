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

using ClearCanvas.ImageServer.Common.WorkQueue;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Process
{
	/// <summary>
	/// Represents the context during processing of DICOM object.
	/// </summary>
	public class SopInstanceProcessorContext
	{
		#region Private Members

		private readonly ServerCommandProcessor _commandProcessor;
		private readonly StudyStorageLocation _studyLocation;
		private readonly string _group;
		private readonly ExternalRequestQueue _request;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an instance of <see cref="SopInstanceProcessorContext"/>
		/// </summary>
		/// <param name="commandProcessor">The <see cref="ServerCommandProcessor"/> used in the context</param>
		/// <param name="studyLocation">The <see cref="StudyStorageLocation"/> of the study being processed</param>
		/// <param name="uidGroup">A String value respresenting the group of SOP instances which are being processed.</param>
		/// <param name="request">An external request that may have triggered this item.</param>
		public SopInstanceProcessorContext(ServerCommandProcessor commandProcessor, StudyStorageLocation studyLocation,
									string uidGroup, ExternalRequestQueue request = null)
		{
			_commandProcessor = commandProcessor;
			_studyLocation = studyLocation;
			_group = uidGroup;
			_request = request;
		}

		#endregion

		#region Public Properties

		public ServerCommandProcessor CommandProcessor
		{
			get { return _commandProcessor; }
		}

		public StudyStorageLocation StudyLocation
		{
			get { return _studyLocation; }
		}

		public string Group
		{
			get { return _group; }
		}

		public ExternalRequestQueue Request
		{
			get { return _request; }
		}

		/// <summary>
		/// If set, sets the DuplicateProcessing policy for the imported SOP.
		/// </summary>
		public DuplicateProcessingEnum? DuplicateProcessing { get; set; }
		#endregion
	}
}
