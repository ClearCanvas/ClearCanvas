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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.StudyManagement;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	public sealed class CopySubsetToClipboardComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(CopySubsetToClipboardComponentViewExtensionPoint))]
	public partial class CopySubsetToClipboardComponent : ApplicationComponent
	{
		private enum RangeSelectionOption
		{
			InstanceNumber = 0,
			Position 
		}

		private enum CopyOption
		{
			CopyRange = 0,
			CopyCustom
		}

		private enum CopyRangeOption
		{
			CopyAll = 0,
			CopyAtInterval
		}

		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _activeViewer;
		private IDisplaySet _currentDisplaySet;

		private int _numberOfImages;

		private RangeSelectionOption _rangeSelectionOption;
		private int _minInstanceNumber;
		private int _maxInstanceNumber;

		private CopyOption _copyOption;

		private CopyRangeOption _copyRangeOption;
		private int _copyRangeStart;
		private int _copyRangeEnd;
		private int _rangeMinimum;
		private int _rangeMaximum;
		private bool _updatingCopyRange;

		private int _copyRangeInterval;
		private static readonly int _rangeMinInterval = 2;
		private int _rangeMaxInterval;

		private string _customRange;

		internal CopySubsetToClipboardComponent(IDesktopWindow desktopWindow)
		{
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			_desktopWindow = desktopWindow;
		}

		#region Internal / Private Methods

		internal IDesktopWindow DesktopWindow
		{
			get { return _desktopWindow; }	
		}

		internal void Close()
		{
			this.Host.Exit();
		}

		private void OnWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			IImageViewer viewer = null;

			if (_desktopWindow.ActiveWorkspace != null)
				viewer = ImageViewerComponent.GetAsImageViewer(_desktopWindow.ActiveWorkspace);
			
			SetActiveViewer(viewer);
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			CurrentDisplaySet = e.SelectedImageBox.DisplaySet;
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			CurrentDisplaySet = e.SelectedDisplaySet;
		}

		private void SetActiveViewer(IImageViewer viewer)
		{
			if (_activeViewer != null)
			{
				_activeViewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
				_activeViewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;
			}

			_activeViewer = viewer;

			IDisplaySet displaySet = null;

			if (_activeViewer != null)
			{
				_activeViewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
				_activeViewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;

				if (_activeViewer.SelectedImageBox != null)
					displaySet = _activeViewer.SelectedImageBox.DisplaySet;
			}

			CurrentDisplaySet = displaySet;
		}

		private void CopyToClipboardInternal()
		{
			if (this.HasValidationErrors)
			{
				base.ShowValidation(true);
			}
			else
			{
				IImageSelectionStrategy strategy;

				if (CopyRange)
				{
					int interval = 1;
					if (CopyRangeAtInterval)
						interval = CopyRangeInterval;

					strategy = new RangeImageSelectionStrategy(CopyRangeStart, CopyRangeEnd, interval, UseInstanceNumber);
				}
				else
				{
					strategy = new CustomImageSelectionStrategy(CustomRange, RangeMinimum, RangeMaximum, UseInstanceNumber);
				}

				Clipboard.Add(CurrentDisplaySet, strategy);
				this.Host.Exit();
			}
		}

		#endregion

		public override void Start()
		{
			_desktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceChanged;

			OnWorkspaceChanged(null, null);

			base.Start();
		}

		public override void Stop()
		{
			_desktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceChanged;

			SetActiveViewer(null);

			base.Stop();
		}

		#region Validation Methods

		[ValidationMethodFor("CustomRange")]
		private ValidationResult ValidateCustomRange()
		{
			List<Range> ranges;
			if (CopyCustom && !CustomImageSelectionStrategy.Parse(CustomRange, RangeMinimum, RangeMaximum, out ranges))
				return new ValidationResult(false, SR.MessageCustomRangeInvalid);

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("CopyRangeStart")]
		private ValidationResult ValidateCopyRangeStart()
		{
			if (CopyRange && (CopyRangeStart < RangeMinimum || CopyRangeStart > CopyRangeEnd))
				return new ValidationResult(false, SR.MessageStartValueOutOfRange);

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("CopyRangeEnd")]
		private ValidationResult ValidateCopyRangeEnd()
		{
			if (CopyRange)
			{
				if (CopyRangeEnd < CopyRangeStart || CopyRangeEnd > RangeMaximum)
					return new ValidationResult(false, SR.MessageEndValueOutOfRange);
			}

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("CopyRangeInterval")]
		private ValidationResult ValidateCopyRangeInterval()
		{
			if (CopyRange && CopyRangeAtInterval)
			{
				if (CopyRangeInterval < RangeMinInterval || CopyRangeInterval > RangeMaxInterval)
					return new ValidationResult(false, SR.MessageRangeIntervalInvalid);
			}

			return new ValidationResult(true, "");
		}

		#endregion

		private IDisplaySet CurrentDisplaySet
		{
			get { return _currentDisplaySet; }
			set
			{
				if (_currentDisplaySet == value)
					return;

				_currentDisplaySet = value;

				UpdateUseInstanceNumber();
				UpdateCopyRange();
				UpdateCopyCustom();

				NotifyPropertyChanged("SourceDisplaySetDescription");
				NotifyPropertyChanged("UsePositionNumberEnabled");
				NotifyPropertyChanged("CopyRangeEnabled");
				NotifyPropertyChanged("CopyRangeAllEnabled");
				NotifyPropertyChanged("CopyRangeStartEnabled");
				NotifyPropertyChanged("CopyRangeEndEnabled");
				NotifyPropertyChanged("Enabled");
			}
		}

		private void UpdateUseInstanceNumber()
		{
			//only change values when there is a display set.
			if (CurrentDisplaySet != null)
			{
				_numberOfImages = 0;
				_minInstanceNumber = int.MaxValue;
				_maxInstanceNumber = int.MinValue;

				_numberOfImages = CurrentDisplaySet.PresentationImages.Count;

				foreach (IPresentationImage image in CurrentDisplaySet.PresentationImages)
				{
					if (image is IImageSopProvider)
					{
						IImageSopProvider provider = (IImageSopProvider)image;
						if (provider.ImageSop.InstanceNumber < _minInstanceNumber)
							_minInstanceNumber = provider.ImageSop.InstanceNumber;
						if (provider.ImageSop.InstanceNumber > _maxInstanceNumber)
							_maxInstanceNumber = provider.ImageSop.InstanceNumber;
					}
				}

				if (!UseInstanceNumberEnabled)
					UseInstanceNumber = false;
			}

			NotifyPropertyChanged("UseInstanceNumberEnabled");
		}

		private void UpdateCopyRange()
		{
			if (CurrentDisplaySet == null)
				return;

			_updatingCopyRange = true;

			if (UseInstanceNumber)
			{
				RangeMinimum = _minInstanceNumber;
				RangeMaximum = _maxInstanceNumber;
			}
			else
			{
				RangeMinimum = 1;
				RangeMaximum = _currentDisplaySet == null ? 1 : _currentDisplaySet.PresentationImages.Count;
			}

			CopyRangeStart = RangeMinimum;
			CopyRangeEnd = RangeMaximum;

			_updatingCopyRange = false;

			UpdateRangeInterval();
		}

		private void UpdateRangeInterval()
		{
			if (_updatingCopyRange)
				return;

			if (CurrentDisplaySet != null)
			{
				RangeMaxInterval = Math.Max(RangeMinInterval, CopyRangeEnd - CopyRangeStart);
				CopyRangeInterval = Math.Min(CopyRangeInterval, RangeMaxInterval);
				CopyRangeInterval = Math.Max(CopyRangeInterval, RangeMinInterval);

				if (!CopyRangeAtIntervalEnabled)
					CopyRangeAtInterval = false;
			}

			NotifyPropertyChanged("CopyRangeIntervalEnabled");
			NotifyPropertyChanged("CopyRangeAtIntervalEnabled");
		}

		private void UpdateCopyCustom()
		{
			if (CurrentDisplaySet != null)
			{
				if (!CopyCustomEnabled)
					CopyCustom = false;
			}

			NotifyPropertyChanged("CustomRangeEnabled");
			NotifyPropertyChanged("CopyCustomEnabled");
		}

		#region Presentation Model

		public string SourceDisplaySetDescription
		{
			get
			{
				if (this.CurrentDisplaySet != null)
					return this.CurrentDisplaySet.Name;
				else
					return SR.MessageNotApplicable;
			}	
		}

		public bool UsePositionNumber
		{
			get { return _rangeSelectionOption == RangeSelectionOption.Position; }
			set
			{
				if (!value)
				{
					_rangeSelectionOption = RangeSelectionOption.InstanceNumber;
					NotifyPropertyChanged("UsePositionNumber");
					NotifyPropertyChanged("UseInstanceNumber");

					UpdateCopyRange();
				}
			}
		}

		public bool UsePositionNumberEnabled
		{
			get { return Enabled; }	
		}

		public bool UseInstanceNumber
		{
			get { return _rangeSelectionOption == RangeSelectionOption.InstanceNumber; }
			set
			{
				if (!value)
				{
					_rangeSelectionOption = RangeSelectionOption.Position;
					NotifyPropertyChanged("UseInstanceNumber");
					NotifyPropertyChanged("UsePositionNumber");

					UpdateCopyRange();
				}
			}
		}

		public bool UseInstanceNumberEnabled
		{
			get { return Enabled && _minInstanceNumber != int.MaxValue && _maxInstanceNumber != int.MinValue; }
		}

		public int RangeMinimum
		{
			get { return _rangeMinimum; }
			private set
			{
				if (_rangeMinimum == value)
					return;

				_rangeMinimum = value;
				NotifyPropertyChanged("RangeMinimum");
			}
		}

		public int RangeMaximum
		{
			get { return _rangeMaximum; }
			private set
			{
				if (_rangeMaximum == value)
					return;

				_rangeMaximum = value;
				NotifyPropertyChanged("RangeMaximum");
			}
		}

		public int RangeMinInterval
		{
			get { return _rangeMinInterval; }	
		}

		public int RangeMaxInterval
		{
			get { return _rangeMaxInterval; }
			private set
			{
				if (value == _rangeMaxInterval)
					return;

				_rangeMaxInterval = value;
				NotifyPropertyChanged("RangeMaxInterval");
			}
		}
		
		public bool CopyRange
		{
			get { return _copyOption == CopyOption.CopyRange; }
			set
			{
				if (!value)
				{
					_copyOption = CopyOption.CopyCustom;
					NotifyPropertyChanged("CopyRange");
					NotifyPropertyChanged("CopyCustom");
				}
			}
		}

		public bool CopyRangeEnabled
		{
			get { return Enabled; }	
		}

		public bool CopyRangeAll
		{
			get { return _copyRangeOption == CopyRangeOption.CopyAll; }
			set
			{
				if (!value)
				{
					_copyRangeOption = CopyRangeOption.CopyAtInterval;
					NotifyPropertyChanged("CopyRangeAll");
					NotifyPropertyChanged("CopyRangeAtInterval");
				}
			}
		}

		public bool CopyRangeAllEnabled
		{
			get { return Enabled && CopyRange; }	
		}

		public int CopyRangeStart
		{
			get { return _copyRangeStart; }
			set
			{
				if (value == _copyRangeStart)
					return;

				_copyRangeStart = value;
				NotifyPropertyChanged("CopyRangeStart");

				UpdateRangeInterval();
			}
		}

		public bool CopyRangeStartEnabled
		{
			get { return Enabled && CopyRange; }
		}

		public int CopyRangeEnd
		{
			get { return _copyRangeEnd; }
			set
			{
				if (value == _copyRangeEnd)
					return;

				_copyRangeEnd = value;
				NotifyPropertyChanged("CopyRangeEnd");

				UpdateRangeInterval();
			}
		}

		public bool CopyRangeEndEnabled
		{
			get { return Enabled && CopyRange; }
		}

		public bool CopyRangeAtInterval
		{
			get { return _copyRangeOption == CopyRangeOption.CopyAtInterval; }
			set
			{
				if (!value)
				{
					_copyRangeOption = CopyRangeOption.CopyAll;
					NotifyPropertyChanged("CopyRangeAtInterval");
					NotifyPropertyChanged("CopyRangeAll");
				}
			}
		}

		public bool CopyRangeAtIntervalEnabled
		{
			get { return CopyRange && CopyRangeEnabled && (CopyRangeEnd - CopyRangeStart) >= RangeMinInterval; }
		}

		public int CopyRangeInterval
		{
			get { return _copyRangeInterval; }
			set
			{
				if (value == _copyRangeInterval)
					return;

				_copyRangeInterval = value;
				NotifyPropertyChanged("CopyRangeInterval");
			}
		}

		public bool CopyRangeIntervalEnabled
		{
			get { return CopyRangeAtInterval && CopyRangeAtIntervalEnabled; }
		}

		public bool CopyCustom
		{
			get { return _copyOption == CopyOption.CopyCustom; }
			set
			{
				if (!value)
				{
					_copyOption = CopyOption.CopyRange;
					NotifyPropertyChanged("CopyCustom");
					NotifyPropertyChanged("CopyRange");
				}
			}
		}

		public bool CopyCustomEnabled
		{
			get { return Enabled && _numberOfImages > 2; }
		}

		public string CustomRange
		{
			get { return _customRange; }
			set
			{
				if (value == _customRange)
					return;

				_customRange = value;
				NotifyPropertyChanged("CustomRange");
			}
		}

		public bool CustomRangeEnabled
		{
			get { return CopyCustom && CopyCustomEnabled; }
		}

		public bool Enabled
		{
			get { return _currentDisplaySet != null && _numberOfImages > 0; }
		}

		public void CopyToClipboard()
		{
			if (!Enabled)
				return;

			try
			{
				BlockingOperation.Run(CopyToClipboardInternal);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClipboardCopyFailed, Host.DesktopWindow);
				this.Host.Exit();
			}
		}

		#endregion
	}
}
