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
    [Cloneable(false)]
    public class NamedVoiLutLinear : VoiLutLinearBase, IVoiLutLinear
    {
        #region Memento

        public sealed class Memento : IEquatable<Memento>
        {
            public readonly string Name;
            public readonly double WindowCenter;
            public readonly double WindowWidth;

            public Memento(string name, double windowWidth, double windowCenter)
            {
                Name = name;
                WindowWidth = windowWidth;
                WindowCenter = windowCenter;
            }

            #region IEquatable<Memento> Members

            public bool Equals(Memento other)
            {
                if (other == null)
                    return false;

                return Name == other.Name && WindowWidth == other.WindowWidth && WindowCenter == other.WindowCenter;
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

        private string _name;
        private double _windowCenter;
        private double _windowWidth;

        public NamedVoiLutLinear(string name, double windowWidth, double windowCenter)
        {
            Name = name;
            WindowWidth = windowWidth;
            WindowCenter = windowCenter;
        }

        protected NamedVoiLutLinear(NamedVoiLutLinear other, ICloningContext context)
        {
            context.CloneFields(other, this);
        }

        #region Public Properties

        public string Name
        {
            get { return _name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("The LUT name must not be empty.");

                if (Equals(_name, value))
                    return;

                _name = value;
                OnLutChanged();
            }
        }

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

        #region Public Methods

        public override string GetDescription()
        {
            return String.Format(SR.FormatDescriptionNamedLinearLut, WindowWidth, WindowCenter, Name);
        }

        public override object CreateMemento()
        {
            return new Memento(Name, WindowWidth, WindowCenter);
        }

        public override void SetMemento(object memento)
        {
            var m = (Memento) memento;
            Name = m.Name;
            WindowWidth = m.WindowWidth;
            WindowCenter = m.WindowCenter;
        }

        #endregion
    }
}