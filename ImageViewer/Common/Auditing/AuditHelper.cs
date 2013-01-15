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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Common.Auditing
{
	/// <summary>
	/// Provides static helper methods to records events in the audit log.
	/// </summary>
	public static class AuditHelper
	{
		/// <summary>
		/// Logs an event to the audit log using the format as described in DICOM Supplement 95.
		/// </summary>
		/// <param name="message">The audit message to log.</param>
		public static void Log(DicomAuditHelper message)
		{
			AuditLogHelper.Log(message);
		}

		/// <summary>
		/// Generates a "User Authentication" login event in the audit log, according to DICOM Supplement 95,
		/// and a "Security Alert" event if the operation failed.
		/// </summary>
		/// <param name="username">The username or asserted username of the account that was logged in.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogin(string username, EventResult eventResult)
		{
			AuditLogHelper.LogLogin(username, eventResult);
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
			AuditLogHelper.LogLogin(username, authenticationServer, eventResult);
		}

		/// <summary>
		/// Generates a "User Authentication" logout event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="username">The username or asserted username of the account that was logged out.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sessionId">The ID of the session that is being logged out.</param>
		public static void LogLogout(string username, string sessionId, EventResult eventResult)
		{
			AuditLogHelper.LogLogout(username, sessionId, eventResult);
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
			AuditLogHelper.LogLogout(username, sessionId, authenticationServer, eventResult);
		}

		/// <summary>
		/// Generates a (received) "Query" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="remoteAETitle">The application entity that issued the query.</param>
		/// <param name="remoteHostName">The hostname of the application entity that issued the query.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sopClassUid">The SOP Class Uid of the type of DICOM Query being received.</param>
		/// <param name="query">The dataset containing the DICOM query received.</param>
		public static void LogQueryReceived(string remoteAETitle, string remoteHostName, EventResult eventResult, string sopClassUid, DicomAttributeCollection query)
		{
			AuditLogHelper.LogQueryReceived(LocalAETitle, remoteAETitle, remoteHostName, EventSource.GetCurrentDicomAE(), eventResult, sopClassUid, query);
		}

		/// <summary>
		/// Generates an (issued) "Query" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="remoteAETitle">The application entity on which the query is taking place.</param>
		/// <param name="remoteHostName">The hostname of the application entity on which the query is taking place.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="sopClassUid">The SOP Class Uid of the type of DICOM Query being issued</param>
		/// <param name="query">The dataset containing the DICOM query being issued</param>
		public static void LogQueryIssued(string remoteAETitle, string remoteHostName, EventSource eventSource, EventResult eventResult, string sopClassUid, DicomAttributeCollection query)
		{
			AuditLogHelper.LogQueryIssued(LocalAETitle, remoteAETitle, remoteHostName, eventSource, EventSource.GetCurrentDicomAE(), eventResult, sopClassUid, query);
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
			AuditLogHelper.LogOpenStudies(aeTitles, instances, eventSource, eventResult);
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
			AuditLogHelper.LogCreateInstances(aeTitles, instances, eventSource, eventResult);
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
			AuditLogHelper.LogUpdateInstances(aeTitles, instances, eventSource, eventResult);
		}

		/// <summary>
		/// Generates a "Begin Transferring DICOM Instances" send event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="remoteAETitle">The application entity to which the transfer was started.</param>
		/// <param name="remoteHostName">The hostname of the application entity to which the transfer was started.</param>
		/// <param name="instances">The studies that were queued for transfer.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogBeginSendInstances(string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			AuditLogHelper.LogBeginSendInstances(LocalAETitle, remoteAETitle, remoteHostName, instances, eventSource, eventResult);
		}

		/// <summary>
		/// Generates a "DICOM Instances Transferred" sent event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="remoteAETitle">The application entity to which the transfer was completed.</param>
		/// <param name="remoteHostName">The hostname of the application entity to which the transfer was completed.</param>
		/// <param name="instances">The studies that were transferred.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogSentInstances(string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			AuditLogHelper.LogSentInstances(LocalAETitle, remoteAETitle, remoteHostName, instances, eventSource, eventResult);
		}

		/// <summary>
		/// Generates a "Begin Transferring DICOM Instances" receive event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="remoteAETitle">The application entity from which the transfer was started.</param>
		/// <param name="remoteHostName">The hostname of the application entity from which the transfer was started.</param>
		/// <param name="instances">The studies that were requested for transfer.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogBeginReceiveInstances(string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			AuditLogHelper.LogBeginReceiveInstances(LocalAETitle, remoteAETitle, remoteHostName, instances, eventSource, eventResult);
		}

		/// <summary>
		/// Generates a "DICOM Instances Transferred" received event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="remoteAETitle">The application entity from which the transfer was completed.</param>
		/// <param name="remoteHostName">The hostname of the application entity from which the transfer was completed.</param>
		/// <param name="instances">The studies that were transferred.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		/// <param name="action">The action taken on the studies that were transferred.</param>
		public static void LogReceivedInstances(string remoteAETitle, string remoteHostName, AuditedInstances instances, EventSource eventSource, EventResult eventResult, EventReceiptAction action)
		{
			AuditLogHelper.LogReceivedInstances(LocalAETitle, remoteAETitle, remoteHostName, instances, eventSource, eventResult, action);
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
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogImportStudies(AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			AuditLogHelper.LogImportStudies(instances, eventSource, EventSource.GetCurrentDicomAE(), eventResult);
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
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogExportStudies(AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			AuditLogHelper.LogExportStudies(instances, eventSource, EventSource.GetCurrentDicomAE(), eventResult);
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
			AuditLogHelper.LogDeleteStudies(aeTitle, instances, eventSource, eventResult);
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
			AuditLogHelper.LogDeleteSeries(aeTitles, instances, eventSource, eventResult);
		}

		/// <summary>
		/// Gets the current or last known AETitle of the local server.
		/// </summary>
		public static string LocalAETitle
		{
			get
			{
				try
				{
					return DicomServer.DicomServer.AETitle;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Unable to retrieve local AE title for auditing.");
					return "<unavailable>";
				}
			}
		}
	}
}