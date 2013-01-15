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
using System.Linq;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Implements a test stub for <see cref="ViewerIntegrationExtensionPoint"/>, for debugging purposes.
	/// </summary>
	//This extension is normally commented out - uncomment to use for debugging (but do not commit!!)
	//[ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
	public class TestViewerIntegration : IViewerIntegration
	{
		class StudyViewer : IStudyViewer
		{
			private readonly string _id;
			private bool _closed;

			internal StudyViewer(string id)
			{
				_id = id;
				Log("open", _id);
			}

			public bool Activate()
			{
				if (_closed)
					return false;
				Log("activate", _id);
				return true;
			}

			public void Close()
			{
				if (_closed)
					return;

				_closed = true;
				Log("close", _id);
			}
		}


		public IStudyViewer[] ViewStudies(ViewStudiesArgs args)
		{
			return args.InstancePerStudy ? 
				args.StudyInstanceUids.Select(uid => new StudyViewer(uid)).Cast<IStudyViewer>().ToArray()
				: new IStudyViewer[] { new StudyViewer(string.Join("; ", args.StudyInstanceUids)) };
		}

		private static void Log(string verb, string id)
		{
			Platform.Log(LogLevel.Debug, "{0}: {1} Identity = {2}", typeof(TestViewerIntegration).Name, verb, id);
		}
	}

}
