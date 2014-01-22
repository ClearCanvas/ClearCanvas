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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common
{
	[SettingsGroupDescription("Application settings for image processing routines on the local machine.")]
	[SettingsProvider(typeof (ExtendedLocalFileSettingsProvider))]
	internal sealed partial class ImageProcessingSettings
	{
		private ImageProcessingSettings() {}
	}

	/// <summary>
	/// Helper class for image processing routines.
	/// </summary>
	public static class ImageProcessingHelper
	{
		private static int _maxParallelThreads = -1;

		/// <summary>
		/// Gets the maximum number of parallel threads to use when performing image processing routines.
		/// </summary>
		/// <remarks>
		/// The value is determined by a combination of the setting and the actual number of logical processors available on the system.
		/// If the setting is set, the value will be either the setting or the number of logical processors, whichever is less.
		/// If the setting is not set, the value will be all but one of the logical processors when 3 or more processors are available;
		/// otherwise, the value will simply be the actual number of logical processors.
		/// </remarks>
		public static int MaxParallelThreads
		{
			get
			{
				if (_maxParallelThreads < 0)
				{
					try
					{
						var setting = ImageProcessingSettings.Default.MaxParallelThreads;
						var cpuCount = Environment.ProcessorCount;
						_maxParallelThreads = setting > 0 ? Math.Min(cpuCount, setting) : (cpuCount > 2 ? cpuCount - 1 : 1);
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Debug, ex);
					}
					if (_maxParallelThreads < 0) _maxParallelThreads = 1;
				}
				return _maxParallelThreads;
			}

#if UNIT_TESTS
			set { _maxParallelThreads = value; }
#endif
		}

		/// <summary>
		/// Parallelizes a query and also sets the degree of parallelism to the configured <see cref="MaxParallelThreads"/>.
		/// </summary>
		public static ParallelQuery<T> AsParallel2<T>(this IEnumerable<T> source)
		{
			return source.AsParallel().WithDegreeOfParallelism(MaxParallelThreads);
		}
	}
}