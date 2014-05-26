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
	/// Abstract base implementation for a lookup table in the standard grayscale image display pipeline that selects range from manufacturer-independent values for display.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <seealso cref="IVoiLut"/>
	[Cloneable(true)]
	public abstract class ComposableVoiLut : ComposableLutBase, IVoiLut
	{
		public abstract double MinInputValue { get; set; }
		public abstract double MaxInputValue { get; set; }
		public abstract double MinOutputValue { get; protected set; }
		public abstract double MaxOutputValue { get; protected set; }
		public abstract double this[double input] { get; }

		public virtual void LookupValues(double[] input, double[] output, int count)
		{
			LutFunctions.LookupLut(input, output, count, this);
		}

		internal override sealed double MinInputValueCore
		{
			get { return MinInputValue; }
			set { MinInputValue = value; }
		}

		internal override sealed double MaxInputValueCore
		{
			get { return MaxInputValue; }
			set { MaxInputValue = value; }
		}

		internal override sealed double MinOutputValueCore
		{
			get { return MinOutputValue; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return MaxOutputValue; }
		}

		internal override sealed double LookupCore(double input)
		{
			return this[input];
		}

		internal override sealed void LookupCore(double[] input, double[] output, int count)
		{
			LookupValues(input, output, count);
		}

		public new IVoiLut Clone()
		{
			return (IVoiLut) base.Clone();
		}
	}
}