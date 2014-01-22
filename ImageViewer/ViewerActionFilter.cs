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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	[ExtensionPoint]
	public class ViewerContextMenuFilterExtensionPoint : ExtensionPoint<IViewerActionFilter>
	{}

	[ExtensionPoint]
	public class ViewerToolbarFilterExtensionPoint : ExtensionPoint<IViewerActionFilter>
	{}

	//TODO (CR Mar 2010): Predicate!
	public interface IViewerActionFilter
	{
		void SetImageViewer(IImageViewer imageViewer);
        
		bool Evaluate(IAction action);
	}

	//TODO (CR Sept 2010): remove this stuff
	public abstract class ViewerActionFilter : IViewerActionFilter
	{
		private class AlwaysTrueFilter : IViewerActionFilter
		{
			#region IViewerActionFilter Members

			public void SetImageViewer(IImageViewer imageViewer)
			{
			}

			public bool Evaluate(IAction action)
			{
				return true;
			}

			#endregion
		}

		public static readonly IViewerActionFilter Null = new AlwaysTrueFilter();

		protected ViewerActionFilter()
		{}

		protected IImageViewer ImageViewer { get; private set; }

		#region IViewerActionFilter Members

		void IViewerActionFilter.SetImageViewer(IImageViewer imageViewer)
		{
			ImageViewer = imageViewer;
		}

		public abstract bool Evaluate(IAction action);

		#endregion

		public static IViewerActionFilter CreateContextMenuFilter()
		{
			try
			{
				return (IViewerActionFilter)new ViewerContextMenuFilterExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return Null;
			}
		}

		public static IViewerActionFilter CreateToolbarFilter()
		{
			try
			{
				return (IViewerActionFilter)new ViewerToolbarFilterExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return Null;
			}
		}
	}
}
