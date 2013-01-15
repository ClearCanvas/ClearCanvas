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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
	/// <summary>
	/// A thread-safe singleton class that help to test a domain entity of alerts.
	/// </summary>
	public sealed class AlertHelper
	{
		private static readonly AlertHelper _instance = new AlertHelper();

		private readonly IList<IPatientAlert> _patientAlertTests;
		private readonly IList<IPatientProfileAlert> _patientProfileAlertTests;
		private readonly IList<IOrderAlert> _orderAlertTests;
		private readonly IList<IExternalPractitionerAlert> _externalPractitionerAlertTests;

		public static AlertHelper Instance
		{
			get { return _instance; }
		}

		private AlertHelper()
		{
			_patientAlertTests = new List<IPatientAlert>();
			var patientEP = new PatientAlertExtensionPoint();
			foreach (var test in patientEP.CreateExtensions())
			{
				_patientAlertTests.Add((IPatientAlert)test);
			}

			_patientProfileAlertTests = new List<IPatientProfileAlert>();
			var ppXP = new PatientProfileAlertExtensionPoint();
			foreach (var test in ppXP.CreateExtensions())
			{
				_patientProfileAlertTests.Add((IPatientProfileAlert)test);
			}

			_orderAlertTests = new List<IOrderAlert>();
			var orderEP = new OrderAlertExtensionPoint();
			foreach (var test in orderEP.CreateExtensions())
			{
				_orderAlertTests.Add((IOrderAlert)test);
			}

			_externalPractitionerAlertTests = new List<IExternalPractitionerAlert>();
			var extPracEP = new ExternalPractitionerAlertExtensionPoint();
			foreach (var test in extPracEP.CreateExtensions())
			{
				_externalPractitionerAlertTests.Add((IExternalPractitionerAlert)test);
			}
		}

		/// <summary>
		/// A thread-safe method for testing alerts
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="context"></param>
		/// <returns>A list of alert notifications on this subject</returns>
		public IList<AlertNotification> Test(Entity subject, IPersistenceContext context)
		{
			var alertNotifications = new List<AlertNotification>();

			if (subject.Is<Patient>())
			{
				foreach (var test in _patientAlertTests)
				{
					var result = test.Test(subject.Downcast<Patient>(), context);
					if (result != null)
						alertNotifications.Add(result);
				}
			}
			else if (subject.Is<PatientProfile>())
			{
				foreach (var test in _patientProfileAlertTests)
				{
					AlertNotification result = test.Test(subject.Downcast<PatientProfile>(), context);
					if (result != null)
						alertNotifications.Add(result);
				}
			}
			else if (subject.Is<Order>())
			{
				foreach (var test in _orderAlertTests)
				{
					var result = test.Test(subject.Downcast<Order>(), context);
					if (result != null)
						alertNotifications.Add(result);
				}
			}
			else if (subject.Is<ExternalPractitioner>())
			{
				foreach (var test in _externalPractitionerAlertTests)
				{
					var result = test.Test(subject.Downcast<ExternalPractitioner>(), context);
					if (result != null)
						alertNotifications.Add(result);
				}
			}

			return alertNotifications;
		}
	}
}
