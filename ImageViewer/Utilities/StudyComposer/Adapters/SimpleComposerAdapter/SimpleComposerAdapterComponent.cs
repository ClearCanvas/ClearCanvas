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
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	[ExtensionPoint]
	public class SimpleComposerAdapterComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (SimpleComposerAdapterComponentViewExtensionPoint))]
	public class SimpleComposerAdapterComponent : ApplicationComponentContainer
	{
		private readonly StudyComposerComponent _composer;
		private readonly LocalComponentHost _composerHost;

		public SimpleComposerAdapterComponent(StudyComposerComponent composer)
		{
			_composer = composer;
			_composerHost = new LocalComponentHost(this, _composer);

			// initialize data sources
			_listPatients = new PatientCollection(composer.Patients);
			_selectedPatient = _listPatients.GetFirstPatient();

			if (_selectedPatient != null)
				_listStudies = new StudyCollection(_selectedPatient.Studies);
			else
				_listStudies = new StudyCollection(null);
			_selectedStudy = _listStudies.GetFirstStudy();

			if (_selectedStudy != null)
				_listSeries = new SeriesCollection(_selectedStudy.Series);
			else
				_listSeries = new SeriesCollection(null);
			_selectedSeries = _listSeries.GetFirstSeries();

			if (_selectedSeries != null)
				_listImages = new ImageCollection(_selectedSeries.Images);
			else
				_listImages = new ImageCollection(null);
			_selectedImage = _listImages.GetFirstImage();

			// create gallery components and hosts
			_gvPatients = new PatientGalleryComponent(_listPatients);
			_gvPatients.SelectionChanged += new EventHandler(PatientGallery_SelectionChanged);
			_gvPatientsHost = new LocalComponentHost(this, _gvPatients);

			_gvStudies = new StudyGalleryComponent(_listStudies);
			_gvStudies.SelectionChanged += new EventHandler(StudyGallery_SelectionChanged);
			_gvStudiesHost = new LocalComponentHost(this, _gvStudies);

			_gvSeries = new SeriesGalleryComponent(_listSeries);
			_gvSeries.SelectionChanged += new EventHandler(SeriesGallery_SelectionChanged);
			_gvSeriesHost = new LocalComponentHost(this, _gvSeries);

			_gvImages = new ImageGalleryComponent(_listImages);
			_gvImages.SelectionChanged += new EventHandler(ImageGallery_SelectionChanged);
			_gvImagesHost = new LocalComponentHost(this, _gvImages);
		}

		#region AppCompContainer overrides

		/// <summary>
		/// Starts this component and the hosted components.
		/// </summary>
		///  <remarks>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			_gvPatientsHost.StartComponent();
			_gvStudiesHost.StartComponent();
			_gvSeriesHost.StartComponent();
			_gvImagesHost.StartComponent();
			_composerHost.StartComponent();
		}

		/// <summary>
		/// Starts this component and the hosted components.
		/// </summary>
		/// <remarks>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Stop()
		{
			_gvPatientsHost.StopComponent();
			_gvStudiesHost.StopComponent();
			_gvSeriesHost.StopComponent();
			_gvImagesHost.StopComponent();
			_composerHost.StopComponent();

			base.Stop();
		}

		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get
			{
				List<IApplicationComponent> list = new List<IApplicationComponent>();
				list.Add(_gvPatients);
				list.Add(_gvStudies);
				list.Add(_gvSeries);
				list.Add(_gvImages);
				list.Add(_composer);
				return list.AsReadOnly();
			}
		}

		public override IEnumerable<IApplicationComponent> VisibleComponents
		{
			get { return this.ContainedComponents; }
		}

		public override void EnsureVisible(IApplicationComponent component)
		{
			if (!this.IsStarted)
				throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);
		}

		public override void EnsureStarted(IApplicationComponent component)
		{
			EnsureVisible(component);
		}

		#endregion

		#region StudyBuilder Methods

		public void RefreshIcons(Size iconSize)
		{
			_composer.RefreshAllIcons(iconSize);

			// the next 3 lines have the effect of triggering a reset event on the binding list, thereby forcing all icons to reload
			BindingList<PatientItem> list = _listPatients.InternalList;
			_listPatients.InternalList = null;
			_listPatients.InternalList = list;
		}

		[Obsolete]
		public void Export(string path)
		{
			_composer.Export(path);
		}

		public void PublishToDirectory()
		{
			_composer.PublishToDirectory();
		}

		public void PublishToLocalDataStore()
		{
			_composer.PublishToLocalDataStore();
		}

		public void PublishToServer()
		{
			_composer.PublishToServer();
		}

		#endregion

		#region Data Sources

		private readonly PatientCollection _listPatients;
		private readonly StudyCollection _listStudies;
		private readonly SeriesCollection _listSeries;
		private readonly ImageCollection _listImages;

		protected PatientCollection Patients
		{
			get { return _listPatients; }
		}

		protected StudyCollection Studies
		{
			get { return _listStudies; }
		}

		protected SeriesCollection Series
		{
			get { return _listSeries; }
		}

		protected ImageCollection Images
		{
			get { return _listImages; }
		}

		#endregion

		#region Selected Items - Properties

		private PatientItem _selectedPatient = null;
		private StudyItem _selectedStudy = null;
		private SeriesItem _selectedSeries = null;
		private ImageItem _selectedImage = null;

		protected PatientItem SelectedPatient
		{
			get { return _selectedPatient; }
			set
			{
				if (_selectedPatient != value)
				{
					if (value == null || _composer.Patients.Contains(value))
					{
						_selectedPatient = value;
						FireSelectedPatientChanged();
					}
				}
			}
		}

		protected StudyItem SelectedStudy
		{
			get { return _selectedStudy; }
			set
			{
				if (_selectedStudy != value)
				{
					if (value == null || (this.SelectedPatient != null && this.SelectedPatient.Studies.Contains(value)))
					{
						_selectedStudy = value;
						FireSelectedStudyChanged();
					}
				}
			}
		}

		protected SeriesItem SelectedSeries
		{
			get { return _selectedSeries; }
			set
			{
				if (_selectedSeries != value)
				{
					if (value == null || (this.SelectedStudy != null && this.SelectedStudy.Series.Contains(value)))
					{
						_selectedSeries = value;
						FireSelectedSeriesChanged();
					}
				}
			}
		}

		protected ImageItem SelectedImage
		{
			get { return _selectedImage; }
			set
			{
				if (_selectedImage != value)
				{
					if (value == null || (this.SelectedSeries != null && this.SelectedSeries.Images.Contains(value)))
					{
						_selectedImage = value;
						FireSelectedImageChanged();
					}
				}
			}
		}

		#endregion

		#region Selected Items - Changed Events

		private event EventHandler _selectedPatientChanged;
		private event EventHandler _selectedStudyChanged;
		private event EventHandler _selectedSeriesChanged;
		private event EventHandler _selectedImageChanged;

		public event EventHandler SelectedPatientChanged
		{
			add { _selectedPatientChanged += value; }
			remove { _selectedPatientChanged -= value; }
		}

		public event EventHandler SelectedStudyChanged
		{
			add { _selectedStudyChanged += value; }
			remove { _selectedStudyChanged -= value; }
		}

		public event EventHandler SelectedSeriesChanged
		{
			add { _selectedSeriesChanged += value; }
			remove { _selectedSeriesChanged -= value; }
		}

		public event EventHandler SelectedImageChanged
		{
			add { _selectedImageChanged += value; }
			remove { _selectedImageChanged -= value; }
		}

		protected virtual void FireSelectedPatientChanged()
		{
			if (this.SelectedPatient != null)
				this.Studies.InternalList = this.SelectedPatient.Studies;
			else
				this.Studies.InternalList = null;

			if (_selectedPatientChanged != null)
				_selectedPatientChanged(this, new EventArgs());

			this.SelectedStudy = this.Studies.GetFirstStudy();
		}

		protected virtual void FireSelectedStudyChanged()
		{
			if (this.SelectedStudy != null)
				this.Series.InternalList = this.SelectedStudy.Series;
			else
				this.Series.InternalList = null;

			if (_selectedStudyChanged != null)
				_selectedStudyChanged(this, new EventArgs());

			this.SelectedSeries = this.Series.GetFirstSeries();
		}

		protected virtual void FireSelectedSeriesChanged()
		{
			if (this.SelectedSeries != null)
				this.Images.InternalList = this.SelectedSeries.Images;
			else
				this.Images.InternalList = null;

			if (_selectedSeriesChanged != null)
				_selectedSeriesChanged(this, new EventArgs());

			this.SelectedImage = this.Images.GetFirstImage();
		}

		protected virtual void FireSelectedImageChanged()
		{
			if (_selectedImageChanged != null)
				_selectedImageChanged(this, new EventArgs());
		}

		#endregion

		#region Gallery Components

		private readonly PatientGalleryComponent _gvPatients;
		private readonly StudyGalleryComponent _gvStudies;
		private readonly SeriesGalleryComponent _gvSeries;
		private readonly ImageGalleryComponent _gvImages;

		private void ImageGallery_SelectionChanged(object sender, EventArgs e)
		{
			ImageItem item = (ImageItem) _gvImages.Selection.Item;
			this.SelectedImage = item;
		}

		private void SeriesGallery_SelectionChanged(object sender, EventArgs e)
		{
			SeriesItem item = (SeriesItem) _gvSeries.Selection.Item;
			this.SelectedSeries = item;
		}

		private void StudyGallery_SelectionChanged(object sender, EventArgs e)
		{
			StudyItem item = (StudyItem) _gvStudies.Selection.Item;
			this.SelectedStudy = item;
		}

		private void PatientGallery_SelectionChanged(object sender, EventArgs e)
		{
			PatientItem item = (PatientItem) _gvPatients.Selection.Item;
			this.SelectedPatient = item;
		}

		#region Patient Gallery Class

		private class PatientGalleryComponent : SimpleComposerAdapterGalleryComponent
		{
			public PatientGalleryComponent(PatientCollection patients) : base(patients, "patients") {}

			protected override DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
			{
				PatientItem patient = (PatientItem) targetItem;
				bool allowed = true;
				foreach (IGalleryItem droppingItem in droppingItems)
				{
					if (droppingItem is StudyItem)
					{
						StudyItem study = (StudyItem) droppingItem;
						if (modifiers == ModifierFlags.None)
							allowed &= !(patient.Studies.Contains(study)); // disallow moves where the item is already in the target tree
						else if (modifiers == ModifierFlags.Shift)
							allowed &= true;
					}
					else if (droppingItem is SeriesItem)
					{
						SeriesItem series = (SeriesItem) droppingItem;
						if (modifiers == ModifierFlags.None)
							allowed &= (patient.Node != GetNodeAncestor(series.Node, 2)); // disallow moves where the item is already in the target tree
						else if (modifiers == ModifierFlags.Shift)
							allowed &= true;
					}
					else if (droppingItem is ImageItem)
					{
						ImageItem image = (ImageItem) droppingItem;
						if (modifiers == ModifierFlags.None)
							allowed &= (patient.Node != GetNodeAncestor(image.Node, 3)); // disallow moves where the item is already in the target tree
						else if (modifiers == ModifierFlags.Shift)
							allowed &= true;
					}
					else
					{
						allowed &= false;
					}
				}

				if (allowed)
				{
					if (modifiers == ModifierFlags.None)
						return DragDropOption.Move;
					else if (modifiers == ModifierFlags.Shift)
						return DragDropOption.Copy;
				}
				return DragDropOption.None;
			}

			protected override DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
			{
				PatientItem patient = (PatientItem) targetItem;
				DragDropOption action = DragDropOption.None;
				foreach (IGalleryItem droppingItem in droppingItems)
				{
					if (droppingItem is StudyItem)
					{
						StudyItem study = (StudyItem) droppingItem;
						if (modifiers == ModifierFlags.None)
						{
							if (!patient.Studies.Contains(study))
							{
								patient.Studies.Add(study.Copy());
								action = DragDropOption.Move;
							}
						}
						else if (modifiers == ModifierFlags.Shift)
						{
							patient.Studies.Add(study.Copy());
							action = DragDropOption.Copy;
						}
					}
					else if (droppingItem is SeriesItem)
					{
						SeriesItem series = (SeriesItem) droppingItem;
						if (modifiers == ModifierFlags.None)
						{
							StudyNode studyNode = (StudyNode) GetNodeAncestor(series.Node, 1);
							PatientNode patientNode = (PatientNode) GetNodeAncestor(studyNode, 1);
							if (patient.Node != patientNode)
							{
								StudyItem study = new StudyItem(studyNode.Copy(false));
								study.Series.Add(series.Copy());
								study.UpdateIcon();
								patient.Studies.Add(study);
								action = DragDropOption.Move;
							}
						}
						else if (modifiers == ModifierFlags.Shift)
						{
							StudyNode studyNode = (StudyNode) GetNodeAncestor(series.Node, 1);
							StudyItem study = new StudyItem(studyNode.Copy(false));
							study.Series.Add(series.Copy());
							study.UpdateIcon();
							patient.Studies.Add(study);
							action = DragDropOption.Copy;
						}
					}
					else if (droppingItem is ImageItem)
					{
						ImageItem image = (ImageItem) droppingItem;
						if (modifiers == ModifierFlags.None)
						{
							SeriesNode seriesNode = (SeriesNode) GetNodeAncestor(image.Node, 1);
							StudyNode studyNode = (StudyNode) GetNodeAncestor(seriesNode, 1);
							PatientNode patientNode = (PatientNode) GetNodeAncestor(studyNode, 1);
							if (patient.Node != patientNode)
							{
								SeriesItem series = new SeriesItem(seriesNode.Copy(false));
								StudyItem study = new StudyItem(studyNode.Copy(false));
								series.Images.Add(image.Copy());
								series.UpdateIcon();
								study.Series.Add(series);
								study.UpdateIcon();
								patient.Studies.Add(study);
								action = DragDropOption.Move;
							}
						}
						else if (modifiers == ModifierFlags.Shift)
						{
							SeriesNode seriesNode = (SeriesNode) GetNodeAncestor(image.Node, 1);
							StudyNode studyNode = (StudyNode) GetNodeAncestor(seriesNode, 1);
							SeriesItem series = new SeriesItem(seriesNode.Copy(false));
							StudyItem study = new StudyItem(studyNode.Copy(false));
							series.Images.Add(image.Copy());
							series.UpdateIcon();
							study.Series.Add(series);
							study.UpdateIcon();
							patient.Studies.Add(study);
							action = DragDropOption.Copy;
						}
					}
				}
				return action;
			}
		}

		#endregion

		#region Study Gallery Class

		private class StudyGalleryComponent : SimpleComposerAdapterGalleryComponent
		{
			public StudyGalleryComponent(StudyCollection studies) : base(studies, "studies") {}

			protected override DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
			{
				StudyItem study = (StudyItem) targetItem;
				bool allowed = true;
				foreach (IGalleryItem droppingItem in droppingItems)
				{
					if (droppingItem is SeriesItem)
					{
						SeriesItem series = (SeriesItem) droppingItem;
						if (modifiers == ModifierFlags.None)
							allowed &= !(study.Series.Contains(series)); // disallow moves where the item is already in the target tree
						else if (modifiers == ModifierFlags.Shift)
							allowed &= true;
					}
					else if (droppingItem is ImageItem)
					{
						ImageItem image = (ImageItem) droppingItem;
						if (modifiers == ModifierFlags.None)
							allowed &= (study.Node != GetNodeAncestor(image.Node, 2)); // disallow moves where the item is already in the target tree
						else if (modifiers == ModifierFlags.Shift)
							allowed &= true;
					}
					else
					{
						allowed &= false;
					}
				}

				if (allowed)
				{
					if (modifiers == ModifierFlags.None)
						return DragDropOption.Move;
					else if (modifiers == ModifierFlags.Shift)
						return DragDropOption.Copy;
				}
				return DragDropOption.None;
			}

			protected override DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
			{
				StudyItem study = (StudyItem) targetItem;
				DragDropOption action = DragDropOption.None;
				foreach (IGalleryItem droppingItem in droppingItems)
				{
					if (droppingItem is SeriesItem)
					{
						SeriesItem series = (SeriesItem) droppingItem;
						if (modifiers == ModifierFlags.None)
						{
							if (!study.Series.Contains(series))
							{
								study.Series.Add(series.Copy());
								action = DragDropOption.Move;
							}
						}
						else if (modifiers == ModifierFlags.Shift)
						{
							study.Series.Add(series.Copy());
							action = DragDropOption.Copy;
						}
					}
					else if (droppingItem is ImageItem)
					{
						ImageItem image = (ImageItem) droppingItem;
						if (modifiers == ModifierFlags.None)
						{
							SeriesNode seriesNode = (SeriesNode) GetNodeAncestor(image.Node, 1);
							StudyNode studyNode = (StudyNode) GetNodeAncestor(seriesNode, 1);
							if (study.Node != studyNode)
							{
								SeriesItem series = new SeriesItem(seriesNode.Copy(false));
								series.Images.Add(image.Copy());
								series.UpdateIcon();
								study.Series.Add(series);
								action = DragDropOption.Move;
							}
						}
						else if (modifiers == ModifierFlags.Shift)
						{
							SeriesNode seriesNode = (SeriesNode) GetNodeAncestor(image.Node, 1);
							SeriesItem series = new SeriesItem(seriesNode.Copy(false));
							series.Images.Add(image.Copy());
							series.UpdateIcon();
							study.Series.Add(series);
							action = DragDropOption.Copy;
						}
					}
				}
				return action;
			}
		}

		#endregion

		#region Series Gallery Class

		private class SeriesGalleryComponent : SimpleComposerAdapterGalleryComponent
		{
			public SeriesGalleryComponent(SeriesCollection series) : base(series, "series") {}

			protected override DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
			{
				SeriesItem series = (SeriesItem) targetItem;
				bool allowed = true;
				foreach (IGalleryItem droppingItem in droppingItems)
				{
					if (droppingItem is ImageItem)
					{
						ImageItem image = (ImageItem) droppingItem;
						if (modifiers == ModifierFlags.None)
							allowed &= !(series.Images.Contains(image)); // disallow moves where the item is already in the target tree
						else if (modifiers == ModifierFlags.Shift)
							allowed &= true;
					}
					else
					{
						allowed &= false;
					}
				}

				if (allowed)
				{
					if (modifiers == ModifierFlags.None)
						return DragDropOption.Move;
					else if (modifiers == ModifierFlags.Shift)
						return DragDropOption.Copy;
				}
				return DragDropOption.None;
			}

			protected override DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
			{
				SeriesItem series = (SeriesItem) targetItem;
				DragDropOption action = DragDropOption.None;
				foreach (IGalleryItem droppingItem in droppingItems)
				{
					if (droppingItem is ImageItem)
					{
						ImageItem image = (ImageItem) droppingItem;
						if (modifiers == ModifierFlags.None)
						{
							if (!series.Images.Contains(image))
							{
								series.Images.Add(image.Copy());
								action = DragDropOption.Move;
							}
						}
						else if (modifiers == ModifierFlags.Shift)
						{
							series.Images.Add(image.Copy());
							action = DragDropOption.Copy;
						}
					}
				}
				return action;
			}
		}

		#endregion

		#region Image Gallery Class

		private class ImageGalleryComponent : SimpleComposerAdapterGalleryComponent
		{
			public ImageGalleryComponent(ImageCollection images) : base(images, "images") {}
		}

		#endregion

		#endregion

		#region Gallery Component Hosts

		private readonly LocalComponentHost _gvPatientsHost;
		private readonly LocalComponentHost _gvStudiesHost;
		private readonly LocalComponentHost _gvSeriesHost;
		private readonly LocalComponentHost _gvImagesHost;

		public ApplicationComponentHost PatientGalleryComponentHost
		{
			get { return _gvPatientsHost; }
		}

		public ApplicationComponentHost StudyGalleryComponentHost
		{
			get { return _gvStudiesHost; }
		}

		public ApplicationComponentHost SeriesGalleryComponentHost
		{
			get { return _gvSeriesHost; }
		}

		public ApplicationComponentHost ImageGalleryComponentHost
		{
			get { return _gvImagesHost; }
		}

		#endregion

		#region Host Class

		private class LocalComponentHost : ApplicationComponentHost
		{
			private readonly SimpleComposerAdapterComponent _owner;

			internal LocalComponentHost(
				SimpleComposerAdapterComponent owner,
				IApplicationComponent component)
				: base(component)
			{
				Platform.CheckForNullReference(owner, "owner");
				_owner = owner;
			}

			#region ApplicationComponentHost overrides

			/// <summary>
			/// Gets the associated desktop window.
			/// </summary>
			public override DesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}

			/// <summary>
			/// Gets or sets the title displayed in the user-interface.
			/// </summary>
			/// <exception cref="NotSupportedException">The host does not support titles.</exception>
			public override string Title
			{
				get { return _owner.Host.Title; }
				set { _owner.Host.Title = value; }
			}

			#endregion
		}

		#endregion

		#region GalleryComponent Base Class

		private abstract class SimpleComposerAdapterGalleryComponent : ReorderableGalleryComponent
		{
			private static readonly Size _iconSize = new Size(64, 64);

			public SimpleComposerAdapterGalleryComponent(IBindingList dataSource, string subActionSite) : base(dataSource, "studyComposer-toolbar", "studyComposer-context") {}

			public override sealed bool AllowsDropOnItem
			{
				get { return true; }
			}

			public override sealed bool AllowsDropAtIndex
			{
				get { return true; }
			}

			public override sealed bool AllowRenaming
			{
				get { return true; }
				set { throw new NotSupportedException(); }
			}

			public override sealed bool ShowDescription
			{
				get { return true; }
				set { throw new NotSupportedException(); }
			}

			public override sealed bool HideSelection
			{
				get { return false; }
				set { throw new NotSupportedException(); }
			}

			public override sealed Size ImageSize
			{
				get { return _iconSize; }
				set { throw new NotSupportedException(); }
			}

			public override sealed int MaxDescriptionLines
			{
				get { return 3; }
				set { throw new NotSupportedException(); }
			}

			public override sealed bool MultiSelect
			{
				get { return true; }
				set { throw new NotSupportedException(); }
			}

			protected static StudyBuilderNode GetNodeAncestor(StudyBuilderNode node, int parentLevel)
			{
				StudyBuilderNode result = null;
				int level = 0;
				while (node != null && level < parentLevel)
				{
					node = node.Parent;
					level++;
				}
				if (level == parentLevel)
					result = node;
				return result;
			}

			protected override DragDropOption CheckDropLocalItems(IList<IGalleryItem> droppingItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
			{
				DragDropOption allowedActions = base.CheckDropLocalItems(droppingItems, targetIndex, actions, modifiers);
				if (allowedActions == DragDropOption.None && modifiers == ModifierFlags.Shift)
				{
					bool allow = true;
					foreach (IGalleryItem droppingItem in droppingItems)
					{
						if (!(droppingItem is IStudyComposerItem))
							allow &= false;
					}

					if (allow)
						return DragDropOption.Copy;
				}
				return allowedActions;
			}

			protected override DragDropOption PerformDropLocalItems(IList<IGalleryItem> droppedItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
			{
				DragDropOption performedActions = base.PerformDropLocalItems(droppedItems, targetIndex, actions, modifiers);
				if (performedActions == DragDropOption.None && modifiers == ModifierFlags.Shift)
				{
					bool allow = true;
					foreach (IGalleryItem droppedItem in droppedItems)
					{
						if (droppedItem is IStudyComposerItem)
						{
							IStudyComposerItem item = (IStudyComposerItem) droppedItem;
							base.DataSource.Insert(targetIndex, item.Clone());
						}
					}
					performedActions = DragDropOption.Copy;
				}
				return performedActions;
			}
		}

		#endregion
	}
}
