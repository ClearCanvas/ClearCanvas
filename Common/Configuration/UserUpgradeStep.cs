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
using System.Collections;

namespace ClearCanvas.Common.Configuration
{
	public class UserUpgradeStepFactoryExtensionPoint : ExtensionPoint<IUserUpgradeStepFactory>
	{
	}

	public interface IUserUpgradeStepFactory
	{
		ICollection<UserUpgradeStep> CreateSteps();
	}

	public abstract class UserUpgradeStep
	{
		[ThreadStatic] private static Stack<string> _stepsInProgress;

		private static Stack<string> StepsInProgress
		{
			get
			{
				if (_stepsInProgress == null)
					_stepsInProgress = new Stack<string>();
				return _stepsInProgress;
			}	
		}

		private bool IsInProgress
		{
			get { return StepsInProgress.Contains(Identifier); }
		}

		#region IUserUpgradeStep Members

		public abstract string Identifier { get; }

		public bool Run()
		{
			UpgradeSettings.CheckUserUpgradeEnabled();

			if (IsInProgress || IsCompleted)
				return false;

			StepsInProgress.Push(Identifier);

			try
			{
				bool success = PerformUpgrade();
				OnCompleted();
				return success;
			}
			finally
			{
				StepsInProgress.Pop();
			}
		}

		#endregion

		protected abstract bool PerformUpgrade();

		#region Internal

		private bool IsCompleted
		{
			get { return UpgradeSettings.Default.IsUserUpgradeStepCompleted(Identifier); }
		}

		private void OnCompleted()
		{
			UpgradeSettings.Default.OnUserUpgradeStepCompleted(Identifier);
		}

		#region Static Factory

		internal static ICollection<UserUpgradeStep> CreateAll()
		{
			List<UserUpgradeStep> steps = new List<UserUpgradeStep>();

			try
			{
				foreach (IUserUpgradeStepFactory factory in new UserUpgradeStepFactoryExtensionPoint().CreateExtensions())
				{
					foreach (UserUpgradeStep step in factory.CreateSteps())
					{
						if (!step.IsCompleted)
							steps.Add(step);
					}
				}
			}
			catch (NotSupportedException)
			{
			}

            foreach (UserUpgradeStep step in new UserSettingsUpgradeStepFactory().CreateSteps())
            {
                if (!step.IsCompleted)
                    steps.Add(step);
            }
            
            return steps;
		}

		#endregion
		#endregion
	}
}