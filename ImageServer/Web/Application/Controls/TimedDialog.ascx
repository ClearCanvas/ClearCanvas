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
<%@ Import Namespace="Resources" %>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="TimedDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.TimedDialog" %>
<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="uc1" %>
<uc1:ModalDialog ID="ModalDialog" runat="server" Title="" >
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server">
            <asp:Panel ID="Panel1" runat="server">
                <asp:Timer ID="TimerTimeout" runat="server" Interval="10" OnTick="TimerTimout_Tick" Enabled="false"/>
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="1" style="height: 24px">
                            <asp:Image ID="IconImage" runat="server" Visible="false" /></td>
                        <td colspan="2" style="height: 24px; padding-right: 10px; padding-left: 10px; padding-bottom: 10px;
                            vertical-align: top; padding-top: 10px; text-align: center;">
                            <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message">
                            </asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="center">
                            <asp:Panel ID="ButtonPanel" runat="server">
                                 <table width="50%" cellspacing="10">
                                    <tr>
                                         <td>
                                            <asp:Button ID="OKButton" runat="server" Text="OK" OnClick="OKButton_Click" Width="77px" />
                                        </td>                                       
                                    </tr>
                                </table>
                            </asp:Panel>                          
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</uc1:ModalDialog>
