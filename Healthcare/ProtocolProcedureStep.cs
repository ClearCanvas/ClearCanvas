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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public abstract class ProtocolProcedureStep : ProcedureStep
    {
        private Protocol _protocol;

        #region Constructors

        public ProtocolProcedureStep(Protocol protocol)
        {
            Platform.CheckForNullReference(protocol, "protocol");
            _protocol = protocol;
        }

        /// <summary>
        /// Default no-args constructor required by nHibernate
        /// </summary>
        protected ProtocolProcedureStep()
        {
        }

        #endregion

        public virtual Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

		public override bool CreateInDowntimeMode
		{
			get { return false; }
		}

		public override bool IsPreStep
		{
			get { return true; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.MinValue; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			if (_protocol != null)
			{
				return CollectionUtils.Select(_protocol.Procedures,
					delegate(Procedure p) { return !Equals(p, this.Procedure); });
			}
			else
			{
				return new List<Procedure>();
			}
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// can't have relatives if no protocol
			if (this.Protocol == null)
				return false;

			// relatives must be protocol steps
			if (!step.Is<ProtocolProcedureStep>())
				return false;

			// check if tied to same protocol
			ProtocolProcedureStep that = step.As<ProtocolProcedureStep>();
			return that.Protocol != null && Equals(this.Protocol, that.Protocol);
		}
	}
}
