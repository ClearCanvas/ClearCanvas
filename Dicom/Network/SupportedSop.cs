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

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Structure used to represent the supported SOP Classes for Scu/Scp operations.
    /// </summary>
    public struct SupportedSop
    {
        private SopClass _sopClass;
        private IList<TransferSyntax> _syntaxList;

        /// <summary>
        /// The <see cref="ClearCanvas.Dicom.SopClass"/> instance supported.
        /// </summary>
        public SopClass SopClass
        {
            get { return _sopClass; }
            set { _sopClass = value; }
        }

        /// <summary>
        /// A list of transfer syntaxes supported by the <see cref="SopClass"/>.
        /// </summary>
        public IList<TransferSyntax> SyntaxList
        {
            get {
                if (_syntaxList == null)
                    _syntaxList = new List<TransferSyntax>();
                return _syntaxList; 
            }
        }

        /// <summary>
        /// Used to add a supported transfer syntax.
        /// </summary>
        /// <param name="syntax">The transfer syntax supproted by the SOP Class.</param>
        public void AddSyntax(TransferSyntax syntax)
        {
            SyntaxList.Add(syntax);
        }
    }
}