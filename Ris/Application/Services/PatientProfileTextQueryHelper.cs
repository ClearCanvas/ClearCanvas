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
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientProfileTextQueryHelper : TextQueryHelper<PatientProfile, PatientProfileSearchCriteria, PatientProfileSummary>
    {
        private readonly IPersistenceContext _context;
        private readonly IPatientProfileBroker _broker;
        private readonly PatientProfileAssembler _assembler;

        public PatientProfileTextQueryHelper(IPersistenceContext context)
        {
            _context = context;
            _broker = _context.GetBroker<IPatientProfileBroker>();
            _assembler = new PatientProfileAssembler();
        }

        protected override PatientProfileSearchCriteria[] BuildCriteria(TextQueryRequest request)
        {
            string query = request.TextQuery;

            // this will hold all criteria
            var criteria = new List<PatientProfileSearchCriteria>();

            // build criteria against names
            PersonName[] names = ParsePersonNames(query);
            criteria.AddRange(CollectionUtils.Map(names,
                delegate(PersonName n)
                {
                    var sc = new PatientProfileSearchCriteria();
                    sc.Name.FamilyName.StartsWith(n.FamilyName);
                    if (n.GivenName != null)
                        sc.Name.GivenName.StartsWith(n.GivenName);
                    return sc;
                }));

            // build criteria against Mrn identifiers
            string[] ids = ParseIdentifiers(query);
            criteria.AddRange(CollectionUtils.Map(ids,
                         delegate(string word)
                         {
                             var c = new PatientProfileSearchCriteria();
                             c.Mrn.Id.StartsWith(word);
                             return c;
                         }));

            // build criteria against Healthcard identifiers
            criteria.AddRange(CollectionUtils.Map(ids,
                         delegate(string word)
                         {
                             var c = new PatientProfileSearchCriteria();
                             c.Healthcard.Id.StartsWith(word);
                             return c;
                         }));

			// sort results by patient last name (add sort directive to first instance only, otherwise we get exceptions)
        	foreach (var criterion in criteria.Take(1))
        	{
        		criterion.Name.FamilyName.SortAsc(0);
        	}


            return criteria.ToArray();
        }

        protected override bool TestSpecificity(PatientProfileSearchCriteria[] where, int threshold)
        {
            return _broker.Count(where) <= threshold;
        }

        protected override IList<PatientProfile> DoQuery(PatientProfileSearchCriteria[] where, SearchResultPage page)
        {
            return _broker.Find(where, page);
        }

        protected override PatientProfileSummary AssembleSummary(PatientProfile domainItem)
        {
            return _assembler.CreatePatientProfileSummary(domainItem, _context);
        }
    }
}
