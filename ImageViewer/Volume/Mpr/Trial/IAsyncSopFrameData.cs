#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public interface IAsyncSopFrameData : ISopFrameData
	{
		/// <summary>
		/// Gets the current loading progress of the asynchronous data source as a percent between 0 and 100.
		/// </summary>
		int ProgressPercent { get; }

		/// <summary>
		/// Event raised when progress of the asynchronous data source has changed.
		/// </summary>
		event AsyncPixelDataProgressEventHandler ProgressChanged;

		/// <summary>
		/// Gets whether or not the asynchronous data source is loaded and ready for use.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Event raised when the asynchronous data source becomes loaded and ready for use.
		/// </summary>
		event AsyncPixelDataEventHandler Loaded;

		/// <summary>
		/// Gets whether or not the asynchronous data source is in a faulted state.
		/// </summary>
		bool IsFaulted { get; }

		/// <summary>
		/// Event raised when the asynchronous data source encounters an error in the background loading operation and enters a faulted state.
		/// </summary>
		event AsyncPixelDataFaultEventHandler Faulted;

		/// <summary>
		/// Gets an <see cref="IDisposable"/> lock on the asynchronous data source, ensuring it will not become unloaded until the lock is disposed.
		/// </summary>
		/// <returns></returns>
		IDisposable AcquireLock();
	}

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public delegate void AsyncPixelDataEventHandler(object sender, AsyncPixelDataEventArgs e);

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public class AsyncPixelDataEventArgs : EventArgs
	{
		public byte[] PixelData { get; private set; }

		public AsyncPixelDataEventArgs(byte[] pixelData)
		{
			PixelData = pixelData;
		}
	}

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public delegate void AsyncPixelDataFaultEventHandler(object sender, AsyncPixelDataFaultEventArgs e);

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public class AsyncPixelDataFaultEventArgs : AsyncPixelDataEventArgs
	{
		public Exception Exception { get; private set; }

		public AsyncPixelDataFaultEventArgs(byte[] pixelData, Exception exception)
			: base(pixelData)
		{
			Exception = exception;
		}
	}

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public delegate void AsyncPixelDataProgressEventHandler(object sender, AsyncPixelDataProgressEventArgs e);

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public class AsyncPixelDataProgressEventArgs : AsyncPixelDataEventArgs
	{
		public int ProgressPercent { get; private set; }
		public bool IsComplete { get; private set; }

		public AsyncPixelDataProgressEventArgs(int progressPercent, bool isComplete, byte[] pixelData)
			: base(pixelData)
		{
			ProgressPercent = progressPercent;
			IsComplete = isComplete;
		}
	}

	/// <summary>
	/// Represents errors that are encountered by an asynchronous data source during the background loading operation.
	/// </summary>
	public class AsyncSopDataSourceException : Exception
	{
		public AsyncSopDataSourceException(Exception innerException)
			: base(innerException.Message, innerException) {}
	}
}