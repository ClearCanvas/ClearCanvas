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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public abstract class ExternalBase : External, IPresentationImageExternal, IDisplaySetExternal
	{
		protected abstract bool CanLaunch(IArgumentHintResolver hintResolver);
		protected abstract bool PerformLaunch(IArgumentHintResolver hintResolver);

		protected virtual IArgumentHintResolver BuildArgumentHints(IPresentationImage image)
		{
			List<IArgumentHint> hints = new List<IArgumentHint>();
			if (image is IImageSopProvider)
			{
				IImageSopProvider imageSopProvider = (IImageSopProvider) image;
				hints.Add(new SopArgumentHint(imageSopProvider.Sop));

				if (imageSopProvider.Sop.DataSource is ILocalSopDataSource)
				{
					ILocalSopDataSource localSopDataSource = (ILocalSopDataSource) imageSopProvider.Sop.DataSource;
					hints.Add(new FileArgumentHint(localSopDataSource.Filename));
				}
			}
			return new ArgumentHintResolver(hints);
		}

		protected virtual IArgumentHintResolver BuildArgumentHints(IEnumerable<IPresentationImage> images)
		{
			List<IArgumentHint> hints = new List<IArgumentHint>();
			if (images != null)
			{
				List<string> filenames = new List<string>();
				foreach (IPresentationImage image in images)
				{
					if (image is IImageSopProvider)
					{
						IImageSopProvider imageSopProvider = (IImageSopProvider) image;
						hints.Add(new SopArgumentHint(imageSopProvider.Sop));

						if (imageSopProvider.Sop.DataSource is ILocalSopDataSource)
						{
							ILocalSopDataSource localSopDataSource = (ILocalSopDataSource) imageSopProvider.Sop.DataSource;
							filenames.Add(localSopDataSource.Filename);
						}
					}
				}
				hints.Add(new FileSetArgumentHint(filenames));
			}
			return new ArgumentHintResolver(hints);
		}

		protected virtual IArgumentHintResolver BuildArgumentHints(IDisplaySet displaySet)
		{
			if (displaySet == null)
				return new ArgumentHintResolver();
			return BuildArgumentHints(displaySet.PresentationImages);
		}

		protected virtual bool HandleLaunchException(Exception ex)
		{
			return false;
		}

		private bool TryCanLaunch(IArgumentHintResolver hintResolver)
		{
			try
			{
				return this.CanLaunch(hintResolver);
			}
			finally
			{
				hintResolver.Dispose();
			}
		}

		private bool TryPerformLaunch(IArgumentHintResolver hintResolver)
		{
			try
			{
				return this.PerformLaunch(hintResolver);
			}
			catch (Exception ex)
			{
				// if the sub class doesn't handle the exception, throw it
				if (!HandleLaunchException(ex))
					throw;
				return false;
			}
			finally
			{
				hintResolver.Dispose();
			}
		}

		#region IPresentationImageExternal Members

		public bool CanLaunch(IPresentationImage image)
		{
			return TryCanLaunch(BuildArgumentHints(image));
		}

		public bool Launch(IPresentationImage image)
		{
			return TryPerformLaunch(BuildArgumentHints(image));
		}

		#endregion

		#region IDisplaySetExternal Members

		public bool CanLaunch(IEnumerable<IPresentationImage> images)
		{
			return TryCanLaunch(BuildArgumentHints(images));
		}

		public bool Launch(IEnumerable<IPresentationImage> images)
		{
			return TryPerformLaunch(BuildArgumentHints(images));
		}

		public bool CanLaunch(IDisplaySet displaySet)
		{
			return TryCanLaunch(BuildArgumentHints(displaySet));
		}

		public bool Launch(IDisplaySet displaySet)
		{
			return TryPerformLaunch(BuildArgumentHints(displaySet));
		}

		#endregion
	}
}