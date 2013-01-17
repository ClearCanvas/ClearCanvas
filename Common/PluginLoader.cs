using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	internal class PluginLoader
	{
		class PluginFile
		{
			public PluginFile(string pluginName, FileInfo fileInfo, PluginAttribute pluginAttribute, Assembly assembly)
			{
				PluginName = pluginName;
				FileInfo = fileInfo;
				PluginAttribute = pluginAttribute;
				Assembly = assembly;
			}

			public readonly string PluginName; //todo: this is weird
			public readonly FileInfo FileInfo;
			public readonly PluginAttribute PluginAttribute;
			public readonly Assembly Assembly;
		}

		private static readonly string _metadataCacheFileName = string.Format("{0}\\pyx.cache", Platform.ApplicationDataDirectory);
		private readonly string _pluginDir;

		public PluginLoader(string pluginDir)
		{
			_pluginDir = pluginDir;
		}

		public event EventHandler<PluginLoadedEventArgs> PluginProgressEvent;

		public List<PluginInfo> LoadPlugins()
		{
			EventsHelper.Fire(PluginProgressEvent, this, new PluginLoadedEventArgs(SR.MessageFindingPlugins, null));

			var pluginFiles = LoadPluginFiles();
			List<PluginInfo> pluginInfos;
			if (!LoadCachedMetadata(pluginFiles, out pluginInfos))
			{
				pluginInfos = BuildMetadata(pluginFiles);
				SaveCachedMetadata(pluginInfos);
			}

			return pluginInfos;
		}

		private List<PluginFile> LoadPluginFiles()
		{
			var plugins = new List<PluginFile>();
			FileProcessor.Process(_pluginDir, "*.dll", path =>
			{
				PluginFile file;
				if (LoadPlugin(path, out file))
				{
					plugins.Add(file);
					var e = new PluginLoadedEventArgs(string.Format(SR.FormatLoadedPlugin, file.PluginName), file.Assembly);
					EventsHelper.Fire(PluginProgressEvent, this, e);
				}
			}, true);

			return plugins;
		}

		private static bool LoadCachedMetadata(List<PluginFile> pluginFiles, out List<PluginInfo> pluginInfos)
		{
			// see if a cache file exists, and has not expired
			// (it is expired if it is older than any plugin file's last write time)
			var cacheFile = new FileInfo(_metadataCacheFileName);
			if (cacheFile.Exists && pluginFiles.All(p => p.FileInfo.LastWriteTime < cacheFile.LastWriteTime))
			{
				try
				{
					var pluginLookup = pluginFiles.ToDictionary(p => p.Assembly.GetName().Name, p => p.Assembly);
					pluginInfos = PluginInfoCache.Read(_metadataCacheFileName, name => pluginLookup[name]);
					return true;
				}
				catch (Exception)
				{
					// not a big deal, it just means the cache isn't accessible right now
					// (maybe another app domain or process is writing to it?)
					// and we need to build meta-data from the binaries
					// 
					//todo loc
					Platform.Log(LogLevel.Debug, "Failed to read metadata cache.");
				}
			}
			pluginInfos = null;
			return false;
		}

		private List<PluginInfo> BuildMetadata(IEnumerable<PluginFile> pluginFiles)
		{
			var results = new List<PluginInfo>();
			foreach (var p in pluginFiles)
			{
				var e = new PluginLoadedEventArgs(string.Format(SR.FormatProcessingPlugin, p.PluginName), p.Assembly);
				EventsHelper.Fire(PluginProgressEvent, this, e);

				// the metadata is built in the PluginInfo constructor
				var pi = new PluginInfo(p.Assembly, p.PluginAttribute.Name, p.PluginAttribute.Description, p.PluginAttribute.Icon);
				results.Add(pi);
			}
			return results;
		}


		private static void SaveCachedMetadata(List<PluginInfo> pluginInfos)
		{
			try
			{
				PluginInfoCache.Write(_metadataCacheFileName, pluginInfos);
			}
			catch (Exception)
			{
				// not a big deal, it just means the cache won't be updated this time around
				//todo loc
				Platform.Log(LogLevel.Debug, "Failed to write metadata cache.");
			}
		}

		private static bool LoadPlugin(string path, out PluginFile pluginFile)
		{
			var pluginName = Path.GetFileName(path);
			try
			{
				// load assembly
				var asm = Assembly.LoadFrom(path);

				// is it a plugin??
				var pluginAttr = (PluginAttribute)asm.GetCustomAttributes(typeof(PluginAttribute), false).FirstOrDefault();
				if (pluginAttr != null)
				{
					pluginFile = new PluginFile(pluginName, new FileInfo(path), pluginAttr, asm);
					return true;
				}
			}
			catch (BadImageFormatException e)
			{
				// unmanaged DLL in the plugin directory
				Platform.Log(LogLevel.Debug, SR.LogFoundUnmanagedDLL, e.FileName);
			}
			catch (ReflectionTypeLoadException e)
			{
				// this exception usually means one of the dependencies is missing
				Platform.Log(LogLevel.Error, SR.LogFailedToProcessPluginAssembly, pluginName);

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

			pluginFile = null;
			return false;
		}
	}
}
