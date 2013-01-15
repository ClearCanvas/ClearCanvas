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
using System.Collections.Specialized;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	/// <summary>
	/// Extension point for views onto <see cref="ImagePropertiesApplicationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ImagePropertiesApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ImagePropertiesApplicationComponentViewExtensionPoint))]
	public class ImagePropertiesApplicationComponent : ApplicationComponent
	{
		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _viewer;

		private readonly List<IImagePropertyProvider> _informationProviders;
		private List<IImageProperty> _imageProperties;

		public ImagePropertiesApplicationComponent(IDesktopWindow desktopWindow)
		{
			_informationProviders = new List<IImagePropertyProvider>();

			try
			{
				foreach(IImagePropertyProvider informationProvider in new ImagePropertyProviderExtensionPoint().CreateExtensions())
					_informationProviders.Add(informationProvider);
			}
			catch(NotSupportedException)
			{
			}

			_desktopWindow = desktopWindow;
			_imageProperties = new List<IImageProperty>();
		}

		public override void  Start()
		{
			SetImageViewer(_desktopWindow.ActiveWorkspace);
			_desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;

			base.Start();
		}

		public override void Stop()
		{
			base.Stop();

			_desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
			SetImageViewer(null);

		}

		/*
		private void InitializeCollapsedCategories()
		{
			StringCollection collapsedCategories = ImagePropertiesSettings.Default.CollapsedCategories ?? new StringCollection();
			_collapsedCategories = new string[collapsedCategories.Count];

			for (int i = 0; i < collapsedCategories.Count; ++i)
				_collapsedCategories[i] = collapsedCategories[i];
		}

		private void SaveSettings()
		{
			ImagePropertiesSettings.Default.CollapsedCategories = new StringCollection();
			foreach (string collapsedCategory in _collapsedCategories)
				ImagePropertiesSettings.Default.CollapsedCategories.AddRange(_collapsedCategories ?? new string[0]);

			ImagePropertiesSettings.Default.Save();
		}
		*/

		private void OnActiveWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			SetImageViewer(e.Item);
		}

		private void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			if (e.SelectedTile.PresentationImage == null)
				UpdateImageProperties(e.SelectedTile.PresentationImage);
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateImageProperties(e.SelectedPresentationImage);
		}

		private static IImageViewer CastToImageViewer(Workspace workspace)
		{
			IImageViewer viewer = null;
			if (workspace != null)
				viewer = ImageViewerComponent.GetAsImageViewer(workspace);

			return viewer;
		}

		private void SetImageViewer(Workspace workspace)
		{
			IImageViewer viewer = CastToImageViewer(workspace);
			if (viewer != _viewer)
			{
				if (_viewer != null)
				{
					_viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;
					_viewer.EventBroker.TileSelected -= OnTileSelected;
				}

				_viewer = viewer;
				if (_viewer != null)
				{
					_viewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
					_viewer.EventBroker.TileSelected += OnTileSelected;

					UpdateImageProperties(viewer.SelectedPresentationImage);
				}
				else
				{
					UpdateImageProperties(null);
				}
			}
		}

		private void UpdateImageProperties(IPresentationImage presentationImage)
		{
			if (presentationImage == null || _informationProviders.Count == 0)
			{
				ImageProperties = new List<IImageProperty>();
			}
			else
			{
				Dictionary<string, IImageProperty> properties = new Dictionary<string, IImageProperty>();
				foreach (IImagePropertyProvider info in _informationProviders)
				{
					foreach (IImageProperty property in info.GetProperties(presentationImage))
						if (!properties.ContainsKey(property.Identifier))
							properties[property.Identifier] = property;
						else
							Platform.Log(LogLevel.Debug, "Image property with identifier '{0}' already exists; ignoring.", property.Identifier);
				}

				if (ImagePropertiesSettings.Default.ShowEmptyValues)
				{
					ImageProperties = new List<IImageProperty>(properties.Values);
				}
				else
				{
					ImageProperties = CollectionUtils.Select(properties.Values,
						delegate(IImageProperty property) { return !property.IsEmpty; });
				}
			}
		}

		#region Presentation Model

		public List<IImageProperty> ImageProperties
		{
			get { return _imageProperties; }
			private set
			{
				if (_imageProperties != value)
				{
					_imageProperties = value;
					NotifyPropertyChanged("ImageProperties");
				}
			}
		}

		//Not used right now.
		//public string[] CollapsedCategories
		//{
		//    get
		//    {
		//        return _collapsedCategories;
		//    }
		//    set
		//    {
		//        _collapsedCategories = value;
		//    }
		//}

		#endregion
	}
}
