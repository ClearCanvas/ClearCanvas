<%--  License

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

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmptySearchResultsMessage.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.EmptySearchResultsMessage" %>

<asp:Panel ID="Panel1" runat="server" CssClass="EmptySearchResultsMessage">
    <asp:Label runat="server" ID="ResultsMessage" Text = "No items found using the provided criteria." />
    <p></p>
    <asp:Panel runat="server"  ID="SuggestionPanel" HorizontalAlign="center">
    <center>
        <table  class="EmptySearchResultsSuggestionPanel">
	        <tr align="left">
	        <td class="EmptySearchResultsSuggestionPanelHeader">
	            <asp:Label runat="server" ID="SuggestionTitle" Text="<%$Resources: SR, EmptySearchResult_Suggestions %>"></asp:Label></td>
	        </tr>
	        <tr align="left" class="EmptySearchResultsSuggestionContent">
	        <td class="EmptySearchResultsSuggestionContent" style="padding:10px;">
	             <asp:PlaceHolder ID="SuggestionPlaceHolder" runat="server"></asp:PlaceHolder>
	        </td>
	        </tr>
	    </table>
        </center>
    </asp:Panel>
    
</asp:Panel>