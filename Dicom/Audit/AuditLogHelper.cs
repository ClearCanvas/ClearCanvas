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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Provides static helper methods to records events in the audit log.
	/// </summary>
	public static class AuditLogHelper
	{
		private const string _messageAuditFailed = "Event Audit Failed.";

		private static readonly object _syncroot = new object();
		private static AuditLog _log;
		private static bool _initialized = false;

		public static AuditLog AuditLog
		{
			get
			{
				if (_initialized) return _log;
				lock (_syncroot)
				{
					if (_initialized) return _log;
					try
					{
						_log = new AuditLog(ProductInformation.Component, "DICOM");
					}
					catch (NotSupportedException)
					{
						Platform.Log(LogLevel.Warn, "No audit sink extensions found - Auditing will be disabled for the remainder of the session.");
					}
					_initialized = true;
					return _log;
				}
			}
		}

		private static bool AuditingEnabled
		{
			get { return AuditLog != null; }
		}

		/// <summary>
		/// Logs an event to the audit log using the format as described in DICOM Supplement 95.
		/// </summary>
		/// <param name="message">The audit message to log.</param>
		public static void Log(DicomAuditHelper message)
		{
			if (AuditingEnabled)
			{
				AuditLog.WriteEntry(message.Operation, message.Serialize(false));
			}
		}

		/// <summary>
		/// Logs an event to the audit log using the format as described in DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// Use this overload to explicitly specify the user and session ID.
		/// </remarks>
		/// <param name="message">The audit message to log.</param>
		/// <param name="username"></param>
		/// <param name="sessionId"></param>
		private static void Log(DicomAuditHelper message, string username, string sessionId)
		{
			if (AuditingEnabled)
			{
				AuditLog.WriteEntry(message.Operation, message.Serialize(false), username, sessionId);
			}
		}

		/// <summary>
		/// Generates a "User Authentication" login event in the audit log, according to DICOM Supplement 95,
		/// and a "Security Alert" event if the operation failed.
		/// </summary>
		/// <param name="username">The username or asserted username of the account that was logged in.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogin(string username, EventResult eventResult)
		{
			LogLogin(username, null, eventResult);
		}

		/// <summary>
		/// Generates a "User Authentication" login event in the audit log, according to DICOM Supplement 95,
		/// and a "Security Alert" event if the operation failed.
		/// </summary>
		/// <param name="username">The username or asserted username of the account that was logged in.</param>
		/// <param name="authenticationServer">The authentication server against which the operation was performed.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogin(string username, EventSource authenticationServer, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var currentProcess = EventSource.GetUserEventSource(LocalHostname); // record that the current process is the one that identified the authentication event
				var auditHelper = new UserAuthenticationAuditHelper(currentProcess, eventResult, UserAuthenticationEventType.Login);
				auditHelper.AddUserParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
				if (authenticationServer != null)
					auditHelper.AddNode(authenticationServer);

				Log(auditHelper);

				if (eventResult != EventResult.Success)
				{
					var alertAuditHelper = new SecurityAlertAuditHelper(currentProcess, eventResult, SecurityAlertEventTypeCodeEnum.NodeAuthentication);
					alertAuditHelper.AddReportingUser(currentProcess);
					alertAuditHelper.AddActiveParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
					Log(alertAuditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "User Authentication" logout event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="username">The username or asserted username of the account that was logged out.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sessionId">The ID of the session that is being logged out.</param>
		public static void LogLogout(string username, string sessionId, EventResult eventResult)
		{
			LogLogout(username, sessionId, null, eventResult);
		}

		/// <summary>
		/// Generates a "User Authentication" logout event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="username">The username or asserted username of the account that was logged out.</param>
		/// <param name="authenticationServer">The authentication server against which the operation was performed.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sessionId">The ID of the session that is being logged out.</param>
		public static void LogLogout(string username, string sessionId, EventSource authenticationServer, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var currentProcess = EventSource.GetUserEventSource(LocalHostname); // record that the current process is the one that identified the authentication event
				var auditHelper = new UserAuthenticationAuditHelper(currentProcess, eventResult, UserAuthenticationEventType.Logout);
				auditHelper.AddUserParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
				if (authenticationServer != null)
					auditHelper.AddNode(authenticationServer);

				Log(auditHelper, username, sessionId);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a (received) "Query" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="localAETitle">The local application entity receiving the query.</param>
		/// <param name="remoteAETitle">The application entity that issued the query.</param>
		/// <param name="remoteHostName">The hostname of the application entity that issued the query.</param>
		/// <param name="currentAE">The current DICOM AE performing the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sopClassUid">The SOP Class Uid of the type of DICOM Query being received.</param>
		/// <param name="ds">The dataset containing the DICOM query received.</param>
		public static void LogQueryReceived(string localAETitle, string remoteAETitle, string remoteHostName, EventSource currentAE, EventResult eventResult, string sopClassUid, DicomAttributeCollection ds)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var auditHelper = new QueryAuditHelper(currentAE, eventResult,
				                                       remoteAETitle ?? localAETitle, remoteHostName ?? LocalHostname, localAETitle, LocalHostname,
				                                       sopClassUid, ds);
				Log(auditHelper);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates an (issued) "Query" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="localAETitle">The local application entity making the query.</param>
		/// <param name="remoteAETitle">The application entity on which the query is taking place.</param>
		/// <param name="remoteHostName">The hostname of the application entity on which the query is taking place.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="currentAE">The current DICOM AE performing the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sopClassUid">The SOP Class Uid of the type of DICOM Query being issued</param>
		/// <param name="ds">The dataset containing the DICOM query being issued</param>
		public static void LogQueryIssued(string localAETitle, string remoteAETitle, string remoteHostName, EventSource eventSource, EventSource currentAE, EventResult eventResult, string sopClassUid, DicomAttributeCollection ds)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var auditHelper = new QueryAuditHelper(eventSource, eventResult,
				                                       localAETitle, LocalHostname, remoteAETitle ?? localAETitle,
				                                       remoteHostName ?? LocalHostname,
				                                       sopClassUid, ds);
				if (eventSource != currentAE)
					auditHelper.AddOtherParticipant(currentAE);
				Log(auditHelper);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" read event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogOpenStudies(IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var aeTitlesArray = ToArray(aeTitles);
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationContentsEventActionCode.R);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" create event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogCreateInstances(IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var aeTitlesArray = ToArray(aeTitles);
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationContentsEventActionCode.C);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" update event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogUpdateInstances(IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var aeTitlesArray = ToArray(aeTitles);
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationContentsEventActionCode.U);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Begin Transferring DICOM Instances" send event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="localAETitle">The local application entity from which the transfer was started.</param>
		/// <param name="remoteAETitle">The application entity to which the transfer was started.</param>
		/// <param name="remoteHostName">The hostname of the application entity to which the transfer was started.</param>
		/// <param name="instances">The studies that were queued for transfer.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogBeginSendInstances(string localAETitle, string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new BeginTransferringDicomInstancesAuditHelper(eventSource, eventResult,
					                                                                 localAETitle, LocalHostname, remoteAETitle ?? localAETitle, remoteHostName ?? LocalHostname, patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "DICOM Instances Transferred" sent event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="localAETitle">The local application entity from which the transfer was completed.</param>
		/// <param name="remoteAETitle">The application entity to which the transfer was completed.</param>
		/// <param name="remoteHostName">The hostname of the application entity to which the transfer was completed.</param>
		/// <param name="instances">The studies that were transferred.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogSentInstances(string localAETitle, string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomInstancesTransferredAuditHelper(eventSource, eventResult, EventReceiptAction.ActionUnknown,
					                                                           localAETitle, LocalHostname, remoteAETitle ?? localAETitle, remoteHostName ?? LocalHostname);
					auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Begin Transferring DICOM Instances" receive event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="localAETitle">The local application entity to which the transfer was started.</param>
		/// <param name="remoteAETitle">The application entity from which the transfer was started.</param>
		/// <param name="remoteHostName">The hostname of the application entity from which the transfer was started.</param>
		/// <param name="instances">The studies that were requested for transfer.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogBeginReceiveInstances(string localAETitle, string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new BeginTransferringDicomInstancesAuditHelper(eventSource, eventResult,
					                                                                 remoteAETitle ?? localAETitle, remoteHostName ?? LocalHostname, localAETitle, LocalHostname, patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "DICOM Instances Transferred" received event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="localAETitle">The local application entity to which the transfer was completed.</param>
		/// <param name="remoteAETitle">The application entity from which the transfer was completed.</param>
		/// <param name="remoteHostName">The hostname of the application entity from which the transfer was completed.</param>
		/// <param name="instances">The studies that were transferred.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="action">The action taken on the studies that were transferred.</param>
		public static void LogReceivedInstances(string localAETitle, string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult, EventReceiptAction action)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomInstancesTransferredAuditHelper(eventSource, eventResult, action,
					                                                           remoteAETitle ?? localAETitle, remoteHostName ?? LocalHostname, localAETitle, LocalHostname);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Data Import" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// One audit event is generated for each file system volume from which data is imported.
		/// If the audited instances are not on a file system, a single event is generated with an empty media identifier.
		/// </remarks>
		/// <param name="instances">The files that were imported.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="currentAE">The current DICOM AE performing the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogImportStudies(AuditedInstances instances, EventSource eventSource, EventSource currentAE, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var fileVolumes = new List<string>(instances.EnumerateFileVolumes());
				if (fileVolumes.Count == 0)
					fileVolumes.Add(string.Empty);

				foreach (var volume in fileVolumes)
				{
					var auditHelper = new DataImportAuditHelper(eventSource, eventResult, volume);
					auditHelper.AddImporter(eventSource);
					if (eventSource != currentAE)
						auditHelper.AddImporter(currentAE);
					foreach (var patient in instances.EnumeratePatients())
						auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies())
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Data Export" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// One audit event is generated for each file system volume to which data is exported.
		/// If the audited instances are not on a file system, a single event is generated with an empty media identifier.
		/// </remarks>
		/// <param name="instances">The files that were exported.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="currentAE">The current DICOM AE performing the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogExportStudies(AuditedInstances instances, EventSource eventSource, EventSource currentAE, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var fileVolumes = new List<string>(instances.EnumerateFileVolumes());
				if (fileVolumes.Count == 0)
					fileVolumes.Add(string.Empty);

				foreach (var volume in fileVolumes)
				{
					var auditHelper = new DataExportAuditHelper(eventSource, eventResult, volume);
					auditHelper.AddExporter(eventSource);
					if (eventSource != currentAE)
						auditHelper.AddExporter(currentAE);
					foreach (var patient in instances.EnumeratePatients())
						auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies())
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Study Deleted" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="aeTitle">The application entity from which the instances were deleted.</param>
		/// <param name="instances">The studies that were deleted.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogDeleteStudies(string aeTitle, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomStudyDeletedAuditHelper(eventSource, eventResult);
					auditHelper.AddUserParticipant(eventSource);
					auditHelper.AddUserParticipant(new AuditProcessActiveParticipant(aeTitle));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" update event in the audit log (with ActionCode of Delete), according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// 
		/// We chose to impleemnt the DicomInstancesAccessed audit log, as opposed to the DicomStudyDeleted audit message because the whole
		/// study isn't being deleted, just a series.
		/// </remarks>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that the series belong that are being deleted.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogDeleteSeries(IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!AuditingEnabled)
				return;

			try
			{
				var aeTitlesArray = ToArray(aeTitles);
				foreach (var patient in instances.EnumeratePatients())
				{
					var auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationContentsEventActionCode.D);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (var study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		private static string LocalHostname
		{
			get { return Dns.GetHostName(); }
		}

		private static T[] ToArray<T>(IEnumerable<T> source)
		{
			// prevent as much unnecessary list copying as is possible
			if (source is T[])
				return (T[]) source;
			if (source is List<T>)
				return ((List<T>) source).ToArray();
			return new List<T>(source).ToArray();
		}
	}
}