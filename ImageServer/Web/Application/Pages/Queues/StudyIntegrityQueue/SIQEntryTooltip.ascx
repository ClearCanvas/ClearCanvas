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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SIQEntryTooltip.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SIQEntryTooltip" %>

<style type="text/css">
    .WrapAtRightEdge
    {
    	overflow-y:auto;
    	overflow-x:auto; 
    	word-break:break-all;
    }
    
</style>

<asp:Panel runat="server" ID="Container" >
    <table>
        <tr>
            <td colspan="2">
                <asp:HyperLink runat="server" ID="StudyLink" Target="_blank"/><span> </span>
                <asp:Label runat="server" ID="Note" ForeColor="Red" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td><%= Labels.SIQ_ReconcileDialog_StudyLocation %></td>
            <td><asp:Label runat="server" ID="FilesystemPath" CssClass="WrapAtRightEdge"/></td>
        </tr>

        <tr>
            <td><%= Labels.SIQ_ReconcileDialog_ConflictingImageLocation %></td>
            <td><asp:Label runat="server" ID="ReconcilePath"  CssClass="WrapAtRightEdge"/></td>
        </tr>
    </table>
</asp:Panel>