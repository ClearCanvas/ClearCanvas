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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginWarningsDialog.ascx.cs" 
            Inherits="ClearCanvas.ImageServer.Web.Application.Controls.LoginWarningsDialog" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="localAsp" %>

<localAsp:ModalDialog ID="ModalDialog1" runat="server" Width="300px" Title="<%$Resources: ColumnHeaders, LoginWarningComponent %>">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" width="100%" CssClass="DialogPanelContent">

            <div id="StudyList">
                <table border="0" cellspacing="5" width="100%">
                    <tr>
                        <td>
                            <div class="DeleteStudiesTableContainer" style="background: white">
                                <asp:Repeater runat="server" ID="WarningListing">
                                    <HeaderTemplate>
                                        <table cellspacing="0" width="100%" class="DeleteStudiesConfirmTable">
                                        <tr><th style="white-space: nowrap" class="GlobalGridViewHeader"><%= ColumnHeaders.AppLogMessage %></th></tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="GlobalGridViewRow"><td><%# HtmlUtility.GetEvalValue(Container.DataItem, "Message", "&nbsp;") %></td></tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>                           
                        </td>
                    </tr>
                </table>
            </div>
                   
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="right">
                        <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="DismissLoginWarnings_Click" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>

        </asp:Panel>
    </ContentTemplate>
</localAsp:ModalDialog>