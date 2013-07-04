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

#if UNIT_TESTS

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ClearCanvas.ImageViewer.Volumes.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	public abstract class AbstractMprTest : AbstractVolumeTest
	{
		protected AbstractMprTest()
		{
			// assert that the correct VTK assemblies have been copied to the unit testing directory, to aid in diagnosing nonsensical test failures
			AssertAssemblyLoadable("vtkCommonDotNet", "vtkCommon.dll");
			AssertAssemblyLoadable("vtkFilteringDotNet", "vtkFiltering.dll");
			AssertAssemblyLoadable("vtkImagingDotNet", "vtkImaging.dll");
		}

		/// <summary>
		/// Asserts the presence of referenced assemblies and/or additional dependencies.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly (e.g. vtkCommonDotNet)</param>
		/// <param name="dependencies">File names of additional dependencies to find at the location of the assembly (e.g. vtkCommon.dll)</param>
		protected static void AssertAssemblyLoadable(string assemblyName, params string[] dependencies)
		{
			Assembly assembly = null;

			try
			{
				// check if .NET will be able to load the assembly
				if (!string.IsNullOrEmpty(assemblyName))
					assembly = Assembly.ReflectionOnlyLoad(assemblyName);
			}
			catch (Exception ex)
			{
				Assert.Fail("The assembly '{0}' could not be loaded: {1}", assemblyName, ex.Message);
			}

			if (assembly != null)
			{
				// check processor architecture of the assembly matches unit test process
				var name = assembly.GetName();
				switch (name.ProcessorArchitecture)
				{
					case ProcessorArchitecture.MSIL:
						break;
					case ProcessorArchitecture.X86:
						Assert.True(IntPtr.Size == 4, "The assembly '{0}' is architecture={1} but the unit test process is running as {2}-bit", assemblyName, name.ProcessorArchitecture, IntPtr.Size*8);
						break;
					case ProcessorArchitecture.Amd64:
					case ProcessorArchitecture.IA64:
						Assert.True(IntPtr.Size == 8, "The assembly '{0}' is architecture={1} but the unit test process is running as {2}-bit", assemblyName, name.ProcessorArchitecture, IntPtr.Size*8);
						break;
					case ProcessorArchitecture.None:
					default:
						Trace.WriteLine(string.Format("The assembly '{0}' is architecture={1}", assembly, name.ProcessorArchitecture));
						break;
				}
			}

			// check for additional dependencies at the location of the assembly
			var baseDir = Path.GetDirectoryName(new Uri((assembly ?? Assembly.GetCallingAssembly()).CodeBase).AbsolutePath) ?? string.Empty;
			foreach (var dependency in dependencies ?? Enumerable.Empty<string>())
			{
				Assert.True(File.Exists(Path.Combine(baseDir, dependency)), "The dependency '{0}' could not be found next to assembly {1} ({2})", dependency, assemblyName, baseDir);
			}
		}
	}
}

#endif