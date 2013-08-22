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
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model.EntityBrokers;
namespace ClearCanvas.ImageServer.Model
{
	public partial class ServiceLock
	{
		#region Private Members
		private Filesystem _filesystem;
		#endregion

		#region Public Properties
		public Filesystem Filesystem
		{
			get
			{
				if (FilesystemKey == null)
					return null;
				if (_filesystem == null)
					_filesystem = Filesystem.Load(FilesystemKey);
				return _filesystem;
			}
		}
		#endregion

        /// <summary>
        /// Finds all <see cref="ServiceLock"/> of the specified <see cref="ServiceLockTypeEnum"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<ServiceLock> FindServicesOfType(ServiceLockTypeEnum type)
        {
            using (var context = new ServerExecutionContext())
            {
                var broker = context.ReadContext.GetBroker<IServiceLockEntityBroker>();
                var criteria = new ServiceLockSelectCriteria();
                criteria.ServiceLockTypeEnum.EqualTo(type);
                return broker.Find(criteria);
            }
        }
	}
}
