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

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Extension point for providers of <see cref="IVolumeSlicerCoreProvider"/> implementations.
	/// </summary>
	[ExtensionPoint]
	public sealed class VolumeSlicerCoreProviderExtensionPoint : ExtensionPoint<IVolumeSlicerCoreProvider> {}

	/// <summary>
	/// Represents a provider of <see cref="IVolumeSlicerCoreProvider"/> implementations.
	/// </summary>
	public interface IVolumeSlicerCoreProvider
	{
		/// <summary>
		/// Called to check if the provider supports creating <see cref="IVolumeSlicerCore"/> using the specified arguments.
		/// </summary>
		bool IsSupported(VolumeSliceArgs args);

		/// <summary>
		/// Called to create an instance of <see cref="IVolumeSlicerCore"/> to create slicings from a volume.
		/// </summary>
		IVolumeSlicerCore CreateSlicerCore(IVolumeReference volumeReference, VolumeSliceArgs args);
	}
}