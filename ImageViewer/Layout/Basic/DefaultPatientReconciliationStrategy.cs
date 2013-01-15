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
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[Cloneable(true)]
	public class PatientInformation : IPatientData
	{
		public PatientInformation()
		{
		}
		internal PatientInformation(IPatientData patientData)
		{
			PatientId = patientData.PatientId;
			PatientsName = patientData.PatientsName;
			PatientsBirthDate = patientData.PatientsBirthDate;
			PatientsBirthTime = patientData.PatientsBirthTime;
			PatientsSex = patientData.PatientsSex;

			PatientSpeciesDescription = patientData.PatientSpeciesDescription;
			PatientSpeciesCodeSequenceCodingSchemeDesignator = patientData.PatientSpeciesCodeSequenceCodingSchemeDesignator;
			PatientSpeciesCodeSequenceCodeValue = patientData.PatientSpeciesCodeSequenceCodeValue;
			PatientSpeciesCodeSequenceCodeMeaning = patientData.PatientSpeciesCodeSequenceCodeMeaning;
			PatientBreedDescription = patientData.PatientBreedDescription;
			PatientBreedCodeSequenceCodingSchemeDesignator = patientData.PatientBreedCodeSequenceCodingSchemeDesignator;
			PatientBreedCodeSequenceCodeValue = patientData.PatientBreedCodeSequenceCodeValue;
			PatientBreedCodeSequenceCodeMeaning = patientData.PatientBreedCodeSequenceCodeMeaning;
			ResponsiblePerson = patientData.ResponsiblePerson;
			ResponsiblePersonRole = patientData.ResponsiblePersonRole;
			ResponsibleOrganization = patientData.ResponsibleOrganization;

		}

		#region IPatientData Members

		public string PatientId { get; set; }
		public string PatientsName { get; private set; }
		public string PatientsBirthDate { get; private set; }
		public string PatientsBirthTime { get; private set; }
		public string PatientsSex { get; private set; }

		#region Species

		public string PatientSpeciesDescription { get; private set; }
		public string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; private set; }
		public string PatientSpeciesCodeSequenceCodeValue { get; private set; }
		public string PatientSpeciesCodeSequenceCodeMeaning { get; private set; }

		#endregion

		#region Breed

		public string PatientBreedDescription { get; private set; }
		public string PatientBreedCodeSequenceCodingSchemeDesignator { get; private set; }
		public string PatientBreedCodeSequenceCodeValue { get; private set; }
		public string PatientBreedCodeSequenceCodeMeaning { get; private set; }

		#endregion

		#region Responsible Person/Organization

		public string ResponsiblePerson { get; private set; }
		public string ResponsiblePersonRole { get; private set; }
		public string ResponsibleOrganization { get; private set; }

		#endregion
		#endregion

		public PatientInformation Clone()
		{
			return CloneBuilder.Clone(this) as PatientInformation;
		}
	}

	//TODO: at some point in the future, expand to a full blown auto reconciler that just wraps the Ris' reconciliation service.

	internal interface IPatientReconciliationStrategy
	{
		//NOTE: I dislike doing this everywhere - need centralized study management.
		void SetStudyTree(StudyTree studyTree);

		IPatientData ReconcileSearchCriteria(IPatientData patient);
		IPatientData ReconcilePatientInformation(IPatientData patient);
	}

	public class DefaultPatientReconciliationStrategy : IPatientReconciliationStrategy
	{
		#region PatientInformation class

		#endregion

		private readonly XmlActionsApplicator _applicator;

		public DefaultPatientReconciliationStrategy()
		{
			_applicator = new XmlActionsApplicator(DefaultActions.GetStandardActions());
		}

		private StudyTree StudyTree { get; set; }

		void IPatientReconciliationStrategy.SetStudyTree(StudyTree studyTree)
		{
			StudyTree = studyTree;		
		}

		public IPatientData ReconcileSearchCriteria(IPatientData patientInfo)
		{
			var patientInformation = new PatientInformation{ PatientId = patientInfo.PatientId };
			return Reconcile(patientInformation, DefaultPatientReconciliationSettings.Default.SearchReconciliationRulesXml, "search-reconciliation-rules");
		}

		public IPatientData ReconcilePatientInformation(IPatientData patientInfo)
		{
			Platform.CheckMemberIsSet(StudyTree, "StudyTree");

			var testPatientInformation = new PatientInformation{ PatientId = patientInfo.PatientId };
			testPatientInformation = Reconcile(testPatientInformation, DefaultPatientReconciliationSettings.Default.PatientReconciliationRulesXml, "patient-reconciliation-rules");

			foreach (var patient in StudyTree.Patients)
			{
				var reconciledPatientInfo = new PatientInformation { PatientId = patient.PatientId };
				reconciledPatientInfo = Reconcile(reconciledPatientInfo, DefaultPatientReconciliationSettings.Default.PatientReconciliationRulesXml, "patient-reconciliation-rules");

				if (reconciledPatientInfo.PatientId == testPatientInformation.PatientId)
					return new PatientInformation(patient) { PatientId = reconciledPatientInfo.PatientId };
			}

			return null;
		}

		private PatientInformation Reconcile(PatientInformation patient, XmlDocument rulesDocument, string rulesElementName)
		{
			PatientInformation returnPatient = patient.Clone();
			if (String.IsNullOrEmpty(patient.PatientId))
				return returnPatient;

			returnPatient.PatientId = returnPatient.PatientId.Trim();

			XmlElement rulesNode = rulesDocument.SelectSingleNode("//" + rulesElementName) as XmlElement;
			if (rulesNode != null)
			{
				foreach (XmlNode ruleNode in rulesNode.SelectNodes("rule"))
				{
					XmlElement ruleElement = ruleNode as XmlElement;
					if (ruleElement != null)
					{
						if (_applicator.Apply(ruleElement, returnPatient))
							break;
					}
				}
			}

			return returnPatient;
		}
	}
}