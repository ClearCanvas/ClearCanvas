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
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using Resources;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    //
    //  Used to display the list of studies.
    //
    public partial class StudyListGridView : GridViewPanel
    {
		#region Delegates
		public delegate void StudyDataSourceCreated(StudyDataSource theSource);
		public event StudyDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<StudySummary> _studies;
	    private Unit _height;
    	private StudyDataSource _dataSource;
	    private List<StudySummary> _selectedStudies;

	    #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new StudyDataSource();

					_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
											{
												Studies = newlist;
											};
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);
					_dataSource.SelectCount();
				}
				if (_dataSource.ResultCount == 0)
				{
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);

					_dataSource.SelectCount();
				}
				return _dataSource.ResultCount;
			}
		}

	    public ServerPartition Partition { get; set; }

	    /// <summary>
        /// Gets/Sets the current selected studies.
        /// </summary>
        public IList<StudySummary> SelectedStudies
        {
            get
            {
				if (_selectedStudies==null)
					LoadSelectedStudies();

				return _selectedStudies.OrderBy(x => x.PatientsName).ToList();
            }
        }


        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<StudySummary> Studies
        {
            get
            {
                return _studies;
            }
            set
            {
                _studies = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get
            {
            	if (ContainerTable != null)
                    return ContainerTable.Height;
            	return _height;
            }
        	set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        public void SetDataSource()
        {
            StudyListControl.DataSource = StudyDataSourceObject;
        }

        public bool IsDataSourceSet()
        {
            return StudyListControl.DataSource != null;
        }

        public bool DisplayVetTags()
        {
            return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Study.VetTags);
        }

		public bool DisplayQCColumn
		{
			get { return false; }
		}

        #endregion
  
  
        #region protected methods
     
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = StudyListControl;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            if (DisplayVetTags())
            {
                foreach (DataControlField o in StudyListControl.Columns)
                {
                    var t = o as TemplateField;
                    if (t != null)
                    {
                        // TODO: This is a bit of a Hack, need something better for this in the future.
                        if (t.Visible == false)
                            t.Visible = true;
                        continue;
                    }

                    var f = o as BoundField;
                    if (f == null) continue;

                    if (f.DataField.Equals("ResponsiblePerson"))
                        f.Visible = true;
                    else if (f.DataField.Equals("ResponsibleOrganization"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Species"))
                        f.Visible = true;
                    else if (f.DataField.Equals("Breed"))
                        f.Visible = true;
                }
            }

			var col = StudyListControl.Columns.OfType<TemplateField>().FirstOrDefault(c => c.HeaderText.Equals(ColumnHeaders.QCStatus));
			if (col != null)
			{
				col.Visible = DisplayQCColumn;
			}


            if(IsPostBack || Page.IsAsync)
            {
                StudyListControl.DataSource = StudyDataSourceObject;    
            } 
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

        

            if (Studies == null)
            {
                return;
            } 

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    StudySummary study = Studies[row.RowIndex];

                    if (study != null)
                    {
                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", study.ThePartition.AeTitle);

                        string reason;
                        if (study.CanScheduleDelete(out reason))
                            row.Attributes.Add("candelete", "true");

                        if (study.CanScheduleMove(out reason))
                            row.Attributes.Add("canmove", "true");

                        if (study.CanScheduleRestore(out reason))
                            row.Attributes.Add("canrestore", "true");

                        if (study.CanViewImages(out reason))
                        {
                            row.Attributes.Add("canviewimages", "true");
                            
                        } else
                        {
                            row.Attributes.Add("canviewimagesreason", reason);
                        }

                        var status = (Label) row.FindControl("StudyStatusEnum");
                        status.Text = ServerEnumDescription.GetLocalizedDescription(study.StudyStatusEnum);


						var qcStatusLink = (HtmlAnchor)row.FindControl("QCStatusLink");
						if (qcStatusLink != null)
						{
							var processing = study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled) || study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ReprocessScheduled);
							if (processing)
							{
							    qcStatusLink.InnerText = string.Empty;
							}
							else
							{
								qcStatusLink.InnerText = study.TheStudy.QCStatusEnum != null
																? ServerEnumDescription.GetLocalizedDescription(study.TheStudy.QCStatusEnum)
																: "N/A";

                                if (study.TheStudy.QCStatusEnum!=QCStatusEnum.Processing)
								    qcStatusLink.HRef = ResolveClientUrl(string.Format("~/Pages/Studies/StudyQCReport.aspx?PartitionAE={0}&StudyUid={1}", study.ThePartition.AeTitle, study.TheStudy.StudyInstanceUid));
							}

						}
						
						

                        LinkButton button = (LinkButton) row.FindControl("ReconcileLinkButton");
                        Label label = (Label)row.FindControl("SeparatorLabel");

                        if (study.IsReconcileRequired)
                        {
                            button.Visible = true;
							label.Visible = true;

                            button.PostBackUrl = ImageServerConstants.PageURLs.StudyIntegrityQueuePage +
                                                 "?PatientID=" + study.PatientId + "&PatientName=" + study.PatientsName + "&PartitionKey=" + study.ThePartition.GetKey();

                            button.Enabled = Context.User.IsInRole(AuthorityTokens.StudyIntegrityQueue.Search);
                        }
                        else
                        {
                            button.Visible = false;
                            label.Visible = false;
                        }

                        button = (LinkButton) row.FindControl("QueueLinkButton");
                        label = (Label) row.FindControl("QueueSeparatorLabel");

                        if(study.IsLocked)
                        {
                            button.Visible = true;
									
							if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.RestoreScheduled))
							{
                                button.PostBackUrl = ImageServerConstants.PageURLs.RestoreQueuePage +
                                                         "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
                                                         study.ThePartition.Key;
                                button.Text = ServerEnumDescription.GetLocalizedDescription(study.QueueStudyStateEnum);
							    button.Enabled = Context.User.IsInRole(AuthorityTokens.RestoreQueue.Search);
							}
							else if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ArchiveScheduled))
							{
                                button.PostBackUrl = ImageServerConstants.PageURLs.ArchiveQueuePage +
                                                         "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
                                                         study.ThePartition.Key;
                                button.Text = ServerEnumDescription.GetLocalizedDescription(study.QueueStudyStateEnum);
							    button.Enabled = Context.User.IsInRole(AuthorityTokens.ArchiveQueue.Search);
							}
							else
							{
                                button.PostBackUrl = ImageServerConstants.PageURLs.WorkQueuePage +
                                                         "?PatientID=" + Server.UrlEncode(study.PatientId) + "&PatientName=" + Server.UrlEncode(study.PatientsName) + "&PartitionKey=" +
                                                         study.ThePartition.Key;
                                button.Text = ServerEnumDescription.GetLocalizedDescription(study.QueueStudyStateEnum);
							    button.Enabled = Context.User.IsInRole(AuthorityTokens.WorkQueue.Search);
							}
                        } else
                        {
                            button.Visible = false;
                        }

                        label.Visible = button.Visible;
                    }
                }
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    message.Message = TheGrid.DataSource == null ? SR.PleaseEnterSearchCriteriaToFindStudies: SR.NoStudiesFound;
                }
            } 
        }

		protected void GetStudyDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new StudyDataSource();

				_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
										{
											Studies = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}

        #endregion

		#region Private Methods

		/// <summary>
		/// Gets a list of <see cref="StudySummary"/> correspeonding to the studies currently selected
		/// </summary>
		/// <returns></returns>
		private void LoadSelectedStudies()
		{
			_selectedStudies = new List<StudySummary>();

			string[] selectedKeys = StudyListControl.SelectedDataKeys;
			if (selectedKeys != null)
			{
				foreach (var key in selectedKeys)
				{
					// Note: the selected study may not be on the current the page. 
					// This can happens for example when some of the studies which appear before it are deleted from the system.
					// For this reason, if the selected study does not appear on the current page, we need to load the studies from the database
					StudySummary studySummary;
					if (!TryFindStudySummaryOnCurrentPage(key, out studySummary))
					{
						var persistentContext = HttpContext.Current.GetSharedPersistentContext();
						var study = Study.Load(persistentContext, new ServerEntityKey("Study", key));
						if (study != null)
						{
							studySummary = StudySummaryAssembler.CreateStudySummary(persistentContext, study);
						}
					}

					if (studySummary != null)
						_selectedStudies.Add(studySummary);
				}
			}
		}

		/// <summary>
		/// Tries to find the study that appears on the current page.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="study"></param>
		/// <returns></returns>
	    private bool TryFindStudySummaryOnCurrentPage(string key, out StudySummary study)
	    {
		    study = null;
			if (_studies != null && _studies.ToList().Exists(x => x.Key.Key.ToString().Equals(key)))
				study = _studies.First(x => x.Key.Key.ToString().Equals(key));

		    return study != null;

	    }

		#endregion

	}

}
