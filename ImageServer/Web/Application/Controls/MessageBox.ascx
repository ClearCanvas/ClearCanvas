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

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MessageBox.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.MessageBox" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="localAsp" %>

<localAsp:ModalDialog ID="ModalDialog" runat="server" Title="" >
    <ContentTemplate>
        <asp:ScriptManagerProxy ID="DialogScriptManager" runat="server"/>
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="1" style="height: 24px">
                            <asp:Image ID="IconImage" runat="server" Visible="false" /></td>
                        <td colspan="2" style="height: 24px; vertical-align: top; text-align: center;">
                            <asp:Panel runat="server" CssClass="ConfirmationContent">
                                <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message" />
                                <p/>
                                <asp:Label ID="WarningMessageLabel" runat="server" Style="text-align: center; color:red;" Text="Warning: blah blah blah" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            <asp:Panel ID="ButtonPanel" runat="server" DefaultButton="NoButton" CssClass="ConfirmationButtonPanel">
                                            <ccUI:ToolbarButton ID="YesButton" runat="server" SkinID="<%$Image:YesButton%>" OnClick="YesButton_Click" />
                                            <ccUI:ToolbarButton ID="NoButton" runat="server" SkinID="<%$Image:NoButton%>" OnClick="NoButton_Click"  />
                                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="OKButton_Click"  />
                                            <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                           
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</localAsp:ModalDialog>
