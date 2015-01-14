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
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common.Utilities;
using JetBrains.Annotations;
using log4net;
using log4net.Config;

// This will cause log4net to look for a configuration file
// called Logging.config in the application base
// directory (i.e. the directory containing TestApp.exe)
// The config file will be watched for changes.

[assembly : XmlConfigurator(ConfigFile = "Logging.config", Watch = true)]

namespace ClearCanvas.Common
{
	/// <summary>
	/// Defines the logging level for calls to one of the <b>Platform.Log</b> methods.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Debug log level.
		/// </summary>
		Debug,

		/// <summary>
		/// Info log level.
		/// </summary>
		Info,

		/// <summary>
		/// Warning log level.
		/// </summary>
		Warn,

		/// <summary>
		/// Error log level.
		/// </summary>
		Error,

		/// <summary>
		/// Fatal log level.
		/// </summary>
		Fatal
	}

	/// <summary>
	/// An extension point for <see cref="IMessageBox"/>es.
	/// </summary>
	[ExtensionPoint()]
	public sealed class MessageBoxExtensionPoint : ExtensionPoint<IMessageBox> {}

	/// <summary>
	/// Defines the Application Root extension point.
	/// </summary>
	/// <remarks>
	/// When one of the <b>Platform.StartApp</b> methods are called,
	/// the platform creates an application root extension and executes it by calling
	/// <see cref="IApplicationRoot.RunApplication" />.
	/// </remarks>
	[ExtensionPoint()]
	public sealed class ApplicationRootExtensionPoint : ExtensionPoint<IApplicationRoot> {}

	/// <summary>
	/// An extension point for <see cref="ITimeProvider"/>s.
	/// </summary>
	/// <remarks>
	/// Used internally by the framework to create a <see cref="ITimeProvider"/> for
	/// use by the application (see <see cref="Platform.Time"/>).
	/// </remarks>
	[ExtensionPoint()]
	public sealed class TimeProviderExtensionPoint : ExtensionPoint<ITimeProvider> {}

	/// <summary>
	/// Defines an extension point for service providers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A service provider is a class that knows how to provide a specific set of services to the 
	/// application.  A given service should be provided exclusively by one provider 
	/// (i.e. no two providers should provide the same service).  The application obtains
	/// services through the <see cref="Platform.GetService"/> method.
	/// </para>
	/// <para>
	/// A service provider may be accessed by multiple threads.  For reasons of thread-safety, a service provider
	/// should return a new instance of the service class for each call to <see cref="IServiceProvider.GetService"/>,
	/// so that each thread receives its own copy of the service.
	/// If the provider returns the same object (singleton), then the service object itself must be thread-safe.
	/// </para>
	/// </remarks>
	[ExtensionPoint]
	public sealed class ServiceProviderExtensionPoint : ExtensionPoint<IServiceProvider> {}

	public interface IDuplexServiceProvider
	{
		object GetService(Type type, object callback);
	}

	[ExtensionPoint]
	public sealed class DuplexServiceProviderExtensionPoint : ExtensionPoint<IDuplexServiceProvider> {}

	/// <summary>
	/// A collection of useful utility functions.
	/// </summary>
	public static class Platform
	{
		#region Private fields

		private static object _syncRoot = new Object();

		private static readonly ILog _log = LogManager.GetLogger(typeof (Platform));
		private static readonly object _namedLogLock = new object();
		private static readonly Dictionary<string, ILog> _namedLogs = new Dictionary<string, ILog>();

		private static string _commonSubFolder = "common";
		private static string _logSubFolder = "logs";
		private static string _manifestSubFolder = "manifest";

		private static volatile string _installDirectory = null;
		private static volatile string _pluginsDirectory = null;
		private static volatile string _commonDirectory = null;
		private static volatile string _logDirectory = null;
		private static volatile string _manifestDirectory = null;
		private static volatile string _applicationDataDirectory = null;

		private static volatile PluginManager _pluginManager;
		private static volatile IApplicationRoot _applicationRoot;
		private static volatile IMessageBox _messageBox;
		private static volatile ITimeProvider _timeProvider;
		private static volatile IServiceProvider[] _serviceProviders;
		private static volatile IDuplexServiceProvider[] _duplexServiceProviders;
	    private static bool? _isMono;

		#endregion

#if UNIT_TESTS

		/// <summary>
		/// Sets the extension factory that is used to instantiate extensions.
		/// </summary>
		/// <remarks>
		/// This purpose of this method is to facilitate unit testing by allowing the creation of extensions
		/// to be controlled by the testing code.
		/// </remarks>
		/// <param name="factory"></param>
		public static void SetExtensionFactory(IExtensionFactory factory)
		{
			lock (_syncRoot)
			{
				//I'm sure there are other places where this might be a problem, but if
				//you use an extension factory that creates service provider extensions,
				//you can get UnknownServiceException problems in unit tests unless
				//these 2 variables are repopulated.
				_serviceProviders = null;
				_duplexServiceProviders = null;
			}

			ExtensionPoint.SetExtensionFactory(factory);
		}

		/// <summary>
		/// Resets the extension factory to the default implementation.
		/// </summary>
		/// <remarks>
		/// This purpose of this method is to facilitate unit testing by allowing the factory to be
		/// reset to the default instance after the <see cref="SetExtensionFactory"/> method has been called.
		/// </remarks>
		public static void ResetExtensionFactory()
		{
			SetExtensionFactory(new DefaultExtensionFactory());
		}
#endif

		/// <summary>
		/// Gets the one and only <see cref="PluginManager"/>.
		/// </summary>
		public static PluginManager PluginManager
		{
			get
			{
				if (_pluginManager == null)
				{
					lock (_syncRoot)
					{
						if (_pluginManager == null)
							_pluginManager = new PluginManager(PluginDirectory);
					}
				}

				return _pluginManager;
			}
		}

		/// <summary>
		/// Gets whether the application is executing on a Win32 operating system
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static bool IsWin32Platform
		{
			get
			{
				PlatformID id = Environment.OSVersion.Platform;
				return (id == PlatformID.Win32NT || id == PlatformID.Win32Windows || id == PlatformID.Win32S || id == PlatformID.WinCE);
			}
		}

	    public static bool IsMono
	    {
	        get
	        {
	            if (!_isMono.HasValue)
                    _isMono = Type.GetType("Mono.Runtime") != null;
                
                return _isMono.Value;
	        }    
	    }

		/// <summary>
		/// Gets whether the application is executing on Mac, Linux, or other Unix-like operating systems
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static bool IsUnixPlatform
		{
			get
			{
				// under mono, which is currently the only way this code would ever run on mac or linux, macs are reported as unix for compatibility reasons
				PlatformID id = Environment.OSVersion.Platform;
				return (id == PlatformID.Unix || id == PlatformID.MacOSX);
			}
		}

		/// <summary>
		/// Gets the file-system path separator character for the current operating system
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static char PathSeparator
		{
			get { return IsWin32Platform ? '\\' : '/'; }
		}

		/// <summary>
		/// Gets the ClearCanvas installation directory.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static string InstallDirectory
		{
			get
			{
				if (_installDirectory == null)
				{
					lock (_syncRoot)
					{
						if (_installDirectory == null)
							_installDirectory = AppDomain.CurrentDomain.BaseDirectory;
					}
				}

				return _installDirectory;
			}
		}

		/// <summary>
		/// Gets the fully qualified plugin directory.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static string PluginDirectory
		{
			get
			{
				if (_pluginsDirectory == null)
				{
					lock (_syncRoot)
					{
						if (_pluginsDirectory == null)
						{
							var pluginsDirectory = Path.Combine(InstallDirectory, ExtensionSettings.Default.PluginPath);

							// fallback to install directory if PluginPath doesn't exist, for backwards compatibility
							_pluginsDirectory = Directory.Exists(pluginsDirectory) ? pluginsDirectory : InstallDirectory;
						}
					}
				}

				return _pluginsDirectory;
			}
		}

		/// <summary>
		/// Gets the fully qualified common directory.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static string CommonDirectory
		{
			get
			{
				if (_commonDirectory == null)
				{
					lock (_syncRoot)
					{
						if (_commonDirectory == null)
						{
							string commonDirectory =
								Path.Combine(InstallDirectory, _commonSubFolder);

							if (Directory.Exists(commonDirectory))
								_commonDirectory = commonDirectory;
							else
								_commonDirectory = InstallDirectory;
						}
					}
				}

				return _commonDirectory;
			}
		}

		/// <summary>
		/// Gets the fully qualified log directory.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static string LogDirectory
		{
			get
			{
				if (_logDirectory == null)
				{
					lock (_syncRoot)
					{
						if (_logDirectory == null)
							_logDirectory = Path.Combine(InstallDirectory, _logSubFolder);
					}
				}

				return _logDirectory;
			}
		}

		/// <summary>
		/// Gets the fully qualified Manifest directory.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static string ManifestDirectory
		{
			get
			{
				if (_manifestDirectory == null)
				{
					lock (_syncRoot)
					{
						if (_manifestDirectory == null)
							_manifestDirectory = Path.Combine(InstallDirectory, _manifestSubFolder);
					}
				}

				return _manifestDirectory;
			}
		}

		/// <summary>
		/// Gets the fully qualified application data directory.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		public static string ApplicationDataDirectory
		{
			get
			{
				if (_applicationDataDirectory == null)
				{
					lock (_syncRoot)
					{
						_applicationDataDirectory = GetApplicationDataDirectory();
					}
				}
				return _applicationDataDirectory;
			}
		}

		private static string GetApplicationDataDirectory()
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			path = Path.Combine(path, "ClearCanvas_Inc"); //TODO this seems to be derived from the AssemblyCompanyAttribute
			if (string.IsNullOrEmpty(ProductInformation.FamilyName))
				path = Path.Combine(path, ProductInformation.GetName(true, false));
			else
			{
				path = Path.Combine(path, string.Format("{0} {1}", ProductInformation.Component, ProductInformation.FamilyName));
			}

			return path;
		}

		#region Time

		/// <summary>
		/// Gets the current time from an extension of <see cref="TimeProviderExtensionPoint"/>, if one exists.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The time returned may differ from the current time on this machine, because the provider may choose
		/// to obtain the time from another source (i.e. a server).
		/// </para>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// </remarks>
		public static DateTime Time
		{
			get { return GetCurrentTime(DateTimeKind.Local); }
		}

		/// <summary>
		/// Gets the current time in UTC from an extension of <see cref="TimeProviderExtensionPoint"/>, if one exists.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The time returned may differ from the current time on this machine, because the provider may choose
		/// to obtain the time from another source (i.e. a server).
		/// </para>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// </remarks>
		public static DateTime UtcTime
		{
			get { return GetCurrentTime(DateTimeKind.Utc); }
		}

		private static DateTime GetCurrentTime(DateTimeKind kind)
		{
			if (_timeProvider == null)
			{
				lock (_syncRoot)
				{
					if (_timeProvider == null)
					{
						try
						{
							// check for a time provider extension
							var xp = new TimeProviderExtensionPoint();
							_timeProvider = (ITimeProvider) xp.CreateExtension();
						}
						catch (NotSupportedException)
						{
							// can't find time provider, default to local time
							Log(LogLevel.Debug, SR.LogTimeProviderNotFound);

							_timeProvider = new LocalTimeProvider();
						}
					}
				}
			}

			// need to lock here, as the time provider itself may not be thread-safe
			// note: lock on _timeProvider rather than _syncRoot, so _syncRoot remains free for other methods
			lock (_timeProvider)
			{
				return _timeProvider.GetCurrentTime(kind);
			}
		}

		#endregion

		#region Application start-up

		/// <summary>
		/// Starts the application.
		/// </summary>
		/// <param name="applicationRootFilter">An extension filter that selects the application root extension to execute.</param>
		/// <param name="args">The set of arguments passed from the command line.</param>
		/// <remarks>
		/// A ClearCanvas based application is started by calling this convenience method from
		/// a bootstrap executable of some kind.  Calling this method results in the loading
		/// of all plugins and creation of an <see cref="IApplicationRoot"/> extension.  
		/// This method is not thread-safe as it should only ever be invoked once per execution, by a single thread.
		/// </remarks>
		public static void StartApp(ExtensionFilter applicationRootFilter, string[] args)
		{
			FatalExceptionHandler.Initialize();

			var xp = new ApplicationRootExtensionPoint();
			_applicationRoot = (applicationRootFilter == null) ?
				(IApplicationRoot) xp.CreateExtension() :
				(IApplicationRoot) xp.CreateExtension(applicationRootFilter);
			_applicationRoot.RunApplication(args);
		}

		/// <summary>
		/// Starts the application.
		/// </summary>
		/// <remarks>
		/// A ClearCanvas based application is started by calling this convenience method from
		/// a bootstrap executable of some kind.  Calling this method results in the loading
		/// of all plugins and creation of an <see cref="IApplicationRoot"/> extension.  
		/// This method is not thread-safe as it should only ever be invoked once per execution, by a single thread.
		/// </remarks>
		public static void StartApp()
		{
			StartApp((ExtensionFilter) null, new string[] {});
		}

		/// <summary>
		/// Starts the application matching the specified fully or partially qualified class name.
		/// </summary>
		/// <param name="appRootClassName">The name of an application root class, which need not be fully qualified.</param>
		/// <param name="args"></param>
		public static void StartApp(string appRootClassName, string[] args)
		{
			var appRoots = new ApplicationRootExtensionPoint().ListExtensions();

			// try an exact match
			var matchingRoots = CollectionUtils.Select(appRoots, info => info.ExtensionClass.FullName == appRootClassName);

			if (matchingRoots.Count == 0)
			{
				// try a partial match
				matchingRoots = CollectionUtils.Select(appRoots,
				                                       info => info.ExtensionClass.FullName.EndsWith(appRootClassName, StringComparison.InvariantCultureIgnoreCase));
			}

			if (matchingRoots.Count == 0)
				throw new NotSupportedException(
					string.Format(SR.ExceptionApplicationRootNoMatches, appRootClassName));
			if (matchingRoots.Count > 1)
				throw new NotSupportedException(
					string.Format(SR.ExceptionApplicationRootMultipleMatches, appRootClassName));

			// start app
			StartApp(new ClassNameExtensionFilter(CollectionUtils.FirstElement(matchingRoots).ExtensionClass.FullName), args);
		}

		#endregion

		#region Service Provision

		/// <summary>
		/// Obtains an instance of the specified service for use by the application.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <typeparam name="TService">The type of service to obtain.</typeparam>
		/// <returns>An instance of the specified service.</returns>
		/// <exception cref="UnknownServiceException">The requested service cannot be provided.</exception>
		public static TService GetService<TService>()
		{
			return (TService) GetService(typeof (TService));
		}

		/// <summary>
		/// For use with the <see cref="GetService{TService}(WithServiceDelegate{TService})"/> method.
		/// </summary>
		public delegate void WithServiceDelegate<T>(T service);

		/// <summary>
		/// Obtains an instance of the specified service for use by the application.  
		/// </summary>
		/// <remarks>
		/// <para>
		/// Instead of returning the service directly, this overload passes the service to the specified delegate for use.
		/// When the delegate returns, this method automatically takes care of determing whether the service implements <see cref="IDisposable"/>
		/// and calling <see cref="IDisposable.Dispose"/> if it does.  The delegate must not cache the returned service
		/// because it may be disposed as soon as the delegate returns.  For the single-use scenario, this overload is preferred
		/// to the other overloads because it automatically manages the lifecycle of the service object.
		/// </para>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// </remarks>
		/// <typeparam name="TService">The service to obtain.</typeparam>
		/// <param name="proc">A delegate that will receive the service for one-time use.</param>
		public static void GetService<TService>(WithServiceDelegate<TService> proc)
		{
			var service = GetService<TService>();

			try
			{
				proc(service);
			}
			finally
			{
				if (service is IDisposable)
				{
					try
					{
						(service as IDisposable).Dispose();
					}
					catch (Exception e)
					{
						// do not allow exceptions thrown from Dispose() because it may have the effect of
						// hiding an exception that was thrown from the service itself
						// if the service fails to dispose properly, we don't care, just log it and move on
						Log(LogLevel.Error, e);
					}
				}
			}
		}

		/// <summary>
		/// Obtains an instance of the specified service for use by the application.  
		/// </summary>
		/// <remarks>
		/// <para>
		/// Instead of returning the service directly, this overload passes the service to the specified delegate for use.
		/// When the delegate returns, this method automatically takes care of determing whether the service implements <see cref="IDisposable"/>
		/// and calling <see cref="IDisposable.Dispose"/> if it does.  The delegate must not cache the returned service
		/// because it may be disposed as soon as the delegate returns.  For the single-use scenario, this overload is preferred
		/// to the other overloads because it automatically manages the lifecycle of the service object.
		/// </para>
		/// <para>
		/// This method is thread-safe.
		/// </para>
		/// </remarks>
		/// <typeparam name="TService">The service to obtain.</typeparam>
		/// <typeparam name="TResult">The type of the function result.</typeparam>
		/// <param name="func">A delegate that will receive the service for one-time use.</param>
		public static TResult GetService<TService, TResult>(Func<TService, TResult> func)
		{
			TResult result = default(TResult);
			GetService<TService>(svc => result = func.Invoke(svc));
			return result;
		}

		/// <summary>
		/// Obtains an instance of the specified service for use by the application.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <param name="service">The type of service to obtain.</param>
		/// <returns>An instance of the specified service.</returns>
		/// <exception cref="UnknownServiceException">The requested service cannot be provided.</exception>
		public static object GetService(Type service)
		{
			// load all service providers if not yet loaded
			if (_serviceProviders == null)
			{
				lock (_syncRoot)
				{
					if (_serviceProviders == null)
						_serviceProviders = new ServiceProviderExtensionPoint().CreateExtensions().Cast<IServiceProvider>().ToArray();
				}
			}

			// attempt to instantiate the requested service
			foreach (IServiceProvider sp in _serviceProviders)
			{
				// the service provider itself may not be thread-safe, so we need to ensure only one thread will access it
				// at a time
				lock (sp)
				{
					object impl = sp.GetService(service);
					if (impl != null)
						return impl;
				}
			}

			var message = string.Format("No service provider was found that can provide the service {0}.", service.FullName);
			throw new UnknownServiceException(message);
		}

		/// <summary>
		/// Obtains an instance of the specified duplex service for use by the application.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <typeparam name="TService">The type of service to obtain.</typeparam>
		/// <typeparam name="TCallback">The type of the callback contract.</typeparam>
		/// <param name="callback">An object that implements the callback contract.</param>
		/// <returns>An instance of the specified service.</returns>
		/// <exception cref="UnknownServiceException">The requested service cannot be provided.</exception>
		public static TService GetDuplexService<TService, TCallback>(TCallback callback)
		{
			return (TService) GetDuplexService(typeof (TService), callback);
		}

		/// <summary>
		/// Obtains an instance of the specified duplex service for use by the application.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		/// <param name="service">The type of service to obtain.</param>
		/// <param name="callback">An object implementing the callback service contract.</param>
		/// <returns>An instance of the specified service.</returns>
		/// <exception cref="UnknownServiceException">The requested service cannot be provided.</exception>
		public static object GetDuplexService(Type service, object callback)
		{
			CheckForNullReference(callback, "callback");

			// load all service providers if not yet loaded
			if (_duplexServiceProviders == null)
			{
				lock (_syncRoot)
				{
					if (_duplexServiceProviders == null)
						_duplexServiceProviders = (new DuplexServiceProviderExtensionPoint()).CreateExtensions().Cast<IDuplexServiceProvider>().ToArray();
				}
			}

			// attempt to instantiate the requested service
			foreach (IDuplexServiceProvider sp in _duplexServiceProviders)
			{
				// the service provider itself may not be thread-safe, so we need to ensure only one thread will access it
				// at a time
				lock (sp)
				{
					object impl = sp.GetService(service, callback);
					if (impl != null)
						return impl;
				}
			}

			var message = string.Format("No duplex service provider was found that can provide the service {0}.", service.FullName);
			throw new UnknownServiceException(message);
		}

		#endregion

		#region Logging

		/// <summary>
		/// Determines if the specified <see cref="LogLevel"/> is enabled.
		/// </summary>
		/// <param name="category">The logging level to check.</param>
		/// <returns>true if the <see cref="LogLevel"/> is enabled, or else false.</returns>
		public static bool IsLogLevelEnabled(LogLevel category)
		{
			return IsLogLevelEnabled((string) null, category);
		}

		/// <summary>
		/// Determines if the specified <see cref="LogLevel"/> is enabled for the named log.
		/// </summary>
		/// <param name="logName">The name of the log.</param>
		/// <param name="category">The logging level to check.</param>
		/// <returns>true if the <see cref="LogLevel"/> is enabled, or else false.</returns>
		public static bool IsLogLevelEnabled(string logName, LogLevel category)
		{
			var log = GetLog(logName);
			return IsLogLevelEnabled(log, category);
		}

		private static bool IsLogLevelEnabled(ILog log, LogLevel category)
		{
			switch (category)
			{
				case LogLevel.Debug:
					return log.IsDebugEnabled;
				case LogLevel.Info:
					return log.IsInfoEnabled;
				case LogLevel.Warn:
					return log.IsWarnEnabled;
				case LogLevel.Error:
					return log.IsErrorEnabled;
				case LogLevel.Fatal:
					return log.IsFatalEnabled;
			}

			return false;
		}

		/// <summary>
		/// Logs the specified message at the specified <see cref="LogLevel"/>.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <param name="category">The logging level.</param>
		/// <param name="message">The message to be logged.</param>
		public static void Log(LogLevel category, object message)
		{
			// Just return without formatting if the log level isn't enabled
			if (!IsLogLevelEnabled(category)) return;

			var ex = message as Exception;
			if (ex != null)
			{
				Log(_log, category, ex, null, null);
			}
			else
			{
				switch (category)
				{
					case LogLevel.Debug:
						_log.Debug(message);
						break;
					case LogLevel.Info:
						_log.Info(message);
						break;
					case LogLevel.Warn:
						_log.Warn(message);
						break;
					case LogLevel.Error:
						_log.Error(message);
						break;
					case LogLevel.Fatal:
						_log.Fatal(message);
						break;
				}
			}
		}

		/// <summary>
		/// Logs the specified message at the specified <see cref="LogLevel"/>.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <param name="category">The log level.</param>
		/// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
		/// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
		[StringFormatMethod("message")]
		public static void Log(LogLevel category, String message, params object[] args)
		{
			Log(_log, category, null, message, args);
		}

		/// <summary>
		/// Logs the specified exception at the specified <see cref="LogLevel"/>.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <param name="ex">The exception to log.</param>
		/// <param name="category">The log level.</param>
		/// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
		/// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
		[StringFormatMethod("message")]
		public static void Log(LogLevel category, Exception ex, String message, params object[] args)
		{
			Log(_log, category, ex, message, args);
		}

		/// <summary>
		/// Logs the specified message at the specified <see cref="LogLevel"/>, to the log with the specified name.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <param name="logName"> </param>
		/// <param name="category">The log level.</param>
		/// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
		/// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
		[StringFormatMethod("message")]
		public static void Log(string logName, LogLevel category, String message, params object[] args)
		{
			Log(logName, category, null, message, args);
		}

		/// <summary>
		/// Logs the specified exception at the specified <see cref="LogLevel"/>, to the log with the specified name.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		/// <param name="ex">The exception to log.</param>
		/// <param name="logName">A named log.</param>
		/// <param name="category">The log level.</param>
		/// <param name="message">Format message, as used with <see cref="System.Text.StringBuilder"/>.</param>
		/// <param name="args">Optional arguments used with <paramref name="message"/>.</param>
		[StringFormatMethod("message")]
		public static void Log(string logName, LogLevel category, Exception ex, String message, params object[] args)
		{
			Log(GetLog(logName), category, ex, message, args);
		}

		private static void Log(ILog log, LogLevel category, Exception ex, String message, object[] args)
		{
			if (IsLogLevelEnabled(log, category))
				Log(log, category, ex, GetLogMessage(ex != null, message, args));
		}

		private static void Log(ILog log, LogLevel category, Exception ex, string message)
		{
			if (log == null) return;

			if (ex == null)
			{
				switch (category)
				{
					case LogLevel.Debug:
						log.Debug(message);
						break;
					case LogLevel.Info:
						log.Info(message);
						break;
					case LogLevel.Warn:
						log.Warn(message);
						break;
					case LogLevel.Error:
						log.Error(message);
						break;
					case LogLevel.Fatal:
						log.Fatal(message);
						break;
				}
			}
			else
			{
				switch (category)
				{
					case LogLevel.Debug:
						log.Debug(message, ex);
						break;
					case LogLevel.Info:
						log.Info(message, ex);
						break;
					case LogLevel.Warn:
						log.Warn(message, ex);
						break;
					case LogLevel.Error:
						log.Error(message, ex);
						break;
					case LogLevel.Fatal:
						log.Fatal(message, ex);
						break;
				}
			}
		}

		private static string GetLogMessage(bool isExceptionLog, string message, params object[] args)
		{
			var sb = new StringBuilder();
			if (isExceptionLog)
			{
				sb.AppendLine("Exception thrown"); // note: it's log. Keep it in English
				sb.AppendLine();
			}

			if (!String.IsNullOrEmpty(message))
			{
				if (args == null || args.Length == 0)
					sb.Append(message);
				else
					sb.AppendFormat(message, args);
			}

			return sb.ToString();
		}

		private static ILog GetLog(string name)
		{
			if (String.IsNullOrEmpty(name))
				return _log;

			lock (_namedLogLock)
			{
				ILog log;
				if (!_namedLogs.TryGetValue(name, out log))
				{
					log = LogManager.GetLogger(name);
					if (log != null)
						_namedLogs[name] = log;
				}

				return log ?? _log;
			}
		}

		#endregion

		#region Message Boxes (obsolete)

		/// <summary>
		/// Displays a message box with the specified message.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe, however displaying message boxes from a thread other than a UI
		/// thread is not a recommended practice.
		/// </remarks>
		[Obsolete("Use DesktopWindow.ShowMessageBox instead", false)]
		public static void ShowMessageBox(string message)
		{
			ShowMessageBox(message, MessageBoxActions.Ok);
		}

		/// <summary>
		/// Displays a message box with the specified message and buttons, and returns a value indicating the action
		/// taken by the user.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe, however displaying message boxes from a thread other than a UI
		/// thread is not a recommended practice.
		/// </remarks>
		[Obsolete("Use DesktopWindow.ShowMessageBox instead", false)]
		public static DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
		{
			// create message box if does not exist
			if (_messageBox == null)
			{
				lock (_syncRoot)
				{
					if (_messageBox == null)
					{
						MessageBoxExtensionPoint xp = new MessageBoxExtensionPoint();
						_messageBox = (IMessageBox) xp.CreateExtension();
					}
				}
			}

			// must lock here, because we have no guarantee that the underlying _messageBox object is thread-safe
			// lock on the _messageBox itself, rather than _syncRoot, so that _syncRoot is free for other threads to lock on
			// (i.e the message box may block this thread for a long time, and all other threads would halt if we locked on _syncRoot)
			lock (_messageBox)
			{
				return _messageBox.Show(message, buttons);
			}
		}

		#endregion

		#region Validation Helpers

		/// <summary>
		/// Checks if a string is empty.
		/// </summary>
		/// <param name="value">The string to check.</param>
		/// <param name="variableName">The variable name of the string to checked.</param>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> or or <paramref name="variableName"/>
		/// is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> is zero length.</exception>
		public static void CheckForEmptyString(string value, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(value, variableName);
			CheckForNullReference(variableName, "variableName");
			if (value.Length == 0)
				throw new ArgumentException(String.Format(SR.ExceptionEmptyString, variableName));
		}

		/// <summary>
		/// Checks if an object reference is null.
		/// </summary>
		/// <param name="variable">The object reference to check.</param>
		/// <param name="variableName">The variable name of the object reference to check.</param>
		/// <remarks>Use for checking if an input argument is <b>null</b>.  To check if a member variable
		/// is <b>null</b> (i.e., to see if an object is in a valid state), use <see cref="CheckMemberIsSet(object,string)"/> instead.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="variableName"/>
		/// is <b>null</b>.</exception>
		public static void CheckForNullReference(object variable, [InvokerParameterName] string variableName)
		{
			if (variableName == null)
				throw new ArgumentNullException("variableName");

			if (null == variable)
				throw new ArgumentNullException(variableName);
		}

		/// <summary>
		/// Checks if an object is of the expected type.
		/// </summary>
		/// <param name="variable">The object to check.</param>
		/// <param name="type">The variable name of the object to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="type"/>
		/// is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> is not the expected type.</exception>
		public static void CheckExpectedType(object variable, Type type)
		{
			CheckForNullReference(variable, "variable");
			CheckForNullReference(type, "type");

			if (!type.IsInstanceOfType(variable))
				throw new ArgumentException(String.Format(SR.ExceptionExpectedType, type.FullName));
		}

		/// <summary>
		/// Checks if a cast is valid.
		/// </summary>
		/// <param name="castOutput">The object resulting from the cast.</param>
		/// <param name="castInputName">The variable name of the object that was cast.</param>
		/// <param name="castTypeName">The name of the type the object was cast to.</param>
		/// <remarks>
		/// <para>To use this method, casts have to be done using the <b>as</b> operator.  The
		/// method depends on failed casts resulting in <b>null</b>.</para>
		/// <para>This method has been deprecated since it does not actually perform any
		/// cast checking itself and entirely relies on correct usage (which is not apparent
		/// through the Visual Studio Intellisence feature) to function as an exception message
		/// formatter. The recommended practice is to use the <see cref="CheckExpectedType"/>
		/// if the cast output need not be consumed, or use the direct cast operator instead.</para>
		/// </remarks>
		/// <example>
		/// <code>
		/// [C#]
		/// layer = new GraphicLayer();
		/// GraphicLayer graphicLayer = layer as GraphicLayer;
		/// // No exception thrown
		/// Platform.CheckForInvalidCast(graphicLayer, "layer", "GraphicLayer");
		///
		/// ImageLayer image = layer as ImageLayer;
		/// // InvalidCastException thrown
		/// Platform.CheckForInvalidCast(image, "layer", "ImageLayer");
		/// </code>
		/// </example>
		/// <exception cref="ArgumentNullException"><paramref name="castOutput"/>,
		/// <paramref name="castInputName"/>, <paramref name="castTypeName"/> is <b>null</b>.</exception>
		/// <exception cref="InvalidCastException">Cast is invalid.</exception>
		[Obsolete("Use Platform.CheckExpectedType or perform a direct cast instead.")]
		public static void CheckForInvalidCast(object castOutput, string castInputName, string castTypeName)
		{
			CheckForNullReference(castOutput, "castOutput");
			CheckForNullReference(castInputName, "castInputName");
			CheckForNullReference(castTypeName, "castTypeName");

			if (castOutput == null)
				throw new InvalidCastException(String.Format(SR.ExceptionInvalidCast, castInputName, castTypeName));
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="n">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="n"/> &lt;= 0.</exception>
		public static void CheckPositive(int n, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (n <= 0)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is true.
		/// </summary>
		/// <param name="testTrueCondition">The value to check.</param>
		/// <param name="conditionName">The name of the condition to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="conditionName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="testTrueCondition"/> is  <b>false</b>.</exception>
		public static void CheckTrue(bool testTrueCondition, string conditionName)
		{
			CheckForNullReference(conditionName, "conditionName");

			if (!testTrueCondition)
				throw new ArgumentException(String.Format(SR.ExceptionConditionIsNotMet, conditionName));
		}

		/// <summary>
		/// Checks if a value is false.
		/// </summary>
		/// <param name="testFalseCondition">The value to check.</param>
		/// <param name="conditionName">The name of the condition to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="conditionName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="testFalseCondition"/> is  <b>true</b>.</exception>
		public static void CheckFalse(bool testFalseCondition, string conditionName)
		{
			CheckForNullReference(conditionName, "conditionName");

			if (testFalseCondition)
				throw new ArgumentException(String.Format(SR.ExceptionConditionIsNotMet, conditionName));
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="x">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="x"/> &lt;= 0.</exception>
		public static void CheckPositive(float x, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (x <= 0.0f)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="x">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="x"/> &lt;= 0.</exception>
		public static void CheckPositive(double x, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (x <= 0.0d)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is non-negative.
		/// </summary>
		/// <param name="n">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="n"/> &lt; 0.</exception>
		public static void CheckNonNegative(int n, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (n < 0)
				throw new ArgumentException(SR.ExceptionArgumentNegative, variableName);
		}

		/// <summary>
		/// Checks if a value is within a specified range.
		/// </summary>
		/// <param name="argumentValue">Value to be checked.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="variableName">Variable name of value to be checked.</param>
		/// <remarks>Checks if <paramref name="min"/> &lt;= <paramref name="argumentValue"/> &lt;= <paramref name="max"/></remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="argumentValue"/> is not within the
		/// specified range.</exception>
		public static void CheckArgumentRange(int argumentValue, int min, int max, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (argumentValue < min || argumentValue > max)
				throw new ArgumentOutOfRangeException(String.Format(SR.ExceptionArgumentOutOfRange, argumentValue, min, max, variableName));
		}

		/// <summary>
		/// Checks if an index is within a specified range.
		/// </summary>
		/// <param name="index">Index to be checked</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="obj">Object being indexed.</param>
		/// <remarks>Checks if <paramref name="min"/> &lt;= <paramref name="index"/> &lt;= <paramref name="max"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="obj"/> is <b>null</b>.</exception>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is not within the
		/// specified range.</exception>
		public static void CheckIndexRange(int index, int min, int max, object obj)
		{
			CheckForNullReference(obj, "obj");

			if (index < min || index > max)
				throw new IndexOutOfRangeException(String.Format(SR.ExceptionIndexOutOfRange, index, min, max, obj.GetType().Name));
		}

		/// <summary>
		/// Checks if a field or property is null.
		/// </summary>
		/// <param name="variable">Field or property to be checked.</param>
		/// <param name="variableName">Name of field or property to be checked.</param>
		/// <remarks>Use this method in your classes to verify that the object
		/// is not in an invalid state by checking that various fields and/or properties
		/// have been set, i.e., are not null.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="variable"/> is <b>null</b>.</exception>
		public static void CheckMemberIsSet(object variable, [InvokerParameterName] string variableName)
		{
			CheckForNullReference(variableName, "variableName");

			if (variable == null)
				throw new InvalidOperationException(String.Format(SR.ExceptionMemberNotSet, variableName));
		}

		/// <summary>
		/// Checks if a field or property is null.
		/// </summary>
		/// <param name="variable">Field or property to be checked.</param>
		/// <param name="variableName">Name of field or property to be checked.</param>
		/// <param name="detailedMessage">A more detailed and informative message describing
		/// why the object is in an invalid state.</param>
		/// <remarks>Use this method in your classes to verify that the object
		/// is not in an invalid state by checking that various fields and/or properties
		/// have been set, i.e., are not null.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="variable"/> is <b>null</b>.</exception>
		public static void CheckMemberIsSet(object variable, [InvokerParameterName] string variableName, string detailedMessage)
		{
			CheckForNullReference(variableName, "variableName");
			CheckForNullReference(detailedMessage, "detailedMessage");

			if (variable == null)
				throw new InvalidOperationException(String.Format(SR.ExceptionMemberNotSetVerbose, variableName, detailedMessage));
		}

		#endregion
	}
}