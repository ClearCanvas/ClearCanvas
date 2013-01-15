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

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents exceptions encountered while deserializing DICOM overlay planes.
	/// </summary>
	public class DicomOverlayDeserializationException : Exception
	{
		private const string _defaultMessage = "An error occurred while attempting to deserialize an overlay plane.";
		private const string _defaultFormattedMessage = "An error occurred while attempting to deserialize the overlay plane ({0}).";

		/// <summary>
		/// Gets the overlay plane group on which the error occurred.
		/// </summary>
		public ushort OverlayGroup { get; private set; }

		/// <summary>
		/// Gets the source of the overlay plane on which the error occurred.
		/// </summary>
		public OverlayPlaneSource OverlaySource { get; private set; }

		/// <summary>
		/// Initializes a new instance of <see cref="DicomOverlayDeserializationException"/> with a default error message.
		/// </summary>
		public DicomOverlayDeserializationException()
			: base(_defaultMessage) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomOverlayDeserializationException"/> with a default error message.
		/// </summary>
		/// <param name="overlayGroup">The overlay plane group on which the error occurred.</param>
		/// <param name="overlaySource">The source of the overlay plane on which the error occurred.</param>
		public DicomOverlayDeserializationException(ushort overlayGroup, OverlayPlaneSource overlaySource)
			: base(string.Format(_defaultFormattedMessage, FormatOverlayPlaneId(overlayGroup, overlaySource)))
		{
			OverlayGroup = overlayGroup;
			OverlaySource = overlaySource;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomOverlayDeserializationException"/> with the specified error message.
		/// </summary>
		/// <param name="overlayGroup">The overlay plane group on which the error occurred.</param>
		/// <param name="overlaySource">The source of the overlay plane on which the error occurred.</param>
		/// <param name="message">The message that describes the error.</param>
		public DicomOverlayDeserializationException(ushort overlayGroup, OverlayPlaneSource overlaySource, string message)
			: base(message)
		{
			OverlayGroup = overlayGroup;
			OverlaySource = overlaySource;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomOverlayDeserializationException"/> with a default error message.
		/// </summary>
		/// <param name="innerException">The exception that is the cause of the current exception, if available.</param>
		public DicomOverlayDeserializationException(Exception innerException)
			: base(_defaultMessage, innerException) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomOverlayDeserializationException"/> with a default error message.
		/// </summary>
		/// <param name="overlayGroup">The overlay plane group on which the error occurred.</param>
		/// <param name="overlaySource">The source of the overlay plane on which the error occurred.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, if available.</param>
		public DicomOverlayDeserializationException(ushort overlayGroup, OverlayPlaneSource overlaySource, Exception innerException)
			: base(string.Format(_defaultFormattedMessage, FormatOverlayPlaneId(overlayGroup, overlaySource)), innerException)
		{
			OverlayGroup = overlayGroup;
			OverlaySource = overlaySource;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomOverlayDeserializationException"/> with the specified error message.
		/// </summary>
		/// <param name="overlayGroup">The overlay plane group on which the error occurred.</param>
		/// <param name="overlaySource">The source of the overlay plane on which the error occurred.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, if available.</param>
		public DicomOverlayDeserializationException(ushort overlayGroup, OverlayPlaneSource overlaySource, string message, Exception innerException)
			: base(message, innerException)
		{
			OverlayGroup = overlayGroup;
			OverlaySource = overlaySource;
		}

		/// <summary>
		/// Gets a formatted string identifying the overlay plane on which an error occurred.
		/// </summary>
		/// <param name="group">The overlay plane group on which the error occurred.</param>
		/// <param name="source">The source of the overlay plane on which the error occurred.</param>
		/// <returns>A formatted string identifying the overlay plane on which an error occurred.</returns>
		public static string FormatOverlayPlaneId(ushort group, OverlayPlaneSource source)
		{
			switch (source)
			{
				case OverlayPlaneSource.Image:
					return string.Format("Image/0x{0:X4}", group);
				case OverlayPlaneSource.PresentationState:
					return string.Format("PresentationState/0x{0:X4}", group);
				case OverlayPlaneSource.User:
				default:
					return "UserCreated";
			}
		}
	}
}