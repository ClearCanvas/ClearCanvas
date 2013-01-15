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
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(ProcedureStepBuilderExtensionPoint))]
	public class RegistrationProcedureStepBuilder : ProcedureStepBuilderBase
	{

		public override Type ProcedureStepClass
		{
			get { return typeof(RegistrationProcedureStep); }
		}

		public override ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure)
		{
			return new RegistrationProcedureStep();
		}

		public override void SaveInstance(ProcedureStep prototype, XmlElement xmlNode)
		{
		}
	}

	public class RegistrationProcedureStep : ProcedureStep
	{
		public RegistrationProcedureStep(Procedure procedure)
			: base(procedure)
		{
		}

		/// <summary>
		/// Default no-args constructor required by NHibernate
		/// </summary>
		public RegistrationProcedureStep()
		{
		}

		public override string Name
		{
			get { return "Registration"; }
		}

		public override bool CreateInDowntimeMode
		{
			get { return true; }
		}

		public override bool IsPreStep
		{
			get { return true; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.Zero; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			return new List<Procedure>();
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new RegistrationProcedureStep(this.Procedure);
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// registration steps do not have related steps
			return false;
		}
	}
}
