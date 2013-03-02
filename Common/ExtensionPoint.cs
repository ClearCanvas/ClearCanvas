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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Abstract base class for extension points.
    /// </summary>
    public abstract class ExtensionPoint : IExtensionPoint
    {
        #region PredicateExtensionFilter

        private class PredicateExtensionFilter : ExtensionFilter
        {
            private Predicate<ExtensionInfo> _predicate;

            public PredicateExtensionFilter(Predicate<ExtensionInfo> predicate)
            {
                _predicate = predicate;
            }

            public override bool Test(ExtensionInfo extension)
            {
                return _predicate(extension);
            }
        }

        #endregion

        #region Static members

        // initialize with the default extension factory
        private static IExtensionFactory _extensionFactory = new DefaultExtensionFactory();


        /// <summary>
        /// Sets the <see cref="IExtensionFactory"/> that is used to create extensions.
        /// </summary>
        internal static void SetExtensionFactory(IExtensionFactory extensionFactory)
        {
            Platform.CheckForNullReference(extensionFactory, "extensionFactory");

            _extensionFactory = extensionFactory;
        }

        #endregion

		/// <summary>
		/// Gets the interface on which the extension is defined.
		/// </summary>
		public abstract Type InterfaceType { get; }

		#region IExtensionPoint methods

    	/// <summary>
    	/// Lists the available extensions.
    	/// </summary>
    	/// <returns>An array of <see cref="ExtensionInfo" /> objects describing the available extensions.</returns>
    	/// <remarks>
    	/// Available extensions are those that are both enabled and licensed for use.
    	/// </remarks>
    	public ExtensionInfo[] ListExtensions()
        {
            return ListExtensionsHelper(null);
        }

    	/// <summary>
    	/// Lists the available extensions, that also match the specified <see cref="ExtensionFilter"/>.
    	/// </summary>
    	/// <returns>An array of <see cref="ExtensionInfo" /> objects describing the available extensions.</returns>
    	/// <remarks>
    	/// Available extensions are those that are both enabled and licensed for use.
    	/// </remarks>
    	public ExtensionInfo[] ListExtensions(ExtensionFilter filter)
        {
            return ListExtensionsHelper(filter);
        }

    	/// <summary>
    	/// Lists the available extensions that match the specified filter.
    	/// </summary>
    	/// <remarks>
    	/// Available extensions are those that are both enabled and licensed for use.
    	/// </remarks>
    	public ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter)
        {
            return ListExtensionsHelper(new PredicateExtensionFilter(filter));
        }

    	/// <summary>
    	/// Instantiates one extension.
    	/// </summary>
    	/// <returns>A reference to the extension.</returns>
    	/// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
    	/// <remarks>
    	/// If more than one extension exists, then the type of the extension that is
    	/// returned is non-deterministic.  If no extensions exist that can be successfully
    	/// instantiated, an exception is thrown. Note that only extensions that are enabled
    	/// and licensed are considered.
    	/// </remarks>
    	public object CreateExtension()
        {
            return AtLeastOne(CreateExtensionsHelper(null, true), this.GetType());
        }

    	/// <summary>
    	/// Instantiates an extension that also matches the specified <see cref="ExtensionFilter" />.
    	/// </summary>
    	/// <returns>A reference to the extension.</returns>
    	/// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
    	/// <remarks>
    	/// If more than one extension exists, then the type of the extension that is
    	/// returned is non-deterministic.  If no extensions exist that can be successfully
    	/// instantiated, an exception is thrown. Note that only extensions that are enabled
    	/// and licensed are considered.
    	/// </remarks>
    	public object CreateExtension(ExtensionFilter filter)
        {
            return AtLeastOne(CreateExtensionsHelper(filter, true), this.GetType());
        }

    	/// <summary>
    	/// Instantiates an extension that matches the specified filter.
    	/// </summary>
    	/// <returns>A reference to the extension.</returns>
    	/// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
    	/// <remarks>
    	/// If more than one extension exists, then the type of the extension that is
    	/// returned is non-deterministic.  If no extensions exist that can be successfully
    	/// instantiated, an exception is thrown. Note that only extensions that are enabled
    	/// and licensed are considered.
    	/// </remarks>
    	public object CreateExtension(Predicate<ExtensionInfo> filter)
        {
            return CreateExtension(new PredicateExtensionFilter(filter));
        }

    	/// <summary>
    	/// Instantiates each available extension.
    	/// </summary>
    	/// <remarks>
    	/// Attempts to instantiate each available extension.  If an extension fails to instantiate
    	/// for any reason, the failure is logged and it is ignored. Note that only extensions that are enabled
    	/// and licensed are considered.
    	/// </remarks>
    	/// <returns>An array of references to the created extensions.  If no extensions were created
    	/// the array will be empty.</returns>
    	public object[] CreateExtensions()
        {
            return CreateExtensionsHelper(null, false);
        }

    	/// <summary>
    	/// Instantiates each available extension that also matches the specified <see cref="ExtensionFilter" />.
    	/// </summary>
    	/// <remarks>
    	/// Attempts to instantiate each matching extension.  If an extension fails to instantiate
    	/// for any reason, the failure is logged and it is ignored. Note that only extensions that are enabled
    	/// and licensed are considered.
    	/// </remarks>
    	/// <returns>An array of references to the created extensions.  If no extensions were created
    	/// the array will be empty.</returns>
    	public object[] CreateExtensions(ExtensionFilter filter)
        {
            return CreateExtensionsHelper(filter, false);
        }

    	/// <summary>
    	/// Instantiates each available extension that matches the specified filter.
    	/// </summary>
    	/// <remarks>
    	/// Attempts to instantiate each matching extension.  If an extension fails to instantiate
    	/// for any reason, the failure is logged and it is ignored. Note that only extensions that are enabled
    	/// and licensed are considered.
    	/// </remarks>
    	/// <returns>An array of references to the created extensions.  If no extensions were created
    	/// the array will be empty.</returns>
    	public object[] CreateExtensions(Predicate<ExtensionInfo> filter)
        {
            return CreateExtensions(new PredicateExtensionFilter(filter));
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method that actually performs the extension creation
        /// from an internal <see cref="IExtensionFactory"/>.
        /// </summary>
		protected object[] CreateExtensionsHelper(ExtensionFilter filter, bool justOne)
        {
            // we assume that the factory itself is thread-safe, and therefore we don't need to lock
            // (don't want to incur the cost of locking if not necessary)
            return _extensionFactory.CreateExtensions(this, filter, justOne);
        }

        /// <summary>
		/// Protected method that actually retrieves the <see cref="ExtensionInfo"/>
		/// objects from an internal <see cref="IExtensionFactory"/>.
        /// </summary>
		protected ExtensionInfo[] ListExtensionsHelper(ExtensionFilter filter)
        {
            // we assume that the factory itself is thread-safe, and therefore we don't need to lock
            // (don't want to incur the cost of locking if not necessary)
            return _extensionFactory.ListExtensions(this, filter);
        }

		/// <summary>
		/// Checks to see if there is at least one object in <paramref name="objs"/> and returns 
		/// the first one, otherwise an exception is thrown.
		/// </summary>
		/// <exception cref="NotSupportedException">Thrown if <paramref name="objs"/> is empty.</exception>
        protected object AtLeastOne(object[] objs, Type extensionPointType)
        {
            if (objs.Length > 0)
            {
                return objs[0];
            }
            else
            {
                throw new NotSupportedException(
                    string.Format(SR.ExceptionNoExtensionsCreated, extensionPointType.FullName));
            }
        }

        #endregion
    }


    /// <summary>
    /// Abstract base class for all extension points.
    /// </summary>
    /// <typeparam name="TInterface">The interface that extensions are expected to implement.</typeparam>
    /// <remarks>
    /// <para>
    /// To define an extension point, create a dedicated subclass of this class, specifying the interface
    /// that extensions are expected to implement.  The name of the subclass should be chosen
    /// with care, as the name effectively acts as a unique identifier which all extensions
    /// will reference.  Once chosen, the name should not be changed, as doing so will break all
    /// existing extensions to this extension point.  There is no need to add any methods to the subclass,
    /// and it is recommended that the class be left empty, such that it serves as a dedicated
    /// factory for creating extensions of this extension point.
    /// </para>
    /// <para>The subclass must also be marked with the <see cref="ExtensionPointAttribute" /> in order
    /// for the framework to discover it at runtime.
    /// </para>
    /// </remarks>
    public abstract class ExtensionPoint<TInterface> : ExtensionPoint
    {
        /// <summary>
        /// Gets the interface that the extension point is defined on.
        /// </summary>
        public override Type InterfaceType
        {
            get { return typeof(TInterface); }
        }
    }

}
