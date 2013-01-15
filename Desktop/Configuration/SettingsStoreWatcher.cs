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
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration
{
	internal class SettingsStoreWatcher : IDisposable
	{
		private Timer _timer;
		private bool _isStoreOnline;
		private event EventHandler _isStoreOnlineChanged;

		public SettingsStoreWatcher()
		{
			UpdateIsStoreOnline();
		}

		public bool IsStoreOnline
		{
			get
			{
				UpdateIsStoreOnline();
				return _isStoreOnline;
			}
		}
		
		public event EventHandler IsStoreOnlineChanged
		{
			add
			{
				if (_isStoreOnlineChanged == null)
					StartWatching();

				_isStoreOnlineChanged += value;
			}
            remove
            {
            	_isStoreOnlineChanged -= value;
            	
				if (_isStoreOnlineChanged == null)
					StopWatching();
            }
		}

		private void StopWatching()
		{
			StopTimer();
		}

		private void StartWatching()
		{
			if (!SettingsStore.IsSupported)
				return;

			UpdateIsStoreOnline();
			StartTimer();
		}

		private void UpdateIsStoreOnline()
		{
			bool current = _isStoreOnline;
			_isStoreOnline = SettingsStore.IsStoreOnline;
			if (_isStoreOnline != current)
				EventsHelper.Fire(_isStoreOnlineChanged, this, EventArgs.Empty);
		}

		private void StartTimer()
		{
			if (_timer != null)
				return;

			_timer = new Timer(unused => UpdateIsStoreOnline(), null, TimeSpan.FromSeconds(5));
			_timer.Start();
		}

		private void StopTimer()
		{
			if (_timer == null)
				return;

			_timer.Stop();
			_timer.Dispose();
			_timer = null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			StopTimer();
		}

		#endregion
	}
}
