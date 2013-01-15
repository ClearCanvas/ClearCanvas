<%-- License

Copyright (c) 2011, ClearCanvas Inc.
All rights reserved.
http://www.clearcanvas.ca

This software is licensed under the Open Software License v3.0.
For the complete license, see http://www.clearcanvas.ca/OSLv3.0
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