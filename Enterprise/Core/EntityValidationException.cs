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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    public class EntityValidationException : Exception
    {
    	private readonly string _message;
        private readonly TestResultReason[] _reasons;

        public EntityValidationException(string message, TestResultReason[] reasons)
        {
        	_message = message;
            _reasons = reasons;
        }

        public EntityValidationException(string message)
            :this(message, new TestResultReason[]{})
        {
        }

		public override string Message
		{
			get
			{
				List<string> messages = new List<string>();
				foreach (TestResultReason reason in _reasons)
					messages.AddRange(BuildMessageStrings(reason));

				if (messages.Count > 0)
				{
					return _message + "\n" + StringUtilities.Combine(messages, "\n");
				}
				else
					return _message;
			}
		}

        public TestResultReason[] Reasons
        {
            get { return _reasons; }
        }

        private static List<string> BuildMessageStrings(TestResultReason reason)
        {
            List<string> messages = new List<string>();
            if (reason.Reasons.Length == 0)
                messages.Add(reason.Message);
            else
            {
                foreach (TestResultReason subReason in reason.Reasons)
                {
                    List<string> subMessages = BuildMessageStrings(subReason);
                    foreach (string subMessage in subMessages)
                    {
                        messages.Add(string.Format("{0} {1}", reason.Message, subMessage));
                    }
                }
            }
            return messages;
        }
    }
}
