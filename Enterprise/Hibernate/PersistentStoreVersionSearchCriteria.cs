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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Search criteria for <see cref="PersistentStoreVersion"/> class.
    /// </summary>
    public class PersistentStoreVersionSearchCriteria : EntitySearchCriteria<PersistentStoreVersion>
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public PersistentStoreVersionSearchCriteria()
        {
        }
	
        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public PersistentStoreVersionSearchCriteria(string key)
            :base(key)
        {
        }
		
        /// <summary>
        /// Copy constructor
        /// </summary>
        protected PersistentStoreVersionSearchCriteria(PersistentStoreVersionSearchCriteria other)
            :base(other)
        {
        }
		
        public override object Clone()
        {
            return new PersistentStoreVersionSearchCriteria(this);
        }
		
        public ISearchCondition<string> Major
        {
            get
            {
                if(!SubCriteria.ContainsKey("Major"))
                {
                    SubCriteria["Major"] = new SearchCondition<string>("Major");
                }
                return (ISearchCondition<string>)SubCriteria["Major"];
            }
        }
	  	
        public ISearchCondition<string> Minor
        {
            get
            {
                if(!SubCriteria.ContainsKey("Minor"))
                {
                    SubCriteria["Minor"] = new SearchCondition<string>("Minor");
                }
                return (ISearchCondition<string>)SubCriteria["Minor"];
            }
        }
	  	
        public ISearchCondition<string> Build
        {
            get
            {
                if(!SubCriteria.ContainsKey("Build"))
                {
                    SubCriteria["Build"] = new SearchCondition<string>("Build");
                }
                return (ISearchCondition<string>)SubCriteria["Build"];
            }
        }
	  	
        public ISearchCondition<string> Revision
        {
            get
            {
                if(!SubCriteria.ContainsKey("Revision"))
                {
                    SubCriteria["Revision"] = new SearchCondition<string>("Revision");
                }
                return (ISearchCondition<string>)SubCriteria["Revision"];
            }
        }	  	
    }
}