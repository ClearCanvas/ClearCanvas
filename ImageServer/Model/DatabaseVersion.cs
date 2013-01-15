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

namespace ClearCanvas.ImageServer.Model
{
	public partial class DatabaseVersion
	{
		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;

			DatabaseVersion a = (DatabaseVersion)obj;

			return a.Build.Equals(Build) 
				&& a.Minor.Equals(Minor)
				&& a.Major.Equals(Major)
				&& a.Revision.Equals(Revision);
		}

		public override int GetHashCode()
		{
			return Build.GetHashCode() + Minor.GetHashCode() + Major.GetHashCode() + Revision.GetHashCode();
		}

		public string GetVersionString()
		{
			return string.Format("{0}.{1}.{2}.{3}", Major ?? "0", Minor ?? "0", Build ?? "0", Revision ?? "0");
		}
	}
}
