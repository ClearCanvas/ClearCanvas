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

using Microsoft.Win32;

namespace ClearCanvas.Ris.Client
{
	public class IEPrintBackgroundSettings
	{
		private readonly RegistryKey _iePrintBackgroundKey;
		private readonly string _iePrintBackgroundKeyPath = @"Software\Microsoft\Internet Explorer\Main";

		private bool _printBackground;
		private string _oldPrintBackground;

		public IEPrintBackgroundSettings()
			: this(true)
		{
		}

		public IEPrintBackgroundSettings(bool printBackground)
		{
			_iePrintBackgroundKey = Registry.CurrentUser.OpenSubKey(_iePrintBackgroundKeyPath, true);
			_printBackground = printBackground;

			if(_iePrintBackgroundKey != null)
			{
				_oldPrintBackground = (string) _iePrintBackgroundKey.GetValue("Print_Background");

				_iePrintBackgroundKey.SetValue("Print_Background", _printBackground ? "yes" : "no");
			}
		}

		public void Revert()
		{
			if (_iePrintBackgroundKey != null)
			{
				_iePrintBackgroundKey.SetValue("Print_Background", _oldPrintBackground);
			}
		}
	}
}