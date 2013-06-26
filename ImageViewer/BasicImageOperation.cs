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

namespace ClearCanvas.ImageViewer
{
	// TODO (later): see if there's a way to cleanly get rid of this and/or 
	// put the delegates into one of the superclasses so it can be used on something other than just images.
	// For now, though, just leave it, since it's code that's already been released.

	/// <summary>
	/// A simple way to implement an <see cref="ImageOperation"/>, using delegates.
	/// </summary>
	public class BasicImageOperation : ImageOperation
	{
		/// <summary>
		/// Defines a delegate used to get the originator for a given <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate IMemorable GetOriginatorDelegate(IPresentationImage image);

		/// <summary>
		/// Defines a delegate used to apply an undoable operation to an <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate void ApplyDelegate(IPresentationImage image);

		private readonly GetOriginatorDelegate _getOriginatorDelegate;
		private readonly ApplyDelegate _applyDelegate;

		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public BasicImageOperation(GetOriginatorDelegate getOriginatorDelegate, ApplyDelegate applyDelegate)
		{
			Platform.CheckForNullReference(getOriginatorDelegate, "getOriginatorDelegate");
			Platform.CheckForNullReference(applyDelegate, "applyDelegate");

			_getOriginatorDelegate = getOriginatorDelegate;
			_applyDelegate = applyDelegate;
		}

		/// <summary>
		/// Gets the originator for the input <see cref="IPresentationImage"/>, which must be <see cref="IMemorable"/>.
		/// </summary>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return _getOriginatorDelegate(image);
		}

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		public override sealed void Apply(IPresentationImage image)
		{
			_applyDelegate(image);
		}
	}

	/// <summary>
	/// A simple way to implement a strongly-typed <see cref="ImageOperation"/>, using delegates.
	/// </summary>
	public class BasicImageOperation<TOriginator> : BasicImageOperation
		where TOriginator : class, IMemorable
	{
		/// <summary>
		/// Defines a delegate used to get the originator for a given <see cref="IPresentationImage"/>.
		/// </summary>
		public new delegate TOriginator GetOriginatorDelegate(IPresentationImage image);

		public BasicImageOperation(GetOriginatorDelegate getOriginatorDelegate, ApplyDelegate applyDelegate)
			: base(i => getOriginatorDelegate(i), applyDelegate) {}

		/// <summary>
		/// Gets the originator for the input <see cref="IPresentationImage"/>.
		/// </summary>
		public new virtual TOriginator GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as TOriginator;
		}
	}
}