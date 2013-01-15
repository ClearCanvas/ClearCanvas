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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	#region Exception Handler

	[ExceptionPolicyFor(typeof(LoadSopsException))]
	[ExceptionPolicyFor(typeof(LoadStudyException))]

	[ExceptionPolicyFor(typeof(NotFoundLoadStudyException))]
	[ExceptionPolicyFor(typeof(InUseLoadStudyException))]
	[ExceptionPolicyFor(typeof(OfflineLoadStudyException))]
	[ExceptionPolicyFor(typeof(NearlineLoadStudyException))]

	[ExceptionPolicyFor(typeof(StudyLoaderNotFoundException))]
	
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	internal sealed class LoadStudyExceptionHandlingPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LoadStudyExceptionHandlingPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			exceptionHandlingContext.Log(LogLevel.Error, e);

			if (e is LoadSopsException)
				Handle((LoadSopsException)e, exceptionHandlingContext);
			else if (e is StudyLoaderNotFoundException)
				Handle((StudyLoaderNotFoundException)e, exceptionHandlingContext);
		}

		private static void Handle(LoadSopsException e, IExceptionHandlingContext exceptionHandlingContext)
		{
			string message;
			if (e is InUseLoadStudyException)
			{
                message = SR.MessageLoadStudyFailedInUse;
			}
			else if (e is NearlineLoadStudyException)
			{
				message = ((NearlineLoadStudyException) e).IsStudyBeingRestored
                            ? SR.MessageLoadStudyFailedNearline
                            : String.Format("{0}  {1}", SR.MessageLoadStudyFailedNearlineNoRestore, SR.MessageContactPacsAdmin);
			}
			else if (e is OfflineLoadStudyException)
			{
                message = SR.MessageLoadStudyFailedOffline;
			}
			else if (e is NotFoundLoadStudyException)
			{
                message = SR.MessageLoadStudyFailedNotFound;
			}
			else
			{
				if (e.PartiallyLoaded)
					message = String.Format(SR.MessageFormatLoadStudyIncomplete, e.Successful, e.Total);
				else
					message = SR.MessageLoadStudyCompleteFailure;

				message = String.Format("{0} {1}", message, SR.MessageContactPacsAdmin);
			}

			exceptionHandlingContext.ShowMessageBox(message);
		}

		private static void Handle(StudyLoaderNotFoundException e, IExceptionHandlingContext exceptionHandlingContext)
		{
			String message = String.Format("{0} {1}", SR.MessageLoadStudyCompleteFailure, SR.MessageContactPacsAdmin);
			exceptionHandlingContext.ShowMessageBox(message);
		}

		#endregion
	}

	#endregion

	#region Exception Classes

	#region Generic Study Loading Exceptions

	/// <summary>
	/// Exception class for a study that could not be loaded because it is currently in use.
	/// </summary>
	/// <remarks>
	/// In Use can be interpreted very broadly, but generally this means that the study is
	/// currently being processed, either locally or by a remote (e.g. CC Streaming) server.
	/// </remarks>
	public class InUseLoadStudyException : LoadStudyException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		public InUseLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		public InUseLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="innerException">The inner exception.</param>
		public InUseLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public InUseLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The study '{0}' is currently in use and cannot be loaded.", studyInstanceUid);
		}
	}

	/// <summary>
	/// Exception class for a study that could not be loaded because it is currently nearline.
	/// </summary>
	/// <remarks>
	/// A 'nearline' study is one that is not readily available, but can be automatically brought back online
	/// in a reasonable amount of time.  In the case of a CC Streaming server, the fact that the
	/// study headers were requested is enough to trigger a restore of the study from an archive.
	/// Therefore, if you receive this exception, there is nothing that needs to be done to trigger
	/// the retrieval other than waiting a few minutes and trying again.
	/// </remarks>
	public class NearlineLoadStudyException : LoadStudyException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		public NearlineLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		public NearlineLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="innerException">The inner exception.</param>
		public NearlineLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public NearlineLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		/// <summary>
		/// Indicates whether or not the server is going to automatically restore
		/// the nearline study due to the request ... or that just so happens to already be the case.
		/// </summary>
		public bool IsStudyBeingRestored { get; set; }

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The study '{0}' is nearline and cannot be loaded.", studyInstanceUid);
		}
	}

	/// <summary>
	/// Exception class for a study that could not be loaded because it is currently offline.
	/// </summary>
	/// <remarks>
	/// In Dicom, 'offline' generally means that a study cannot be retrieved from archive without manual intervention.
	/// </remarks>
	public class OfflineLoadStudyException : LoadStudyException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		public OfflineLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		public OfflineLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="innerException">The inner exception.</param>
		public OfflineLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public OfflineLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The study '{0}' is offline and cannot be loaded.", studyInstanceUid);
		}
	}

	/// <summary>
	/// Exception class for a study that could not be loaded because it could not be found.
	/// </summary>
	public class NotFoundLoadStudyException : LoadStudyException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		public NotFoundLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		public NotFoundLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="innerException">The inner exception.</param>
		public NotFoundLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public NotFoundLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The specified study '{0}' was not found.", studyInstanceUid);
		}
	}

	/// <summary>
	/// Exception class for a study whose <see cref="Sop"/>s could not be loaded, either partially or totally.
	/// </summary>
	/// <remarks>
	/// This exception will be thrown from <see cref="IImageViewer.LoadStudy(ClearCanvas.ImageViewer.LoadStudyArgs)"/> and <see cref="ImageViewerComponent.LoadStudies"/>
	/// when at least one <see cref="Sop"/> could not be loaded.  It can also be thrown in the case of
	/// an outright failure when the exact reason is unknown (e.g. nearline, offline, in use).
	/// </remarks>
	public class LoadStudyException : LoadSopsException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		public LoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		public LoadStudyException(string studyInstanceUid, string message)
			: this(studyInstanceUid, 0, 0, message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: this(studyInstanceUid, 0, 0, message, innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="total">The total number of <see cref="Sop"/>s in the study.</param>
		/// <param name="failed">The number of <see cref="Sop"/>s that failed to load.</param>
		public LoadStudyException(string studyInstanceUid, int total, int failed)
			: this(studyInstanceUid, total, failed, FormatMessage(studyInstanceUid, total, failed))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="total">The total number of <see cref="Sop"/>s in the study.</param>
		/// <param name="failed">The number of <see cref="Sop"/>s that failed to load.</param>
		/// <param name="message">A custom message.</param>
		public LoadStudyException(string studyInstanceUid, int total, int failed, string message)
			: base(total, failed, message)
		{
			StudyInstanceUid = studyInstanceUid;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <see cref="Exception.Message"/> is automatically determined based on <paramref name="studyInstanceUid"/>.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="total">The total number of <see cref="Sop"/>s in the study.</param>
		/// <param name="failed">The number of <see cref="Sop"/>s that failed to load.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadStudyException(string studyInstanceUid, int total, int failed, Exception innerException)
			: this(studyInstanceUid, total, failed, FormatMessage(studyInstanceUid, total, failed), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance uid.</param>
		/// <param name="total">The total number of <see cref="Sop"/>s in the study.</param>
		/// <param name="failed">The number of <see cref="Sop"/>s that failed to load.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadStudyException(string studyInstanceUid, int total, int failed, string message, Exception innerException)
			: base(total, failed, message, innerException)
		{
			StudyInstanceUid = studyInstanceUid;
		}

		/// <summary>
		/// Gets the study instance uid of the study that failed to load, either partially or totally.
		/// </summary>
		public readonly string StudyInstanceUid;

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("An error occurred while attempting to load study '{0}'.", studyInstanceUid);
		}

		private static string FormatMessage(string studyInstanceUid, int total, int failed)
		{
			return String.Format("{0} of {1} sops failed to load for study '{2}'.", failed, total, studyInstanceUid);
		}
	}

	/// <summary>
	/// Exception class for <see cref="Sop"/>s that could not be loaded, either partially or totally.
	/// </summary>
	/// <remarks>
	/// This exception will be thrown from <see cref="ImageViewerComponent.LoadImages(string[])"/> when one or more
	/// files (not studies) has failed to load.
	/// </remarks>
	public class LoadSopsException : Exception
	{
		private readonly int _total;
		private readonly int _failed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="total">The total number of <see cref="Sop"/>s.</param>
		/// <param name="failed">The number of <see cref="Sop"/>s that failed to load.</param>
		public LoadSopsException(int total, int failed)
			: this(total, failed, FormatMessage(total, failed))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="total">The total number of sops/images.</param>
		/// <param name="failed">The number of sops/images that failed to load.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadSopsException(int total, int failed, Exception innerException)
			: this(total, failed, FormatMessage(total, failed), innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="total">The total number of sops/images.</param>
		/// <param name="failed">The number of sops/images that failed to load.</param>
		/// <param name="message">A custom message.</param>
		public LoadSopsException(int total, int failed, string message)
			: this(message)
		{
			_total = total;
			_failed = failed;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="total">The total number of <see cref="Sop"/>s.</param>
		/// <param name="failed">The number of <see cref="Sop"/>s that failed to load.</param>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadSopsException(int total, int failed, string message, Exception innerException)
			: this(message, innerException)
		{
			_total = total;
			_failed = failed;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">A custom message.</param>
		public LoadSopsException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">A custom message.</param>
		/// <param name="innerException">The inner exception.</param>
		public LoadSopsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Gets or sets the total number of <see cref="Sop"/>s the Framework tried to load.
		/// </summary>
		public int Total
		{
			get { return _total; }
		}

		/// <summary>
		/// Gets or sets the number of <see cref="Sop"/>s that failed to loaded.
		/// </summary>
		public int Failed
		{
			get { return _failed; }
		}

		/// <summary>
		/// Gets the number of <see cref="Sop"/>s that loaded successfully.
		/// </summary>
		public int Successful
		{
			get { return this.Total - this.Failed; }
		}

		/// <summary>
		/// Gets whether or not any <see cref="Sop"/>s were successfully loaded.
		/// </summary>
		public bool AnyLoaded
		{
			get { return Successful > 0; }
		}

		/// <summary>
		/// Gets whether or not the <see cref="Sop"/>s were partially loaded.
		/// </summary>
		/// <remarks>
		/// A return value of true means that at least one <see cref="Sop"/> was loaded successfully,
		/// and at least one failed to load.
		/// </remarks>
		public bool PartiallyLoaded
		{
			get { return AnyLoaded && Failed > 0; }
		}

		private static string FormatMessage(int total, int failed)
		{
			return String.Format("{0} of {1} sops have failed to load.", failed, total);
		}
	}

	#endregion
	#endregion
}
