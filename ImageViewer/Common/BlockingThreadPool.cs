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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	/// <remarks>
	/// DO NOT USE THIS CLASS, as it will be removed or refactored in the future.
	/// </remarks>
	public class BlockingThreadPool<T> : ThreadPoolBase
	{
		private readonly IBlockingEnumerator<T> _blockingEnumerator;
		private readonly ProcessItemDelegate<T> _processItem;

		public BlockingThreadPool(IBlockingEnumerator<T> blockingEnumerator, ProcessItemDelegate<T> processItem)
		{
			_blockingEnumerator = blockingEnumerator;
			_processItem = processItem;
		}

		protected override bool OnStop(bool completeBeforeStop)
		{
			if (!base.OnStop(completeBeforeStop))
				return false;

			_blockingEnumerator.IsBlocking = false;
			return true;
		}

		protected override bool OnStart()
		{
			if (!base.OnStart())
				return false;

			_blockingEnumerator.IsBlocking = true;
			return true;
		}

		protected override void RunThread()
		{
			while (base.State != StartStopState.Stopping)
			{
				foreach (T item in _blockingEnumerator)
				{
					try
					{
						_processItem(item);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}
				}
			}
		}
	}
}
