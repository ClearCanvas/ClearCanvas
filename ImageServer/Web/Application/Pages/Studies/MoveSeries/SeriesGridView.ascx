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

<%@ Import namespace="Microsoft.JScript"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesGridView.ascx.cs" 
Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.SeriesGridView" %>

            <asp:GridView ID="SeriesListControl" runat="server" SkinID="GlobalGridView">
                <Columns>
                            <asp:BoundField DataField="SeriesNumber" HeaderText="<%$Resources: Labels, SeriesNumber %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="Modality" HeaderText="<%$Resources: Labels, Modality %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesDescription" HeaderText="<%$Resources:Labels, Description %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="NumberOfSeriesRelatedInstances" HeaderText="<%$Resources:Labels, Instances %>">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesInstanceUid" HeaderText="<%$Resources:Labels, SeriesInstanceUID %>">
                                <HeaderStyle Wrap="False" />  
                            </asp:BoundField>
                            <asp:TemplateField  HeaderText="<%$Resources:Labels, PerformedOn %>">
                                <ItemTemplate>

                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />  
                            </asp:TemplateField>
                                    </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="<%$Resources: SR, StudyDetails_NoSeriesForThisStudy%>" />
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
             </asp:GridView>

