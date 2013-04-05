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
		float ProgressPercent { get; }
		event AsyncPixelDataProgressEventHandler ProgressChanged;
		bool IsLoaded { get; }
		event AsyncPixelDataEventHandler Loaded;
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
	public delegate void AsyncPixelDataProgressEventHandler(object sender, AsyncPixelDataProgressEventArgs e);

	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public class AsyncPixelDataProgressEventArgs : AsyncPixelDataEventArgs
	{
		public float ProgressPercent { get; private set; }
		public bool IsComplete { get; private set; }

		public AsyncPixelDataProgressEventArgs(float progressPercent, bool isComplete, byte[] pixelData)
			: base(pixelData)
		{
			ProgressPercent = progressPercent;
			IsComplete = isComplete;
		}
	}
}