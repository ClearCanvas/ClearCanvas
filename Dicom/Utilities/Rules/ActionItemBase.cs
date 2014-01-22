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
using System.Globalization;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules.Specifications;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Defines time unitS used in server rule actions
    /// </summary>
    public enum TimeUnit
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }


    /// <summary>
    /// Exception that is thrown when value conversion fails.
    /// </summary>
    public class TypeConversionException : Exception
    {
        public TypeConversionException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Base class for all server rule actions implementing <see cref="IActionItem{T}"/> 
    /// </summary>
    public abstract class ActionItemBase<TActionContext> : IActionItem<TActionContext>
        where TActionContext : ActionContext
    {
        private string _failureReason = "Success";

        #region Constructors

        protected ActionItemBase(string name)
        {
            Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the action
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the failure when the action execution fails.
        /// </summary>
		//public string FailureReason
		//{
		//    get { return _failureReason; }
		//    set { _failureReason = value; }
		//}

        #endregion

        #region IActionItem<ServerActionContext> Members

		public virtual ActionExecuteResult Execute(TActionContext context)
        {
            try
            {
				return new ActionExecuteResult(OnExecute(context));
            }
            catch (Exception e)
            {
                return new ActionExecuteResult(false, String.Format("{0} {1}", e.Message, e.StackTrace));
            }
        }

        #region Public Static Methods

        /// <summary>
        /// Calculates the new time of the specified time, offset by a specified period.
        /// </summary>
        /// <param name="start">Starting time</param>
        /// <param name="offset">The offset period</param>
        /// <param name="unit">The unit of the offset period</param>
        /// <returns></returns>
        protected static DateTime CalculateOffsetTime(DateTime start, int offset, TimeUnit unit)
        {
            DateTime time = start;

            switch (unit)
            {
                case TimeUnit.Minutes:
                    time = time.AddMinutes(offset);
                    break;

                case TimeUnit.Hours:
                    time = time.AddHours(offset);
                    break;

                case TimeUnit.Days:
                    time = time.AddDays(offset);
                    break;

                case TimeUnit.Weeks:
                    time = time.AddDays(offset * 7);
                    break;

                case TimeUnit.Months:
                    time = time.AddMonths(offset);
                    break;

                case TimeUnit.Years:
                    time = time.AddYears(offset);
                    break;
            }

            return time;
        }


        /// <summary>
        /// Evaluates an expression in the specified context.
        /// </summary>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="context">The context to evaluate the expression</param>
        /// <returns></returns>
        protected static object Evaluate(Expression expression, TActionContext context)
        {
            Platform.CheckForNullReference(expression, "expression");
            Platform.CheckForNullReference(context, "context");
            Platform.CheckForNullReference(context.Message, "context.Message");

            return expression.Evaluate(context.Message);
        }

        /// <summary>
        /// Evaluates an expression in the specified context and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">Expected returne type</typeparam>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="context">The context to evaluate the expression</param>
        /// <returns></returns>
        /// <exception cref="TypeConversionException"> thrown if the value of the expression cannot be converted into specified type</exception>
        protected static T Evaluate<T>(Expression expression, TActionContext context)
        {
            object value = Evaluate(expression, context);

            if (value is T)
            {
                // if the expression was evaluated to the same type then just return it
                return (T)value;
            }


            if (typeof(T) == typeof(string))
            {
                return (T)(object)value.ToString();
            }
            if (typeof(T) == typeof(int))
            {
                int result = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
                return (T)(object)result;
            }
            if (typeof(T) == typeof(uint))
            {
                uint result = uint.Parse(value.ToString(), CultureInfo.InvariantCulture);
                return (T)(object)result;
            }
            if (typeof(T) == typeof(long))
            {
                long result = long.Parse(value.ToString(), CultureInfo.InvariantCulture);
                return (T)(object)result;
            }
            if (typeof(T) == typeof(ulong))
            {
                ulong result = ulong.Parse(value.ToString(), CultureInfo.InvariantCulture);
                return (T)(object)result;
            }
            if (typeof(T) == typeof(float))
            {
                float result = float.Parse(value.ToString(), CultureInfo.InvariantCulture);
                return (T)(object)result;
            }
            if (typeof(T) == typeof(double))
            {
                double result = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
                return (T)(object)result;
            }
            if (typeof(T) == typeof(DateTime))
            {
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    throw new TypeConversionException(String.Format("Unable to convert value for expression {0} (value={1}) to {2}", expression.Text,
                                      value, typeof(T)));
                }

                DateTime? result = DateTimeParser.Parse(value.ToString());
                if (result == null)
                {                  
                    throw new TypeConversionException(String.Format("Unable to convert value for expression {0} (value={1}) to {2}", expression.Text,
                                      value, typeof(T)));                    
                }
                return (T)(object)result.Value;
            }  
            throw new TypeConversionException(String.Format("Unable to retrieve value for expression {0} as {1}: Unsupported conversion.",
                              expression.Text, typeof(T)));
        }

        /// <summary>
        /// Evaluates an expression in the specified context and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">Expected returne type</typeparam>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="context">The context to evaluate the expression</param>
        /// <param name="defaultValue">Returned value if the expression cannot be evaluated</param>
        /// <returns></returns>
        protected static T Evaluate<T>(Expression expression, TActionContext context, T defaultValue)
        {
            try
            {
                return Evaluate<T>(expression, context);
            }
            catch (NoSuchDicomTagException)
            {
                // Alert?

                return defaultValue;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unable to evaluate expression {0}. Using default value={1}", expression.Text, defaultValue);
                return defaultValue;
            }
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Called to execute the action.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if the action execution succeeds. false otherwise.</returns>
        protected abstract bool OnExecute(TActionContext context);

        #endregion

        #endregion
    }
}
