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
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Provides a set of methods for performing functional-style operations on collections.
	/// </summary>
	public static class CollectionUtils
	{
		/// <summary>
		/// Delegate for use with <see cref="CollectionUtils.Reduce{TItem,TMemo}"/>.
		/// </summary>
		public delegate M ReduceDelegate<T, M>(T item, M memo);

		/// <summary>
		/// Selects all items in the target collection that match the specified predicate, returning
		/// them as a new collection of the specified type.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <typeparam name="TResultCollection">The type of collection to return.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static TResultCollection Select<TItem, TResultCollection>(IEnumerable target, Predicate<TItem> predicate)
			where TResultCollection : ICollection<TItem>, new()
		{
			var result = new TResultCollection();
			foreach (TItem item in target)
			{
				if (predicate(item))
				{
					result.Add(item);
				}
			}
			return result;
		}

		/// <summary>
		/// Selects all items in the target collection that match the specified predicate.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static List<TItem> Select<TItem>(IEnumerable target, Predicate<TItem> predicate)
		{
			var result = new List<TItem>();
			foreach (TItem item in target)
			{
				if (predicate(item))
				{
					result.Add(item);
				}
			}
			return result;
		}

		/// <summary>
		/// Selects all items in the target collection that match the specified predicate.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static List<TItem> Select<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
		{
			return Select((IEnumerable) target, predicate);
		}

		/// <summary>
		/// Selects all items in the target collection that match the specified predicate.
		/// </summary>
		/// <remarks>
		/// This overload accepts an untyped collection, and returns an untyped collection.
		/// </remarks>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static ArrayList Select(IEnumerable target, Predicate<object> predicate)
		{
			var result = new ArrayList();
			foreach (var item in target)
			{
				if (predicate(item))
				{
					result.Add(item);
				}
			}
			return result;
		}

		/// <summary>
		/// Excludes all items in the target collection that match the specified predicate, returning
		/// the rest of the items as a new collection of the specified type.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <typeparam name="TResultCollection">The type of collection to return.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static TResultCollection Reject<TItem, TResultCollection>(IEnumerable target, Predicate<TItem> predicate)
			where TResultCollection : ICollection<TItem>, new()
		{
			return Select<TItem, TResultCollection>(target, item => !predicate(item));
		}

		/// <summary>
		/// Excludes all items in the target collection that match the specified predicate, returning
		/// the rest of the items as a new collection.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static List<TItem> Reject<TItem>(IEnumerable target, Predicate<TItem> predicate)
		{
			return Select(target, (TItem item) => !predicate(item));
		}

		/// <summary>
		/// Excludes all items in the target collection that match the specified predicate, returning
		/// the rest of the items as a new collection.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>A collection containing the subset of matching items from the target collection.</returns>
		public static List<TItem> Reject<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
		{
			return Reject((IEnumerable) target, predicate);
		}

		/// <summary>
		/// Excludes all items in the target collection that match the specified predicate, returning
		/// the rest of the items as a new collection.
		/// </summary>
		/// <remarks>
		/// This overload accepts an untyped collection and returns an untyped collection.
		/// </remarks>
		/// <param name="target">The collection to operate on</param>
		/// <param name="predicate">The predicate to test</param>
		/// <returns>A collection containing the subset of matching items from the target collection</returns>
		public static ArrayList Reject(IEnumerable target, Predicate<object> predicate)
		{
			return Select(target, item => !predicate(item));
		}

		/// <summary>
		/// Returns the first item in the target collection that matches the specified predicate, or
		/// null if no match is found.
		/// </summary>
		/// <remarks>
		/// <typeparamref name="TItem"/> must be a reference type, not a value type.
		/// </remarks>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>The first matching item, or null if no match are found.</returns>
		public static TItem SelectFirst<TItem>(IEnumerable target, Predicate<TItem> predicate)
			where TItem : class
		{
			foreach (TItem item in target)
			{
				if (predicate(item))
				{
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the first item in the target collection that matches the specified predicate, or
		/// null if no match is found.
		/// </summary>
		/// <remarks>
		/// <typeparamref name="TItem"/> must be a reference type, not a value type.
		/// </remarks>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>The first matching item, or null if no match are found.</returns>
		public static TItem SelectFirst<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
			where TItem : class
		{
			return SelectFirst((IEnumerable) target, predicate);
		}

		/// <summary>
		/// Returns the first item in the target collection that matches the specified predicate, or
		/// null if no match is found.
		/// </summary>
		/// <remarks>
		/// This overload accepts an untyped collection.
		/// </remarks>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="predicate">The predicate to test.</param>
		/// <returns>The first matching item, or null if no matches are found.</returns>
		public static object SelectFirst(IEnumerable target, Predicate<object> predicate)
		{
			foreach (var item in target)
			{
				if (predicate(item))
				{
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// Maps the specified collection onto a new collection according to the specified map function.
		/// </summary>
		/// <remarks>
		/// Allows the type of the return collection to be specified.
		/// </remarks>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <typeparam name="TResultItem">The type of item returned by the map function.</typeparam>
		/// <typeparam name="TResultCollection">The type of collection to return.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="mapFunction">A delegate that performs the mapping.</param>
		/// <returns>A new collection of the specified type, containing a mapped entry for each entry in the target collection.</returns>
		public static TResultCollection Map<TItem, TResultItem, TResultCollection>(IEnumerable target, Converter<TItem, TResultItem> mapFunction)
			where TResultCollection : ICollection<TResultItem>, new()
		{
			var result = new TResultCollection();
			foreach (TItem item in target)
			{
				result.Add(mapFunction(item));
			}
			return result;
		}

		/// <summary>
		/// Maps the specified collection onto a new collection according to the specified map function.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <typeparam name="TResultItem">The type of item returned by the map function.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="mapFunction">A delegate that performs the mapping.</param>
		/// <returns>A new collection containing a mapped entry for each entry in the target collection.</returns>
		public static List<TResultItem> Map<TItem, TResultItem>(IEnumerable target, Converter<TItem, TResultItem> mapFunction)
		{
			var result = new List<TResultItem>();
			foreach (TItem item in target)
			{
				result.Add(mapFunction(item));
			}
			return result;
		}

		/// <summary>
		/// Maps the specified collection onto a new collection according to the specified map function.
		/// </summary>
		/// <remarks>
		/// This overload operates on an untyped collection and returns an untyped collection.
		/// </remarks>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="mapFunction">A delegate that performs the mapping.</param>
		/// <returns>A new collection containing a mapped entry for each entry in the target collection.</returns>
		public static ArrayList Map(IEnumerable target, Converter<object, object> mapFunction)
		{
			var result = new ArrayList();
			foreach (var item in target)
			{
				result.Add(mapFunction(item));
			}
			return result;
		}

		/// <summary>
		/// Maps the specified dictionary onto a new dictionary according to the specified map function.
		/// </summary>
		/// <typeparam name="K">Key type of target dictionary.</typeparam>
		/// <typeparam name="V">Value type of target dictionary.</typeparam>
		/// <typeparam name="K2">Key type of result dictionary.</typeparam>
		/// <typeparam name="V2">Value type of result dictionary.</typeparam>
		/// <param name="target"></param>
		/// <param name="mapFunc"></param>
		/// <returns>A new dictionary containing a mapped entry for each entry in the target collection.</returns>
		public static Dictionary<K2, V2> Map<K, V, K2, V2>(IDictionary<K, V> target, Converter<KeyValuePair<K, V>, KeyValuePair<K2, V2>> mapFunc)
		{
			var result = new Dictionary<K2, V2>();
			foreach (var kvp in target)
			{
				var kvp2 = mapFunc(kvp);
				result.Add(kvp2.Key, kvp2.Value);
			}
			return result;
		}

		/// <summary>
		/// Reduces the specified collection to a singular value according to the specified reduce function.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <typeparam name="TMemo">The type of the singular value to reduce the collection to.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="initial">The initial value for the reduce operation.</param>
		/// <param name="reduceFunction">A delegate that performs the reduce operation.</param>
		/// <returns>The value of the reduce operation.</returns>
		public static TMemo Reduce<TItem, TMemo>(IEnumerable target, TMemo initial, ReduceDelegate<TItem, TMemo> reduceFunction)
		{
			var memo = initial;
			foreach (TItem item in target)
			{
				memo = reduceFunction(item, memo);
			}
			return memo;
		}

		/// <summary>
		/// Performs the specified action for each item in the target collection.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEach<TItem>(IEnumerable target, Action<TItem> action)
		{
			foreach (TItem item in target)
			{
				action(item);
			}
		}

		/// <summary>
		/// Performs the specified action for each item in the target collection.
		/// </summary>
		/// <typeparam name="TItem">The type of items in the target collection.</typeparam>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEach<TItem>(IEnumerable<TItem> target, Action<TItem> action)
		{
			ForEach((IEnumerable) target, action);
		}

		/// <summary>
		/// Performs the specified action for each item in the target collection.
		/// </summary>
		/// <remarks>
		/// This overload operates on an untyped collection.
		/// </remarks>
		/// <param name="target">The collection to operate on.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEach(IEnumerable target, Action<object> action)
		{
			foreach (var item in target)
			{
				action(item);
			}
		}

		/// <summary>
		/// Returns true if any item in the target collection satisfies the specified predicate.
		/// </summary>
		public static bool Contains<TItem>(IEnumerable target, Predicate<TItem> predicate)
		{
			foreach (TItem item in target)
			{
				if (predicate(item))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns true if any item in the target collection satisfies the specified predicate.
		/// </summary>
		public static bool Contains<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
		{
			return Contains((IEnumerable) target, predicate);
		}

		/// <summary>
		/// Returns true if any item in the target collection satisfies the specified predicate.
		/// </summary>
		public static bool Contains(IEnumerable target, Predicate<object> predicate)
		{
			foreach (var item in target)
			{
				if (predicate(item))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns true if all items in the target collection satisfy the specified predicate.
		/// </summary>
		public static bool TrueForAll<TItem>(IEnumerable target, Predicate<TItem> predicate)
		{
			foreach (TItem item in target)
			{
				if (!predicate(item))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Returns true if all items in the target collection satisfy the specified predicate.
		/// </summary>
		public static bool TrueForAll<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
		{
			return TrueForAll((IEnumerable) target, predicate);
		}

		/// <summary>
		/// Returns true if all items in the target collection satisfy the specified predicate.
		/// </summary>
		public static bool TrueForAll(IEnumerable target, Predicate<object> predicate)
		{
			foreach (var item in target)
			{
				if (!predicate(item))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the first element in the target collection, or null if the collection is empty.
		/// </summary>
		public static object FirstElement(IEnumerable target)
		{
			if (target is IList)
			{
				var list = (IList) target;
				return list.Count > 0 ? list[0] : null;
			}
			else
			{
				var e = target.GetEnumerator();
				return e.MoveNext() ? e.Current : null;
			}
		}

		/// <summary>
		/// Returns the first element in the target collection, or the specified <paramref name="defaultValue"/> if the collection is empty.
		/// </summary>
		public static TItem FirstElement<TItem>(IEnumerable target, TItem defaultValue)
		{
			var value = FirstElement(target);
			return value != null ? (TItem) value : defaultValue;
		}

		/// <summary>
		/// Returns the first element in the target collection, or null if the collection is empty.
		/// </summary>
		/// <remarks>
		/// TItem must be a reference type, not a value type.
		/// </remarks>
		public static TItem FirstElement<TItem>(IEnumerable target)
			where TItem : class
		{
			return FirstElement<TItem>(target, null);
		}

		/// <summary>
		/// Returns the first element in the target collection, or null if the collection is empty.
		/// </summary>
		/// <remarks>
		/// TItem must be a reference type, not a value type.
		/// </remarks>
		public static TItem FirstElement<TItem>(IEnumerable<TItem> target)
			where TItem : class
		{
			return FirstElement<TItem>(target, null);
		}

		/// <summary>
		/// Returns the last element in the target collection, or null if the collection is empty.
		/// </summary>
		public static object LastElement(IEnumerable target)
		{
			if (target is IList)
			{
				var list = (IList) target;
				return list.Count > 0 ? list[list.Count - 1] : null;
			}
			else
			{
				object element = null;
				var e = target.GetEnumerator();
				while (e.MoveNext())
					element = e.Current;
				return element;
			}
		}

		/// <summary>
		/// Returns the last element in the target collection, or the specified <paramref name="defaultValue "/> if the collection is empty.
		/// </summary>
		public static TItem LastElement<TItem>(IEnumerable target, TItem defaultValue)
		{
			var value = LastElement(target);
			return value != null ? (TItem) value : defaultValue;
		}

		/// <summary>
		/// Returns the last element in the target collection, or null if the collection is empty.
		/// </summary>
		///<remarks>
		/// TItem must be a reference type, not a value type.
		/// </remarks>
		public static TItem LastElement<TItem>(IEnumerable target)
			where TItem : class
		{
			return LastElement<TItem>(target, null);
		}

		/// <summary>
		/// Returns the last element in the target collection, or null if the collection is empty.
		/// </summary>
		///<remarks>
		/// TItem must be a reference type, not a value type.
		/// </remarks>
		public static TItem LastElement<TItem>(IEnumerable<TItem> target)
			where TItem : class
		{
			return LastElement<TItem>(target, null);
		}

		/// <summary>
		/// Removes all items in the target collection that match the specified predicate.
		/// </summary>
		/// <remarks>
		/// Unlike <see cref="Reject"/>, this method modifies the target collection itself.
		/// </remarks>
		public static void Remove<TItem>(ICollection<TItem> target, Predicate<TItem> predicate)
		{
			var removes = new List<TItem>();
			foreach (var item in target)
			{
				if (predicate(item))
					removes.Add(item);
			}
			foreach (var item in removes)
			{
				target.Remove(item);
			}
		}

		/// <summary>
		/// Removes all items in the target collection that match the specified predicate.
		/// </summary>
		/// <remarks>
		/// Unlike <see cref="Reject"/>, this method modifies the target collection itself.
		/// </remarks>
		public static void Remove(IList target, Predicate<object> predicate)
		{
			var removes = new List<object>();
			foreach (var item in target)
			{
				if (predicate(item))
					removes.Add(item);
			}
			foreach (var item in removes)
			{
				target.Remove(item);
			}
		}

		/// <summary>
		/// Returns a list of the items in the target collection, sorted according to the specified comparison.
		/// </summary>
		/// <remarks>
		/// Does not modify the target collection, since it may not even be a sortable collection.
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static List<TItem> Sort<TItem>(IEnumerable target, Comparison<TItem> comparison)
		{
			var list = new List<TItem>(new TypeSafeEnumerableWrapper<TItem>(target));
			list.Sort(comparison);
			return list;
		}

		/// <summary>
		/// Returns a list of the items in the target collection, sorted using the default comparer.
		/// </summary>
		/// <remarks>
		/// Does not modify the target collection, since it may not even be a sortable collection.
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static List<TItem> Sort<TItem>(IEnumerable target)
		{
			var list = new List<TItem>(new TypeSafeEnumerableWrapper<TItem>(target));
			list.Sort();
			return list;
		}

		/// <summary>
		/// Returns a list of the items in the target collection, sorted according to the specified comparison.
		/// </summary>
		/// <remarks>
		/// Does not modify the target collection, since it may not even be a sortable collection.
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static List<TItem> Sort<TItem>(IEnumerable<TItem> target, Comparison<TItem> comparison)
		{
			return Sort((IEnumerable) target, comparison);
		}

		/// <summary>
		/// Returns a list of the items in the target collection, sorted using the default comparer.
		/// </summary>
		/// <remarks>
		/// Does not modify the target collection, since it may not even be a sortable collection.
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static List<TItem> Sort<TItem>(IEnumerable<TItem> target)
		{
			var list = new List<TItem>(target);
			list.Sort();
			return list;
		}

		/// <summary>
		/// Converts the target enumerable to an array of the specified type.
		/// </summary>
		public static TItem[] ToArray<TItem>(IEnumerable target)
		{
			// optimize if collection
			if (target is ICollection<TItem>)
			{
				var c = (ICollection<TItem>) target;
				var arr = new TItem[c.Count];
				c.CopyTo(arr, 0);
				return arr;
			}
			else
			{
				var list = new List<TItem>(new TypeSafeEnumerableWrapper<TItem>(target));
				return list.ToArray();
			}
		}

		/// <summary>
		/// Converts the target enumerable to an array of the specified type.
		/// </summary>
		public static TItem[] ToArray<TItem>(IEnumerable<TItem> target)
		{
			// optimize if collection
			if (target is ICollection<TItem>)
			{
				var c = (ICollection<TItem>) target;
				var arr = new TItem[c.Count];
				c.CopyTo(arr, 0);
				return arr;
			}
			else
			{
				var list = new List<TItem>(target);
				return list.ToArray();
			}
		}

		/// <summary>
		/// Returns the minimum value in the target collection, or the specified <paramref name="nullValue "/> if the target is empty.
		/// </summary>
		/// <remarks>
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static TItem Min<TItem>(IEnumerable target, TItem nullValue, Comparison<TItem> comparison)
		{
			return FindExtremeValue(target, nullValue, comparison, -1);
		}

		/// <summary>
		/// Returns the minimum value in the target collection, or the specified <paramref name="nullValue "/> if the target is empty.
		/// </summary>
		/// <remarks>
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static TItem Min<TItem>(IEnumerable<TItem> target, TItem nullValue, Comparison<TItem> comparison)
		{
			return FindExtremeValue(target, nullValue, comparison, -1);
		}

		/// <summary>
		/// Returns the minimum value in the target collection, or the specified <paramref name="nullValue "/> if the target is empty.
		/// </summary>
		/// <remarks>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Min<TItem>(IEnumerable target, TItem nullValue)
		{
			return Min(target, nullValue, Comparer<TItem>.Default.Compare);
		}

		/// <summary>
		/// Returns the minimum value in the target collection, or the specified <paramref name="nullValue "/> if the target is empty.
		/// </summary>
		/// <remarks>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Min<TItem>(IEnumerable<TItem> target, TItem nullValue)
		{
			return Min(target, nullValue, Comparer<TItem>.Default.Compare);
		}

		/// <summary>
		/// Returns the minimum value in the target collection, or null if the collection is empty.
		/// </summary>
		/// <remarks>
		/// <para>The collection must contain object references, not value types.</para>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Min<TItem>(IEnumerable target)
			where TItem : class, IComparable<TItem>
		{
			return Min<TItem>(target, null);
		}

		/// <summary>
		/// Returns the minimum value in the target collection, or null if the collection is empty.
		/// </summary>
		/// <remarks>
		/// <para>The collection must contain object references, not value types.</para>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Min<TItem>(IEnumerable<TItem> target)
			where TItem : class, IComparable<TItem>
		{
			return Min(target, null);
		}

		/// <summary>
		/// Returns the maximum value in the target collection, or the specified <paramref name="nullValue"/> if the collection is empty.
		/// </summary>
		/// <remarks>
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static TItem Max<TItem>(IEnumerable target, TItem nullValue, Comparison<TItem> comparison)
		{
			return FindExtremeValue(target, nullValue, comparison, 1);
		}

		/// <summary>
		/// Returns the maximum value in the target collection, or the specified <paramref name="nullValue"/> if the collection is empty.
		/// </summary>
		/// <remarks>
		/// If the collection may contain nulls, the comparison must handle nulls.
		/// </remarks>
		public static TItem Max<TItem>(IEnumerable<TItem> target, TItem nullValue, Comparison<TItem> comparison)
		{
			return FindExtremeValue(target, nullValue, comparison, 1);
		}

		/// <summary>
		/// Returns the maximum value in the target collection, or the specified <paramref name="nullValue"/> if the collection is empty.
		/// </summary>
		/// <remarks>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Max<TItem>(IEnumerable target, TItem nullValue)
		{
			return Max(target, nullValue, Comparer<TItem>.Default.Compare);
		}

		/// <summary>
		/// Returns the maximum value in the target collection, or the specified <paramref name="nullValue"/> if the collection is empty.
		/// </summary>
		/// <remarks>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Max<TItem>(IEnumerable<TItem> target, TItem nullValue)
		{
			return Max(target, nullValue, Comparer<TItem>.Default.Compare);
		}

		/// <summary>
		/// Returns the maximum value in the target collection, or the null if the collection is empty.
		/// </summary>
		/// <remarks>
		/// <para>The collection must contain object references, not value types.</para>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Max<TItem>(IEnumerable target)
			where TItem : class, IComparable<TItem>
		{
			return Max<TItem>(target, null);
		}

		/// <summary>
		/// Returns the maximum value in the target collection, or the null if the collection is empty.
		/// </summary>
		/// <remarks>
		/// <para>The collection must contain object references, not value types.</para>
		/// <para>If the collection contains nulls, they are treated as less than any other value.</para>
		/// </remarks>
		public static TItem Max<TItem>(IEnumerable<TItem> target)
			where TItem : class, IComparable<TItem>
		{
			return Max(target, null);
		}

		/// <summary>
		/// Helper method to provide implementation of <b>Min</b> and <b>Max</b>.
		/// </summary>
		private static T FindExtremeValue<T>(IEnumerable items, T nullValue, Comparison<T> comparison, int sign)
		{
			var enumerator = items.GetEnumerator();

			// empty collection - return nullValue
			if (!enumerator.MoveNext())
				return nullValue;

			// enumerate all items to find extreme value
			var memo = (T) enumerator.Current;
			while (enumerator.MoveNext())
			{
				var item = (T) enumerator.Current;
				if (comparison(item, memo)*sign > 0)
					memo = item;
			}
			return memo;
		}

		/// <summary>
		/// Compares two collections to determine if they are equal, optionally considering the order of elements.
		/// </summary>
		/// <remarks>
		/// Two collections are considered equal if they contain the same number of elements and every element
		/// contained in one collection is contained in the other. If <paramref name="orderSensitive"/> is true,
		/// the elements must also enumerate in the same order.  Equality of individual elements is determined
		/// by their implementation of <see cref="object.Equals(object)"/>.
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="orderSensitive"></param>
		/// <returns></returns>
		//TODO: write unit test
		public static bool Equal<T>(ICollection<T> x, ICollection<T> y, bool orderSensitive)
		{
			if (ReferenceEquals(x, y))
				return true;

			if (x == null || y == null)
				return false;

			if (x.Count != y.Count)
				return false;

			// if order matters, compare each item one by one
			if (orderSensitive)
			{
				var enumY = y.GetEnumerator();
				foreach (var item in x)
				{
					if (!enumY.MoveNext())
						return false;
					if (!Equals(enumY.Current, item))
						return false;
				}
				return true;
			}

			// order does not matter, so need to do an O(N2) comparison
			return TrueForAll(x, y.Contains);
		}

		/// <summary>
		/// Compares two collections to determine if they are equal, optionally considering the order of elements.
		/// </summary>
		/// <remarks>
		/// Two collections are considered equal if they contain the same number of elements and every element
		/// contained in one collection is contained in the other. If <paramref name="orderSensitive"/> is true,
		/// the elements must also enumerate in the same order.  Equality of individual elements is determined
		/// by their implementation of <see cref="object.Equals(object)"/>.
		/// </remarks>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="orderSensitive"></param>
		/// <returns></returns>
		//TODO: write unit test
		public static bool Equal(ICollection x, ICollection y, bool orderSensitive)
		{
			if (ReferenceEquals(x, y))
				return true;

			if (x == null || y == null)
				return false;

			if (x.Count != y.Count)
				return false;

			// if order matters, compare each item one by one
			if (orderSensitive)
			{
				var enumY = y.GetEnumerator();
				foreach (var itemX in x)
				{
					if (!enumY.MoveNext())
						return false;
					if (!Equals(enumY.Current, itemX))
						return false;
				}
				return true;
			}

			// order does not matter, so need to do an O(N2) comparison
			return TrueForAll(x, itemX => Contains(y, itemY => Equals(itemY, itemX)));
		}

		/// <summary>
		/// Returns a new list containing only the unique elements of the target collection, preserving the order.
		/// Relies on <see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/>, since a dictionary
		/// is used internally to create the unique set of results.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <returns></returns>
		public static List<T> Unique<T>(IEnumerable<T> target)
		{
			return Unique(target, null);
		}

		/// <summary>
		/// Returns a new list containing only the unique elements of the target collection, preserving the order.
		/// The specified <see cref="IEqualityComparer{T}"/> is used to determine uniqueness.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static List<T> Unique<T>(IEnumerable<T> target, IEqualityComparer<T> comparer)
		{
			var set = comparer == null ? new Dictionary<T, T>() : new Dictionary<T, T>(comparer);
			var result = new List<T>();
			var resultContainsNull = false;
			foreach (var item in target)
			{
				// handle null item as a special case, because cannot insert it as a key into the hash table
				if (item == null)
				{
					if (!resultContainsNull) result.Add(item);
					resultContainsNull = true;
				}
				else if (!set.ContainsKey(item))
				{
					set.Add(item, item);
					result.Add(item);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a new list containing only the unique elements of the target collection, preserving the order.
		/// Relies on <see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/>, since a dictionary
		/// is used internally to create the unique set of results.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static ArrayList Unique(IEnumerable target)
		{
			return Unique(target, null);
		}

		/// <summary>
		/// Returns a new list containing only the unique elements of the target collection, preserving the order.
		/// The specified <see cref="IEqualityComparer"/> is used to determine uniqueness.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static ArrayList Unique(IEnumerable target, IEqualityComparer comparer)
		{
			var set = comparer == null ? new Hashtable() : new Hashtable(comparer);
			var result = new ArrayList();
			var resultContainsNull = false;
			foreach (var item in target)
			{
				// handle null item as a special case, because cannot insert it as a key into the hash table
				if (item == null)
				{
					if (!resultContainsNull) result.Add(item);
					resultContainsNull = true;
				}
				else if (!set.ContainsKey(item))
				{
					set.Add(item, item);
					result.Add(item);
				}
			}
			return result;
		}

		/// <summary>
		/// Casts each item in the target collection to the specified type, and returns the results
		/// in a new list.
		/// </summary>
		/// <typeparam name="TOutput"></typeparam>
		/// <param name="target"></param>
		/// <returns></returns>
		public static List<TOutput> Cast<TOutput>(IEnumerable target)
		{
			return Map(target, (object input) => (TOutput) input);
		}

		/// <summary>
		/// Concatenates all target collections into a single collection.  The items are added
		/// in order.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="targets"></param>
		/// <returns></returns>
		public static List<TItem> Concat<TItem>(params IEnumerable<TItem>[] targets)
		{
			var result = new List<TItem>();
			foreach (var target in targets)
			{
				result.AddRange(target);
			}
			return result;
		}

		/// <summary>
		/// Concatenates all target collections into a single collection.  The items are added
		/// in order.
		/// </summary>
		/// <param name="targets"></param>
		/// <returns></returns>
		public static ArrayList Concat(params ICollection[] targets)
		{
			var result = new ArrayList();
			foreach (var target in targets)
			{
				result.AddRange(target);
			}
			return result;
		}

		/// <summary>
		/// Concatenates all target collections into a single collection.  The items are added
		/// in order.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="targets"></param>
		/// <returns></returns>
		public static List<TItem> Concat<TItem>(List<List<TItem>> targets)
		{
			var result = new List<TItem>();
			foreach (var target in targets)
			{
				result.AddRange(target);
			}
			return result;
		}

		/// <summary>
		/// Partitions elements of the target collection into sub-groups based on the specified key generating function,
		/// and returns a dictionary of the generated keys, where each value is a list of the items that produced that key.
		/// Items appear in the sub-lists in the order in which they were enumerated from the target.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="target"></param>
		/// <param name="keyFunc"></param>
		/// <returns></returns>
		public static Dictionary<K, List<T>> GroupBy<T, K>(IEnumerable<T> target, Converter<T, K> keyFunc)
		{
			var results = new Dictionary<K, List<T>>();
			foreach (var item in target)
			{
				var key = keyFunc(item);
				List<T> group;
				if (!results.TryGetValue(key, out group))
				{
					results[key] = group = new List<T>();
				}
				group.Add(item);
			}
			return results;
		}

		/// <summary>
		/// Creates a dictionary from the target collection by mapping each element to a corresponding key and value using the specified functions.
		/// </summary>
		/// <typeparam name="T">Target collection element type.</typeparam>
		/// <typeparam name="K">Key type.</typeparam>
		/// <typeparam name="V">Value type.</typeparam>
		/// <param name="target"></param>
		/// <param name="keyFunc"></param>
		/// <param name="valueFunc"></param>
		/// <returns></returns>
		public static Dictionary<K, V> MakeDictionary<T, K, V>(IEnumerable<T> target, Converter<T, K> keyFunc, Converter<T, V> valueFunc)
		{
			var result = new Dictionary<K, V>();
			foreach (var item in target)
			{
				result.Add(keyFunc(item), valueFunc(item));
			}
			return result;
		}

		/// <summary>
		/// Creates a read-only view of a source dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> source)
		{
			Platform.CheckForNullReference(source, "source");
			return new ReadOnlyDictionary<TKey, TValue>(source);
		}
	}
}