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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Specifies the <see cref="GraphicAnnotationSerializer"/> to use when serializing a DICOM graphic annotation (DICOM PS 3.3 C.10.5)
	/// </summary>
	/// <remarks>
	/// Only one attribute may be specified on any given class. Attributes decorating base classes are inherited (and can be overriden)
	/// by the derived class.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DicomSerializableGraphicAnnotationAttribute : Attribute
	{
		private readonly Type _serializerType;
		private GraphicAnnotationSerializer _serializer;

		/// <summary>
		/// Constructs a new <see cref="DicomSerializableGraphicAnnotationAttribute"/>.
		/// </summary>
		/// <param name="serializerType">The concrete implementation of <see cref="GraphicAnnotationSerializer"/> to use.</param>
		public DicomSerializableGraphicAnnotationAttribute(Type serializerType)
		{
			if (!typeof (GraphicAnnotationSerializer).IsAssignableFrom(serializerType))
				throw new ArgumentException("Serializer type must derive from GraphicAnnotationSerializer.", "serializerType");
			if (serializerType.IsAbstract)
				throw new ArgumentException("Serializer type must not be abstract.", "serializerType");
			if (serializerType.GetConstructor(Type.EmptyTypes) == null)
				throw new ArgumentException("Serializer type must have a public, parameter-less constructor.", "serializerType");

			_serializerType = serializerType;
		}

		/// <summary>
		/// Gets an instance of the specified serializer.
		/// </summary>
		public GraphicAnnotationSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					_serializer = (GraphicAnnotationSerializer) _serializerType.GetConstructor(Type.EmptyTypes).Invoke(null);
				}
				return _serializer;
			}
		}
	}

	/// <summary>
	/// Base class for a state-less class that serializes <see cref="IGraphic"/>s into <see cref="GraphicAnnotationSequenceItem"/>s according to DICOM PS 3.3 C.10.5.
	/// </summary>
	/// <remarks>
	/// Concrete implementations of this class must have a public, parameter-less constructor.
	/// </remarks>
	public abstract class GraphicAnnotationSerializer
	{
		/// <summary>
		/// Serializes the specified graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		protected abstract void Serialize(IGraphic graphic, GraphicAnnotationSequenceItem serializationState);

		/// <summary>
		/// Helper method to serialize a graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		/// <returns>True if the graphic was serializable; False otherwise.</returns>
		public static bool SerializeGraphic(IGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(serializationState, "serializationState");

			object[] attributes = graphic.GetType().GetCustomAttributes(typeof (DicomSerializableGraphicAnnotationAttribute), true);
			if (attributes.Length > 0)
			{
				((DicomSerializableGraphicAnnotationAttribute) attributes[0]).Serializer.Serialize(graphic, serializationState);
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Typed class for a state-less class that serializes a particular type of <see cref="IGraphic"/>s according to DICOM PS 3.3 C.10.5.
	/// </summary>
	/// <remarks>
	/// Concrete implementations of this class must have a public, parameter-less constructor.
	/// </remarks>
	/// <typeparam name="T">The type of <see cref="IGraphic"/> that the serializer supports.</typeparam>
	public abstract class GraphicAnnotationSerializer<T> : GraphicAnnotationSerializer where T : IGraphic
	{
		/// <summary>
		/// Serializes the specified graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		protected abstract void Serialize(T graphic, GraphicAnnotationSequenceItem serializationState);

		/// <summary>
		/// Serializes the specified graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		protected override sealed void Serialize(IGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			this.Serialize((T) graphic, serializationState);
		}
	}
}