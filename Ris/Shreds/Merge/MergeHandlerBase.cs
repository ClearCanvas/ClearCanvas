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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Abstract base implementation of <see cref="IMergeHandler"/>.
	/// </summary>
	/// <typeparam name="TTarget"></typeparam>
	public abstract class MergeHandlerBase<TTarget> : IMergeHandler
		where TTarget : Entity
	{
		protected delegate void Action<T1, T2>(T1 x, T2 y);
		protected delegate int MergeStep(TTarget target, int stage, IPersistenceContext context);
		protected delegate IList<TItem> BatchProvider<TItem, TItemCriteria>(TTarget practitioner, Action<TItemCriteria> priorityFilter, int batchSize, IPersistenceContext context);

		private readonly int _itemsPerTransaction;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="itemsPerTransaction"></param>
		protected MergeHandlerBase(int itemsPerTransaction)
		{
			_itemsPerTransaction = itemsPerTransaction;
		}


		#region IMergeHandler members

		/// <summary>
		/// Gets a value indicating whether this handler supports merging of the specified target.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool SupportsTarget(Entity target)
		{
			return target.Is<TTarget>();
		}

		/// <summary>
		/// Asks this handler to perform part of the merge operation, beginning at the specified stage.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="stage"></param>
		/// <param name="context"></param>
		/// <returns>The stage at which the merge operation should continue next.</returns>
		public int Merge(Entity target, int stage, IPersistenceContext context)
		{
			var steps = this.MergeSteps;

			if (stage < 0 || stage >= steps.Length)
				throw new InvalidOperationException("Invalid stage.");

			var step = steps[stage];
			return step((TTarget)target, stage, context);
		}

		#endregion

		#region Protected API
		
		/// <summary>
		/// Gets the set of merge steps to be performed.
		/// </summary>
		/// <remarks>
		/// Defines a set of migration steps to be executed. The first step in the list is always executed first.
		/// The execution of each step returns an integer indicating which step to execute next.
		/// </remarks>
		protected abstract MergeStep[] MergeSteps { get; }

		/// <summary>
		/// Helper method for defining a <see cref="MergeStep"/> to migrate a batch of items.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TItemCriteria"></typeparam>
		/// <param name="target"></param>
		/// <param name="stage"></param>
		/// <param name="batchProvider"></param>
		/// <param name="priorityFilter"></param>
		/// <param name="processAction"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected int Migrate<TItem, TItemCriteria>(
			TTarget target,
			int stage,
			BatchProvider<TItem, TItemCriteria> batchProvider,
			Action<TItemCriteria> priorityFilter,
			Action<TTarget, TItem> processAction,
			IPersistenceContext context)
		{
			// get batch
			var batch = batchProvider(target, priorityFilter, _itemsPerTransaction, context);
			
			// process items
			foreach (var item in batch)
			{
				processAction(target, item);
			}

			// if any items were processed in this batch, there may be more items
			// so remain at the same stage
			// if no items were processed, advance to the next stage
			return (batch.Count > 0) ? stage : stage + 1;
		}

		#endregion
	}
}
