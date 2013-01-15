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

namespace ClearCanvas.Healthcare
{
    public class WorklistSearchCriteria : EntitySearchCriteria<Worklist>
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public WorklistSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public WorklistSearchCriteria(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected WorklistSearchCriteria(WorklistSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new WorklistSearchCriteria(this);
        }

        public ISearchCondition<string> Name
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Name"))
                {
                    this.SubCriteria["Name"] = new SearchCondition<string>("Name");
                }
                return (ISearchCondition<string>)this.SubCriteria["Name"];
            }
        }

        public ISearchCondition<string> FullClassName
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("FullClassName"))
                {
                    this.SubCriteria["FullClassName"] = new SearchCondition<string>("FullClassName");
                }
                return (ISearchCondition<string>)this.SubCriteria["FullClassName"];
            }
        }
    }
}
