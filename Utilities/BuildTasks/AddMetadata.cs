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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class AddMetadata : Task
	{
		[Output]
		[Required]
		public ITaskItem[] Items { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string[] Values { get; set; }

		public override bool Execute()
		{
			if (IsNullOrEmpty(Items))
			{
				Log.LogError("Items must be supplied.");
				return false;
			}

			if (IsNullOrEmpty(Values))
			{
				Log.LogError("Values must be supplied.");
				return false;
			}

			if (Items.Length != Values.Length)
			{
				Log.LogError("Items and Values must have the same number of items.");
				return false;
			}

			var newItems = new ITaskItem[Items.Length];
			for (int n = 0; n < Items.Length; n++)
			{
				var newItem = newItems[n] = new TaskItem(Items[n]);
				newItem.SetMetadata(Name, Values[n]);
			}
			Items = newItems;
			return true;
		}

		private static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			return collection == null || collection.Count == 0;
		}
	}
}