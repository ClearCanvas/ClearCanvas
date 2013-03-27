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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <remarks>
	/// This type is part of a trial API and is not intended for general use.
	/// </remarks>
	public class AsyncFrame : Frame
	{
		protected internal AsyncFrame(ImageSop parentImageSop, int frameNumber)
			: base(parentImageSop, frameNumber)
		{
			SupportsAsync = ParentImageSop.DataSource.GetFrameData(FrameNumber) is IAsyncSopFrameData;
		}

		public bool SupportsAsync { get; private set; }

		public override byte[] GetNormalizedPixelData()
		{
			return base.GetNormalizedPixelData() ?? new byte[Rows*Columns*BitsStored/8*SamplesPerPixel];
		}

		public float AsyncProgressPercent
		{
			get { return SupportsAsync ? ((IAsyncSopFrameData) ParentImageSop.DataSource.GetFrameData(FrameNumber)).ProgressPercent : 100; }
		}

		public event AsyncPixelDataProgressEventHandler AsyncProgressChanged
		{
			add { if (SupportsAsync) ((IAsyncSopFrameData) ParentImageSop.DataSource.GetFrameData(FrameNumber)).ProgressChanged += value; }
			remove { if (SupportsAsync) ((IAsyncSopFrameData) ParentImageSop.DataSource.GetFrameData(FrameNumber)).ProgressChanged -= value; }
		}

		public bool IsAsyncLoaded
		{
			get { return !SupportsAsync || ((IAsyncSopFrameData) ParentImageSop.DataSource.GetFrameData(FrameNumber)).IsLoaded; }
		}

		public event AsyncPixelDataEventHandler AsyncLoaded
		{
			add { if (SupportsAsync) ((IAsyncSopFrameData) ParentImageSop.DataSource.GetFrameData(FrameNumber)).Loaded += value; }
			remove { if (SupportsAsync) ((IAsyncSopFrameData) ParentImageSop.DataSource.GetFrameData(FrameNumber)).Loaded -= value; }
		}
	}
}