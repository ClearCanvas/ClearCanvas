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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Generic environment utilities.
	/// </summary>
	public static class EnvironmentUtilities
	{
		private static string _machineIdentifier;

		/// <summary>
		/// Gets a unique identifier for the machine.
		/// </summary>
		public static string MachineIdentifier
		{
			get
			{
				if (_machineIdentifier == null)
				{
					var input = string.Format("CLEARCANVASRTW::{0}::{1}::{2}::{3}::{4}", GetProcessorId(), GetMotherboardSerial(), GetSystemVolumeSerial(), GetBiosSerial(), GetSystemUuid());
					using (var sha256 = new SHA256CryptoServiceProvider2())
					{
						_machineIdentifier = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
					}
				}
				return _machineIdentifier;
			}
		}

		private static string GetProcessorId()
		{
			try
			{
				// read the CPUID of the first processor
				using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
				using (var results = new ManagementObjectSearcherResults(searcher))
				{
					foreach (var processor in results)
					{
						var processorId = processor.GetString("ProcessorId");
						if (processorId != null) processorId = processorId.Trim();
						if (!string.IsNullOrEmpty(processorId))
							return processorId;
					}
				}

				// if the processor doesn't support the CPUID opcode, concatenate some immutable characteristics of the processor
				using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, AddressWidth, Architecture, Family, Level, Revision FROM Win32_Processor"))
				using (var results = new ManagementObjectSearcherResults(searcher))
				{
					foreach (var processor in results)
					{
						var manufacturer = processor.GetString("Manufacturer");
						var addressWidth = processor.GetUInt16("AddressWidth");
						var architecture = processor.GetUInt16("Architecture");
						var family = processor.GetUInt16("Family");
						var level = processor.GetUInt16("Level");
						var revision = processor.GetUInt16("Revision");
						return string.Format(CultureInfo.InvariantCulture, "CPU-{0}-{1}-{2:X2}-{3:X2}-{4}-{5:X4}", manufacturer, addressWidth, architecture, family, level, revision);
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve processor ID.");
			}
			return string.Empty;
		}

		private static string GetBiosSerial()
		{
			try
			{
				// read the s/n of BIOS
				using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS"))
				using (var results = new ManagementObjectSearcherResults(searcher))
				{
					foreach (var bios in results)
					{
						var serialNumber = bios.GetString("SerialNumber");
						if (serialNumber != null) serialNumber = serialNumber.Trim();
						if (!string.IsNullOrEmpty(serialNumber))
							return serialNumber;
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve BIOS serial number.");
			}
			return string.Empty;
		}

		private static string GetMotherboardSerial()
		{
			try
			{
				// read the s/n of the motherboard
				using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
				using (var results = new ManagementObjectSearcherResults(searcher))
				{
					foreach (var motherboard in results)
					{
						var serialNumber = motherboard.GetString("SerialNumber");
						if (serialNumber != null) serialNumber = serialNumber.Trim();
						if (!string.IsNullOrEmpty(serialNumber))
							return serialNumber;
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve baseboard serial number.");
			}
			return string.Empty;
		}

		private static string GetSystemVolumeSerial()
		{
			try
			{
				// read the volume serial of the system drive
				var systemDrive = (Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) ?? "C:").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
				using (var searcher = new ManagementObjectSearcher(string.Format("SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID='{0}'", WqlEscape(systemDrive))))
				using (var results = new ManagementObjectSearcherResults(searcher))
				{
					foreach (var logicalDisk in results)
					{
						var volumeSerial = logicalDisk.GetString("VolumeSerialNumber");
						if (volumeSerial != null) volumeSerial = volumeSerial.Trim();
						if (!string.IsNullOrEmpty(volumeSerial))
							return volumeSerial;
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve system volume serial.");
			}
			return string.Empty;
		}

		private static string GetSystemUuid()
		{
			try
			{
				// read the UUID of the system
				using (var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
				using (var results = new ManagementObjectSearcherResults(searcher))
				{
					foreach (var system in results)
					{
						var uuid = system.GetString("UUID");
						if (uuid != null) uuid = uuid.Trim();
						if (!string.IsNullOrEmpty(uuid))
							return uuid;
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to retrieve system UUID.");
			}
			return string.Empty;
		}

		private static string GetString(this ManagementBaseObject wmiObject, string propertyName)
		{
			try
			{
				var value = wmiObject[propertyName];
				if (value != null)
					return value.ToString();
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, @"WMI property ""{0}"" was not in the expected format.", propertyName);
			}
			return null;
		}

		private static ushort? GetUInt16(this ManagementBaseObject wmiObject, string propertyName)
		{
			try
			{
				var value = wmiObject[propertyName];
				if (value != null)
					return (ushort) value;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, @"WMI property ""{0}"" was not in the expected format.", propertyName);
			}
			return null;
		}

		/// <summary>
		/// Escapes WQL WHERE clause string literals.
		/// </summary>
		/// <seealso cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa394054%28v=vs.85%29.aspx"/>
		private static string WqlEscape(string literal)
		{
			return string.IsNullOrEmpty(literal) ? literal : literal.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
		}

		/// <summary>
		/// Utility class for proper disposal of results from WMI queries.
		/// </summary>
		private class ManagementObjectSearcherResults : IEnumerable<ManagementBaseObject>, IDisposable
		{
			private List<ManagementBaseObject> _results;

			public ManagementObjectSearcherResults(ManagementObjectSearcher searcher)
			{
				// the result collection is a wrapper around a COM enumerator that constructs new COM objects per iteration
				// because of this, don't ever enumerate the collection directly - cache the results from one iteration and enumerate the cache instead
				_results = new List<ManagementBaseObject>();
				using (var results = searcher.Get())
				{
					foreach (var result in results)
						_results.Add(result);
				}
			}

			public void Dispose()
			{
				if (_results != null)
				{
					foreach (var result in _results)
						result.Dispose();
					_results.Clear();
					_results = null;
				}
			}

			public IEnumerator<ManagementBaseObject> GetEnumerator()
			{
				return _results.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}