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
using System.Web.UI;
using System.Web.UI.WebControls;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// Control to display the summary information of a grid
    /// </summary>
    public partial class GridPager : UserControl
    {
        #region Private Members

        private GridView _target;
        private ImageServerConstants.GridViewPagerPosition _position;
        private string _targetUpdatePanelID;

        #endregion Private Members

        #region Public Properties

        public ImageServerConstants.GridViewPagerPosition PagerPosition
        {
            get { return _position; }
            set { _position = value; }
        }

        public string CurrentPageSaver
        {
            get; set;
        }

        public string AssociatedUpdatePanelID
        {
            set { _targetUpdatePanelID = value; }
        }

        /// <summary>
        /// Sets/Gets the grid associated with this control
        /// </summary>
        public GridView Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Sets/Retrieve the name of the item in the list.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Sets/Retrieves the name for the more than one items in the list.
        /// </summary>
        public string PluralItemName { get; set; }

        public int ItemCount
        {
            get 
            {
                int count = 0;
                if(GetRecordCountMethod != null)
                {
                    count = GetRecordCountMethod();
                    ViewState[ImageServerConstants.PagerItemCount] = count;
                }

                return count;
            }
        }

        #endregion Public Properties

        #region Public Delegates

        /// <summary>
        /// Methods to retrieve the number of records.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The number of records may be different than the value reported by <seealso cref="GridPager.Target"/>
        /// </remarks>
        public delegate int GetRecordCountMethodDelegate();

        /// <summary>
        /// Sets the method to be used by this control to retrieve the total number of records.
        /// </summary>
        public GetRecordCountMethodDelegate GetRecordCountMethod;

        #endregion Public Delegates

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // TODO: UpdateUI may fail if the DataBind has not been called on the target. 
                // Should this be moved inside IsDataBound condition?
                UpdateUI();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchUpdateProgress.AssociatedUpdatePanelID = _targetUpdatePanelID;
            SetPageContainerCssClass();
        }

        protected void PageButtonClick(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = Target.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "":
                    Target.PageIndex = intCurIndex;
                    break;
                case ImageServerConstants.Prev:
                    Target.PageIndex = intCurIndex - 1;
                    break;
                case ImageServerConstants.Next:
                    Target.PageIndex = intCurIndex + 1;
                    break;
                case ImageServerConstants.First:
                    Target.PageIndex = 0;
                    break;
                case ImageServerConstants.Last:
                    Target.PageIndex = Target.PageCount - 1;
                    break;
                default:

                    if (CurrentPage.Text.Equals(string.Empty))
                        Target.PageIndex = intCurIndex;
                    else
                    {
                        int newPage = Convert.ToInt32(Request.Form[CurrentPage.UniqueID]);

                        //Adjust page to match 0..n, and handle boundary conditions.
                        if (newPage > Target.PageCount)
                        {
                            newPage = _target.PageCount - 1;
                            if (newPage < 0) newPage = 0;
                        }
                        else if (newPage != 0) newPage -= 1;

                        Target.PageIndex = newPage;
                    }

                    break;
            }

            Target.Refresh();
        }

        private int AdjustCurrentPageForDisplay(int page)
        {
            if (_target.PageCount == 0)
            {
                page = 0;
            } else if (page == 0 )
            {
                page = 1;
            } else if (page >= _target.PageCount)
            {
                page = _target.PageCount;
            } else
            {
                page += 1;
            }

            return page;
        }

        private void EnableCurrentPage(bool enable)
        {
            if(enable)
            {
                string script =
                    "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" +
                    ChangePageButton.ClientID + "').click();return false;}} else {return true}; ";

                CurrentPage.Attributes.Add("onkeydown", script);
                CurrentPage.Attributes.Add("onclick", "javascript: document.getElementById('" + CurrentPage.ClientID + "').select();");
                CurrentPage.Enabled = true;
            } else
            {
                CurrentPage.Attributes.Add("onkeydown", "return false;");
                CurrentPage.Attributes.Add("onclick", "return false;");
                CurrentPage.Enabled = false;
            }
        }

        #endregion Protected methods

        #region Public methods

        

        public void InitializeGridPager(string singleItemLabel, string multipleItemLabel, GridView grid, GetRecordCountMethodDelegate recordCount, ImageServerConstants.GridViewPagerPosition position)
        {
            _position = position;
            ItemName = singleItemLabel;
            PluralItemName = multipleItemLabel;
            Target = grid;
            GetRecordCountMethod = recordCount;
            Target.DataBound += DataBoundHandler;
        }

        
        public void Reset()
        {
            // TODO: Why do we need this method beside Refresh() and UpdateUI()
            // In other words, was it necessary to store the count in viewstate? 
            ViewState[ImageServerConstants.PagerItemCount] = null;
        }

        /// <summary>
        /// Refresh the pager based on the latest data of the associated gridview control.
        /// This should be called whenever the gridview is databound.
        /// Note: this method will move the page
        /// </summary>
        /// 
        public void Refresh()
        {
         //   if (Target.Rows.Count == 0 && ItemCount > 0)
           // {
                // This happens when the last item on the current page is removed
                //Target.Refresh();
                // Note: if this method is called on DataBound event, it will be called again when the target is updated
                // However, we shoud have not have infinite loop because the the if condition
            //}
            //else
           // {
                UpdateUI();
        //    }   
        }

        #endregion Public methods

        #region Private Methods

        /// <summary>
        /// Update the UI contents
        /// </summary>
        private void UpdateUI()
        {
            if (_target != null && _target.DataSource != null)
            {
                ItemCountLabel.Text = string.Format("{0} {1}", ItemCount, ItemCount == 1 ? ItemName : PluralItemName);

                CurrentPage.Text = AdjustCurrentPageForDisplay(_target.PageIndex).ToString();
                EnableCurrentPage(_target.PageCount > 1);

                PageCountLabel.Text =
                    string.Format(" {0} {1}", Resources.GridPager.PageOf, AdjustCurrentPageForDisplay(_target.PageCount));

                SetPageContainerWidth(_target.PageCount);

                if (_target.PageIndex > 0)
                {
                    PrevPageButton.Enabled = true;
                    PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerPreviousEnabled;

                    FirstPageButton.Enabled = true;
                    FirstPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerFirstEnabled;
                }
                else
                {
                    PrevPageButton.Enabled = false;
                    PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerPreviousDisabled;

                    FirstPageButton.Enabled = false;
                    FirstPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerFirstDisabled;
                }


                if (_target.PageIndex < _target.PageCount - 1)
                {
                    NextPageButton.Enabled = true;
                    NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerNextEnabled;

                    LastPageButton.Enabled = true;
                    LastPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerLastEnabled;
                }
                else
                {
                    NextPageButton.Enabled = false;
                    NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerNextDisabled;

                    LastPageButton.Enabled = false;
                    LastPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerLastDisabled;

                }
            }
            else
            {
                ItemCountLabel.Text = string.Format("0 {0}", PluralItemName);
                CurrentPage.Text = "0";
                EnableCurrentPage(false);
                PageCountLabel.Text = string.Format(" {0} 0", Resources.GridPager.PageOf);;
                PrevPageButton.Enabled = false;
                PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerPreviousDisabled;
                NextPageButton.Enabled = false;
                NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerNextDisabled;

                FirstPageButton.Enabled = false;
                FirstPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerFirstDisabled;
                LastPageButton.Enabled = false;
                LastPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerLastDisabled;

            }
        }

        private void SetPageContainerWidth(int pageCount)
        {
            if (pageCount > 9999) CurrentPageContainer.Style.Add("width", "187px");
            else if (pageCount > 99999) CurrentPageContainer.Style.Add("width", "197px");
            else if (pageCount > 999999) CurrentPageContainer.Style.Add("width", "207px");
            else if (pageCount > 9999999) CurrentPageContainer.Style.Add("width", "217px");
        }

        private void SetPageContainerCssClass()
        {
            if (Request.UserAgent.Contains("Chrome")) CurrentPageContainer.CssClass = "CurrentPageContainer_Chrome";
            else if (Request.UserAgent.Contains("MSIE")) CurrentPageContainer.CssClass = "CurrentPageContainer";
            else CurrentPageContainer.CssClass = "CurrentPageContainer_FF";
        }

        private void DataBoundHandler(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }
}