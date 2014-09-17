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
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Abstract base class for application component hosts.
	/// </summary>
	public abstract class ApplicationComponentHost : IApplicationComponentHost
	{
		private readonly IApplicationComponent _component;
		private IApplicationComponentView _componentView;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="component">The component to be hosted.</param>
		protected ApplicationComponentHost(IApplicationComponent component)
		{
			_component = component;
			_component.SetHost(this);
		}

		/// <summary>
		/// Starts the hosted component.
		/// </summary>
		public virtual void StartComponent()
		{
			if (_component.IsStarted)
				throw new InvalidOperationException(SR.ExceptionComponentAlreadyStarted);

			_component.Start();
		}

		/// <summary>
		/// Stops the hosted component.
		/// </summary>
		public virtual void StopComponent()
		{
			if (!_component.IsStarted)
				throw new InvalidOperationException(SR.ExceptionComponentNeverStarted);

			_component.Stop();
		}

		/// <summary>
		/// Gets a value indicating whether the hosted component has been started.
		/// </summary>
		public bool IsStarted
		{
			get { return _component.IsStarted; }
		}

		/// <summary>
		/// Gets the hosted component.
		/// </summary>
		public IApplicationComponent Component
		{
			get { return _component; }
		}

		/// <summary>
		/// Gets the view for the hosted component, creating it if it has not yet been created.
		/// </summary>
		public IApplicationComponentView ComponentView
		{
			get
			{
				if (_componentView == null)
				{
					_componentView = (IApplicationComponentView) ViewFactory.CreateAssociatedView(_component.GetType());
					_componentView.SetComponent(_component);
				}
				return _componentView;
			}
		}

		#region IApplicationComponentHost Members

		/// <summary>
		/// Asks the host to exit.
		/// </summary>
		/// <exception cref="NotSupportedException">The host does not support exit requests.</exception>
		public virtual void Exit()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the associated command history object.
		/// </summary>
		/// <exception cref="NotSupportedException">The host does not support command history.</exception>
		public virtual CommandHistory CommandHistory
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Shows a message box in the associated desktop window.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public virtual DialogBoxAction ShowMessageBox([param : Localizable(true)] string message, MessageBoxActions buttons)
		{
			return this.DesktopWindow.ShowMessageBox(message, this.Title, buttons);
		}

		/// <summary>
		/// Asks the host to set the title in the user-interface.
		/// </summary>
		/// <exception cref="NotSupportedException">The host does not support titles.</exception>
		public void SetTitle([param : Localizable(true)] string title)
		{
			this.Title = title;
		}

		/// <summary>
		/// Gets or sets the title displayed in the user-interface.
		/// </summary>
		/// <exception cref="NotSupportedException">The host does not support titles.</exception>
		[Localizable(true)]
		public virtual string Title
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the associated desktop window.
		/// </summary>
		public abstract DesktopWindow DesktopWindow { get; }

		#endregion
	}
}