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
using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
	[DataContract(Namespace = DicomEditNamespace.Value)]
	[EditType("CA1E69EE-5264-4370-9339-F65ED9C78744")]
	public class EditSet : Edit
	{
		private List<Edit> _edits;

		[DataMember(IsRequired = true)]
		public Condition Condition { get; set; }

		[DataMember(IsRequired = true)]
		public List<Edit> Edits
		{
			get { return _edits ?? (_edits = new List<Edit>()); }
			set { _edits = value; }
		}

		public virtual bool AppliesTo(IEditContext context)
		{
			return Condition == null || Condition.IsMatch(context.DataSet);
		}

		#region Overrides of Edit

		public override void Apply(IEditContext context)
		{
			if (context.Excluded || !AppliesTo(context))
				return;

			foreach (var edit in Edits)
			{
				edit.Apply(context);
				if (context.Excluded)
					return;
			}
		}

		#endregion

		public override string ToString()
		{
			if (Edits.Count == 0)
				return "No Edits";

			if (Edits.Count == 1)
				return "1 Edit";

			return String.Format("{0} Edits", Edits.Count);
		}
	}
}