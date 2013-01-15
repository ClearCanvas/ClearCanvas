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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Action model that allows a user to control a <see cref="IPagingController{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class PagingActionModel<TItem> : SimpleActionModel
	{
		private readonly IDesktopWindow _desktopWindow;
		private readonly IPagingController<TItem> _controller;

		///<summary>
		/// Constructor.
		///</summary>
		///<param name="controller"></param>
		///<param name="desktopWindow"></param>
		public PagingActionModel(IPagingController<TItem> controller, IDesktopWindow desktopWindow)
			: base(new ApplicationThemeResourceResolver(typeof(PagingActionModel<TItem>).Assembly))
		{
			_controller = controller;
			_desktopWindow = desktopWindow;

			AddAction("Previous", SR.TitlePrevious, "Icons.PreviousPageToolSmall.png");
			AddAction("Next", SR.TitleNext, "Icons.NextPageToolSmall.png");

			Next.SetClickHandler(OnNext);
			Previous.SetClickHandler(OnPrevious);

			Next.Enabled = _controller.HasNext;
			Previous.Enabled = _controller.HasPrevious;  // can't go back from first

			_controller.PageChanged += PageChangedEventHandler;
		}

		private void OnNext()
		{
			try
			{
				_controller.GetNext();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, _desktopWindow);
			}
		}

		private void OnPrevious()
		{
			try
			{
				_controller.GetPrevious();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, _desktopWindow);
			}
		}

		private void PageChangedEventHandler(object sender, PageChangedEventArgs<TItem> args)
		{
			this.Next.Enabled = _controller.HasNext;
			this.Previous.Enabled = _controller.HasPrevious;
		}

		private ClickAction Next
		{
			get { return (ClickAction)this["Next"]; }
		}

		private ClickAction Previous
		{
			get { return (ClickAction)this["Previous"]; }
		}
	}
}
