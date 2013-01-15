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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// ExternalPractitionerContactPoint entity
	/// </summary>
	public partial class ExternalPractitionerContactPoint
	{
		private delegate TOutput Converter<TInput1, TInput2, TOutput>(TInput1 input1, TInput2 input2);

		/// <summary>
		/// Creates a new contact point that is the result of merging the two specified contact points.
		/// </summary>
		/// <remarks>
		/// Any phone numbers, addresses and email addresses in the supplied arguments will supercede those
		/// from the source contact points.
		/// </remarks> 
		public static ExternalPractitionerContactPoint MergeContactPoints(
			ExternalPractitionerContactPoint right,
			ExternalPractitionerContactPoint left,
			string name,
			string description,
			ResultCommunicationMode preferredCommunicationMode,
			InformationAuthorityEnum informationAuthority,
			IList<TelephoneNumber> phoneNumbers,
			IList<Address> addresses,
			IList<EmailAddress> emailAddresses
			)
		{
			// sanity checks
			if (Equals(right, left))
				throw new WorkflowException("Cannot merge a contact point with itself.");
			if (!Equals(right.Practitioner, left.Practitioner))
				throw new WorkflowException("Only contact points belonging to the same practitioner can be merged.");
			if (right.Deactivated || left.Deactivated)
				throw new WorkflowException("Cannot merge a contact point that is de-activated.");
			if (right.IsMerged || left.IsMerged)
				throw new WorkflowException("Cannot merge a contact point that has already been merged.");

			var practitioner = right.Practitioner; // same practitioner for both

			// create new contact point, using specified name and description
			// the new contact point is default if either of the source contact points were the default
			// start with empty value collections
			var result = new ExternalPractitionerContactPoint(
				practitioner,
				name,
				description,
				preferredCommunicationMode,
				informationAuthority,
				right.IsDefaultContactPoint || left.IsDefaultContactPoint,
				phoneNumbers ?? new List<TelephoneNumber>(),
				addresses ?? new List<Address>(),
				emailAddresses ?? new List<EmailAddress>(),
				null);

			practitioner.ContactPoints.Add(result);

			// merge value collections from source contact points, and update source contact points appropriately
			foreach (var contactPoint in new[] { right, left })
			{
				// merge all value collections into the result
				MergeValueCollection(result.TelephoneNumbers, contactPoint.TelephoneNumbers, (x, y) => x.IsSameNumber(y));
				MergeValueCollection(result.Addresses, contactPoint.Addresses, (x, y) => x.IsSameAddress(y));
				MergeValueCollection(result.EmailAddresses, contactPoint.EmailAddresses, (x, y) => x.IsSameEmailAddress(y));

				// mark as merged
				contactPoint.SetMergedInto(result);
			}

			// mark the practitioner as being edited
			practitioner.MarkEdited();

			return result;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="practitioner"></param>
		public ExternalPractitionerContactPoint(ExternalPractitioner practitioner)
		{
			_practitioner = practitioner;
			_practitioner.ContactPoints.Add(this);

			_telephoneNumbers = new List<TelephoneNumber>();
			_addresses = new List<Address>();
			_emailAddresses = new List<EmailAddress>();
		}

		/// <summary>
		/// Gets the current work address, or null if there is no current work address.
		/// </summary>
		public virtual Address CurrentAddress
		{
			get
			{
				return CollectionUtils.SelectFirst(this.Addresses, address => address.Type == AddressType.B && address.IsCurrent);
			}
		}

		/// <summary>
		/// Gets the current work fax number, or null if there is no current work fax number.
		/// </summary>
		public virtual TelephoneNumber CurrentFaxNumber
		{
			get
			{
				return CollectionUtils.SelectFirst(this.TelephoneNumbers, f => f.Use == TelephoneUse.WPN && f.Equipment == TelephoneEquipment.FX && f.IsCurrent);
			}
		}

		/// <summary>
		/// Gets the current work phone number, or null if there is no current work phone number.
		/// </summary>
		public virtual TelephoneNumber CurrentPhoneNumber
		{
			get
			{
				return CollectionUtils.SelectFirst(this.TelephoneNumbers, p => p.Use == TelephoneUse.WPN && p.Equipment == TelephoneEquipment.PH && p.IsCurrent);
			}
		}

		/// <summary>
		/// Gets the current email address, or null if there is no email address.
		/// </summary>
		public virtual EmailAddress CurrentEmailAddress
		{
			get
			{
				return CollectionUtils.SelectFirst(this.EmailAddresses, emailAddress => emailAddress.IsCurrent);
			}
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
		public virtual ExternalPractitionerContactPoint GetUltimateMergeDestination()
		{
			var dest = this;
			while (dest.MergedInto != null)
				dest = dest.MergedInto;

			return dest;
		}

		/// <summary>
		/// Creates a copy of this contact point, assigning it to the specified owner.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public virtual ExternalPractitionerContactPoint CreateCopy(ExternalPractitioner owner)
		{
			var copy = new ExternalPractitionerContactPoint(
				owner,
				_name,
				_description,
				_preferredResultCommunicationMode,
				_informationAuthority,
				_isDefaultContactPoint,
				CollectionUtils.Map(_telephoneNumbers, (TelephoneNumber tn) => (TelephoneNumber)tn.Clone()),
				CollectionUtils.Map(_addresses, (Address a) => (Address) a.Clone()),
				CollectionUtils.Map(_emailAddresses, (EmailAddress e) => (EmailAddress) e.Clone()),
				null);
			
			copy.MarkDeactivated(_deactivated);

			owner.ContactPoints.Add(copy);
			return copy;
		}

		/// <summary>
		/// Marks this contact point as being merged into the specified other.
		/// </summary>
		/// <param name="other"></param>
		protected internal virtual void SetMergedInto(ExternalPractitionerContactPoint other)
		{
			_mergedInto = other;
			_deactivated = true;
			_isDefaultContactPoint = false;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
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
				throw new WorkflowException("Cannot activate a merged contact point");

			_deactivated = deactivated;
		}

		/// <summary>
		/// Copies items in the source collection into the dest collection, if the item does not already exist in the dest collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dest"></param>
		/// <param name="src"></param>
		/// <param name="isSameFunction"></param>
		private static void MergeValueCollection<T>(ICollection<T> dest, IEnumerable<T> src, Converter<T, T, bool> isSameFunction)
			where T : ICloneable
		{
			foreach (var srcItem in src)
			{
				if (CollectionUtils.Contains(dest, destItem => isSameFunction(destItem, srcItem)))
					continue;

				var copy = (T)srcItem.Clone();
				dest.Add(copy);
			}
		}

	}
}