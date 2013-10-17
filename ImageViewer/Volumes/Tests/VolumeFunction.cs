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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	public class VolumeFunction
	{
		/// <summary>
		/// Empty void.
		/// </summary>
		public static VolumeFunction Void = new VolumeFunction("Void", (x, y, z) => 0);

		/// <summary>
		/// Nested fuzzy shells.
		/// </summary>
		public static VolumeFunction Shells = new VolumeFunction("Shells", (x, y, z) => (float) Math.Sin(0.5*Math.Sqrt(x*x + y*y + z*z)));

		/// <summary>
		/// Weird structures vaguely resembling barcodes from different angles.
		/// </summary>
		public static VolumeFunction Barcodes = new VolumeFunction("Barcodes", (x, y, z) => (float) (x*Math.Sin(y)*(2*SquarePulseTrain(z, 25) - 1)));

		/// <summary>
		/// Odd shaded stripes.
		/// </summary>
		public static VolumeFunction Stripes = new VolumeFunction("Stripes", (x, y, z) => (x*x*x - 3*x*y*y)*(2*SquarePulseTrain(z, 25) - 1));

		/// <summary>
		/// An elliptical rod stretching diagonally.
		/// </summary>
		public static VolumeFunction Rebar = new VolumeFunction("Rebar", (x, y, z) => UnitStep(5 - Distance(x, y, z + 15, z + 15)));

		/// <summary>
		/// Two elliptical rods crossing each other.
		/// </summary>
		public static VolumeFunction Duel = new VolumeFunction("Duel", (x, y, z) => UnitStep(5 - Distance(x, y, 50, 0.8f*z + 9)) + 2*UnitStep(5 - Distance(z, y, 50, 0.8f*x + 22)));

		/// <summary>
		/// Three stars at (15,15,15), (75,25,50) and (15,85,15) of radius 2.
		/// </summary>
		public static VolumeFunction Stars = new VolumeFunction("Stars", (x, y, z) => UnitStep(2 - Distance(x, y, z, 15, 15, 15)) + 2*UnitStep(2 - Distance(x, y, z, 75, 25, 50)) + 3*UnitStep(2 - Distance(x, y, z, 15, 85, 15)));

		/// <summary>
		/// Three stars at (15,15,15), (75,25,50) and (15,85,15). These should appear as regular spheres of radius 3 if the data is interpreted with a +30 degree gantry tilt about X.
		/// </summary>
		public static VolumeFunction StarsTilted030X = new VolumeFunction("StarsTilted030X", (x, y, z) => UnitStep(3f - Distance(x, y, z, 15, 15 + (z - 15)*(float) Math.Tan(Math.PI/6), 15)) + 2*UnitStep(3f - Distance(x, y, z, 75, 25 + (z - 50)*(float) Math.Tan(Math.PI/6), 50)) + 3*UnitStep(3f - Distance(x, y, z, 15, 85 + (z - 15)*(float) Math.Tan(Math.PI/6), 15)));

		/// <summary>
		/// Three stars at (15,15,15), (75,25,50) and (15,85,15). These should appear as regular spheres of radius 3 if the data is interpreted with a -15 degree gantry tilt about X.
		/// </summary>
		public static VolumeFunction StarsTilted345X = new VolumeFunction("StarsTilted345X", (x, y, z) => UnitStep(3f - Distance(x, y, z, 15, 15 + (z - 15)*(float) Math.Tan(-Math.PI/12), 15)) + 2*UnitStep(3f - Distance(x, y, z, 75, 25 + (z - 50)*(float) Math.Tan(-Math.PI/12), 50)) + 3*UnitStep(3f - Distance(x, y, z, 15, 85 + (z - 15)*(float) Math.Tan(-Math.PI/12), 15)));

		private delegate float VolumeFunctionDelegate(float x, float y, float z);

		private readonly VolumeFunctionDelegate _function;
		private readonly string _name;

		private VolumeFunction(string name, VolumeFunctionDelegate function)
		{
			_function = function;
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public virtual float Evaluate(float x, float y, float z)
		{
			return _function(x, y, z);
		}

		public VolumeFunction Normalize(int side)
		{
			return Normalize(side, side, side);
		}

		public VolumeFunction Normalize(int width, int height, int depth)
		{
			return new NormalizedVolumeFunction(width, height, depth, this);
		}

		public Volume CreateVolume(int side, bool signed)
		{
			return CreateVolume(side, side, side, signed);
		}

		public Volume CreateVolume(int width, int height, int depth, bool signed)
		{
			DicomAttributeCollection dataset = CreateMockDataset(_name, width, height, signed);
			Size3D dimensions = new Size3D(width, height, depth);
			Vector3D spacing = new Vector3D(1, 1, 1);
			Vector3D originPatient = new Vector3D(0, 0, 0);
			Matrix3D orientationPatient = Matrix3D.GetIdentity();
			if (signed)
			{
				short[] data = CreateSignedArray(width, height, depth);
				return new S16Volume(data, dimensions, spacing, originPatient, orientationPatient, dataset, short.MinValue);
			}
			else
			{
				ushort[] data = CreateUnsignedArray(width, height, depth);
				return new U16Volume(data, dimensions, spacing, originPatient, orientationPatient, dataset, ushort.MinValue);
			}
		}

		public unsafe ISopDataSource[] CreateSops(int width, int height, int sliceCount, bool signed = false, bool bpp8 = false)
		{
			string studyInstanceUid = DicomUid.GenerateUid().UID;
			string seriesInstanceUid = DicomUid.GenerateUid().UID;
			string frameOfReferenceUid = DicomUid.GenerateUid().UID;

			var sops = new List<ISopDataSource>();
			for (int z = 0; z < sliceCount; z++)
			{
				var data = new byte[width*height*(bpp8 ? 1 : 2)];
				fixed (byte* pData = data)
				{
					if (bpp8)
					{
						var pDataS8 = (sbyte*) pData;
						for (int y = 0; y < height; y++)
						{
							for (int x = 0; x < width; x++)
							{
								byte value = (byte) (((ushort) Math.Max(Math.Min(Evaluate(x, y, z), ushort.MaxValue), ushort.MinValue)) >> 8);
								if (signed)
									pDataS8[y*width + x] = (sbyte) (value - 128);
								else
									pData[y*width + x] = value;
							}
						}
					}
					else
					{
						var pDataU16 = (ushort*) pData;
						var pDataS16 = (short*) pData;
						for (int y = 0; y < height; y++)
						{
							for (int x = 0; x < width; x++)
							{
								ushort value = (ushort) Math.Max(Math.Min(Evaluate(x, y, z), ushort.MaxValue), ushort.MinValue);
								if (signed)
									pDataS16[y*width + x] = (short) (value - 32768);
								else
									pDataU16[y*width + x] = value;
							}
						}
					}
				}

				var sop = new SimpleSopDataSource(CreateMockDataset(_name, width, height, signed, bpp8));
				sop[DicomTags.PixelData].Values = data;
				sop[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				sop[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				sop[DicomTags.FrameOfReferenceUid].SetStringValue(frameOfReferenceUid);
				sop[DicomTags.ImageOrientationPatient].SetStringValue(@"1\0\0\0\1\0");
				sop[DicomTags.ImagePositionPatient].SetStringValue(string.Format(@"0\0\{0}", z));
				sops.Add(sop);
			}
			return sops.ToArray();
		}

		public short[] CreateSignedArray(int width, int height, int depth)
		{
			return Array.ConvertAll(CreateUnsignedArray(width, height, depth), v => (short) (v - 32768));
		}

		public ushort[] CreateUnsignedArray(int width, int height, int depth)
		{
			ushort[] data = new ushort[width*height*depth];
			for (int z = 0; z < depth; z++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						data[((z*height) + y)*width + x] = (ushort) Math.Max(Math.Min(Evaluate(x, y, z), ushort.MaxValue), ushort.MinValue);
					}
				}
			}
			return data;
		}

		private static DicomAttributeCollection CreateMockDataset(string patientName, int columns, int rows, bool signed, bool bpp8 = false)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			dataset[DicomTags.PatientId].SetStringValue("PATIENT");
			dataset[DicomTags.PatientsName].SetStringValue(patientName);
			dataset[DicomTags.StudyId].SetStringValue("STUDY");
			dataset[DicomTags.SeriesDescription].SetStringValue("SERIES");
			dataset[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			dataset[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.PixelSpacing].SetStringValue(@"1\1");
			dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			dataset[DicomTags.BitsStored].SetInt32(0, bpp8 ? 8 : 16);
			dataset[DicomTags.BitsAllocated].SetInt32(0, bpp8 ? 8 : 16);
			dataset[DicomTags.HighBit].SetInt32(0, bpp8 ? 7 : 15);
			dataset[DicomTags.PixelRepresentation].SetInt32(0, signed ? 1 : 0);
			dataset[DicomTags.Rows].SetInt32(0, rows);
			dataset[DicomTags.Columns].SetInt32(0, columns);
			dataset[DicomTags.WindowCenter].SetInt32(0, signed ? 0 : (bpp8 ? 128 : 32768));
			dataset[DicomTags.WindowWidth].SetInt32(0, bpp8 ? 256 : 65536);
			dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");
			return dataset;
		}

		#region Standard Functions

		private static float SquarePulseTrain(float t, float period)
		{
			return Math.Abs((int) (2*t/period) + (t >= 0 ? 1 : 0))%2;
		}

		private static float UnitStep(float t)
		{
			return t >= 0 ? 1f : 0f;
		}

		private static float Distance(float x0, float y0, float x1, float y1)
		{
			float dx = x1 - x0;
			float dy = y1 - y0;
			return (float) Math.Sqrt(dx*dx + dy*dy);
		}

		private static float Distance(float x0, float y0, float z0, float x1, float y1, float z1)
		{
			float dx = x1 - x0;
			float dy = y1 - y0;
			float dz = z1 - z0;
			return (float) Math.Sqrt(dx*dx + dy*dy + dz*dz);
		}

		#endregion

		#region NormalizedVolumeFunction Class

		private class NormalizedVolumeFunction : VolumeFunction
		{
			private readonly float _offset;
			private readonly float _scale;

			public NormalizedVolumeFunction(int width, int height, int depth, VolumeFunction function)
				: base(function._name, function._function)
			{
				float rangeMin = float.MaxValue;
				float rangeMax = float.MinValue;
				for (int z = 0; z < depth; z++)
				{
					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							float v = function._function(x, y, z);
							rangeMin = Math.Min(rangeMin, v);
							rangeMax = Math.Max(rangeMax, v);
						}
					}
				}
				_offset = -rangeMin;
				_scale = 65535/(rangeMax - rangeMin);
			}

			public override float Evaluate(float x, float y, float z)
			{
				return (base.Evaluate(x, y, z) + _offset)*_scale;
			}
		}

		#endregion

		#region SimpleSopDataSource Class

		private class SimpleSopDataSource : SopDataSource
		{
			private readonly DicomAttributeCollection _dataset;
			private readonly SimpleSopFrameData _frameData;

			public SimpleSopDataSource(DicomAttributeCollection dataset)
			{
				_dataset = dataset;
				_frameData = new SimpleSopFrameData(1, this);
			}

			public override bool IsImage
			{
				get { return true; }
			}

			protected override ISopFrameData GetFrameData(int frameNumber)
			{
				return _frameData;
			}

			public override DicomAttribute this[DicomTag tag]
			{
				get { return _dataset[tag]; }
			}

			public override DicomAttribute this[uint tag]
			{
				get { return _dataset[tag]; }
			}

			public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
			{
				return _dataset.TryGetAttribute(tag, out attribute);
			}

			public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
			{
				return _dataset.TryGetAttribute(tag, out attribute);
			}

			private class SimpleSopFrameData : SopFrameData
			{
				public SimpleSopFrameData(int frameNumber, SopDataSource parent) : base(frameNumber, parent) {}

				public override byte[] GetNormalizedPixelData()
				{
					return (byte[]) Parent[DicomTags.PixelData].Values;
				}

				public override byte[] GetNormalizedOverlayData(int overlayNumber)
				{
					return new byte[0];
				}

				public override void Unload() {}
			}
		}

		#endregion
	}
}

#endif