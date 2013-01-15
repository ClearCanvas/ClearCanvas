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
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// The exception that is thrown when any <see cref="Exception"/> is thrown in the rendering pipeline.
	/// </summary>
	public class RenderingException : Exception
	{
		/// <summary>
		/// Gets the <see cref="DrawMode"/> of the failed rendering operation.
		/// </summary>
		public readonly DrawMode DrawMode = DrawMode.Render;

		/// <summary>
		/// Gets the window ID of the failed rendering operation.
		/// </summary>
		/// <remarks>
		/// On Windows platforms, this would be the window handle, or hWnd.
		/// </remarks>
		public readonly IntPtr WindowId = IntPtr.Zero;

		/// <summary>
		/// Gets the context ID of the failed rendering operation.
		/// </summary>
		/// <remarks>
		/// On Windows platforms, this would be the device context handle, or hDC.
		/// </remarks>
		public readonly IntPtr ContextId = IntPtr.Zero;

		/// <summary>
		/// Gets the client rectangle of the failed rendering operation.
		/// </summary>
		public readonly Rectangle ClientRectangle = Rectangle.Empty;

		/// <summary>
		/// Gets the clipping rectangle of the failed rendering operation.
		/// </summary>
		public readonly Rectangle ClipRectangle = Rectangle.Empty;

		/// <summary>
		/// Initializes a <see cref="RenderingException"/>.
		/// </summary>
		/// <param name="innerException">The actual exception that was thrown in the rendering pipeline.</param>
		/// <param name="drawArgs">The <see cref="DrawArgs"/> of the failed rendering operation.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="innerException"/> is null.</exception>
		public RenderingException(Exception innerException, DrawArgs drawArgs)
			: base("An exception was thrown in the rendering pipeline.", innerException)
		{
			if (innerException == null)
				throw new ArgumentNullException("innerException", "An inner exception must be provided.");

			// record as much information as possible about the rendering operation for debugging purposes
			if (drawArgs != null)
			{
				DrawMode = drawArgs.DrawMode;
				if (drawArgs.RenderingSurface != null)
				{
					WindowId = drawArgs.RenderingSurface.WindowID;
					ContextId = drawArgs.RenderingSurface.ContextID;
					ClientRectangle = drawArgs.RenderingSurface.ClientRectangle;
					ClipRectangle = drawArgs.RenderingSurface.ClipRectangle;
				}
			}
		}

		//TODO (CR Sept 2010): remove or call InnerExceptionMessage.
		/// <summary>
		/// Gets a message describing the exception suitable for presentation to the end-user.
		/// </summary>
		public string UserMessage
		{
			get
			{
				var message = new StringBuilder(SR.MessageRenderingPipelineFailure);
				var specificMessage = InnerException.Message;
				if (!string.IsNullOrEmpty(specificMessage))
				{
					message.AppendLine();
					message.AppendFormat(SR.FormatReason, specificMessage);
				}
				return message.ToString();
			}
		}
	}
}