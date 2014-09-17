#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

#region Inline Attributions
// The source code contained in this file is based on an original work
// from the Apache Software Foundation that was licensed under the 
// Apache License, Version 2.0 a copy of which is located at
//
// http://www.apache.org/licenses/LICENSE-2.0

#endregion

using System;
using System.IO;
using System.Text;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ClearCanvas.Common.log4net
{
    /// <summary>
    /// Appends logging events to a file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Logging events are sent to the file specified by
    /// the <see cref="File"/> property.
    /// </para>
    /// <para>
    /// The file can be opened in either append or overwrite mode 
    /// by specifying the <see cref="AppendToFile"/> property.
    /// If the file path is relative it is taken as relative from 
    /// the application base directory. The file encoding can be
    /// specified by setting the <see cref="Encoding"/> property.
    /// </para>
    /// <para>
    /// The layout's <see cref="ILayout.Header"/> and <see cref="ILayout.Footer"/>
    /// values will be written each time the file is opened and closed
    /// respectively. If the <see cref="AppendToFile"/> property is <see langword="true"/>
    /// then the file may contain multiple copies of the header and footer.
    /// </para>
    /// <para>
    /// This appender will first try to open the file for writing when <see cref="ActivateOptions"/>
    /// is called. This will typically be during configuration.
    /// If the file cannot be opened for writing the appender will attempt
    /// to open the file again each time a message is logged to the appender.
    /// If the file cannot be opened for writing when a message is logged then
    /// the message will be discarded by this appender.
    /// </para>
    /// <para>
    /// The <see cref="FileAppender"/> supports pluggable file locking models via
    /// the <see cref="LockingModel"/> property.
    /// The default behavior, implemented by <see cref="FileAppender.MutexLock"/> 
    /// is to use a Mutex to lock the file across processe
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    /// <author>Rodrigo B. de Oliveira</author>
    /// <author>Douglas de la Torre</author>
    /// <author>Niall Daley</author>
    public class FileAppender : TextWriterAppender
    {
        #region LockingStream Inner Class

        /// <summary>
        /// Write only <see cref="Stream"/> that uses the <see cref="LockingModelBase"/> 
        /// to manage access to an underlying resource.
        /// </summary>
        protected sealed class LockingStream : Stream, IDisposable
        {
            /// <summary>
            /// Exception based on the LockState.
            /// </summary>
            public sealed class LockStateException : LogException
            {
                /// <summary>
                /// Constructor.
                /// </summary>
                /// <param name="message">Exception message.</param>
                public LockStateException(string message)
                    : base(message)
                {
                }
            }

            private Stream m_realStream = null;
            private readonly LockingModelBase m_lockingModel = null;
            private int m_readTotal = -1;
            private int m_lockLevel = 0;

            /// <summary>
            /// The Lock level.
            /// </summary>
            public int LockLevel
            {
                get { return m_lockLevel; }
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="locking">The locking model.</param>
            /// <param name="lockLevel">The lock level.</param>
            public LockingStream(LockingModelBase locking, int lockLevel)
            {
                if (locking == null)
                {
                    const string message = "Locking model may not be null";
                    throw new ArgumentException(message, "locking");
                }
                m_lockingModel = locking;

                m_lockLevel = lockLevel;
            }

            #region Override Implementation of Stream

            // Methods
            /// <summary>
            /// Begin read.
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <param name="callback"></param>
            /// <param name="state"></param>
            /// <returns></returns>
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                AssertLocked();
                IAsyncResult ret = m_realStream.BeginRead(buffer, offset, count, callback, state);
                m_readTotal = EndRead(ret);
                return ret;
            }

            /// <summary>
            /// True asynchronous writes are not supported, the implementation forces a synchronous write.
            /// </summary>
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                AssertLocked();
                IAsyncResult ret = m_realStream.BeginWrite(buffer, offset, count, callback, state);
                EndWrite(ret);
                return ret;
            }

            /// <summary>
            /// Close the log file.
            /// </summary>
            public override void Close()
            {
                m_lockingModel.CloseFile();
            }

            /// <summary>
            /// End an asynchronous read.
            /// </summary>
            /// <param name="asyncResult"></param>
            /// <returns></returns>
            public override int EndRead(IAsyncResult asyncResult)
            {
                AssertLocked();
                return m_readTotal;
            }
            /// <summary>
            /// End an asychronous write.
            /// </summary>
            /// <param name="asyncResult"></param>
            public override void EndWrite(IAsyncResult asyncResult)
            {
                //No-op, it has already been handled
            }
            /// <summary>
            /// Flush write data.
            /// </summary>
            public override void Flush()
            {
                AssertLocked();
                m_realStream.Flush();
            }
            /// <summary>
            /// Read from the stream.
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                lock (this)
                {
                    return m_realStream.Read(buffer, offset, count);
                }
            }
            /// <summary>
            /// Read a byte from the stream.
            /// </summary>
            /// <returns></returns>
            public override int ReadByte()
            {
                return m_realStream.ReadByte();
            }
            /// <summary>
            /// Seek in the stream.
            /// </summary>
            /// <param name="offset"></param>
            /// <param name="origin"></param>
            /// <returns></returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                AssertLocked();
                return m_realStream.Seek(offset, origin);
            }
            /// <summary>
            /// Set the length of the stream.
            /// </summary>
            /// <param name="value"></param>
            public override void SetLength(long value)
            {
                AssertLocked();
                m_realStream.SetLength(value);
            }
            /// <summary>
            /// Dispose the stream.
            /// </summary>
            void IDisposable.Dispose()
            {
                Close();
            }
            /// <summary>
            /// Write to the stream.
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                AssertLocked();
                m_realStream.Write(buffer, offset, count);
            }
            /// <summary>
            /// Write a byte to the stream.
            /// </summary>
            /// <param name="value"></param>
            public override void WriteByte(byte value)
            {
                AssertLocked();
                m_realStream.WriteByte(value);
            }

            // Properties
            /// <summary>
            /// Can the stream read?
            /// </summary>
            public override bool CanRead
            {
                get { return false; }
            }
            /// <summary>
            /// Can the stream seek?
            /// </summary>
            public override bool CanSeek
            {
                get
                {
                    AssertLocked();
                    return m_realStream.CanSeek;
                }
            }
            /// <summary>
            /// Can the stream write?
            /// </summary>
            public override bool CanWrite
            {
                get
                {
                    AssertLocked();
                    return m_realStream.CanWrite;
                }
            }
            /// <summary>
            /// What is the length of the stream?
            /// </summary>
            public override long Length
            {
                get
                {
                    AssertLocked();
                    return m_realStream.Length;
                }
            }
            /// <summary>
            /// what is the current position within the stream?
            /// </summary>
            public override long Position
            {
                get
                {
                    AssertLocked();
                    return m_realStream.Position;
                }
                set
                {
                    AssertLocked();
                    m_realStream.Position = value;
                }
            }

            #endregion Override Implementation of Stream

            #region Locking Methods
            /// <summary>
            /// Reopen the file.
            /// </summary>
            public void Reopen()
            {
                if (m_realStream != null)
                {
                    m_lockingModel.ReopenFile();
                    m_realStream = m_lockingModel.Stream;
                    m_realStream.Seek(0, SeekOrigin.End);
                }
            }

            private void AssertLocked()
            {
                lock (this)
                {
                    if (!m_lockingModel.Locked)
                        throw new LockStateException("The file is not currently locked");
                }
            }
            /// <summary>
            /// Acquire a lock on the file.
            /// </summary>
            /// <returns></returns>
            public bool AcquireLock()
            {
                bool ret;
                lock (this)
                {
                    if (!m_lockingModel.Locked)
                    {
                        // If lock is already acquired, nop
                        if (m_lockingModel.AcquireLock())
                        {
                            m_lockLevel = 1;

                            m_realStream = m_lockingModel.Stream;

                            try
                            {
								if (m_realStream.Position > m_realStream.Length) 
								{
									// if the file is rolled in another thread, our stream sees the updated stream length but the position IS NOT CHANGED
									// any further attempts to set the position causes an exception because the current position is beyond the end of the
									// file, so we must reopen the stream (which will put the cursor at the end of the file anyway)
									Reopen();
								}
								else 
								{
									if (m_realStream.CanSeek)
										m_realStream.Seek(0, SeekOrigin.End);
								}
                            }
                            catch (Exception e)
                            {
                                LogLog.Error(typeof(FileAppender),"FileAppender: INTERNAL ERROR.  Unable to see file: " + e.Message);
                            }

                            ret = true;
                        }
                        else
                            ret = false;
                    }
                    else
                    {
                        m_lockLevel++;
                        ret = true;
                        if (m_realStream == null)
                            m_realStream = m_lockingModel.Stream;
                    }
                }
                return ret;
            }

            /// <summary>
            /// Release a lock on the file.
            /// </summary>
            public void ReleaseLock()
            {
                lock (this)
                {
                    m_lockLevel--;
                    if (m_lockLevel == 0)
                    {
                        m_realStream = null;
                        // If already unlocked, nop
                        m_lockingModel.ReleaseLock();
                    }
                }
            }

            #endregion Locking Methods
        }

        #endregion LockingStream Inner Class

        #region Locking Models

        /// <summary>
        /// Locking model base class
        /// </summary>
        /// <remarks>
        /// <para>
        /// Base class for the locking models available to the <see cref="FileAppender"/> derived loggers.
        /// </para>
        /// </remarks>
        public abstract class LockingModelBase
        {
            private FileAppender m_appender = null;

            ///<summary>
            /// The underlying stream for the file.
            ///</summary>
            public abstract Stream Stream { get; }
            /// <summary>
            /// Reopen the file.
            /// </summary>
            public abstract void ReopenFile();
            /// <summary>
            /// Is it locked?
            /// </summary>
            public abstract bool Locked { get; }

            /// <summary>
            /// Open the output file
            /// </summary>
            /// <param name="filename">The filename to use</param>
            /// <param name="append">Whether to append to the file, or overwrite</param>
            /// <param name="encoding">The encoding to use</param>
            /// <remarks>
            /// <para>
            /// Open the file specified and prepare for logging. 
            /// No writes will be made until <see cref="AcquireLock"/> is called.
            /// Must be called before any calls to <see cref="AcquireLock"/>,
            /// <see cref="ReleaseLock"/> and <see cref="CloseFile"/>.
            /// </para>
            /// </remarks>
            public abstract Stream OpenFile(string filename, bool append, Encoding encoding);

            /// <summary>
            /// Close the file
            /// </summary>
            /// <remarks>
            /// <para>
            /// Close the file. No further writes will be made.
            /// </para>
            /// </remarks>
            public abstract void CloseFile();

            /// <summary>
            /// Acquire the lock on the file
            /// </summary>
            /// <returns>A stream that is ready to be written to.</returns>
            /// <remarks>
            /// <para>
            /// Acquire the lock on the file in preparation for writing to it. 
            /// Return a stream pointing to the file. <see cref="ReleaseLock"/>
            /// must be called to release the lock on the output file.
            /// </para>
            /// </remarks>
            public abstract bool AcquireLock();

            /// <summary>
            /// Release the lock on the file
            /// </summary>
            /// <remarks>
            /// <para>
            /// Release the lock on the file. No further writes will be made to the 
            /// stream until <see cref="AcquireLock"/> is called again.
            /// </para>
            /// </remarks>
            public abstract void ReleaseLock();

            /// <summary>
            /// Gets or sets the <see cref="FileAppender"/> for this LockingModel
            /// </summary>
            /// <value>
            /// The <see cref="FileAppender"/> for this LockingModel
            /// </value>
            /// <remarks>
            /// <para>
            /// The file appender this locking model is attached to and working on
            /// behalf of.
            /// </para>
            /// <para>
            /// The file appender is used to locate the security context and the error handler to use.
            /// </para>
            /// <para>
            /// The value of this property will be set before <see cref="OpenFile"/> is
            /// called.
            /// </para>
            /// </remarks>
            public FileAppender CurrentAppender
            {
                get { return m_appender; }
                set { m_appender = value; }
            }
        }

        ///<summary>
        ///Class that represents a mutex based locking
        ///model
        ///</summary>
        public class MutexLock : LockingModelBase, IDisposable
        {
            private string m_filename;
            private string m_mutexname;
            private bool m_append;
            private bool m_mutexLocked = false;
            private Stream m_stream = null;

            // TODO (CR Jun 2012) this should use the ExcluseLock class instead
            private Mutex m_mutex;

            /// <summary>
            /// Locked property.
            /// </summary>
            public override bool Locked
            {
                get { return m_mutexLocked; }
            }

            /// <summary>
            /// The underlying stream.
            /// </summary>
            public override Stream Stream
            {
                get
                {
                    if (Locked) return m_stream;

                    return null;
                }
            }

            private static string GetMutexName(string fileName)
            {
                string canonicalName = ConvertToFullPath(fileName).ToLower();

                canonicalName = canonicalName.Replace('\\', '_');
                canonicalName = canonicalName.Replace('/', '_');
                canonicalName = canonicalName.Replace(':', '_');

                return "Global\\filelock-mutex-" + canonicalName;
            }

            /// <summary>
            /// Reopen the log file.
            /// </summary>
            public override void ReopenFile()
            {
                CloseFile();
                OpenFile(m_filename, m_append, Encoding.Default);
            }

            /// <summary>
            /// Prepares to open the file when the first message is logged.
            /// </summary>
            /// <param name="filename">The filename to use</param>
            /// <param name="append">Whether to append to the file, or overwrite</param>
            /// <param name="encoding">The encoding to use</param>
            /// <remarks>
            /// <para>
            /// Open the file specified and prepare for logging. 
            /// No writes will be made until <see cref="AcquireLock"/> is called.
            /// Must be called before any calls to <see cref="AcquireLock"/>,
            /// <see cref="ReleaseLock"/> and <see cref="CloseFile"/>.
            /// </para>
            /// </remarks>
            public override Stream OpenFile(string filename, bool append, Encoding encoding)
            {
                m_filename = filename;
                m_append = append;

                if (m_stream == null)
                {
                    try
                    {
                        using (CurrentAppender.SecurityContext.Impersonate(this))
                        {
                            // Ensure that the directory structure exists
                            string directoryFullName = Path.GetDirectoryName(m_filename);

                            // Only create the directory if it does not exist
                            // doing this check here resolves some permissions failures
                            if (!Directory.Exists(directoryFullName))
                            {
                                Directory.CreateDirectory(directoryFullName);
                            }

                            FileMode fileOpenMode = append ? FileMode.Append : FileMode.Create;
                            m_stream = new FileStream(filename, fileOpenMode, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);

                            m_append = true;
                        }
                    }
                    catch (Exception e1)
                    {
                        CurrentAppender.ErrorHandler.Error("Unable to open file " + m_filename + ". " + e1.Message);
                        if (m_stream != null)
                        {
                            m_stream.Close();
                            m_stream = null;
                        }
                    }
                }
                return m_stream;
            }

            /// <summary>
            /// Close the file
            /// </summary>
            /// <remarks>
            /// <para>
            /// Close the file. No further writes will be made.
            /// </para>
            /// </remarks>
            public override void CloseFile()
            {
                using (CurrentAppender.SecurityContext.Impersonate(this))
                {
                    if (m_stream != null)
                        m_stream.Close();
                    m_stream = null;
                }
            }

            /// <summary>
            /// Acquire the lock on the file
            /// </summary>
            /// <returns>A stream that is ready to be written to.</returns>
            /// <remarks>
            /// <para>
            /// Acquire the lock on the file in preparation for writing to it. 
            /// Return a stream pointing to the file. <see cref="ReleaseLock"/>
            /// must be called to release the lock on the output file.
            /// </para>
            /// </remarks>
            public override bool AcquireLock()
            {
                if (m_mutex == null)
                {
                    m_mutex = CreateMutex();
                    m_mutexLocked = false;
                }
                else
                {
                    if (m_mutexLocked == false && !m_mutexname.Equals(GetMutexName(m_filename)))
                    {
                        m_mutex.Close();
                        m_mutex = null;
                        m_mutex = CreateMutex();
                    }
                }

                if (m_mutex.WaitOne(5000, true))
                {
                    m_mutexLocked = true;
                    return true;
                }

                return false;
            }

            private Mutex CreateMutex()
            {
            	m_mutexname = GetMutexName(m_filename);

                try
                {
                    // Open the mutex.
                    m_mutex = Mutex.OpenExisting(m_mutexname);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    // The named mutex does not exist.
                    MutexSecurity mSec = new MutexSecurity();

                    MutexAccessRule rule = new MutexAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        MutexRights.FullControl, AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                	bool mutexWasCreated;
                	m_mutex = new Mutex(false, m_mutexname, out mutexWasCreated, mSec);
                }
                catch (UnauthorizedAccessException)
                {
                    // The named mutex exists, but the user does not have the security access required to use it.
                    LogLog.Warn(typeof(FileAppender), "The named mutex exists, but the user does not have the security access required to use it.");
                    try
                    {
                        m_mutex = Mutex.OpenExisting(m_mutexname, MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                        // Get the current ACL. This requires MutexRights.ReadPermissions.
                        MutexSecurity mSec = m_mutex.GetAccessControl();

                        // Now grant the user the correct rights.
                        MutexAccessRule rule = new MutexAccessRule(
                            new SecurityIdentifier(WellKnownSidType.WorldSid, null), 
                            MutexRights.FullControl, AccessControlType.Allow);
                        mSec.AddAccessRule(rule);

                        // Update the ACL. This requires MutexRights.ChangePermissions.
                        m_mutex.SetAccessControl(mSec);

                        LogLog.Debug(typeof(FileAppender), "Updated mutex security.");

                       m_mutex = Mutex.OpenExisting(m_mutexname);

                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        LogLog.Error(typeof(FileAppender), "Unable to change permissions on mutex.", ex);
                        m_mutex = new Mutex(false, m_mutexname);
                    }
                }

                return m_mutex;
            }

            /// <summary>
            /// Release the lock on the file
            /// </summary>
            /// <remarks>
            /// <para>
            /// Release the lock on the file. No further writes will be made to the 
            /// stream until <see cref="AcquireLock"/> is called again.
            /// </para>
            /// </remarks>
            public override void ReleaseLock()
            {
                if (m_mutex != null)
                {
                    m_mutexLocked = false;
                    m_mutex.ReleaseMutex();
                }
            }

            /// <summary>
            /// IDisposable implementation.
            /// </summary>
            public void Dispose()
            {
                if (m_mutex != null)
                {
                    if (m_mutexLocked)
                        m_mutex.ReleaseMutex();
                    m_mutex.Close();
                    m_mutex = null;
                }

                if (m_stream != null)
                    m_stream.Close();
                m_stream = null;
            }
        }
        #endregion Locking Models

        #region Public Instance Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// <para>
        /// Default constructor
        /// </para>
        /// </remarks>
        public FileAppender()
        {
        }

        /// <summary>
        /// Construct a new appender using the layout, file and append mode.
        /// </summary>
        /// <param name="layout">the layout to use with this appender</param>
        /// <param name="filename">the full path to the file to write to</param>
        /// <param name="append">flag to indicate if the file should be appended to</param>
        /// <remarks>
        /// <para>
        /// Obsolete constructor.
        /// </para>
        /// </remarks>
        [Obsolete("Instead use the default constructor and set the Layout, File & AppendToFile properties")]
        public FileAppender(ILayout layout, string filename, bool append)
        {
            lock (this)
            {
                Layout = layout;
                File = filename;
                AppendToFile = append;
                ActivateOptions();
            }
        }

        /// <summary>
        /// Construct a new appender using the layout and file specified.
        /// The file will be appended to.
        /// </summary>
        /// <param name="layout">the layout to use with this appender</param>
        /// <param name="filename">the full path to the file to write to</param>
        /// <remarks>
        /// <para>
        /// Obsolete constructor.
        /// </para>
        /// </remarks>
        [Obsolete("Instead use the default constructor and set the Layout & File properties")]
        public FileAppender(ILayout layout, string filename)
            : this(layout, filename, true)
        {
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the path to the file that logging will be written to.
        /// </summary>
        /// <value>
        /// The path to the file that logging will be written to.
        /// </value>
        /// <remarks>
        /// <para>
        /// If the path is relative it is taken as relative from 
        /// the application base directory.
        /// </para>
        /// </remarks>
        virtual public string File
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        /// <summary>
        /// Gets or sets a flag that indicates whether the file should be
        /// appended to or overwritten.
        /// </summary>
        /// <value>
        /// Indicates whether the file should be appended to or overwritten.
        /// </value>
        /// <remarks>
        /// <para>
        /// If the value is set to false then the file will be overwritten, if 
        /// it is set to true then the file will be appended to.
        /// </para>
        /// The default value is true.
        /// </remarks>
        public bool AppendToFile
        {
            get { return m_appendToFile; }
            set { m_appendToFile = value; }
        }

        /// <summary>
        /// Gets or sets <see cref="Encoding"/> used to write to the file.
        /// </summary>
        /// <value>
        /// The <see cref="Encoding"/> used to write to the file.
        /// </value>
        /// <remarks>
        /// <para>
        /// The default encoding set is <see cref="System.Text.Encoding.Default"/>
        /// which is the encoding for the system's current ANSI code page.
        /// </para>
        /// </remarks>
        public Encoding Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="SecurityContext"/> used to write to the file.
        /// </summary>
        /// <value>
        /// The <see cref="SecurityContext"/> used to write to the file.
        /// </value>
        /// <remarks>
        /// <para>
        /// Unless a <see cref="SecurityContext"/> specified here for this appender
        /// the <see cref="SecurityContextProvider.DefaultProvider"/> is queried for the
        /// security context to use. The default behavior is to use the security context
        /// of the current thread.
        /// </para>
        /// </remarks>
        public SecurityContext SecurityContext
        {
            get { return m_securityContext; }
            set { m_securityContext = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="FileAppender.LockingModel"/> used to handle locking of the file.
        /// </summary>
        /// <value>
        /// The <see cref="FileAppender.LockingModel"/> used to lock the file.
        /// </value>
        /// <remarks>
        /// <para>
        /// Gets or sets the <see cref="FileAppender.LockingModel"/> used to handle locking of the file.
        /// </para>
        /// <para>
        /// There are two built in locking models, <see cref="FileAppender.MutexLock"/>.
        /// The former locks the file from the start of logging to the end and the 
        /// later lock only for the minimal amount of time when logging each message.
        /// </para>
        /// <para>
        /// The default locking model is the <see cref="FileAppender.MutexLock"/>.
        /// </para>
        /// </remarks>
        public LockingModelBase LockingModel
        {
            get { return m_lockingModel; }
            set { m_lockingModel = value; }
        }

        #endregion Public Instance Properties

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// Activate the options on the file appender. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// <para>
        /// This will cause the file to be opened.
        /// </para>
        /// </remarks>
        override public void ActivateOptions()
        {
            base.ActivateOptions();

            if (m_securityContext == null)
            {
                m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
            }

            if (m_lockingModel == null)
            {
                m_lockingModel = new MutexLock();
            }

            m_lockingModel.CurrentAppender = this;

            using (SecurityContext.Impersonate(this))
            {
                m_fileName = ConvertToFullPath(m_fileName.Trim());
            }

            if (m_fileName != null)
            {
                SafeOpenFile(m_fileName, m_appendToFile);
            }
            else
            {
                LogLog.Warn(typeof(FileAppender), "FileAppender: File option not set for appender [" + Name + "].");
                LogLog.Warn(typeof(FileAppender), "FileAppender: Are you using FileAppender instead of ConsoleAppender?");
            }
        }

        #endregion Override implementation of AppenderSkeleton

        #region Override implementation of TextWriterAppender

        /// <summary>
        /// Closes any previously opened file and calls the parent's <see cref="TextWriterAppender.Reset"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Resets the filename and the file stream.
        /// </para>
        /// </remarks>
        override protected void Reset()
        {
            base.Reset();
            m_fileName = null;
        }

        /// <summary>
        /// Called to initialize the file writer
        /// </summary>
        /// <remarks>
        /// <para>
        /// Will be called for each logged message until the file is
        /// successfully opened.
        /// </para>
        /// </remarks>
        override protected void PrepareWriter()
        {
            SafeOpenFile(m_fileName, m_appendToFile);
        }

        /// <summary>
        /// This method is called by the <see cref="AppenderSkeleton.DoAppend(LoggingEvent)"/>
        /// method. 
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
        /// <remarks>
        /// <para>
        /// Writes a log statement to the output stream if the output stream exists 
        /// and is writable.  
        /// </para>
        /// <para>
        /// The format of the output will depend on the appender's layout.
        /// </para>
        /// </remarks>
        override protected void Append(LoggingEvent loggingEvent)
        {
            LockingStream theStream = m_stream;

            if (theStream.AcquireLock())
            {
                try
                {
                    base.Append(loggingEvent);
                }
                finally
                {
                    theStream.ReleaseLock();
                }
            }
        }

        /// <summary>
        /// This method is called by the <see cref="AppenderSkeleton.DoAppend(LoggingEvent[])"/>
        /// method. 
        /// </summary>
        /// <param name="loggingEvents">The array of events to log.</param>
        /// <remarks>
        /// <para>
        /// Acquires the output file locks once before writing all the events to
        /// the stream.
        /// </para>
        /// </remarks>
        override protected void Append(LoggingEvent[] loggingEvents)
        {
            LockingStream theStream = m_stream;

            if (theStream.AcquireLock())
            {
                try
                {
                    base.Append(loggingEvents);
                }
                finally
                {
                    theStream.ReleaseLock();
                }
            }
        }

        /// <summary>
        /// Writes a footer as produced by the embedded layout's <see cref="ILayout.Footer"/> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Writes a footer as produced by the embedded layout's <see cref="ILayout.Footer"/> property.
        /// </para>
        /// </remarks>
        protected override void WriteFooter()
        {
            if (m_stream != null)
            {
                //WriteFooter can be called even before a file is opened
                LockingStream theStream = m_stream;

                if (theStream.AcquireLock())
                {
                    try
                    {
                        base.WriteFooter();
                    }
                    finally
                    {
                        theStream.ReleaseLock();
                    }
                }
            }
        }

        /// <summary>
        /// Writes a header produced by the embedded layout's <see cref="ILayout.Header"/> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Writes a header produced by the embedded layout's <see cref="ILayout.Header"/> property.
        /// </para>
        /// </remarks>
        protected override void WriteHeader()
        {
            if (m_stream != null)
            {
                LockingStream theStream = m_stream;

                if (theStream.AcquireLock())
                {
                    try
                    {
                        base.WriteHeader();
                    }
                    finally
                    {
                        theStream.ReleaseLock();
                    }
                }
            }
        }

        /// <summary>
        /// Closes the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Closes the underlying <see cref="TextWriter"/>.
        /// </para>
        /// </remarks>
        protected override void CloseWriter()
        {
            if (m_stream != null)
            {
                LockingStream theStream = m_stream;

                if (theStream.AcquireLock())
                {
                    try
                    {
                        base.CloseWriter();
                    }
                    finally
                    {
                        theStream.ReleaseLock();
                    }
                }
            }
        }

        #endregion Override implementation of TextWriterAppender

        #region Public Instance Methods

        /// <summary>
        /// Closes the previously opened file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Writes the <see cref="ILayout.Footer"/> to the file and then
        /// closes the file.
        /// </para>
        /// </remarks>
        protected void CloseFile()
        {
            WriteFooterAndCloseWriter();
        }

        #endregion Public Instance Methods

        #region Protected Instance Methods

        /// <summary>
        /// Sets and <i>opens</i> the file where the log output will go. The specified file must be writable.
        /// </summary>
        /// <param name="fileName">The path to the log file. Must be a fully qualified path.</param>
        /// <param name="append">If true will append to fileName. Otherwise will truncate fileName</param>
        /// <remarks>
        /// <para>
        /// Calls <see cref="OpenFile"/> but guarantees not to throw an exception.
        /// Errors are passed to the <see cref="TextWriterAppender.ErrorHandler"/>.
        /// </para>
        /// </remarks>
        virtual protected void SafeOpenFile(string fileName, bool append)
        {
            try
            {
                OpenFile(fileName, append);
            }
            catch (Exception e)
            {
                ErrorHandler.Error("OpenFile(" + fileName + "," + append + ") call failed.", e, ErrorCode.FileOpenFailure);
            }
        }

        /// <summary>
        /// Sets and <i>opens</i> the file where the log output will go. The specified file must be writable.
        /// </summary>
        /// <param name="fileName">The path to the log file. Must be a fully qualified path.</param>
        /// <param name="append">If true will append to fileName. Otherwise will truncate fileName</param>
        /// <remarks>
        /// <para>
        /// If there was already an opened file, then the previous file
        /// is closed first.
        /// </para>
        /// <para>
        /// This method will ensure that the directory structure
        /// for the <paramref name="fileName"/> specified exists.
        /// </para>
        /// </remarks>
        virtual protected void OpenFile(string fileName, bool append)
        {
            if (LogLog.IsErrorEnabled)
            {
                // Internal check that the fileName passed in is a rooted path
                bool isPathRooted;
                using (SecurityContext.Impersonate(this))
                {
                    isPathRooted = Path.IsPathRooted(fileName);
                }
                if (!isPathRooted)
                {
                    LogLog.Error(typeof(FileAppender), "FileAppender: INTERNAL ERROR. OpenFile(" + fileName + "): File name is not fully qualified.");
                }
            }

            lock (this)
            {
                Reset();

                LogLog.Debug(typeof(FileAppender), "FileAppender: Opening file for writing [" + fileName + "] append [" + append + "]");

                // Save these for later, allowing retries if file open fails
                m_fileName = fileName;
                m_appendToFile = append;

                int lockLevel = 0;

                if (m_stream != null)
                    lockLevel = m_stream.LockLevel;
                LockingModel.CurrentAppender = this;
                LockingModel.OpenFile(fileName, append, m_encoding);
                m_stream = new LockingStream(LockingModel, lockLevel);

                if (m_stream != null)
                {
                    LockingStream theStream = m_stream;

                    if (theStream.AcquireLock())
                    {
                        try
                        {
                            SetQWForFiles(new StreamWriter(m_stream, m_encoding));
                        }
                        finally
                        {
                            theStream.ReleaseLock();
                        }
                    }
                }

                WriteHeader();
            }
        }

        /// <summary>
        /// Sets the quiet writer used for file output
        /// </summary>
        /// <param name="fileStream">the file stream that has been opened for writing</param>
        /// <remarks>
        /// <para>
        /// This implementation of <see cref="SetQWForFiles(Stream)"/> creates a <see cref="StreamWriter"/>
        /// over the <paramref name="fileStream"/> and passes it to the 
        /// <see cref="SetQWForFiles(TextWriter)"/> method.
        /// </para>
        /// <para>
        /// This method can be overridden by sub classes that want to wrap the
        /// <see cref="Stream"/> in some way, for example to encrypt the output
        /// data using a <c>System.Security.Cryptography.CryptoStream</c>.
        /// </para>
        /// </remarks>
        virtual protected void SetQWForFiles(Stream fileStream)
        {
            SetQWForFiles(new StreamWriter(fileStream, m_encoding));
        }

        /// <summary>
        /// Sets the quiet writer being used.
        /// </summary>
        /// <param name="writer">the writer over the file stream that has been opened for writing</param>
        /// <remarks>
        /// <para>
        /// This method can be overridden by sub classes that want to
        /// wrap the <see cref="TextWriter"/> in some way.
        /// </para>
        /// </remarks>
        virtual protected void SetQWForFiles(TextWriter writer)
        {
            QuietWriter = new QuietTextWriter(writer, ErrorHandler);
        }

        #endregion Protected Instance Methods

        #region Protected Static Methods

        /// <summary>
        /// Convert a path into a fully qualified path.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>The fully qualified path.</returns>
        /// <remarks>
        /// <para>
        /// Converts the path specified to a fully
        /// qualified path. If the path is relative it is
        /// taken as relative from the application base 
        /// directory.
        /// </para>
        /// </remarks>
        protected static string ConvertToFullPath(string path)
        {
            return SystemInfo.ConvertToFullPath(path);
        }

        #endregion Protected Static Methods

        #region Private Instance Fields

        /// <summary>
        /// Flag to indicate if we should append to the file
        /// or overwrite the file. The default is to append.
        /// </summary>
        private bool m_appendToFile = true;

        /// <summary>
        /// The name of the log file.
        /// </summary>
        private string m_fileName = null;

        /// <summary>
        /// The encoding to use for the file stream.
        /// </summary>
        private Encoding m_encoding = Encoding.Default;

        /// <summary>
        /// The security context to use for privileged calls
        /// </summary>
        private SecurityContext m_securityContext;

        /// <summary>
        /// The stream to log to. Has added locking semantics
        /// </summary>
        protected LockingStream m_stream = null;

        /// <summary>
        /// The locking model to use
        /// </summary>
        private LockingModelBase m_lockingModel = new MutexLock();

        #endregion Private Instance Fields
    }
}
