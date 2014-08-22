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
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// File related utilities.
	/// </summary>
	public static class FileUtils
	{
		private const int RETRY_MIN_DELAY = 100; // 100 ms
		private const long RETRY_MAX_DELAY = 10*1000; // 10 Seconds 

		/// <summary>
		/// Replacement for <see cref="File.Delete"/> that retries if the file is in use.  Succeeds when file doesn't exist.
		/// </summary>
		/// <remarks>
		/// As is the case with <see cref="File.Delete"/>, this method does not throw an exception when the file doesn't exist.
		/// </remarks>
		/// <param name="path">The path to delete</param>
		public static void Delete(string path)
		{
			Delete(path, RETRY_MAX_DELAY, null, RETRY_MIN_DELAY);
		}

		/// <summary>
		/// Replacement for <see cref="File.Delete"/> that retries if the file is in use.
		/// </summary>
		/// <param name="path">The path to delete.</param>
		/// <param name="timeout">The timeout in milliseconds to attempt to retry to delete the file</param>
		/// <param name="stopSignal">An optional stopSignal to tell the delete operation to stop if retrying</param>
		/// <param name="retryMinDelay">The minimum number of milliseconds to delay when deleting.</param>
		public static void Delete(string path, long timeout, ManualResetEvent stopSignal, int retryMinDelay)
		{
			Exception lastException = null;
			long begin = Environment.TickCount;
			bool cancelled = false;

			while (!cancelled)
			{
				try
				{
					// File.Delete does not throw an exception when the file does not exist.
					// It does throw an exception if the path is invalid, however.
					File.Delete(path);
					return;
				}
				catch (IOException e)
				{
					// other IO exceptions should be treated as retry
					lastException = e;
					var rand = new Random();
					Thread.Sleep(rand.Next(retryMinDelay, 2*retryMinDelay));
				}

				if (timeout > 0 && Environment.TickCount - begin > timeout)
				{
					throw lastException;
				}

				if (stopSignal != null)
				{
					cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(retryMinDelay), false);
				}
			}

			if (!cancelled)
				throw lastException;
		}

		/// <summary>
		/// Replacement for <see><cref>File.Copy</cref></see> that retries if the file is in use.
		/// </summary>
		/// <param name="source">The path to copy from.</param>
		/// <param name="destination">The path to copy to.</param>
		/// <param name="overwrite">Overwrite an existing destination file.</param>
		public static void Copy(string source, string destination, bool overwrite)
		{
			Copy(source, destination, overwrite, RETRY_MAX_DELAY, null, RETRY_MIN_DELAY);
		}

		/// <summary>
		/// Replacement for <see><cref>File.Copy</cref></see> that retries if the file is in use.
		/// </summary>
		/// <param name="source">The path to copy from.</param>
		/// <param name="destination">The path to copy to.</param>
		/// <param name="overwrite">Boolean value to indicate whether to overwrite the destination if it exists</param>
		/// <param name="timeout">The timeout in milliseconds to attempt to retry to delete the file</param>
		/// <param name="stopSignal">An optional stopSignal to tell the delete operation to stop if retrying</param>
		/// <param name="retryMinDelay">The minimum number of milliseconds to delay when deleting.</param>
		public static void Copy(string source, string destination, bool overwrite,
		                        long timeout, ManualResetEvent stopSignal, int retryMinDelay)
		{
			Exception lastException = null;
			long begin = Environment.TickCount;
			bool cancelled = false;

			while (!cancelled)
			{
				try
				{
					// check if the file still exists every time in case it is moved/deleted 
					// so that we are not stucked in the loop
					if (!File.Exists(source))
						throw new FileNotFoundException(String.Format("Source file {0} does not exist", source), source);

					File.Copy(source, destination, overwrite);
					return;
				}
				catch (IOException e)
				{
					// other IO exceptions should be treated as retry
					lastException = e;
					var rand = new Random();
					Thread.Sleep(rand.Next(retryMinDelay, 2*retryMinDelay));
				}

				if (timeout > 0 && Environment.TickCount - begin > timeout)
				{
					throw lastException;
				}

				if (stopSignal != null)
				{
					cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(retryMinDelay), false);
				}
			}

			if (!cancelled)
				throw lastException;
		}

		/// <summary>
		/// Replacement for <see cref="File.Replace(string,string,string)"/> that retries if the file is in use.
		/// </summary>
		/// <param name="source">The path to move from.</param>
		/// <param name="destination">The path to move to.</param>
		/// <param name="existingDestinationBackup">The path where <paramref name="destination"/> will be moved if it already exists.</param>
		public static void Replace(string source, string destination, string existingDestinationBackup)
		{
			Replace(source, destination, existingDestinationBackup, RETRY_MAX_DELAY, null, RETRY_MIN_DELAY);
		}

		/// <summary>
		/// Replacement for <see cref="File.Replace(string,string,string)"/> that retries if the file is in use.
		/// </summary>
		/// <param name="source">The path to copy from.</param>
		/// <param name="destination">The path to copy to.</param>
		/// <param name="existingDestinationBackup">The path where <paramref name="destination"/> will be moved if it already exists.</param>
		/// <param name="timeout">The timeout in milliseconds to attempt to retry to delete the file</param>
		/// <param name="stopSignal">An optional stopSignal to tell the delete operation to stop if retrying</param>
		/// <param name="retryMinDelay">The minimum number of milliseconds to delay when deleting.</param>
		public static void Replace(string source, string destination, string existingDestinationBackup,
		                           long timeout, ManualResetEvent stopSignal, int retryMinDelay)
		{
			Exception lastException = null;
			long begin = Environment.TickCount;
			bool cancelled = false;

			while (!cancelled)
			{
				try
				{
					// check if the file still exists every time in case it is moved/deleted 
					// so that we are not stucked in the loop
					if (!File.Exists(source))
						throw new FileNotFoundException(String.Format("Source file {0} does not exist", source), source);

					if (!File.Exists(destination))
					{
						File.Move(source, destination);
					}
					else
					{
						Delete(existingDestinationBackup, timeout, stopSignal, retryMinDelay);
						File.Move(destination, existingDestinationBackup);
						File.Move(source, destination);
					}

					return;
				}
				catch (DriveNotFoundException)
				{
					throw;
				}
				catch (FileNotFoundException)
				{
					throw;
				}
				catch (PathTooLongException)
				{
					throw;
				}
				catch (IOException e)
				{
					// other IO exceptions should be treated as retry
					lastException = e;
					var rand = new Random();
					Thread.Sleep(rand.Next(retryMinDelay, 2*retryMinDelay));
				}

				if (timeout > 0 && Environment.TickCount - begin > timeout)
				{
					throw lastException;
				}

				if (stopSignal != null)
				{
					cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(retryMinDelay), false);
				}
			}

			if (!cancelled)
				throw lastException;
		}

		/// <summary>
		/// Gets whether or not a file has the READ ONLY attribute flag.
		/// </summary>
		public static bool IsFileReadOnly(string file)
		{
			return (File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
		}

		/// <summary>
		/// Changes whether or not a file has the READ ONLY attribute flag.
		/// </summary>
		public static void SetFileReadOnly(string file, bool readOnlyFlag)
		{
			var attribs = File.GetAttributes(file);

			// only call set attributes if the readonly flag needs to be changed
			if (readOnlyFlag ^ ((attribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly))
				File.SetAttributes(file, readOnlyFlag ? (attribs | FileAttributes.ReadOnly) : (attribs & ~FileAttributes.ReadOnly));
		}

		/// <summary>
		/// Changes whether or not a file has the READ ONLY attribute flag.
		/// </summary>
		/// <returns>Returns true if file exists and READ ONLY attribute was set without errors (whether or not the attribute flag actually changed).</returns>
		public static bool TrySetFileReadOnly(string file, bool readOnlyFlag)
		{
			if (!File.Exists(file)) return false;
			try
			{
				SetFileReadOnly(file, readOnlyFlag);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Creates copy of the specified file and returns the path to the backup file.
		/// This method allows a file to be backed up more than once with different extensions.
		/// </summary>
		/// <param name="source"></param>
		/// <returns>The path to the backup file. Null if the file is not backed up (it doesn't exist).</returns>
		/// <param name="backupDirectory">A backup directory, if null backup in the same folder</param>
		/// <remarks>If the file is in use, retry will be attempted until it succeeds.</remarks>
		public static string Backup(string source, string backupDirectory)
		{
			var sourceInfo = new FileInfo(source);
			if (File.Exists(source))
			{
				int i = 0;
				bool filenameAbtained = false;
				while (true)
				{
					// check if the file still exists every time in case it is moved/deleted 
					// so that we are not stucked in the loop
					if (!File.Exists(source))
						return null;
					if (sourceInfo.Directory == null)
						return null;

					string backup = Path.Combine(string.IsNullOrEmpty(backupDirectory)
						? sourceInfo.Directory.FullName
						: backupDirectory,
					                             i < 20
						                             ? String.Format("{0}.bak({1})", sourceInfo.Name, i)
						                             : Guid.NewGuid().ToString() + Path.GetExtension(sourceInfo.FullName));

					try
					{
						if (File.Exists(backup))
						{
							i++;
							continue;
						}

						using (FileStream stream =
							FileStreamOpener.OpenForSoleUpdate(backup, FileMode.CreateNew, RETRY_MIN_DELAY))
						{
							stream.Close();
							filenameAbtained = true;
						}
					}
					catch (Exception)
					{
						//NOTE: this method would cause an infinite loop if the filesystem cannot be written to.

						// try another file name
						i++;
					}

					if (filenameAbtained)
					{
						Copy(source, backup, true);
						return backup;
					}
				}
			}

			return null;
		}
	}
}