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
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;


namespace ClearCanvas.Healthcare {

    [ExtensionOf(typeof(ProcedureStepBuilderExtensionPoint))]
    public class ModalityProcedureStepBuilder : ProcedureStepBuilderBase
    {

        public override Type ProcedureStepClass
        {
            get { return typeof(ModalityProcedureStep); }
        }

        public override ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure)
        {
            ModalityProcedureStep step = new ModalityProcedureStep();

            // set description
            step.Description = GetAttribute(xmlNode, "description", true);

            // set modality - need to look up by ID
            try
            {
                string modalityId = GetAttribute(xmlNode, "modality", true);
                ModalitySearchCriteria where = new ModalitySearchCriteria();
                where.Id.EqualTo(modalityId);

                // TODO might as well cache this query
                step.Modality = PersistenceScope.CurrentContext.GetBroker<IModalityBroker>().FindOne(where);
            }
            catch (EntityNotFoundException e)
            {
                throw new ProcedureBuilderException("Modality ID {0} is not valid.", e);
            }

            return step;
        }

        public override void SaveInstance(ProcedureStep prototype, XmlElement xmlNode)
        {
            ModalityProcedureStep step = (ModalityProcedureStep) prototype;
            xmlNode.SetAttribute("description", step.Description);
            xmlNode.SetAttribute("modality", step.Modality.Id);
        }
    }



    /// <summary>
    /// ModalityProcedureStep entity
    /// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public class ModalityProcedureStep : ProcedureStep
	{
        private string _description;
        private Modality _modality;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="description"></param>
        /// <param name="modality"></param>
        public ModalityProcedureStep(Procedure procedure, string description, Modality modality)
            :base(procedure)
        {
            _description = description;
            _modality = modality;
        }

        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public ModalityProcedureStep()
        {
        }

        public override string Name
        {
            get { return "Modality"; }
        }

		public override bool CreateInDowntimeMode
		{
			get { return true; }
		}

		public override bool IsPreStep
		{
			get { return false; }
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
			return new ModalityProcedureStep(this.Procedure, _description, _modality);
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// modality steps do not have related steps
			return false;
		}

        /// <summary>
        /// Gets or sets the description of this step (e.g. CT Chest w/o contrast).
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the modality on which this step is to be performed.
        /// </summary>
        public virtual Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

		private static IValidationRuleSet GetValidationRules()
		{
			// modalities must be associated with performing facility
			var modalityAlignsWithPerformingFacilityRule = new ValidationRule<ModalityProcedureStep>(
				OrderRules.ModalityAlignsWithPerformingFacility);

			return new ValidationRuleSet(new[]
			{
				modalityAlignsWithPerformingFacilityRule
			});
		}
	}
}