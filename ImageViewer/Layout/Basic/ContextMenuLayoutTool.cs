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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
    /// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
    /// it so that it reflects the state of the active workspace.
	/// </summary>
	[ActionPlaceholder(_placeHolderActionId, _contextMenuSite + "/DisplaySets", _groupHint)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class ContextMenuLayoutTool : ImageViewerTool
	{
		private const string _placeHolderActionId = "display0";
		private const string _groupHint = "DisplaySets";
    	private const string _contextMenuSite = "imageviewer-contextmenu";

		private static readonly List<IActionFactory> _actionFactories = CreateActionFactories();
		private static readonly DefaultContextMenuActionFactory _defaultActionFactory = new DefaultContextMenuActionFactory();

		private List<string> _currentPathElements;

		private ImageSetGroups _imageSetGroups;
		private readonly IComparer<IImageSet> _comparer = new StudyDateComparer();

		private readonly IPatientReconciliationStrategy _patientReconciliationStrategy = new DefaultPatientReconciliationStrategy();

		public ContextMenuLayoutTool()
		{
		}

		private static List<IActionFactory> CreateActionFactories()
		{
			List<IActionFactory> factories = new List<IActionFactory>();

			try
			{
				foreach (IActionFactory factory in new ActionFactoryExtensionPoint().CreateExtensions())
					factories.Add(factory);
			}
			catch (NotSupportedException)
			{
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Exception encountered while trying to create context menu action factories.");
			}

			return factories;
		}

		public override IActionSet Actions
		{
			get { return base.Actions.Union(GetDisplaySetActions()); }
		}
		
		/// <summary>
        /// Overridden to subscribe to workspace activation events
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

			_patientReconciliationStrategy.SetStudyTree(base.ImageViewer.StudyTree);
			_imageSetGroups = new ImageSetGroups(base.Context.Viewer.LogicalWorkspace.ImageSets);
		}

		protected override void Dispose(bool disposing)
		{
			_imageSetGroups.Dispose();
			base.Dispose(disposing);
		}

    	/// <summary>
		/// Gets an array of <see cref="IAction"/> objects that allow selection of specific display
		/// sets for display in the currently selected image box.
		/// </summary>
		/// <returns></returns>
		private IActionSet GetDisplaySetActions()
		{
#if TRACEGROUPS
			TraceGroups();
#endif
    		const string rootPath = _contextMenuSite;

			_currentPathElements = new List<string>();
			List<IAction> actions = new List<IAction>();

			FilteredGroup<IImageSet> rootGroup = GetRootGroup(_imageSetGroups.Root);
			if (rootGroup != null)
			{
				var actionPlaceholder = ActionPlaceholder.GetPlaceholderAction(_contextMenuSite, base.Actions, _placeHolderActionId);

				ActionFactoryContext context = new ActionFactoryContext
				{
					DesktopWindow = Context.DesktopWindow,
					ImageViewer = Context.Viewer,
					Namespace = GetType().FullName,
					ActionPlaceholder = actionPlaceholder
				};

			    bool showImageSetNames = base.ImageViewer.LogicalWorkspace.ImageSets.Count > 1;
				int loadingPriorsNumber = 0;

				foreach (FilteredGroup<IImageSet> group in TraverseImageSetGroups(rootGroup, rootPath))
				{
					string basePath = StringUtilities.Combine(_currentPathElements, "/");

					//not incredibly efficient, but there really aren't that many items.
					List<IImageSet> orderedItems = new List<IImageSet>(group.Items);
					orderedItems.Sort(_comparer);

					foreach (IImageSet imageSet in orderedItems)
					{
						string imageSetPath;
						if (showImageSetNames)
							imageSetPath = String.Format("{0}/{1}", basePath, imageSet.Name.Replace("/", "-"));
						else
							imageSetPath = basePath;

						context.Initialize(imageSet, imageSetPath);
						
						foreach (IActionFactory factory in _actionFactories)
							actions.AddRange(factory.CreateActions(context));

						if (actions.Count == 0 || !context.ExcludeDefaultActions)
							actions.AddRange(_defaultActionFactory.CreateActions(context));
					}

					if (group.Items.Count > 0 && base.ImageViewer.PriorStudyLoader.IsActive)
						actions.Add(CreateLoadingPriorsAction(actionPlaceholder, basePath, ++loadingPriorsNumber));
				}
			}

			return new ActionSet(actions);
		}

		private IEnumerable<FilteredGroup<IImageSet>> TraverseImageSetGroups(FilteredGroup<IImageSet> group, string rootPath)
		{
			List<IImageSet> allItems = group.GetAllItems();
			if (allItems.Count != 0)
			{
				if (_currentPathElements.Count == 0)
					_currentPathElements.Add(rootPath);
				else
					_currentPathElements.Add(group.Label.Replace("/", "-"));

				yield return group;
			}

			foreach (FilteredGroup<IImageSet> child in group.ChildGroups)
			{
				foreach (FilteredGroup<IImageSet> nonEmptyChild in TraverseImageSetGroups(child, rootPath))
					yield return nonEmptyChild;
			}

			if (allItems.Count != 0)
				_currentPathElements.RemoveAt(_currentPathElements.Count - 1);
		}

		private FilteredGroup<IImageSet> GetRootGroup(FilteredGroup<IImageSet> group)
		{
			if (group.HasItems)
				return group;

			int validChildGroups = 0;
    		foreach (FilteredGroup<IImageSet> child in group.ChildGroups)
    		{
    			if (child.GetAllItems().Count > 0)
    				++validChildGroups;
    		}

			//if this group has more than one child group with items anywhere in it's tree, then it's first.
			if (validChildGroups > 1)
				return group;

			foreach (FilteredGroup<IImageSet> child in group.ChildGroups)
			{
				FilteredGroup<IImageSet> rootGroup = GetRootGroup(child);
				if (rootGroup != null)
					return rootGroup;
			}

    		return null;
		}

		private static IClickAction CreateLoadingPriorsAction(ActionPlaceholder actionPlaceholder, string basePath, int number)
		{
			const string actionIdPrefix = "loadingPriors";
			
			Path pathSuffix = new Path(basePath);
			pathSuffix = pathSuffix.SubPath(1, pathSuffix.Segments.Count - 1);
			pathSuffix = pathSuffix.Append(new PathSegment(actionIdPrefix));

			string actionId = actionIdPrefix + number;
			var action = actionPlaceholder.CreateMenuAction(actionId, pathSuffix.ToString(), ClickActionFlags.None, null);
			action.Label = SR.LabelLoadingPriors;
			action.SetClickHandler(delegate { });
			return action;
		}

#if TRACEGROUPS

		private void TraceGroups()
		{
			TraceGroup(_imageSetGroups.Root, _imageSetGroups.Root.Name);
		}

		private void TraceGroup(FilteredGroup<IImageSet> group, string currentGroupPath)
		{
			foreach (IImageSet imageSet in group.Items)
			{
				string imageSetPath = String.Format("{0}/{1}", currentGroupPath, imageSet.Name);
				Trace.WriteLine(imageSetPath);
			}

			foreach (FilteredGroup<IImageSet> childGroup in group.ChildGroups)
			{
				string name = childGroup.Label;
				string groupPath = String.Format("{0}/{1}", currentGroupPath, name);
				TraceGroup(childGroup, groupPath);
			}
		}
#endif
	}
}
