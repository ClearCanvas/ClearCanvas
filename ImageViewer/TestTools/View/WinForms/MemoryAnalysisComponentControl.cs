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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="MemoryAnalysisComponent"/>.
    /// </summary>
    public partial class MemoryAnalysisComponentControl : ApplicationComponentUserControl
    {
        private MemoryAnalysisComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MemoryAnalysisComponentControl(MemoryAnalysisComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

            BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

            _systemFreeMemory.DataBindings.Add("Value", _component, "SystemFreeMemoryMB", true,
                                               DataSourceUpdateMode.OnPropertyChanged);

            _processVirtualMemory.DataBindings.Add("Value", _component, "ProcessVirtualMemoryMB", true,
                                               DataSourceUpdateMode.OnPropertyChanged);

            _processPrivateBytes.DataBindings.Add("Value", _component, "ProcessPrivateBytesMB", true,
                                               DataSourceUpdateMode.OnPropertyChanged);

            _processWorkingSet.DataBindings.Add("Value", _component, "ProcessWorkingSetMB", true,
                                               DataSourceUpdateMode.OnPropertyChanged);

            _largeObjectBufferSize.DataBindings.Add("Value", _component, "LargeObjectBufferSizeKB", true,
                                               DataSourceUpdateMode.OnPropertyChanged);

            _largeObjectRepeatCount.DataBindings.Add("Value", _component, "LargeObjectRepeatCount", true,
                                   DataSourceUpdateMode.OnPropertyChanged);

            _largeObjectsHeldMemory.DataBindings.Add("Value", _component, "TotalLargeObjectMemoryMB", true,
                                                     DataSourceUpdateMode.OnPropertyChanged);

            _heldMemoryIncrement.DataBindings.Add("Value", _component, "MemoryIncrementKB", true,
        	                                   DataSourceUpdateMode.OnPropertyChanged);

            _heldMemoryRepeatCount.DataBindings.Add("Value", _component, "MemoryRepeatCount", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

        	_heapMemory.DataBindings.Add("Value", _component, "HeapMemoryMB", true, 
				DataSourceUpdateMode.OnPropertyChanged);

			_heldMemory.DataBindings.Add("Value", _component, "HeldMemoryMB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_markedMemory.DataBindings.Add("Value", _component, "MemoryMarkMB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_memoryDifference.DataBindings.Add("Value", _component, "MemoryDifferenceMB", true,
				DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _consumeMaxMemory_Click(object sender, EventArgs e)
		{
			_component.ConsumeMaximumMemory();
		}

		private void _consumeIncrement_Click(object sender, EventArgs e)
		{
			_component.AddHeldMemory();
		}

		private void _releaseHeldMemory_Click(object sender, EventArgs e)
		{
			_component.ReleaseHeldMemory();
		}

		private void _collect_Click(object sender, EventArgs e)
		{
			Cursor old = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			_component.Collect();
			Cursor.Current = old;
		}

		private void _markMemory_Click(object sender, EventArgs e)
		{
			_component.MarkMemory();
		}

        private void _consumeLarge_Click(object sender, EventArgs e)
        {
            _component.AddLargeObjects();
        }

        private void _releaseLarge_Click(object sender, EventArgs e)
        {
            _component.ReleaseHeldLargeObjects();
        }

        private void _useHeldMemory_Click(object sender, EventArgs e)
        {
            _component.UseHeldMemory();
        }

        private void _useLargeObjects_Click(object sender, EventArgs e)
        {
            _component.ReleaseAllLargeObjects();
        }
    }
}
