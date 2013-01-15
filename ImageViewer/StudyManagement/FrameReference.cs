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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Interface defining a transient reference to a <see cref="Frame"/>.
	/// </summary>
	/// <remarks>
	/// See <see cref="ISopReference"/> for a detailed explanation of 'transient references'.
	/// </remarks>
	public interface IFrameReference : IImageSopProvider, IDisposable
	{
		/// <summary>
		/// Clones an existing <see cref="IFrameReference"/>, creating a new transient reference.
		/// </summary>
		IFrameReference Clone();
	}

	public partial class Frame
	{
		private class FrameReference : IFrameReference
		{
			private readonly Frame _frame;
			private ISopReference _sopReference;

			public FrameReference(Frame frame)
			{
				_frame = frame;
				_sopReference = _frame.ParentImageSop.CreateTransientReference();
			}

			#region IFrameReference Members

			public IFrameReference Clone()
			{
				return new FrameReference(_frame);
			}

			#endregion

			#region IImageSopProvider Members

			public ImageSop ImageSop
			{
				get { return _frame.ParentImageSop; }
			}

			public Frame Frame
			{
				get { return _frame; }
			}

			#endregion

			#region ISopProvider Members
			
			Sop ISopProvider.Sop
			{
				get { return _frame.ParentImageSop; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_sopReference != null)
				{
					_sopReference.Dispose();
					_sopReference = null;
				}
			}

			#endregion
		}
	}
}
