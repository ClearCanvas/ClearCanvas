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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
	public class DialogHelper
	{
		public static string createConfirmationMessage(string message)
		{
			return string.Format("<span class=\"ConfirmDialogMessage\">{0}</span>", message);
		}

		public static string createStudyTable(IList<Study> studies)
		{
			string message;

			message =
				string.Format("<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">{0}</td><td colspan=\"2\">{1}</td><td>{2}</td></tr>",
                    ColumnHeaders.PatientName, ColumnHeaders.AccessionNumber, ColumnHeaders.StudyDescription);


			int i = 0;
			foreach (Study study in studies)
			{
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}&nbsp;</td><td>&nbsp;</td><td>{2}&nbsp;</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}

        // TODO: this looks very bad. Fix it
        public static string createSeriesTable(IList<Series> series)
        {
            string message;

            message =
                string.Format("<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">{0}</td><td colspan=\"2\">{1}</td><td colspan=\"2\">{2}</td><td colspan=\"2\">{3}</td></tr>",
                    ColumnHeaders.Modality, ColumnHeaders.SeriesDescription, ColumnHeaders.SeriesCount, ColumnHeaders.SeriesInstanceUID);
            
            int i = 0;
            foreach (Series s in series)
            {
                String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}</td><td>&nbsp;</td><td>{2}</td><td>&nbsp;</td><td>{3}</td></tr>",
                                 s.Modality, s.SeriesDescription, s.NumberOfSeriesRelatedInstances, s.SeriesInstanceUid);
                message += text;

                i++;
            }
            message += "</table>";

            return message;
        }

        // TODO: this looks very bad. Fix it
        public static string createStudyTable(IList<StudySummary> studies)
		{
			string message;

			message =
				string.Format("<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">{0}</td><td colspan=\"2\">{1}</td><td>{2}</td></tr>",
                    ColumnHeaders.PatientName, ColumnHeaders.AccessionNumber, ColumnHeaders.StudyDescription);

			int i = 0;
			foreach (StudySummary study in studies)
			{
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}&nbsp;</td><td>&nbsp;</td><td>{1}&nbsp;</td><td>&nbsp;</td><td>{2}&nbsp;</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}
	}
}