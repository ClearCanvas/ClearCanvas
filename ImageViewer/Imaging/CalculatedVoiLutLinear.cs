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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base functionality for Luts where the <see cref="WindowWidth"/>
	/// and <see cref="WindowCenter"/> are calculated and/or retrieved from an external source.
	/// </summary>
	/// <seealso cref="IVoiLutLinear"/>
	[Cloneable(true)]
	public abstract class CalculatedVoiLutLinear : VoiLutLinearBase, IVoiLutLinear
	{
		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected CalculatedVoiLutLinear()
		{
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Gets the <see cref="WindowWidth"/>.
		/// </summary>
		protected sealed override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		/// <summary>
		/// Gets the <see cref="WindowCenter"/>.
		/// </summary>
		protected sealed override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		#endregion

		#region IVoiLutLinear Members

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		public abstract double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		public abstract double WindowCenter { get; }

		#endregion
	}
}
