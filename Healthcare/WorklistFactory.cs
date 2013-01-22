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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Manages instantiation of worklists.
	/// </summary>
	public class WorklistFactory
	{
		#region Static members

		private static readonly WorklistFactory _theInstance = new WorklistFactory();

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static WorklistFactory Instance
		{
			get { return _theInstance; }
		}

		#endregion

		private readonly List<Type> _worklistClasses;

		private WorklistFactory()
		{
			_worklistClasses = CollectionUtils.Map(new WorklistExtensionPoint().ListExtensions(), (ExtensionInfo info) => info.ExtensionClass.Resolve());
		}

		/// <summary>
		/// Lists all known worklist classes (extensions of <see cref="WorklistExtensionPoint"/>), optionally
		/// including those marked with the <see cref="StaticWorklistAttribute"/>.
		/// </summary>
		/// <param name="includeStatic"></param>
		/// <returns></returns>
		public Type[] ListWorklistClasses(bool includeStatic)
		{
			return includeStatic ? _worklistClasses.ToArray() :
				CollectionUtils.Select(_worklistClasses, wc => !Worklist.GetIsStatic(wc)).ToArray();
		}

		/// <summary>
		/// Creates an instance of the worklist class as specified by the class name, which may be fully or
		/// only partially qualified.
		/// </summary>
		/// <param name="worklistClassName"></param>
		/// <returns></returns>
		public Worklist CreateWorklist(string worklistClassName)
		{
			return CreateWorklist(ResolvePartialClassName(worklistClassName));
		}

		/// <summary>
		/// Creates an instance of the specified worklist class.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public Worklist CreateWorklist(Type worklistClass)
		{
			return (Worklist)Activator.CreateInstance(worklistClass);
		}


		private Type ResolvePartialClassName(string worklistClassName)
		{
			var worklistClass = CollectionUtils.SelectFirst(_worklistClasses, t => t.FullName.Contains(worklistClassName));

			if (worklistClass == null)
				throw new ArgumentException(string.Format("{0} is not a valid worklist class name.", worklistClassName));

			return worklistClass;
		}
	}
}