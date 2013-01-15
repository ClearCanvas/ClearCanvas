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

namespace ClearCanvas.Dicom
{
	/// <summary>
    /// Specifies an interface that allows an object to have
    /// DICOM properties set to specific values. An example where
    /// this interface may be used is in the loading of DICOM image
    /// files from disk, but where the header may be stored in a
    /// fast-access database. The database may use this interface
    /// to set all the properties of the loaded image, from the set
    /// of properties stored in the database, 
    /// while knowing nothing about the type of
    /// of the image object, other than that it implements this 
    /// interface.
    /// </summary>
    public interface IDicomPropertySettable
    {
        void SetStringProperty(String propertyName, String value);
        void SetIntProperty(String propertyName, int value);
        void SetUintProperty(String propertyName, uint value);
        void SetDoubleProperty(String propertyName, Double value);
    }
}
