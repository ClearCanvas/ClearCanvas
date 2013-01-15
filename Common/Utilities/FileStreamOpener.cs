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
    // TODO (CR Jun 2012): Feels like this should be an instance class rather than static. The fact that a
    // ManualResetEvent is passed into a method in order to cancel the method seems odd.

    /// <summary>
    /// Provides convenient blocking methods for file opening.
    /// </summary>
    /// <remarks>
    /// The FileStreamOpener class is an abstraction for file opening.  It has a built in mechanism to retry opening a file 
    /// if there is a sharing collision with the file.  This should make software opening files to be more resilliant if 
    /// files are attempted to be opened at the same time.
    /// </remarks>
    public static class FileStreamOpener
    {
        #region Private Members
        private const int RETRY_MIN_DELAY = 250; 
        private const int FILE_MISSING_OVERRIDE_TIMEOUT = 2; // # of seconds to abort if the file is missing.
        #endregion

        /// <summary>
        /// Opens a file for update, using specified mode
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">Opening mode</param>
        /// <returns>A <see cref="FileStream"/> with sole write access.</returns>
        /// <remarks>
        /// <para>
        /// This methods will block indefinitely until the file is opened or exceptions are thrown because file cannot be open 
        /// using the specified mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// </para>
        /// <para>
        /// Once the file is opened, subsequent attempt to open the file for writing will fail until the returned stream is closed. However, other processes are 
        /// allowed to open the files for reading.
        /// </para>
        /// </remarks>
        static public FileStream OpenForSoleUpdate(string path, FileMode mode)
        {
            return OpenForSoleUpdate(path, mode, -1, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Opens a file for update, using specified opening mode and timeout period.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">File opening mode</param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <returns>The stream object for the specified file</returns>
        /// <exception cref="TimeoutException">Thrown when the file cannot be opened after the specified timeout</exception>
        /// <remarks>
        /// This methods will block until the specified file is opened or timeout has been reached.
        /// If the file cannot be open using the specified mode because it doesn't exist, exceptions may be be thrown 
        /// depending on the file opening mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// <para>
        /// Subsequent attempt to open the file for writing will fail until the returned stream is closed. However, other processes are 
        /// allowed to open the files for reading.
        /// </para>
        /// </remarks>
        static public FileStream OpenForSoleUpdate(string path, FileMode mode, int timeout)
        {
            return OpenForSoleUpdate(path, mode, timeout, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Opens a file for update, using specified opening mode and waits until timeout expires or a cancelling signal is set.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">File opening mode</param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <param name="stopSignal">Cancelling signal</param>
        /// <param name="retryMinDelay">Minimum delay between retries</param>
        /// <returns>The stream object for the specified file</returns>
        /// <exception cref="TimeoutException">Thrown when the file cannot be opened after the specified timeout</exception>
        /// <remarks>
        /// <para>
        /// This methods will block until the specified file is opened or when timeout has been reached or the cancelling signal 
        /// is set. If the file cannot be open using the specified mode because it doesn't exist, exceptions may be be thrown 
        /// depending on the file opening mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// <para>
        /// The returned stream will have opened with <see cref="FileAccess.Write"/> and <see cref="FileShare.Read"/>  permissions.
        /// Subsequent attempt to open the file for writing will fail until the stream is closed. However, other processes are 
        /// allowed to open the files for reading.
        /// </para>
        /// If cancel signal is set and the file hasn't been opened, <b>null</b> will be returned.
        /// </para>
        /// </remarks>
        static public FileStream OpenForSoleUpdate(string path, FileMode mode, int timeout, ManualResetEvent stopSignal, int retryMinDelay)
        {
            FileStream stream = null;

            // wait until we can lock the compressed header file for update
            long begin = Environment.TickCount;
            while (true)
            {
                try
                {
                    stream =
                        new FileStream(path, mode, FileAccess.Write, FileShare.Read
                        /* don't block others from reading this file */, 65536 /* This was configurable in the past, but we never changed it */,
                                       FileOptions.WriteThrough /* WriteThrough mode could be turned off in the past */);
                    break;
                }
                catch (FileNotFoundException)
                {
                    // The caller should've used FileMode.CreateNew or FileMode.OpenOrCreate
                    // Nothing can be done if it doesn't.
                    throw;
                }
                catch (DirectoryNotFoundException)
                {
                    // The path is invalid
                    throw;
                }
                catch (PathTooLongException)
                {
                    // The path is too long
                    throw;
                }
                catch (IOException)
                {
                    // other types of exception should be treated as retry
                    var rand = new Random();
                    Thread.Sleep(rand.Next(retryMinDelay, 2 * retryMinDelay));
                }

                if (stream == null)
                {
                    if (timeout > 0 && Environment.TickCount - begin > timeout)
                    {
                        throw new TimeoutException();
                    }

                    if (stopSignal != null)
                        stopSignal.WaitOne(TimeSpan.FromMilliseconds(100), false);

                }                
            }

            return stream;
        }

        /// <summary>
        /// Opens a file for reading, using specified opening mode.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns>a <see cref="FileStream"/> with read permission.</returns>
        /// <remarks>
        /// <para>
        /// </para>This method will be blocked indefinitely until the file is opened using the specified mode or exceptions
        /// are thrown because it doesn't exist. If access permission exceptions occur, the method will try to open the file again.
        /// <para>
        /// The file will be opened using <see cref="FileAccess.Read"/> and <see cref="FileShare.ReadWrite"/> permissions.
        /// </para>
        /// </remarks>
        static public FileStream OpenForRead(string path, FileMode mode)
        {
            return OpenForRead(path, mode, -1, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Opens a file for reading, using specified opening mode and timeout period
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <returns></returns>
        /// <remarks>
        /// The file will be opened using <see cref="FileAccess.Read"/> and <see cref="FileShare.ReadWrite"/> permissions.
        /// Once the file has been opened, other processes still can open the files for reading.
        /// </remarks>
        static public FileStream OpenForRead(string path, FileMode mode, int timeout)
        {
            return OpenForRead(path, mode, timeout, null, RETRY_MIN_DELAY);
        }


        /// <summary>
        /// Opens a file for reading, using specified opening mode and waits until timeout expires or a cancelling signal is set.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">File opening mode</param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <param name="stopSignal">Cancelling signal</param>
        /// <param name="retryMinDelay">Minimum delay between retries</param>
        /// <returns>The stream object for the specified file</returns>
        /// <exception cref="TimeoutException">Thrown when the file cannot be opened after the specified timeout</exception>
        /// <remarks>
        /// <para>
        /// This methods will block until the specified file is opened or when timeout has been reached or the cancelling signal 
        /// is set. If the file cannot be open using the specified mode because it doesn't exist, exceptions may be be thrown 
        /// depending on the file opening mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// </para>
        /// 
        /// <para>
        /// If cancel signal is set and the file hasn't been opened, <b>null</b> will be returned.
        /// </para>
        /// 
        /// <para>
        /// The returned stream will have opened with <see cref="FileAccess.Read"/> and <see cref="FileShare.ReadWrite"/> permissions.
        /// </para>
        /// 
        /// </remarks>
        static public FileStream OpenForRead(string path, FileMode mode, long timeout, ManualResetEvent stopSignal, int retryMinDelay)
        {
            FileStream stream = null;
            long begin = Environment.TickCount;
            bool cancelled = false;

            while (!cancelled)
            {
                Exception lastException;
                try
                {
                    stream = new FileStream(path, mode, FileAccess.Read, FileShare.ReadWrite /* allow others to update this file */);
                    break;
                }
                catch (FileNotFoundException e)
                {
                    // Maybe it is being swapped?
                    lastException = e;
                    
                    // regardless of what the caller wants, if we can't find the file 
                    // after FILE_MISSING_OVERRIDE_TIMEOUT seconds, we should abort so that we don't block 
                    // the application for too long.
                    TimeSpan elapse = TimeSpan.FromMilliseconds(Environment.TickCount - begin);
                    if (elapse > TimeSpan.FromSeconds(FILE_MISSING_OVERRIDE_TIMEOUT))
                        throw;
                }
                catch (DirectoryNotFoundException)
                {
                    // The path is invalid
                    throw;
                }
                catch (PathTooLongException)
                {
                    // The path is too long
                    throw;
                }
                catch (IOException e)
                {
                    // other IO exceptions should be treated as retry
                    lastException = e; 
                    var rand = new Random();
                    Thread.Sleep(rand.Next(retryMinDelay, 2 * retryMinDelay));
                }

                if (stream == null)
                {
                    if (timeout > 0 && Environment.TickCount - begin > timeout)
                    {
                        if (lastException != null)
                            throw lastException;
                        else
                            throw new TimeoutException();
                    }

                    if (stopSignal != null)
                    {
                        cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(100), false);
                    }
                }
            }

            return stream;
        }
    }
}
