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
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class for DICOM Files and Messages
    /// </summary>
    /// <seealso cref="DicomFile"/>
    /// <seealso cref="DicomMessage"/>
    public abstract class DicomMessageBase
    {
        /// <summary>
        /// The Transfer Syntax of the DICOM file or message
        /// </summary>
        public abstract TransferSyntax TransferSyntax { get; set; }
    
        /// <summary>
        /// The Meta information for the message.
        /// </summary>
        public DicomAttributeCollection MetaInfo
        {
            get;
            internal set;
        }

        /// <summary>
        /// The DataSet for the message.
        /// </summary>
        public DicomAttributeCollection DataSet
        {
            get; internal set;
        }

        public void ChangeTransferSyntax(TransferSyntax newTransferSyntax)
        {
            ChangeTransferSyntax(newTransferSyntax, null, null);
        }

        public void ChangeTransferSyntax(TransferSyntax newTransferSyntax, IDicomCodec inputCodec, DicomCodecParameters inputParameters)
        {
            IDicomCodec codec = inputCodec;
            DicomCodecParameters parameters = inputParameters;
            if (newTransferSyntax.Encapsulated && TransferSyntax.Encapsulated)
                throw new DicomCodecException("Source and destination transfer syntaxes encapsulated");

            if (newTransferSyntax.Encapsulated)
            {
                if (codec == null)
                {
                    codec = DicomCodecRegistry.GetCodec(newTransferSyntax);
                    if (codec == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to get registered codec for {0}", newTransferSyntax);
                        throw new DicomCodecException("No registered codec for: " + newTransferSyntax.Name);
                    }
                }
                if (parameters == null)
                    parameters = DicomCodecRegistry.GetCodecParameters(newTransferSyntax, DataSet);

            	DicomAttribute pixelData;
	            if (DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData))
	            {
		            if (pixelData.IsNull)
			            throw new DicomCodecException("Sop pixel data has no valid value and cannot be compressed.");

		            new OverlayPlaneModuleIod(DataSet).ExtractEmbeddedOverlays();

		            var pd = new DicomUncompressedPixelData(DataSet);
		            using (var pixelStream = ((DicomAttributeBinary) pixelData).AsStream())
		            {
			            //Before compression, make the pixel data more "typical", so it's harder to mess up the codecs.
			            //NOTE: Could combine mask and align into one method so we're not iterating twice, but I prefer having the methods separate.
			            if (DicomUncompressedPixelData.RightAlign(pixelStream, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
			            {
				            var newHighBit = (ushort) (pd.HighBit - pd.LowBit);
				            Platform.Log(LogLevel.Debug, "Right aligned pixel data (High Bit: {0}->{1}).", pd.HighBit, newHighBit);

				            pd.HighBit = newHighBit; //correct high bit after right-aligning.
				            DataSet[DicomTags.HighBit].SetUInt16(0, newHighBit);
			            }
			            if (DicomUncompressedPixelData.ZeroUnusedBits(pixelStream, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
			            {
				            Platform.Log(LogLevel.Debug, "Zeroed some unused bits before compression.");
			            }
		            }

		            // Set transfer syntax before compression, the codecs need it.
		            var fragments = new DicomCompressedPixelData(pd) {TransferSyntax = newTransferSyntax};
		            codec.Encode(pd, fragments, parameters);
		            fragments.UpdateMessage(this);

		            //TODO: should we validate the number of frames in the compressed data?
		            if (!DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData) || pixelData.IsNull)
			            throw new DicomCodecException("Sop has no pixel data after compression.");
	            }
	            else
	            {
		            //A bit cheap, but check for basic image attributes - if any exist
		            // and are non-empty, there should probably be pixel data too.
		            DicomAttribute spectroscopyData;
		            if (!DataSet.TryGetAttribute(DicomTags.SpectroscopyData, out spectroscopyData))
		            {
			            DicomAttribute attribute;
			            if (DataSet.TryGetAttribute(DicomTags.Rows, out attribute) && !attribute.IsNull)
				            throw new DicomCodecException(
					            "Suspect Sop appears to be an image (Rows is non-empty), but has no pixel data.");

			            if (DataSet.TryGetAttribute(DicomTags.Columns, out attribute) && !attribute.IsNull)
				            throw new DicomCodecException(
					            "Suspect Sop appears to be an image (Columns is non-empty), but has no pixel data.");
		            }
		            TransferSyntax = newTransferSyntax;
	            }
            }
            else
            {
                if (codec == null)
                {
                    codec = DicomCodecRegistry.GetCodec(TransferSyntax);
                    if (codec == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to get registered codec for {0}", TransferSyntax);

                        throw new DicomCodecException("No registered codec for: " + TransferSyntax.Name);
                    }

                    if (parameters == null)
                        parameters = DicomCodecRegistry.GetCodecParameters(TransferSyntax, DataSet);
                }

				DicomAttribute pixelData;
				if (DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData))
				{
					if (pixelData.IsNull)
						throw new DicomCodecException("Sop pixel data has no valid value and cannot be decompressed.");

					var fragments = new DicomCompressedPixelData(DataSet);
                    var pd = new DicomUncompressedPixelData(fragments);

                    codec.Decode(fragments, pd, parameters);

                    pd.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
                    TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

                    pd.UpdateMessage(this);

					//TODO: should we validate the number of frames in the decompressed data?
					if (!DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData) || pixelData.IsNull)
						throw new DicomCodecException("Sop has no pixel data after decompression.");
				}
                else
                {
					DicomAttribute spectroscopyData;
	                if (!DataSet.TryGetAttribute(DicomTags.SpectroscopyData, out spectroscopyData))
	                {
		                //NOTE: doing this for consistency, really.
		                DicomAttribute attribute;
		                if (DataSet.TryGetAttribute(DicomTags.Rows, out attribute) && !attribute.IsNull)
			                throw new DicomCodecException(
				                "Suspect Sop appears to be an image (Rows is non-empty), but has no pixel data.");

		                if (DataSet.TryGetAttribute(DicomTags.Columns, out attribute) && !attribute.IsNull)
			                throw new DicomCodecException(
				                "Suspect Sop appears to be an image (Columns is non-empty), but has no pixel data.");
	                }
	                TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
                }
            }
        }

        /// <summary>
        /// Load the contents of attributes in the message into a structure or class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will use reflection to look at the contents of the object specified by
        /// <paramref name="obj"/> and copy the values of attributes within the <see cref="MetaInfo"/>
        /// and <see cref="DataSet"/> for the message to fields in the object with 
        /// the <see cref="DicomFieldAttribute"/> attribute set for them.
        /// </para>
        /// </remarks>
        /// <param name="obj"></param>
        public void LoadDicomFields(object obj)
        {
            MetaInfo.LoadDicomFields(obj);
            DataSet.LoadDicomFields(obj);
        }

        #region Dump
        /// <summary>
        /// Dump the contents of the message to a StringBuilder.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        public abstract void Dump(StringBuilder sb, string prefix, DicomDumpOptions options);

        /// <summary>
        /// Dump the contents of the message to a string.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        /// <returns>The dump of the message.</returns>
        public string Dump(string prefix, DicomDumpOptions options)
        {
            var sb = new StringBuilder();
            Dump(sb, prefix, options);
            return sb.ToString();
        }

        /// <summary>
        /// Dump the contents of themessage to a string with the default dump options.
        /// </summary>
        /// <param name="prefix">A prefix to place in front of each dump line.</param>
        /// <returns>The dump of the message.</returns>
        public string Dump(string prefix)
        {
            return Dump(prefix, DicomDumpOptions.Default);
        }

        /// <summary>
        /// Dump the contents of the message to a string with the default options set.
        /// </summary>
        /// <returns>The dump of the message.</returns>
        public string Dump()
        {
            return Dump(String.Empty, DicomDumpOptions.Default);
        }
        #endregion

		/// <summary>
		/// Hash override that sums the hashes of the attributes within the message.
		/// </summary>
		/// <returns>The sum of the hashes of the attributes in the message.</returns>
		public override int GetHashCode()
		{
			if (MetaInfo.Count > 0 || DataSet.Count > 0)
			{
				int hash = 0;
				foreach (DicomAttribute attrib in MetaInfo)
					hash += attrib.GetHashCode();
				foreach (DicomAttribute attrib in DataSet)
					hash += attrib.GetHashCode();
				return hash;
			}
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
		    var failureReasons = new List<DicomAttributeComparisonResult>();
		    return Equals(obj, ref failureReasons);
		}

        /// <summary>
		/// Check if the contents of the DicomAttributeCollection is identical to another DicomAttributeCollection instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method compares the contents of two attribute collections to see if they are equal.  The method
		/// will step through each of the tags within the collection, and compare them to see if they are equal.  The
		/// method will also recurse into sequence attributes to be sure they are equal.</para>
		/// </remarks>
		/// <param name="obj">The object to compare to.</param>
        /// <param name="comparisonResults">A list of <see cref="DicomAttributeComparisonResult"/>  describing why the objects are not equal.</param>
		/// <returns>true if the collections are equal.</returns>
		public bool Equals(object obj, ref List<DicomAttributeComparisonResult> comparisonResults)
		{
			var a = obj as DicomFile;
			if (a == null)
			{
			    var result = new DicomAttributeComparisonResult
			                     {
			                         ResultType = ComparisonResultType.InvalidType,
			                         Details =
			                             String.Format("Comparison object is invalid type: {0}",
			                                           obj.GetType())
			                     };
			    comparisonResults.Add(result);

				return false;
			}

            if (!MetaInfo.Equals(a.MetaInfo, ref comparisonResults))
				return false;
            if (!DataSet.Equals(a.DataSet, ref comparisonResults))
				return false;

			return true;
		}
	}
}