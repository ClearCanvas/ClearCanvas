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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration
{
	[ExtensionPoint]
	public class UserUpgradeProgressDialogViewExtensionPoint : ExtensionPoint<IUserUpgradeProgressDialogView> {}

	internal class UserUpgradeProgressDialog
	{
		private IUserUpgradeProgressDialogView _dialogView;

		private UserUpgradeProgressDialog() {}

		private void RunUpgrade()
		{
			var strategy = UserUpgradeStrategy.Create();
			if (strategy == null)
				return;

			if (strategy.TotalSteps < 5 || null == (_dialogView = CreateDialog()))
			{
				strategy.Run();
				return;
			}

			var task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
					{
						strategy.ProgressChanged += (sender, e) => context.ReportProgress(new BackgroundTaskProgress(strategy.CurrentStep - 1, strategy.TotalSteps, String.Empty));
						strategy.Run();
						context.Complete();
					}, false, strategy) { ThreadUICulture = Desktop.Application.CurrentUICulture };

			task.ProgressUpdated += (sender, e) => _dialogView.SetProgressPercent(e.Progress.Percent);
			task.Terminated += (sender, e) => _dialogView.Close(strategy.FailedCount > 0 ? SR.MessageUserUpgradeFailures : null);
			task.Run();

			_dialogView.RunModal(SR.TitleUpdatingPreferences, SR.MessageUpdatingPreferences);
			task.Dispose();
		}

		private static IUserUpgradeProgressDialogView CreateDialog()
		{
			try
			{
				return (IUserUpgradeProgressDialogView) new UserUpgradeProgressDialogViewExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException) {}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Failed to create user upgrade progress dialog view.");
			}

			return null;
		}

		public static void RunUpgradeAndShowProgress()
		{
			new UserUpgradeProgressDialog().RunUpgrade();
		}
	}

	public interface IUserUpgradeProgressDialogView : IView
	{
		void RunModal(string title, string startupMessage);
		void SetMessage(string message);
		void SetProgressPercent(int progressPercent);
		void Close(string failureMessage);
	}
}