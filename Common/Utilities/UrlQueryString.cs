﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Utility class for constructing URL query strings from collections of values.
	/// </summary>
	/// <remarks>
	/// Values are automatically URL-encoded by this class.
	/// </remarks>
	public class UrlQueryString
	{
		/// <summary>
		/// Builds a query string from the specified values.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string Build(IEnumerable<KeyValuePair<string, object>> values)
		{
			return new UrlQueryString(values).ToString();
		}

		/// <summary>
		/// Builds a query string from the values taken from the public properties of the specified object.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string Build(object values)
		{
			return new UrlQueryString(values).ToString();
		}


		private readonly List<Tuple<string, object>> _values;

		/// <summary>
		/// Constructs an empty instance.
		/// </summary>
		public UrlQueryString()
		{
			_values = new List<Tuple<string, object>>();
		}

		/// <summary>
		/// Constructs an instance initialized with the specified values.
		/// </summary>
		/// <param name="values"></param>
		public UrlQueryString(IEnumerable<Tuple<string, object>> values)
		{
			_values = values.ToList();
		}

		/// <summary>
		/// Constructs an instance initialized with the specified values.
		/// </summary>
		/// <param name="values"></param>
		public UrlQueryString(IEnumerable<KeyValuePair<string, object>> values)
		{
			_values = values.Select(kvp => Tuple.Create(kvp.Key, kvp.Value)).ToList();
		}

		/// <summary>
		/// Constructs an instance initialized with values taken from the public properties of the specified object.
		/// </summary>
		/// <param name="values"></param>
		public UrlQueryString(object values)
		{
			_values = ObjectLiteralToTuples(values).ToList();
		}

		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(string key, object value)
		{
			_values.Add(Tuple.Create(key, value));
		}

		/// <summary>
		/// Adds the specified values.
		/// </summary>
		/// <param name="values"></param>
		public void Add(IEnumerable<Tuple<string, object>> values)
		{
			_values.AddRange(values);
		}

		/// <summary>
		/// Adds the specified values.
		/// </summary>
		/// <param name="values"></param>
		public void Add(IEnumerable<KeyValuePair<string, object>> values)
		{
			_values.AddRange(values.Select(kvp => Tuple.Create(kvp.Key, kvp.Value)));
		}

		/// <summary>
		/// Adds the values taken from the public properties of the specified object.
		/// </summary>
		/// <param name="values"></param>
		public void Add(object values)
		{
			_values.AddRange(ObjectLiteralToTuples(values));
		}

		/// <summary>
		/// Gets the resulting query string represented by this instance, using the &amp; character as a separator.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString("&");
		}

		/// <summary>
		/// Gets the resulting query string represented by this instance using the specified separator.
		/// </summary>
		/// <returns></returns>
		public string ToString(string separator)
		{
			return string.Join(separator,
				_values.Select(tuple => string.Format("{0}={1}", tuple.Item1, HttpUtility.UrlEncode(tuple.Item2.ToString()))
				).ToArray());
		}

		private static IEnumerable<Tuple<string, object>> ObjectLiteralToTuples(object objLiteral)
		{
			return from k in objLiteral.GetType().GetProperties()
				   let v = k.GetValue(objLiteral, null)
				   where v != null						// exclude properties that are null-valued
				   select Tuple.Create(k.Name, v);
		}
	}
}
