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

        	_memoryIncrement.DataBindings.Add("Value", _component, "MemoryIncrementKB", true,
        	                                   DataSourceUpdateMode.OnPropertyChanged);

        	_heapMemory.DataBindings.Add("Value", _component, "HeapMemoryKB", true, 
				DataSourceUpdateMode.OnPropertyChanged);

			_heldMemory.DataBindings.Add("Value", _component, "HeldMemoryKB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_markedMemory.DataBindings.Add("Value", _component, "MemoryMarkKB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_memoryDifference.DataBindings.Add("Value", _component, "MemoryDifferenceKB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_largeObjectMemory.DataBindings.Add("Value", _component, "TotalLargeObjectMemoryKB", true,
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

		private void _unloadPixelData_Click(object sender, EventArgs e)
		{
			_component.UnloadPixelData();
		}
    }
}
