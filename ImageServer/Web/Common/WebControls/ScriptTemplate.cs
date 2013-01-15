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
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Provides convenience mean to load a javascript template from embedded resource.
    /// </summary>
    public  class ScriptTemplate
    {
        #region Private Members
        private String _script;

        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ScriptTemplate"/>
        /// </summary>
        /// <param name="validator">The validator to which the validator belongs.</param>
        /// <param name="name">Fully-qualified name of the javascript template (including the namespace)</param>
        /// <remarks>
        /// 
        /// </remarks>
        public ScriptTemplate(BaseValidator validator,  string name):
            this(validator.GetType().Assembly, name)
        {
            Replace("@@CLIENTID@@", validator.ClientID);
            Replace("@@INPUT_NAME@@", validator.InputName);
            Replace("@@INPUT_CLIENTID@@", validator.InputControl.ClientID);
            Replace("@@INPUT_NORMAL_BKCOLOR@@", ColorTranslator.ToHtml(validator.InputNormalColor));
            Replace("@@INPUT_INVALID_BKCOLOR@@", ColorTranslator.ToHtml(validator.InvalidInputColor));
            Replace("@@INPUT_NORMAL_BORDERCOLOR@@", ColorTranslator.ToHtml(validator.InputNormalBorderColor));
            Replace("@@INPUT_INVALID_BORDERCOLOR@@", ColorTranslator.ToHtml(validator.InvalidInputBorderColor));
            Replace("@@INPUT_NORMAL_CSS@@", validator.InputNormalCSS);
            Replace("@@INPUT_INVALID_CSS@@", validator.InvalidInputCSS);            
            Replace("@@INVALID_INPUT_INDICATOR_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.Container.ClientID);
            Replace("@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.TooltipLabel.ClientID);
            Replace("@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.TooltipLabelContainer.ClientID);
            Replace("@@ERROR_MESSAGE@@", Escape(validator.Text));
            Replace("@@IGNORE_EMPTY_VALUE@@", validator.IgnoreEmptyValue? "true":"false");
            
        }


        public ScriptTemplate(Assembly assembly, string name)
        {
            Stream stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
                throw new Exception(String.Format("Resource not found: {0}", name));
            StreamReader reader = new StreamReader(stream);
            _script = reader.ReadToEnd();
            stream.Close();
            reader.Dispose();

        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        public string Script
        {
            get { return _script; }
        }

        #endregion Public properties

        #region Public Methods

        /// <summary>
        /// Replaces a token in the script with the specified value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Replace(string key, string value)
        {
            _script = _script.Replace(key, value);
        }

        #endregion Public Methods



        /// <summary>
        /// Escapes special character in a text so that it can be used as a string in javascript.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Replace("'", "\\'");
        }
    }
}
