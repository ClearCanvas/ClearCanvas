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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// Extension point for views onto <see cref="CalibrationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class CalibrationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CalibrationComponent class.
	/// </summary>
	[AssociateView(typeof(CalibrationComponentViewExtensionPoint))]
	public class CalibrationComponent : ApplicationComponent
	{
		private double _lengthInCm;
		private static readonly int _decimalPlaces = 1;
		private static readonly double _minimum = 0.1;
		private static readonly double _increment = 0.1;

		public CalibrationComponent()
		{
			_lengthInCm = 1.0;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public CalibrationComponent(double lengthInCm)
		{
			Platform.CheckPositive(lengthInCm, "lengthInCm");

			_lengthInCm = Math.Round(lengthInCm, _decimalPlaces);
		}

		[ValidateGreaterThan(0.0, Inclusive = false, Message = "MessageInvalidLength")]
		public double LengthInCm
		{
			get { return _lengthInCm; }
			set { _lengthInCm = value; }
		}

		public int DecimalPlaces
		{
			get { return _decimalPlaces; }
		}

		public double Minimum
		{
			get { return _minimum; }
		}

		public double Increment
		{
			get { return _increment; }
		}

		public void Accept()
		{
			if (base.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

	}
}
