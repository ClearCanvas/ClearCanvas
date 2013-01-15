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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class CursorManager : IDisposable
    {
        private bool _disposed = false;
        private Control _owner;
        private Cursor _prevCursor;
        public CursorManager(Cursor cursor)
            : this(null, cursor)
        {
        }

        public CursorManager(Control owner, Cursor cursor)
        {
            _owner = owner;
            if (_owner != null)
            {
                _prevCursor = _owner.Cursor;
                _owner.Cursor = cursor;
            }
            else
            {
                _prevCursor = Cursor.Current;
                Cursor.Current = cursor;
            }
        }

        ~CursorManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_owner != null)
                    _owner.Cursor = _prevCursor;
                else
                    Cursor.Current = _prevCursor;

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
