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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class ReservedActionModelKeyStrokeProviderExtensionPoint : ExtensionPoint<IReservedActionModelKeyStrokeProvider>
	{
		internal static IList<XKeys> GetReservedActionModelKeyStrokes(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");

			var reserved = new List<XKeys>();
			var xp = new ReservedActionModelKeyStrokeProviderExtensionPoint();
			foreach (IReservedActionModelKeyStrokeProvider provider in xp.CreateExtensions())
			{
				provider.SetViewer(imageViewer);
				reserved.AddRange(provider.ReservedKeyStrokes);
			}
			reserved.RemoveAll(k => k == XKeys.None);
			return reserved.AsReadOnly();
		}
	}

	public interface IReservedActionModelKeyStrokeProvider
	{
		void SetViewer(IImageViewer imageViewer);

		IEnumerable<XKeys> ReservedKeyStrokes { get; }
	}

	public abstract class ReservedActionModelKeyStrokeProviderBase : IReservedActionModelKeyStrokeProvider
	{
		private IImageViewer _imageViewer;

		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		protected IDesktopWindow DesktopWindow
		{
			get { return _imageViewer.DesktopWindow; }
		}

		void IReservedActionModelKeyStrokeProvider.SetViewer(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		public abstract IEnumerable<XKeys> ReservedKeyStrokes { get; }
	}
}