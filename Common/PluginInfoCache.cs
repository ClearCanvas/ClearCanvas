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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClearCanvas.Common
{
	[Serializable]
	internal class PluginInfoCache
	{
		#region SurrogateSelector class

		/// <summary>
		/// This class supports serialization of <see cref="TypeRef"/> and <see cref="AssemblyRef"/> objects,
		/// via their respective serialization surrogate classes.
		/// </summary>
		public class SurrogateSelector : ISurrogateSelector
		{
			public void ChainSelector(ISurrogateSelector selector)
			{
				throw new NotImplementedException();
			}

			public ISurrogateSelector GetNextSelector()
			{
				throw new NotImplementedException();
			}

			public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
			{
				if (type == typeof(TypeRef))
				{
					selector = this;
					return new TypeRef.SerializationSurrogate();
				}

				if (type == typeof(AssemblyRef))
				{
					selector = this;
					return new AssemblyRef.SerializationSurrogate();
				}

				selector = null;
				return null;
			}

		}

		#endregion

		private readonly List<PluginInfo> _plugins;
		private readonly byte[] _checkSum;

		internal PluginInfoCache(List<PluginInfo> plugins, byte[] checkSum)
		{
			_plugins = plugins;
			_checkSum = checkSum;
		}

		public List<PluginInfo> Plugins
		{
			get { return _plugins; }
		}

		public byte[] CheckSum
		{
			get { return _checkSum; }
		}

		public static PluginInfoCache Read(string file)
		{
			// Attempt to open the file for read access, and *non-exclusively*...
			// This is important because other app domains, or other processes, may need to read the file at the same time.
			// If we can't get access to the file (e.g. another process is writing to it), an exception will be thrown and this method will fail.
			// This is by design.
			using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// note: we could have done some custom serialization here, but BinaryFormatter
				// is easy and actually performs well enough for our purposes
				var formatter = new BinaryFormatter { SurrogateSelector = new SurrogateSelector() };
				return (PluginInfoCache)formatter.Deserialize(fs);
			}
		}

		public void Write(string file)
		{
			// ensure the path exists, in case this is the first time we're writing out the file
			var directory = Path.GetDirectoryName(file);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			// Attempt to open the file for write access, and *exclusively*...
			// If we can't get an exclusive lock on the file, an exception will be thrown and this method will fail.
			// This is by design.
			using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				// note: we could have done some custom serialization here, but BinaryFormatter
				// is easy and actually performs well enough for our purposes
				var formatter = new BinaryFormatter {SurrogateSelector = new SurrogateSelector()};
				formatter.Serialize(fs, this);
			}
		}


	}
}
