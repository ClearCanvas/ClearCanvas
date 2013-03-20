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


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.ServerPartitionPanel" %>

<%@ Register Src="ServerPartitionGridPanel.ascx" TagName="ServerPartitionGridPanel" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    
        <script>
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
    </script>
         
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell Wrap="false">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="right" valign="bottom">
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, AETitle %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <ccUI:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox"></ccUI:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, ServerPartitionDescription %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <ccUI:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox"></ccUI:TextBox>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, ServerPartitionStatus %>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList>
                                            </td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="<%$Image:SearchIcon%>" OnClick="SearchButton_Click"/></asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                                        <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddPartitionButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="AddPartitionButton_Click" />
                                        <ccUI:ToolbarButton ID="EditPartitionButton" runat="server" SkinID="<%$Image:EditButton%>" OnClick="EditPartitionButton_Click" />
                                        <ccUI:ToolbarButton ID="DeletePartitionButton" runat="server" SkinID="<%$Image:DeleteButton%>" OnClick="DeletePartitionButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server"  CssClass="SearchPanelResultContainer">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;">                                    <localAsp:ServerPartitionGridPanel ID="ServerPartitionGridPanel" runat="server" Height="500px" /></td></tr>
                            </table>                        
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>                
                </asp:TableRow>
            </asp:Table>
            
    </ContentTemplate>
</asp:UpdatePanel>
