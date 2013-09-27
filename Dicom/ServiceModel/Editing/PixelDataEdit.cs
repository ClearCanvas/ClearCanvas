using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
	public interface IEditPixelDataContext
	{
		DicomAttributeCollection DataSet { get; }

		bool Excluded { get; }
		void Exclude();

		int FrameNumber { get; }
		int Rows { get; }
		int Columns { get; }
		int SamplesPerPixel { get; }
		int BitsAllocated { get; }
		int BitsStored { get; }
		int HighBit { get; }
		int PixelRepresentation { get; }

		PhotometricInterpretation PhotometricInterpretation { get; }
		int PlanarConfiguration { get; }

		byte[] PixelData { get; }

		//ushort ToDicomUShort(int value);
		//short ToDicomShort(int value);
		//byte ToDicomByte(int value);
		//sbyte ToDicomSByte(int value);
	}

	[DataContract(Namespace = DicomEditNamespace.Value)]
	[EditType("3636399C-B2CF-404F-9D7C-E695E40E1C75")]
	public class PixelDataEditSet : PixelDataEdit
	{
		private List<PixelDataEdit> _edits;

		[DataMember(IsRequired = true)]
		public Condition Condition { get; set; }

		[DataMember(IsRequired = true)]
		public List<PixelDataEdit> Edits
		{
			get { return _edits ?? (_edits = new List<PixelDataEdit>()); }
			set { _edits = value; }
		}

		public virtual bool AppliesTo(IEditPixelDataContext context)
		{
			return Condition == null || Condition.IsMatch(context.DataSet);
		}

		public override void Apply(IEditPixelDataContext context)
		{
			if (context.Excluded || !AppliesTo(context))
				return;

			foreach (var edit in Edits)
			{
				edit.Apply(context);
				if (context.Excluded)
					return;
			}
		}

		public override string ToString()
		{
			if (Edits.Count == 0)
				return "No Pixel Data Edits";

			if (Edits.Count == 1)
				return "1 Pixel Data Edit";

			return String.Format("{0} Pixel Data Edits", Edits.Count);
		}
	}

	[KnownType("GetKnownTypes")]
	[DataContract(Namespace = DicomEditNamespace.Value)]
	public abstract class PixelDataEdit : DataContractBase
	{
		/// <summary>
		/// Applies edits to pixel data.
		/// </summary>
		/// <remarks>It is expected that all input pixel data contains no embedded overlays, 
		/// is right aligned, and is "normalized" such that it is properly represented (numerically)
		/// using the correct corresponding value type (e.g. byte, sbyte, ushort, short).</remarks>
		public abstract void Apply(IEditPixelDataContext context);

		public static IEnumerable<Type> GetKnownTypes()
		{
			return Edit.GetKnownTypes();
		}
	}
}
