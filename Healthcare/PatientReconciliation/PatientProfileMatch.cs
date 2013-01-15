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

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    public class PatientProfileMatch
    {
        public enum ScoreValue
        {
            High = 2,
            Moderate = 1,
            Low = 0
        }

        private PatientProfile _patientProfile;
        private ScoreValue _score;

        public PatientProfileMatch(PatientProfile patientProfile, ScoreValue score)
        {
            _patientProfile = patientProfile;
            _score = score;
        }

        public PatientProfile PatientProfile
        {
            get { return _patientProfile; }
        }
	
        public ScoreValue Score
        {
            get { return _score; }
            private set { _score = value; }
        }


        public static IList<PatientProfileMatch> CreateList(PatientProfile self, IList<PatientProfile> profileList, ScoreValue score)
        {
            IList<PatientProfileMatch> matchList = new List<PatientProfileMatch>();
            foreach (PatientProfile profile in profileList)
            {
                bool found = false;
                foreach (PatientProfile existing in self.Patient.Profiles)
                {
                    if (profile.Equals(existing))
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    matchList.Add(new PatientProfileMatch(profile, score));
                }
            }
            return matchList;
        }

        public static IList<PatientProfileMatch> Combine(IList<PatientProfileMatch> leftSet, IList<PatientProfileMatch> rightSet)
        {
            IList<PatientProfileMatch> result = new List<PatientProfileMatch>();

            foreach (PatientProfileMatch left in leftSet)
            {
                result.Add(left);
            }
            foreach (PatientProfileMatch right in rightSet)
            {
                bool found = false;
                foreach (PatientProfileMatch alreadyAdded in result)
                {
                    if (right.PatientProfile.Equals(alreadyAdded.PatientProfile))
                    {
                        found = true;
                        if (right.Score > alreadyAdded.Score)
                        {
                            alreadyAdded.Score = right.Score;
                        }
                        break;
                    }
                }
                if (found == false)
                {
                    result.Add(right);
                }
            }

            return result;
        }
    }
}
