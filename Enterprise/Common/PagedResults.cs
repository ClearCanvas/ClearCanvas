using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.Enterprise.Common
{
	public class PagedResults<TItem> : IEnumerable<TItem>
	{
		private readonly List<TItem> _items;
		private readonly string _nextPageToken;

		public PagedResults(List<TItem> items, string positionToken)
		{
			_items = items;
			_nextPageToken = positionToken;
		}

		public bool HasNextPage
		{
			get { return _nextPageToken != null; }
		}

		public string NextPageToken
		{
			get { return _nextPageToken; }
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public IList<TItem> Items
		{
			get { return _items.AsReadOnly(); }
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public PagedResults<TSubclass> Downcast<TSubclass>()
			where TSubclass : TItem
		{
			return new PagedResults<TSubclass>(_items.Cast<TSubclass>().ToList(), _nextPageToken);
		}
	}
}
