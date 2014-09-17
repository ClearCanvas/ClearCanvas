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
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingCorePrefetchingStrategy : ICorePrefetchingStrategy
	{
		public bool CanRetrieveFrame(Frame frame)
		{
            var streamingFrame = frame.ParentImageSop.DataSource as StreamingSopDataSource;
            if (streamingFrame == null)
                return false;

            IStreamingSopFrameData frameData = streamingFrame.GetFrameData(frame.FrameNumber);
			return !frameData.PixelDataRetrieved;
		}

		public void RetrieveFrame(Frame frame)
		{
			try
			{
				var dataSource = (IStreamingSopDataSource) frame.ParentImageSop.DataSource;
				var frameData = dataSource.GetFrameData(frame.FrameNumber);
				frameData.RetrievePixelData();
			}
			catch (OutOfMemoryException)
			{
				Platform.Log(LogLevel.Error, "Out of memory trying to retrieve pixel data.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error retrieving frame pixel data.");
			}
		}

		public bool CanDecompressFrame(Frame frame)
		{
		    var streamingFrame = frame.ParentImageSop.DataSource as StreamingSopDataSource;
			if (streamingFrame == null)
				return false;

			var frameData = streamingFrame.GetFrameData(frame.FrameNumber);
			return frameData.PixelDataRetrieved;
		}

		public void DecompressFrame(Frame frame)
		{
			try
			{
				//TODO: try to trigger header retrieval for data luts?
				frame.GetNormalizedPixelData();
			}
			catch (OutOfMemoryException)
			{
				Platform.Log(LogLevel.Error, "Out of memory trying to decompress pixel data.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error decompressing frame pixel data.");
			}
		}
	}
}