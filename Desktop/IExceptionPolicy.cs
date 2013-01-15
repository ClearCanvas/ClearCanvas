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

namespace ClearCanvas.Desktop
{
    ///<summary>
	/// Extension point for <see cref="IExceptionPolicy"/>s.
    ///</summary>
    [ExtensionPoint]
    public sealed class ExceptionPolicyExtensionPoint : ExtensionPoint<IExceptionPolicy> { }

    /// <summary>
    /// Provides Exception specific handling policies.
    /// </summary>
    /// <example>
    /// <code>
    /// [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    /// [ExceptionPolicyFor(typeof(FooException))]
    /// public class FooExceptionPolicy : IExceptionPolicy
    /// {
    ///     ...
    /// }
    /// </code>
    /// </example>
    public interface IExceptionPolicy
    {
        ///<summary>
        /// Handles the specified exception.
        ///</summary>
        void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext);
    }

    ///<summary>
    /// Specifies an exception type to which an <see cref="IExceptionPolicy"/> applies.
    ///</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExceptionPolicyForAttribute : Attribute
    {
        private readonly Type _exceptionType;

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="exceptionType">The type of exception the policy is for.</param>
        public ExceptionPolicyForAttribute(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        ///<summary>
        /// Gets the type of exception the policy is for.
        ///</summary>
        public Type ExceptionType
        {
            get { return _exceptionType; }
        }
    }
}

