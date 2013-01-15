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

namespace ClearCanvas.Dicom.Iod
{
	public interface IApplicationEntity
	{
        string Name { get; }
        string AETitle { get; }
		string Description { get; }
		string Location { get; }

	    // TODO (CR Mar 2012): Unsure about this
        IScpParameters ScpParameters { get; }
        IStreamingParameters StreamingParameters { get; }
    }

    public interface IScpParameters
    {
        string HostName { get; }
        int Port { get; }
    }

    public interface IStreamingParameters
    {
        int HeaderServicePort { get; }
        int WadoServicePort { get; }
    }
}
