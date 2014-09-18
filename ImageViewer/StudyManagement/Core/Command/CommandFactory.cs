#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
	internal static class CommandFactory
	{
		private static readonly FileProcessingDisposition _disposition = FileManagementSettings.Default.FileProcessingDisposition;

		public static CommandBase CreateRenameFileCommand(string source, string destination, bool failIfExists)
		{
			switch (_disposition)
			{
				case FileProcessingDisposition.Availability:
					return new RenameFileCommand(source, destination, failIfExists);
				case FileProcessingDisposition.Performance:
				default:
					return new FastRenameFileCommand(source, destination, failIfExists);
			}
		}

		public static CommandBase CreateCopyFileCommand(string sourceFile, string destinationFile, bool failIfExists)
		{
			return new CopyFileCommand(sourceFile, destinationFile, failIfExists);
		}

		public static CommandBase CreateSaveDicomFileCommand(string path, DicomFile dcf, bool failIfExists)
		{
			switch (_disposition)
			{
				case FileProcessingDisposition.Availability:
					return new SaveDicomFileCommand(path, dcf, failIfExists);
				case FileProcessingDisposition.Performance:
				default:
					return new FastSaveDicomFileCommand(path, dcf, failIfExists);
			}
		}
	}
}