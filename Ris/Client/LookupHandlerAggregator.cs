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
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// An implementation of <see cref="ILookupHandler"/> that aggregates a set of child lookup handlers,
	/// allowing different types of objects to be found in a single lookup.
	/// </summary>
	public abstract class LookupHandlerAggregator : ILookupHandler
	{
		#region SuggestionProviderAggregator class

		/// <summary>
		/// Aggregates suggestions from different providers.
		/// </summary>
		class SuggestionProviderAggregator : ISuggestionProvider
		{
			private readonly LookupHandlerAggregator _owner;
			private event EventHandler<SuggestionsProvidedEventArgs> _suggestionsProvided;
			private readonly Dictionary<ISuggestionProvider, ILookupHandler> _lookupHandlers;

			internal SuggestionProviderAggregator(LookupHandlerAggregator owner)
			{
				_owner = owner;
				_lookupHandlers = new Dictionary<ISuggestionProvider, ILookupHandler>();
				foreach (var handler in _owner.ChildHandlers.Keys)
				{
					var suggestionProvider = handler.SuggestionProvider;

					// cache ref from suggest provider back to lookup handler, so we have an easy back pointer
					_lookupHandlers[suggestionProvider] = handler;

					// subscribe to changes from each suggestion provider
					suggestionProvider.SuggestionsProvided += SuggestionsProvidedEventHandler;
				}
			}

			/// <summary>
			/// Notifies the user-interfaces that an updated list of suggestions is available.
			/// </summary>
			public event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided
			{
				add { _suggestionsProvided += value; }
				remove { _suggestionsProvided -= value; }
			}

			/// <summary>
			/// Called by the user-inteface to inform this object of changes in the user query text.
			/// </summary>
			public void SetQuery(string query)
			{
				// update query in each suggestion provider
				foreach (var sp in _lookupHandlers.Keys)
				{
					sp.SetQuery(query);
				}
			}

			private void SuggestionsProvidedEventHandler(object sender, SuggestionsProvidedEventArgs e)
			{
				var childLookup = _lookupHandlers[(ISuggestionProvider)sender];

				// update the cached list of suggested items for the child lookup handler
				var items = _owner.SuggestedItemsCache[childLookup];
				items.Clear();
				items.AddRange(new TypeSafeEnumerableWrapper<object>(e.Items));

				// provide aggregate list of items
				var aggregate = CollectionUtils.Concat(new List<List<object>>(_owner.SuggestedItemsCache.Values));

				// sort the aggregate list according to the formatting of each item
				aggregate.Sort((x, y) => _owner.FormatItem(x).CompareTo(_owner.FormatItem(y)));
				EventsHelper.Fire(_suggestionsProvided, this, new SuggestionsProvidedEventArgs(aggregate));
			}
		}

		#endregion

		private readonly ISuggestionProvider _suggestionProvider;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="childHandlers"></param>
		protected LookupHandlerAggregator(Dictionary<ILookupHandler, Type> childHandlers)
		{
			this.ChildHandlers = childHandlers;

			// initialize the dictionary with an empty list for each handler
			this.SuggestedItemsCache = new Dictionary<ILookupHandler, List<object>>();
			foreach (var handler in childHandlers.Keys)
			{
				this.SuggestedItemsCache.Add(handler, new List<object>());
			}
			_suggestionProvider = new SuggestionProviderAggregator(this);
		}

		/// <summary>
		/// Attempts to resolve a query to a single item, optionally interacting with the user.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="query">The text query.</param>
		/// <param name="interactive">True if interaction with the user is allowed.</param>
		/// <param name="result">The singular result.</param>
		/// <returns>True if the query was resolved to a singular item, otherwise false.</returns>
		public bool Resolve(string query, bool interactive, out object result)
		{
			if(interactive)
			{
				return ResolveNameInteractive(query, out result);
			}

			// otherwise give each child a chance to resolve it without interaction
			var results = new List<object>();
			foreach (var handler in ChildHandlers.Keys)
			{
				object temp;
				if (handler.Resolve(query, false, out temp))
					results.Add(temp);
			}

			// check whether we've resolved it to a singular value
			result = results.Count == 1 ? CollectionUtils.FirstElement(results) : null;
			return result != null;
		}

		/// <summary>
		/// Formats an item for display in the user-interface.
		/// </summary>
		/// <remarks>
		/// Override this method to intercept formatting for customization.
		/// </remarks>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual string FormatItem(object item)
		{
			// determine which handler should format the item
			foreach (var kvp in SuggestedItemsCache)
			{
				if (kvp.Value.Contains(item))
					return kvp.Key.FormatItem(item);
			}

			foreach (var kvp in ChildHandlers)
			{
				if (item.GetType() == kvp.Value)
					return kvp.Key.FormatItem(item);
			}

			return null;
		}

		/// <summary>
		/// Gets a suggestion provider that provides suggestions for the lookup field.
		/// </summary>
		public ISuggestionProvider SuggestionProvider
		{
			get { return _suggestionProvider; }
		}

		/// <summary>
		/// Attempts to resolve the specified query to a single result, with user input.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		protected abstract bool ResolveNameInteractive(string query, out object result);

		/// <summary>
		/// Set of child lookup handlers that are aggregated by this instance and the type of item each handler can lookup.
		/// </summary>
		private Dictionary<ILookupHandler, Type> ChildHandlers { get; set; }

		/// <summary>
		/// Cache of the current list of suggestions for each child lookup handler.
		/// </summary>
		private Dictionary<ILookupHandler, List<object>> SuggestedItemsCache { get; set; }

	}
}
