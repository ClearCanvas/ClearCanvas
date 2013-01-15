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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class ChangePixelAspectRatioComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (ChangePixelAspectRatioComponentViewExtensionPoint))]
	public class ChangePixelAspectRatioComponent : ApplicationComponent
	{
		private bool _convertWholeDisplaySet;
		private bool _removeCalibration;
		private int _aspectRatioColumn;
		private int _aspectRatioRow;
		private bool _increasePixelDimensions;

		internal ChangePixelAspectRatioComponent()
		{
			_increasePixelDimensions = false;
			_convertWholeDisplaySet = false;
			_removeCalibration = false;
			_aspectRatioRow = 1;
			_aspectRatioColumn = 1;
		}

		[ValidationMethodFor("AspectRatioColumn")]
		private ValidationResult ValidateAspectRatioColumn()
		{
			if (AspectRatioRow == AspectRatioColumn)
				return new ValidationResult(false, "You must specify a non-1:1 aspect ratio");

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("AspectRatioRow")]
		private ValidationResult ValidateAspectRatioRow()
		{
			if (AspectRatioRow == AspectRatioColumn)
				return new ValidationResult(false, "You must specify a non-1:1 aspect ratio");

			return new ValidationResult(true, "");
		}

		public bool IncreasePixelDimensions
		{
			get { return _increasePixelDimensions; }
			set
			{
				if (_increasePixelDimensions == value)
					return;

				_increasePixelDimensions = value;
				NotifyPropertyChanged("IncreasePixelDimensions");
			}
		}
		public bool RemoveCalibration
		{
			get { return _removeCalibration; }
			set
			{
				if (_removeCalibration == value)
					return;

				_removeCalibration = value;
				NotifyPropertyChanged("RemoveCalibration");
			}
		}

		public int AspectRatioRow
		{
			get { return _aspectRatioRow; }
			set
			{
				if (_aspectRatioRow == value)
					return;

				_aspectRatioRow = value;
				NotifyPropertyChanged("AspectRatioRow");
			}
		}

		public int AspectRatioColumn
		{
			get { return _aspectRatioColumn; }
			set
			{
				if (_aspectRatioColumn == value)
					return;

				_aspectRatioColumn = value;
				NotifyPropertyChanged("AspectRatioColumn");
			}
		}

		public bool ConvertWholeDisplaySet
		{
			get { return _convertWholeDisplaySet; }
			set
			{
				if (_convertWholeDisplaySet == value)
					return;

				_convertWholeDisplaySet = value;
				NotifyPropertyChanged("ConvertWholeDisplaySet");
			}
		}

		public void Accept()
		{
			if (base.HasValidationErrors)
			{
				base.ShowValidation(true);	
				return;
			}

			base.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
		}

		public void Cancel()
		{
			base.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}
	}
}