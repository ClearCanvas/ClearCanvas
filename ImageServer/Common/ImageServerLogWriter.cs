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


namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Class representing a log file being written.  The log file is ssaved in a MemoryStream in memory.
	/// </summary>
	/// <typeparam name="TLogClass">The class type being written to the log file.</typeparam>
	class ImageServerLogFile<TLogClass> : IDisposable
	{
		private MemoryStream _ms;
		private StreamWriter _sw;
		private readonly string _logType;

		public DateTime FirstTimestamp;
		public DateTime LastTimestamp;
		public string ZipFile;
		public string ZipDirectory;
		public DateTime Date;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="timestamp">The timestamp of the first log being written.</param>
		/// <param name="archivePath">The path to place the log file in.</param>
		/// <param name="logSize">The size of the log file.</param>
		/// <param name="logType">A string representation of the type of logs being written.  Used for logging and naming purposes.</param>
		public ImageServerLogFile(DateTime timestamp, string archivePath, int logSize, string logType)
		{
			_logType = logType;
			FirstTimestamp = timestamp;
			LastTimestamp = timestamp;
			Date = timestamp.Date;
			ZipDirectory = Path.Combine(archivePath, Date.ToString("yyyy-MM"));
			ZipFile = Path.Combine(ZipDirectory, String.Format("{0}Log_{1}.zip", logType, Date.ToString("yyyy-MM-dd")));
			_ms = new MemoryStream(logSize + 3 * 1024);
			_sw = new StreamWriter(_ms);
		}

		/// <summary>
		/// The name of the current log file.
		/// </summary>
		public string LogFileName
		{
			get
			{
				return String.Format("ImageServer{0}_{1}_to_{2}.log", _logType, FirstTimestamp.ToString("yyyy-MM-dd_HH-mm-ss"),
					LastTimestamp.ToString("HH-mm-ss"));
			}
		}
	
		/// <summary>
		/// The memory stream being written to.
		/// </summary>
		public MemoryStream Stream
		{
			get { return _ms; }
		}

		/// <summary>
		/// Write a log to the file.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="timestamp"></param>
		public void Write(TLogClass log, DateTime timestamp)
		{
			_sw.WriteLine(log.ToString());
			_sw.Flush();
			LastTimestamp = timestamp;
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			if (_sw != null)
			{
				_sw.Close();
				_sw.Dispose();
				_sw = null;
			}
			if (_ms != null)
			{
				_ms.Close();
				_ms.Dispose();
				_ms = null;
			}
		}
	}

	/// <summary>
	/// Simple writer class for archiving log data into zip files.
	/// </summary>
	/// <remarks>
	/// The class assumes that logs are supplied in increasing time order.  It will
	/// write a max of 10MB to a specific log file, then create a new log file with the
	/// name containing a time stamp.
	/// </remarks>
	public class ImageServerLogWriter<TLogClass> : IDisposable
	{
		private readonly string _logDirectory;
		private int _logSize = 10*1024*1024;
		private ImageServerLogFile<TLogClass> _archiveLog;
		private readonly string _logType;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logDirectory">The filesystem directory to place the log files into.</param>
		/// <param name="logType">A textual description of the type of log being archived.</param>
		public ImageServerLogWriter(string logDirectory, string logType)
		{
			_logDirectory = logDirectory;
			_logType = logType;
		}

		/// <summary>
		/// The size of the log files to be generated.
		/// </summary>
		public int LogFileSize
		{
			get { return _logSize; }
			set { _logSize = value; }
		}

		/// <summary>
		/// Write to the log.
		/// </summary>
		/// <param name="log">The class instance to log.</param>
		/// <param name="timestamp">The timestamp associated with the log.</param>
		/// <returns>true, if the logs have been flushed.</returns>
		public bool WriteLog(TLogClass log, DateTime timestamp)
		{
			DateTime logDate = timestamp.Date;
			if (_archiveLog == null)
			{
				_archiveLog = new ImageServerLogFile<TLogClass>(timestamp, _logDirectory, LogFileSize, _logType);
				Platform.Log(LogLevel.Info, "Starting archival of {0} logs for {1}",_logType, _archiveLog.FirstTimestamp.ToLongDateString());
			}

			if (logDate.Equals(_archiveLog.Date))
			{
				_archiveLog.Write(log, timestamp);
				if (_archiveLog.Stream.Length > LogFileSize)
				{
					FlushLog();
					return true;
				}
				return false;
			}

			// Flush the current log
			FlushLog();
		
			// Simple recursive call to rewrite, since the log has been flushed, will only go 1 deep
			// on the recursion because FlushLog set _archiveLog = null.
			WriteLog(log, timestamp);

			return true;
		}

		/// <summary>
		/// Routine for flushing the log file to the correct zip file.
		/// </summary>
		public void FlushLog()
		{
			if (_archiveLog == null) return;

			Platform.Log(LogLevel.Info, "Flushing log of {0} for {1}", _logType, _archiveLog.FirstTimestamp.ToLongDateString());

			if (!Directory.Exists(_logDirectory))
				Directory.CreateDirectory(_logDirectory);
			if (!Directory.Exists(_archiveLog.ZipDirectory))
				Directory.CreateDirectory(_archiveLog.ZipDirectory);

            var zipService = Platform.GetService<IZipService>();
			using (var zipServiceWriter = zipService.OpenWrite(_archiveLog.ZipFile))
			{
                zipServiceWriter.ForceCompress = true;

                var comment = 
					String.Format("Log of {0} from {1} to {2}", _logType, _archiveLog.FirstTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
					              _archiveLog.LastTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                zipServiceWriter.AddFileStream(_archiveLog.LogFileName, _archiveLog.Stream, comment);
			    
                zipServiceWriter.Save();
			}

			_archiveLog.Dispose();
			_archiveLog = null;
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			if (_archiveLog != null)
			{
				FlushLog();
			}
		}
	}
}
