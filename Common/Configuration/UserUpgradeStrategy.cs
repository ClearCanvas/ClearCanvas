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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	public interface IUserUpgradeStrategy
	{
		void Run();
		bool IsRunning { get; }

		int TotalSteps { get; }
		int CurrentStep { get; }

		int SuccessfulCount { get; }
		int FailedCount { get; }
		int RemainingCount { get; }

		event EventHandler ProgressChanged;
	}

	public class UserUpgradeStrategy : IUserUpgradeStrategy
	{
		private event EventHandler _progressChanged;

		private UserUpgradeStrategy(ICollection<UserUpgradeStep> steps)
		{
			Steps = steps;
		}

		private ICollection<UserUpgradeStep> Steps { get; set; }
	
		public int TotalSteps { get { return Steps.Count; } }
		public int CurrentStep { get; private set; }

		public int SuccessfulCount { get; private set; }
		public int FailedCount { get; private set; }
		public int RemainingCount { get; private set; }
	
		public event EventHandler ProgressChanged
		{
			add { _progressChanged += value; }
			remove { _progressChanged -= value; }
		}

		public static IUserUpgradeStrategy Create()
		{
			if (!UpgradeSettings.IsUserUpgradeEnabled())
				return null;

			ICollection<UserUpgradeStep> steps = UserUpgradeStep.CreateAll();
			if (steps.Count == 0)
				return null;

			return new UserUpgradeStrategy(steps);
		}

		public bool IsRunning { get; private set; }

		public void Run()
		{
			if (IsRunning)
				return;

			IsRunning = true;
			
			foreach (UserUpgradeStep step in Steps)
			{
				try
				{
					++CurrentStep;
					step.Run();
					++SuccessfulCount;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "User upgrade step failed: {0}", step.Identifier);
					++FailedCount;
				}
				finally
				{
					--RemainingCount;
					EventsHelper.Fire(_progressChanged, this, EventArgs.Empty);
				}
			}

			IsRunning = false;
			EventsHelper.Fire(_progressChanged, this, EventArgs.Empty);
		}
	}
}