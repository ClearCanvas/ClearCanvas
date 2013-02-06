<%-- License

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

<%@ Page Language="C#" MasterPageFile="~/Pages/Common/MainContentSection.master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails.Default" %>

<%@ Register Src="SeriesDetailsPanel.ascx" TagName="SeriesDetailsPanel" TagPrefix="uc1" %>

<asp:Content runat="server" ID="MainMenuContent" ContentPlaceHolderID="MainMenuPlaceHolder">
    <asp:Table runat="server"><asp:TableRow><asp:TableCell HorizontalAlign="right" style="padding-top: 12px;"><asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" Text="<%$Resources: Labels, Close %>" OnClientClick="javascript: window.close(); return false;" /></asp:TableCell></asp:TableRow></asp:Table>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentSectionPlaceHolder" runat="server">
            <asp:UpdatePanel runat="server" ID="updatepanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panel1" runat="server" Width="100%">
                        <uc1:SeriesDetailsPanel id="SeriesDetailsPanel1" runat="server">
                    </uc1:SeriesDetailsPanel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>