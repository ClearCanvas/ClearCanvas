#pragma region License

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

#pragma endregion

#pragma once

using namespace System;

namespace ClearCanvas {
namespace ImageViewer {
namespace Core {
namespace Functions {
	
	/// <summary>
	/// Provides a collection of methods for performing minimum intensity projection.
	/// </summary>
	public ref class MinimumIntensityProjection abstract sealed
	{
	public:
		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		static void ProjectOrthogonal(unsigned int* slabData, unsigned int* pixelData, int subsamples, int pixelsPerSubsample);
		
		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		static void ProjectOrthogonal(int* slabData, int* pixelData, int subsamples, int pixelsPerSubsample);
		
		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		static void ProjectOrthogonal(unsigned short* slabData, unsigned short* pixelData, int subsamples, int pixelsPerSubsample);
		
		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		static void ProjectOrthogonal(short* slabData, short* pixelData, int subsamples, int pixelsPerSubsample);
		
		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		static void ProjectOrthogonal(unsigned char* slabData, unsigned char* pixelData, int subsamples, int pixelsPerSubsample);
		
		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		static void ProjectOrthogonal(signed char* slabData, signed char* pixelData, int subsamples, int pixelsPerSubsample);

		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subregion of a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <remarks>
		/// This overload allows specifying a specific subregion of the slab to be projected, thus allowing for a large slab to be split up into multiple
		/// independent subregions that can be processed in parallel on multiple threads.
		/// </remarks>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		/// <param name="blockOffset">The offset, in pixels, into each subsample at which the projection will begin.</param>
		/// <param name="blockCount">The number of pixels to project for each subsample.</param>
		static void ProjectOrthogonal(unsigned int* slabData, unsigned int* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);

		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subregion of a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <remarks>
		/// This overload allows specifying a specific subregion of the slab to be projected, thus allowing for a large slab to be split up into multiple
		/// independent subregions that can be processed in parallel on multiple threads.
		/// </remarks>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		/// <param name="blockOffset">The offset, in pixels, into each subsample at which the projection will begin.</param>
		/// <param name="blockCount">The number of pixels to project for each subsample.</param>
		static void ProjectOrthogonal(int* slabData, int* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);

		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subregion of a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <remarks>
		/// This overload allows specifying a specific subregion of the slab to be projected, thus allowing for a large slab to be split up into multiple
		/// independent subregions that can be processed in parallel on multiple threads.
		/// </remarks>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		/// <param name="blockOffset">The offset, in pixels, into each subsample at which the projection will begin.</param>
		/// <param name="blockCount">The number of pixels to project for each subsample.</param>
		static void ProjectOrthogonal(unsigned short* slabData, unsigned short* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);

		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subregion of a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <remarks>
		/// This overload allows specifying a specific subregion of the slab to be projected, thus allowing for a large slab to be split up into multiple
		/// independent subregions that can be processed in parallel on multiple threads.
		/// </remarks>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		/// <param name="blockOffset">The offset, in pixels, into each subsample at which the projection will begin.</param>
		/// <param name="blockCount">The number of pixels to project for each subsample.</param>
		static void ProjectOrthogonal(short* slabData, short* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);

		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subregion of a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <remarks>
		/// This overload allows specifying a specific subregion of the slab to be projected, thus allowing for a large slab to be split up into multiple
		/// independent subregions that can be processed in parallel on multiple threads.
		/// </remarks>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		/// <param name="blockOffset">The offset, in pixels, into each subsample at which the projection will begin.</param>
		/// <param name="blockCount">The number of pixels to project for each subsample.</param>
		static void ProjectOrthogonal(unsigned char* slabData, unsigned char* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);

		/// <summary>
		/// Performs orthogonal minimum intensity projection on a subregion of a subsampled 3D volumetric slab, aggregating it into a single 2D planar image.
		/// </summary>
		/// <remarks>
		/// This overload allows specifying a specific subregion of the slab to be projected, thus allowing for a large slab to be split up into multiple
		/// independent subregions that can be processed in parallel on multiple threads.
		/// </remarks>
		/// <param name="slabData">The 3D volumetric slab to be projected. (Length must be exactly <paramref name="subsamples"/> x <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="pixelData">Buffer to receive the output 2D planar image. (Length must be exactly <paramref name="pixelsPerSubsample"/>).</param>
		/// <param name="subsamples">The number of subsamples in the 3D volumetric slab.</param>
		/// <param name="pixelsPerSubsample">The number of pixels per subsample in the 3D volumetric slab.</param>
		/// <param name="blockOffset">The offset, in pixels, into each subsample at which the projection will begin.</param>
		/// <param name="blockCount">The number of pixels to project for each subsample.</param>
		static void ProjectOrthogonal(signed char* slabData, signed char* pixelData, int subsamples, int pixelsPerSubsample, int blockOffset, int blockCount);
	};

}
}
}
}
