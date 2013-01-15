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

using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// DatabaseVersion entity
    /// </summary>	
    public class PersistentStoreVersion : Core.Entity
    {
        #region Private fields
     	
        private string _major;
        private string _minor;
        private string _build;
        private string _revision;
	  	
        #endregion
	  	
        #region Constructors
	  	
        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public PersistentStoreVersion()
        {
        }

        /// <summary>
        /// All fields constructor
        /// </summary>
        public PersistentStoreVersion(string major1, string minor1, string build1, string revision1)
        {
            _major = major1;
            _minor = minor1;
            _build = build1;		  	
            _revision = revision1;		  	
        }
		
        #endregion
	  	
        #region Public Properties

        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Major
        {
            get { return _major; }
            set { _major = value; }
        }

        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Build
        {
            get { return _build; }
            set { _build = value; }
        }		
		
        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }
	  	
        #endregion
    }
}