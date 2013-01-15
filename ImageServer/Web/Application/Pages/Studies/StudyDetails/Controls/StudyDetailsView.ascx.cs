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
using System.Threading;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class StudyDetailsView : System.Web.UI.UserControl
    {
        #region Private members

        private Unit _width;

        private IList<StudySummary> _studies = new List<StudySummary>();

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the list of studies whose information are displayed
        /// </summary>
        public IList<StudySummary> Studies
        {
            get { return _studies; }
            set { _studies = value; }
        }

        public Unit Width
        {
            get { return _width; }
            set { _width = value;

                StudyDetailView.Width = value;
            }
        }

        public bool DisplayVetTags()
        {
            return Thread.CurrentPrincipal.IsInRole(Enterprise.Authentication.AuthorityTokens.Study.VetTags);            
        }


        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DisplayVetTags())
            {
                foreach (DataControlField o in StudyDetailView.Fields)
                {
                    // TODO: This is a bit of a Hack, need something better for this in the future.
                    var t = o as TemplateField;
                    if (t!=null)
                    {
                        if (t.Visible == false)
                            t.Visible = true;
                        continue;
                    }

                    var f = o as BoundField;
                    if (f == null) continue;

                    if (f.DataField.Equals("ResponsiblePerson"))
                        f.Visible = true;
                    else if (f.DataField.Equals("ResponsiblePersonRole"))
                        f.Visible = true;
                    else if (f.DataField.Equals("ResponsibleOrganization"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Species"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Breed"))
                        f.Visible = true;
                }
            }

            StudyDetailView.DataSource = Studies;
            StudyDetailView.DataBind();            
        }

        #endregion Protected Methods
    }
}