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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Abstract base class for entity search criteria classes.
	/// </summary>
	public abstract class EntitySearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria(string key)
			: base(key)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected EntitySearchCriteria(EntitySearchCriteria other)
			: base(other)
		{
		}

		/// <summary>
		/// Gets the search condition on OID.
		/// </summary>
		public ISearchCondition<object> OID
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("OID"))
				{
					this.SubCriteria["OID"] = new SearchCondition<object>("OID");
				}
				return (ISearchCondition<object>)this.SubCriteria["OID"];
			}
		}
	}

	/// <summary>
	/// Abstract base class for entity search criteria classes.
	/// </summary>
	public abstract class EntitySearchCriteria<TEntity> : EntitySearchCriteria
		where TEntity : Entity
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria(string key)
			: base(key)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria()
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected EntitySearchCriteria(EntitySearchCriteria<TEntity> other)
			: base(other)
		{
		}

		/// <summary>
		/// Specifies that the entity is equal to the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		public void EqualTo(TEntity entity)
		{
			this.OID.EqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity is not equal to the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		public void NotEqualTo(TEntity entity)
		{
			this.OID.NotEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity is in the specified set of entities.
		/// </summary>
		/// <param name="entities"></param>
		public void In(IEnumerable<TEntity> entities)
		{
			this.OID.In(CollectionUtils.Map(entities, (TEntity item) => item.OID));
		}

		/// <summary>
		/// Specifies that the entity reference is null.
		/// </summary>
		public void IsNull()
		{
			this.OID.IsNull();
		}

		/// <summary>
		/// Specifies that the entity reference is non null.
		/// </summary>
		public void IsNotNull()
		{
			this.OID.IsNotNull();
		}

		/// <summary>
		/// Specifies that the entity be less than the specified entity (by OID).
		/// </summary>
		public void LessThan(TEntity entity)
		{
			this.OID.LessThan(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be less than or equal to the specified entity (by OID).
		/// </summary>
		public void LessThanOrEqualTo(TEntity entity)
		{
			this.OID.LessThanOrEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be more than the specified entity (by OID).
		/// </summary>
		public void MoreThan(TEntity entity)
		{
			this.OID.MoreThan(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be more than or equal to the specified entity (by OID).
		/// </summary>
		public void MoreThanOrEqualTo(TEntity entity)
		{
			this.OID.MoreThanOrEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be used to sort the results in ascending order (by OID).
		/// </summary>
		/// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
		public void SortAsc(int position)
		{
			this.OID.SortAsc(position);
		}

		/// <summary>
		/// Specifies that the entity be used to sort the results in descending order (by OID).
		/// </summary>
		/// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
		public void SortDesc(int position)
		{
			this.OID.SortDesc(position);
		}

	}
}
