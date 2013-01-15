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

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    
    public partial class EmptySearchResultsMessage : System.Web.UI.UserControl
    {
        [ParseChildren(true)]
        public class SuggestionPanelContainer: Panel, INamingContainer
        {
        }
        
        private readonly SuggestionPanelContainer _suggestionPanelContainer = new SuggestionPanelContainer();
        private ITemplate _suggestionTemplate = null;


        [TemplateContainer(typeof(SuggestionPanelContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate SuggestionTemplate
        {
            get
            {
                return _suggestionTemplate;
            }
            set
            {
                _suggestionTemplate = value;
            }
        }

        public string Message
        { 
            get { return ResultsMessage.Text;}
            set{ ResultsMessage.Text = value;}
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (_suggestionTemplate != null)
            {
                _suggestionTemplate.InstantiateIn(_suggestionPanelContainer);
                SuggestionPlaceHolder.Controls.Add(_suggestionPanelContainer);
            }

            SuggestionPanel.Visible = _suggestionTemplate != null;
        }


    }
}