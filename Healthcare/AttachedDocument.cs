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
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{

	public class AttachedDocumentCreationArgs
	{
		public string MimeType { get; set; }
		public string FileExtension { get; set; }
		public string LocalContentFilePath { get; set; }
	}


	/// <summary>
	/// AttachedDocument entity
	/// </summary>
	public partial class AttachedDocument
	{
		/// <summary>
		/// Creates a <see cref="AttachedDocument"/> instance, uploading the specified file to the document store.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="documentStore"></param>
		/// <returns></returns>
		public static AttachedDocument Create(AttachedDocumentCreationArgs args, IAttachedDocumentStore documentStore)
		{
			// create the new document object, and put the remote file
			var document = new AttachedDocument
				{
					MimeType = args.MimeType,
					FileExtension = args.FileExtension,
				};

			document.PutFile(documentStore, args.LocalContentFilePath);
			return document;
		}




		/// <summary>
		/// Copy constructor that creates either an exact copy, or an "unprocessed" copy.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="unprocessed"></param>
		protected AttachedDocument(AttachedDocument source, bool unprocessed)
		{
			_creationTime = unprocessed ? Platform.Time : source.CreationTime;
			_mimeType = source.MimeType;
			_fileExtension = source.FileExtension;

			_contentUrl = source.ContentUrl;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Duplicates this document.
		/// </summary>
		/// <remarks>
		/// Does not duplicate the remote resource. However, the remote resource is treated as immutable,
		/// so having multiple instances referring to the same remote resource poses no problems.
		/// </remarks>
		/// <returns></returns>
		public virtual AttachedDocument Duplicate(bool unprocessed)
		{
			return new AttachedDocument(this, unprocessed);
		}

		/// <summary>
		/// Creates a ghost copy of this document.
		/// </summary>
		/// <returns></returns>
		public virtual AttachedDocument CreateGhostCopy()
		{
			return new AttachedDocument(this, false) { GhostOf = this };
		}

		/// <summary>
		/// Gets the time at which this document was received.
		/// </summary>
		/// <remarks>
		/// By default, this property returns the same value as <see cref="CreationTime"/>.
		/// </remarks>
		public virtual DateTime? DocumentReceivedTime
		{
			get { return _creationTime; }
		}

		/// <summary>
		/// Marks this document as having been attached.
		/// </summary>
		public virtual void Attach()
		{
			// nothing to do
		}

		/// <summary>
		/// Marks this document as being detached.
		/// </summary>
		public virtual void Detach()
		{
			// nothing to do
		}

		/// <summary>
		/// Summary of derived-class specific details of the attached document
		/// </summary>
		public virtual IDictionary<string, string> DocumentHeaders
		{
			get { return new Dictionary<string, string>(); }
		}

		/// <summary>
		/// Gets the type of attached document (e.g. Fax, Scanned, etc.).
		/// </summary>
		/// <remarks>
		/// By default this property returns a value of "Document".
		/// </remarks>
		public virtual string DocumentTypeName
		{
			get { return "Document"; }
		}

		/// <summary>
		/// Gets the file associated with this attached document, from the document store.
		/// </summary>
		/// <returns></returns>
		public virtual string GetFile(IAttachedDocumentStore documentStore)
		{
			return documentStore.GetDocument(_contentUrl);
		}

		/// <summary>
		/// Sets the file associated with this attached document, and stores a copy to the document store.
		/// </summary>
		/// <returns></returns> 
		public virtual void PutFile(IAttachedDocumentStore documentStore, string localFilePath)
		{
			const string pathDelimiter = "/";

			var builder = new StringBuilder();
			builder.Append(_creationTime.Year.ToString());
			builder.Append(pathDelimiter);
			builder.Append(_creationTime.Month.ToString());
			builder.Append(pathDelimiter);
			builder.Append(_creationTime.Day.ToString());
			builder.Append(pathDelimiter);

			// important that we always generate a new GUID here, because multiple AttachedDocument objects
			// are allowed to refer to the same remote resource - therefore we must treat the remote resource
			// as immutable
			builder.AppendFormat("{0}.{1}", Guid.NewGuid().ToString("D"), _fileExtension);

			_contentUrl = builder.ToString();
			documentStore.PutDocument(_contentUrl, localFilePath);
		}



		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_creationTime = _creationTime.AddMinutes(minutes);
		}
	}
}