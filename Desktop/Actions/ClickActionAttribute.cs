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
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to declare "click" actions.
    /// </summary>
    public abstract class ClickActionAttribute : ActionInitiatorAttribute
    {
        private readonly string _path;
        private readonly string _clickHandler;
    	private bool _initiallyAvailable = true;
        private ClickActionFlags _flags;
		private XKeys _keyStroke;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="clickHandler">The name of the method that will be invoked when the action is clicked.</param>
        public ClickActionAttribute(string actionID, string path, string clickHandler)
            :base(actionID)
        {
            _path = path;
            _clickHandler = clickHandler;
            _flags = ClickActionFlags.None; // default value, will override if named parameter is specified
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        public ClickActionAttribute(string actionID, string path)
            : this(actionID, path, null)
        {
        }

		/// <summary>
		/// Gets the name of the method that will be invoked when the action is clicked.
		/// </summary>
    	public string ClickHandler
    	{
			get { return _clickHandler; }
    	}

		/// <summary>
		/// Gets or sets a value indicating whether or not the action should be available by default when not overriden by the action model.
		/// </summary>
    	public bool InitiallyAvailable
    	{
			get { return _initiallyAvailable; }
			set { _initiallyAvailable = value; }
    	}

        /// <summary>
        /// Gets or sets the flags that customize the behaviour of the action.
        /// </summary>
        public ClickActionFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        /// <summary>
        /// Gets or sets the key-stroke that should invoke the action from the keyboard.
        /// </summary>
		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

		/// <summary>
		/// The suggested location of the action in the action model.
		/// </summary>
		public string Path { get { return _path; } }
		
		/// <summary>
		/// Applies this attribute to an <see cref="IAction"/> instance, via the specified <see cref="IActionBuildingContext"/>.
    	/// </summary>
    	/// <remarks>
    	/// Because this action is an <see cref="ActionInitiatorAttribute"/>, this method actually
    	/// creates the associated <see cref="ClickAction"/>.  <see cref="ActionDecoratorAttribute"/>s
    	/// merely modify the properties of the action.
    	/// </remarks>
    	public override void Apply(IActionBuildingContext builder)
        {
            // assert _action == null
            ActionPath path = new ActionPath(this.Path, builder.ResourceResolver);
            builder.Action = CreateAction(builder.ActionID, path, this.Flags, builder.ResourceResolver);
			builder.Action.Available = this.InitiallyAvailable;
            builder.Action.Persistent = true;
            ((ClickAction)builder.Action).KeyStroke = this.KeyStroke;
            builder.Action.Label = path.LastSegment.LocalizedText;

            if (!string.IsNullOrEmpty(_clickHandler))
            {
                // check that the method exists, etc
                ValidateClickHandler(builder.ActionTarget, _clickHandler);

                ClickHandlerDelegate clickHandler =
                    (ClickHandlerDelegate)Delegate.CreateDelegate(typeof(ClickHandlerDelegate), builder.ActionTarget, _clickHandler);
                ((ClickAction)builder.Action).SetClickHandler(clickHandler);
            }
        }

		/// <summary>
		/// Creates the <see cref="ClickAction"/> represented by this attribute.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="flags">Flags that specify the click behaviour of the action.</param>
		/// <param name="resolver">The object used to resolve the action path and icons.</param>
        protected abstract ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver);

        private static void ValidateClickHandler(object target, string methodName)
        {
            MethodInfo info = target.GetType().GetMethod(
                methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);

            if (info == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderMethodDoesNotExist, methodName, target.GetType().FullName));
            }
        }
    }
}
