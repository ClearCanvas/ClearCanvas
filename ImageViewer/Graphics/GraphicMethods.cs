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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines utility extension methods for working with <see cref="IGraphic"/>s.
	/// </summary>
	public static class GraphicMethods
	{
		/// <summary>
		/// Performs an action on the <see cref="IGraphic"/> within the context of source coordinates.
		/// </summary>
		/// <remarks>
		/// This extension method provides simpler notation for trivial cases where spatial coordinates
		/// need to be retrieved from a graphic under a specific coordinate system. 
		/// </remarks>
		/// <typeparam name="TGraphic">Type of the <see cref="IGraphic"/>.</typeparam>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="graphic">The <see cref="IGraphic"/> on which to operate.</param>
		/// <param name="function">A delegate to perform the action on the specified <paramref name="graphic"/>.</param>
		/// <returns>The result of the <paramref name="function"/>.</returns>
		public static TResult WithSourceCoordinates<TGraphic, TResult>(this TGraphic graphic, Func<TGraphic, TResult> function)
			where TGraphic : IGraphic
		{
			return WithCoordinateSystem(graphic, CoordinateSystem.Source, function);
		}

		/// <summary>
		/// Performs an action on the <see cref="IGraphic"/> within the context of destination coordinates.
		/// </summary>
		/// <remarks>
		/// This extension method provides simpler notation for trivial cases where spatial coordinates
		/// need to be retrieved from a graphic under a specific coordinate system. 
		/// </remarks>
		/// <typeparam name="TGraphic">Type of the <see cref="IGraphic"/>.</typeparam>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="graphic">The <see cref="IGraphic"/> on which to operate.</param>
		/// <param name="function">A delegate to perform the action on the specified <paramref name="graphic"/>.</param>
		/// <returns>The result of the <paramref name="function"/>.</returns>
		public static TResult WithDestinationCoordinates<TGraphic, TResult>(this TGraphic graphic, Func<TGraphic, TResult> function)
			where TGraphic : IGraphic
		{
			return WithCoordinateSystem(graphic, CoordinateSystem.Destination, function);
		}

		/// <summary>
		/// Performs an action on the <see cref="IGraphic"/> within the context of the specified coordinates.
		/// </summary>
		/// <remarks>
		/// This extension method provides simpler notation for trivial cases where spatial coordinates
		/// need to be retrieved from a graphic under a specific coordinate system. 
		/// </remarks>
		/// <typeparam name="TGraphic">Type of the <see cref="IGraphic"/>.</typeparam>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="graphic">The <see cref="IGraphic"/> on which to operate.</param>
		/// <param name="coordinateSystem">The desired <see cref="CoordinateSystem"/>.</param>
		/// <param name="function">A delegate to perform the action on the specified <paramref name="graphic"/>.</param>
		/// <returns>The result of the <paramref name="function"/>.</returns>
		public static TResult WithCoordinateSystem<TGraphic, TResult>(this TGraphic graphic, CoordinateSystem coordinateSystem, Func<TGraphic, TResult> function)
			where TGraphic : IGraphic
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(function, "function");

			graphic.CoordinateSystem = coordinateSystem;
			try
			{
				return function.Invoke(graphic);
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Performs an action on the <see cref="IGraphic"/> within the context of source coordinates.
		/// </summary>
		/// <remarks>
		/// This extension method provides simpler notation for trivial cases where spatial coordinates
		/// need to be retrieved from a graphic under a specific coordinate system. 
		/// </remarks>
		/// <typeparam name="TGraphic">Type of the <see cref="IGraphic"/>.</typeparam>
		/// <param name="graphic">The <see cref="IGraphic"/> on which to operate.</param>
		/// <param name="action">A delegate to perform the action on the specified <paramref name="graphic"/>.</param>
		public static void WithSourceCoordinates<TGraphic>(this TGraphic graphic, Action<TGraphic> action)
			where TGraphic : IGraphic
		{
			WithCoordinateSystem(graphic, CoordinateSystem.Source, action);
		}

		/// <summary>
		/// Performs an action on the <see cref="IGraphic"/> within the context of destination coordinates.
		/// </summary>
		/// <remarks>
		/// This extension method provides simpler notation for trivial cases where spatial coordinates
		/// need to be retrieved from a graphic under a specific coordinate system. 
		/// </remarks>
		/// <typeparam name="TGraphic">Type of the <see cref="IGraphic"/>.</typeparam>
		/// <param name="graphic">The <see cref="IGraphic"/> on which to operate.</param>
		/// <param name="action">A delegate to perform the action on the specified <paramref name="graphic"/>.</param>
		public static void WithDestinationCoordinates<TGraphic>(this TGraphic graphic, Action<TGraphic> action)
			where TGraphic : IGraphic
		{
			WithCoordinateSystem(graphic, CoordinateSystem.Destination, action);
		}

		/// <summary>
		/// Performs an action on the <see cref="IGraphic"/> within the context of the specified coordinates.
		/// </summary>
		/// <remarks>
		/// This extension method provides simpler notation for trivial cases where spatial coordinates
		/// need to be retrieved from a graphic under a specific coordinate system. 
		/// </remarks>
		/// <typeparam name="TGraphic">Type of the <see cref="IGraphic"/>.</typeparam>
		/// <param name="graphic">The <see cref="IGraphic"/> on which to operate.</param>
		/// <param name="coordinateSystem">The desired <see cref="CoordinateSystem"/>.</param>
		/// <param name="action">A delegate to perform the action on the specified <paramref name="graphic"/>.</param>
		public static void WithCoordinateSystem<TGraphic>(this TGraphic graphic, CoordinateSystem coordinateSystem, Action<TGraphic> action)
			where TGraphic : IGraphic
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(action, "action");

			graphic.CoordinateSystem = coordinateSystem;
			try
			{
				action.Invoke(graphic);
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}
		}
	}
}