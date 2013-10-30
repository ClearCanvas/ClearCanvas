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
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer
{
	//TODO (CR Mar 2010): name, method names?
	//It's a factory, so Create methods?
	//IImageViewerSetupHelper?
	public interface IViewerSetupHelper
	{
		void SetImageViewer(IImageViewer viewer);

		ILayoutManager GetLayoutManager(); 
		
		ITool[] GetTools();
		//TODO (CR Sept 2010): remove this stuff
		IViewerActionFilter GetContextMenuFilter();
		IViewerActionFilter GetToolbarFilter();

		IPriorStudyFinder GetPriorStudyFinder();
	}

	public class ViewerSetupHelper : IViewerSetupHelper
	{
		public ViewerSetupHelper()
		{
		}

		internal ViewerSetupHelper(ILayoutManager layoutManager, IPriorStudyFinder priorStudyFinder)
		{
			LayoutManager = layoutManager;
			PriorStudyFinder = priorStudyFinder;
		}

		protected IImageViewer ImageViewer { get; private set; }
		
		public ILayoutManager LayoutManager { get; set; }
		public ITool[] Tools { get; set; }
		public IViewerActionFilter ContextMenuFilter { get; set; }
		public IViewerActionFilter ToolbarFilter { get; set; }
		public IPriorStudyFinder PriorStudyFinder { get; set; }

		protected virtual ITool[] GetTools()
		{
			if (Tools != null)
				return Tools;

			try
			{
				object[] extensions = new ImageViewerToolExtensionPoint().CreateExtensions();
				return CollectionUtils.Map(extensions, (object tool) => (ITool) tool).ToArray();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No viewer tool extensions found.");
				return new ITool[0];
			}
		}

		protected virtual IViewerActionFilter GetContextMenuFilter()
		{
			return ContextMenuFilter ?? ViewerActionFilter.CreateContextMenuFilter();
		}

		protected virtual IViewerActionFilter GetToolbarFilter()
		{
			return ToolbarFilter ?? ViewerActionFilter.CreateToolbarFilter();
		}

		protected virtual ILayoutManager GetLayoutManager()
		{
			return LayoutManager ?? ClearCanvas.ImageViewer.LayoutManager.Create();
		}

		protected virtual IPriorStudyFinder GetPriorStudyFinder()
		{
			return PriorStudyFinder ?? ClearCanvas.ImageViewer.PriorStudyFinder.Create();
		}

		#region IViewerSetupHelper Members

		void IViewerSetupHelper.SetImageViewer(IImageViewer viewer)
		{
			ImageViewer = viewer;
		}

		ITool[] IViewerSetupHelper.GetTools()
		{
			return GetTools();
		}

		IViewerActionFilter IViewerSetupHelper.GetContextMenuFilter()
		{
			return GetContextMenuFilter();
		}

		IViewerActionFilter IViewerSetupHelper.GetToolbarFilter()
		{
			return GetToolbarFilter();
		}

		ILayoutManager IViewerSetupHelper.GetLayoutManager()
		{
			return GetLayoutManager();
		}

		IPriorStudyFinder IViewerSetupHelper.GetPriorStudyFinder()
		{
			return GetPriorStudyFinder();
		}

		#endregion
	}
}