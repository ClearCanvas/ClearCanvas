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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Application for generating <see cref="ManifestInput"/> for use with <see cref="ManifestGenerationApplication"/>.
    /// </summary>
    /// <remarks>
    /// The ManifestInputGenerationApplication is used to generate a sample <see cref="ManifestInput"/> file
    /// that contains all the files contained in a given software distribution.  The output file can then 
    /// be edited and used with <see cref="ManifestGenerationApplication"/> to generate an actual 
    /// <see cref="ClearCanvasManifest"/> for a ClearCanvas product.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ManifestInputGenerationApplication : IApplicationRoot
    {
        #region Private Members

        private readonly ManifestInput _manifestInput = new ManifestInput();
        private readonly ManifestCommandLine _parms = new ManifestCommandLine();
        private bool _addedIgnoreLogs;
        
        #endregion Private Members

        #region Public Methods
        
        public void RunApplication(string[] args)
        {
            try
            {
                _parms.Parse(args);
         
                // Scan the specified directory
                ScanDirectory();

                // Save the manifest
                if (File.Exists(_parms.Manifest))
                    File.Delete(_parms.Manifest);

                ManifestInput.Serialize(_parms.Manifest, _manifestInput);

                Environment.ExitCode = 0;
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                _parms.PrintUsage(Console.Out);
                Environment.ExitCode = -1;
            }
            catch (Exception e)
            {
            	const string message = "Unexpected exception when generating manifest input: {0}";
            	Console.WriteLine(message, e.Message);
                Environment.ExitCode = -1;
            }
        }

        #endregion Public Methods

        #region Private Methods
        
        private void ScanDirectory()
        {
            if (!Directory.Exists(_parms.DistributionDirectory))
            {
                throw new ApplicationException("Directory does not exist: " + _parms.DistributionDirectory);
            }

            FileProcessor.Process(_parms.DistributionDirectory, null,
                                     ScanFile, true);
        }

        private void ScanFile(string filePath, out bool cancel)
        {
            cancel = false;

            if (!filePath.StartsWith(_parms.DistributionDirectory)) 
                return;

            ManifestInput.InputFile input = new ManifestInput.InputFile
                                                {
                                                    Name = filePath.Substring(_parms.DistributionDirectory.Length)
                                                };

            if (input.Name.StartsWith("logs\\"))
            {
                if (!_addedIgnoreLogs)
                {
                    ManifestInput.InputFile inputLogs = new ManifestInput.InputFile
                                                            {
                                                                Ignore = true, 
                                                                Name = "logs/"
                                                            };
                    _manifestInput.Files.Add(inputLogs);
                    _addedIgnoreLogs = true;
                }
                return;
            }

            if (input.Name.EndsWith("exe") || input.Name.EndsWith("dll"))
                input.Checksum = true;
            else
                input.Checksum = false;

            if (input.Name.EndsWith("exe.config"))
                input.Config = true;

            if (input.Name.EndsWith("critical.config"))
            {
                input.Config = false;
                input.Checksum = true;
            }

            _manifestInput.Files.Add(input);
        }

        #endregion Private Methods
    }
}
