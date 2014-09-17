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
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for implementations of <see cref="ISuggestionProvider"/> that provides most of the boilerplate
    /// functionality.
    /// </summary>
    /// <remarks>
    /// <para>
	/// This class implements a two-phase algorithm for providing a list of suggestions in response to a query string.  In the first phase,
	/// the query string is used to obtain a "shortlist" of suggestions.  Once the shortlist has been obtained, the second phase is entered,
	/// where subsequent refinements of the query string are used to refine the shortlist.  Clearing the query
	/// string, or modifying it such that it is no longer a refinement of the query that generated the initial shortlist, causes
	/// a transition back to the first phase, where the query is used to obtain a new shortlist.
	/// </para>
	/// <para>
	/// It is the responsibility of the subclass to obtain the shortlist by implementing the <see cref="GetShortList"/> method.  This method
	/// is always invoked on a background thread, in order to avoid locking up the user-interface.  Therefore, the subclass need not worry
	/// about the duration of execution of this method (within reason).  The method may return null in the event that the specified query string
	/// is too broad to return a shortlist.  In this case, the method will be called repeatedly, with each change to the query string, until
	/// it returns a non-null value.
	/// </para>
	/// <para>
	/// The subclass may choose to implement the refinement logic as well, by overriding <see cref="RefineShortList"/>.  However,
	/// this is typically not necessary as several standard refinement strategies are provided.  These are <see cref="RefinementStrategies.StartsWith"/>,
	/// which refines the list to include only those items that start with the query string, and
	/// <see cref="RefinementStrategies.MatchAllTerms"/>, which refines the list to include only those items that contain all terms specified
	/// in the query string, regardless of the order of those terms.  This is the default strategy used, in not explicitly specified.
	/// </para>
    /// </remarks>
    /// <typeparam name="TItem"></typeparam>
    public abstract class SuggestionProviderBase<TItem> : ISuggestionProvider
    {
		#region Refinement Strategies

		/// <summary>
		/// Defines an interface to an object that is responsible for refining a shortlist according to a query string.
		/// </summary>
		public interface IRefinementStrategy
		{
			/// <summary>
			/// Refines the specified shortlist to obtain an even shorter list, based on the specified query.
			/// </summary>
			/// <param name="shortList"></param>
			/// <param name="query"></param>
			/// <param name="formatter"></param>
			/// <returns></returns>
			IList<TItem> Refine(IList<TItem> shortList, string query, Converter<TItem, string> formatter);
		}

		/// <summary>
		/// Defines a set of predefined refinement strategies.
		/// </summary>
		public static class RefinementStrategies
		{
			/// <summary>
			/// A refinement strategy that will return only items that start with the query string.
			/// </summary>
			public static readonly IRefinementStrategy StartsWith = new StartsWithStrategy();

			/// <summary>
			/// A refinement strategy that will return items containing all of the terms in the query string,
			/// regardless of the order in which the terms occur.
			/// </summary>
			public static readonly IRefinementStrategy MatchAllTerms = new MatchAllTermsStrategy();


			class StartsWithStrategy : IRefinementStrategy
			{
				public IList<TItem> Refine(IList<TItem> shortList, string query, Converter<TItem, string> formatter)
				{
					// refine the short-list
					return CollectionUtils.Select(shortList,
						delegate(TItem item)
						{
							var itemString = formatter(item);
							return itemString.StartsWith(query, StringComparison.CurrentCultureIgnoreCase);
						});
				}
			}

			class MatchAllTermsStrategy : IRefinementStrategy
			{
				private static readonly Regex _termDefinition = new Regex(@"\b([^\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

				public IList<TItem> Refine(IList<TItem> shortList, string query, Converter<TItem, string> formatter)
				{
					// break query up into keywords
					var keywords = CollectionUtils.Map(_termDefinition.Matches(query), (Match m) => m.Value);

					// refine the short-list
					return CollectionUtils.Select(shortList,
						delegate(TItem item)
						{
							// for an item to be included, the formatted string must contain *all* keywords
							var itemString = formatter(item);
							return CollectionUtils.TrueForAll(keywords,
								kw => Regex.Match(itemString, @"\b" + Regex.Escape(kw), RegexOptions.IgnoreCase).Success);
						});
				}
			}
		}

		#endregion

		#region State classes (state pattern implementation)

		/// <summary>
		/// Base class for states
		/// </summary>
		abstract class State
		{
			protected readonly SuggestionProviderBase<TItem> _owner;

			protected State(SuggestionProviderBase<TItem> owner)
			{
				_owner = owner;
			}

			/// <summary>
			/// Called when a state is first instantiated, to "begin" that state.
			/// </summary>
			public abstract void Begin();

			/// <summary>
			/// Called when the user updates the query string.
			/// </summary>
			/// <param name="query"></param>
			public abstract void Update(string query);

			protected void BeginRequest(string query)
			{
				_owner.ChangeState(new ShortlistRequestState(_owner, query));
			}
		}

		/// <summary>
		/// Defines the state when the shortlist is not known.
		/// </summary>
		class CleanState : State
		{
			public CleanState(SuggestionProviderBase<TItem> owner)
				: base(owner)
			{
			}

			public override void Begin()
			{
			}

			public override void Update(string query)
			{
				BeginRequest(query);
			}

		}

		/// <summary>
		/// Defines the state where a request for a shortlist is in progress.
		/// </summary>
		class ShortlistRequestState : State
		{
			private BackgroundTask _task;
			private readonly string _query;
			private string _currentQuery;

			public ShortlistRequestState(SuggestionProviderBase<TItem> owner, string query)
				: base(owner)
			{
				_currentQuery = _query = query;
			}

			public override void Begin()
			{
				_task = new BackgroundTask(
					delegate(IBackgroundTaskContext context)
					{
						try
						{
							var results = _owner.GetShortList(_query);
							context.Complete(results);
						}
						catch (Exception e)
						{
							context.Error(e);
						}
					}, false, _query) { ThreadUICulture = Desktop.Application.CurrentUICulture };
				_task.Terminated += OnTerminated;
				_task.Run();
			}

			public override void Update(string query)
			{
				// do nothing while a request is pending
				_currentQuery = query;
			}

			private void OnTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
			{
				if (e.Reason == BackgroundTaskTerminatedReason.Exception)
				{
					// not much we can do about it, except log it and return to blank state
					Platform.Log(LogLevel.Error, e.Exception);
					_owner.ChangeState(new CleanState(_owner));
				}
				else
				{
					var shortlist = (IList<TItem>)e.Result;
					if(shortlist == null)
					{
						// the request did not return a shortlist
						// has the query been updated in the interim? if so, begin a new request immediately
						if (_currentQuery != _query)
						{
							BeginRequest(_currentQuery);
						}
						else
						{
							// return to blank state
							_owner.ChangeState(new CleanState(_owner));
						}
					}
					else
					{
						// the request obtained a shortlist
						// has the query been updated in the interim? if so, is it a refinement of the query that obtained the shortlist?
						if (_currentQuery == _query || _owner.IsQueryRefinement(_currentQuery, _query))
						{
							if(_owner.AutoSort)
							{
								shortlist = CollectionUtils.Sort(shortlist, (x, y) => _owner.FormatItem(x).CompareTo(_owner.FormatItem(y)));
							}

							_owner.ChangeState(new ShortlistKnownState(_owner, _currentQuery, shortlist));
						}
						else
						{
							// the query was updated and is not a refinement, hence the shortlist is not valid
							// begin a new request
							BeginRequest(_currentQuery);
						}
					}
				}
			}

		}

    	/// <summary>
		/// Defines the state where the shortlist is known.
		/// </summary>
		class ShortlistKnownState : State
		{
			private readonly string _query;
			private readonly IList<TItem> _shortlist;

			public ShortlistKnownState(SuggestionProviderBase<TItem> owner, string query, IList<TItem> shortlist)
				: base(owner)
			{
				_query = query;
				_shortlist = shortlist;
			}

			public override void Begin()
			{
				Update(_query);
			}

			public override void Update(string query)
			{
				if(query == _query || _owner.IsQueryRefinement(query, _query))
				{
					var refinedList = _owner.RefineShortList(_shortlist, query);
					_owner.PostSuggestions(refinedList);
				}
				else
				{
					// shortlist no longer valid - clear suggestions
					_owner.PostSuggestions(new TItem[]{});

					// request new shortlist
					BeginRequest(query);
				}
			}
		}

		#endregion

		private readonly IRefinementStrategy _refinementStrategy;
		private State _state;
		private event EventHandler<SuggestionsProvidedEventArgs> _suggestionsProvided;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected SuggestionProviderBase()
			:this(RefinementStrategies.MatchAllTerms)
		{

		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected SuggestionProviderBase(IRefinementStrategy refinementStrategy)
        {
			_state = new CleanState(this);
			_refinementStrategy = refinementStrategy;
			this.AutoSort = true;	// default
        }

		///<summary>
		/// Gets a value indicating whether this instance will automatically sort contents according display format.
		///</summary>
		public bool AutoSort { get; set; }

		#region ISuggestionProvider Members

		/// <summary>
		/// Notifies the user-interfaces that an updated list of suggestions is available.
		/// </summary>
		event EventHandler<SuggestionsProvidedEventArgs> ISuggestionProvider.SuggestionsProvided
		{
			add { _suggestionsProvided += value; }
			remove { _suggestionsProvided -= value; }
		}

		/// <summary>
		/// Called by the user-inteface to inform this object of changes in the user query text.
		/// </summary>
		void ISuggestionProvider.SetQuery(string query)
		{
			Update(StringUtilities.EmptyIfNull(query));
		}

		#endregion

		#region Protected API

		/// <summary>
        /// Called to obtain the initial source list for the specified query.  May return null if no items are available.
        /// </summary>
        /// <remarks>
        /// <para>
		/// This method is called to obtain an initial list of items for a given query.  The method should return 
		/// null if no items are available for the specified query (or if the query is too "broad" to return any suggestions).
		/// This method is called repeatedly each time the user updates the query, until a non-null result is returned.
		/// Once this method returns a non-null result, it is not called again as long as subsequent queries are increasingly
		/// "specific", as defined by the implementation of <see cref="IsQueryRefinement"/>.  The method <see cref="RefineShortList"/> is called instead.
		/// However, as soon as a query is encountered that is less specific than the query that generated the shortlist
		/// (e.g. the user presses backspace enough times), this method will be called again to generate a new source list.
		/// </para>
		/// <para>
		/// In order to keep the UI responsive, this method is invoked on a background thread, meaning that the implementation
		/// is free to make remote calls or perform other time consuming tasks to generate the list, without fear of locking up the UI.
		/// </para>
        /// </remarks>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract IList<TItem> GetShortList(string query);

        /// <summary>
        /// Called to format the specified item for display in the suggestion list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract string FormatItem(TItem item);

        /// <summary>
        /// Called to successively refine a shortlist of items.
        /// </summary>
        /// <param name="shortList"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is called to refine the short-list obtained by the initial call(s) to <see cref="GetShortList"/>.
        /// The default implementation invokes the refinement strategy that was specified in the constructor.
        /// Override this method to implement custom refinement logic.
        /// </remarks>
        protected virtual IList<TItem> RefineShortList(IList<TItem> shortList, string query)
        {
            // refine the short-list
        	return _refinementStrategy.Refine(shortList, query, FormatItem);
        }

        /// <summary>
        /// Called to determine if the specified query is a refinement of the previous query,
        /// that is, whether the existing shortlist can be refined or should be discarded.
        /// </summary>
		/// <param name="query"></param>
        /// <param name="previousQuery"></param>
        /// <returns></returns>
        /// <remarks>
        /// The default implementation of this method returns true if query starts with previousQuery.
        /// </remarks>
        protected virtual bool IsQueryRefinement(string query, string previousQuery)
        {
            return query.StartsWith(previousQuery);
        }

		#endregion

		#region Helpers

		/// <summary>
		/// Updates the state machine.
		/// </summary>
		/// <param name="query"></param>
		private void Update(string query)
        {
			_state.Update(query);
        }

		/// <summary>
		/// Called by the <see cref="State"/> classes to change the state of this object.
		/// </summary>
		/// <param name="state"></param>
		private void ChangeState(State state)
		{
			_state = state;
			_state.Begin();
		}

		/// <summary>
		/// Posts the specified list of suggested items to the consumer of this provider.
		/// </summary>
		/// <param name="suggestions"></param>
		private void PostSuggestions(IEnumerable<TItem> suggestions)
		{
			EventsHelper.Fire(_suggestionsProvided, this, new SuggestionsProvidedEventArgs(new List<TItem>(suggestions)));
		}

		#endregion
	}
}
