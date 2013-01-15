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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Note entity
    /// </summary>
	public partial class Note
    {
        #region Constructors

        /// <summary>
        /// Constructor for creating a new note with recipients.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="author"></param>
        /// <param name="onBehalfOf"></param>
        /// <param name="body"></param>
        /// <param name="urgent"></param>
        public Note(string category, Staff author, StaffGroup onBehalfOf, string body, bool urgent)
        {
            _category = category;
            _author = author;
        	_onBehalfOfGroup = onBehalfOf;
            _body = body;
        	_urgent = urgent;
            _postings = new HashedSet<NotePosting>();

            _creationTime = Platform.Time;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this note has been posted.
        /// </summary>
        public virtual bool IsPosted
        {
            get { return _postTime != null; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Posts the note with no recipients.
        /// </summary>
        public virtual void Post()
        {
            Post(Platform.Time, new Staff[]{}, new StaffGroup[]{});
        }

		/// <summary>
		/// Posts the note to the specified recipients.
		/// </summary>
		/// <param name="staffRecipients"></param>
		/// <param name="groupRecipients"></param>
		public virtual void Post(IEnumerable<Staff> staffRecipients, IEnumerable<StaffGroup> groupRecipients)
		{
			Post(Platform.Time, staffRecipients, groupRecipients);
		}

		/// <summary>
		/// Gets a value indicating whether this note can (should) be acknowledged by the specified staff.
		/// </summary>
		/// <param name="staff"></param>
		/// <returns></returns>
		public virtual bool CanAcknowledge(Staff staff)
		{
			return GetAcknowledgeablePostings(staff).Count > 0;
		}

        /// <summary>
        /// Marks this note as being acknowledged by the specified staff.
        /// </summary>
        /// <remarks>
        /// If the specified staff is not a recipient of the note,
        /// and is not a member of a <see cref="StaffGroup"/> that is a recipient,
        /// a <see cref="WorkflowException"/> will be thrown.
        /// </remarks>
        /// <param name="staff"></param>
        public virtual void Acknowledge(Staff staff)
        {
            // cannot acknowledge if not posted
            if(!IsPosted)
                throw new WorkflowException("Cannot acknowledge a note that has not been posted.");


            // find all un-acknowledged postings that this staff person could acknowledge
            var acknowledgeablePostings = GetAcknowledgeablePostings(staff);

            // if none, this is a workflow exception
            if(acknowledgeablePostings.Count == 0)
                throw new NoteAcknowledgementException("The specified staff was either not a recipient of this note, or the note has already been acknowledged.");

            // acknowledge the posting
            foreach (var posting in acknowledgeablePostings)
            {
                posting.Acknowledge(staff);
            }

            // update the 'fully acknowledged' status of this note
            _isFullyAcknowledged = CollectionUtils.TrueForAll(_postings, posting => posting.IsAcknowledged);
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
			_postTime = _postTime.HasValue ? _postTime.Value.AddMinutes(minutes) : _postTime;
		}

    	#endregion

        #region Overridables

        /// <summary>
        /// Called from <see cref="Post()"/>, allowing subclasses to cancel the post operation by throwing an exception.
        /// </summary>
        /// <remarks>
        /// Override this method to perform validation prior to posting.  Throw an exception to cancel the post.
        /// </remarks>
        protected virtual void BeforePost()
        {

        }

        #endregion


        #region Helpers

        /// <summary>
        /// Posts the note with the specified post-time.
        /// </summary>
        /// <param name="postTime"></param>
        /// <param name="staffRecipients"></param>
        /// <param name="groupRecipients"></param>
        private void Post(DateTime postTime, IEnumerable<Staff> staffRecipients, IEnumerable<StaffGroup> groupRecipients)
        {
            // create postings for any recipients
			foreach (var recipient in staffRecipients)
            {
            	NotePosting posting = new StaffNotePosting(postTime, this, false, null, recipient);
                _postings.Add(posting);
            }
			foreach (var recipient in groupRecipients)
			{
				NotePosting posting = new GroupNotePosting(postTime, this, false, null, recipient);
				_postings.Add(posting);
			}

			if (_postings.Count > 0)
				_hasPostings = true;

            // give subclass a chance to do some processing
            BeforePost();

            // set the post time
            _postTime = postTime;
        }

		private List<NotePosting> GetAcknowledgeablePostings(Staff staff)
		{
			return CollectionUtils.Select(_postings, posting => posting.CanAcknowledge(staff));
		}
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
        }

        #endregion
    }
}
