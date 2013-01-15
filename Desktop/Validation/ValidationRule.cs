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
using ClearCanvas.Common.Specifications;
using System.Xml;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Default implementation of <see cref="IValidationRule"/>.
    /// </summary>
    public class ValidationRule : IValidationRule
    {
		/// <summary>
		/// Delegate used to validate an <see cref="IApplicationComponent"/> and return the 
		/// results as a <see cref="TestResult"/>.
		/// </summary>
        public delegate ValidationResult ValidationDelegate(IApplicationComponent component);

        private string _propertyName;
        private ValidationDelegate _callback;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The property to which this rule applies.</param>
        /// <param name="callback">A method that performs the validation.</param>
        public ValidationRule(string propertyName, ValidationDelegate callback)
        {
            Platform.CheckForNullReference(propertyName, "propertyName");
            Platform.CheckForNullReference(callback, "callback");

            _propertyName = propertyName;
            _callback = callback;
        }

        /// <summary>
        /// Constructor that accepts an instance of a <see cref="ISpecification"/>.
        /// </summary>
        /// <remarks>
		/// The specification defines the validation rule.
		/// </remarks>
        /// <param name="propertyName">The property to which the rule applies.</param>
        /// <param name="spec">The specification to use to evaluate the rule.</param>
        public ValidationRule(string propertyName, ISpecification spec)
            : this(propertyName, (new SpecificationEvaluator(spec)).Evaluate)
        {
        }

        #region IValidationRule members

        /// <summary>
        /// Gets the name of the property on the application component that this rule applies to.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Obtains the current result of evaluating this rule based on the current state of the application component.
        /// </summary>
        public ValidationResult GetResult(IApplicationComponent component)
        {
            return _callback(component);
        }

        #endregion

        #region SpecificationEvaluator

        /// <summary>
        /// Helper class for evaluating a specification as a validation rule.
        /// </summary>
        private class SpecificationEvaluator
        {
            private ISpecification _specification;

            public SpecificationEvaluator(ISpecification specification)
            {
                _specification = specification;
            }

            public ValidationResult Evaluate(IApplicationComponent component)
            {
                try
                {
                    TestResult result = _specification.Test(component);
                    return new ValidationResult(result.Success, result.Success ? null : GetTopLevelMessage(result.Reasons));
                }
                catch (Exception e)
                {
                    // if the evaluation of the specification throws an exception, 
                    // treat it as a failed validation and return the exception message
                    // this will assist the administrator in correcting the specification XML
                    return new ValidationResult(false, string.Format("Error evaluating validation rule: {0}", e.Message));
                }
            }

            private static string GetTopLevelMessage(TestResultReason[] reasons)
            {
                if (reasons.Length == 0)
                    return null;

                string message = reasons[0].Message;
                return (!string.IsNullOrEmpty(message)) ? message : GetTopLevelMessage(reasons[0].Reasons);
            }
        }

        #endregion
    }
}
