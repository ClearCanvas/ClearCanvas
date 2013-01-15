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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
    /// <summary>
    /// The most basic of Linear Luts where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> can be directly set/manipulated.
    /// </summary>
    /// <seealso cref="IBasicVoiLutLinear"/>
    [Cloneable(true)]
    public sealed class BasicVoiLutLinear : VoiLutLinearBase, IBasicVoiLutLinear
    {
        #region Window/Level Memento class

        private class Memento : IEquatable<Memento>
        {
            public readonly double WindowCenter;
            public readonly double WindowWidth;

            public Memento(double windowWidth, double windowCenter)
            {
                WindowWidth = windowWidth;
                WindowCenter = windowCenter;
            }

            #region IEquatable<Memento> Members

            public bool Equals(Memento other)
            {
                if (other == null)
                    return false;

                return WindowWidth == other.WindowWidth && WindowCenter == other.WindowCenter;
            }

            #endregion

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;

                if (obj is Memento)
                    return Equals((Memento) obj);

                return false;
            }
        }

        #endregion

        #region Private Fields

        private double _windowCenter;
        private double _windowWidth;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Constructor.  
        /// </summary>
        /// <remarks>
        /// Allows the initial <see cref="WindowWidth"/> and <see cref="WindowCenter"/> to be set.
        /// </remarks>
        /// <param name="windowWidth">The initial Window Width.</param>
        /// <param name="windowCenter">The initial Window Center.</param>
        public BasicVoiLutLinear(double windowWidth, double windowCenter)
            : base()
        {
            WindowWidth = windowWidth;
            WindowCenter = windowCenter;
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <remarks>
        /// The initial <see cref="WindowWidth"/> and <see cref="WindowCenter"/> are 1 and 0, respectively.
        /// </remarks>
        public BasicVoiLutLinear()
            : this(1, 0)
        {
        }

        #endregion

        #region Protected Methods

        #region Overrides

        /// <summary>
        /// Gets the <see cref="WindowWidth"/>.
        /// </summary>
        protected override double GetWindowWidth()
        {
            return WindowWidth;
        }

        /// <summary>
        /// Gets the <see cref="WindowCenter"/>.
        /// </summary>
        protected override double GetWindowCenter()
        {
            return WindowCenter;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Window Width.
        /// </summary>
        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                if (value == _windowWidth)
                    return;

                if (value < 1)
                    value = 1;

                _windowWidth = value;
                base.OnLutChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Window Center.
        /// </summary>
        public double WindowCenter
        {
            get { return _windowCenter; }
            set
            {
                if (value == _windowCenter)
                    return;

                _windowCenter = value;
                base.OnLutChanged();
            }
        }

        #endregion

        #region Methods

        #region Overrides

        /// <summary>
        /// Gets an abbreviated description of the Lut.
        /// </summary>
        public override string GetDescription()
        {
            return String.Format(SR.FormatDescriptionBasicLinearLut, WindowWidth, WindowCenter);
        }

        /// <summary>
        /// Creates a memento, through which the Lut's state can be restored.
        /// </summary>
        public override object CreateMemento()
        {
            return new Memento(WindowWidth, WindowCenter);
        }

        /// <summary>
        /// Sets the Lut's state from the input memento object.
        /// </summary>
        /// <exception cref="InvalidCastException">Thrown when the memento is unrecognized, which should never happen.</exception>
        /// <param name="memento">The memento to use to restore a previous state.</param>
        public override void SetMemento(object memento)
        {
            var windowLevelMemento = (Memento) memento;
            WindowWidth = windowLevelMemento.WindowWidth;
            WindowCenter = windowLevelMemento.WindowCenter;
        }

        #endregion

        #endregion
    }
}