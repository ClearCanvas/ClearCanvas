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
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Healthcare {
    
    public class PatientClassConversionException : Exception
    {
    }

    /// <summary>
    /// Visit entity
    /// </summary>
    [UniqueKey("VisitNumber", new string[] {"VisitNumber.Id", "VisitNumber.AssigningAuthority"})]
	public partial class Visit : Entity
	{
        private void CustomInitialize()
        {
        }

		/// <summary>
		/// Gets the patient profile associated with this visit.
		/// </summary>
		public virtual PatientProfile PatientProfile
		{
			get
			{
				return CollectionUtils.SelectFirst(_patient.Profiles,
					profile => Equals(profile.Mrn.AssigningAuthority, _visitNumber.AssigningAuthority));
			}
		}

		public virtual void CopyFrom(Visit v)
		{
			this.VisitNumber.Id = v.VisitNumber.Id;
			this.VisitNumber.AssigningAuthority = v.VisitNumber.AssigningAuthority;
			this.Status = v.Status;
			this.AdmitTime = v.AdmitTime;
			this.PatientClass = v.PatientClass;
			this.PatientType = v.PatientType;
			this.AdmissionType = v.AdmissionType;
			this.Facility = v.Facility;
			this.DischargeTime = v.DischargeTime;
			this.DischargeDisposition = v.DischargeDisposition;
			this.VipIndicator = v.VipIndicator;
			//this.AmbulatoryStatus = v.AmbulatoryStatus;
			this.PreadmitNumber = v.PreadmitNumber;

			foreach (AmbulatoryStatusEnum a in v.AmbulatoryStatuses)
			{
				this.AmbulatoryStatuses.Add(a);
			}

			foreach (VisitPractitioner vp in v.Practitioners)
			{
				VisitPractitioner practitioner = new VisitPractitioner();
				practitioner.CopyFrom(vp);
				this.Practitioners.Add(practitioner);
			}

			foreach (VisitLocation vl in v.Locations)
			{
				VisitLocation location = new VisitLocation();
				location.CopyFrom(vl);
				this.Locations.Add(location);
			}

			ExtendedPropertyUtils.Update(this.ExtendedProperties, v.ExtendedProperties);
		}

        public virtual void Cancel()
        {
            this.Status = VisitStatus.CX;
        }

        public virtual void CancelPreAdmit()
        {
            this.Status = VisitStatus.PC;
        }

        /// <summary>
        /// Infers VisitStatus from AdmitTime and DischargeTime.  
        /// </summary>
        public virtual void InferVisitStatus()
        {
            if (this.AdmitTime.HasValue) this.Status = VisitStatus.AA;
            if (this.DischargeTime.HasValue) this.Status = VisitStatus.DC;
        }

        public virtual void Discharge(DateTime dischargeDateTime, string dischargeDispostion)
        {
            if (this.Status != VisitStatus.DC && this.Status != VisitStatus.CX)
            {
                this.Status = VisitStatus.DC;
                this.DischargeTime = dischargeDateTime;
                this.DischargeDisposition = dischargeDispostion;
            }
        }

        public virtual void PopCurrentLocation()
        {
            DateTime previousCurrentDate = DateTime.MinValue;
            VisitLocation current = null;
            VisitLocation previousCurrent = null;
            
            foreach (VisitLocation vl in this.Locations)
            {
                if (vl.Role == VisitLocationRole.CR)
                {
                    if (vl.EndTime == null)
                    {
                        current = vl;
                    }
                    else if (vl.EndTime > previousCurrentDate)
                    {
                        previousCurrent = vl;
                    }
                }
            }

            if (current != null && previousCurrent != null)
            {
                this.Locations.Remove(current);
                previousCurrent.EndTime = null;
            }
            else
            {
            }
        }

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_admitTime = _admitTime.HasValue ? _admitTime.Value.AddMinutes(minutes) : _admitTime;
			_dischargeTime = _dischargeTime.HasValue ? _dischargeTime.Value.AddMinutes(minutes) : _dischargeTime;

			foreach (VisitPractitioner prac in _practitioners)
			{
				prac.StartTime = prac.StartTime.HasValue ? prac.StartTime.Value.AddMinutes(minutes) : prac.StartTime;
				prac.EndTime = prac.EndTime.HasValue ? prac.EndTime.Value.AddMinutes(minutes) : prac.EndTime;
			}

			foreach (VisitLocation loc in _locations)
			{
				loc.StartTime = loc.StartTime.HasValue ? loc.StartTime.Value.AddMinutes(minutes) : loc.StartTime;
				loc.EndTime = loc.EndTime.HasValue ? loc.EndTime.Value.AddMinutes(minutes) : loc.EndTime;
				
			}
		}
    }
}
