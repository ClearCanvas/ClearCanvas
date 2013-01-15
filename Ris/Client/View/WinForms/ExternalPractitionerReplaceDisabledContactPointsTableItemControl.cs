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
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class ExternalPractitionerReplaceDisabledContactPointsTableItemControl : UserControl
	{
		private readonly ExternalPractitionerReplaceDisabledContactPointsTableItem _item;

		public ExternalPractitionerReplaceDisabledContactPointsTableItemControl(ExternalPractitionerReplaceDisabledContactPointsTableItem item)
		{
			InitializeComponent();
			_item = item;

			_oldContactPointName.DataBindings.Add("Text", _item, "OldContactPointName", true, DataSourceUpdateMode.OnPropertyChanged);
			_oldContactPointInfo.DataBindings.Add("Text", _item, "OldContactPointInfo", true, DataSourceUpdateMode.OnPropertyChanged);
			_newContactPointInfo.DataBindings.Add("Text", _item, "NewContactPointInfo", true, DataSourceUpdateMode.OnPropertyChanged);

			_replacedWith.DataSource = _item.NewContactPointChoices;
			_replacedWith.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _item.FormatNewContactPointChoice(args.ListItem); };
			_replacedWith.DataBindings.Add("Value", _item, "SelectedNewContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}

	public class ExternalPractitionerReplaceDisabledContactPointsTable : DynamicCustomControlTable<ExternalPractitionerReplaceDisabledContactPointsTableItem>
	{
		public ExternalPractitionerReplaceDisabledContactPointsTable()
		{
			this.ColumnCount = 1;
		}

		#region Overrides of DynamicCustomControlTable<ExternalPractitionerReplaceDisabledContactPointsTableItem>

		protected override void ClearCustomData()
		{
			// Nothing to do.
		}

		protected override List<Control> GetRowControls(ExternalPractitionerReplaceDisabledContactPointsTableItem rowData)
		{
			return new List<Control> { CreateCustomeControl(rowData) };
		}

		#endregion

		private ExternalPractitionerReplaceDisabledContactPointsTableItemControl CreateCustomeControl(ExternalPractitionerReplaceDisabledContactPointsTableItem item)
		{
			return new ExternalPractitionerReplaceDisabledContactPointsTableItemControl(item)
			{
				Name = GetUniqueControlName(item),
				Dock = DockStyle.Fill
			};
		}

		private static string GetUniqueControlName(ExternalPractitionerReplaceDisabledContactPointsTableItem item)
		{
			return string.Format("{0}", item.OldContactPoint.Name);
		}
	}
}
