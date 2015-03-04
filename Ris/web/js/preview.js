// License

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

// End License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:

//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//       this list of conditions and the following disclaimer in the documentation
//       and/or other materials provided with the distribution.
//     * Neither the name of ClearCanvas Inc. nor the names of its contributors
//       may be used to endorse or promote products derived from this software without
//       specific prior written permission.

// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
// OF SUCH DAMAGE.

/*
 *	Provides several genaral methods for formatting data for preview pages.
 */
var Preview = function () {

	var _alerts = 
		{
			NoteAlert: { icon: "AlertNote.png", getTooltip: function(name, reasons) { return SR.Alerts.HighSeverityNotes.interp(name, reasons); } },
			LanguageAlert: { icon: "AlertNote.png", getTooltip: function(name, reasons) { return SR.Alerts.Language.interp(name, reasons); } },
			ReconciliationAlert: { icon: "AlertReconcile.png", getTooltip: function(name, reasons) { return SR.Alerts.UnreconciledRecords.interp(name,reasons); } },
			IncompleteDemographicDataAlert: { icon: "AlertIncompleteData.png", getTooltip: function(name, reasons) { return SR.Alerts.IncompleteDemographicData.interp(name, reasons); } },
			InvalidVisitAlert: { icon: "AlertVisit.png", getTooltip: function(name, reasons) { return SR.Alerts.InvalidVisit.interp(reasons); } },
			PatientAllergyAlert: { icon: "AlertAllergy2.png", getTooltip: function(name, reasons) { return SR.Alerts.Allergies.interp(name, reasons); } },

			// External Practitioner Related Alerts
			IncompleteDataAlert: { icon: "AlertIncompleteData.png", getTooltip: function(name, reasons) { return SR.Alerts.IncompleteData.interp(name, reasons); } },
			IncompleteContactPointDataAlert: { icon: "AlertNote.png", getTooltip: function(name, reasons) { return SR.Alerts.IncompleteContactPointData.interp(reasons); } },
			PossibleDuplicateAlert: { icon: "AlertReconcile.png", getTooltip: function(name, reasons) { return SR.Alerts.PossibleDuplicate.interp(name); } }
		};

	var _getAlertIcon = function(alertItem)
	{
		var alert = _alerts[alertItem.AlertId];
		return alert ? alert.icon : "AlertGeneral.png";
	};
	
	var _getAlertTooltip = function(alertItem, name)
	{
		var escapedReasons = [];
		alertItem.Reasons.each(function(reason) 
			{ 
				// escape single and double quote
				var escapedReason = reason.replace(/"/gi, '&quot;').replace(/'/gi, '\''); 
				escapedReasons.add(escapedReason);
			});
	
		var reasons = String.combine(escapedReasons, "; ");

		var alert = _alerts[alertItem.AlertId];
		return alert ? alert.getTooltip(name, reasons) : reasons;
	};
	
	return {
		createAlerts: function(containingElement, alertItems, name)
		{
			if (alertItems)
			{
				var alertHtml = "";
				alertItems.each(function(item) { alertHtml += Preview.getAlertHtml(item, name); });

				containingElement.innerHTML = alertHtml;
			}
		},

		getAlertHtml: function(alertItem, name)
		{
			var toolTip = '"' + _getAlertTooltip(alertItem, name) + '"';
			return "<img class='alert' src='" + imagePath + "/" + _getAlertIcon(alertItem) + "' alt=" + toolTip + "/>";
		},

		getPatientAge: function(dateOfBirth, deathIndicator, timeOfDeath)
		{
			if (dateOfBirth == null)
				return SR.PatientAge.DateOfBirthUnknown;
				
			var endDate = (deathIndicator == true ? timeOfDeath : new Date());

			//Define a variable to hold the anniversary of theBirthdate in the endDate year
			var theBirthdateThisYear = new Date(endDate);
			theBirthdateThisYear.setYMD(endDate.getFullYear(), dateOfBirth.getMonth(), dateOfBirth.getDate());

			// calculate the age at endDate
			var age = endDate.getFullYear() - dateOfBirth.getFullYear();
			if (endDate < theBirthdateThisYear) 
				age--;

			var ageString = age;
			if (age >= 2)
				ageString = age;
			else
			{
				// display number of month if less than 2 years old
				// of number of days if less than or equal to 31 days
				var days = Math.floor((endDate - dateOfBirth)/(1000*60*60*24));
				if (days < 0)
					return "undefined";
				else if (days < 1)
					ageString = "0";
				else if (days <= 31)
					ageString = SR.PatientAge.Days.interp(days);
				else
				{
					// Calculate the number of month
					var yearDiff = endDate.getFullYear() - dateOfBirth.getFullYear();
					var month = endDate.getMonth() - dateOfBirth.getMonth();
					if (endDate.getDate() < dateOfBirth.getDate())
						month -= 1;

					// no month should be negative
					month += (month < 0 ? 12 : 0);
					
					// add 12 month if already 1 year old
					month += (age == 1 ? 12 : 0);

					// special case for exactly 12 month 
					month += (month == 0 && yearDiff == 0 ? 12 : 0);

					ageString = SR.PatientAge.Months.interp(month);
				}
			}

			if (deathIndicator == true)
				ageString += " " + SR.PatientAge.Deceased;
				
			return ageString;
		},

		getPatientAgeInYears: function(dateOfBirth, deathIndicator, timeOfDeath)
		{
			if (dateOfBirth == null)
				return 0;
				
			var endDate = (deathIndicator == true ? timeOfDeath : new Date());

			//Define a variable to hold the anniversary of theBirthdate in the endDate year
			var theBirthdateThisYear = new Date(endDate);
			theBirthdateThisYear.setYMD(endDate.getFullYear(), dateOfBirth.getMonth(), dateOfBirth.getDate());

			// calculate the age at endDate
			var age = endDate.getFullYear() - dateOfBirth.getFullYear();
			if (endDate < theBirthdateThisYear) 
				age--;

			return age;
		},

		formatPerformingFacilityList: function(procedures)
		{
			var facilities = [];
			for(var i = 0; i < procedures.length; i++)
			{
				if (facilities.indexOf(procedures[i].PerformingFacility.Name) < 0)
					facilities.add(procedures[i].PerformingFacility.Name);
			}
			
			return String.combine(facilities, "<br>");
		},
		
		filterProcedureByModality: function(procedures, modalityIdFilter)
		{
			var isStepInModality = function (step)
			{
				return modalityIdFilter.find(
					function(id) 
					{
						return step.Modality.Id == id; 
					}) != null;
			}
			
			var isProcedureInModality = function (p)
			{
				if (!p.ProcedureSteps)
					return false;
					
				var mps = p.ProcedureSteps.select(function(step) { return step.StepClassName == "ModalityProcedureStep"; });
				return mps.find(isStepInModality) != null;
			}

			return procedures.select(isProcedureInModality);
		},
		
		formatVisitCurrentLocation: function(visit)
		{
			if (!visit || !visit.CurrentLocation)
				return null;
			
			if (visit.CurrentRoom || visit.CurrentBed)
				return visit.CurrentLocation.Name + ", " + (visit.CurrentRoom || SR.Visits.RoomNotSpecified) + (visit.CurrentBed ? "/" + visit.CurrentBed : "");
			else
				return visit.CurrentLocation.Name;
		}
	};
}();

/*
 *	Provides helper functions used by ProceduresTable, ProtocolProceduresTable and ReportingProceduresTable
 */
Preview.ProceduresTableHelper = function () {
	return {
		formatProcedureSchedule: function(scheduledStartTime, schedulingRequestTime, showDescriptiveTime, schedulingCode)
		{
			var formattedText;
			if (scheduledStartTime)
				formattedText = showDescriptiveTime ? Ris.formatDescriptiveDateTime(scheduledStartTime) : Ris.formatDateTime(scheduledStartTime);
			else if (schedulingRequestTime)
				formattedText = SR.Procedures.RequestedFor.interp(showDescriptiveTime ? Ris.formatDescriptiveDateTime(schedulingRequestTime) : Ris.formatDateTime(schedulingRequestTime));
			else
				formattedText = SR.Procedures.NotScheduled;

			if (schedulingCode)
				formattedText += " (" + schedulingCode.Value + ")";

			return formattedText;
		},

		formatProcedureSchedulingCode: function(schedulingCode, showValue)
		{
			return schedulingCode == null ? ""
				: (showValue ? schedulingCode.Code : schedulingCode.Value);
		},

		formatProcedureStatus: function(status, scheduledStartTime, startTime, checkInTime, checkOutTime)
		{
			if (status.Code == 'CA')
				return status.Value;  // show status instead of "Unscheduled" for unscheduled procedure that are cancelled
				
			if (!scheduledStartTime && !startTime)
				return SR.Procedures.StatusUnscheduled;

			if (status.Code == 'SC' && checkInTime)
				return SR.Procedures.StatusCheckedIn;
			
			if (status.Code == 'IP' && checkOutTime)
				return SR.Procedures.StatusPerformed;
				
			return status.Value; 
		},

		formatProcedureStartEndTime: function(startTime, checkOutTime)
		{
			if (!startTime)
				return SR.Procedures.NotStarted;

			if (checkOutTime)
				return SR.Procedures.EndedAt.interp(Ris.formatDateTime(checkOutTime));

			return SR.Procedures.StartedAt.interp(Ris.formatDateTime(startTime));
		},

		formatProcedurePerformingStaff: function(procedure)
		{
			var firstMps = procedure.ProcedureSteps && procedure.ProcedureSteps.select(function(step) { return step.StepClassName == "ModalityProcedureStep"; }).firstElement();

			if (!firstMps)
				return "";
			else if (firstMps.Performer)
				return Ris.formatPersonName(firstMps.Performer.Name);
			else if (firstMps.ScheduledPerformer)
				return Ris.formatPersonName(firstMps.ScheduledPerformer.Name);
			else
				return "";
		},
		
		addHeading: function(parentElement, text, className)
		{
			var heading = document.createElement("P");
			heading.className = className || 'sectionheading';
			heading.innerText = text;
			parentElement.appendChild(heading);
		},
			
		addTable: function(parentElement, id, noTableHeading)
		{

			noTableHeading = !!noTableHeading;

			var htmlTableContainer = document.createElement("DIV");
			htmlTableContainer.className = "ProceduresTableContainer";
			var htmlTable = document.createElement("TABLE");
			htmlTableContainer.appendChild(htmlTable);
			parentElement.appendChild(htmlTableContainer);
			var body = document.createElement("TBODY");
			htmlTable.appendChild(body);

			if(noTableHeading)
			{
				var headingRow = document.createElement("TR");
				body.appendChild(headingRow);
			}

			return htmlTable;
		},
		
		addTableWithClass: function(parentElement, id, noTableHeading, cssClass)
		{

			noTableHeading = !!noTableHeading;

			var htmlTableContainer = document.createElement("DIV");
			htmlTableContainer.className = cssClass;
			var htmlTable = document.createElement("TABLE");
			htmlTableContainer.appendChild(htmlTable);
			parentElement.appendChild(htmlTableContainer);
			var body = document.createElement("TBODY");
			htmlTable.appendChild(body);

			if(noTableHeading)
			{
				var headingRow = document.createElement("TR");
				body.appendChild(headingRow);
			}

			return htmlTable;
		}		
	};
}();

/*
 *	Create a table of imaging services with the following columns:
 *		- Procedure
 *		- Schedule
 *		- Status
 *		- Performing Facility
 *		- Ordering Physician
 *
 *	The imaging service of the selected procedure is excluded from the list
 *
 *	Exposes two methods
 *		- createActive:
 *		- createPast:
 */
Preview.ImagingServiceTable = function () {

	var _isProcedureStatusActive = function(procedureStatus)
	{
		return procedureStatus.Code == "SC" || 
				procedureStatus.Code == "IP";
	};

	var _orderRequestScheduledDateComparison = function(data1, data2)
	{
		return Date.compareMoreRecent(data1.SchedulingRequestTime, data2.SchedulingRequestTime);
	};

	var _procedureScheduledDateComparison = function(data1, data2)
	{
		return Date.compareMoreRecent(data1.ProcedureScheduledStartTime, data2.ProcedureScheduledStartTime);
	};

	var _getActiveProcedures = function(patientOrderData)
	{
		var today = Date.today();

		var presentScheduledProcedures = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime &&
						Date.compare(item.ProcedureScheduledStartTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus);
			}).sort(_procedureScheduledDateComparison);

		var presentNotScheduledProceduress = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime == null &&
						item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus);
			}).sort(_orderRequestScheduledDateComparison);
			
		return presentScheduledProcedures.concat(presentNotScheduledProceduress);
	};

	var _getNonActiveProcedures = function(patientOrderData)
	{

		var today = Date.today();

		// List only the non-Active present procedures
		var presentScheduledProcedures = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime && Date.compare(item.ProcedureScheduledStartTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus) == false;
			}).sort(_procedureScheduledDateComparison);

		// List only the non-Active present not-scheduled procedures
		var presentNotScheduledProceduress = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime == null &&
						item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) >= 0 &&
						_isProcedureStatusActive(item.ProcedureStatus) == false;
			}).sort(_orderRequestScheduledDateComparison);

		var pastScheduledProcedures = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime && Date.compare(item.ProcedureScheduledStartTime, today) < 0;
			}).sort(_procedureScheduledDateComparison);

		var pastNotScheduledProceduress = patientOrderData.select(
			function(item) 
			{ 
				return item.ProcedureScheduledStartTime == null
				&& item.SchedulingRequestTime && Date.compare(item.SchedulingRequestTime, today) < 0;
			}).sort(_orderRequestScheduledDateComparison);

		return presentScheduledProcedures.concat(
				presentNotScheduledProceduress.concat(
				pastScheduledProcedures.concat(pastNotScheduledProceduress)));
	};

	var _formatPerformingFacility = function(item, memberName)
	{
		return item.ProcedurePerformingFacility ? item.ProcedurePerformingFacility[memberName] : "";
	};
	
	var _createHelper = function(parentElement, ordersList, sectionHeading)
	{
		if(ordersList.length == 0)
		{
			parentElement.style.display = 'none';
			return;
		}
		else
		{
			parentElement.style.display = 'block';
		}

		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
			 [
				{   label: SR.ImagingServices.ColumnHeadings.Procedure,
					cellType: "text",
					getValue: function(item) { return Ris.formatOrderListItemProcedureName(item); }
				},
				{   label: SR.ImagingServices.ColumnHeadings.ScheduledFor,
					cellType: "text",
					getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ProcedureScheduledStartTime, item.SchedulingRequestTime, true, item.ProcedureSchedulingCode); },
					getTooltip: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ProcedureScheduledStartTime, item.SchedulingRequestTime, false, item.ProcedureSchedulingCode); }
				},
				{   label: SR.ImagingServices.ColumnHeadings.Status,
					cellType: "text",
					getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStatus(item.ProcedureStatus, item.ProcedureScheduledStartTime, item.ProcedureStartTime, item.ProcedureCheckInTime, item.ProcedureCheckOutTime); }
				},
				{   label: SR.ImagingServices.ColumnHeadings.PerformingFacility,
					cellType: "text",
					getValue: function(item) { return _formatPerformingFacility(item, 'Name'); },
					getTooltip: function(item) { return _formatPerformingFacility(item, 'Description'); }
				},
				{   label: SR.ImagingServices.ColumnHeadings.OrderingPhysician,
					cellType: "link",
					getValue: function(item)  { return Ris.formatPersonName(item.OrderingPractitioner.Name); },
					clickLink: function(item) { Ris.openPractitionerDetails(item.OrderingPractitioner); }
				}
			 ]);

		htmlTable.rowCycleClassNames = ["row0", "row1"];
		htmlTable.bindItems(ordersList);
	};

	return {
		createActive: function(parentElement, ordersList)
		{
			var activeProcedures = _getActiveProcedures(ordersList);
					
			_createHelper(parentElement, activeProcedures, SR.ImagingServices.ActiveImagingServices);
			Preview.SectionContainer.create(parentElement, SR.ImagingServices.ActiveImagingServices, { collapsible: true, initiallyCollapsed: true });						
		},
		
		createPast: function(parentElement, ordersList, options)
		{
			var pastProcedures = _getNonActiveProcedures(ordersList);
				
			_createHelper(parentElement, pastProcedures, SR.ImagingServices.PastImagingServices);
			Preview.SectionContainer.create(parentElement, SR.ImagingServices.PastImagingServices, { collapsible: true, initiallyCollapsed: true });						
		}
	};
}();

/*
 *	Create a table of procedures with the following columns:
 *		- Procedure
 *		- Status
 *		- Schedule
 *		- Start/End Time
 *		- Performing Staff
 *
 *	Exposes one method: create(...)
 */
Preview.ProceduresTable = function () {
	return {
		create: function(parentElement, procedures, options)
		{
			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			if(options && options.AddSectionHeading)
			{
				Preview.ProceduresTableHelper.addHeading(parentElement, SR.Procedures.Procedures);
			}

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: SR.Procedures.ColumnHeadings.Procedure,
						cellType: "text",
						getValue: function(item) { return Ris.formatProcedureName(item); }
					},
					{   label: SR.Procedures.ColumnHeadings.Status,
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStatus(item.Status, item.ScheduledStartTime, item.StartTime, item.CheckInTime, item.CheckOutTime); }
					},
					{   label: SR.Procedures.ColumnHeadings.ScheduledFor,
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ScheduledStartTime, null, true, item.SchedulingCode); },
						getTooltip: function(item) { return Preview.ProceduresTableHelper.formatProcedureSchedule(item.ScheduledStartTime, null, false, item.SchedulingCode); }
					},
					{   label: SR.Procedures.ColumnHeadings.StartEndTime,
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedureStartEndTime(item.StartTime, item.CheckOutTime); }
					},
					{   label: SR.Procedures.ColumnHeadings.PerformingStaff,
						cellType: "text",
						getValue: function(item) { return Preview.ProceduresTableHelper.formatProcedurePerformingStaff(item); }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procedures);
			
			Preview.SectionContainer.create(parentElement, SR.Procedures.Procedures);
		}
	};
}();

/*
 *	Create a table of procedures with protocol-specific details contained in the following columns:
 *		- Procedure
 *		- Protocol
 *		- Code
 *		- Author
 *		- Urgency
 *
 *	Exposes one method: create(...)
 */
Preview.ProtocolProceduresTable = function () {
			
	var _formatProtocolStatus = function(protocol)
	{
		if(!protocol)
			return SR.Protocols.NotProtocolled;

		if(protocol.Status.Code == "RJ")
			return protocol.Status.Value + " - "+ protocol.RejectReason.Value;

		return protocol.Status.Value; 
	}
	
	var _formatProtocolCode = function(protocol)
	{
		if (!protocol)
			return "";
			
		return String.combine(protocol.Codes.map(function(code) { return code.Name; }), "<br>");
	}
	
	var _formatProtocolAuthor = function(protocol)
	{
		if (!protocol || !protocol.Author)
			return "";
		
		return Ris.formatPersonName(protocol.Author.Name);
	}
	
	var _formatProtocolUrgency = function(protocol)
	{
		if (!protocol || !protocol.Urgency)
			return "";
			
		return protocol.Urgency.Value;
	}

	var _filterProcedureByProtocols = function(procedures)
	{
		var hasProtocols = function (p)
		{
			if (!p.ProcedureSteps)
				return false;
				
			var mps = p.ProcedureSteps.select(function(step) { return step.StepClassName == "ProtocolAssignmentStep"; });
			return mps.length > 0;
		}

		return procedures.select(hasProtocols);
	}
	
	return {
	
		// TODO: WTIS -> urgency and override in UHN-specific script.
		
		create: function(parentElement, procedures, notes)
		{
			procedures = procedures || [];
			notes = notes || [];
		
			procedures = _filterProcedureByProtocols(procedures);
			notes = notes.select(function(note) { return note.Category == "Protocol"; });						

			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			// group procedures according to their protocol ref(indicates linked protocolling),
			// or by procedureRef, if they don't have a protocol (effectively not grouping them)
			var procGroupings = procedures.groupBy(
				function(proc)
				{
					return proc.Protocol ? proc.Protocol.ProtocolRef : proc.ProcedureRef;
				});

			// transform the groupings into individual items that can be displayed in the table	
			procGroupings = procGroupings.map(
				function(grouping)
				{
					// reduce each procedure Grouping to a single item, where all procedure names are concatenated (all other fields should be identical)
					return grouping.reduce({},
						function(memo, item) 
						{
							return {
								Procedure : memo.Procedure ? [memo.Procedure, Ris.formatProcedureName(item)].join(', ') : Ris.formatProcedureName(item),
								// if the procedure was cancelled, leave the protocol status blank
								Status : memo.Status || ((item.Status.Code == "CA") ? "" : _formatProtocolStatus(item.Protocol)),
								Protocol: memo.Protocol || _formatProtocolCode(item.Protocol),
								Author : memo.Author || _formatProtocolAuthor(item.Protocol),
								Urgency: memo.Urgency || _formatProtocolUrgency(item.Protocol)
							};
						});
				});

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: SR.Protocols.ColumnHeadings.Procedure,
						cellType: "text",
						getValue: function(item) { return item.Procedure; }
					},
					{   label: SR.Protocols.ColumnHeadings.Status,
						cellType: "text",
						getValue: function(item) { return item.Status; }
					},
					{   label: SR.Protocols.ColumnHeadings.Protocol,
						cellType: "html",
						getValue: function(item) { return item.Protocol; }
					},
					{   label: SR.Protocols.ColumnHeadings.Author,
						cellType: "text",
						getValue: function(item) { return item.Author; }
					},
					{   label: SR.Protocols.ColumnHeadings.Urgency,
						cellType: "text",
						getValue: function(item) { return item.WTIS; }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procGroupings);
									
			if(notes.length > 0) {
				Preview.OrderNotesTable.createAsSubSection(parentElement, notes, "Notes");
			}

			Preview.SectionContainer.create(parentElement, "Protocols");
		}
	};
}();

/*
 *	Create a table of procedures with reporting-specific details contained in the following columns:
 *		- Procedure
 *		- Status
 *		- Start/End Time
 *		- Performing Staff
 *		- Owner
 *
 *	Exposes one method: create(...)
 */
Preview.ReportingProceduresTable = function () {
	var _getActiveReportingStep = function(procedure)
	{
		var reportingStepNames = ["InterpretationStep", "TranscriptionStep", "TranscriptionReviewStep", "VerificationStep", "PublicationStep"];
		var activeStatusCode = ["SC", "IP"];
		var isActiveReportingStep = function(step) { return reportingStepNames.indexOf(step.StepClassName) >= 0 && activeStatusCode.indexOf(step.State.Code) >= 0; };
		return procedure.ProcedureSteps ? procedure.ProcedureSteps.select(isActiveReportingStep).firstElement() : null;
	}
	
	var _getLastCompletedPublicationStep = function(procedure)
	{
		var isCompletedPublicationStep = function(step) { return step.StepClassName == "PublicationStep" && step.State.Code == "CM"; };
		var compreStepEndTime = function(step1, step2) { return Date.compare(step1.EndTime, step2.EndTime); };
		return procedure.ProcedureSteps ? procedure.ProcedureSteps.select(isCompletedPublicationStep).sort(compreStepEndTime).reverse().firstElement() : null;
	}
		 
	var _formatProcedureReportingStatus = function(procedure)
	{
		// bug #3470: there is no point drilling down into the "reporting" status unless the procedure is actually In Progress or Completed
		// if not, we just return the Procedure status
		if(["IP", "CM"].indexOf(procedure.Status.Code) == -1)
			return Preview.ProceduresTableHelper.formatProcedureStatus(procedure.Status, procedure.ScheduledStartTime, procedure.StartTime, procedure.CheckInTime, procedure.CheckOutTime);
	
		var activeReportingStep = _getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = _getLastCompletedPublicationStep(procedure);

		var lastStep = activeReportingStep ? activeReportingStep : lastCompletedPublicationStep;
		
		// bug #3470: there may not be any reporting steps yet, or the procedure may have been linked to another - there is no way to tell
		// for now, just return "". The linked procedure problem should be fixed server-side in a future version
		if(!lastStep) return "";
		
		var isAddendum = activeReportingStep && lastCompletedPublicationStep;

		var stepName = lastStep.ProcedureStepName;
		var addendumPrefix = isAddendum ? SR.Procedures.AddendumPrefix : "";

		var formattedStatus;
		switch(lastStep.State.Code)
		{
			case "SC":
				formattedStatus = SR.Procedures.StatusPending.interp(stepName);
				break;
			case "IP":
				formattedStatus = SR.Procedures.StatusInProgress.interp(stepName); 
				break;
			case "SU":
				formattedStatus = SR.Procedures.StatusSuspended.interp(stepName);
				break;
			case "CM":
				formattedStatus = SR.Procedures.StatusCompleted.interp(stepName);
				// Exceptions to formatting
				if (stepName == "Verification")
					formattedStatus = SR.Procedures.StatusVerified.interp(stepName);
				else if (stepName == "Publication")
					formattedStatus = SR.Procedures.StatusPublished.interp(stepName);
				break;
				
			case "DC":
				formattedStatus = SR.Procedures.StatusCancelled.interp(stepName);
				break;
			default: break;
		}

		return addendumPrefix + formattedStatus;
	}
	
	var _formatProcedureReportingOwner = function(procedure)
	{
		var activeReportingStep = _getActiveReportingStep(procedure);
		var lastCompletedPublicationStep = _getLastCompletedPublicationStep(procedure);

		var lastStep = activeReportingStep ? activeReportingStep : lastCompletedPublicationStep;

		if (!lastStep)
			return "";

		if (lastStep.Performer)
			return Ris.formatPersonName(lastStep.Performer.Name)
		
		if (lastStep.ScheduledPerformer)
			return Ris.formatPersonName(lastStep.ScheduledPerformer.Name);
			
		return "";
	}
	
	var _getImageAvailabilityIcon = function(imageAvailability)
	{
		switch (imageAvailability.Code)
		{
			case "X":
				statusIcon = "question.png";
			case "N":
				return "question.png";
			case "Z":
				return "shield_red.png";
			case "P":
				return "shield_yellow.png";
			case "C":
				return "shield_green.png";
			default:
				return "question.png";
		}
		
	}
		 
	return {
		create: function(parentElement, procedures)
		{
			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			// group procedures according to their active reporting step (indicates linked reporting),
			// or by procedureRef, if they don't have an active reporting step (effectively not grouping them)
			var procGroupings = procedures.groupBy(
				function(proc)
				{
					var ps = _getActiveReportingStep(proc);
					return ps ? ps.ProcedureStepRef : proc.ProcedureRef;
				});

			// transform the groupings into individual items that can be displayed in the table	
			procGroupings = procGroupings.map(
				function(grouping)
				{
					// reduce each procedure Grouping to a single item, where all procedure names are concatenated (all other fields should be identical)
					return grouping.reduce({},
						function(memo, item) 
						{
							return {
								ProcedureName : memo.ProcedureName ? [memo.ProcedureName, Ris.formatProcedureName(item)].join(', ') : Ris.formatProcedureName(item),
								Status : memo.Status || _formatProcedureReportingStatus(item),
								Schedule: memo.Schedule || Preview.ProceduresTableHelper.formatProcedureSchedule(item.ScheduledStartTime, null, true, item.SchedulingCode),
								StartEndTime: memo.StartEndTime || Preview.ProceduresTableHelper.formatProcedureStartEndTime(item.StartTime, item.CheckOutTime),
								PerformingStaff : memo.PerformingStaff || Preview.ProceduresTableHelper.formatProcedurePerformingStaff(item),
								Owner: memo.Owner || _formatProcedureReportingOwner(item)
							};
						});
				});

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: SR.Procedures.ColumnHeadings.Procedure,
						cellType: "text",
						getValue: function(item) { return item.ProcedureName; }
					},
					{   label: SR.Procedures.ColumnHeadings.Status,
						cellType: "text",
						getValue: function(item) { return item.Status; }
					},
					{   label: SR.Procedures.ColumnHeadings.ScheduledFor,
						cellType: "text",
						getValue: function(item) { return item.Schedule; }
					},
					{   label: SR.Procedures.ColumnHeadings.StartEndTime,
						cellType: "text",
						getValue: function(item) { return item.StartEndTime; }
					},
					{   label: SR.Procedures.ColumnHeadings.PerformingStaff,
						cellType: "text",
						getValue: function(item) { return item.PerformingStaff; }
					},
					{   label: SR.Procedures.ColumnHeadings.Owner,
						cellType: "text",
						getValue: function(item) { return item.Owner; }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(procGroupings);
			
			Preview.SectionContainer.create(parentElement, SR.Procedures.Procedures);
		}
	};
}();

/*
 *	Create a table of reports with the following columns:
 *		- Procedure
 *		- Report Status
 *
 *	Exposes one method: create(...)
 */
Preview.ReportListTable = function () {
	var _onReportListSelectionChanged = function(reportListItem)
	{
		if(!Ris) return;
	
		var request = 
		{
			GetReportDetailRequest: { ReportRef: reportListItem.ReportRef }
		};

		var service = Ris.getPatientDataService();
		var data = service.getData(request);

		if (data == null || data.length == 0)
		{
			Field.show($("reportContent"), false);
			return;
		}

		var reportDetail = data.GetReportDetailResponse ? data.GetReportDetailResponse.Report : null;
		Preview.ReportPreview.create($("reportContent"), reportDetail, { hideSectionContainer: true });
	};	
		

	return {
		create: function(parentElement, reportList)
		{
			if(reportList.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}
			
			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement, null);

			var reportContent = document.createElement("DIV");
			reportContent.id = "reportContent";
			parentElement.appendChild(reportContent);

			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, autoSelectFirstElement: true, addColumnHeadings:true },
			[
				{   label: SR.Reports.ColumnHeadings.Procedure,
					cellType: "text",
					getValue: function(item) { return Ris.formatOrderListItemProcedureName(item); }
				},
				{   label: SR.Reports.ColumnHeadings.Status,
					cellType: "text",
					getValue: function(item) { return item.ReportStatus.Value; }
				}
			]);

			htmlTable.onRowClick = function(sender, args)
			{
				_onReportListSelectionChanged(args.item);
			};

			htmlTable.mouseOverClassName = "mouseover";
			htmlTable.highlightClassName = "highlight";
			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(reportList);
			
			Preview.SectionContainer.create(parentElement, SR.Reports.Reports);
		}
	};
}();

/*
 *	Create one or more tables of notes with a preformatted HTML column:
 *	The notes can be split into tables for specific note categories if desired.
 * 	
 *	Exposes one method: create(parentElement, notes, subsections, hideHeading, canAcknowledge)
 * 		parentElement - parent node for table(s)
 *		notes - the list of note objects
 *		subsections - optional - a list of objects of form { category: "SomeNoteCategory", subsectionHeading: "SomeHeading" }.  
 *			If no subsections are specified, all notes are shown in a single table.
 *		hideHeading - optional - hide heading and category headings
 *		checkBoxesProperties - optional - customize the checkbox behaviour, including callback when a checkbox is toggled and override when a checkbox is visible
 *  Also exposes defaultSubsections array which can be used as the subsections parameter in create(...)
 */
Preview.OrderNotesTable = function () {
	var _createNotesTable = function(parentElement, notes, title)
	{
		if (notes.length == 0)
			return;
			
		if(title)
		{
			Preview.ProceduresTableHelper.addHeading(parentElement, title, 'subsectionheading');
		}			

		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement, null, true);
		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: false },
		[
			{   
				cellType: "html",
				getValue: function(item) 
				{
					var divTag = document.createElement("div");
					Preview.OrderNoteSection.create(divTag, item);
					
					// retrieve the innerHTML of the template table
					return Field.getValue(divTag);
				}
			}
		]);

		// highlight the row that need to be acknowledged
		htmlTable.renderRow = function(sender, args)
		{
			if(args.item.CanAcknowledge)
			{
				args.htmlRow.className = "attention" + (args.rowIndex % 2);
			}
		};

//		htmlTable.rowCycleClassNames = ["row1", "row0"];

		htmlTable.bindItems(notes);
	};

	return {
		create: function(parentElement, notes, heading, collapsedByDefault)
		{
			if(notes.length == 0)
				return;

			_createNotesTable(parentElement, notes);

			Preview.SectionContainer.create(parentElement, heading, { collapsible: true, initiallyCollapsed: collapsedByDefault });
		},
		
		createAsSubSection: function(parentElement, notes, title) {
			if(notes.length == 0)
				return;

			_createNotesTable(parentElement, notes, title);			
		}
	};
}();

/*
 *	Create a list of conversation items. Each item contains the author, post date and a message.
 * 	
 *	Exposes one method: create(parentElement, notes, subsections, hideHeading, canAcknowledge)
 * 		parentElement - parent node for table(s)
 *		notes - the list of note objects
 *		subsections - optional - a list of objects of form { category: "SomeNoteCategory", subsectionHeading: "SomeHeading" }.  
 *			If no subsections are specified, all notes are shown in a single table.
 *		hideHeading - optional - hide heading and category headings
 *		checkBoxesProperties - optional - customize the checkbox behaviour, including callback when a checkbox is toggled and override when a checkbox is visible
 *  Also exposes defaultSubsections array which can be used as the subsections parameter in create(...)
 */
Preview.ConversationHistory = function () {
	var _createSubsection = function(parentElement, notes, categoryFilter, subsectionHeading, hideHeading, checkBoxesProperties)
	{
		var filteredNotes = categoryFilter ? notes.select(function(note) { return note.Category == categoryFilter; }) : notes;

		if (filteredNotes.length == 0)
			return;

		if(subsectionHeading && !hideHeading)
		{
			Preview.ProceduresTableHelper.addHeading(parentElement, subsectionHeading, 'subsectionheading');
		}

		var canAcknowledge = checkBoxesProperties && checkBoxesProperties.onItemChecked && filteredNotes.find(function(note) { return note.CanAcknowledge; });

		var htmlTable = Preview.ProceduresTableHelper.addTableWithClass(parentElement, "NoteEntryTable", noColumnHeadings=true, "ConversationHistoryTable");
		htmlTable = Table.createTable(htmlTable, { checkBoxes: false, checkBoxesProperties: checkBoxesProperties, editInPlace: false, flow: false, addColumnHeadings: false },
		[
			{   label: SR.OrderNotes.ColumnHeadings.OrderNote,
				cellType: "html",
				getValue: function(item) 
				{
					var divTag = document.createElement("div");
					Preview.ConversationNote.create(divTag, item);
					
					// retrieve the innerHTML of the template table
					return Field.getValue(divTag);
				}
			}
		]);

		htmlTable.bindItems(filteredNotes);
		
		// after binding the items, need to hook up the check boxes manually
		notes.each(
			function(note)
			{			
				var checkBox = $(note.OrderNoteRef);	// the OrderNoteRef was used as the checkbox "id"
				// checkBox may be null if that note was not ack'able
				if(checkBox)
				{
					checkBox.onclick = function() { checkBoxesProperties.onItemChecked(note, checkBox.checked); };
				}
			});
	};

	return {
		create: function(parentElement, notes, subsections, hideHeading, checkBoxesProperties)
		{		
			if(notes.length == 0)
				return;

			if(subsections)
			{
				for(var i = 0; i < subsections.length; i++)
				{
					if(subsections[i])
					{
						_createSubsection(parentElement, notes, subsections[i].category, subsections[i].subsectionHeading, hideHeading, checkBoxesProperties);
					}
				}
			}
			else
			{
				_createSubsection(parentElement, notes);
			}

			if(!hideHeading)
				Preview.SectionContainer.create(parentElement, "Order Notes");
		},
		
		defaultSubsections:
		[
			{category:"General", subsectionHeading:"General"}, 
			{category:"Protocol", subsectionHeading:"Protocol"}, 
			{category:"PrelimDiagnosis", subsectionHeading:"Preliminary Diagnosis"}
		]
	};
}();

/*
 *	Create a report preview.
 * 	
 *	Exposes one method: create(...)
 */
Preview.ReportPreview = function () {
	var _parseReportObject = function(reportJsml)
	{
		try
		{
			return JSML.parse(reportJsml);
		}
		catch(e)
		{
			return null;
		}
	}

	var _parseReportContent = function(reportJsml)
	{
		var reportObject = _parseReportObject(reportJsml);
		if (!reportObject)
		{
			// the Content was not JSML, but just plain text
			return reportJsml || "";
		}

		var reportText = reportObject.ReportText;
		if (!reportText)
			return "";
			
		return String.replaceLineBreak(reportText);
	}

	var _formatReportStatus = function(report, isAddendum)
	{
		var timePropertyMap = {X: 'CancelledTime', D: 'CreationTime', P: 'PreliminaryTime', F: 'CompletedTime'};
		var timeText = Ris.formatDateTime(report[timePropertyMap[report.Status.Code]]);
		var warningText = " " + (isAddendum ? SR.ReportPreview.AddendumNotFinalized : SR.ReportPreview.ReportNotFinalized);

		var statusText = report.Status.Value + " - " + timeText;

		if (['D', 'P'].indexOf(report.Status.Code) > -1)
			statusText = "<font color='red'>" + statusText + warningText + "</font>";

		return " (" + statusText + ")";
	}

	var _formatReportPerformer = function(reportPart)
	{
		if (reportPart == null)
			return "";

		var formattedReport = "<br>";
		
		if (reportPart.InterpretedBy)
			formattedReport += "<br> " + SR.ReportPreview.InterpretedBy.interp(Ris.formatPersonName(reportPart.InterpretedBy.Name));

		if (reportPart.TranscribedBy)
			formattedReport += "<br> " + SR.ReportPreview.TranscribedBy.interp(Ris.formatPersonName(reportPart.TranscribedBy.Name));

		if (reportPart.VerifiedBy)
			formattedReport += "<br> " + SR.ReportPreview.VerifiedBy.interp(Ris.formatPersonName(reportPart.VerifiedBy.Name));

		if (reportPart.Supervisor)
			formattedReport += "<br> " + SR.ReportPreview.Supervisor.interp(Ris.formatPersonName(reportPart.Supervisor.Name));

		return formattedReport;
	}

	return {
		create: function(element, report, options)
		{
			if (element == null || report == null || report.Parts == null || report.Parts.length == 0)
			{
				element.style.display = 'none';
				return;
			}

			// force options to boolean values
			options = options || {};
			options.hideSectionContainer = !!options.hideSectionContainer;

			var formattedReport = "<br>";

			formattedReport += '<div id="transcriptionErrorsDiv" style="{display:none; border: 1px solid black; text-align: center; color: red; margin-bottom:1em; font-weight:bold;}">Transcription has errors.</div>'

			if (report.Parts.length > 1)
			{
				for (var i = report.Parts.length-1; i > 0; i--)
				{
					var addendumPart = report.Parts[i];
					var addendumContent = addendumPart && addendumPart.ExtendedProperties && addendumPart.ExtendedProperties.ReportContent ? addendumPart.ExtendedProperties.ReportContent : "";
					var parsedReportContent = _parseReportContent(addendumContent);
					if (parsedReportContent)
					{
						formattedReport += "<div class='reportPreview'>";
						formattedReport += "<b>" + SR.ReportPreview.Addendum + " " + _formatReportStatus(addendumPart, true) + "</b><br><br>";
						formattedReport += parsedReportContent;
						formattedReport += _formatReportPerformer(addendumPart);
						formattedReport += "<br><br>";
						formattedReport += "</div>";
					}
				}
			}

			var part0 = report.Parts[0];
			var reportContent = part0 && part0.ExtendedProperties && part0.ExtendedProperties.ReportContent ? part0.ExtendedProperties.ReportContent : "";
			formattedReport += "<div class='reportPreview'>";
			formattedReport += "<b>" + SR.ReportPreview.Report + " " + _formatReportStatus(part0) + "</b>";
			formattedReport += "<br><br>";
			formattedReport += _parseReportContent(reportContent);
			formattedReport += _formatReportPerformer(part0);
			formattedReport += "<br><br>";
			formattedReport += "</div>";

			element.innerHTML = formattedReport;
			
			if (!options.hideSectionContainer)
				Preview.SectionContainer.create(element, SR.ReportPreview.Report);
		},
		
		toggleTranscriptionErrors: function(hasErrors)
		{
			if($("transcriptionErrorsDiv"))
			{
				Field.show($("transcriptionErrorsDiv"), hasErrors);
			}
		}
	};
}();

/*
 *	Create a summary of a single imaging service.
 */
Preview.ImagingServiceSection = function () {
	var _html = 
		'<div class="SectionTableContainer">' +
		'<table width="100%" border="0" cellspacing="5">'+
		'	<tr>'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.ImagingService+'</td>'+
		'		<td colspan="3"><div id="DiagnosticServiceName"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.AccessionNumber+'</td>'+
		'		<td width="200"><div id="AccessionNumber"/></td>'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.Priority+'</td>'+
		'		<td width="200"><div id="OrderPriority"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.OrderingPhysician+'</td>'+
		'		<td width="200">'+
		'			<div id="OrderingPhysician"></div>'+
		'			<div id="OrderingPhysicianContactPointDetails" style="{font-size:75%;}">'+
		'				<div id="OrderingPhysicianAddress"/></div>'+
		'				<div id="OrderingPhysicianPhone"></div>'+
		'				<div id="OrderingPhysicianFax"></div>'+
		'				<div id="OrderingPhysicianEmail"></div>'+
		'			</div>'+
		'		</td>'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.PerformingFacilityDept+'</td>'+
		'		<td width="200"><div id="PerformingFacility"/></td>'+
		'	</tr>'+
		// Yen: Removed thess fields, since they aren't needed for the time being.  May need to add them back in future releases. (jr 2012)
		// '	<tr>'+
		// '		<td width="120" class="propertyname">Patient Class</td>'+
		// '		<td width="200"><div id="PatientClass"/></td>'+
		// '		<td width="120" class="propertyname">Location, Room/Bed</td>'+
		// '		<td width="200"><div id="LocationRoomBed"/></td>'+
		// '	</tr>'+
		'	<tr>'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.Indication+'</td>'+
		'		<td colspan="4"><div id="ReasonForStudy"/></td>'+
		'	</tr>'+
		'	<tr id="EnteredBySection">'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.EnteredBy+'</td>'+
		'		<td width="200" colspan="3"><div id="EnteredBy"/></td>'+
		'	</tr>'+
		'	<tr id="AlertsSection">'+
		'		<td width="120" class="propertyname">'+SR.ImagingServices.Alerts+'</td>'+
		'		<td width="200" colspan="3"><div id="Alerts"/></td>'+
		'	</tr>'+
		'	<tr id="CancelSection">'+
		'		<td colspan="4">'+
		'			<p class="subsectionheading">'+SR.ImagingServices.OrderCancelled+'</p>'+
		'			<table width="100%" border="0">'+
		'				<tr id="CancelledBySection">'+
		'					<td width="150" class="propertyname">'+SR.ImagingServices.CancelledBy+'</td>'+
		'					<td><div id="CancelledBy"/></td>'+
		'				</tr>'+
		'				<tr>'+
		'					<td width="150" class="propertyname">'+SR.ImagingServices.CancelReason+'</td>'+
		'					<td><div id="CancelReason"/></td>'+
		'				</tr>'+
		'			</table>'+
		'		</td>'+
		'	</tr>'+
		'</table></div>';

	function getPractitionerContactPoint(practitioner, recipients)
	{
		var contactPoint = null;
		recipients.each(function(recipient) 
		{
			if(recipient && recipient.Practitioner && recipient.Practitioner.PractitionerRef == practitioner.PractitionerRef)
			{
				contactPoint = recipient.ContactPoint;
			}
		});
		return contactPoint;
	}
	
	function formatPerformingFacility(procedureDetails)
	{
		var parts = procedureDetails
			.map(function(p) { return p.PerformingFacility.Name + (p.PerformingDepartment ? " ("+p.PerformingDepartment.Name+")" : ""); })
			.unique();
		return String.combine(parts, "<br>");
	}

	return {
		create: function (element, orderDetail, options)
		{
			if(orderDetail == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("DiagnosticServiceName"), orderDetail.DiagnosticService.Name);
			Field.setValue($("AccessionNumber"), Ris.formatAccessionNumber(orderDetail.AccessionNumber));
			Field.setValue($("OrderPriority"), orderDetail.OrderPriority.Value);
			Field.setLink($("OrderingPhysician"), Ris.formatPersonName(orderDetail.OrderingPractitioner.Name), function() { Ris.openPractitionerDetails(orderDetail.OrderingPractitioner); });
			Field.setValue($("PerformingFacility"), formatPerformingFacility(orderDetail.Procedures));
			
			// visit information may not exist, depending on whether the Visit Workflow feature is enabled
			// Yen: Removed thess fields, since they aren't needed for the time being.  May need to add them back in future releases. (jr 2012)
//			if (orderDetail.Visit && orderDetail.Visit.PatientClass) {
//				Field.setValue($("PatientClass"), orderDetail.Visit.PatientClass.Value);
//				Field.setValue($("LocationRoomBed"), Preview.formatVisitCurrentLocation(orderDetail.Visit));
//			}
			Field.setValue($("ReasonForStudy"), orderDetail.ReasonForStudy);
			Field.setValue($("EnteredBy"), orderDetail.EnteredBy ? (Ris.formatPersonName(orderDetail.EnteredBy.Name) + ' (' + orderDetail.EnteredBy.StaffType.Value + ')') : "");
			if (orderDetail.CancelReason)
			{
				Field.setValue($("CancelledBy"), Ris.formatStaffNameAndRole(orderDetail.CancelledBy));
				Field.setValue($("CancelReason"), orderDetail.CancelReason.Value);
			}
			else
			{
				Field.show($("CancelSection"), false);
			}

			Field.show($("EnteredBySection"), false);
			Field.show($("CancelledBySection"), false);
			Field.show($("AlertsSection"), false);
			Field.show($("OrderingPhysicianContactPointDetails"), false);

			if (options)
			{
				Field.show($("EnteredBySection"), options.ShowEnterCancelByStaff);
				Field.show($("CancelledBySection"), options.ShowEnterCancelByStaff);

				if (options.Alerts && options.Alerts.length > 0)
				{
					Field.show($("AlertsSection"), true);
					var alertHtml = "";
					options.Alerts.each(function(item) { alertHtml += Preview.getAlertHtml(item); });
					Field.setPreFormattedValue($("Alerts"), alertHtml);
				}

				var contactPoint = getPractitionerContactPoint(orderDetail.OrderingPractitioner, orderDetail.ResultRecipients);
				if (options.ShowOrderingPhysicianContactPointDetails && contactPoint)
				{
					Field.show($("OrderingPhysicianContactPointDetails"), true);

					Field.show($("OrderingPhysicianAddress"), contactPoint.CurrentAddress != null);
					Field.show($("OrderingPhysicianPhone"), contactPoint.CurrentPhoneNumber != null);
					Field.show($("OrderingPhysicianFax"), contactPoint.CurrentFaxNumber != null);
					Field.show($("OrderingPhysicianEmail"), contactPoint.CurrentEmailAddress != null);

					Field.setValue($("OrderingPhysicianAddress"), Ris.formatAddress(contactPoint.CurrentAddress));
					Field.setValue($("OrderingPhysicianPhone"), "Phone: " + Ris.formatTelephone(contactPoint.CurrentPhoneNumber));
					Field.setValue($("OrderingPhysicianFax"), "Fax: " + Ris.formatTelephone(contactPoint.CurrentFaxNumber));
					Field.setValue($("OrderingPhysicianEmail"), contactPoint.CurrentEmailAddress ? contactPoint.CurrentEmailAddress.Address : "");
				}
			}

			Preview.SectionContainer.create(element, SR.ImagingServices.ImagingService);
		}
	};

}();

/*
 *	Create a container for the element with the specified title.
 */
Preview.SectionContainer = function () {

	var _createContainer = function(element, title, collapsible, collapsedByDefault)
	{
		var _createContentContainer = function(contentElement)
		{
			var divElement = document.createElement("div");
			divElement.className = "ContentContainer";
			divElement.appendChild(contentElement);
			return divElement;
		};
		
		var _createCell = function(row, className, text)
		{
			var cell = row.insertCell();
			cell.className = className;
			cell.innerText = text || "";
			return cell;
		};
		
		var _createHeadingCell = function(row, className, text, collapsible, collapsedByDefault)
		{
			var cell = row.insertCell();
			cell.className = className;

			if (collapsible) 
			{
				var expandImageSrc = imagePath + "/" + "Expand.png";
				var collapseImageSrc = imagePath + "/" + "Collapse.png";

				// Preload the images so the browser will have the necessary images in the browser's cache before the script starts to run
				if (document.images)
				{
					var expandPic = new Image(11,11); 
					expandPic.src = expandImageSrc;

					var collapsePic = new Image(11,11); 
					collapsePic.src = collapseImageSrc;
				}

				cell.innerHTML = 
					"<a href='javascript:void(0)' class='collapsibleSectionHeading' onclick='Collapse.toggleCollapsedSection(this)' style='{text-decoration: none; color: white;}'>" +
					"<img src='" + (collapsedByDefault ? expandImageSrc : collapseImageSrc) + 
					"' border='0' style='{margin-right: 5px; margin-left: -8px; background: #1b4066;}'/>" + text + "</a>";
			}
			else
			{
				cell.innerText = text || "";
			}

			return cell;
		};

		var content = _createContentContainer(element);

		var table = document.createElement("table");
		var body = document.createElement("tbody");
		table.className = "SectionContainer";
		table.appendChild(body);

		var row, cell;
		row = body.insertRow();
		_createCell(row, "SectionHeadingLeft");
		_createHeadingCell(row, "SectionHeadingBackground", title, collapsible, collapsedByDefault);
		_createCell(row, "SectionHeadingRight");

		row = body.insertRow();
		cell = _createCell(row, "ContentCell");
		cell.colSpan = "3";
		cell.appendChild(content);
		
		if(collapsible) Collapse.collapseSection(table, collapsedByDefault);
		
		return table;
	};

	return {
		
		/*
			options:
				collapsible: true/false - indicates whether the section is collapsible or not
				initiallyCollapsed: true/false - indicates whether a collapsible section is initially collapsed
		*/
		create: function (element, title, options)
		{			
			// no need to create a contrainer if the element is hidden
			if (element.style.display == 'none')
				return;
			
			// default value for options if not supplied
			options = options || {};

			// Replace the element with the new element that is encased in the container.
			// We cannot simply use innerHTML because all the event handler of the element will not
			// be propagated to the new element.  Hence the DOM manipulation to preserve the handlers.
			var parent = element.parentNode;
			var nextSibling = element.nextSibling;
			var newElement = _createContainer(element, title, options.collapsible, options.initiallyCollapsed);
			if (nextSibling)
				parent.insertBefore(newElement, nextSibling);
			else
				parent.appendChild(newElement);
		}
	};

}();

/*
 *	Create a banner section of the pages with the content (i.e. patient demographics).
 */
Preview.PatientBannner = function () {

	var _createContentContainer = function(contentElement)
	{
		var divElement = document.createElement("div");
		divElement.appendChild(contentElement);
		return divElement;
	};

	var _createContainer = function(element)
	{
		var _createCell = function(row, className, text)
		{
			var cell = row.insertCell();
			cell.className = className;
			cell.innerText = text || "";
			return cell;
		};

		var content = _createContentContainer(element);

		var table = document.createElement("table");
		var body = document.createElement("tbody");
		table.style.borderSpacing = 0;
		table.style.padding = 0;
		table.appendChild(body);

		var row, cell;
		row = body.insertRow();
		_createCell(row, "PatientBanner_topleft");
		_createCell(row, "PatientBanner_top");
		_createCell(row, "PatientBanner_topright");

		row = body.insertRow();
		_createCell(row, "PatientBanner_left");
		cell = _createCell(row, "PatientBanner_content");
		cell.appendChild(content);
		_createCell(row, "PatientBanner_right");

		row = body.insertRow();
		_createCell(row, "PatientBanner_bottomleft");
		_createCell(row, "PatientBanner_bottom");
		_createCell(row, "PatientBanner_bottomright");

		return table;
	};

	return {
		create: function (element)
		{
			// no need to create a contrainer if the element is hidden
			if (element.style.display == 'none')
				return;

			// Replace the element with the new element that is encased in the container.
			// We cannot simply use innerHTML because all the event handler of the element will not
			// be propagated to the new element.  Hence the DOM manipulation to preserve the handlers.
			var parent = element.parentNode;
			var nextSibling = element.nextSibling;
			var newElement = _createContainer(element);
			if (nextSibling)
				parent.insertBefore(newElement, nextSibling);
			else
				parent.appendChild(newElement);
		}
	};

}();

/*
 *	Create a summary of demographic information of a single patient profile.
 */
Preview.PatientDemographicsSection = function () {
	var _html = 
		'<table border="0" cellspacing="0" cellpadding="0" class="PatientDemographicsTable">'+
		'	<tr>'+
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.DateOfBirth+'</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="dateOfBirth"/></td>'+
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.Age+'</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="age"/></td>'+
		'   </tr><tr>' +
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.Sex+'</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="sex"/></td>'+
		'		<td valign="top" class="DemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.Healthcard+'</td><td valign="top" class="DemographicsCell" nowrap="nowrap"><div id="healthcard"/></td>'+
		'	</tr>'+
		'	<tr id="BillingInformationRow">'+
		'		<td class="DemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.BillingInformation+'</td>'+
		'		<td colspan="3" class="DemographicsCell"><div id="billingInformation"/></td>'+
		'	</tr>'+
		'	<tr id="HomePhoneRow">'+
		'		<td class="ContactInfoDemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.HomePhone+'</td>'+
		'		<td colspan="3" class="ContactInfoDemographicsCell"><div id="currentHomePhone"/></td>'+
		'	</tr>'+
		'	<tr id="HomeAddressRow">'+
		'		<td class="ContactInfoDemographicsLabel" nowrap="nowrap">'+SR.PatientDemographics.HomeAddress+'</td>'+
		'		<td colspan="3" class="ContactInfoDemographicsCell" nowrap="nowrap"><div id="currentHomeAddress"/></td>'+
		'	</tr>'+
		'	<tr><td colspan="4"><img src="../images/blank.gif"/></td></tr>'+
		'</table>';

	return {
		create: function(element, patientProfile)
		{
			if(patientProfile == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("age"), Preview.getPatientAge(patientProfile.DateOfBirth, patientProfile.DeathIndicator, patientProfile.TimeOfDeath));
			Field.setValue($("sex"), patientProfile.Sex.Value);
			Field.setValue($("dateOfBirth"), Ris.formatDate(patientProfile.DateOfBirth));
			Field.setValue($("healthcard"), Ris.formatHealthcard(patientProfile.Healthcard));
			
			if (patientProfile.BillingInformation)
				Field.setValue($("billingInformation"), patientProfile.BillingInformation);
			else
				Field.show($("BillingInformationRow"), false);
				
			if (patientProfile.CurrentHomePhone)
				Field.setValue($("currentHomePhone"), Ris.formatTelephone(patientProfile.CurrentHomePhone));
			else
				Field.show($("HomePhoneRow"), false);

			if (patientProfile.CurrentHomeAddress)
				Field.setValue($("currentHomeAddress"), Ris.formatAddress(patientProfile.CurrentHomeAddress));
			else
				Field.show($("HomeAddressRow"), false);
		}
	};
}();

/*
 *	Create a banner showing a patient name, MRN and any provided alerts.
 */
Preview.PatientBannerSection = function() {
	return {
		create: function(element, patientProfile, alerts)
		{
			if(patientProfile == null)
				return;

			var patientName = Ris.formatPersonName(patientProfile.Name);
			var patientMRN = Ris.formatMrn(patientProfile.Mrn);
			Preview.BannerSection.create(element, patientName, patientMRN, patientName, alerts);
			
		}
	};
}();

/*
 *	Create a two line banner section with alerts
 *	Exposes:
 *		create(element, line1, line2, patientName, alerts)
 *			element - parent node for the banner
 *			line1 - first line text
 *			line2 - second line text
 *			patientName - patient name, used in alert hover text
 *			alerts - a list of alerts (from Ris.getPatientDataService) to display
 */
Preview.BannerSection = function() {
	var _html =
		'<table width="100%" border="0" cellspacing="0" cellpadding="0" class="PatientBannerTable">'+
		'	<tr>'+
		'		<td class="patientnameheading"><div id="line1" /></td>'+
		'		<td rowspan="2" align="right"><div id="alerts"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="patientmrnheading"><div id="line2"/></td>'+
		'	</tr>'+
		'</table>';

	return {
		create: function(element, line1, line2, patientName, alerts)
		{
			element.innerHTML = _html;

			//TODO: this seems pretty hokey, having "line1", "line2", "alerts" as identifiers - collisions are likely
			Field.setValue($("line1"), line1);
			Field.setValue($("line2"), line2);
			
			Preview.createAlerts($("alerts"), alerts, patientName);
			Preview.PatientBannner.create(element.parentNode);
		}
	};
}();

/*
 *	Create a order note view shoing author, post date, urgency, receipients and note body.
 *	Exposes:
 *		create(element, note)
 *			element - parent node for the order note
 *			note - the order note object
 */
Preview.OrderNoteSection = function() {
	var _formatStaffNameAndRoleAndOnBehalf = function(author, onBehalfOfGroup)
		{
      var authorName = Ris.formatStaffNameAndRole(author);
			return (onBehalfOfGroup != null) ? SR.OrderNotes.OnBehalfOf.interp(authorName, onBehalfOfGroup.Name) : authorName;
		};

	var _formatAcknowledgedTime = function(acknowledgedTime)
		{
			return acknowledgedTime ? " " + SR.OrderNotes.AcknowledgedAtTime.interp(Ris.formatDateTime(acknowledgedTime)) : "";
		};
		
	var _formatAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "<br>";

			// create string of group acknowledgements
			var formattedGroups = String.combine(
				groups.map(function(r) 
					{
						return _formatStaffNameAndRoleAndOnBehalf(r.AcknowledgedByStaff, r.Group) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			// create string of staff acknowledgements
			// if staff already acknowledged for a group, no need to list it the second time in the staff recipients
			var groupAckStaffIds = groups.map(function(r) { return r.AcknowledgedByStaff.StaffId; }).unique();
			var formattedStaff = String.combine(
				staffs.map(function(r) 
					{ 
						var staffIdAlreadyExist = groupAckStaffIds.find(function(id) { return id == r.Staff.StaffId; });
						return staffIdAlreadyExist ? "" : Ris.formatStaffNameAndRole(r.Staff) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	var _formatNotAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "; ";

			var formattedGroups = String.combine(
				groups.map(function(r) { return r.Group.Name; }), 
				recipientSeparator);

			var formattedStaff = String.combine(
				staffs.map(function(r) { return Ris.formatStaffNameAndRole(r.Staff); }), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	return {
		create: function(element, note)
		{
			if(note == null)
				return;

			note.GroupRecipients = note.GroupRecipients || [];
			note.StaffRecipients = note.StaffRecipients || [];
			var acknowledgedGroups = note.GroupRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var acknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var notAcknowledgedGroups = note.GroupRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });
			var notAcknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });

			var html = "";
			html += '<table style="{width:98%; margin-top: 4px;}" border="0" cellspacing="0" cellpadding="0">';
			html += '	<tr class="orderNoteHeading">';
			html += '		<td style="{width:100%;}"><span style="{padding-right: 5px;}" class="orderNoteLabel">'+SR.OrderNotes.From+'</span>';
			html += '		' + _formatStaffNameAndRoleAndOnBehalf(note.Author, note.OnBehalfOfGroup) + '</td>';
			//html += '		<td>' + (note.Urgent ? "<img alt='Urgent' src='" + imagePath + "/urgent.gif'/>" : "") + '</td>';
			html += '		<td style="{width:5em; text-align:right; padding-right: 5px;}">' + (note.Urgent ? '<span class="urgentTextMark">'+SR.OrderNotes.LabelUrgent+'</span>' : "")  + '</td>';
			html += '		<td style="{width:9.5em;text-align:right; padding-right: 5px;}" class="orderNoteLabel" NOWRAP title="' +  Ris.formatDateTime(note.PostTime) + '">' + Ris.formatDateTime(note.PostTime) + '</td>';
			html += '	</tr>';
			if (acknowledgedGroups.length > 0 || acknowledgedStaffs.length > 0)
			{
				html += '	<tr id="acknowledgedRow" class="orderNoteHeading">';
				html += '		<td colspan="3" NOWRAP valign="top"><span style="{padding-right: 5px;}" class="orderNoteLabel">'+SR.OrderNotes.AcknowledgedBy+'</span>';
				html += '		' + String.replaceLineBreak(_formatAcknowledged(acknowledgedGroups, acknowledgedStaffs)) + '<div id="acknowledged"></td>';
				html += '	</tr>';
			}
			if (notAcknowledgedGroups.length > 0 || notAcknowledgedStaffs.length > 0)
			{
				html += '	<tr id="notAcknowledgedRow" class="orderNoteHeading">';
				html += '		<td valign="top" colspan="3"><span style="{padding-right: 5px;}" class="orderNoteLabel">'+SR.OrderNotes.WaitingForAcknowledgement+'</span>';
				html += '		<B>' + String.replaceLineBreak(_formatNotAcknowledged(notAcknowledgedGroups, notAcknowledgedStaffs)) + '</B></td>';
				html += '	</tr>';
			}
			html += '	<tr>';
			html += '		<td colspan="3" class="orderNote">' +  String.replaceLineBreak(note.NoteBody) + '</td>';
			html += '	</tr>';
			html += '</table>';
			
			element.innerHTML = html;
		}
	};
}();

/*
 *	Create a conversation note that shows author, post date, urgency, recipients and note body.
 *	Exposes:
 *		create(element, note)
 *			element - parent node for the order note
 *			note - the order note object
 */
Preview.ConversationNote = function() {
	var _formatStaffNameAndRoleAndOnBehalf = function(author, onBehalfOfGroup)
		{
      var authorName = Ris.formatStaffNameAndRole(author);
			return (onBehalfOfGroup != null) ? SR.OrderNotes.OnBehalfOf.interp(authorName, onBehalfOfGroup.Name) : authorName;
		};

	var _formatAcknowledgedTime = function(acknowledgedTime)
		{
			return acknowledgedTime ? " " + SR.OrderNotes.AcknowledgedAtTime.interp(Ris.formatDateTime(acknowledgedTime)) : "";
		};
		
	var _formatAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "<br>";

			// create string of group acknowledgements
			var formattedGroups = String.combine(
				groups.map(function(r) 
					{
						return _formatStaffNameAndRoleAndOnBehalf(r.AcknowledgedByStaff, r.Group) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			// create string of staff acknowledgements
			// if staff already acknowledged for a group, no need to list it the second time in the staff recipients
			var groupAckStaffIds = groups.map(function(r) { return r.AcknowledgedByStaff.StaffId; }).unique();
			var formattedStaff = String.combine(
				staffs.map(function(r) 
					{ 
						var staffIdAlreadyExist = groupAckStaffIds.find(function(id) { return id == r.Staff.StaffId; });
						return staffIdAlreadyExist ? "" : Ris.formatStaffNameAndRole(r.Staff) + _formatAcknowledgedTime(r.AcknowledgedTime); 
					}), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	var _formatNotAcknowledged = function(groups, staffs)
		{
			var recipientSeparator = "; ";

			var formattedGroups = String.combine(
				groups.map(function(r) { return r.Group.Name; }), 
				recipientSeparator);

			var formattedStaff = String.combine(
				staffs.map(function(r) { return Ris.formatStaffNameAndRole(r.Staff); }), 
				recipientSeparator);

			return String.combine([formattedGroups, formattedStaff], recipientSeparator);
		};
	
	return {
		create: function(element, note)
		{			
			if(note == null)
				return;

			note.GroupRecipients = note.GroupRecipients || [];
			note.StaffRecipients = note.StaffRecipients || [];
			var acknowledgedGroups = note.GroupRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var acknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return recipient.IsAcknowledged; });
			var notAcknowledgedGroups = note.GroupRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });
			var notAcknowledgedStaffs = note.StaffRecipients.select(function(recipient) { return !recipient.IsAcknowledged; });
			var checkBoxId = note.OrderNoteRef;

			var html = "";
			html += '<table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td class="ConversationNote_topleft"></td><td class="ConversationNote_top"></td><td class="ConversationNote_topright"></td></tr>';
			html += '<tr><td class="ConversationNote_left_upper"></td><td class="ConversationNote_content_upper">';
			html += '<table width="100%" class="ConversationNoteDetails" border="0" cellspacing="0" cellpadding="0">';
			html += '	<tr>';
			html += '		<td><span style="{color: #205F87; font-weight: bold; padding-right: 10px;}">'+SR.OrderNotes.From+'</span> ' 
				+  _formatStaffNameAndRoleAndOnBehalf(note.Author, note.OnBehalfOfGroup) 
				//+ '<span style="{padding-left: 20px;}">' 
				//+ (note.Urgent ? "<img alt='Urgent' src='" + imagePath + "/urgent.gif'/>" : "") 
				+ (note.Urgent ? '<span class="urgentTextMark" style="{margin-left: 20px;}">'+SR.OrderNotes.LabelUrgent+'</span>' : "") + "</td>";
				//+ '</span></td>';
			html += '		<td style="{padding-right: 10px; text-align:right; color: #205F87; font-weight: bold;}" NOWRAP title="' +  Ris.formatDateTime(note.PostTime) + '">' + Ris.formatDateTime(note.PostTime) + '</td>';
			html += '	</tr>';
			if (acknowledgedGroups.length > 0 || acknowledgedStaffs.length > 0) {
				html += '	<tr id="acknowledgedRow">';
				html += '		<td colspan="2" NOWRAP valign="top"><span style="{color: #205F87; font-weight: bold; padding-right: 10px;}">'+SR.OrderNotes.AcknowledgedBy+'</span>';
				html += '		' + String.replaceLineBreak(_formatAcknowledged(acknowledgedGroups, acknowledgedStaffs)) + '<div id="acknowledged"></td>';
				html += '	</tr>';
			}
			if (notAcknowledgedGroups.length > 0 || notAcknowledgedStaffs.length > 0) {
				html += '	<tr id="notAcknowledgedRow">';
				html += note.CanAcknowledge
						? '		<td valign="middle" colspan="2" ><input type="checkbox" id="' + checkBoxId + '"/><span style="{margin-left: 5px; margin-right: 10px;}">'+SR.OrderNotes.WaitingForAcknowledgement+'</span>'
						: '		<td colspan="2" NOWRAP valign="top"><span style="{padding-right: 10px;}">'+SR.OrderNotes.WaitingForAcknowledgement+'</span>';
				html += '		' + String.replaceLineBreak(_formatNotAcknowledged(notAcknowledgedGroups, notAcknowledgedStaffs)) + '</td>';
				html += '	</tr>';
			}
			html += '   </table>';
			html += '   </td><td class="ConversationNote_right_upper"></td></tr>';
			html += '   <tr><td class="ConversationNote_left_lower"></td><td class="ConversationNote_content_lower"><table>'
			html += '	<tr>';
			html += '		<td colspan="4" style="{text-align:justify;}"><div class="ConversationNoteMessage">' +  String.replaceLineBreak(note.NoteBody) + '</div></td>';
			html += '   </table>';
			html += '</td><td class="ConversationNote_right_lower"></td></tr>';
			html += '<tr><td class="ConversationNote_bottomleft"></td><td class="ConversationNote_bottom"></td><td class="ConversationNote_bottomright"></td></tr></table>';
			html += '	</tr>';
			element.innerHTML = html;
		}
	};
}();

Preview.OrderedProceduresTable = function() {

	return {
		create: function(parentElement, procedures)
		{
			if(procedures.length == 0)
			{
				parentElement.style.display = 'none';
				return;
			}
			else
			{
				parentElement.style.display = 'block';
			}

			var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement, "ProceduresTable");
			var htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
				[
					{	label: SR.OrderedProcedures.ColumnHeadings.Procedure,
						cellType: "html",
						getValue: function(item) 
						{
							var html = "";
							var formattedProcedureStatus = Preview.ProceduresTableHelper.formatProcedureStatus(item.Status, item.ScheduledStartTime, item.StartTime, item.CheckInTime, item.CheckOutTime);
							
							html += "<p class='sectionheading'>";
							html += "	<a href='javascript:void(0)' class='collapsibleHeading' onclick='Collapse.toggleCollapsed(this)'><span class='plusMinus'>+</span>" + Ris.formatProcedureName(item) + "</a>";
							html += "</p>";
							html += "<div class='collapsibleContent'>";
							html += "<table>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.Status+"</td>";
							html += "	<td width='200'>" + formattedProcedureStatus + "</td>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.PerformingFacility+"</td>";
							html += "	<td width='200'>" + item.PerformingFacility.Name + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.ScheduledStartTime+"</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.ScheduledStartTime) + "</td>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.SchedulingCode+"</td>";
							html += "	<td width='200'>" + Preview.ProceduresTableHelper.formatProcedureSchedulingCode(item.SchedulingCode) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.CheckInTime+"</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.CheckInTime) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.PerformingStartTime+"</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.StartTime) + "</td>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.PerformingEndTime+"</td>";
							html += "	<td width='200'>" + Ris.formatDateTime(item.CheckOutTime) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.ReportPublishedTime+"</td>";
							if(item.Status.Code == 'CA' || item.Status.Code == 'DC')
							{
								html += "	<td width='200'></td>";
							}
							else
							{
								html += "	<td width='200'>" + Ris.formatDateTime(item.EndTime) + "</td>";
							}
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.OrderedProcedures.CancelledTime+"</td>";
							if(item.Status.Code == 'CA' || item.Status.Code == 'DC')
							{
								html += "	<td width='200'>" + Ris.formatDateTime(item.EndTime) + "</td>";
							}
							else
							{
								html += "	<td width='200'></td>";
							}
							html += "</tr>";
							html += "</table>";
							html += "</div>";

							return html;
						}
					}
				]);

			htmlTable.errorProvider = errorProvider;   // share errorProvider with the rest of the form
			htmlTable.bindItems(procedures);

			Preview.SectionContainer.create(parentElement, SR.OrderedProcedures.OrderedProcedures);
		}
	}
}();

/*
 *	Create a table of details for a single visit.
 */ 
Preview.VisitDetailsSection = function () {

	var _html = 
		'<div class="SectionTableContainer">'+
		'	<table cellspacing="5">'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.VisitNumber+'</td>'+
		'			<td width="200"><div id="VisitNumber"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Visits.Facility+'</td>'+
		'			<td width="200"><div id="Facility"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.VisitStatus+'</td>'+
		'			<td width="200"><div id="VisitStatus"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Visits.CurrentLocation+'</td>'+
		'			<td width="200"><div id="CurrentLocation"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.PatientClass+'</td>'+
		'			<td width="200"><div id="PatientClass"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Visits.CurrentRoom+'</td>'+
		'			<td width="200"><div id="CurrentRoom"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.PatientType+'</td>'+
		'			<td width="200"><div id="PatientType"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Visits.CurrentBed+'</td>'+
		'			<td width="200"><div id="CurrentBed"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.AdmissionType+'</td>'+
		'			<td width="200"><div id="AdmissionType"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Visits.DischargeDisposition+'</td>'+
		'			<td width="200"><div id="DischargeDisposition"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.AdmitDateTime+'</td>'+
		'			<td width="200"><div id="AdmitTime"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Visits.DischargeDateTime+'</td>'+
		'			<td width="200"><div id="DischargeTime"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.PreAdmitNumber+'</td>'+
		'			<td><div id="PreAdmitNumber"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.VIP+'</td>'+
		'			<td><div id="VipFlag"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Visits.AmbulatoryStatuses+'</td>'+
		'			<td><div id="AmbulatoryStatuses"/></td>'+
		'		</tr>'+
		'	</table>'+
		'</div>';

	return 	{
		create: function(element, visitDetail) {

			if(visitDetail == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("VisitNumber"), Ris.formatMrn(visitDetail.VisitNumber));
			Field.setValue($("Facility"), visitDetail.Facility.Name);
			Field.setValue($("VisitStatus"), visitDetail.Status.Value);
			Field.setValue($("AdmitTime"), Ris.formatDateTime(visitDetail.AdmitTime));
			Field.setValue($("DischargeTime"), Ris.formatDateTime(visitDetail.DischargeTime));
			Field.setValue($("PatientClass"), visitDetail.PatientClass ? visitDetail.PatientClass.Value : "");
			Field.setValue($("PatientType"), visitDetail.PatientType ? visitDetail.PatientType.Value : "");
			Field.setValue($("AdmissionType"), visitDetail.AdmissionType ? visitDetail.AdmissionType.Value : "");
			Field.setValue($("DischargeDisposition"), visitDetail.DischargeDisposition);
			Field.setValue($("CurrentLocation"), visitDetail.CurrentLocation ? visitDetail.CurrentLocation.Name : null);
			Field.setValue($("CurrentRoom"), visitDetail.CurrentRoom ? visitDetail.CurrentRoom : null);
			Field.setValue($("CurrentBed"), visitDetail.CurrentBed ? visitDetail.CurrentBed : null);
			Field.setValue($("PreAdmitNumber"), visitDetail.PreadmitNumber);
			Field.setValue($("VipFlag"), visitDetail.VipIndicator ? "Yes" : "No");
			
			var ambulatoryStatuses = String.combine(visitDetail.AmbulatoryStatuses.map(function(status) { return status.Value; }), ", ");
			Field.setValue($("AmbulatoryStatuses"), ambulatoryStatuses);

			Preview.SectionContainer.create(element, "Visit Details");
		}
	};
}();

/*
 *	Create a summary of the physician details of a single visit.
 */ 
Preview.PhysiciansSection = function () {

	var _html = 
		'<div class="SectionTableContainer">'+
		'	<table cellspacing="5">'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Physicians.AttendingPhysician+'</td>'+
		'			<td width="200"><div id="AttendingPhysician"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Physicians.ReferringPhysician+'</td>'+
		'			<td width="200"><div id="ReferringPhysician"/></td>'+
		'		</tr>'+
		'		<tr>'+
		'			<td width="120" class="propertyname">'+SR.Physicians.ConsultingPhysician+'</td>'+
		'			<td width="200"><div id="ConsultingPhysician"/></td>'+
		'			<td width="120" class="propertyname">'+SR.Physicians.AdmittingPhysician+'</td>'+
		'			<td width="200"><div id="AdmittingPhysician"/></td>'+
		'		</tr>'+
		'	</table>'+
		'</div>';

	return 	{
		create: function(element, visitDetail) {

			if(visitDetail == null || visitDetail.ExtendedProperties == null)
				return;

			element.innerHTML = _html;

			Field.setValue($("AttendingPhysician"), visitDetail.ExtendedProperties.AttendingPhysician);
			Field.setValue($("ReferringPhysician"), visitDetail.ExtendedProperties.ReferringPhysician);
			Field.setValue($("ConsultingPhysician"), visitDetail.ExtendedProperties.ConsultingPhysician);
			Field.setValue($("AdmittingPhysician"), visitDetail.ExtendedProperties.AdmittingPhysician);

			Preview.SectionContainer.create(element, "Physicians");
		}
	};
}();

Preview.ExternalPractitionerSummary = function() {

	var _detailsHtml = 
		'<table cellspacing="5" class="PatientDemographicsTable">'+
		'	<tr>'+
		'		<td width="150" class="DemographicsLabel">'+SR.ExternalPractitioners.LicenseNumber+'</td>'+
		'		<td><div id="LicenseNumber"/></td>'+
		'		<td width="150" class="DemographicsLabel">'+SR.ExternalPractitioners.VerifiedStatus+'</td>'+
		'		<td><div id="IsVerified"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td width="150" class="DemographicsLabel">'+SR.ExternalPractitioners.BillingNumber+'</td>'+
		'		<td><div id="BillingNumber"/></td>'+
		'		<td width="150" class="DemographicsLabel">'+SR.ExternalPractitioners.LastVerifiedAt+'</td>'+
		'		<td><div id="LastVerified"/></td>'+
		'	</tr>'+
		'	</table>';

	var _createBanner = function(element, externalPractitionerSummary, alerts)
		{
			var bannerContainer = document.createElement("div");
			element.appendChild(bannerContainer);

			var bannerName = document.createElement("div");
			bannerContainer.appendChild(bannerName);
			var formattedName = Ris.formatPersonName(externalPractitionerSummary.Name);
			Preview.BannerSection.create(bannerName, formattedName, "", formattedName, alerts);

			var bannerDetails = document.createElement("div");
			bannerDetails.innerHTML = _detailsHtml;
			bannerContainer.appendChild(bannerDetails);

			Field.setValue($("LicenseNumber"), externalPractitionerSummary.LicenseNumber);
			Field.setValue($("BillingNumber"), externalPractitionerSummary.BillingNumber);
			Field.setValue($("IsVerified"), externalPractitionerSummary.IsVerified ? SR.ExternalPractitioners.VerifiedYes : SR.ExternalPractitioners.VerifiedNo);
			Field.setValue($("LastVerified"), Ris.formatDateTime(externalPractitionerSummary.LastVerifiedTime));
		};

	var _createExtendedPropertiesTable = function (element, externalPractitionerSummary)
		{
			var extendedProperties = externalPractitionerSummary.ExtendedProperties;
			if(!extendedProperties)
				return;

			var propertyArray = [];
			for (var propertyName in extendedProperties)
			{
				if (extendedProperties.hasOwnProperty(propertyName))
				{
					var propertyValue = propertyName == "AccessionNumber"
						? Ris.formatAccessionNumber(extendedProperties[propertyName])
						: extendedProperties[propertyName];
					
					var p = { PropertyName: propertyName, PropertyValue: propertyValue };
					propertyArray.push(p);
				}
			}

			if(propertyArray.length == 0)
				return;

			var htmlTable = Preview.ProceduresTableHelper.addTable(element, "ExtendedPropertiesTable");
			htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
				 [
					{   label: SR.ExternalPractitioners.ColumnHeadings.Name,
						cellType: "text",
						getValue: function(item) { return item.PropertyName; }
					},
					{   label: SR.ExternalPractitioners.ColumnHeadings.Value,
						cellType: "html",
						getValue: function(item) { return item.PropertyValue; }
					}
				 ]);

			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(propertyArray);
			
			Preview.SectionContainer.create(htmlTable, SR.ExternalPractitioners.AdditionalProperties);
		}

	var _createContactPointTable = function(element, externalPractitionerSummary)
		{
			if(!externalPractitionerSummary.ContactPoints || externalPractitionerSummary.ContactPoints.length == 0)
				return;

			var activeContactPoints = externalPractitionerSummary.ContactPoints.select(function(item) { return item.Deactivated == false; });

			var htmlTable = Preview.ProceduresTableHelper.addTable(element, "ContactPointsTable");
			var htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false },
				[
					{
						label: SR.ExternalPractitionerContactPoints.ColumnHeadings.Name,
						cellType: "html",
						getValue: function(item)
						{
							var name;
							if(item.IsDefaultContactPoint)
								name = SR.ExternalPractitionerContactPoints.DefaultContactPoint.interp(item.Name);
							else
								name = item.Name;
							return "<div class='DemographicsLabel'>" + name + "</div>"
						}
					},
					{	label: SR.ExternalPractitionerContactPoints.ColumnHeadings.ContactPoint,
						cellType: "html",
						getValue: function(item) 
						{
							var html = "";

							html += "<table>";
							if(item.Description)
							{
								html += "<tr>";
								html += "	<td width='120' class='propertyname'>"+SR.ExternalPractitionerContactPoints.Description+"</td>";
								html += "	<td >" + item.Description + "</td>";
								html += "</tr>";
							}
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.ExternalPractitionerContactPoints.PhoneNumber+"</td>";
							html += "	<td>" + (Ris.formatTelephone(item.CurrentPhoneNumber) || SR.ExternalPractitionerContactPoints.NotEntered) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.ExternalPractitionerContactPoints.FaxNumber+"</td>";
							html += "	<td>" + (Ris.formatTelephone(item.CurrentFaxNumber) || SR.ExternalPractitionerContactPoints.NotEntered) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.ExternalPractitionerContactPoints.Address+"</td>";
							html += "	<td>" + (Ris.formatAddress(item.CurrentAddress) || SR.ExternalPractitionerContactPoints.NotEntered) + "</td>";
							html += "</tr>";
							html += "<tr>";
							html += "	<td width='120' class='propertyname'>"+SR.ExternalPractitionerContactPoints.EmailAddress+"</td>";
							html += "	<td>" + (item.CurrentEmailAddress && item.CurrentEmailAddress.Address ? item.CurrentEmailAddress.Address : SR.ExternalPractitionerContactPoints.NotEntered) + "</td>";
							html += "</tr>";

							html += "</table>";

							return html;
						}
					}
				]);
			htmlTable.rowCycleClassNames = ["row0", "row1"];
			htmlTable.bindItems(activeContactPoints);

			Preview.SectionContainer.create(htmlTable, SR.ExternalPractitionerContactPoints.ContactPoints);
		};

	return {
		create: function(element, externalPractitionerSummary, alerts) {

			if(externalPractitionerSummary == null)
				return;

			_createBanner(element, externalPractitionerSummary, alerts);
			_createExtendedPropertiesTable(element, externalPractitionerSummary);
			_createContactPointTable(element, externalPractitionerSummary);
		}
	};
}();

Preview.ResultRecipientsSection = function() {

	var _removeOrderingPractitionerFromRecipientsList = function(orderingPractitioner, resultRecipients)
	{
		if (!orderingPractitioner)
			return;

		// select the recipient representing the ordering practitioner at the default contact point
		var orderingRecipient = resultRecipients.find(
			function(recipient) 
			{ 
				return recipient.Practitioner.PractitionerRef == orderingPractitioner.PractitionerRef && recipient.ContactPoint.IsDefaultContactPoint;
			});

			// if not found, then select the first recipient representing the ordering practitioner
		if (orderingRecipient == null)
		{
			orderingRecipient = resultRecipients.find(
				function(recipient) 
				{ 
					return recipient.Practitioner.PractitionerRef == orderingPractitioner.PractitionerRef;
				});
		}

		// if the recipient object exists for the ordering practitioner (and this *should* always be the case), remove it
		if (orderingRecipient != null)
			resultRecipients.remove(orderingRecipient);
	};

	var _createResultRecipientsTable = function(parentElement, resultRecipients, title)
	{
		if (resultRecipients.length == 0)
			return;
			
		if(title)
		{
			Preview.ProceduresTableHelper.addHeading(parentElement, title, 'subsectionheading');
		}

		var htmlTable = Preview.ProceduresTableHelper.addTable(parentElement);
		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
		[
			{   label: SR.ResultRecipients.ColumnHeadings.Practitioner,
				cellType: "link",
				getValue: function(item) { return Ris.formatPersonName(item.Practitioner.Name); },
				clickLink: function(item) { Ris.openPractitionerDetails(item.Practitioner); }
			},
			{	label: SR.ResultRecipients.ColumnHeadings.ContactPoint,
				cellType: "html",
				getValue: function(item) 
				{
					var contactPoint = item.ContactPoint;
					var html = "";

					html += "<table>";
					html += "<tr>";
					html += "	<td width='120' class='propertyname'>"+SR.ResultRecipients.Name+"</td>";
					html += "	<td>" + contactPoint.Name + "</td>";
					html += "</tr>";
					if(contactPoint.Description)
					{
						html += "<tr>";
						html += "	<td width='120' class='propertyname'>"+SR.ResultRecipients.Description+"</td>";
						html += "	<td >" + contactPoint.Description + "</td>";
						html += "</tr>";
					}
					html += "<tr>";
					html += "	<td width='120' class='propertyname'>"+SR.ResultRecipients.PhoneNumber+"</td>";
					html += "	<td>" + (Ris.formatTelephone(contactPoint.CurrentPhoneNumber) || SR.ResultRecipients.NotEntered) + "</td>";
					html += "</tr>";
					html += "<tr>";
					html += "	<td width='120' class='propertyname'>"+SR.ResultRecipients.FaxNumber+"</td>";
					html += "	<td>" + (Ris.formatTelephone(contactPoint.CurrentFaxNumber) || SR.ResultRecipients.NotEntered) + "</td>";
					html += "</tr>";
					html += "<tr>";
					html += "	<td width='120' class='propertyname'>"+SR.ResultRecipients.Address+"</td>";
					html += "	<td>" + (Ris.formatAddress(contactPoint.CurrentAddress) || SR.ResultRecipients.NotEntered) + "</td>";
					html += "</tr>";
					html += "<tr>";
					html += "	<td width='120' class='propertyname'>"+SR.ResultRecipients.EmailAddress+"</td>";
					html += "	<td>" + (contactPoint.CurrentEmailAddress && contactPoint.CurrentEmailAddress.Address ? contactPoint.CurrentEmailAddress.Address : SR.ResultRecipients.NotEntered) + "</td>";
					html += "</tr>";

					html += "</table>";

					return html;
				}
			}
		]);

		htmlTable.rowCycleClassNames = ["row1", "row0"];
		htmlTable.bindItems(resultRecipients);
	};

	return {
		create: function(parentElement, orderingPractitioner, resultRecipients, heading)
		{
			if(resultRecipients.length == 0)
				return;

			_removeOrderingPractitionerFromRecipientsList(orderingPractitioner, resultRecipients);
			_createResultRecipientsTable(parentElement, resultRecipients);

			Preview.SectionContainer.create(parentElement, heading, { collapsible: true, initiallyCollapsed: false });
		}
	}
}();

Preview.WorklistItemsPreview = function () {

	var _worklistPropertiesHtml = 
		'<p class="sectionheading">'+SR.Worklists.Worklist+'</p>'+
		'<table>'+
		'	<tr>'+
		'		<td class="propertyname" width="125">'+SR.Worklists.PrintedBy+'</td>'+
		'		<td><div id="PrintedBy"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="propertyname">'+SR.Worklists.FolderSystem+'</td>'+
		'		<td><div id="FolderSystem"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="propertyname">'+SR.Worklists.FolderName+'</td>'+
		'		<td><div id="FolderName"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="propertyname">'+SR.Worklists.Description+'</td>'+
		'		<td><div id="Description"/></td>'+
		'	</tr>'+
		'	<tr>'+
		'		<td class="propertyname">'+SR.Worklists.Showing+'</td>'+
		'		<td><div id="ShowingCount"/></td>'+
		'	</tr>'+
		'</table>'+
		'<br>';

	var _formatProcedure = function(procedureName, portable, lateralityEnum)
	{
		var laterality = lateralityEnum && lateralityEnum.Code != 'N' ? lateralityEnum.Value : null;

		if (portable && laterality)
			return procedureName + " (" + SR.Worklists.Portable + "/" + laterality + ")";
		else if (portable)
			return procedureName + " (" + SR.Worklists.Portable + ")";
		else if (laterality)
			return procedureName + " (" + laterality + ")";

		return procedureName;
	};

	var _createPropertiesTable = function(element, worklist)
	{
		var worklistDetailsSection = document.createElement("div");
		worklistDetailsSection.innerHTML = _worklistPropertiesHtml;
		element.appendChild(worklistDetailsSection);

		Field.setValue($("PrintedBy"), Ris.formatPersonName(worklist.PrintedBy) + " on " + Ris.formatDateTime(worklist.PrintedTime));
		Field.setValue($("FolderSystem"), worklist.FolderSystemName);
		Field.setValue($("FolderName"), worklist.FolderName);
		Field.setValue($("Description"), worklist.FolderDescription);
		Field.setValue($("ShowingCount"), worklist.Items.length == worklist.TotalItemCount 
			? "All " + worklist.TotalItemCount + " items"
			: worklist.Items.length + " of " + worklist.TotalItemCount + " items");
	};

	var _createItemsTable = function(element, items)
	{
		if(items.length == 0)
			return;

		var worklistItemsSection = document.createElement("div");
		element.appendChild(worklistItemsSection);
			
		var htmlTable = Preview.ProceduresTableHelper.addTable(worklistItemsSection);
		htmlTable = Table.createTable(htmlTable, { editInPlace: false, flow: false, addColumnHeadings: true },
			 [
				{   label: SR.Procedures.Mrn,
					cellType: "text",
					getValue: function(item) { return Ris.formatMrn(item.Mrn); }
				},
				{   label: SR.Procedures.Name,
					cellType: "text",
					getValue: function(item) { return Ris.formatPersonName(item.PatientName); }
				},
				{   label: SR.Procedures.AccessionNumber,
					cellType: "text",
					noWrap: true,
					getValue: function(item) { return item.AccessionNumber; }
				},
				{   label: SR.Procedures.OrderPriority,
					cellType: "text",
					getValue: function(item) { return item.OrderPriority.Value; }
				},
				{   label: SR.Procedures.PatientClass,
					cellType: "text",
					getValue: function(item) { return item.PatientClass.Value; }
				},
				{   label: SR.Procedures.Procedure,
					cellType: "text",
					getValue: function(item) { return _formatProcedure(item.ProcedureName, item.ProcedurePortable, item.ProcedureLaterality); }
				},
				{   label: SR.Procedures.Time,
					cellType: "html",
					noWrap: true,
					getValue: function(item) { return Ris.formatDate(item.Time) + "<br>" + Ris.formatTime(item.Time); }
				}
			 ]);

		htmlTable.rowCycleClassNames = ["row0", "row1"];
		htmlTable.bindItems(items);
	};

	return {
		create: function(element, printContext)
		{
			_createPropertiesTable(element, printContext);
			_createItemsTable(element, printContext.Items);
		}
	};
}();