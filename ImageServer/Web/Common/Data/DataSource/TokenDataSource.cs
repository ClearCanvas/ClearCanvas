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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Services.Common;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class TokenDataSource : BaseDataSource
	{
        #region Private Members

		private int _resultCount;
        #endregion Private Members

        #region Public Members
        public delegate void TokenFoundSetDelegate(IList<TokenRowData> list);
        public TokenFoundSetDelegate TokenFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }
        #endregion

        #region Private Methods

        private IList<TokenRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            Array tokenRowData = null;
            Array tokenRowDataRange = Array.CreateInstance(typeof(TokenRowData), maximumRows);

            resultCount = 0;

            if (maximumRows == 0) return new List<TokenRowData>();

            using(AuthorityManagement service = new AuthorityManagement())
            {
                IList<AuthorityTokenSummary> tokens = service.ListAuthorityTokens();
                List<TokenRowData> tokenRows = CollectionUtils.Map<AuthorityTokenSummary, TokenRowData>(
                    tokens, delegate(AuthorityTokenSummary token)
                           {
                               TokenRowData row = new TokenRowData(token);
                               return row;
                           });

                tokenRowData = CollectionUtils.ToArray(tokenRows);

                int copyLength = adjustCopyLength(startRowIndex, maximumRows, tokenRowData.Length);

                Array.Copy(tokenRowData, startRowIndex, tokenRowDataRange, 0, copyLength);

                if(copyLength < tokenRowDataRange.Length)
                {
                    tokenRowDataRange = resizeArray(tokenRowDataRange, copyLength);
                }
            };

            if (tokenRowData != null)
            {
                resultCount = tokenRowData.Length;
            }

            return CollectionUtils.Cast<TokenRowData>(tokenRowDataRange);
        }

        #endregion Private Methods

        #region Public Methods
        public IEnumerable<TokenRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<TokenRowData> _list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (TokenFoundSet != null)
                TokenFoundSet(_list);

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
    public class TokenRowData
    {
        private string _name;
        private string _description;

        public TokenRowData(AuthorityTokenSummary token)
        {
            Name = token.Name;
            Description = token.Description;
        }

        public TokenRowData()
        {
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }

        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        } 
    }
}