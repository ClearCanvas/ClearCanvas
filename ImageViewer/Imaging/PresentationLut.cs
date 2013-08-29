using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLut : IComposableLut
	{
		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		/// <remarks>This is set internally by the framework.</remarks>
		new int MinOutputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		/// <remarks>This is set internally by the framework.</remarks>
		new int MaxOutputValue { get; set; }

		/// <summary>
		/// Gets the output value for the given input value.
		/// </summary>
		new int this[double value] { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IPresentationLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		new IPresentationLut Clone();
	}

	[Cloneable(true)]
	public abstract class PresentationLut : ComposableLutBase, IPresentationLut
	{
		private double _minInputValue = double.MinValue;
		private double _maxInputValue = double.MaxValue;
		private int _minOutputValue = int.MinValue;
		private int _maxOutputValue = int.MaxValue;

		protected PresentationLut() {}

		#region Overrides of ComposableLutBase

		internal override sealed double MinInputValueCore
		{
			get { return _minInputValue; }
			set
			{
				if (FloatComparer.AreEqual(_minInputValue, value))
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		internal override sealed double MaxInputValueCore
		{
			get { return _maxInputValue; }
			set
			{
				if (FloatComparer.AreEqual(_maxInputValue, value))
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		internal override sealed double MinOutputValueCore
		{
			get { return _minOutputValue; }
		}

		internal override sealed double MaxOutputValueCore
		{
			get { return _maxOutputValue; }
		}

		internal override sealed double Lookup(double input)
		{
			return this[input];
		}

		#endregion

		#region IPresentationLut Members

		public double MinInputValue
		{
			get { return MinInputValueCore; }
			set { MinInputValueCore = value; }
		}

		public double MaxInputValue
		{
			get { return MaxInputValueCore; }
			set { MaxInputValueCore = value; }
		}

		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
			set
			{
				if (FloatComparer.AreEqual(_maxOutputValue, value))
					return;

				_maxOutputValue = value;
				OnLutChanged();
			}
		}

		public int MinOutputValue
		{
			get { return _minOutputValue; }
			set
			{
				if (FloatComparer.AreEqual(_minOutputValue, value))
					return;

				_minOutputValue = value;
				OnLutChanged();
			}
		}

		public abstract int this[double input] { get; }

		public new IPresentationLut Clone()
		{
			return base.Clone() as IPresentationLut;
		}

		#endregion
	}

	[Cloneable(true)]
	public class PresentationLutLinear : PresentationLut
	{
		public override int this[double value]
		{
			get
			{
				//Rather than accessing the methods repeatedly.
				var minInput = MinInputValue;
				var maxInput = MaxInputValue;
				var minOutput = MinOutputValue;
				var maxOutput = MaxOutputValue;

				//Optimization.
				if (value <= minInput)
					return minOutput;
				if (value >= maxInput)
					return maxOutput;

				var inputRange = maxInput - minInput;
				double outputRange = maxOutput - minOutput;
				return Math.Min(maxOutput, Math.Max(minOutput, (int) Math.Round(outputRange*(value - minInput)/inputRange + minOutput)));
			}
		}

		public override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3}_{4}", GetType(), MinInputValue, MaxInputValue, MinOutputValue, MaxOutputValue);
		}

		public override string GetDescription()
		{
			return SR.DescriptionPresentationLut;
		}
	}
}