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
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Rules engine for applying rules against DICOM files and performing actions.
    /// </summary>
    /// <remarks>
    /// The SRulesEngine encapsulates code to apply rules against DICOM file 
    /// objects.  It will load the rules from the persistent store, maintain them by type,
    /// and then can apply them against specific files.
    /// </remarks>
    /// <seealso cref="ActionContext"/>
    /// <example>
    /// Here is an example rule for routing all images with Modality set to CT to an AE
    /// Title CLEARCANVAS.
    /// <code>
    /// <rule id="CT Rule">
    ///   <condition expressionLanguage="dicom">
    ///     <equal test="$Modality" refValue="CT"/>
    ///   </condition>
    ///   <action>
    ///     <auto-route device="CLEARCANVAS"/>
    ///   </action>
    /// </rule>
    /// </code>
    /// </example>
    public class RulesEngine<TActionContext, TTypeEnum>
        where TActionContext : ActionContext
    {
        protected readonly List<TTypeEnum> _omitList = new List<TTypeEnum>();
        protected readonly List<TTypeEnum> _includeList = new List<TTypeEnum>();
        protected readonly Dictionary<TTypeEnum, RuleTypeCollection<TActionContext, TTypeEnum>> _typeList =
            new Dictionary<TTypeEnum, RuleTypeCollection<TActionContext, TTypeEnum>>();

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="RulesEngineStatistics"/> for the rules engine.
        /// </summary>
        public RulesEngineStatistics Statistics { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a specific {TTypeEnum} to be omitted from the rules engine.
        /// </summary>
        /// <remarks>
        /// This method can be called multiple times, however, the <see cref="AddIncludeType"/> method
        /// cannot be called if this method has already been called.  Note that this method must be 
        /// called before <see cref="Load"/> to have an affect.
        /// </remarks>
        /// <param name="type">The type to omit</param>
        public void AddOmittedType(TTypeEnum type)
        {
            if (_includeList.Count > 0)
                throw new ApplicationException("Include list already has values, cannot add ommitted type.");
            _omitList.Add(type);
        }

        /// <summary>
        /// Limit the rules engine to only include specific Type types.
        /// </summary>
        /// <remarks>
        /// This methad can be called multiple times to include multiple types, however, the
        /// <see cref="AddOmittedType"/> method cannot be called if this method has already been 
        /// called.  Note that this method must be called before <see cref="Load"/> to have an affect.
        /// </remarks>
        /// <param name="type">The type to incude.</param>
        public void AddIncludeType(TTypeEnum type)
        {
            if (_omitList.Count > 0)
                throw new ApplicationException("Omitted list already has values, cannot add included type.");
            _includeList.Add(type);
        }


        /// <summary>
        /// Execute the rules against the context for the rules.
        /// </summary>
        /// <param name="context">A class containing the context for applying the rules.</param>
        public virtual void Execute(TActionContext context)
        {
            Statistics.ExecutionTime.Start();

            foreach (RuleTypeCollection<TActionContext, TTypeEnum> typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context, false);
            }

            Statistics.ExecutionTime.End();
        }

        /// <summary>
        /// Execute the rules against the context for the rules.
        /// </summary>
        /// <param name="context">A class containing the context for applying the rules.</param>
        /// <param name="stopOnFirst">Stop on first valid rule of type.</param>
        public void Execute(TActionContext context, bool stopOnFirst)
        {
            Statistics.ExecutionTime.Start();

            foreach (RuleTypeCollection<TActionContext, TTypeEnum> typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context, stopOnFirst);
            }

            Statistics.ExecutionTime.End();
        }
        #endregion
    }
}