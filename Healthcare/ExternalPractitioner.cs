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
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// ExternalPractitioner entity
	/// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public partial class ExternalPractitioner
	{
		/// <summary>
		/// Creates a new practitioner that is the result of merging the two specified practitioners.
		/// </summary>
		/// <param name="right"></param>
		/// <param name="left"></param>
		/// <param name="name"></param>
		/// <param name="licenseNumber"></param>
		/// <param name="billingNumber"></param>
		/// <param name="extendedProperties"></param>
		/// <param name="defaultContactPoint"></param>
		/// <param name="deactivatedContactPoints"></param>
		/// <param name="contactPointReplacements"></param>
		/// <returns></returns>
		public static ExternalPractitioner MergePractitioners(
			ExternalPractitioner right,
			ExternalPractitioner left,
			PersonName name,
			string licenseNumber,
			string billingNumber,
			IDictionary<string, string> extendedProperties,
			ExternalPractitionerContactPoint defaultContactPoint,
			ICollection<ExternalPractitionerContactPoint> deactivatedContactPoints,
			IDictionary<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint> contactPointReplacements)
		{
			// sanity check
			if (Equals(right, left))
				throw new WorkflowException("Cannot merge a practitioner with itself.");
			if (right.Deactivated || left.Deactivated)
				throw new WorkflowException("Cannot merge a practitioner that is de-activated.");
			if (right.IsMerged || left.IsMerged)
				throw new WorkflowException("Cannot merge a practitioner that has already been merged.");
			if (defaultContactPoint != null && defaultContactPoint.IsMerged)
				throw new WorkflowException("Cannot assigned a merged contact point as default");

			// update properties on result record
			var result = new ExternalPractitioner { Name = name, LicenseNumber = licenseNumber, BillingNumber = billingNumber };

			ExtendedPropertyUtils.Update(result.ExtendedProperties, extendedProperties);

			// construct the set of retained contact points
			var retainedContactPoints = new HashedSet<ExternalPractitionerContactPoint>();
			retainedContactPoints.AddAll(contactPointReplacements.Values);

			// some of the replacement contact points are merged.  This should not be allowed.
			if (CollectionUtils.Contains(contactPointReplacements.Values, cp => cp.IsMerged))
				throw new WorkflowException("Cannot replace a contact point with another that has already been merged.");

			// add any existing contact point that was not in the replacement list (because it is implicitly being retained)
			foreach (var contactPoint in CollectionUtils.Concat(right.ContactPoints, left.ContactPoints))
			{
				// No need to retain a merged contact point.  Because its replacement would already be retained.
				if (!contactPointReplacements.ContainsKey(contactPoint) && !contactPoint.IsMerged)
					retainedContactPoints.Add(contactPoint);
			}

			// for all retained contact points, create a copy attached to the result practitioner,
			// and mark the original as having been merged into the copy
			foreach (var original in retainedContactPoints)
			{
				var copy = original.CreateCopy(result);
				result.ContactPoints.Add(copy);

				copy.IsDefaultContactPoint = original.Equals(defaultContactPoint);
				copy.MarkDeactivated(original.Deactivated || deactivatedContactPoints.Contains(original));
				original.SetMergedInto(copy);
			}

			// for all replaced contact points, mark the original as being merged into the 
			// copy of the replacement
			foreach (var kvp in contactPointReplacements)
			{
				kvp.Key.SetMergedInto(kvp.Value.MergedInto);
			}

			// mark both left and right as edited and merged
			foreach (var practitioner in new[] { right, left })
	{
				practitioner.MarkEdited();
				practitioner.SetMergedInto(result);
			}

			// mark the result as being edited
			result.MarkEdited();
			return result;
		}

		/// <summary>
		/// Gets the default contact point, or null if no default contact point exists.
		/// </summary>
		public virtual ExternalPractitionerContactPoint DefaultContactPoint
		{
			get
			{
				return CollectionUtils.SelectFirst(_contactPoints, cp => cp.IsDefaultContactPoint);
			}
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Mark the entity as being edited.  The edit time is recorded and the entity is now unverified.
		/// </summary>
		public virtual void MarkEdited()
		{
			_lastEditedTime = Platform.Time;
			_isVerified = false;
		}

		/// <summary>
		/// Mark the entity as being verified.  The verify time is recorded.
		/// </summary>
		public virtual void MarkVerified()
		{
			if (this.IsMerged)
				throw new WorkflowException("Cannot verify a merged practitioner");

			_lastVerifiedTime = Platform.Time;
			_isVerified = true;
		}

		/// <summary>
		/// Mark the entity as being deactivated.
		/// </summary>
		public virtual void MarkDeactivated(bool deactivated)
		{
			if (_deactivated == deactivated)
				return;

			// Trying to activate a merged practitioner
			if (_deactivated && this.IsMerged)
				throw new WorkflowException("Cannot activate a merged practitioner");

			_deactivated = deactivated;
		}

		/// <summary>
		/// Gets a value indicating whether this entity was merged.
		/// </summary>
		public virtual bool IsMerged
		{
			get { return _mergedInto != null; }
		}

		/// <summary>
		/// Gets the ultimate merge destination by following the merge link chain to the end.
		/// </summary>
		/// <returns></returns>
		public virtual ExternalPractitioner GetUltimateMergeDestination()
		{
			var dest = this;
			while (dest.MergedInto != null)
				dest = dest.MergedInto;

			return dest;
		}

		/// <summary>
		/// Marks this practitioner as being merged into the specified other.
		/// </summary>
		/// <param name="other"></param>
		protected internal virtual void SetMergedInto(ExternalPractitioner other)
		{
			_mergedInto = other;
			_deactivated = true;
		}

		private static IValidationRuleSet GetValidationRules()
		{
			// ensure that not both the procedure type and procedure type groups filters are being applied
			var exactlyOneDefaultContactPointRule = new ValidationRule<ExternalPractitioner>(
				delegate(ExternalPractitioner externalPractitioner)
				{
					// The rule is not applicable to deactivated external practitioner
					if (externalPractitioner.Deactivated)
						return new TestResult(true, "");

					var activeDefaultContactPoints = CollectionUtils.Select(
						externalPractitioner.ContactPoints,
						contactPoint => contactPoint.IsDefaultContactPoint && !contactPoint.Deactivated);
					var success = activeDefaultContactPoints.Count == 1;

					return new TestResult(success, SR.MessageValidateExternalPractitionerRequiresExactlyOneDefaultContactPoint);
				});

			return new ValidationRuleSet(new[] { exactlyOneDefaultContactPointRule });
		}

	}
}