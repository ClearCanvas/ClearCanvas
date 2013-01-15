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
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    partial class StudyIntegrityQueue
    {
        private Study _study;
        private readonly object _syncLock = new object();
        protected StudyStorage _studyStorage;


        /// <summary>
        /// Loads the related <see cref="Study"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Study LoadStudy(IPersistenceContext context)
        {
            StudyStorage storage = LoadStudyStorage(context);

            if (_study == null)
            {
                lock (_syncLock)
                {
                    _study = storage.LoadStudy(context);
                }
            }
            return _study;
        }

        /// <summary>
        /// Loads the related <see cref="StudyStorage"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            if (_studyStorage == null)
            {
                lock (_syncLock)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey);
                }
            }
            return _studyStorage;
        }
    }
}
