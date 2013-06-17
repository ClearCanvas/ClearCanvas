using System;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Represents a page of a result set.
	/// </summary>
	public struct PageRange : IEquatable<PageRange>
	{
		/// <summary>
		/// Gets a page range that includes all items.
		/// </summary>
		public static readonly PageRange All = new PageRange(null, int.MaxValue - 1);

		/// <summary>
		/// Gets the position token that identifies the starting point.
		/// </summary>
		public readonly string PositionToken;

		/// <summary>
		/// Gets the number of items to return.
		/// </summary>
		public readonly int MaxItems;

		/// <summary>
		/// Constructs a page range for the specified number of items starting at the first result.
		/// </summary>
		public PageRange(int maxItems)
			:this(null, maxItems)
		{
		}

		/// <summary>
		/// Constructs a page range at the specified starting position, for the specified number of items.
		/// </summary>
		public PageRange(string positionToken, int maxItems)
		{
			PositionToken = positionToken;
			MaxItems = maxItems;
		}

		/// <summary>
		/// Gets a value indicating whether this page range represents the first page of results.
		/// </summary>
		public bool IsFirst
		{
			get { return PositionToken == null; }
		}

		public bool Equals(PageRange other)
		{
			return Equals(other.PositionToken, PositionToken) && other.MaxItems == MaxItems;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (PageRange)) return false;
			return Equals((PageRange) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((PositionToken != null ? PositionToken.GetHashCode() : 0) * 397) ^ MaxItems.GetHashCode();
			}
		}

		public static bool operator ==(PageRange left, PageRange right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PageRange left, PageRange right)
		{
			return !left.Equals(right);
		}
	}
}
