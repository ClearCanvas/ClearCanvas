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

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core
{
	public class ApplySopRulesCommand : CommandBase
	{
		private readonly ServerActionContext _context;
		private readonly ServerRulesEngine _engine;

		public ApplySopRulesCommand(ServerActionContext context, ServerRulesEngine engine)
			: base("Apply SOP Rules Engine and insert Archival Request", false)
		{
			_context = context;
			_engine = engine;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			// Run the rules engine against the object.
			_engine.Execute(_context);

			// Do insert into the archival queue.  Note that we re-run this with each object processed
			// so that the scheduled time is pushed back each time.  Note, however, if the study only 
			// has one image, we could incorrectly insert an ArchiveQueue request, since the 
			// study rules haven't been run.  We re-run the command when the study processed
			// rules are run to remove out the archivequeue request again, if it isn't needed.
			_context.CommandProcessor.AddCommand(
				new InsertArchiveQueueCommand(_context.ServerPartitionKey, _context.StudyLocationKey));
		}

		protected override void OnUndo()
		{
			
		}
	}
}