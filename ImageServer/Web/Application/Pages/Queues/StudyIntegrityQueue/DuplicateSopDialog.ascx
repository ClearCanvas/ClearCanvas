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
<%@ Import Namespace="ClearCanvas.ImageServer.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.DuplicateSopDialog"
   CodeBehind="DuplicateSopDialog.ascx.cs" %>
<%@ Import Namespace="Resources" %>

<script type="text/javascript">
    Sys.Application.add_load(function() {
        var useDuplicateRadioButtonRadio = $("#<%= UseDuplicateRadioButton.ClientID %>");
        var replaceAsIsRadioButton = $("#<%= ReplaceAsIsRadioButton.ClientID %>");
        
        useDuplicateRadioButtonRadio.click(function() {
            CheckDataTruncation();
        });
        replaceAsIsRadioButton.click(function() {
            CheckDataTruncation();
        });

        function CheckDataTruncation() {
            var field = $("#<%= FieldsMayTruncate.ClientID %>");
            var maytruncate = field.val() == "true";
            if (maytruncate) {
                alert("<%= SR.SIQ_DataMayBeTruncated %>");
            }
        }

    });
    
    Sys.Application.add_load(function()
        {
            var okButton = $find("<%= OKButton.ClientID %>");
            var useExistingRadio = $("#<%= UseExistingSopRadioButton.ClientID %>");
            var useDuplicateRadio = $("#<%= UseDuplicateRadioButton.ClientID %>");
            var deleteDuplicateRadio = $("#<%= DeleteDuplicateRadioButton.ClientID %>");
            var replaceAsIsRadio = $("#<%=ReplaceAsIsRadioButton.ClientID %>");
            
            var useExistingWarning = $("#<%=UseExistingWarningPanel.ClientID %>");
            var useDuplicateWarning = $("#<%=UseDuplicateWarningPanel.ClientID %>");
            
            var dataIsConsistent = <%= DataIsConsistent? "true":"false" %>;
            
            okButton.set_enable(false);            
            useExistingRadio.attr("checked", false);
            useDuplicateRadio.attr("checked", false);
            deleteDuplicateRadio.attr("checked", false);
            replaceAsIsRadio.attr("checked", false);
            useExistingWarning.hide();
            useDuplicateWarning.hide();
            
            replaceAsIsRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);
                    if (!dataIsConsistent)
                    {
                        useExistingWarning.hide();
                        useDuplicateWarning.hide();
                    }
                }
            );
            
            useExistingRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);
                    if (!dataIsConsistent)
                    {
                        useExistingWarning.show();
                        useDuplicateWarning.hide();
                    }
                }
            );
            
            useDuplicateRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);
                    if (!dataIsConsistent)
                    {
                        useExistingWarning.hide();
                        useDuplicateWarning.show();
                    }
                 }
            );
            
            deleteDuplicateRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);    
                    useExistingWarning.hide();
                    useDuplicateWarning.hide();
                }
            );
        });
</script>

<ccAsp:ModalDialog ID="DuplicateSopReconcileModalDialog" runat="server" Title="<%$ Resources:Titles, ReconcileDuplicateSOPDialog %>">
   <ContentTemplate>
      <asp:HiddenField runat="server" ID="FieldsMayTruncate" />
      <aspAjax:TabContainer runat="server" ID="TabContainer" Width="950px" ActiveTabIndex="0"
         CssClass="DialogTabControl">
         <aspAjax:TabPanel runat="server" ID="OverviewTab" HeaderText="<%$Resources: Titles, SIQ_ReconcileDialog_OverviewTabTitle %>"
            Height="100%" CssClass="DialogTabControl">
            <ContentTemplate>
               <asp:Panel ID="Panel1" runat="server" CssClass="ReconcilePanel">
                  <asp:Table ID="Table1" runat="server">
                     <asp:TableRow CssClass="ReconcileHeaderRow">
                        <asp:TableCell><%=Labels.SIQ_ReconcileDialog_ExistingStudy %></asp:TableCell>
                        <asp:TableCell CssClass="Separator">
                           <asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="1px" /></asp:TableCell>
                        <asp:TableCell><span class="ConflictingStudyTitle"><%=Labels.SIQ_ReconcileDialog_DuplicateSOP %></span></asp:TableCell>
                     </asp:TableRow>
                     <asp:TableRow>
                        <asp:TableCell ColumnSpan="3">
                           <div class="StudyInstanceUIDMessage">
                              <%=SR.StudyInstanceUID %>:
                              <asp:Label ID="StudyInstanceUIDLabel" runat="server" Text='<%# DuplicateEntryDetails.StudyInstanceUid %>'></asp:Label></div>
                        </asp:TableCell>
                     </asp:TableRow>
                     <asp:TableRow VerticalAlign="Top">
                        <asp:TableCell>
                           <asp:Table ID="Table2" runat="server">
                              <asp:TableRow>
                                 <asp:TableCell>
                                    <div class="StudyInformation">
                                       <table>
                                          <tr>
                                             <td width="130px" class="DialogLabelBackground">
                                                <asp:Label ID="Label1" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientName%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="PreformattedLabel1" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.Name %>' />
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label321" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientID%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ExistingPatientID" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.PatientID %>' />
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label322" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientBirthdate%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ExistingPatientBirthDate" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.BirthDate %>' />
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label323" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.AccessionNumber%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ExistingAccessionNumber" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ExistingStudy.AccessionNumber %>' />
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label324" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientSex%></asp:Label>
                                             </td>
                                             <td>
                                                <table cellpadding="0" cellspacing="0">
                                                   <tr>
                                                      <td>
                                                         <asp:Label ID="ExistingPatientSex" runat="server" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.Sex %>'></asp:Label>
                                                      </td>
                                                      <td>
                                                         <ccAsp:InvalidInputIndicator ID="InvalidInputIndicator1" runat="server" SkinID="InvalidInputIndicator" />
                                                      </td>
                                                   </tr>
                                                </table>
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label325" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.IssuerOfPatientID%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ExistingPatientIssuerOfPatientID" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.IssuerOfPatientID %>' />
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label7" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.StudyDate%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ExistingStudyDate" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ExistingStudy.StudyDate %>' />
                                             </td>
                                          </tr>
                                       </table>
                                    </div>
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                       <tr>
                                          <td style="padding-left: 10px; padding-right: 10px;">
                                             <div class="SeriesTitle">
                                                <%=Labels.Series%></div>
                                          </td>
                                       </tr>
                                    </table>
                                    <div class="SeriesInformation">
                                       <table cellpadding="0" cellspacing="0" width="100%">
                                          <tr>
                                             <td style="padding: 0px 12px 0px 4px;">
                                                <div class="ReconcileGridViewPanel" style="height: 150px;">
                                                   <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ExistingPatientSeriesGridView"
                                                      Width="440px" AutoGenerateColumns="false">
                                                      <Columns>
                                                         <asp:BoundField HeaderText="<%$Resources: ColumnHeaders,SeriesNumber%>" DataField="SeriesNumber" />
                                                         <asp:TemplateField HeaderText="<%$Resources: ColumnHeaders,SeriesDescription%>" HeaderStyle-HorizontalAlign="left"
                                                            ItemStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                               <asp:Label runat="server" ID="SeriesDescription" Text='<%# Eval("Description") %>'
                                                                  ToolTip='<%# Eval("SeriesInstanceUid") %>'></asp:Label>
                                                            </ItemTemplate>
                                                         </asp:TemplateField>
                                                         <asp:BoundField HeaderText="<%$Resources: ColumnHeaders,Modality%>" DataField="Modality"
                                                            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                                         <asp:BoundField HeaderText="<%$Resources: ColumnHeaders,Instances%>" DataField="NumberOfInstances" />
                                                      </Columns>
                                                      <RowStyle CssClass="ReconcileSeriesGridViewRow" />
                                                      <HeaderStyle CssClass="ReconcileSeriesGridViewHeader" />
                                                   </asp:GridView>
                                                </div>
                                             </td>
                                          </tr>
                                       </table>
                                    </div>
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                       <tr>
                                          <td style="padding-left: 10px; padding-right: 10px;">
                                             <div class="SeriesTitle">
                                                <%= Titles.DicomTagsTitle %></div>
                                          </td>
                                       </tr>
                                    </table>
                                    <div class="SeriesInformation">
                                       <table cellpadding="0" cellspacing="0" width="100%">
                                          <tr>
                                             <td style="padding: 0px 12px 0px 4px;">
                                                <div class="ReconcileGridViewPanel" style="height: 90px; margin-bottom: 10px;">
                                                   <asp:GridView runat="server" CssClass="ReconcileComparisonResultGridView" ID="ComparisonResultGridView"
                                                      Width="100%" BackColor="white" GridLines="Horizontal" BorderColor="Transparent"
                                                      AutoGenerateColumns="false">
                                                      <Columns>
                                                         <asp:BoundField HeaderText="<%$Resources:ColumnHeaders,DicomTagName %>" DataField="TagName" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" ItemStyle-VerticalAlign="Top" />
                                                         <asp:BoundField HeaderText="<%$Resources:ColumnHeaders,DicomTagDetails %>" DataField="Details" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-HorizontalAlign="Left" />
                                                      </Columns>
                                                      <RowStyle CssClass="ReconcileComparisonResultGridViewRow" />
                                                      <HeaderStyle CssClass="ReconcileComparisonResultGridViewHeader" />
                                                   </asp:GridView>
                                                </div>
                                             </td>
                                          </tr>
                                       </table>
                                    </div>
                                 </asp:TableCell>
                              </asp:TableRow>
                           </asp:Table>
                        </asp:TableCell>
                        <asp:TableCell CssClass="Separator">
                           <asp:Image ID="Image2" runat="server" SkinID="Spacer" Width="2px" /></asp:TableCell>
                        <asp:TableCell>
                           <asp:Table ID="Table3" runat="server">
                              <asp:TableRow>
                                 <asp:TableCell>
                                    <div class="StudyInformation">
                                       <table>
                                          <tr>
                                             <td width="130px" class="DialogLabelBackground">
                                                <asp:Label ID="Label2" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientName %></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ConflictingNameLabel" CssClass="StudyField"
                                                   Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name)? SR.NotSpecified: DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name%>' />
                                             </td>
                                          </tr>
                                          <tr>
                                             <td class="DialogLabelBackground">
                                                <asp:Label ID="Label3" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientID%></asp:Label>
                                             </td>
                                             <td>
                                                <ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIDLabel" CssClass="StudyField"
                                                   Text='<%# DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientId %>' />
                                             </td>
                                             <tr>
                                                <td class="DialogLabelBackground">
                                                   <asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientBirthdate %></asp:Label>
                                                </td>
                                                <td>
                                                   <ccUI:PreformattedLabel runat="server" ID="ConflictingPatientBirthDate" CssClass="StudyField"
                                                      Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate)? SR.NotSpecified:DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate %>' />
                                                </td>
                                             </tr>
                                             <tr>
                                                <td class="DialogLabelBackground">
                                                   <asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.AccessionNumber %></asp:Label>
                                                </td>
                                                <td>
                                                   <ccUI:PreformattedLabel runat="server" ID="ConflictingAccessionNumberLabel" CssClass="StudyField"
                                                      Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.AccessionNumber)? SR.NotSpecified:DuplicateEntryDetails.ConflictingImageSet.StudyInfo.AccessionNumber %>' />
                                                </td>
                                             </tr>
                                             <tr>
                                                <td class="DialogLabelBackground">
                                                   <asp:Label ID="Label6" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.PatientSex %></asp:Label>
                                                </td>
                                                <td>
                                                   <table>
                                                      <tr>
                                                         <td>
                                                            <asp:TextBox ID="ConflictingPatientSex" runat="server" CssClass="StudyInfoField"
                                                               BorderWidth="0" ReadOnly="true" Width="95" ValidationGroup="DuplicateSOPValidationGroup"
                                                               BorderStyle="None" BackColor="Transparent" Font-Size="14px" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex)? SR.NotSpecified:DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex %>'></asp:TextBox>
                                                         </td>
                                                         <td>
                                                            <ccUI:Warning runat="server" ID="UnknownSexWarning" SkinID="<%$ Image : Warning %>"
                                                               Message="<%$Resources: InputValidation, SIQ_ReconcileDialog_PatientSexOverriddenOnMerge%>" />
                                                         </td>
                                                      </tr>
                                                   </table>
                                                </td>
                                             </tr>
                                             <tr>
                                                <td class="DialogLabelBackground">
                                                   <asp:Label ID="Label8" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.IssuerOfPatientID %></asp:Label>
                                                </td>
                                                <td>
                                                   <ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIssuerOfPatientID" CssClass="StudyField"
                                                      Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId)? SR.NotSpecified:DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId %>' />
                                                </td>
                                             </tr>
                                             <tr>
                                                <td class="DialogLabelBackground">
                                                   <asp:Label ID="Label9" runat="server" CssClass="DialogTextBoxLabel"><%=Labels.StudyDate %></asp:Label>
                                                </td>
                                                <td>
                                                   <ccUI:PreformattedLabel runat="server" ID="ConflictingStudyDate" CssClass="StudyField"
                                                      Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.StudyDate)? SR.NotSpecified:DuplicateEntryDetails.ConflictingImageSet.StudyInfo.StudyDate %>' />
                                                </td>
                                             </tr>
                                       </table>
                                    </div>
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                       <tr>
                                          <td style="padding-left: 10px; padding-right: 10px;">
                                             <div class="SeriesTitle">
                                                <%=Labels.Series %></div>
                                          </td>
                                       </tr>
                                    </table>
                                    <div class="SeriesInformation">
                                       <table cellpadding="0" cellspacing="0" width="100%">
                                          <tr>
                                             <td style="padding: 0px 12px 0px 4px;">
                                                <div class="ReconcileGridViewPanel" style="height: 150px;">
                                                   <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ConflictingPatientSeriesGridView"
                                                      Width="440px" AutoGenerateColumns="false">
                                                      <Columns>
                                                         <asp:BoundField HeaderText="<%$Resources:ColumnHeaders,SeriesNumber %>" DataField="SeriesNumber" />
                                                         <asp:TemplateField HeaderText="<%$Resources:ColumnHeaders,SeriesDescription %>" HeaderStyle-HorizontalAlign="left"
                                                            ItemStyle-HorizontalAlign="Left">
                                                            <ItemTemplate>
                                                               <asp:Label runat="server" ID="SeriesDescription" Text='<%# Eval("SeriesDescription") %>'
                                                                  ToolTip='<%# Eval("SeriesInstanceUid") %>'></asp:Label>
                                                            </ItemTemplate>
                                                         </asp:TemplateField>
                                                         <asp:BoundField HeaderText="<%$Resources:ColumnHeaders,Modality %>" DataField="Modality"
                                                            HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                                         <asp:BoundField HeaderText="<%$Resources:ColumnHeaders,Instances %>" DataField="NumberOfInstances" />
                                                      </Columns>
                                                      <RowStyle CssClass="ReconcileSeriesGridViewRow" />
                                                      <HeaderStyle CssClass="ReconcileSeriesGridViewHeader" />
                                                   </asp:GridView>
                                                </div>
                                             </td>
                                          </tr>
                                       </table>
                                    </div>
                                 </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                 <asp:TableCell Style="padding: 0px 10px 10px 10px;">
                                    <asp:Panel ID="Panel2" runat="server" CssClass="ReconcileButtonsTable">
                                       <asp:Table runat="server" ID="OptionTable" Width="100%" CellPadding="0" CellSpacing="0"
                                          Style="margin-left: 5px;">
                                          <asp:TableRow>
                                             <asp:TableCell ColumnSpan="2"><span style="font-size: 8px;">&nbsp;</span></asp:TableCell>
                                          </asp:TableRow>
                                          <asp:TableRow>
                                             <asp:TableCell>
                                                <asp:RadioButton runat="server" ID="UseExistingSopRadioButton" Text="<%$Resources: InputLabels, SIQ_ReconcileDuplicateDialog_UseExistingDemographics %>"
                                                   GroupName="DuplicateSopDecision" Checked="true" /></asp:TableCell>
                                             <asp:TableCell>
                                                <asp:RadioButton runat="server" ID="UseDuplicateRadioButton" Text="<%$Resources: InputLabels, SIQ_ReconcileDuplicateDialog_UseDuplicateDemographics %>"
                                                   GroupName="DuplicateSopDecision" Checked="false" /></asp:TableCell>
                                          </asp:TableRow>
                                          <asp:TableRow>
                                             <asp:TableCell>
                                                <asp:RadioButton runat="server" ID="ReplaceAsIsRadioButton" Text="<%$Resources: InputLabels, SIQ_ReconcileDuplicateDialog_ReplaceAsIs %>"
                                                   GroupName="DuplicateSopDecision" Checked="false" /></asp:TableCell>
                                             <asp:TableCell>
                                                <asp:RadioButton runat="server" ID="DeleteDuplicateRadioButton" Text="<%$Resources: InputLabels, SIQ_ReconcileDuplicateDialog_DeleteDuplicates %>"
                                                   GroupName="DuplicateSopDecision" CssClass="ReconcileRadioButton" /></asp:TableCell>
                                          </asp:TableRow>
                                          <asp:TableRow>
                                             <asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
                                          </asp:TableRow>
                                          <asp:TableRow runat="server" ID="OverwritewWarningPanel" Style="padding: 2px;">
                                             <asp:TableCell ColumnSpan="2">
                                                <asp:Panel runat="server" ID="UseExistingWarningPanel" CssClass="OverwritewWarningPanel"
                                                   Style="margin-right: 10px; margin-bottom: 10px;">
                                                   <%=SR.SIQ_ReconcileDuplicateDialog_UseExistingWarning %>
                                                </asp:Panel>
                                                <asp:Panel runat="server" ID="UseDuplicateWarningPanel" CssClass="OverwritewWarningPanel"
                                                   Style="margin-right: 10px; margin-bottom: 10px;">
                                                   <%=SR.SIQ_ReconcileDuplicateDialog_UseDuplicateWarning %>
                                                </asp:Panel>
                                             </asp:TableCell>
                                          </asp:TableRow>
                                       </asp:Table>
                                    </asp:Panel>
                                 </asp:TableCell>
                              </asp:TableRow>
                           </asp:Table>
                        </asp:TableCell>
                     </asp:TableRow>
                  </asp:Table>
               </asp:Panel>
            </ContentTemplate>
         </aspAjax:TabPanel>
         <aspAjax:TabPanel runat="server" ID="DetailsTab" HeaderText="<%$Resources: Titles, SIQ_ReconcileDialog_AdditionalInfoTabTitle %>">
            <ContentTemplate>
               <asp:Panel ID="Panel4" runat="server" Height="100%">
                  <asp:Panel ID="Panel5" runat="server" CssClass="AdditionalInformationPanel">
                     <table width="100%">
                        <tr>
                           <td colspan="2">
                              <div class="AdditionalInfoSectionHeader FilesystemSectionHeader">
                                 <%= Labels.SIQ_ReconcileDialog_FilesystemLocations %></div>
                           </td>
                        </tr>
                        <tr>
                           <td class="DialogLabelBackground" style="margin-left: 5px;">
                              <asp:Label ID="Label10" runat="server" CssClass="DialogTextBoxLabel" Text="<%$Resources: Labels,SIQ_ReconcileDialog_StudyLocation %>"></asp:Label>
                           </td>
                           <td>
                              <asp:Label runat="server" ID="StudyLocation"></asp:Label>
                           </td>
                        </tr>
                        <tr>
                           <td class="DialogLabelBackground" style="margin-left: 5px;">
                              <asp:Label ID="Label12" runat="server" CssClass="DialogTextBoxLabel" Text="<%$Resources: Labels,SIQ_ReconcileDialog_DuplicateSOPLocation %>"></asp:Label>
                           </td>
                           <td>
                              <asp:Label runat="server" ID="DuplicateSopLocation"></asp:Label>
                           </td>
                        </tr>
                     </table>
                  </asp:Panel>
               </asp:Panel>
            </ContentTemplate>
         </aspAjax:TabPanel>
      </aspAjax:TabContainer>
      <table cellpadding="0" cellspacing="0" width="100%">
         <tr>
            <td align="right">
               <asp:Panel ID="Panel6" runat="server" CssClass="DefaultModalDialogButtonPanel">
                  <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OKButton%>" OnClick="OKButton_Click" />
                  <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                     OnClick="CancelButton_Click" />
               </asp:Panel>
            </td>
         </tr>
      </table>
   </ContentTemplate>
</ccAsp:ModalDialog>
<ccAsp:MessageBox runat="server" ID="MessageBox" />
