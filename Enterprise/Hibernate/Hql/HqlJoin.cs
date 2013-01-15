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

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public enum HqlJoinMode
    {
        Inner,
        Right,
        Left
    }

    public class HqlJoin : HqlElement
    {
        private readonly string _alias;
        private readonly string _source;
        private readonly HqlJoinMode _mode;
        private readonly bool _fetch;

        public HqlJoin(string source, string alias)
            :this(source, alias, HqlJoinMode.Inner, false)
        {
        }

        public HqlJoin(string source, string alias, HqlJoinMode mode)
            : this(source, alias, mode, false)
        {
        }

        public HqlJoin(string source, string alias, HqlJoinMode mode, bool fetch)
        {
            _source = source;
            _alias = alias;
            _mode = mode;
            _fetch = fetch;
        }

    	public string Alias
    	{
    		get { return _alias; }
    	}

    	public string Source
    	{
    		get { return _source; }
    	}

    	public HqlJoinMode Mode
    	{
    		get { return _mode; }
    	}

    	public bool Fetch
    	{
    		get { return _fetch; }
    	}

    	public override string Hql
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (_mode != HqlJoinMode.Inner)
                    sb.AppendFormat("{0} ", _mode.ToString().ToLower());

                sb.Append("join ");
                if (_fetch)
                    sb.Append("fetch ");
                sb.Append(_source);
                if (!string.IsNullOrEmpty(_alias))
                    sb.AppendFormat(" {0}", _alias);

                return sb.ToString();
            }
        }
    }
}
