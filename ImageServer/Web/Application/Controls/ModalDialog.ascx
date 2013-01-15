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

<%@ Import Namespace="System.Net.Mime" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ModalDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ModalDialog" %>
<%@ Import Namespace="Resources" %>

<asp:UpdatePanel ID="ModalDialogUpdatePanel" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
        <!-- Dialog Box -->
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                
                <asp:Table ID="DialogContainer" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0px"
                    Style="display: none;">
                    <asp:TableRow ID="TableRow1" runat="server">
                        <asp:TableCell>
                            <asp:Panel runat="server" ID="TitleBarPanel">
                                <asp:Panel runat="server" ID="DefaultTitlePanel">
                                    <asp:Table ID="Table3" runat="server" CellPadding="0" CellSpacing="0" Width="100%">
                                        <asp:TableRow>
                                            <asp:TableCell ID="TitleBarLeft" CssClass="DefaultModalDialogTitleBarLeft">
                                            </asp:TableCell>
                                            <asp:TableCell ID="TitleBarCenter" CssClass="DefaultModalDialogTitleBarTitle" Width="100%">
                                                    <asp:Panel runat="server" style="padding-bottom: 4px">
                                                        <asp:Label ID="TitleLabel" runat="server" Text="&nbsp;"></asp:Label>
                                                        </asp:Panel>                                                    
                                            </asp:TableCell>
                                            <asp:TableCell CssClass="DefaultModalDialogTitleBarRight" HorizontalAlign="Right">
                                                <asp:UpdateProgress runat="server" ID="UpdateProgress" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="1000">
                                                    <ProgressTemplate>
                                                        <asp:Image ID="Image5" runat="server" SkinID="AjaxLoadingIndicator" style="display:block; padding-right: 5px; padding-bottom: 5px;"/>     
                                                    </ProgressTemplate>
                                                </asp:UpdateProgress>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="CustomizedTitleBarPanel">
                                    <asp:PlaceHolder ID="TitlePanelPlaceHolder" runat="server"></asp:PlaceHolder>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Panel runat="server" ID="ContentPanel" CssClass="DefaultModalDialogContent">
                                <asp:PlaceHolder ID="ContentPlaceHolder" runat="server"></asp:PlaceHolder>
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>

            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- Covers the entire background and prevents the user from clicking on the screen outside of
             the dialog box -->
        <asp:Panel ID="DummyPanel" runat="server" Height="10px" Style="left: 26px; position: absolute;
            top: -5px" Width="115px">
        </asp:Panel>
        <aspAjax:ModalPopupExtender ID="ModalPopupExtender" runat="server" TargetControlID="DummyPanel" 
            PopupControlID="DialogContainer" PopupDragHandleControlID="TitleBarPanel" RepositionMode="None">
        </aspAjax:ModalPopupExtender>
        


    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress runat="server" ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="1000">
    <ProgressTemplate>
        <div class="ScreenBlocker" style="z-index:100003">
            
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
                
        
