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
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Thrown when an error occurs while cloning an <see cref="IDisplaySet"/>.
	/// </summary>
	public class DisplaySetCloningException : Exception
	{
		internal DisplaySetCloningException(IDisplaySet sourceDisplaySet, Exception innerException)
			: base(BuildMessage(sourceDisplaySet), innerException)
		{

		}

		internal static string BuildMessage(IDisplaySet sourceDisplaySet)
		{
			string message = "An error has occurred while attempting to clone a display set";
			if (!String.IsNullOrEmpty(sourceDisplaySet.Name))
				message += string.Format(" (name = {0})", sourceDisplaySet.Name);
			
			message += ".";
			return message;
		}
	}

	/// <summary>
	/// Thrown when an error occurs while cloning an <see cref="IPresentationImage"/>.
	/// </summary>
	public class PresentationImageCloningException : Exception
	{
		internal PresentationImageCloningException(IPresentationImage sourceImage, Exception innerException)
			: base(BuildMessage(sourceImage), innerException)
		{
		}

		internal static string BuildMessage(IPresentationImage sourceImage)
		{
			string message = "An error has occurred while attempting to clone an image";
			if (!String.IsNullOrEmpty(sourceImage.Uid))
				message += string.Format(" (uid = {0})", sourceImage.Uid);

			message += ".";
			return message;
		}
	}

	/// <summary>
	/// Exception policy for cloning of <see cref="IPresentationImage"/>s and <see cref="IDisplaySet"/>s.
	/// </summary>
	[ExceptionPolicyFor(typeof(DisplaySetCloningException))]
	[ExceptionPolicyFor(typeof(PresentationImageCloningException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class CloningExceptionPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public CloningExceptionPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (e is DisplaySetCloningException)
				exceptionHandlingContext.ShowMessageBox(SR.MessageErrorCloningDisplaySet);
			else
				exceptionHandlingContext.ShowMessageBox(SR.MessageErrorCloningPresentationImage);
		}

		#endregion
	}
}
