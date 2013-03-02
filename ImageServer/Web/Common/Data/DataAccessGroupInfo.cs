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

using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class DataAccessGroupInfo
    {
        public string AuthorityGroupRef { get; private set; }
        public string Name { get; private set; }
        public string Description { get; set; }
        public bool HasAccessToCurrentPartition { get; set; }
        public bool CanAccessAllPartitions { get; set; }
        public bool CanAccessAllStudies { get; set; }

        public DataAccessGroupInfo(string authorityGroupRef, string name)
        {
            AuthorityGroupRef = authorityGroupRef;
            Name = name;
        }

        public DataAccessGroupInfo(AuthorityGroupDetail detail)
        {
            Name = detail.Name;
            Description = detail.Description;
            AuthorityGroupRef = detail.AuthorityGroupRef.ToString(false, false);
        }
    }


    public class DataAccessGroupInfoCollection : List<DataAccessGroupInfo>
    {
        public DataAccessGroupInfoCollection(IEnumerable<DataAccessGroupInfo> list)
            : base(list)
        { }

        public bool ContainsGroupWithAllPartitionAccess
        {
            get { return Exists(item => item.CanAccessAllPartitions); }
        }

        public bool ContainsGroupWithAllStudiesAccess
        {
            get { return Exists(item => item.CanAccessAllStudies); }
        }
    }

    public static class DataAccessGroupListItemConverter
    {
        public static ListItem Convert(DataAccessGroupInfo info)
        {
            string displayContent = GetRenderedHtml(info);

            var item = new ListItem(displayContent, info.AuthorityGroupRef);
            item.Attributes["title"] = info.Description;

            item.Selected = info.HasAccessToCurrentPartition;
            item.Enabled = !info.CanAccessAllPartitions;

            return item;
        }

        private static string GetRenderedHtml(DataAccessGroupInfo info)
        {
            StringBuilder html = new StringBuilder();
            html.Append(info.Name);

            if (info.CanAccessAllStudies)
                html.AppendFormat("<span class='GlocalSeeNotesMarker'/> * </span>");

            return html.ToString();

        }
    }

    public class DatagroupComparer : IComparer<DataAccessGroupInfo>
    {
        public int Compare(DataAccessGroupInfo x, DataAccessGroupInfo y)
        {
            if (x.CanAccessAllPartitions)
            {
                if (!y.CanAccessAllPartitions)
                    return -1; //x first

                return x.Name.CompareTo(y.Name); // alphabetically
            }
            else
            {
                if (y.CanAccessAllPartitions)
                    return 1; // y first

                return x.Name.CompareTo(y.Name); // alphabetically
            }
        }
    }

    public class AuthorityGroupCategories
    {
        public IList<AuthorityGroupSummary> RegularAuthorityGroups { get; set; }
        public IList<AuthorityGroupSummary> DataAccessAuthorityGroups { get; set; }

        public IList<AuthorityGroupSummary> All
        {
            get
            {
                var list = new List<AuthorityGroupSummary>();
                list.AddRange(RegularAuthorityGroups);
                list.AddRange(DataAccessAuthorityGroups);
                return list;
            }
        }


    }
}