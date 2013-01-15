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

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	public abstract class DefaultPresetVoiLutOperationComponent : PresetVoiLutOperationComponent
	{
		protected DefaultPresetVoiLutOperationComponent()
		{
			this.Valid = true;
		}

		#region Sealed Off Application Component functionality

		public sealed override bool Modified
		{
			get
			{
				return false;
			}
			protected set
			{
				throw new InvalidOperationException(SR.ExceptionThePropertyCannotBeModified);
			}
		}

		public sealed override bool HasValidationErrors
		{
			get
			{
				return false;
			}
		}

		public sealed override void ShowValidation(bool show)
		{
		}

		public sealed override void Start()
		{
			base.Start();
		}

		public sealed override void Stop()
		{
			base.Stop();
		}

		#endregion

		public sealed override void Validate()
		{
		}
	}
}
