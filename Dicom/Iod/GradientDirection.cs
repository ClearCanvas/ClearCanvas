using System;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents a gradient direction
	/// </summary>
	public class GradientDirection :IEquatable<GradientDirection>
	{
		#region Fields

		public double X;
		public double Y;
		public double Z;

		#endregion

		#region Static Members

		/// <summary>
		/// A GradientDirection that is (0,0,0)
		/// </summary>
		public static GradientDirection Zero =new GradientDirection(0,0,0);

		#endregion

		#region Constructors

		public GradientDirection(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;


		}

		#endregion

		#region Public Methods

		public bool Equals(GradientDirection other)
		{
			if (ReferenceEquals(this,other)) return true;
			if (other==null) return false;

			return Math.Abs(X - other.X) <= float.Epsilon && Math.Abs(Y - other.Y) <= float.Epsilon && Math.Abs(Z - other.Z) <= float.Epsilon;
		}

		public override int GetHashCode()
		{
			// copied from Vector3D
			return -0x760F6FAB ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode(); 
		}


		/// <summary>
		/// Gets a string suitable for direct insertion into a <see cref="DicomAttributeMultiValueText"/> attribute.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"{0:G12}\{1:G12}\{2:G12}", X, Y, Z);
		}

		#endregion


		/// <summary>
		/// Parses a <see cref="GradientDirection"/> from a DICOM multi-valued string.
		/// </summary>
		/// <param name="multiValuedString">Gradient direction, defined as X Y and Z, separated by a backslash.</param>
		/// <returns>
		/// NULL if there are not exactly 2 parsed values in the input string.
		/// </returns>
		public static GradientDirection FromString(string multiValuedString)
		{
			if (string.IsNullOrEmpty(multiValuedString)) return null;
			double[] values;
			if (DicomStringHelper.TryGetDoubleArray(multiValuedString, out values) && values.Length == 3)
				new GradientDirection(values[0], values[1], values[2]);

			return null;

		}

	}
}