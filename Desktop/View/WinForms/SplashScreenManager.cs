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
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// This class handles displaying and dismissing a splash screen in an application.
	/// </summary>
	public class SplashScreenManager
	{
		/// <summary>
		/// The splash screen form.
		/// </summary>
		private static SplashScreen _splashScreen = null;

		// A separate thread is needed to display the splash screen so the main thread can 
		// continue loading the application
		private static Thread _displayThread = null;	// Thread-shared resource
		private static Mutex mutex = new Mutex();

		// Two timers are used to ensure that various forms (the splash screen and _formToActivate) are
		// updated and accessed from the threads they were created in (to avoid cross-threading exceptions)
		private static System.Windows.Forms.Timer _displayTimer = null;
		private static System.Windows.Forms.Timer _dismissTimer = null;

		// Members used in updating the contents of the splash screen (thread-shared resources)
		private static string _status = string.Empty;
		private static string _licenseText = string.Empty;
		private static List<Assembly> _assemblies = new List<Assembly>();

		private static bool _updateLicenseText = false;

		// Members used in closing the splash sceen (thread-shared resources)
		private static bool _closing = false;
		private static int _closeStartTime = 0;
		private static Form _formToActivate = null;
		private static bool _stopDisplayTimer = false;
		private static bool _stopDismissTimer = false;

		// Constants
		private const double OpacityDelta = 0.20;	// 20%
		private const int CloseDelay = 20;			// 0.02 seconds
		private const int TimerInterval = 50;		// 50 ms

		/// <summary>
		/// Begins a new background thread that immediately creates and displays and new splash screen.
		/// The thread also handles fading the splash screen in and out, and updating its contents.
		/// </summary>
		public static void DisplaySplashScreen()
		{
			System.Windows.Forms.Application.EnableVisualStyles();

			// Shared resource access follows
			mutex.WaitOne();

			// Make sure it's only launched once
			if (_displayThread != null)
				return;

			mutex.ReleaseMutex();

			_displayThread = new Thread(new ThreadStart(CreateSplashScreen));
			_displayThread.IsBackground = true;
			_displayThread.SetApartmentState(ApartmentState.STA);
			_displayThread.Start();
		}

		/// <summary>
		/// Begins the process of closing the splash screen, which typically involves a slight 
		/// delay followed by a fade out.
		/// </summary>
		/// <param name="formToActivate">The new form to activate once the splash screen is closed.</param>
		public static bool DismissSplashScreen(Form formToActivate)
		{
			// Shared resource access follows
			mutex.WaitOne();

			// Make sure it's already launched and not being dismissed
			if (_displayThread == null || _closing)
			{
				mutex.ReleaseMutex();
				return false;
			}

			// Flag the splash screen for closing
			_closing = true;
			_closeStartTime = Environment.TickCount;

			// Store the form until we can activate it properly in the dismiss timer thread
			_formToActivate = formToActivate;

			// Flag the license info for updating
			//_updateLicenseInfo = true;

			mutex.ReleaseMutex();

			// Initialize the dismiss timer
			_dismissTimer = new System.Windows.Forms.Timer();
			_dismissTimer.Tick += new EventHandler(OnDismissTimer);
			_dismissTimer.Interval = 50;

			_dismissTimer.Start();

			return true;
		}

		/// <summary>
		/// Set the 'status' text to display in the splash screen.  This text is generally used 
		/// to indicate some sort of loading progress.
		/// </summary>
		/// <param name="status">The new 'status' text to display on the splash screen.</param>
		public static void SetStatus(string status)
		{
			// Shared resource access follows
			mutex.WaitOne();

			// Store the status temporarily until we can set it properly in the timer thread
			_status = status;

			mutex.ReleaseMutex();
		}

		/// <summary>
		/// Set the license text to display in the splash screen.
		/// </summary>
		/// <param name="licenseText">The new license text to display on the splash screen.</param>
		public static void SetLicenseText(string licenseText)
		{
			// Shared resource access follows
			mutex.WaitOne();

			// Store the license text temporarily until we can set it properly in the timer thread
			_licenseText = licenseText;
			_updateLicenseText = true;

			mutex.ReleaseMutex();
		}

		public static void AddAssemblyIcon(Assembly pluginAssembly)
		{
			// Shared resource access follows
			mutex.WaitOne();

			// Store the assembly temporarily until we can add it properly in the timer thread
			if (pluginAssembly != null)
				_assemblies.Add(pluginAssembly);

			mutex.ReleaseMutex();
		}

		/// <summary>
		/// An entry point for the display thread that creates a splash screen form and begins a 
		/// timer to handle fading it in and out and updating it.  A timer is used so that the 
		/// splash screen form accessed through the same thread it was created in.  This avoids 
		/// cross-threading form exceptions.
		/// </summary>
		private static void CreateSplashScreen()
		{
			// Shared resource access follows
			mutex.WaitOne();

			// Create the splash screen
			_splashScreen = new SplashScreen();

			// Reset the 'stop timer' flags
			_stopDismissTimer = false;
			_stopDisplayTimer = false;

			mutex.ReleaseMutex();

			// Initialize the display timer
			_displayTimer = new System.Windows.Forms.Timer();
			_displayTimer.Tick += new EventHandler(OnDisplayTimer);
			_displayTimer.Interval = TimerInterval;

			_displayTimer.Start();

			System.Windows.Forms.Application.Run(_splashScreen);
		}

		/// <summary>
		/// The display timer event handler that handles updating the contents of the splash screen, 
		/// fading it in and out, cleaning it up when it's been dismissed, and signalling the dismiss
		/// timer to end.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private static void OnDisplayTimer(object sender, EventArgs e)
		{
			// Shared resource access follows
			mutex.WaitOne();

			if (_splashScreen != null)
			{
				// Send the splash screen to the back if there's a licensing error
				//if (Sentinelle.Aegis.Model.AegisSessionManager.LicenseError)
				//	_splashScreen.SendToBack();

				// Update the status
				_splashScreen.UpdateStatusText(_status);

				// Update the license
				if (_updateLicenseText)
				{
					_splashScreen.UpdateLicenseText(_licenseText);
					_updateLicenseText = false;
				}

				// Update the icons
				while (_assemblies.Count > 0)
				{
					_splashScreen.AddAssemblyIcon(_assemblies[0]);
					_assemblies.RemoveAt(0);
				}

				if (_closing)
				{
					// Wait a fixed amount of time before actually closing the splash screen
					int timeElapsedSinceClose = Environment.TickCount - _closeStartTime;
					if (timeElapsedSinceClose >= CloseDelay)
					{
						// Fade out, if necessary
						if (_splashScreen.Opacity > 0)
							_splashScreen.UpdateOpacity(_splashScreen.Opacity - OpacityDelta);
						else
						{
							// We've faded out - flag the dismiss timer to stop
							_stopDismissTimer = true;
						}
					}
				}
				else
				{
					// Fade in, if necessary
					if (_splashScreen.Opacity < 1)
						_splashScreen.UpdateOpacity(_splashScreen.Opacity + OpacityDelta);
				}
			}

			// Check to see if it's time to stop the display timer
			if (_stopDisplayTimer)
			{
				// Stop the timer
				_displayTimer.Stop();
				_displayTimer = null;

				// Close the splash screen
				if (_splashScreen != null)
				{
					_splashScreen.Close();
					_splashScreen.Dispose();
					_splashScreen = null;
				}

				// Destroy the thread
				_displayThread = null;
			}

			mutex.ReleaseMutex();
		}

		/// <summary>
		/// The dismiss timer event handler that handles activating the form that's to take focus 
		/// once the splash screen is closed and signalling the display timer to end.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnDismissTimer(object sender, EventArgs e)
		{
			// Shared resource access follows
			mutex.WaitOne();

			// Check to see if it's time to stop the dismiss timer
			if (_stopDismissTimer)
			{
				// Stop the timer
				_dismissTimer.Stop();
				_dismissTimer = null;

				// Once the splash screen has been closed, activate the new form (if any) and kill the dismiss timer
				if (_formToActivate != null)
					_formToActivate.Activate();

				// Flag the display timer to stop
				_stopDisplayTimer = true;
			}

			mutex.ReleaseMutex();
		}
	}
}
