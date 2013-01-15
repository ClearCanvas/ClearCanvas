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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Drawing;

namespace ClearCanvas.Samples.Cad
{
	/// <summary>
	/// Extension point for views onto <see cref="CadApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CadApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CadApplicationComponent class
	/// </summary>
	[AssociateView(typeof(CadApplicationComponentViewExtensionPoint))]
	public class CadApplicationComponent : ImageViewerToolComponent
	{
		private bool _thresholdEnabled = false;
		private decimal _thresholdMinimum = 0;
		private decimal _thresholdMaximum = 2000;
		private decimal _threshold = 500;

		private bool _opacityEnabled = false;
		private decimal _opacityMinimum = 0;
		private decimal _opacityMaximum = 100;
		private decimal _opacity = 50;

		/// <summary>
		/// Constructor
		/// </summary>
		public CadApplicationComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext)
		{
			this.ImageViewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(EventBroker_PresentationImageSelected);
		}


		public override void Start()
		{
			// TODO prepare the component for its live phase
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#region Threshold properties

		public bool ThresholdEnabled
		{
			get { return _thresholdEnabled; }
			set
			{
				if (_thresholdEnabled != value)
				{
					_thresholdEnabled = value;
					NotifyPropertyChanged("ThresholdEnabled");
				}
			}
		}

		public decimal ThresholdMinimum
		{
			get { return _thresholdMinimum; }
			set
			{
				if (_thresholdMinimum != value)
				{
					_thresholdMinimum = value;
					NotifyPropertyChanged("ThresholdMinimum");
				}
			}
		}

		public decimal ThresholdMaximum
		{
			get { return _thresholdMaximum; }
			set
			{
				if (_thresholdMaximum != value)
				{
					_thresholdMaximum = value;
					NotifyPropertyChanged("ThresholdMaximum");
				}
			}
		}

		public decimal Threshold
		{
			get { return _threshold; }
			set
			{
				if (_threshold != value)
				{
					_threshold = value;
					NotifyPropertyChanged("Threshold");
					AnalyzeInternal();
				}
			}
		}

		#endregion

		#region Opacity properties

		public bool OpacityEnabled
		{
			get { return _opacityEnabled; }
			set
			{
				if (_opacityEnabled != value)
				{
					_opacityEnabled = value;
					NotifyPropertyChanged("OpacityEnabled");
				}
			}
		}

		public decimal OpacityMinimum
		{
			get { return _opacityMinimum; }
			set
			{
				if (_opacityMinimum != value)
				{
					_opacityMinimum = value;
					NotifyPropertyChanged("OpacityMinimum");
				}
			}
		}

		public decimal OpacityMaximum
		{
			get { return _opacityMaximum; }
			set
			{
				if (_opacityMaximum != value)
				{
					_opacityMaximum = value;
					NotifyPropertyChanged("OpacityMaximum");
				}
			}
		}

		public decimal Opacity
		{
			get { return _opacity; }
			set
			{
				if (_opacity != value)
				{
					_opacity = value;
					NotifyPropertyChanged("Opacity");
					AnalyzeInternal();
				}
			}
		}

		#endregion

		public void Analyze()
		{
			AnalyzeInternal();

			this.OpacityEnabled = true;
			this.ThresholdEnabled = true;
		}

		private void AnalyzeInternal()
		{
			IPresentationImage presImage = this.ImageViewer.SelectedPresentationImage;
			IImageGraphicProvider imageGraphicProvider = presImage as IImageGraphicProvider;
			GrayscaleImageGraphic imageGraphic = imageGraphicProvider.ImageGraphic as GrayscaleImageGraphic;
			IOverlayGraphicsProvider overlayProvider = presImage as IOverlayGraphicsProvider;

			CadOverlayGraphic cadOverlay = GetCadOverlayGraphic(overlayProvider);
			if (cadOverlay == null)
			{
				cadOverlay = new CadOverlayGraphic(imageGraphic);
				overlayProvider.OverlayGraphics.Add(cadOverlay);
			}

			MemorableUndoableCommand command = new MemorableUndoableCommand(cadOverlay);
			command.BeginState = cadOverlay.CreateMemento();
			cadOverlay.Threshold = (int)this.Threshold;
			cadOverlay.Opacity = (int)this.Opacity;
			cadOverlay.Analyze();
			command.EndState = cadOverlay.CreateMemento();

			this.ImageViewer.CommandHistory.AddCommand(command);
		}

		private CadOverlayGraphic GetCadOverlayGraphic(IOverlayGraphicsProvider provider)
		{
			foreach (IGraphic graphic in provider.OverlayGraphics)
			{
				if (graphic is CadOverlayGraphic)
					return graphic as CadOverlayGraphic;
			}

			return null;
		}

		void EventBroker_PresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			IOverlayGraphicsProvider provider = e.SelectedPresentationImage as IOverlayGraphicsProvider;

			CadOverlayGraphic cadOverlay = GetCadOverlayGraphic(provider);
			
			if (cadOverlay == null)
			{
				this.ThresholdEnabled = false;
				this.OpacityEnabled = false;
			}
			else
			{
				this.Threshold = cadOverlay.Threshold;
				this.Opacity = cadOverlay.Opacity;
				this.ThresholdEnabled = true;
				this.OpacityEnabled = true;
			}
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			OnSubjectChanged();
		}
	}
}
