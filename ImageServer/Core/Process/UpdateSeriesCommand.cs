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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Process
{
	/// <summary>
	/// Updates the existing Series record in the database
	/// </summary>
	public class UpdateSeriesCommand : ServerDatabaseCommand
	{
		private readonly DicomAttributeCollection _data;
		private readonly ServerActionContext _context;
		private readonly StudyStorageLocation _storageLocation;


		/// <summary>
		/// Creates an instance of <see cref="UpdateSeriesCommand"/> to update the existing Series record in the database
		/// </summary>
		public UpdateSeriesCommand(StudyStorageLocation storageLocation, ServerActionContext sopContext)
			: base(String.Concat("Update Series Command"))
		{
			_storageLocation = storageLocation;
			_data = sopContext.Message.DataSet;
			_context = sopContext;
		}

		#region Overrides of ServerDatabaseCommand

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			var seriesUid = _data[DicomTags.SeriesInstanceUid].ToString();

			var broker= updateContext.GetBroker<ISeriesEntityBroker>();
			
			var criteria = new SeriesSelectCriteria();
			criteria.ServerPartitionKey.EqualTo(_context.ServerPartition.Key);
			criteria.StudyKey.EqualTo(_storageLocation.Study.Key);
			criteria.SeriesInstanceUid.EqualTo(seriesUid);
			var series=broker.FindOne(criteria);

			if (series!=null)
			{
				var updates = new SeriesUpdateColumns();
				if (_data.LoadDicomFields(updates))
					broker.Update(series.Key, updates);
			}

			
		}

		#endregion
	}
}