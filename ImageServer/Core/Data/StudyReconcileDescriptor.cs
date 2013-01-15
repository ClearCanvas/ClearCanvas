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
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents information encoded in the xml of a <see cref="StudyHistory"/> record of type <see cref="StudyHistoryTypeEnum.StudyReconciled"/>.
	/// </summary>
	/// <remarks>
	/// </remarks>
	[XmlRoot("Reconcile")]
	public class StudyReconcileDescriptor
	{
		#region Private Members

		private List<BaseImageLevelUpdateCommand> _commands;

		#endregion

		#region Public Properties

		/// <summary>
		/// Reconciliation option
		/// </summary>
		public StudyReconcileAction Action { get; set; }

		/// <summary>
		/// User-defined description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether the reconciliation was automatic or manual.
		/// </summary>
		public bool Automatic { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="StudyInformation"/>
		/// </summary>
		public StudyInformation ExistingStudy { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="ImageSetDescriptor"/>
		/// </summary>
		public ImageSetDescriptor ImageSetData { get; set; }

		/// <summary>
		/// Gets or sets the commands that are part of the reconciliation process.
		/// </summary>
		[XmlArray("Commands")]
		[XmlArrayItem("Command", Type = typeof(AbstractProperty<BaseImageLevelUpdateCommand>))]
		public List<BaseImageLevelUpdateCommand> Commands
		{
			get
			{
				if (_commands == null)
					_commands = new List<BaseImageLevelUpdateCommand>();
				return _commands;
			}
			set { _commands = value; }
		}

		public string UserName { get; set; }

		public List<SeriesMapping> SeriesMappings { get; set; }

		#endregion
	}

	[XmlRoot("Reconcile")]
	public class ReconcileCreateStudyDescriptor : StudyReconcileDescriptor
	{
		public ReconcileCreateStudyDescriptor()
		{
			Action = StudyReconcileAction.CreateNewStudy;
		}
	}

	[XmlRoot("Reconcile")]
	public class ReconcileDiscardImagesDescriptor : StudyReconcileDescriptor
	{
		public ReconcileDiscardImagesDescriptor()
		{
			Action = StudyReconcileAction.Discard;
		}
	}

	[XmlRoot("Reconcile")]
	public class ReconcileMergeToExistingStudyDescriptor : StudyReconcileDescriptor
	{
		public ReconcileMergeToExistingStudyDescriptor()
		{
			Action = StudyReconcileAction.Merge;
		}
	}

	[XmlRoot("Reconcile")]
	public class ReconcileProcessAsIsDescriptor : StudyReconcileDescriptor
	{
		public ReconcileProcessAsIsDescriptor()
		{
			Action = StudyReconcileAction.ProcessAsIs;
		}
	}
}