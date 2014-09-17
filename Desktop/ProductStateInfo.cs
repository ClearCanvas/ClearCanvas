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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents the interface of a class that can interpret the license information
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	public interface IDesktopProductLicenseInfoInterpreter
	{
		bool IsEvaluation { get; }
		string GetTitlebarText();
		string GetProductStateDescription();
	}

	[ExtensionPoint]
	public sealed class DesktopLicenseInfoInterpreterExtensionPoint : ExtensionPoint<IDesktopProductLicenseInfoInterpreter>
	{
		internal DesktopLicenseInfoInterpreterExtensionPoint() {}

		internal static IDesktopProductLicenseInfoInterpreter CreateInstance()
		{
			try
			{
				// check for a provider extension
				return (IDesktopProductLicenseInfoInterpreter) new DesktopLicenseInfoInterpreterExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return new DefaultDesktopLicenseInfoInterpreter();
			}
		}

		private sealed class DefaultDesktopLicenseInfoInterpreter : IDesktopProductLicenseInfoInterpreter
		{
			public bool IsEvaluation
			{
				get { return false; }
			}

			public string GetTitlebarText()
			{
				return null;
			}

			public string GetProductStateDescription()
			{
				return null;
			}
		}
	}

	/// <summary>
	/// Helper class to retrieve information related to the current state of the product (usually determined on the license)
	/// </summary>
	public static class ProductStateInfo
	{
		private static readonly IDesktopProductLicenseInfoInterpreter _licenseInterpreter;

		static ProductStateInfo()
		{
			_licenseInterpreter = DesktopLicenseInfoInterpreterExtensionPoint.CreateInstance();
		}

		public static bool IsEvaluationCopy
		{
			get { return _licenseInterpreter != null && _licenseInterpreter.IsEvaluation; }
		}

		public static string GetProductLicenseStateDescription()
		{
			return _licenseInterpreter == null ? null : _licenseInterpreter.GetProductStateDescription();
		}

		public static string GetTitlebarText()
		{
			return _licenseInterpreter == null ? null : _licenseInterpreter.GetTitlebarText();
		}
	}
}