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

using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// <see cref="CommandBase"/> for extracting a zip file containing study files to a specific directory.
	/// </summary>
	public class ExtractZipCommand : CommandBase
	{
		private readonly string _zipFile;
		private readonly string _destinationFolder;
		private readonly bool _overwrite;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="zip">The zip file to extract.</param>
		/// <param name="destinationFolder">The destination folder.</param>
		public ExtractZipCommand(string zip, string destinationFolder): base("Extract Zip File",true)
		{
			_zipFile = zip;
			_destinationFolder = destinationFolder;
			_overwrite = false;
		}

		/// <summary>
		/// Do the unzip.
		/// </summary>
		protected override void OnExecute(CommandProcessor theProcessor)
		{
		    var zipService = Platform.GetService<IZipService>();
			using (var zipReader = zipService.OpenRead(_zipFile))
			{
                zipReader.ExtractAll(_destinationFolder, _overwrite);
			}
		}

		/// <summary>
		/// Undo.  Remove the destination folder.
		/// </summary>
		protected override void OnUndo()
		{
			Directory.Delete(_destinationFolder, true);
		}
	}
}
