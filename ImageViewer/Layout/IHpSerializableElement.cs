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

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Defines the interface to an object that is serializable as part of a hanging protocol.
	/// </summary>
	public interface IHpSerializableElement
	{
		/// <summary>
		/// Gets the state to be saved in the HP.
		/// </summary>
		/// <returns>An instance of a data-contract class - see <see cref="HpDataContractAttribute"/></returns>
		object GetState();

		/// <summary>
		/// Populates this element's state from a saved HP.
		/// </summary>
		/// <param name="state">An instance of a data-contract class - see <see cref="HpDataContractAttribute"/></param>
		void SetState(object state);
	}
}
