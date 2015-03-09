using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.Enterprise.Common
{
	public class PagedResults<TItem> : IEnumerable<TItem>
	{
		private readonly IList<TItem> _items;
		private readonly string _positionToken;
		private readonly bool _hasNextPage;

		public PagedResults(IList<TItem> items, string positionToken, bool hasNextPage)
		{
			_items = items;
			_positionToken = positionToken;
			_hasNextPage = hasNextPage;
		}

		public bool HasNextPage
		{
			get { return _hasNextPage; }
		}

		public string PositionToken
		{
			get { return _positionToken; }
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public IList<TItem> Items
		{
			get { return _items; }
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
			return new PagedResults<TSubclass>(_items.Cast<TSubclass>().ToList(), _positionToken, _hasNextPage);
		}
	}
}
