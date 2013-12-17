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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	public abstract class AbstractVolumeTest
	{
		protected delegate void InitializeSopDataSourceDelegate(ISopDataSource sopDataSource);

		protected delegate void TestVolumeDelegate(Volume volume);

		/// <summary>
		/// Creates a 100x100x100 volume using the specified volume function using 100 frames of dimensions 100x100.
		/// </summary>
		/// <param name="function">The function with which to generate frame data - 0-99 in each dimension.</param>
		/// <param name="initializer">A delegate to initialize additional SOP attributes for each of the 100 frames.</param>
		/// <param name="testMethod">A test routine with which to exercise the volume. The volume is disposed automatically afterwards.</param>
		/// <param name="signed">Whether or not the source frames should have signed pixel data.</param>
		/// <param name="bpp8">Whether or not the source frames should have 8 bit allocated/stored pixel data.</param>
		protected static void TestVolume(VolumeFunction function, InitializeSopDataSourceDelegate initializer, TestVolumeDelegate testMethod, bool signed = false, bool bpp8 = false)
		{
			function = function.Normalize(100);
			List<ImageSop> images = new List<ImageSop>();
			try
			{
				foreach (ISopDataSource sopDataSource in function.CreateSops(100, 100, 100, signed, bpp8))
				{
					if (initializer != null)
						initializer.Invoke(sopDataSource);
					images.Add(new ImageSop(sopDataSource));
				}

				using (Volume volume = Volume.Create(EnumerateFrames(images)))
				{
					if (testMethod != null)
						testMethod.Invoke(volume);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("Thrown: {0}", ex.GetType().Name));
				throw;
			}
			finally
			{
				DisposeAll(images);
			}
		}

		protected static IEnumerable<Frame> EnumerateFrames(IEnumerable<ImageSop> imageSops)
		{
			foreach (ImageSop imageSop in imageSops)
				foreach (Frame frame in imageSop.Frames)
					yield return frame;
		}

		protected static void DisposeAll<T>(IEnumerable<T> disposables) where T : class, IDisposable
		{
			foreach (T disposable in disposables)
				if (disposable != null)
					disposable.Dispose();
		}

		private static IList<KnownSample> _starsKnownSamples;

		/// <summary>
		/// Gets a list of known points and expected values for the <see cref="VolumeFunction.Stars"/> test volume.
		/// </summary>
		protected static IList<KnownSample> StarsKnownSamples
		{
			get
			{
				if (_starsKnownSamples == null)
				{
					List<KnownSample> samplePoints = new List<KnownSample>();
					samplePoints.Add(new KnownSample(new Vector3D(15, 15, 15), (int) (65535/3f))); // The sphere at (15,15,15) is coloured 1/3 of full scale
					samplePoints.Add(new KnownSample(new Vector3D(75, 25, 50), (int) (65535*2/3f))); // The sphere at (75,25,50) is coloured 2/3 of full scale
					samplePoints.Add(new KnownSample(new Vector3D(15, 85, 15), 65535)); // The sphere at (15,85,15) is coloured 3/3 of full scale
					samplePoints.Add(new KnownSample(new Vector3D(50, 50, 50), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(75, 75, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 25, 25), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 50, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(15, 50, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 15, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 50, 15), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 85, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(85, 50, 15), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(15, 50, 85), 0)); // anything else should be 0
					_starsKnownSamples = samplePoints.AsReadOnly();
				}
				return _starsKnownSamples;
			}
		}

		protected struct KnownSample
		{
			public readonly Vector3D Point;
			public readonly int Value;

			public KnownSample(Vector3D point, int value)
			{
				Point = point;
				Value = value;
			}

			public override string ToString()
			{
				return string.Format(@"{0} @{1}", Value, Point);
			}
		}

		protected static string FormatVector(Vector3D vector)
		{
			if (vector == null)
				return "(null)";
			return string.Format("({0:f1},{1:f1},{2:f1})", vector.X, vector.Y, vector.Z);
		}
	}
}

#endif