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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ICorePrefetchingStrategy
	{
		bool CanRetrieveFrame(Frame frame);

		void RetrieveFrame(Frame frame);

		bool CanDecompressFrame(Frame frame);

		void DecompressFrame(Frame frame);
	}

	public class SimpleCorePrefetchingStrategy : ICorePrefetchingStrategy
	{
		private readonly Predicate<Frame> _canRetrieve;

		public SimpleCorePrefetchingStrategy()
			: this(ignore => true)
		{
		}

		public SimpleCorePrefetchingStrategy(Predicate<Frame> canRetrieve)
		{
			Platform.CheckForNullReference(canRetrieve, "canRetrieve");
			_canRetrieve = canRetrieve;
		}

		public bool CanRetrieveFrame(Frame frame)
		{
			return _canRetrieve(frame);
		}

		public void RetrieveFrame(Frame frame)
		{
			frame.GetNormalizedPixelData();
		}

		public bool CanDecompressFrame(Frame frame)
		{
			return false;
		}

		public void DecompressFrame(Frame frame) { }
	}

}