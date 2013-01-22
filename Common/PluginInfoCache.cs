using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClearCanvas.Common
{
	internal class PluginInfoCache
	{
		public static void Write(string file, List<PluginInfo> plugins)
		{
			// Attempt to open the file for write access, and *exclusively*...
			// If we can't get an exclusive lock on the file, an exception will be thrown and this method will fail.
			// This is by design.
			using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				// note: we could have done some custom serialization here, but BinaryFormatter
				// is easy and actually performs well enough for our purposes
				var formatter = new BinaryFormatter();
				formatter.Serialize(fs, plugins);
			}
		}

		public static List<PluginInfo> Read(string file, Func<string, Assembly> assemblyResolver)
		{
			var sw = new Stopwatch();

			Platform.Log(LogLevel.Info, "PluginInfoCache.Read:BEGIN {0}", sw.ElapsedMilliseconds);
			sw.Start();

			// Attempt to open the file for write access, and *non-exclusively*...
			// This is important because other app domains, or other processes, may need to read the file at the same time.
			// If we can't get access to the file (e.g. another process is writing to it), an exception will be thrown and this method will fail.
			// This is by design.
			using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// note: we could have done some custom serialization here, but BinaryFormatter
				// is easy and actually performs well enough for our purposes
				var formatter = new BinaryFormatter();
				var plugins = (List<PluginInfo>)formatter.Deserialize(fs);

				foreach (var plugin in plugins)
				{
					plugin.Assembly.SetResolver(assemblyResolver);
				}

				Platform.Log(LogLevel.Info, "PluginInfoCache.Read:END {0}", sw.ElapsedMilliseconds);
				return plugins;
			}
		}

	}
}
