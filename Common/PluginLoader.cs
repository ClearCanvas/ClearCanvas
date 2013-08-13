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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Encapsulates logic for loading plugins and plugin meta-data from disk.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the framework and is not intended for application use.
	/// </remarks>
	internal class PluginLoader
	{
		private class LoadPluginResult
		{
			public LoadPluginResult(bool isPlugin, Assembly assembly, PluginInfo pluginInfo)
			{
				IsPlugin = isPlugin;
				Assembly = assembly;
				PluginInfo = pluginInfo;
			}

			public readonly bool IsPlugin;
			public readonly Assembly Assembly;
			public readonly PluginInfo PluginInfo;
		}

		private readonly string _primaryCacheFile;
		private readonly string[] _alternateCacheFiles;
		private readonly string _pluginDir;

		internal PluginLoader(string pluginDir, string primaryCacheFile, string[] alternateCacheFiles)
		{
			_pluginDir = pluginDir;
			_primaryCacheFile = primaryCacheFile;
			_alternateCacheFiles = alternateCacheFiles;
		}

		/// <summary>
		/// Occurs when a plugin assembly was loaded into memory.
		/// </summary>
		internal event EventHandler<PluginLoadedEventArgs> PluginLoaded;

		/// <summary>
		/// Loads plugin meta-data, without necessarily loading the plugin assemblies.
		/// </summary>
		/// <remarks>
		/// Calling this method may load the plugin assemblies into memory, but only if cached meta-data cannot be found.
		/// </remarks>
		internal List<PluginInfo> LoadPluginInfo()
		{
			// build list of candidate plugin files
			// Note: the reason for the "group by" operation is that some non-plugin files
			// may have the same file name (located in different sub-folders) - e.g. localization satellite assemblies in ASP.NET,
			// and we need to eliminate these prior to building the pluginPathLookup dictionary below.
			var pluginCandidates = (from p in ListPluginCandidateFiles()
			                        group p by Path.GetFileNameWithoutExtension(p)
			                        into g
			                        where g.Count() == 1
			                        select g.First())
				.ToList();

			// establish assembly load resolver
			var pluginPathLookup = BuildAssemblyMap(pluginCandidates);
			AssemblyRef.SetResolver(name => LoadPlugin(pluginPathLookup[name], false).Assembly);

			// see if we can load the meta-data from a cache
			var checkSum = ComputeCheckSum(pluginCandidates);
			List<PluginInfo> pluginInfos;
			if (!TryLoadCachedMetadata(new[] {_primaryCacheFile}.Concat(_alternateCacheFiles), checkSum, out pluginInfos))
			{
				// No cached meta-data, so we need to load the plugins
				// and build the meta-data from scratch.
				LoadPluginFiles(pluginCandidates, true, out pluginInfos);
				SaveCachedMetadata(pluginInfos, checkSum);
			}

			return pluginInfos;
		}

		private List<string> ListPluginCandidateFiles()
		{
			var plugins = new List<string>();
			FileProcessor.Process(_pluginDir, "*.dll", plugins.Add, true);
			return plugins;
		}

		private void LoadPluginFiles(IEnumerable<string> pluginCandidates, bool processMetadata, out List<PluginInfo> pluginInfos)
		{
			EventsHelper.Fire(PluginLoaded, this, new PluginLoadedEventArgs(SR.MessageFindingPlugins, null));

			var loadResults = pluginCandidates.Select(pc => LoadPlugin(pc, processMetadata)).ToList();

			pluginInfos = processMetadata ? loadResults.Where(r => r.IsPlugin).Select(r => r.PluginInfo).ToList() : null;
		}

		private static bool TryLoadCachedMetadata(IEnumerable<string> cacheFilePaths, byte[] checkSum, out List<PluginInfo> pluginInfos)
		{
			cacheFilePaths = cacheFilePaths.Where(File.Exists).ToList();
			if (cacheFilePaths.Any())
			{
				try
				{
					var pluginInfoCache = PluginInfoCache.Read(cacheFilePaths.First());
					if (pluginInfoCache.CheckSum.SequenceEqual(checkSum))
					{
						pluginInfos = pluginInfoCache.Plugins;
						return true;
					}
				}
				catch (Exception)
				{
					// not a big deal, it just means the cache isn't accessible right now
					// (maybe another app domain or process is writing to it?)
					// and we need to build meta-data from the binaries
					Platform.Log(LogLevel.Debug, "Failed to read plugin metadata cache.");
				}
			}
			pluginInfos = null;
			return false;
		}

		private void SaveCachedMetadata(List<PluginInfo> pluginInfos, byte[] checkSum)
		{
			try
			{
				var pluginInfoCache = new PluginInfoCache(pluginInfos, checkSum);
				pluginInfoCache.Write(_primaryCacheFile);
			}
			catch (Exception)
			{
				// not a big deal, it just means the cache won't be updated this time around
				Platform.Log(LogLevel.Debug, "Failed to write plugin metadata cache.");
			}
		}

		private LoadPluginResult LoadPlugin(string path, bool processMetadata)
		{
			try
			{
				// load assembly
				var asm = Assembly.LoadFrom(path);

				// is it a plugin??
				var pluginAttr = (PluginAttribute) asm.GetCustomAttributes(typeof (PluginAttribute), false).FirstOrDefault();
				if (pluginAttr == null)
					return new LoadPluginResult(false, asm, null);

				var fileName = Path.GetFileName(path);
				Platform.Log(LogLevel.Debug, "Loaded plugin {0}", fileName);
				var e = new PluginLoadedEventArgs(string.Format(SR.FormatLoadedPlugin, fileName), asm);
				EventsHelper.Fire(PluginLoaded, this, e);

				// do not create a PluginInfo unless explicitly asked for, because it is expensive
				var pluginInfo = processMetadata ? new PluginInfo(asm, pluginAttr.Name, pluginAttr.Description, pluginAttr.Icon) : null;
				return new LoadPluginResult(true, asm, pluginInfo);
			}
			catch (BadImageFormatException e)
			{
				// unmanaged DLL in the plugin directory
				Platform.Log(LogLevel.Debug, SR.LogFoundUnmanagedDLL, e.FileName);
			}
			catch (ReflectionTypeLoadException e)
			{
				// this exception usually means one of the dependencies is missing
				Platform.Log(LogLevel.Error, SR.LogFailedToProcessPluginAssembly, Path.GetFileName(path));

				// log a detail message for each missing dependency
				foreach (var loaderException in e.LoaderExceptions)
				{
					// just log the message, don't need the full stack trace
					Platform.Log(LogLevel.Error, loaderException.Message);
				}
			}
			catch (FileNotFoundException e)
			{
				Platform.Log(LogLevel.Error, e, "File not found while loading plugin: {0}", path);
			}
			catch (Exception e)
			{
				// there was a problem processing this assembly
				Platform.Log(LogLevel.Error, e, SR.LogFailedToProcessPluginAssembly, path);
			}

			return new LoadPluginResult(false, null, null);
		}

		private static byte[] ComputeCheckSum(IEnumerable<string> pluginCandidatePaths)
		{
			// include config files in the check sum
			var configFiles = Directory.EnumerateFiles(Platform.InstallDirectory, "*.exe.config", SearchOption.TopDirectoryOnly);
			configFiles = configFiles.Concat(Directory.EnumerateFiles(Platform.InstallDirectory, "*.exe.critical.config", SearchOption.TopDirectoryOnly));

			var orderedFiles = configFiles.Concat(pluginCandidatePaths).Select(f => new FileInfo(f)).OrderBy(fi => fi.FullName);

			// generate a checksum based on the name, create time, and last write time of each file
			using (var byteStream = new MemoryStream())
			using (var hash = new SHA256CryptoServiceProvider2())
			{
				foreach (var fi in orderedFiles)
				{
					var name = Encoding.Unicode.GetBytes(fi.Name);
					byteStream.Write(name, 0, name.Length);

					var createTime = BitConverter.GetBytes(fi.CreationTimeUtc.Ticks);
					byteStream.Write(createTime, 0, createTime.Length);

					var writeTime = BitConverter.GetBytes(fi.LastWriteTimeUtc.Ticks);
					byteStream.Write(writeTime, 0, createTime.Length);
				}
				return hash.ComputeHash(byteStream.GetBuffer());
			}
		}

		private static Dictionary<string, string> BuildAssemblyMap(IEnumerable<string> filePaths)
		{
			var result = new Dictionary<string, string>();
			foreach (var p in filePaths)
			{
				try
				{
					var name = AssemblyName.GetAssemblyName(p).Name;
					result.Add(name, p);
				}
				catch (Exception)
				{
					// not an assembly
					Platform.Log(LogLevel.Debug, "The file at {0} does not seem to be an assembly.", p);
				}
			}
			return result;
		}
	}
}