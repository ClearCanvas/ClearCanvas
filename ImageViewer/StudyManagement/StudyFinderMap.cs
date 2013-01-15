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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a study finder extension point.
	/// </summary>
	[ExtensionPoint()]
	public sealed class StudyFinderExtensionPoint : ExtensionPoint<IStudyFinder>
    {
    }

	/// <summary>
	/// Exception thrown when an <see cref="IStudyFinder"/> with the specified
	/// <see cref="FinderName">name</see> could not be found.
	/// </summary>
	public class StudyFinderNotFoundException : Exception
	{
		internal StudyFinderNotFoundException(string name)
		{
			FinderName = name;
		}

		/// <summary>
		/// Gets the name of the requested <see cref="IStudyFinder"/>.
		/// </summary>
		public readonly string FinderName;
	}

	/// <summary>
	/// A map of <see cref="IStudyFinder"/> objects.
	/// </summary>
    internal sealed class StudyFinderMap : IEnumerable
	{
        private readonly Dictionary<string, IStudyFinder> _studyFinderMap = new Dictionary<string, IStudyFinder>();
		private static readonly Dictionary<string, string> _supportedStudyFinders;

		internal StudyFinderMap()
		{
			CreateStudyFinders();
		}

		static StudyFinderMap()
		{
			StudyFinderMap map = new StudyFinderMap();
			_supportedStudyFinders = new Dictionary<string, string>();
			foreach (IStudyFinder finder in map._studyFinderMap.Values)
				_supportedStudyFinders[finder.Name] = finder.Name;
		}

		public static bool IsStudyFinderSupported(string studyFinderName)
		{
			Platform.CheckForEmptyString(studyFinderName, "studyFinderName");
			return _supportedStudyFinders.ContainsKey(studyFinderName);
		}

		/// <summary>
		/// Gets the <see cref="IStudyFinder"/> with the specified name.
		/// </summary>
		/// <param name="studyFinderName"></param>
		/// <returns></returns>
        public IStudyFinder this[string studyFinderName]
		{
			get
			{
				Platform.CheckForEmptyString(studyFinderName, "studyFinderName");

				if (!_studyFinderMap.ContainsKey(studyFinderName))
					throw new StudyFinderNotFoundException(studyFinderName);

				return _studyFinderMap[studyFinderName];
			}
		}

		private void CreateStudyFinders()
		{
            try
            {
				StudyFinderExtensionPoint xp = new StudyFinderExtensionPoint();
				object[] studyFinders = xp.CreateExtensions();

				foreach (IStudyFinder studyFinder in studyFinders)
					_studyFinderMap.Add(studyFinder.Name, studyFinder);
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _studyFinderMap.GetEnumerator();
		}

		#endregion
	}
}
