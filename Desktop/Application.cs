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
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{

	#region Extension Points

	/// <summary>
	/// Defines an extension point for providing an implementation of <see cref="IGuiToolkit"/>.
	/// </summary>
	/// <remarks>
	/// The application requires one extension of this point.
	/// </remarks>
	[ExtensionPoint]
	public sealed class GuiToolkitExtensionPoint : ExtensionPoint<IGuiToolkit> {}

	/// <summary>
	/// Defines an extension point for providing an optional implementation of <see cref="ISessionManager"/>.
	/// </summary>
	/// <remarks>
	/// The framework will use one extension of this point if found, but no extension is required.</remarks>
	[ExtensionPoint]
	public sealed class SessionManagerExtensionPoint : ExtensionPoint<ISessionManager> {}

	/// <summary>
	/// Defines an extension point for a view onto the application.
	/// </summary>
	/// <remarks>
	/// One extension is required, or the application will not run.
	/// </remarks>
	[ExtensionPoint]
	public sealed class ApplicationViewExtensionPoint : ExtensionPoint<IApplicationView> {}

	/// <summary>
	/// Tool context interface for tools that extend <see cref="ApplicationToolExtensionPoint"/>.
	/// </summary>
	public interface IApplicationToolContext : IToolContext {}

	/// <summary>
	/// Defines an extension point for application tools, which are global to the application.
	/// </summary>
	/// <remarks>
	/// Application tools are global to the application. An application tool is instantiated exactly once.
	/// Application tools cannot have actions because they are not associated with any UI entity.
	/// Extensions should expect to recieve a tool context of type <see cref="IApplicationToolContext"/>.
	/// </remarks>
	[ExtensionPoint]
	public sealed class ApplicationToolExtensionPoint : ExtensionPoint<ITool> {}

	#endregion

	/// <summary>
	/// Singleton class that represents the desktop application.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class extends <see cref="ApplicationRootExtensionPoint"/> and provides the implementation of
	/// <see cref="IApplicationRoot"/> for a desktop application.  This class may be subclassed if necessary.
	/// In order for the framework to use the subclass, it must be passed to <see cref="Platform.StartApp(ExtensionFilter, string[])"/>.
	/// (Typically this is done by passing the class name as a command line argument to the executable).
	/// </para>
	/// <para>
	/// The class provides a number of static convenience methods that may be freely used by application code.
	/// These static members should not be considered thread-safe unless they specifically state that they are.
	/// </para>
	/// <para>
	/// The <see cref="Instance"/> property can be used to obtain the singleton instance of the class (or subclass).
	/// </para>
	/// </remarks>
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	[AssociateView(typeof (ApplicationViewExtensionPoint))]
	public class Application : IApplicationRoot
	{
		private static Application _instance;

		#region UI Thread Synchronization

		internal static SynchronizationContext SynchronizationContext
		{
			get { return _instance._synchronizationContext; }
		}

		/// <summary>
		/// Marshals a delegate over to the UI thread for execution.
		/// </summary>
		/// <remarks>
		/// If the current thread is not the UI thread, the delegate is "posted" to the UI thread
		/// for execution, otherwise it is executed immediately.
		/// </remarks>
		/// <returns>True, if the delegate was (or will be) executed.</returns>
		internal static bool MarshalDelegate(Delegate del, params object[] args)
		{
			var syncContext = SynchronizationContext;
			if (syncContext == null)
				return false;

			if (Equals(syncContext, SynchronizationContext.Current))
				del.DynamicInvoke(args);
			else
				syncContext.Post(ignore => del.DynamicInvoke(args), null);

			return true;
		}

		#endregion

		#region Public Static Members

		/// <summary>
		/// Gets the singleton instance of the <see cref="Application"/> object.
		/// </summary>
		public static Application Instance
		{
			get { return _instance; }
		}

		public static SessionStatus SessionStatus
		{
			get { return SessionManager.Current.SessionStatus; }
		}

		public static event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged
		{
			add { SessionManager.Current.SessionStatusChanged += value; }
			remove { SessionManager.Current.SessionStatusChanged -= value; }
		}

		/// <summary>
		/// Gets the toolkit ID of the currently loaded GUI <see cref="IGuiToolkit"/>,
		/// or null if the toolkit has not been loaded yet.
		/// </summary>
		public static string GuiToolkitID
		{
			get { return _instance != null && _instance.GuiToolkit != null ? _instance.GuiToolkit.ToolkitID : null; }
		}

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		public static string Name
		{
			get { return _instance.ApplicationName; }
		}

		/// <summary>
		/// Gets the version of the application.
		/// </summary>
		public static Version Version
		{
			get { return _instance.ApplicationVersion; }
		}

		/// <summary>
		/// Gets the collection of application windows.
		/// </summary>
		public static DesktopWindowCollection DesktopWindows
		{
			get { return _instance.Windows; }
		}

		/// <summary>
		/// Gets the currently active window.
		/// </summary>
		/// <value>The active window, or null if no windows have been created.</value>
		public static DesktopWindow ActiveDesktopWindow
		{
			get { return DesktopWindows.ActiveWindow; }
		}

		/// <summary>
		/// Shows a message box using the application name as the title.
		/// </summary>
		/// <remarks>
		/// It is preferable to use one of the <b>ClearCanvas.Desktop.DesktopWindow.ShowMessageBox</b> 
		/// methods if a desktop window is available, since they will ensure that the message box window is 
		/// associated with the parent desktop window. This method is provided for situations where a 
		/// message box needs to be displayed prior to the creation of any desktop windows.
		/// </remarks>
		/// <param name="message">The message to display.</param>
		/// <param name="actions">The actions that the user may take.</param>
		/// <returns>The resulting action taken by the user.</returns>
		/// <seealso cref="ClearCanvas.Desktop.DesktopWindow.ShowMessageBox(string, MessageBoxActions)"/>
		/// <seealso cref="ClearCanvas.Desktop.DesktopWindow.ShowMessageBox(string, string, MessageBoxActions)"/>
		public static DialogBoxAction ShowMessageBox([param : Localizable(true)] string message, MessageBoxActions actions)
		{
			return _instance.View.ShowMessageBox(message, actions);
		}

		/// <summary>
		/// Attempts to close all open desktop windows and terminate the application.
		/// </summary>
		/// <remarks>
		/// The request to quit is not guaranteed to succeed.  Specifically, it will fail if an
		/// open workspace demands user-interaction in order to close, in which case the user may
		/// cancel the operation.  The request may also be cancelled programmatically, by handlers
		/// of the <see cref="Quitting"/> event.
		/// </remarks>
		/// <returns>True if the application successfully quits, or false if it does not.</returns>
		/// <seealso cref="Shutdown"/>
		public static bool Quit()
		{
			_instance.Quit(false);
			return _instance._guiToolkit == null;
		}

		/// <summary>
		/// Forcibly closes all open desktop windows and terminates the application.
		/// </summary>
		/// <remarks>
		/// The call will forcibly terminate the application without allowing desktop components
		/// to cancel the operation. To allow desktop components an opportunity to cancel the operation,
		/// consider calling <see cref="Quit"/> instead.
		/// </remarks>
		/// <seealso cref="Quit"/>
		public static void Shutdown()
		{
			_instance.Quit(true);
		}

		/// <summary>
		/// Invalidates the current session, which will typically require the user to
		/// re-authenticate in order to continue using the application.
		/// </summary>
		/// <returns></returns>
		public static void InvalidateSession()
		{
			try
			{
				SessionManager.Current.InvalidateSession();
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// Occurs when a request has been made for the application to quit.
		/// </summary>
		/// <remarks>
		/// This event is raised after all desktop windows have been closed, but prior to termination of
		/// the <see cref="ISessionManager"/>.
		/// </remarks>
		public static event EventHandler<QuittingEventArgs> Quitting
		{
			add { _instance._quitting += value; }
			remove { _instance._quitting -= value; }
		}

		/// <summary>
		/// Gets or sets the current application UI <see cref="CultureInfo">culture</see>.
		/// </summary>
		public static CultureInfo CurrentUICulture
		{
			get { return _instance != null ? _instance.CurrentUICultureCore : CultureInfo.InstalledUICulture; }
			set { if (_instance != null) _instance.CurrentUICultureCore = value; }
		}

		/// <summary>
		/// Fired when the value of <see cref="CurrentUICulture"/> changes.
		/// </summary>
		public static event EventHandler CurrentUICultureChanged
		{
			add { if (_instance != null) _instance._currentUICultureChanged += value; }
			remove { if (_instance != null) _instance._currentUICultureChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the current application UI <see cref="ApplicationTheme">theme</see>.
		/// </summary>
		public static ApplicationTheme CurrentUITheme
		{
			get { return _instance != null ? _instance.CurrentUIThemeCore : ApplicationTheme.DefaultApplicationTheme; }
			set { if (_instance != null) _instance.CurrentUIThemeCore = value; }
		}

		/// <summary>
		/// Fired when the value of <see cref="CurrentUITheme"/> changes.
		/// </summary>
		public static event EventHandler CurrentUIThemeChanged
		{
			add { if (_instance != null) _instance._currentUIThemeChanged += value; }
			remove { if (_instance != null) _instance._currentUIThemeChanged -= value; }
		}

		#endregion

		#region ApplicationToolContext

		private class ApplicationToolContext : ToolContext, IApplicationToolContext
		{
			public ApplicationToolContext(Application application) {}
		}

		#endregion

		private enum QuitState
		{
			NotQuitting,
			QuittingNormally,
			QuittingForcefully
		}

		private string _appName;
		private Version _appVersion;
		private volatile IGuiToolkit _guiToolkit;
		private IApplicationView _view;
		private DesktopWindowCollection _windows;
		private ToolSet _toolSet;

		private volatile bool _initialized; // flag to be set when initialization is complete
		private QuitState _quitState;
		private event EventHandler<QuittingEventArgs> _quitting;
		private volatile SynchronizationContext _synchronizationContext;

		// i18n support
		private readonly object _currentUICultureSyncLock = new object();
		private event EventHandler _currentUICultureChanged;
		private CultureInfo _currentUICulture;

		// apptheme support
		private readonly object _currentUIThemeSyncLock = new object();
		private event EventHandler _currentUIThemeChanged;
		private ApplicationTheme _currentUITheme;

		/// <summary>
		/// Default constructor, for internal framework use only.
		/// </summary>
		public Application()
		{
			_instance = this;
		}

		#region IApplicationRoot members

		/// <summary>
		/// Implementation of <see cref="IApplicationRoot.RunApplication"/>.  Runs the application.
		/// </summary>
		void IApplicationRoot.RunApplication(string[] args)
		{
			Run(args);
			//When we're quitting forcefully, typically due to an unhandled error,
			//Cleanup probably won't work anyway.
			if (IsQuittingNormally)
				CleanUp();
		}

		#endregion

		#region Protected overridables

		/// <summary>
		/// Initializes the application. Override this method to perform custom initialization.
		/// </summary>
		/// <remarks>
		/// Initializes the application, including the session manager, application tools and root window.
		/// The GUI toolkit and application view have already been initialized prior to this method being
		/// called.
		/// </remarks>
		/// <param name="args">Arguments passed in from the command line.</param>
		/// <returns>True if initialization was successful, false if the application should terminate immediately.</returns>
		protected virtual bool Initialize(string[] args)
		{
			// initialize the application UI culture from local setting
			CurrentUICulture = InstalledLocales.Instance.Selected.GetCultureInfo();

			// initialize session
			if (!InitializeSessionManager())
				return false;

			UserUpgradeProgressDialog.RunUpgradeAndShowProgress();

			// load tools
			_toolSet = new ToolSet(new ApplicationToolExtensionPoint(), new ApplicationToolContext(this));

			try
			{
				// create a root window
				_windows.AddNew("Root");
			}
			catch (Exception e)
			{
				ExceptionHandler.ReportUnhandled(e);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Called after the GUI toolkit message loop terminates, to clean up the application.  Override
		/// this method to perform custom clean-up.  Be sure to call the base class method.
		/// </summary>
		protected virtual void CleanUp()
		{
			_synchronizationContext = null;
			if (_view != null && _view is IDisposable)
			{
				(_view as IDisposable).Dispose();
				_view = null;
			}

			if (_toolSet != null)
			{
				_toolSet.Dispose();
				_toolSet = null;
			}

			if (_windows != null)
			{
				(_windows as IDisposable).Dispose();
			}

			if (_guiToolkit != null && _guiToolkit is IDisposable)
			{
				(_guiToolkit as IDisposable).Dispose();
			}
		}

		/// <summary>
		/// Raises the <see cref="Quitting"/> event.
		/// </summary>
		protected virtual void OnQuitting(QuittingEventArgs args)
		{
			EventsHelper.Fire(_quitting, this, args);
		}

		/// <summary>
		/// Raises the <see cref="CurrentUICultureChanged"/> event.
		/// </summary>
		protected virtual void OnCurrentUICultureCoreChanged(EventArgs e)
		{
			EventsHelper.Fire(_currentUICultureChanged, this, e);
		}

		/// <summary>
		/// Raises the <see cref="CurrentUIThemeChanged"/> event.
		/// </summary>
		protected virtual void OnCurrentUIThemeCoreChanged(EventArgs e)
		{
			EventsHelper.Fire(_currentUIThemeChanged, this, e);
		}

		/// <summary>
		/// Gets the display name for the application. Override this method to provide a custom display name.
		/// </summary>
		protected virtual string GetName()
		{
			return ProductInformation.GetName(false, false);
		}

		/// <summary>
		/// Gets the version of the application, which is by default the version of this assembly.
		/// Override this method to provide custom version information.
		/// </summary>
		protected virtual Version GetVersion()
		{
			return ProductInformation.Version;
		}

		#endregion

		#region Protected members

		/// <summary>
		/// Closes all desktop windows.
		/// </summary>
		protected bool CloseAllWindows()
		{
			// make a copy of the windows collection for iteration
			List<DesktopWindow> windows = new List<DesktopWindow>(_windows);
			foreach (DesktopWindow window in windows)
			{
				// if the window is still open, try to close the window
				// (the check is necessary because there is no guarantee the window is still open)
				if (window.State == DesktopObjectState.Open)
				{
					bool closed = window.Close(UserInteraction.Allowed, CloseReason.ApplicationQuit);

					// if one fails, abort
					if (!closed)
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Gets the collection of desktop windows.
		/// </summary>
		protected DesktopWindowCollection Windows
		{
			get { return _windows; }
		}

		/// <summary>
		/// Gets the GUI toolkit.
		/// </summary>
		protected IGuiToolkit GuiToolkit
		{
			get { return _guiToolkit; }
		}

		/// <summary>
		/// Gets the application view.
		/// </summary>
		protected IApplicationView View
		{
			get { return _view; }
		}

		/// <summary>
		/// Gets or sets the current application UI culture.
		/// </summary>
		protected virtual CultureInfo CurrentUICultureCore
		{
			get
			{
				lock (_currentUICultureSyncLock)
				{
					return _currentUICulture ?? CultureInfo.InstalledUICulture;
				}
			}
			set
			{
				value = value ?? CultureInfo.InstalledUICulture;
				if (_currentUICulture != value)
				{
					lock (_currentUICultureSyncLock)
					{
						if (_currentUICulture != value)
						{
							_currentUICulture = value;
							OnCurrentUICultureCoreChanged(EventArgs.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the current application UI theme.
		/// </summary>
		protected virtual ApplicationTheme CurrentUIThemeCore
		{
			get
			{
				lock (_currentUIThemeSyncLock)
				{
					return _currentUITheme ?? ApplicationTheme.DefaultApplicationTheme;
				}
			}
			set
			{
				if (_currentUITheme != value)
				{
					lock (_currentUIThemeSyncLock)
					{
						if (_currentUITheme != value)
						{
							_currentUITheme = value;
							OnCurrentUIThemeCoreChanged(EventArgs.Empty);
						}
					}
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Implements the logic to start up the desktop by running the GUI toolkit and creating the application view.
		/// </summary>
		private void Run(string[] args)
		{
			// load gui toolkit
			try
			{
				_guiToolkit = (IGuiToolkit) (new GuiToolkitExtensionPoint()).CreateExtension();
			}
			catch (Exception ex)
			{
				ExceptionHandler.ReportUnhandled(ex);
				return;
			}

			_guiToolkit.Started += delegate
			                       	{
			                       		// load application view
			                       		try
			                       		{
			                       			_synchronizationContext = SynchronizationContext.Current;
			                       			_view = (IApplicationView) ViewFactory.CreateAssociatedView(this.GetType());
			                       		}
			                       		catch (Exception ex)
			                       		{
			                       			ExceptionHandler.ReportUnhandled(ex);
			                       			TerminateGuiToolkit();
			                       			return;
			                       		}

			                       		// initialize
			                       		if (!Initialize(args))
			                       		{
			                       			TerminateGuiToolkit();
			                       			return;
			                       		}

			                       		_initialized = true;

			                       		// now that the desktop is fully initialized, take advantage of idle time to 
			                       		// load any outstanding plugins
			                       		Platform.PluginManager.EnableBackgroundAssemblyLoading(true);
			                       	};

			// init windows collection
			_windows = new DesktopWindowCollection(this);
			_windows.ItemClosed += delegate
			                       	{
			                       		// terminate the app when the window count goes to 0 if the app isn't already quitting
			                       		if (_windows.Count == 0 && !IsQuitting)
			                       		{
			                       			Quit(false);
			                       		}
			                       	};

			// start message pump - this will block until _guiToolkit.Terminate() is called
			_guiToolkit.Run();
		}

		#region Quitting

		private bool IsQuittingForcefully
		{
			get { return _quitState == QuitState.QuittingForcefully; }
		}

		private bool IsQuittingNormally
		{
			get { return _quitState == QuitState.QuittingNormally; }
		}

		private bool IsQuitting
		{
			get { return _quitState != QuitState.NotQuitting; }
		}

		#region Fatal

		#endregion

		private void Quit(bool force)
		{
			if (!_initialized && !force)
				throw new InvalidOperationException("This method cannot be called until the Application is fully initialized");

			SynchronizationContext syncContext = _synchronizationContext;
			if (syncContext == null)
				return;

			if (SynchronizationContext.Current == syncContext)
			{
				DoQuit(force);
			}
			else
			{
				try
				{
					//Whether or not this actually gets executed on the UI thread is irrelevant,
					//because if it doesn't, then the app has already quit!
					syncContext.Send(unused => DoQuit(force), null);
				}
				catch {}
			}
		}

		/// <summary>
		/// Implements the logic to terminate the desktop, including closing all windows and terminating the session.
		/// </summary>
		/// <returns>True if the application is really going to terminate, false otherwise.</returns>
		private void DoQuit(bool force)
		{
			if (IsQuitting)
				return;

			if (!force)
			{
				_quitState = QuitState.QuittingNormally;
				if (!CloseAllWindows())
				{
					_quitState = QuitState.NotQuitting;
					return;
				}

				// send quitting event
				QuittingEventArgs args = new QuittingEventArgs();
				OnQuitting(args);
			}
			else
			{
				_quitState = QuitState.QuittingForcefully;
			}

			try
			{
				SessionManager.Current.TerminateSession();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			// shut down the GUI message loop
			TerminateGuiToolkit();
		}

		#endregion

		private void TerminateGuiToolkit()
		{
			if (_guiToolkit == null)
				return;

			try
			{
				_synchronizationContext = null;
				_guiToolkit.Terminate();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Environment.Exit(-1);
			}
			finally
			{
				_guiToolkit = null;
			}
		}

		/// <summary>
		/// Initializes the session manager, using an extension if one is provided.
		/// </summary>
		/// <returns></returns>
		private static bool InitializeSessionManager()
		{
			try
			{
				return SessionManager.Current.InitiateSession();
			}
			catch (Exception ex)
			{
				// log error as fatal
				Platform.Log(LogLevel.Fatal, ex);

				// any exception thrown here should be considered a "false" return value
				return false;
			}
		}

		/// <summary>
		/// Gets the cached application name.
		/// </summary>
		private string ApplicationName
		{
			get
			{
				if (_appName == null)
					_appName = GetName();
				return _appName;
			}
		}

		/// <summary>
		/// Gets the cached application version.
		/// </summary>
		private Version ApplicationVersion
		{
			get
			{
				if (_appVersion == null)
					_appVersion = GetVersion();
				return _appVersion;
			}
		}

		/// <summary>
		/// Creates a view for a desktop window.
		/// </summary>
		internal IDesktopWindowView CreateDesktopWindowView(DesktopWindow window)
		{
			return _view.CreateDesktopWindowView(window);
		}

		#endregion
	}
}