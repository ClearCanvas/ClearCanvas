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

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// Hanging Protocol Iod.
    /// </summary>
    /// /// <remarks>
    /// <para>As defined in the DICOM Standard 2011, Part 3, Section A.44.3 (Table A.44.3-1)</para>
    /// </remarks>
    public class HangingProtocolIod
    {
        private readonly IDicomAttributeProvider _dicomAttributeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolIod"/> class.
        /// </summary>	
        public HangingProtocolIod() : this(new DicomAttributeCollection()) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="HangingProtocolIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
        public HangingProtocolIod(IDicomAttributeProvider provider)
		{
			_dicomAttributeProvider = provider;

            this.HangingProtocolDefinition = new HangingProtocolDefinitionModuleIod(_dicomAttributeProvider);
            this.HangingProtocolDisplay = new HangingProtocolDisplayModuleIod(_dicomAttributeProvider);
            this.HangingProtocolEnvironment = new HangingProtocolEnvironmentModuleIod(_dicomAttributeProvider);
			this.SopCommon = new SopCommonModuleIod(_dicomAttributeProvider);
		}

		public IDicomAttributeProvider DicomAttributeProvider
		{
			get { return _dicomAttributeProvider; }
		}

        #region Hanging Protocol IE

        /// <summary>
        /// Gets the Hanging Protocol Definition module (required usage).
        /// </summary>
        public readonly HangingProtocolDefinitionModuleIod HangingProtocolDefinition;

        /// <summary>
        /// Gets the Hanging Protocol Environment module (required usage).
        /// </summary>
        public readonly HangingProtocolEnvironmentModuleIod HangingProtocolEnvironment;

        /// <summary>
        /// Gets the Hanging Protocol Display module (required usage).
        /// </summary>
        public readonly HangingProtocolDisplayModuleIod HangingProtocolDisplay;

        /// <summary>
        /// Gets the Sop Common module (required usage).
        /// </summary>
        public readonly SopCommonModuleIod SopCommon;

        #endregion
    }
}
