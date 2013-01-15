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

using System.ComponentModel;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// The abstract base class for nodes within the study builder tree represented by the <see cref="StudyBuilder"/> class.
	/// </summary>
	/// <remarks>
	/// This class should not be (and cannot be) inherited directly. To add a node to the study builder tree, instantiate a node at the desired tree level instead.
	/// </remarks>
	/// <see cref="PatientNode"/>
	/// <see cref="StudyNode"/>
	/// <see cref="SeriesNode"/>
	/// <see cref="SopInstanceNode"/>
	/// <see cref="StudyBuilder"/>
	public abstract class StudyBuilderNode
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private readonly string _key;
		private StudyBuilderNode _parent = null;

		internal StudyBuilderNode()
		{
			_key = serialGener.GetNext().ToString().PadLeft(8, '0');
		}

		/// <summary>
		/// Fires the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property on this node that changed.</param>
		protected virtual void FirePropertyChanged(string propertyName)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Raised when a property on this node has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Internally used key to uniquely identify the node.
		/// </summary>
		internal string Key
		{
			get { return _key; }
		}

		/// <summary>
		/// Gets the parent of this node, or null if the node is not in a study builder tree.
		/// </summary>
		public StudyBuilderNode Parent
		{
			get { return _parent; }
			internal set
			{
				if (_parent != value)
				{
					_parent = value;
					FirePropertyChanged("Parent");
				}
			}
		}

		private static readonly SerialGenerator serialGener = new SerialGenerator();

		/// <summary>
		/// A unique serial number generator for generating unique keys
		/// </summary>
		private class SerialGenerator
		{
			private volatile int _next;

			public int GetNext()
			{
				lock (this)
				{
					return _next++;
				}
			}
		}
	}
}