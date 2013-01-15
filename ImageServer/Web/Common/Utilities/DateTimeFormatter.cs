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
using System.Threading;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    /// <summary>
    /// Provides unified methods to convert a datetime object into a human readable format suitable for display on a web page.
    /// </summary>
    /// <remarks>
    /// Use <see cref="DateTimeFormatter"/> to ensure consistent date/time format is generated in the entire applications. The default date/time 
    /// format is specified in the configuration or taken from the the system region setting. Caller can provide specific date/time format
    /// when calling <see cref="Format(DateTime, string)"/>.
    /// </remarks>
    /// <example>
    /// The following example illustrates how to use DateTimeFormatter to convert a <see cref="DateTime"/> object into a "dd/MM/yyyy" format
    /// <code>
    /// Console.WriteLine("Today is " + DateTimeFormatter.Format(DateTime.Today, "dd/MM/yy")
    /// </code>
    /// </example>
    /// <example>
    /// The following example illustrates how to use DateTimeFormatter to obtain a string representation of the today's date. The date format
    /// (dd/MM/yyyy) is set in the configuration.
    /// 
    /// <code>
    /// app.config:
    /// 
    /// <applicationSettings>
    ///    <ClearCanvas.ImageServer.Web.Common.UISettings>
    ///        <setting name="DateFormat" serializeAs="String">
    ///            <value>dd/MM/yyyy</value>
    ///        </setting>
    ///    </ClearCanvas.ImageServer.Web.Common.UISettings>
    /// </applicationSettings>
    /// 
    /// Console.WriteLine("Today is " + DateTimeFormatter.Format(DateTime.Today, DateTimeFormatter.Style.Date)
    /// </code>
    /// 
    /// <example>
    /// The following example illustrates how to use DateTimeFormatter to obtain a string representation of the current time. The date format
    /// (hh:mm:ss tt) is set in the configuration.
    /// 
    /// <code>
    /// app.config:
    /// 
    /// <applicationSettings>
    ///    <ClearCanvas.ImageServer.Web.Common.UISettings>
    ///        <setting name="DateFormat" serializeAs="String">
    ///            <value>hh:mm:ss tt</value>
    ///        </setting>
    ///    </ClearCanvas.ImageServer.Web.Common.UISettings>
    /// </applicationSettings>
    /// 
    /// Console.WriteLine("Now is " + DateTimeFormatter.Format(DateTime.Today, DateTimeFormatter.Style.Time)
    /// </code>
    /// 
    public class DateTimeFormatter: IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// The output date time style.
        /// </summary>
        public enum Style
        {
            DateTime, // output both date and time
            Date, // output the date only
            Time,  // output the time only
			Timestamp // Timestamp
        }

        #region Private members
        private static string _defaultDateTimeFormat; 
        private static string _defaultDateFormat; 
        private static string _defaultTimeFormat;
		private static string _defaultTimestampFormat;

        #endregion Private members


        #region Constructor

        static DateTimeFormatter()
        {
        	DateTimeFormatInfo fmt = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            
            DefaultDateFormat = null;
            if (String.IsNullOrEmpty(UISettings.Default.DateFormat))
            {
                if (fmt != null)
                {
                    DefaultDateFormat = fmt.LongDatePattern;

                }
                else
                {
                    DefaultDateFormat = "MMM dd, yyyy";

                }
            }
            else
            {
                // use the defined format
                DefaultDateFormat = UISettings.Default.DateFormat;
            }

            DefaultTimeFormat = null;
            if (String.IsNullOrEmpty(UISettings.Default.TimeFormat))
            {
                if (fmt != null)
                {
                    DefaultTimeFormat = fmt.LongTimePattern;

                }
                else
                {
                    DefaultTimeFormat = "HH:mm:ss";

                }
            }
            else
            {
                // use the defined format
                DefaultTimeFormat = UISettings.Default.TimeFormat;
            }

			DefaultTimestampFormat = null;
			if (String.IsNullOrEmpty(UISettings.Default.TimestampFormat))
			{
				if (fmt != null)
				{
					DefaultTimestampFormat = fmt.FullDateTimePattern;

				}
				else
				{
					DefaultTimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";

				}
			}
			else
			{
				// use the defined format
				DefaultTimestampFormat = UISettings.Default.TimestampFormat;
			}

            _defaultDateTimeFormat = null;
            if (String.IsNullOrEmpty(UISettings.Default.DateTimeFormat))
            {
                if (fmt != null)
                {
                    _defaultDateTimeFormat = fmt.FullDateTimePattern;

                }
                else
                {
                    _defaultDateTimeFormat = _defaultDateFormat + " " + _defaultTimeFormat;

                }    
            }
            else
            {
                // use the defined format
                _defaultDateTimeFormat = UISettings.Default.DateTimeFormat;
            }


        }

        #endregion Constructor

        #region Public Properties
        static string DefaultDateTimeFormat
        {
            get
            {
                return _defaultDateTimeFormat;
            }
            set
            {
                _defaultDateTimeFormat = value;
            }
        }

        public static string DefaultDateFormat
        {
            get { return _defaultDateFormat; }
            set { _defaultDateFormat = value; }
        }

        public static string DefaultTimeFormat
        {
            get { return _defaultTimeFormat; }
            set { _defaultTimeFormat = value; }
        }
		public static string DefaultTimestampFormat
		{
			get { return _defaultTimestampFormat; }
			set { _defaultTimestampFormat = value; }
		}
		#endregion Public Properties

        #region Public methods


        /// <summary>
        /// Formats the specified date/time
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="outputStyle"></param>
        /// <returns></returns>
        /// 
        static public string Format(DateTime dt, Style outputStyle)
        {
            DateTimeFormatter formatter = new DateTimeFormatter();
                    
            switch(outputStyle)
            {
                case Style.DateTime:
                    return Format(dt, DefaultDateTimeFormat);
                case Style.Date:
                    return Format(dt, DefaultDateFormat);
                case Style.Time:
                    return Format(dt, DefaultTimeFormat);
				case Style.Timestamp:
					return Format(dt, DefaultTimestampFormat);

                default:
                    return dt.ToString();
            }
        }

        static public string Format(DateTime dt, string format)
        {
            DateTimeFormatter formatter = new DateTimeFormatter();
            return formatter.Format(format, dt, formatter);
        }

        static public string Format(DateTime dt)
        {
            return Format(dt, DefaultDateTimeFormat);
        }

        #endregion Public methods

        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg != null && arg is DateTime)
            {
                DateTime dt = (DateTime) arg ;

                if (String.IsNullOrEmpty(format))
                    format = DefaultDateTimeFormat;

                return dt.ToString(format, formatProvider);
            }

            return null;
        }

        #endregion

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(DateTime))
                return this;
            else
                return null;
        }

        #endregion
    }
}
