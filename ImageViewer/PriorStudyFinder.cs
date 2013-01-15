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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an extension point for image layout management.
	/// </summary>
	[ExtensionPoint()]
	public sealed class PriorStudyFinderExtensionPoint : ExtensionPoint<IPriorStudyFinder>
	{
	}

	/// <summary>
	/// Abstract base class for an <see cref="IPriorStudyFinder"/>.
	/// </summary>
	public abstract class PriorStudyFinder : IPriorStudyFinder
	{
		private class NullPriorStudyFinder : IPriorStudyFinder
		{
			public NullPriorStudyFinder()
			{
			}

			public PriorStudyFinderResult FindPriorStudies()
			{
                return new PriorStudyFinderResult(new StudyItemList(), true);
			}

			#region IPriorStudyFinder Members

			public void SetImageViewer(IImageViewer viewer)
			{
			}

			public void Cancel()
			{
			}

			#endregion
		}

		/// <summary>
		/// Convenient static property for an <see cref="IPriorStudyFinder"/> that does nothing.
		/// </summary>
		public static readonly IPriorStudyFinder Null = new NullPriorStudyFinder();

		private IImageViewer _viewer;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected PriorStudyFinder()
		{
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		protected IImageViewer Viewer
		{
			get { return _viewer; }	
		}

		#region IPriorStudyFinder Members

		/// <summary>
		/// Sets the <see cref="IImageViewer"/> for which prior studies are to found (and added/loaded).
		/// </summary>
		public void SetImageViewer(IImageViewer viewer)
		{
			_viewer = viewer;
		}

		/// <summary>
		/// Gets the list of prior studies.
		/// </summary>
        public abstract PriorStudyFinderResult FindPriorStudies();

		/// <summary>
		/// Cancels the search for prior studies.
		/// </summary>
		public abstract void Cancel();

		#endregion

		public static IPriorStudyFinder Create()
		{
			try
			{
				return (IPriorStudyFinder)new PriorStudyFinderExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}

			return Null;
		}
	}
}
