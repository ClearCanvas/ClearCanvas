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
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Defines a custom table that allows user to choose a list of value for each property.
	/// </summary>
	public class ExtendedPropertyChoicesTable : DynamicCustomControlTable<ExtendedPropertyChoicesTableData>
	{
		private readonly Dictionary<ExtendedPropertyChoicesTableData, Control> _propertiesMap;

		public ExtendedPropertyChoicesTable()
		{
			_propertiesMap = new Dictionary<ExtendedPropertyChoicesTableData, Control>();

			this.ColumnCount = 2;
		}

		/// <summary>
		/// Gets a dictionary of the property name vs its current value in the control.
		/// </summary>
		public Dictionary<string, string> CurrentValues
		{
			get
			{
				var currentValues = new Dictionary<string, string>();
				CollectionUtils.ForEach(_propertiesMap,
					delegate(KeyValuePair<ExtendedPropertyChoicesTableData, Control> kvp)
						{
							var selectedItemText = kvp.Value.Text;
							currentValues.Add(kvp.Key.PropertyName, selectedItemText);
						});

				return currentValues;
			}
		}

		#region Override Methods

		protected override void ClearCustomData()
		{
			_propertiesMap.Clear();
		}

		protected override List<Control> GetRowControls(ExtendedPropertyChoicesTableData rowData)
		{
			var controls = new List<Control>();

			// Add a Label control for field Name column
			var fieldControlName = GetUniqueControlName("Field", rowData.PropertyName);
			var fieldControl = DynamicTableCommonControlFactory.CreateLabelControl(fieldControlName, rowData.PropertyName);
			controls.Add(fieldControl);

			var valueControlName = GetUniqueControlName("Value", rowData.PropertyName);
			var valueControl = DynamicTableCommonControlFactory.CreateComboBoxControl(valueControlName, rowData.ValueChoices[0], rowData.ValueChoices);
			controls.Add(valueControl);

			_propertiesMap.Add(rowData, valueControl);

			return controls;
		}

		private static string GetUniqueControlName(string column, string name)
		{
			return string.Format("{0}_{1}", column, name);
		}
		
		#endregion
	}
}
