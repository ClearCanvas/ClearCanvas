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
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Printing
{
	/// <summary>
	/// Page model for radiology reports.
	/// </summary>
	public class ReportPageModel : PageModel
	{
		/// <summary>
		/// Practitioner facade.
		/// </summary>
		public class PractitionerFacade
		{
			private readonly ExternalPractitioner _practitioner;
			private readonly ExternalPractitionerContactPoint _contactPoint;

			internal PractitionerFacade(ExternalPractitioner practitioner, ExternalPractitionerContactPoint contactPoint)
			{
				_practitioner = practitioner;
				_contactPoint = contactPoint;
			}

			public NameFacade Name
			{
				get { return new NameFacade(_practitioner.Name); }
			}

			public AddressFacade Address
			{
				get
				{
					var address = (_contactPoint == null ? null : _contactPoint.CurrentAddress) ?? new Address();
					return new AddressFacade(address);
				}
			}

			public override string ToString()
			{
				return this.Name.ToString();
			}
		}

		/// <summary>
		/// Procedure facade.
		/// </summary>
		public class ProcedureFacade
		{
			private readonly Procedure _procedure;

			internal ProcedureFacade(Procedure procedure)
			{
				_procedure = procedure;
			}

			public string Name
			{
				get { return _procedure.Type.Name; }
			}

			public string Code
			{
				get { return _procedure.Type.Id; }
			}

			public override string ToString()
			{
				return this.Name;
			}
		}

		/// <summary>
		/// Procedures facade.
		/// </summary>
		public class ProceduresFacade
		{
			private readonly IList<ProcedureFacade> _procedures;

			internal ProceduresFacade(IEnumerable<Procedure> procedures)
			{
				_procedures = procedures.Select(p => new ProcedureFacade(p)).ToList();
			}

			public int Count
			{
				get { return _procedures.Count; }
			}

			public ProcedureFacade this[int i]
			{
				get { return _procedures[i]; }
			}

			public override string ToString()
			{
				return string.Join(", ", _procedures.Select(rp => rp.Name));
			}
		}

		/// <summary>
		/// Report Part facade.
		/// </summary>
		public class ReportPartFacade
		{
			private static readonly Dictionary<ReportPartStatus, Func<ReportPart, DateTime?>> _timePropertyMap
				= new Dictionary<ReportPartStatus, Func<ReportPart, DateTime?>> 
				  	{
						{ReportPartStatus.X, part => part.CancelledTime},
						{ReportPartStatus.D, part => part.CreationTime},
						{ReportPartStatus.P, part => part.PreliminaryTime},
						{ReportPartStatus.F, part => part.CompletedTime},
				  	};

			private readonly ReportPart _part;
			private readonly ReportPartStatusEnum _status;

			internal ReportPartFacade(ReportPart part)
			{
				_part = part;
				_status = PersistenceScope.CurrentContext.GetBroker<IEnumBroker>().Find<ReportPartStatusEnum>(_part.Status.ToString());
			}

			public int Index
			{
				get { return _part.Index; }
			}

			public string Status
			{
				get { return _status.Value; }
			}

			public string StatusDate
			{
				get { return FormatDate(_timePropertyMap[_part.Status](_part)); }
			}

			public string StatusDateTime
			{
				get { return FormatTime(_timePropertyMap[_part.Status](_part)); }
			}

			public string Body
			{
				get { return GetBody(); }
			}

			public NameFacade InterpretedBy
			{
				get { return _part.Interpreter == null ? null : new NameFacade(_part.Interpreter.Name) ; }
			}

			public NameFacade VerifiedBy
			{
				get { return _part.Verifier == null ? null : new NameFacade(_part.Verifier.Name); }
			}

			public NameFacade TranscribedBy
			{
				get { return _part.Transcriber == null ? null : new NameFacade(_part.Transcriber.Name); }
			}

			public NameFacade SupervisedBy
			{
				get { return _part.Supervisor == null ? null : new NameFacade(_part.Supervisor.Name); }
			}

			public string CreationDateTime
			{
				get { return FormatTime(_part.CreationTime); }
			}

			public string PreliminaryDateTime
			{
				get { return FormatTime(_part.PreliminaryTime); }
			}

			public string CompletedDateTime
			{
				get { return FormatTime(_part.CompletedTime); }
			}

			public string CreationDate
			{
				get { return FormatDate(_part.CreationTime); }
			}

			public string PreliminaryDate
			{
				get { return FormatDate(_part.PreliminaryTime); }
			}

			public string CompletedDate
			{
				get { return FormatDate(_part.CompletedTime); }
			}

			public override string ToString()
			{
				return string.Format("Report Part {0}, {1} - {2}", this.Index, this.Status, this.StatusDateTime);
			}

			private string FormatTime(DateTime? time)
			{
				//todo: can we centralize formatting somewhere
				return time == null ? null : time.Value.ToString("yyyy-MM-dd HH:mm");
			}

			private string FormatDate(DateTime? time)
			{
				//todo: can we centralize formatting somewhere
				return time == null ? null : time.Value.ToString("yyyy-MM-dd");
			}

			private string GetBody()
			{
				//todo: can we centralize this logic somewhere
				string content;
				if (!_part.ExtendedProperties.TryGetValue("ReportContent", out content))
					return null;

				if(string.IsNullOrEmpty(content))
					return null;

				var xmlBody = XDocument.Parse(content);
				if(xmlBody.Root == null)
					return null;

				var body = xmlBody.Root.Elements("ReportText").Select(n => n.Value).FirstOrDefault();
				return FormatHtml(body);
			}

			private string FormatHtml(string text)
			{
				if(string.IsNullOrEmpty(text))
					return string.Empty;

				return HttpUtility.HtmlEncode(text)
						.Replace("\r\n", "<br>")
						.Replace("\r", "<br>")
						.Replace("\n", "<br>");
			}
		}

		/// <summary>
		/// Report Parts facade.
		/// </summary>
		public class ReportPartsFacade
		{
			private readonly IList<ReportPartFacade> _reportParts;

			internal ReportPartsFacade(IEnumerable<ReportPart> reportParts)
			{
				_reportParts = reportParts
					.Where(p => p.Status != ReportPartStatus.X)
					.Select(p => new ReportPartFacade(p)).ToList();
			}

			public int Count
			{
				get { return _reportParts.Count; }
			}

			public ReportPartFacade this[int i]
			{
				get { return _reportParts[i]; }
			}

			public override string ToString()
			{
				return string.Format("ReportParts ({0})", this.Count);
			}
		}


		private readonly Report _report;
		private readonly ExternalPractitionerContactPoint _recipient;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="report"></param>
		public ReportPageModel(Report report)
			:this(report, GetDefaultRecipient(report))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="report"></param>
		/// <param name="recipient"></param>
		public ReportPageModel(Report report, ExternalPractitionerContactPoint recipient)
			: base(new PrintTemplateSettings().ReportTemplateUrl)
		{
			Platform.CheckForNullReference(report, "report");
			Platform.CheckForNullReference(recipient, "recipient");

			_report = report;
			_recipient = recipient;
		}

		public override Dictionary<string, object> Variables
		{
			get { return GetVariables(); }
		}

		/// <summary>
		/// Gets the variables available to the print template.
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, object> GetVariables()
		{
			var variables = new Dictionary<string, object>();

			var procedure = _report.Procedures.First();
			var order = procedure.Order;
			var patientProfile = procedure.PatientProfile;

			// letterhead
			variables["Letterhead"] = new LetterheadFacade(procedure.PerformingFacility.Code);

			// patient
			variables["Patient"] = new PatientFacade(patientProfile);

			// recipient
			variables["Recipient"] = new PractitionerFacade(_recipient.Practitioner, _recipient);

			// order
			variables["AccessionNumber"] = order.AccessionNumber;
			variables["OrderingPractitioner"] = new PractitionerFacade(order.OrderingPractitioner, null);

			// procedures
			variables["Procedures"] = new ProceduresFacade(_report.Procedures);

			// report
			variables["ReportParts"] = new ReportPartsFacade(_report.Parts);

			return variables;
		}

		private static ExternalPractitionerContactPoint GetDefaultRecipient(Report report)
		{
			var order = report.Procedures.First().Order;

			// determine default contact point
			return order.ResultRecipients.Select(rr => rr.PractitionerContactPoint).FirstOrDefault(cp => Equals(cp.Practitioner, order.OrderingPractitioner))
					?? order.OrderingPractitioner.ContactPoints.First();
		}
	}
}
