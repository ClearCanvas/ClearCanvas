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

namespace ClearCanvas.Common.Shreds
{
	/// <summary>
	/// Abstract base implementation of <see cref="IShred"/>.  Shred implementations should inherit
	/// from this class rather than implement <see cref="IShred"/> directly.
	/// </summary>
    public abstract class Shred : MarshalByRefObject, IShred
    {
		///<summary>
		///Obtains a lifetime service object to control the lifetime policy for this instance.
		///</summary>
		public override object InitializeLifetimeService()
        {
            // cause lifetime lease to never expire
            return null;
        }

        #region IShred Members

		/// <summary>
		/// Called to start the shred.
		/// </summary>
		/// <remarks>
		/// This method should perform any initialization of the shred, and then return immediately.
		/// </remarks>
        public abstract void Start();

		/// <summary>
		/// Called to stop the shred.
		/// </summary>
		/// <remarks>
		/// This method should perform any necessary clean-up, and then return immediately.
		/// </remarks>
        public abstract void Stop();

		/// <summary>
		/// Gets the display name of the shred.
		/// </summary>
		/// <returns></returns>
        public abstract string GetDisplayName();

		/// <summary>
		/// Gets a description of the shred.
		/// </summary>
		/// <returns></returns>
        public abstract string GetDescription();

        #endregion       
    }
}
