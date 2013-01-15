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
		private int _activeRetrieveThreads = 0;
		private int _activeDecompressThreads = 0;

		public bool CanRetrieveFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return !frameData.PixelDataRetrieved;
		}

		public void RetrieveFrame(Frame frame)
		{
			Interlocked.Increment(ref _activeRetrieveThreads);

			try
			{
				IStreamingSopDataSource dataSource = (IStreamingSopDataSource) frame.ParentImageSop.DataSource;
				IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);

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
			finally
			{
				Interlocked.Decrement(ref _activeRetrieveThreads);
			}
		}

		public bool CanDecompressFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return frameData.PixelDataRetrieved;
		}

		public void DecompressFrame(Frame frame)
		{
			Interlocked.Increment(ref _activeDecompressThreads);
			try
			{
				string message = String.Format("Decompressing Frame (active threads: {0})", Thread.VolatileRead(ref _activeDecompressThreads));
				Trace.WriteLine(message);

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
			finally
			{
				Interlocked.Decrement(ref _activeDecompressThreads);
			}
		}
	}
}