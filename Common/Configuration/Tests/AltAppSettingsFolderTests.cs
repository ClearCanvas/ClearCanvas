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

#if UNIT_TESTS && !__MonoCS__

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using ClearCanvas.Common.Utilities.Tests;
using NUnit.Framework;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	internal class AltAppSettingsFolderTests
	{
		/// <summary>
		/// Tests known evidence hashes gathered from the field.
		/// </summary>
		[Test]
		public void TestKnownEvidenceHashes()
		{
			Assert.AreEqual("bxushgmob2kgr2bbhnkomw10ycdtxyd1", UserUpgradeStrategy.ComputeEvidenceHash(@"file:///c:/program files/clearcanvas/clearcanvas workstation/clearcanvas.desktop.executable.exe".ToUpperInvariant()));
			Assert.AreEqual("h4r1xj22u5jcc205qirm1uq5sdyih1a5", UserUpgradeStrategy.ComputeEvidenceHash(@"file:///c:\Program Files\ClearCanvas\ClearCanvas Workstation\ClearCanvas.Desktop.Executable.exe".ToUpperInvariant()));
		}

		/// <summary>
		/// Tests a large number of randomly generated inputs against a reference implementation of the evidence hashing function.
		/// </summary>
		[Test]
		public void TestRandomEvidenceHashes()
		{
			var rng = new PseudoRandom(0x262E5635);
			var data = new byte[512];

			using (var sha1 = new SHA1CryptoServiceProvider())
			using (var ms = new MemoryStream())
			{
				var bf = new BinaryFormatter();
				for (var n = 0; n < 10000; ++n)
				{
					rng.NextBytes(data);
					var evidenceInfo = Convert.ToBase64String(data);

					ms.SetLength(0);
					bf.Serialize(ms, evidenceInfo);
					var expectedHash = ToBase32(sha1.ComputeHash(ms.ToArray()));
					var actualHash = UserUpgradeStrategy.ComputeEvidenceHash(evidenceInfo);

					Assert.AreEqual(expectedHash, actualHash, "n = {0}", n);
				}
			}
		}

		/// <summary>
		/// Tests scenario where entry assembly was strong named - evidence is not path based, so there's no alt folder
		/// </summary>
		[Test]
		public void TestStrongNameEvidence()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Laboratories\ExperimentalRats";
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_abcdefghijklmnopqrstuvwxyz012345\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:/Program Files/ACME Laboratories/ExperimentalRats/TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, true);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.IsNullOrEmpty(altFolder, "strong named entry assembly should not have an alt folder");
		}

		/// <summary>
		/// Tests scenario where some non-url evidence was used - we don't have any data points to demonstrate what should happen here, so we didn't implement it
		/// </summary>
		[Test]
		public void TestUnsupportedEvidence()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Laboratories\ExperimentalRats";
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_abcdefghijklmnopqrstuvwxyz012345\3.0.0.0\user.config";

			var evidence = new Evidence();
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.IsNullOrEmpty(altFolder, "app domain without url in evidence is unusual, and we don't have any data points to back up any implementation, so we don't implement it!");
		}

		/// <summary>
		/// Tests that assembly URL is properly normalized to be case insensitive.
		/// </summary>
		[Test]
		public void TestPathCaseInsensitive()
		{
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_abcdefghijklmnopqrstuvwxyz012345\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:/PROGRAM Files/ACME Laboratories/ExperimentalRats/TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(null, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0", altFolder, "assembly url should be case insensitive");
		}

		/// <summary>
		/// Tests that assembly URL is properly normalized to be case insensitive.
		/// </summary>
		[Test]
		public void TestPathCaseInsensitive2()
		{
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_abcdefghijklmnopqrstuvwxyz012345\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"FILE:///c:/program Files/ACME Laboratories/ExperimentalRats/thebrain.EXE";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(null, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0", altFolder, "assembly url should be case insensitive");
		}

		/// <summary>
		/// Tests scenario where the current path is just different from what we expected the old path to be
		/// </summary>
		[Test]
		public void TestDistinctAltFolder()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Laboratories\ExperimentalRats";
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_abcdefghijklmnopqrstuvwxyz012345\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:/Program Files/ACME Laboratories/ExperimentalRats/TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0", altFolder, "getting the old path when the current path is based on some unknown (in this case, dummy) hashing");
		}

		/// <summary>
		/// Tests scenario where the current path is the same as what we compute the old path to be
		/// </summary>
		[Test]
		public void TestSameAsCurrentFolder()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Laboratories\ExperimentalRats";
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:/Program Files/ACME Laboratories/ExperimentalRats/TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.IsNullOrEmpty(altFolder, "should return null when alt path is exactly the same as current path");
		}

		/// <summary>
		/// Tests scenario where the current path is the same as what we compute the old path to be (previous install directory is unknown)
		/// </summary>
		[Test]
		public void TestSameAsCurrentFolderNoPreviousInstallDirectory()
		{
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:/Program Files/ACME Laboratories/ExperimentalRats/TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(null, evidence, appDataPath);
			Assert.IsNullOrEmpty(altFolder, "should return null when alt path is exactly the same as current path");
		}

		/// <summary>
		/// Tests the .NET 4.0 over .NET 2.0 upgrade scenario - backslashes used instead of forward slashes
		/// </summary>
		[Test]
		public void TestAltFolderOnSlashes()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Laboratories\ExperimentalRats";
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_2vbxhp2udm4o5dcwujyvygdqgjfgjfvn\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:\Program Files\ACME Laboratories\ExperimentalRats\TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0", altFolder, "getting the old path (with forward slashes) if the current path is based on backslashes");
		}

		/// <summary>
		/// Tests the .NET 4.0 over .NET 2.0 upgrade scenario - backslashes used instead of forward slashes (previous install directory is unknown)
		/// </summary>
		[Test]
		public void TestAltFolderOnSlashesNoPreviousInstallDirectory()
		{
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_2vbxhp2udm4o5dcwujyvygdqgjfgjfvn\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:\Program Files\ACME Laboratories\ExperimentalRats\TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(null, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0", altFolder, "getting the old path (with forward slashes) if the current path is based on backslashes");
		}

		/// <summary>
		/// Tests the scenario where the user changed the install folder during upgrade
		/// </summary>
		[Test]
		public void TestInstallFolderChanged()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Labs\Rats";
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:\Program Files\ACME Laboratories\ExperimentalRats\TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_gyj1vfoawarsonr4ssufrpzggih1qwle", altFolder, "should find correct former install directory");
		}

		/// <summary>
		/// Tests the scenario where the user changed the install folder during upgrade
		/// </summary>
		[Test]
		public void TestInstallFolderChanged2()
		{
			const string previousInstallDir = @"c:/program files/acme labs/rats/"; // note the different slashes, trailing slash and capitalization
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:\Program Files\ACME LABORATORIES\ExperimentalRats\TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_gyj1vfoawarsonr4ssufrpzggih1qwle", altFolder, "should find correct former install directory");
		}

		/// <summary>
		/// Tests the scenario where the user changed the install folder during upgrade
		/// </summary>
		[Test]
		public void TestInstallFolderChanged3()
		{
			const string previousInstallDir = @"C:\Program Files\ACME Labs\Rats\"; // note the different slashes, trailing slash and capitalization
			const string appDataPath = @"C:\Documents and Settings\Snowball\LocalSettings\AppData\ACME Laboratories\Acme.Laboratories.Exper_Url_kgyle1vbltt4mwwk4hgjgr1z52ofp1u0\3.0.0.0\user.config";
			const string assemblyName = "Acme.Laboratories.Experimental.Rat.TheBrain";
			const string assemblyVersion = "2.1.0.1";
			const string assemblyUrl = @"file:///C:/program Files/ACME Laboratories/ExperimentalRats/TheBrain.exe";

			var evidence = CreateAppDomainEvidence(assemblyName, assemblyVersion, assemblyUrl, false);
			var altFolder = UserUpgradeStrategy.GetAlternateAppSettingsFolder(previousInstallDir, evidence, appDataPath);
			Assert.AreEqual("Acme.Laboratories.Exper_Url_gyj1vfoawarsonr4ssufrpzggih1qwle", altFolder, "should find correct former install directory");
		}

		private static Evidence CreateAppDomainEvidence(string assemblyName, string assemblyVersion, string assemblyUrl, bool hasStrongName)
		{
			var evidence = new Evidence();
			evidence.AddHostEvidence(new Url(assemblyUrl));
			if (hasStrongName) evidence.AddHostEvidence(new StrongName(new StrongNamePublicKeyBlob(new byte[160]), assemblyName, new Version(assemblyVersion)));
			return evidence;
		}

		/// <summary>
		/// Reference implementation of the Base32 converter from System.Configuration.ClientConfigPaths
		/// </summary>
		private static string ToBase32(byte[] buff)
		{
			// or just invoke the real function using reflection
			return (string) _base32ReferenceMethod.Invoke(null, new object[] {buff});
		}

		private static readonly MethodInfo _base32ReferenceMethod = typeof (ConfigurationManager).Assembly.GetType("System.Configuration.ClientConfigPaths")
			.GetMethod("ToBase32StringSuitableForDirName", BindingFlags.Static | BindingFlags.NonPublic, null, new[] {typeof (byte[])}, null);
	}
}

#endif