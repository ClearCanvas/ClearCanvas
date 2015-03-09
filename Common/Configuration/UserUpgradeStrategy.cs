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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Policy;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	public interface IUserUpgradeStrategy
	{
		void Run();
		bool IsRunning { get; }

		int TotalSteps { get; }
		int CurrentStep { get; }

		int SuccessfulCount { get; }
		int FailedCount { get; }
		int RemainingCount { get; }

		event EventHandler ProgressChanged;
	}

	public class UserUpgradeStrategy : IUserUpgradeStrategy
	{
		private event EventHandler _progressChanged;

		private UserUpgradeStrategy(ICollection<UserUpgradeStep> steps)
		{
			Steps = steps;
		}

		private ICollection<UserUpgradeStep> Steps { get; set; }

		public int TotalSteps
		{
			get { return Steps.Count; }
		}

		public int CurrentStep { get; private set; }

		public int SuccessfulCount { get; private set; }
		public int FailedCount { get; private set; }
		public int RemainingCount { get; private set; }

		public event EventHandler ProgressChanged
		{
			add { _progressChanged += value; }
			remove { _progressChanged -= value; }
		}

		public static IUserUpgradeStrategy Create()
		{
			if (!UpgradeSettings.IsUserUpgradeEnabled())
				return null;

			ICollection<UserUpgradeStep> steps = UserUpgradeStep.CreateAll();
			if (steps.Count == 0)
				return null;

			return new UserUpgradeStrategy(steps);
		}

		public bool IsRunning { get; private set; }

		public void Run()
		{
			if (IsRunning)
				return;

			IsRunning = true;

			// if we know of an alternate app settings folder where a user.config might be found in previous versions, try to migrate it if necessary
			var formerAppConfigFolder = GetAlternateAppSettingsFolder();
			if (!string.IsNullOrWhiteSpace(formerAppConfigFolder))
			{
				MigrateFormerAppConfigFile(true, formerAppConfigFolder);
				MigrateFormerAppConfigFile(false, formerAppConfigFolder);
			}

			foreach (UserUpgradeStep step in Steps)
			{
				try
				{
					++CurrentStep;
					step.Run();
					++SuccessfulCount;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "User upgrade step failed: {0}", step.Identifier);
					++FailedCount;
				}
				finally
				{
					--RemainingCount;
					EventsHelper.Fire(_progressChanged, this, EventArgs.Empty);
				}
			}

			IsRunning = false;
			EventsHelper.Fire(_progressChanged, this, EventArgs.Empty);
		}

		private static void MigrateFormerAppConfigFile(bool roamingProfile, params string[] formerAppConfigFolders)
		{
			try
			{
				// get the current version of the entry assembly (i.e. executable)
				var currentVersionString = Assembly.GetEntryAssembly().GetName().Version.ToString();

				// if the entry assembly isn't even versioned, upgrading settings doesn't make any sense, so just stop here
				if (String.IsNullOrWhiteSpace(currentVersionString)) return;
				var currentVersion = new Version(currentVersionString.Trim());

				// get the base user configuration path for the entry assembly
				var configuration = ConfigurationManager.OpenExeConfiguration(roamingProfile ? ConfigurationUserLevel.PerUserRoaming : ConfigurationUserLevel.PerUserRoamingAndLocal);
				var appConfigPath = Path.GetDirectoryName(Path.GetDirectoryName(configuration.FilePath));
				var baseConfigPath = Path.GetDirectoryName(appConfigPath);

				// the .NET implementation for getting previous version's config file just checks for existence of highest version directory before current version
				// it doesn't even care whether or not a user.config file actually exists in the directory, just that the directory with correct name format is there
				// thus, we won't check for existence of user.config either for determining whether or not we need to migrate from former directories

				// check all subdirectories of the current app config path for one which represents a version earlier than the current version
				// if such a folder exists, then it will be used as the previous version config file, so we are done here
				if (String.IsNullOrWhiteSpace(appConfigPath) || String.IsNullOrWhiteSpace(baseConfigPath)
				    || (Directory.Exists(appConfigPath) && Directory.GetDirectories(appConfigPath).Any(x =>
				                                                                                       	{
				                                                                                       		Version version;
				                                                                                       		return Version.TryParse(Path.GetFileName(x), out version) && version < currentVersion;
				                                                                                       	}))) return;

				// if an earlier version doesn't exist, check the former app settings folder names (former names usually happen due to change of CLR version)
				foreach (var formerAppConfigFolder in formerAppConfigFolders)
				{
					var formerAppConfigPath = Path.Combine(baseConfigPath, formerAppConfigFolder);
					if (!Directory.Exists(formerAppConfigPath)) continue;

					// enumerate the subdirectories of the former app config path for the highest version prior to current version that has a user.config file
					const string userConfig = "user.config";
					foreach (var previousVersion in Directory.GetDirectories(formerAppConfigPath).Select(x =>
					                                                                                     	{
					                                                                                     		Version version;
					                                                                                     		return Version.TryParse(Path.GetFileName(x), out version) && version < currentVersion ? version : null;
					                                                                                     	}).Where(x => x != null).OrderByDescending(x => x))
					{
						var formerConfigFile = Path.Combine(formerAppConfigPath, previousVersion.ToString().Trim(), userConfig);
						if (File.Exists(formerConfigFile))
						{
							// copy the user.config for the highest previous version in one of the former directories (respecting order of preference) to the current app config path
							// we keep the previous version, so that when .NET tries to get previous config file, it will see this copy as the previous version (which it was, but in another CLR)
							var migratedConfigPath = Path.Combine(appConfigPath, previousVersion.ToString().Trim());
							var migratedConfigFile = Path.Combine(migratedConfigPath, userConfig);
							Directory.CreateDirectory(migratedConfigPath);
							File.Copy(formerConfigFile, migratedConfigFile);
							Platform.Log(LogLevel.Debug, "Migrated previous version user settings from a different app directory\r\nSource: {0}\r\nDestination: {1}", formerConfigFile, migratedConfigFile);
							return;
						}
					}
				}
			}
			catch (Exception ex)
			{
				// if any exception is thrown, just log and continue
				Platform.Log(LogLevel.Debug, ex, "Failure while attempting to migrate previous version user settings in a different app directory");
			}
		}

		/// <summary>
		/// Determines an alternate application settings folder for the current app domain.
		/// </summary>
		private static string GetAlternateAppSettingsFolder()
		{
			try
			{
				var appDomainEvidence = AppDomain.CurrentDomain.Evidence;
				var configurationFilePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
				var previousInstallDir = UpgradeSettings.Default.PreviousInstallDir;
				return GetAlternateAppSettingsFolder(previousInstallDir, appDomainEvidence, configurationFilePath);
			}
			catch (Exception ex)
			{
				// if any exception is thrown, just log and continue
				Platform.Log(LogLevel.Debug, ex, "Failure while attempting to determine an alternate application settings directory");
				return null;
			}
		}

		/// <summary>
		/// Determines an alternate application settings folder for the provided details. (This overload really only exists for unit test purposes)
		/// </summary>
		internal static string GetAlternateAppSettingsFolder(string previousInstallDir, Evidence appDomainEvidence, string configurationFilePath)
		{
			try
			{
                //NOTE: the GetHostEvidence<T> method is not implemented on Mono, and this will throw an exception.

				// if the strong name evidence is available for the appdomain, it would've been used and there wouldn't be an 'alternate'
				if (appDomainEvidence.GetHostEvidence<StrongName>() != null) return null;

				// get the current app settings folder name
				var currentAppConfigFolder = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(configurationFilePath)));
				if (string.IsNullOrWhiteSpace(currentAppConfigFolder) || currentAppConfigFolder.Length <= 32) return null;

				// check that it uses the 'Url' type and strip off the hash
				var baseAppConfigFolder = currentAppConfigFolder.Substring(0, currentAppConfigFolder.Length - 32);
				if (!baseAppConfigFolder.EndsWith("_Url_", StringComparison.InvariantCultureIgnoreCase)) return null;

				// get the url evidence for the appdomain
				var urlEvidence = appDomainEvidence.GetHostEvidence<Url>();
				if (urlEvidence == null) return null;

				// if a previous install directory is known, figure out what the url evidence would have looked like for the same executable in that directory
				// otherwise, just assume it would be same as the current url evidence
				var evidenceInfo = string.IsNullOrWhiteSpace(previousInstallDir) ? urlEvidence.Value : "file:///" + previousInstallDir.TrimEnd('/', '\\') + '/' + Path.GetFileName(new Uri(urlEvidence.Value).LocalPath);

				// normalize the evidence info: this is what changed between CLR 2.0 and 4.0 - the choice of slash type!
				// if the slash behavior changes again in a future CLR, this code may need to be modified to compute and try multiple possibilities
				evidenceInfo = evidenceInfo.Replace('\\', '/').ToUpperInvariant();

				// now concatenate the base folder name with the evidence hash to get the 'alternate' folder name, and return it if it's actually different
				var formerAppConfigFolder = baseAppConfigFolder + ComputeEvidenceHash(evidenceInfo);
				return formerAppConfigFolder != currentAppConfigFolder ? formerAppConfigFolder : null;
			}
			catch (Exception ex)
			{
				// if any exception is thrown, just log and continue
				Platform.Log(LogLevel.Debug, ex, "Failure while attempting to determine an alternate application settings directory");
				return null;
			}
		}

		/// <summary>
		/// Takes the SHA-1 hash of the evidence info and generates a Base32 string exactly as .NET computes it
		/// </summary>
		internal static string ComputeEvidenceHash(string evidenceInfo)
		{
			const string base32Alphabet = "abcdefghijklmnopqrstuvwxyz012345";
			const int maskLow5Bits = 31; //   00011111
			const int maskMid2Bits = 96; //   01100000
			const int maskHigh3Bits = 224; // 11100000
			const int maskHigh1Bit = 128; //  10000000

			using (var ms = new MemoryStream())
			using (var sha1 = new SHA1CryptoServiceProvider())
			{
				new BinaryFormatter().Serialize(ms, evidenceInfo); // serialize the evidence string to binary form
				var hash = sha1.ComputeHash(ms.ToArray()); // a SHA1 hash has 20 bytes
				var base32String = new char[20*8/5]; // Base32 is 5 bits per char
				for (var n = 0; n < 4; ++n) // process in 4 blocks of 5 bytes (= 8 Base32 digits)
				{
					byte b0 = hash[5*n + 0], b1 = hash[5*n + 1], b2 = hash[5*n + 2], b3 = hash[5*n + 3], b4 = hash[5*n + 4];
					base32String[8*n + 0] = base32Alphabet[b0 & maskLow5Bits];
					base32String[8*n + 1] = base32Alphabet[b1 & maskLow5Bits];
					base32String[8*n + 2] = base32Alphabet[b2 & maskLow5Bits];
					base32String[8*n + 3] = base32Alphabet[b3 & maskLow5Bits];
					base32String[8*n + 4] = base32Alphabet[b4 & maskLow5Bits];
					base32String[8*n + 5] = base32Alphabet[(b0 & maskHigh3Bits) >> 5 | (b3 & maskMid2Bits) >> 2];
					base32String[8*n + 6] = base32Alphabet[(b1 & maskHigh3Bits) >> 5 | (b4 & maskMid2Bits) >> 2];
					base32String[8*n + 7] = base32Alphabet[(b2 & maskHigh3Bits) >> 5 | (b3 & maskHigh1Bit) >> 4 | (b4 & maskHigh1Bit) >> 3];
				}
				return new string(base32String);
			}
		}
	}
}