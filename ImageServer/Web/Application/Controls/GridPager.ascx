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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.GridPager" %>
<%@ Import Namespace="Resources" %>
<table width="100%" cellpadding="0" cellspacing="0" class="GlobalGridPager" border="0">
    <tr>
        <td align="left" valign="top" style="padding-left: 6px; padding-bottom: 2px; padding-top: 0px;">
                                            <% if (PagerPosition == ImageServerConstants.GridViewPagerPosition.Top)
                                   { %>
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td valign="top">
                        <asp:Image runat="server" SkinID="<%$ Image : GridViewPagerTotalStudiesLeft%>"/>
                    </td>
                    <td valign="top" nowrap="nowrap">
                        <%
                            if (Request.UserAgent.Contains("Chrome"))
                            {%>
                        <div id="ItemCountContainer_Chrome">
                        <%} else if (Request.UserAgent.Contains("MSIE")) {%>
                        <div id="ItemCountContainer">
                        <%}%>       
                        <% else {%>
                        <div id="ItemCountContainer_FF">
                        <%}%>                    
                            <asp:Label ID="ItemCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
                        </div>
                    </td>
                    <td valign="top">
                        <asp:Image ID="Image1" runat="server" SkinID="<%$ Image : GridViewPagerTotalStudiesRight%>" />
                    </td>
                </tr>
            </table>
                        <%} %>            
        </td>
        <td align="center">
            <% if (PagerPosition == ImageServerConstants.GridViewPagerPosition.Top)
               { %>
               
            <asp:UpdateProgress ID="SearchUpdateProgress" runat="server" DisplayAfter="50">
                <ProgressTemplate>
                    <asp:Image ID="Image10" runat="server" SkinID="<%$Image:Searching %>"/>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <%} %>
        </td>
        <td align="right" style="padding-right: 6px; padding-bottom: 2px; padding-top: 0px;">
            <table cellspacing="0" cellpadding="0">
                <tr>
                    <td valign="top" style="padding-top: 0px;">
                        <asp:ImageButton ID="FirstPageButton" runat="server" CommandArgument="First" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
                    </td>                
                    <td valign="top" style="padding-top: 0px;">
                        <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
                    </td>
                    <td nowrap="nowrap">
                        <asp:panel ID="CurrentPageContainer" runat="server">
                            <asp:Label ID="Label3" runat="server" Text="<%$Resources: GridPager, Page %>" CssClass="GlobalGridPagerLabel" />
                            <asp:TextBox ID="CurrentPage" runat="server" Width="85px" CssClass="GridViewTextBox"
                                Style="font-size: 12px;" />
                            <asp:Label ID="PageCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
                            <aspAjax:FilteredTextBoxExtender runat="server" ID="CurrentPageFilter" FilterType="Numbers" TargetControlID="CurrentPage"  />
                            </asp:panel>
                    </td>
                    <td valign="top" style="padding-top: 0px;">
                        <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
                    </td>
                    <td valign="top" style="padding-right: 1px; padding-top: 0px;">
                        <asp:ImageButton ID="LastPageButton" runat="server" CommandArgument="Last" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
                    </td>     
                    <td>
                        <%-- This Link Button is used to submit the Page from the TextBox when the user clicks enter on the text box. --%>
                        <asp:LinkButton ID="ChangePageButton" runat="server" CommandArgument="ChangePage"
                            CommandName="Page" OnCommand="PageButtonClick" Text="" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
