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

#pragma warning disable 1591
// ReSharper disable LocalizableElement

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TestComponent"/>.
    /// </summary>
    public partial class TestComponentControl : ApplicationComponentUserControl
    {
        private TestComponent _component;
    	private static volatile bool _crashed;
		private static volatile bool _reportCrash;
    	private static int _crashAttemptCount;
		private static int _crashCount;
		private static int _circumventDialogCount;
		private static SynchronizationContext _syncContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestComponentControl(TestComponent component)
            :base(component)
        {
            InitializeComponent();

			if (_syncContext == null)
				_syncContext = SynchronizationContext.Current;

            _component = component;

            _label.DataBindings.Add("Text", _component, "Name");
            _text.DataBindings.Add("Text", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
        }

    	private TimeSpan CrashDelay
    	{
			get { return _delayCrash.Checked ? TimeSpan.FromSeconds((int)_crashDelay.Value) : TimeSpan.Zero; }	
		}

		private void _showMessageBox_Click(object sender, EventArgs e)
        {
            _component.ShowMessageBox();
        }

        private void _showDialogBox_Click(object sender, EventArgs e)
        {
            _component.ShowDialogBox();
        	CircumventCrash();
		}

        private void _close_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _setTitle_Click(object sender, EventArgs e)
        {
            _component.SetTitle();
        }

        private void _modify_Click(object sender, EventArgs e)
        {
            _component.Modify();
        }

        private void _accept_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

		private void _showWorkspaceDialogBox_Click(object sender, EventArgs e)
		{
			_component.ShowWorkspaceDialogBox();
			CircumventCrash();
		}

		private void _buttonCrashUI_Click(object sender, EventArgs e)
		{
			Crash(CrashDelay, true);
		}

		private void _crashThreadPool_Click(object sender, EventArgs e)
		{
			Crash(CrashDelay, false);
		}

		private void _buttonCrashThread_Click(object sender, EventArgs e)
		{
			CrashWorkerThread(CrashDelay);
		}

		private void Crash(TimeSpan delay, bool crashUIThread)
		{
			_reportCrash = _catchAndReport.Checked;
			Platform.Log(LogLevel.Info, "Crash attempt #{0} ({1})", Interlocked.Increment(ref _crashAttemptCount), crashUIThread ? "UI Thread" : "Thread Pool");
	
			ThreadPool.QueueUserWorkItem(delegate
			                             	{
			                             		Thread.Sleep(delay);
			                             		if (crashUIThread)
			                             			_syncContext.Post(delegate { Throw(); }, null);
			                             		else
			                             			Throw();
			                             	}, null);
		}

		private void CrashWorkerThread(TimeSpan delay)
		{
			Platform.Log(LogLevel.Info, "Crash attempt #{0} (Worker Thread)", Interlocked.Increment(ref _crashAttemptCount));
			
			_reportCrash = _catchAndReport.Checked;
			var thread = new Thread(delegate(object obj)
			{
				Thread.Sleep(delay);
				Throw();
			});

			thread.Start();
		}

		private static void Throw()
		{
			_crashed = true;

			try
			{
				throw new Exception(String.Format("Crash #{0}", Interlocked.Increment(ref _crashCount)));
			}
			catch (Exception e)
			{
				if (!_reportCrash)
					throw;

				ExceptionHandler.ReportUnhandled(e);
			}
		}
	
		private void CircumventCrash()
		{
			if (!_crashed || !_circumventCrash.Checked)
				return;

			Platform.Log(LogLevel.Info, "Circumvent dialog {0}", ++_circumventDialogCount);
			var result = System.Windows.Forms.MessageBox.Show("Circumvent!");
			Debug.Assert(result == System.Windows.Forms.DialogResult.OK);

			CrashWorkerThread(TimeSpan.Zero);
			Crash(TimeSpan.Zero, false);

			//show some random form.
			Platform.Log(LogLevel.Info, "Circumvent dialog {0}", ++_circumventDialogCount);
			Form form = new Form();
			result = form.ShowDialog();
			Debug.Assert(result == System.Windows.Forms.DialogResult.Cancel);

			Crash(TimeSpan.Zero, true);
		}

		private void _infoAlertButton_Click(object sender, EventArgs e)
		{
			_component.AlertInfo();
		}

		private void _warningAlertButton_Click(object sender, EventArgs e)
		{
			_component.AlertWarning();
		}

		private void _errorAlertButton_Click(object sender, EventArgs e)
		{
			_component.AlertError();
		}
	}
}

// ReSharper restore LocalizableElement