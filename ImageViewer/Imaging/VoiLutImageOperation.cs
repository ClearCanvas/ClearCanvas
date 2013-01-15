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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A specialization of the <see cref="BasicImageOperation"/> where the
	/// originator is an <see cref="IVoiLutManager"/>.
	/// </summary>
	public class VoiLutImageOperation : BasicImageOperation
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public VoiLutImageOperation(ApplyDelegate applyDelegate)
			: base(GetVoiLutManager, applyDelegate)
		{
		}

		/// <summary>
		/// Returns the <see cref="IVoiLutManager"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="IVoiLutManager"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as IVoiLutManager;
		}

		private static IMemorable GetVoiLutManager(IPresentationImage image)
		{
			if (image is IVoiLutProvider)
				return ((IVoiLutProvider)image).VoiLutManager;

			return null;
		}
	}
}
