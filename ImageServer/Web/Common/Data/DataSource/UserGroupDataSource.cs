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
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{

	public class UserGroupDataSource : BaseDataSource
	{
        #region Private Members

		private string _groupName;

        private int _resultCount;

        #endregion Private Members

        #region Public Members
        public delegate void UserGroupFoundSetDelegate(IList<UserGroupRowData> list);
        public UserGroupFoundSetDelegate UserGroupFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }

	    public string GroupName
	    {
            get { return _groupName; }
            set { _groupName = value; }	        
	    }

        #endregion

        #region Private Methods

        private IList<UserGroupRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            Array authorityRowData;
            Array authorityRowDataRange = Array.CreateInstance(typeof(UserGroupRowData), maximumRows);

            resultCount = 0;

            if (maximumRows == 0) return new List<UserGroupRowData>();

            using(AuthorityManagement service = new AuthorityManagement())
            {
                IList<AuthorityGroupSummary> list = service.ListAllAuthorityGroups();
                IList<AuthorityGroupSummary> filteredList = new List<AuthorityGroupSummary>();

                if(!string.IsNullOrEmpty(GroupName))
                {
                	string matchString = GroupName;

					while (matchString.StartsWith("*") || matchString.StartsWith("?"))
						matchString = matchString.Substring(1);
					while (matchString.EndsWith("*")||matchString.EndsWith("?"))
						matchString = matchString.Substring(0, matchString.Length - 1);

					matchString = matchString.Replace("*", "[A-Za-z0-9]*");
					matchString = matchString.Replace("?", ".");

                    foreach(AuthorityGroupSummary group in list)
                    {
						if (Regex.IsMatch(group.Name,matchString,RegexOptions.IgnoreCase))
							filteredList.Add(group);
                    }
                } 
				else
                {
                    filteredList = list;
                }

                List<UserGroupRowData> rows = CollectionUtils.Map<AuthorityGroupSummary, UserGroupRowData>(
                    filteredList, delegate(AuthorityGroupSummary group)
                              {
                                  UserGroupRowData row =
                                      new UserGroupRowData(service.LoadAuthorityGroupDetail(group.AuthorityGroupRef));
                                  return row;
                              });

                authorityRowData = CollectionUtils.ToArray(rows);

                int copyLength = adjustCopyLength(startRowIndex, maximumRows, authorityRowData.Length);

                Array.Copy(authorityRowData, startRowIndex, authorityRowDataRange, 0, copyLength);

                if (copyLength < authorityRowDataRange.Length)
                {
                    authorityRowDataRange = resizeArray(authorityRowDataRange, copyLength);
                }
            };

            resultCount = authorityRowData.Length;

            return CollectionUtils.Cast<UserGroupRowData>(authorityRowDataRange);
        }

        #endregion Private Methods

        #region Public Methods
        public IEnumerable<UserGroupRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<UserGroupRowData> _list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (UserGroupFoundSet != null)
                UserGroupFoundSet(_list);

            return _list;

        }

        public int SelectCount()
        {
            if (ResultCount != 0) return ResultCount;

            // Ignore the search results
            InternalSelect(0, 1, out _resultCount);

            return ResultCount;
        }

        #endregion Public Methods
    }

    [Serializable]
    public class UserGroupRowData
    {
        private int _tokenCount;
        private List<TokenSummary> _tokens = new List<TokenSummary>();

        public UserGroupRowData() {}
        
        public UserGroupRowData(AuthorityGroupDetail group)
        {
            Ref = group.AuthorityGroupRef.Serialize();
            Name = group.Name;
            Description = group.Description;
            DataGroup = group.DataGroup;
            foreach(AuthorityTokenSummary token in group.AuthorityTokens)
            {
                Tokens.Add(new TokenSummary(token.Name, token.Description));
            }

            TokenCount = group.AuthorityTokens.Count;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Ref { get; set; }

        public bool DataGroup { get; set; }

        public string Password { get; set; }

        public int TokenCount
        {
            get { return _tokenCount; }
            set { _tokenCount = value; }
        }

        public List<TokenSummary> Tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }
    }

    [Serializable]
    public class TokenSummary
    {
        public TokenSummary(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}





