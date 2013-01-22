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
				return plugins;
			}
		}

	}
}
