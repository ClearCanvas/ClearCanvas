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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layout.Basic.OverlayManagers;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public interface IOverlayManager
	{
		/// <summary>
		/// Gets a unique name for the manager.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a name for display to the user.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Gets whether or not this overlay is "important", meaning the user must see it initially, regardless of selection.
		/// </summary>
		/// <remarks>
		/// If true, it is expected that <see cref="IsSelectedByDefault"/> will always return true.
		/// </remarks>
		bool IsImportant { get; }

		/// <summary>
		/// Gets an <see cref="IconSet"/> for display to the user.
		/// </summary>
		IconSet IconSet { get; }

		/// <summary>
		/// An <see cref="IResourceResolver"/> for resolving the <see cref="IconSet"/> and <see cref="DisplayName"/> resources.
		/// </summary>
		IResourceResolver ResourceResolver { get; }

		/// <summary>
		/// Gets whether or not the overlay is to be "selected" by default for the given modality.
		/// </summary>
		/// <remarks>If <see cref="IsImportant"/> is true, then this must return true for all modalities.</remarks>
		bool IsSelectedByDefault(string modality);

		/// <summary>
		/// Shows the overlay on the given image.
		/// </summary>
		void ShowOverlay(IPresentationImage image);

		/// <summary>
		/// Hides the overlay on the given image.
		/// </summary>
		void HideOverlay(IPresentationImage image);
	}

	public abstract class OverlayManager : IOverlayManager
	{
		private IResourceResolver _resolver;

		protected OverlayManager(string name, string displayName)
		{
			Name = name;
			DisplayName = displayName;
			IsImportant = false;
		}

		#region Implementation of IOverlaySelection

		public string Name { get; private set; }
		public string DisplayName { get; private set; }

		public bool IsImportant { get; protected set; }

		public IconSet IconSet { get; protected set; }

		public IResourceResolver ResourceResolver
		{
			get { return _resolver ?? (_resolver = new ApplicationThemeResourceResolver(this.GetType().Assembly)); }
			protected set { _resolver = value; }
		}

		public abstract bool IsSelectedByDefault(string modality);

		public void ShowOverlay(IPresentationImage image)
		{
			SetOverlayVisible(image, true);
		}

		public void HideOverlay(IPresentationImage image)
		{
			SetOverlayVisible(image, false);
		}

		#endregion

		public abstract void SetOverlayVisible(IPresentationImage image, bool visible);

		internal static List<IOverlayManager> CreateAll()
		{
			return new List<IOverlayManager>
			       	{
			       		new TextOverlayManager(),
			       		new ScaleOverlayManager(),
			       		new DicomOverlayManager(),
			       		new ShutterOverlayManager(),
			       		new ColorBarManager()
			       	};
		}
	}
}