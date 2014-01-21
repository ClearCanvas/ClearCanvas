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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Vtk.Rendering
{
	internal static class OpenGlImplementation
	{
		private static readonly Version _glVersion;

		static OpenGlImplementation()
		{
			// N.B. cannot be statically initialized before any rendering happens because glGetString can fail if called outside an existing OGL context
			// thus this class should only be called as necessary during rendering
			try
			{
				var oglVersion = Marshal.PtrToStringAnsi(glGetString(GL_VERSION)) ?? string.Empty;
				var result = new Regex(@"(\d+)\.(\d+)").Match(oglVersion);
				if (result.Success)
				{
					_glVersion = new Version(int.Parse(result.Groups[1].Value), int.Parse(result.Groups[2].Value));
				}
			}
			catch (Exception ex)
			{
				_glVersion = new Version(1, 1);
				Platform.Log(LogLevel.Debug, ex, "Failed to detect OpenGL version.");
			}

			// OpenGL 1.2 defines GL_UNSIGNED_INT_8_8_8_8_REV for optimized copying of 32-bit BGRA data
			// but without manufacturer drivers for video card, Windows defaults to an OpenGL 1.1-based software implementation
			ReadPixelsTypeBgra = _glVersion >= new Version(1, 2) ? GL_UNSIGNED_INT_8_8_8_8_REV : GL_UNSIGNED_BYTE;
		}

		/// <summary>
		/// Gets a type suitable for optimal reading of 32-bit BGRA data
		/// </summary>
		public static readonly uint ReadPixelsTypeBgra;

		#region Win32 API

		// ReSharper disable InconsistentNaming

		private const uint GL_VERSION = 0x1F02;

		private const uint GL_UNSIGNED_BYTE = 0x1401;
		private const uint GL_UNSIGNED_INT_8_8_8_8_REV = 0x8367;

		[DllImport("opengl32.dll")]
		private static extern IntPtr glGetString(uint name);

		// ReSharper restore InconsistentNaming

		#endregion
	}
}