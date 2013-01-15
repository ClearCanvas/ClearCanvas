<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/Common/MainContentSection.Master" Title="ClearCanvas ImageServer" Codebehind="WorkQueueItemDeleted.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDeleted" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="right" style="padding-top: 12px;">
                <asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" Text="<%$Resources: Labels,Close %>" OnClientClick="javascript: window.close(); return false;"></asp:LinkButton> 
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentSectionPlaceHolder" >
    <table width="100%" cellpadding="0" cellspacing="0">
                <tr><td class="MainContentTitle"><asp:Label ID="WorkQueueItemTitle" runat="server" Text="<%$Resources: Titles, WorkQueueItemDetails %>"></asp:Label></td></tr>
                <tr><td>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td>
                                    <ccUI:ToolbarButton ID="RescheduleToolbarButton" runat="server" SkinID="<%$Image:RescheduleButton%>"/>
                                    <ccUI:ToolbarButton ID="ResetButton" runat="server" SkinID="<%$Image:ResetButton%>" />
                                    <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="<%$Image:DeleteButton%>" />
                                    <ccUI:ToolbarButton ID="ReprocessButton" runat="server" SkinID="<%$Image:ReprocessButton%>"/>                                    
                            </td></tr>
                            <tr><td>
                                <asp:Panel ID="Panel1" runat="server" BackColor="white" style="padding: 10px; border: solid 1px #305ba6">
                                    <asp:Panel ID="Panel2" runat="server" CssClass="WorkQueueItemDeletedMessage" style="margin: 0px;">
                                        <%= Resources.SR.WorkQueueSuccessfullyDeleted %>
                                    </asp:Panel>
                                </asp:Panel>                            
                            </td></tr>
                       </table>
                  </td></tr>                    
              </table>
 </asp:Content>