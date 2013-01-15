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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerPartitionSelector.ascx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ServerPartitionSelector" %>

<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Security"%>
<%@ Import Namespace="Resources" %>

<asp:Panel runat="server" ID="PartitionPanel" CssClass="PartitionPanel">
    <asp:Label ID="PartitionLabel" runat="server" Text="<%$Resources: Labels,Partitions %>" CssClass="SearchTextBoxLabel" EnableViewState="False" style="padding-left: 5px;"/>
    <asp:Panel ID="PartitionLinkPanel" runat="server" Visible="True" Wrap="True"/> 
    
</asp:Panel>
  
<asp:Panel runat="server" ID="NoPartitionPanel" CssClass="PartitionPanel" Visible="false">
    <% if (SessionManager.Current.User.IsInRole(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Configuration.ServerPartitions)){%>
    <asp:Panel ID="Panel1" runat="server" CssClass="AddPartitionMessage">
        <asp:Literal ID="Literal1" runat="server" Text="<%$Resources: SR, NoPartitionAvailable %>"></asp:Literal>
        <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/Pages/Admin/Configure/ServerPartitions/Default.aspx" CssClass="AddPartitionLink" Text="<%$Resources:Labels,AddNewPartition %>"></asp:LinkButton>
    </asp:Panel>
    
    <%} else {%>
    <asp:Panel ID="Panel2" runat="server" CssClass="AddPartitionMessage">
        <asp:Literal ID="Literal2" runat="server" Text="<%$Resources: SR, NoPartitionAvailable %>"></asp:Literal>
        <asp:Literal ID="Literal3" runat="server" Text="<%$Resources: SR, ContactAdmin %>"></asp:Literal>
        
    </asp:Panel>
    
    <%}%>
</asp:Panel>
    
