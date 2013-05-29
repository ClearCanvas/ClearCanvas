#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public interface IKeyObjectSelectionDocumentInformation
	{
		KeyObjectSelectionDocumentTitle DocumentTitle { get; set; }
		string Author { get; set; }
		string Description { get; set; }
		string SeriesDescription { get; set; }
	}
}