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
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// PatientProfileDiffComponent class
	/// </summary>
	public class PatientProfileDiffComponent : DHtmlComponent
	{
		#region Healthcare Context

		// Internal data contract used for jscript deserialization
		[DataContract]
		public class HealthcareContext : DataContractBase
		{
			private readonly PatientProfileDiffComponent _owner;
			public HealthcareContext(PatientProfileDiffComponent owner)
			{
				_owner = owner;
			}

			[DataMember]
			public List<string> ProfileAuthorities
			{
				get { return _owner._profileAuthorities; }
			}

			[DataMember]
			public List<Field> Fields
			{
				get { return _owner._fields; }
			}
		}

		[DataContract]
		public class Field
		{
			private readonly string _heading;
			private readonly bool _isDiscrepant;
			private List<Value> _values;

			public Field(string heading, bool isDiscrepant, string leftValue, string rightValue, string diffMask)
			{
				_heading = heading;
				_isDiscrepant = isDiscrepant;
				BuildValues(leftValue, rightValue, diffMask);
			}

			private void BuildValues(string leftValue, string rightValue, string diffMask)
			{
				_values = new List<Value>();
				var leftSegments = new List<Segment>();
				var rightSegments = new List<Segment>();

				if (!_isDiscrepant)
				{
					leftSegments.Add(new Segment(leftValue, false));
					_values.Add(new Value(leftSegments));
					rightSegments.Add(new Segment(rightValue, false));
					_values.Add(new Value(rightSegments));
					return;
				}

				var maskChar = diffMask[0];
				var leftSegment = new StringBuilder();
				var rightSegment = new StringBuilder();
				for (var i = 0; i < diffMask.Length; i++)
				{
					if (!diffMask[i].Equals(maskChar))
					{
						leftSegments.Add(new Segment(leftSegment.ToString(), maskChar == ' '));
						rightSegments.Add(new Segment(rightSegment.ToString(), maskChar == ' '));
						maskChar = diffMask[i];
						leftSegment = new StringBuilder();
						rightSegment = new StringBuilder();
					}
					leftSegment.Append(leftValue[i]);
					rightSegment.Append(rightValue[i]);
				}
				leftSegments.Add(new Segment(leftSegment.ToString(), maskChar == ' '));
				rightSegments.Add(new Segment(rightSegment.ToString(), maskChar == ' '));

				_values.Add(new Value(leftSegments));
				_values.Add(new Value(rightSegments));
				return;
			}

			[DataMember]
			public string Heading
			{
				get { return _heading; }
			}

			[DataMember]
			public List<Value> Values
			{
				get { return _values; }
			}

			[DataMember]
			public bool IsDiscrepancy
			{
				get { return _isDiscrepant; }
			}
		}

		[DataContract]
		public class Value
		{
			private readonly List<Segment> _segments;

			public Value(List<Segment> segments)
			{
				_segments = segments;
			}

			[DataMember]
			public List<Segment> Segments
			{
				get { return _segments; }
			}
		}

		[DataContract]
		public class Segment
		{
			private readonly string _text;
			private readonly bool _isDiscrepant;

			public Segment(string text, bool discrepant)
			{
				_text = text;
				_isDiscrepant = discrepant;
			}

			[DataMember]
			public string Text { get { return _text; } }

			[DataMember]
			public bool IsDiscrepant { get { return _isDiscrepant; } }
		}

		#endregion

		private readonly List<Field> _fields;
		private EntityRef[] _profileRefs;
		private List<string> _profileAuthorities;

		/// <summary>
		/// Constructor
		/// </summary>
		public PatientProfileDiffComponent()
		{
			_fields = new List<Field>();
			_profileAuthorities = new List<string>();
		}

		public EntityRef[] ProfilesToCompare
		{
			get { return _profileRefs; }
			set
			{
				_profileRefs = value;

				if (this.IsStarted)
				{
					Refresh();
				}
			}
		}

		public override void Start()
		{
			Refresh();

			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return new HealthcareContext(this);
		}

		private void Refresh()
		{
			_fields.Clear();

			if (_profileRefs != null && _profileRefs.Length == 2)
			{
				Async.Request(this,
					(IPatientReconciliationService service) => service.LoadPatientProfileDiff(new LoadPatientProfileDiffRequest(_profileRefs[0], _profileRefs[1])),
					response =>
					{
						var diffData = response.ProfileDiff;

						_profileAuthorities =
							new List<string>(new[] { diffData.LeftProfileAssigningAuthority, diffData.RightProfileAssigningAuthority });

						AddField(SR.ColumnHealthcardNumber, diffData.Healthcard);
						AddField(SR.ColumnFamilyName, diffData.FamilyName);
						AddField(SR.ColumnGivenName, diffData.GivenName);
						AddField(SR.ColumnMiddleName, diffData.MiddleName);
						AddField(SR.ColumnDateOfBirth, diffData.DateOfBirth);
						AddField(SR.ColumnSex, diffData.Sex);

						AddField(SR.ColumnHomePhone, diffData.HomePhone);
						AddField(SR.ColumnWorkPhone, diffData.WorkPhone);
						AddField(SR.ColumnHomeAddress, diffData.HomeAddress);
						AddField(SR.ColumnWorkAddress, diffData.WorkAddress);

						SetUrl(WebResourcesSettings.Default.PatientReconciliationPageUrl);
						NotifyAllPropertiesChanged();
					});
			}
			SetUrl(WebResourcesSettings.Default.PatientReconciliationPageUrl);
			NotifyAllPropertiesChanged();
		}

		private void AddField(string heading, PropertyDiff propertyDiff)
		{
			_fields.Add(new Field(heading, propertyDiff.IsDiscrepant, propertyDiff.AlignedLeftValue, propertyDiff.AlignedRightValue, propertyDiff.DiffMask));
		}
	}
}
