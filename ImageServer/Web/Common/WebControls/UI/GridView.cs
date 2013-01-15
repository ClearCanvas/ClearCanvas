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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.js", "text/javascript")]

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Custom ASP.NET AJAX grid view control.
    /// </summary>
    /// <remarks>
    /// This custom grid view control offers a couple of improvements over the standard ASP.NET GridView:
    /// multiple selection and client-side item selection (ie. not postback to the server). 
    /// </remarks>
    [ToolboxData("<{0}:GridView runat=server></{0}:GridView>")]
    public class GridView : System.Web.UI.WebControls.GridView , IScriptControl
    {
        public enum SelectionModeEnum
        {
            Disabled,
            Single,
            Multiple
        }

        #region Constants

        private const string HtmlAttributeIsTooltip = "istooltip";

        #endregion

        #region Private members

        private string _onClientRowClick;
        private string _onClientRowDblClick;
        private SelectionModeEnum _selectionMode = SelectionModeEnum.Single;
        private Hashtable _selectedRows = new Hashtable();
        private Hashtable _selectedDataKeys = new Hashtable();
        private bool _isDataBound = false;
        private bool _selectUsingDataKeys = false;

        #endregion Private members

        #region Public Properties
        /// <summary>
        /// Sets or gets the selection mode.
        /// </summary>
        /// <remark>
        /// </remark>
        public SelectionModeEnum SelectionMode
        {
            get { return _selectionMode; }
            set { _selectionMode = value; }
        }

        public bool SelectUsingDataKeys
        {
            get { return _selectUsingDataKeys; }
            set { _selectUsingDataKeys = value; }
        }

        // TODO: a lot of classes check this property before calling Databind.
        // We should just override the Databind method in this class. 
        public bool IsDataBound
        {
            get { return _isDataBound;  }
            set { _isDataBound = value; }
        }

        /// <summary>
        /// Sets or gets the client-side script function that will be called on the client when a row in selected.
        /// </summary>
        /// <remark>
        /// The row will be selected before the specified function is called.
        /// </remark>
        public string OnClientRowClick
        {
            get { return _onClientRowClick; }
            set { _onClientRowClick = value; }
        }

        /// <summary>
        /// Sets or gets the client-side script function name that will be called in the client browser when users double-click on a row.
        /// </summary>
        /// <remark>
        /// 
        /// </remark>
        public string OnClientRowDblClick
        {
            get { return _onClientRowDblClick; }
            set { _onClientRowDblClick = value; }
        }

        /// <summary>
        /// Gets or sets the ID for the tooltip content
        /// </summary>
        public string TooltipContainerControlID { get; set; }


        /// <summary>
        /// Sets or gets the Mouseover highlighting enabled/disabled.
        /// </summary>
        /// <remark>
        /// 
        /// </remark>
        public bool MouseHoverRowHighlightEnabled
        {
            get
            {
                string newClientID = ClientID;
                if (newClientID.LastIndexOf("_") != -1)
                {
                    newClientID = newClientID.Substring(newClientID.LastIndexOf("_")+1);
                }
                if (ViewState["MouseHoverRowHighlightEnabled_" + newClientID] != null)
                    return (bool)ViewState["MouseHoverRowHighlightEnabled_" + newClientID];
                else
                    return true;
            }
            set
            {
                //For some reason, the gridview calls this property twice, once with the name of the gridview alone (E.G. AlertGridView), 
                //and a second time with the entire client ID (e.g. ctl00_MainContentPlaceHolder_AlertGridView"). To get around this, 
                //the name of the Gridview only is used and the value is set only the first time, which is the intended value.
                string newClientID = ClientID;
                if(newClientID.LastIndexOf("_") != -1)
                {
                    newClientID = newClientID.Substring(newClientID.LastIndexOf("_")+1);
                }
                if (ViewState["MouseHoverRowHighlightEnabled_" + newClientID] == null)
                    ViewState["MouseHoverRowHighlightEnabled_" + newClientID] = value;
            }
        }

        /// <summary>
        /// Sets or gets the Highlight color of a row  
        /// </summary>
        /// <remark>
        /// 
        /// </remark>
        public Color RowHighlightColor
        {
            get
            {
                if (ViewState["RowHighlightColor"] != null)
                    return (Color)ViewState["RowHighlightColor"];
                else
                {
                    // default color
                    return Color.FromArgb(238,238,238);
                }
            }

            set { ViewState["RowHighlightColor"] = value; }
        }


        /// <summary>
        /// Gets the index of the first selected item.
        /// </summary>
        /// <remarsk>
        /// Use <see cref="SelectedIndices"/> instead.
        /// </remarsk>
        public override int SelectedIndex
        {
            get
            {
                int[] indices = SelectedIndices;
                if (indices == null || indices.Length == 0)
                    return -1;

                return indices[0];
            }
        }

        /// <summary>
        /// Gets the indices of the rows being selected
        /// </summary>
        /// <remarks>
        /// <see cref="SelectedIndices"/>=<b>null</b> or <see cref="SelectedIndices"/>.Length=0 if no row is being selected.
        /// </remarks>
        public int[] SelectedIndices
        {
            get
            {
                if (_selectedRows.Keys.Count == 0)
                    return null;

                int[] rows = new int[_selectedRows.Keys.Count];
                int i = 0;
                foreach (int r in _selectedRows.Keys)
                    rows[i++] = r;

                return rows;
            }
        }

        public string[] SelectedDataKeys
        {
            get
            {
                if (_selectedDataKeys.Keys.Count == 0) return null;

                string[] dataKeys = new string[_selectedDataKeys.Keys.Count];
                int i = 0;
                foreach (string s in _selectedDataKeys.Keys)
                    dataKeys[i++] = s;

                return dataKeys;
            }
        }

        #endregion Public Properties


        #region Public Methods

        public override void RenderControl(HtmlTextWriter writer)
        {
            SaveState();
            base.RenderControl(writer);
        }

        protected virtual void SaveState()
        {
            string stateValue = "";
            string selectedDataKeys = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            int[] rows = SelectedIndices;
            if (rows != null)
                stateValue = serializer.Serialize(rows);

            string[] dataKeys = SelectedDataKeys;
            if (dataKeys != null)
                selectedDataKeys = serializer.Serialize(dataKeys);

            Page.ClientScript.RegisterHiddenField(ClientID + "SelectedRowIndices", stateValue);
            Page.ClientScript.RegisterHiddenField(ClientID + "SelectedRowDataKeys", selectedDataKeys);
        }

        protected virtual void LoadState()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string selectedKeysValue = Page.Request[ClientID + "SelectedRowDataKeys"];
			if (!string.IsNullOrEmpty(selectedKeysValue) && selectedKeysValue != "null")
			{
				string[] dataRows = serializer.Deserialize<string[]>(selectedKeysValue);
				foreach (string r in dataRows)
				{
					if (!string.IsNullOrEmpty(r))
						_selectedDataKeys[r] = true;
				}
			}
		
			string statesValue = Page.Request[ClientID + "SelectedRowIndices"];
			if (!string.IsNullOrEmpty(statesValue))
			{
				int[] rows = serializer.Deserialize<int[]>(statesValue);
				foreach (int r in rows)
				{
					_selectedRows[r] = true;
				}
			}			
        }

        /// <summary>
        /// Clear the current selections.
        /// </summary>
        public void ClearSelections()
        {
            int[] rows = SelectedIndices;
            if (rows != null && Rows != null)
            {
               if (Rows.Count > 0)
               {
                        for (int i = 0; i < rows.Length; i++)
                        {
                            int rowIndex = rows[i];
							if (rowIndex < Rows.Count && rowIndex >= 0)
							{                                
                                Rows[rowIndex].RowState = DataControlRowState.Normal;
								Rows[rowIndex].Attributes["selected"] = "false";
							}
                        }
                    }
            }
			_selectedRows = new Hashtable();
			_selectedDataKeys = new Hashtable();
        }

        #endregion Public Methods
        
        #region IScriptControl Members

        
        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor(GetType().FullName, ClientID);

            desc.AddProperty("clientStateFieldID", ClientID + "SelectedRowIndices");
            desc.AddProperty("selectedKeysStateFieldID", ClientID + "SelectedRowDataKeys");
            desc.AddProperty("SelectionMode", SelectionMode.ToString());

            string style = StyleToString(SelectedRowStyle);
            desc.AddProperty("SelectedRowStyle", style);
            desc.AddProperty("SelectedRowCSS", SelectedRowStyle.CssClass);

            style = StyleToString(RowStyle);
            desc.AddProperty("UnSelectedRowStyle", style);
            desc.AddProperty("UnSelectedRowCSS", RowStyle.CssClass);

            desc.AddProperty("SelectUsingDataKeys", SelectUsingDataKeys);


            if (AlternatingRowStyle!=null)
            {
                style = StyleToString(AlternatingRowStyle);
                desc.AddProperty("AlternatingRowStyle", style);
                desc.AddProperty("AlternatingRowCSS", AlternatingRowStyle.CssClass); 
            }
            else
            {
                // use the normal row style
                style = StyleToString(RowStyle);
                desc.AddProperty("UnSelectedRowStyle", style);
                desc.AddProperty("UnSelectedRowCSS", RowStyle.CssClass);

            }
            
            if (OnClientRowClick!=null)
                desc.AddEvent("onClientRowClick", OnClientRowClick);

            if (OnClientRowDblClick!=null)
                desc.AddEvent("onClientRowDblClick", OnClientRowDblClick);
            
            return new ScriptDescriptor[] { desc };
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();

            reference.Path = Page.ClientScript.GetWebResourceUrl(GetType(), "ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.js");
            return new ScriptReference[] { reference };
        }

        #endregion IScriptControl Members


        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptControl(this);    
            }

            if (!Page.IsPostBack)
            {
                if (!IsDataBound)
                {
                    DataBind();
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptDescriptors(this);
            }

            foreach(GridViewRow row in Rows)
            {
                if (row.RowType==DataControlRowType.DataRow)
                {
                    if (SelectUsingDataKeys)
                    {
                        if (DataKeys != null && DataKeys.Count <= Rows.Count)
                        {
                            if (row.RowIndex < DataKeys.Count)
                            {
                                row.Attributes["dataKey"] = DataKeys[row.RowIndex].Value.ToString();
                                if (_selectedDataKeys.ContainsKey(row.Attributes["dataKey"]))
                                    row.Attributes["selected"] = "true";
                                else
                                {
                                    row.Attributes["selected"] = "false";
                                }
                            }
                            else row.Attributes["dataKey"] = "Data Key Does not exist for row with index" + row.RowIndex;
                        }
                    }
                    else
                    {
                        if (_selectedRows.ContainsKey(row.RowIndex))
                        {
                            row.RowState = DataControlRowState.Selected;
                            row.Attributes["selected"] = "true";
                        }
                        else
                        {
                            row.Attributes["selected"] = "false";
                        }
                    }
                }


            }
            base.Render(writer);
        }

        protected override void OnSelectedIndexChanging(GridViewSelectEventArgs e)
        {

            base.OnSelectedIndexChanging(e);

            int rowIndex = e.NewSelectedIndex;

            SelectRow(rowIndex);
        
        }
        
        public new void SelectRow(int rowIndex)
        {
            Rows[rowIndex].RowState = DataControlRowState.Selected;
            Rows[rowIndex].Attributes["selected"] = "true";
             
            _selectedRows[rowIndex] = true;
        }

        /// <summary>
        /// Refresh the data in the gridview.
        /// 
        /// NOTE: If the GridView is used in an UpdatePanel, it should also be updated too.
        /// Databound event will be fired.
        /// </summary>
        public void Refresh()
        {
            ClearSelections();
            DataBind();

            // NOTE: If the GridView is used in an UpdatePanel, it should also be updated too 
            // the UI on the client is actually refreshed
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
           
            if (!DesignMode)
            {
                if (Page.IsPostBack)
                {
                    LoadState();
                }
                else if (SelectedIndex>=0)
                {
                    _selectedRows[SelectedIndex] = true;
                }

                PagerSettings.Visible = false;
            }

            //1 is the smallest value the PageSize can be set to, and since, it's unlikely that we would want
            //a page size of 1, it will indicate that we want to use the page size from the web.config.
            //This allows individual gridviews to set custom page sizes, should it be necessary.
            if(PageSize == 1)
            {
                PageSize = Int32.Parse(ConfigurationManager.AppSettings["PageSize"]);
            }
                    
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientMouseEventsScript();
        }


        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            base.OnRowCreated(e);

            if(e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                e.Row.Attributes["isEmptyDataRow"] = "true";
                return;
            } else if(e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Attributes["isHeaderRow"] = "true";
                return;
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // the following lines will disable text select on the row.
                // We need to disable it because IE and firefox interpret Ctrl-Click as text selection and will display
                // a box around the cell users click on.

                // Firefox way:
                e.Row.Attributes["onmousedown"] = "if (event.ctrlKey && typeof (event.preventDefault) != 'undefined') {event.preventDefault();}";
                // IE way:
                e.Row.Attributes["onselectstart"] = "return false;"; // disable select

                e.Row.Attributes["isdatarow"] = "true";
                e.Row.Attributes["rowIndex"] = (e.Row.RowIndex).ToString();

                if (e.Row.RowType == DataControlRowType.DataRow && MouseHoverRowHighlightEnabled)
                {
                    e.Row.Style["cursor"] = "hand";
                    string onMouseOver = string.Format("this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='{0}';", ColorTranslator.ToHtml(RowHighlightColor));
                    string onMouseOut = "this.style.backgroundColor=this.originalstyle;";
                    e.Row.Attributes.Add("onmouseover", onMouseOver);
                    e.Row.Attributes.Add("onmouseout", onMouseOut);
                }

                if (!string.IsNullOrEmpty(TooltipContainerControlID))
                {
                    var tooltip = e.Row.FindControl(TooltipContainerControlID) as WebControl;
                    if (tooltip == null)
                        throw new Exception(string.Format("Could not find TooltipContainerControlID '{0}'. Make sure it is a WebControl", TooltipContainerControlID));

                    tooltip.Attributes[HtmlAttributeIsTooltip] = "true";

                }
            }

            
        }


        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            IsDataBound = true;
        }
        
        #endregion Protected Methods

        static string StyleToString(Style s)
        {
            StringBuilder sb = new StringBuilder();
            
            if (s.BackColor != Color.Empty)
                sb.Append("background-color:"
                   + ColorTranslator.ToHtml(s.BackColor) + ";");
            if (s.ForeColor != Color.Empty)
                sb.Append("color:"
                   + ColorTranslator.ToHtml(s.ForeColor) + ";");

            #region -- Border --
            string color = "black";
            string width = "";
            string style = "solid";
            if (s.BorderColor != Color.Empty)
                color = ColorTranslator.ToHtml(s.BorderColor);
            if (s.BorderStyle != BorderStyle.NotSet)
                style = s.BorderStyle.ToString();
            if (s.BorderWidth != Unit.Empty)
                width = s.BorderWidth.ToString();
            if (color != "" && width != "" && style != "")
                sb.Append("border:" + color + " " + width + " " + style + ";");
            #endregion

            #region -- Font --

            #region -- Font General --
            if (s.Font.Size != FontUnit.Empty)
                sb.Append("font-size:" + s.Font.Size + ";");
            if (s.Font.Bold)
                sb.Append("font-weight:Bold;");
            if (s.Font.Italic)
                sb.Append("font-style:Italic;");
            #endregion

            #region -- Font Names --
            ArrayList fontList = new ArrayList();
            if (s.Font.Name.Length != 0)
                fontList.Add(s.Font.Name);
            foreach (string f in s.Font.Names)
                fontList.Add(f);
            if (fontList.Count > 0)
            {
                string fontString = "";
                for (int i = 0; i < fontList.Count; i++)
                {
                    if (i == 0)
                        fontString = (string)fontList[i];
                    else
                        fontString += ", " + fontList[i];
                }
                sb.Append("font-family:" + fontString + ";");
            }
            #endregion

            #region - Text Decoration --
            ArrayList decorList = new ArrayList();
            if (s.Font.Underline)
                decorList.Add("underline");
            if (s.Font.Overline)
                decorList.Add("overline");
            if (s.Font.Strikeout)
                decorList.Add("line-through");
            if (decorList.Count > 0)
            {
                string strDecor = "";
                for (int i = 0; i < decorList.Count; i++)
                {
                    if (i == 0)
                        strDecor = (string)decorList[i];
                    else
                        strDecor += ", " + decorList[i];
                }
                sb.Append("text-decoration:" + strDecor + ";");
            }
            #endregion

            #endregion

            #region -- Height and Width --
            if (!s.Height.IsEmpty)
                sb.Append("height:" + s.Height + ";");
            if (!s.Width.IsEmpty)
                sb.Append("width:" + s.Width + ";");
            #endregion

            return sb.ToString();

        }

        private void RegisterClientMouseEventsScript()
        {
            var script = @"var gv = $('#@@GridViewClientID@@');
                           var rows = gv.find('[isdatarow]');";

            script = script.Replace("@@GridViewClientID@@", ClientID);
                
            // show/hide tooltip
            if (!string.IsNullOrEmpty(TooltipContainerControlID))
            {
                script += @"rows.mouseenter(function(e){
                                $(this).find('@@TooltipSelector@@')
                                .css('position','absolute')
                                .css('left', e.pageX +20)
                                .css('top', e.pageY)
                                .show();
                        });";

                script += @"rows.mouseleave(function(){
                                $(this).find('@@TooltipSelector@@')
                                .hide();
                        });";


                var tooltipSelector = "[" + HtmlAttributeIsTooltip + "]"; // selector based on attribute
                script = script.Replace("@@TooltipSelector@@", tooltipSelector);
            
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "MouseOverClientScript", script, true);
        }
    }
}
