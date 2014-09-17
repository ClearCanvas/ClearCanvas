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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM Image SOP Instance.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that there should no longer be any need to derive from this class; the <see cref="Sop"/>, <see cref="ImageSop"/>
	/// and <see cref="Frame"/> classes are now just simple Bridge classes (see Bridge Design Pattern)
	/// to <see cref="ISopDataSource"/> and <see cref="ISopFrameData"/>.  See the
	/// remarks for <see cref="ISopDataSource"/> for more information.
	/// </para>
	/// <para>Also, for more information on 'transient references' and the lifetime of <see cref="Sop"/>s,
	/// see <see cref="ISopReference"/>.
	/// </para>
	/// </remarks>
	public partial class ImageSop : Sop
	{
	    private class CachedValues
	    {
	        public string AnatomicalOrientationType;
            public string PresentationIntentType;
            public int? NumberOfFrames;
	    }

	    private bool? _isMultiframe;

	    private readonly object _syncLock = new object();
		private volatile FrameCollection _frames;

	    private CachedValues _cache = new CachedValues();

		/// <summary>
		/// Constructs a new instance of <see cref="ImageSop"/> from a local file.
		/// </summary>
		/// <param name="filename">The path to a local DICOM Part 10 file.</param>
		public ImageSop(string filename)
			: base(filename)
		{
			_functionalGroups = new FunctionalGroupMapCache(DataSource, DataSource.SopClassUid);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop(ISopDataSource dataSource)
			: base(dataSource)
		{
			_functionalGroups = new FunctionalGroupMapCache(DataSource, DataSource.SopClassUid);
		}

		/// <summary>
		/// A collection of <see cref="Frame"/> objects.
		/// </summary>
		/// <remarks>
		/// DICOM distinguishes between regular image SOPs and multiframe image SOPs.
		/// ClearCanvas, however, does not make this distinction, as it requires that 
		/// two sets of client code be written.  Instead, all image SOPs are considered
		/// to be multiframe, with regular images being a special case of a multiframe
		/// image with one frame. It can be assumed that all images contain at least
		/// one frame.
		/// </remarks>
		/// <seealso cref="NumberOfFrames"/>
		public FrameCollection Frames
		{
			get
			{
				if (_frames == null)
				{
					lock (_syncLock)
					{
						if (_frames == null)
						{
							var frames = new FrameCollection();
							for (int i = 1; i <= NumberOfFrames; i++)
								frames.Add(CreateFrame(i));

							_frames = frames;
						}
					}
				}

				return _frames;
			}
		}

		#region General Series Module

		/// <summary>
		/// Gets the Anatomical Orientation Type.
		/// </summary>
		public virtual string AnatomicalOrientationType
		{
		    get
		    {
		        var value = _cache.AnatomicalOrientationType;
		        if (value != null) return value;
		        
                DicomAttribute attribute;
		        _cache.AnatomicalOrientationType = value = TryGetAttribute(DicomTags.AnatomicalOrientationType, out attribute) ? attribute.ToString() : String.Empty;
		        return value;
		    }
		}

		#endregion

		#region DX Series Module

		/// <summary>
		/// Gets the Presentation Intent Type.
		/// </summary>
		public virtual string PresentationIntentType
		{
		    get
		    {
                var value = _cache.PresentationIntentType;
		        if (value != null) return value;
		        
                DicomAttribute attribute;
		        _cache.PresentationIntentType = value = TryGetAttribute(DicomTags.PresentationIntentType, out attribute) ? attribute.ToString() : String.Empty;
		        return value;
		    }
		}

		#endregion

		#region Multi-Frame Module

		/// <summary>
		/// Gets the number of frames in the image SOP.
		/// </summary>
		/// <remarks>
		/// Regular, non-multiframe DICOM images do not have this tag. However, because 
		/// such images are treated as multiframes with a single frame, 
		/// <see cref="NumberOfFrames"/> returns 1 in that case.
		/// </remarks>
		public virtual int NumberOfFrames
		{
		    get
		    {
		        var value = _cache.NumberOfFrames;
		        if (value.HasValue) return value.Value;
		        
                DicomAttribute attribute;
		        _cache.NumberOfFrames = value = Math.Max(TryGetAttribute(DicomTags.NumberOfFrames, out attribute) ? attribute.GetInt32(0, 1) : 1, 1);
		        return value.Value;
		    }
		}

		#endregion

        /// <summary>
        /// Resets any values that were cached after reading from <see cref="Sop.DataSource"/>.
        /// </summary>
        /// <remarks>
        /// Many of the property values are cached for performance reasons, as they generally never change, 
        /// and parsing values from the image header can be expensive, especially when done repeatedly.
        /// </remarks>
        public override void ResetCache()
        {
            base.ResetCache();
            _cache = new CachedValues();
            foreach (var frame in Frames)
                frame.ResetCache();
        }

		protected override IEnumerable<TransferSyntax> GetAllowableTransferSyntaxes()
		{
			var list = new List<TransferSyntax>(base.GetAllowableTransferSyntaxes());
			list.AddRange(DicomCodecRegistry.GetCodecTransferSyntaxes());
			return list;
		}

		/// <summary>
		/// Factory method to create the frame with the specified frame number.
		/// </summary>
		/// <param name="frameNumber">The numeric identifier of the <see cref="Frame"/> to create; frame numbers are <b>one-based</b>.</param>
		protected virtual Frame CreateFrame(int frameNumber)
		{
			return new Frame(this, frameNumber);
		}

		/// <summary>
		/// Validates the <see cref="ImageSop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="ImageSop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		protected override void ValidateInternal()
		{
			base.ValidateInternal();

			ValidateAllowableTransferSyntax();

			foreach (Frame frame in Frames)
				frame.Validate();
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">True if disposing, false if finalizing.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				lock (_syncLock)
				{
					if (_frames != null)
					{
						foreach (Frame frame in _frames)
							(frame as IDisposable).Dispose();

						_frames = null;
					}
				}
			}
		}

		/// <summary>
		/// Gets whether or not this Image SOP instance is a multi-frame image.
		/// </summary>
		public bool IsMultiframe
		{
			// we include a check for the functional groups since an "enhanced" image could have only one frame, but have important data encoded in functional groups anyway
			get
			{
			    if (!_isMultiframe.HasValue)
                    _isMultiframe =  NumberOfFrames > 1 || new MultiFrameFunctionalGroupsModuleIod(DataSource).HasValues();
			    return _isMultiframe.Value;
			}
		}

		/// <summary>
		/// Checks whether or not the specified SOP Class indicates a supported image type.
		/// </summary>
		/// <param name="sopClass">The SOP Class UID to be checked.</param>
		/// <returns>True if the SOP Class is a supported image type; False otherwise.</returns>
		public static bool IsSupportedSopClass(string sopClass)
		{
			return IsImageSop(sopClass);
		}

		/// <summary>
		/// Checks whether or not the specified SOP Class indicates a supported image type.
		/// </summary>
		/// <param name="sopClass">The SOP Class to be checked.</param>
		/// <returns>True if the SOP Class is a supported image type; False otherwise.</returns>
		public static bool IsSupportedSopClass(SopClass sopClass)
		{
			return IsImageSop(sopClass);
		}
	}
}