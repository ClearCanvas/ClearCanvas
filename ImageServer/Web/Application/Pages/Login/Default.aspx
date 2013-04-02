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


<%@ Page Language="C#" AutoEventWireup="True" Codebehind="LoginPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Login.LoginPage" %>
<%@ Import Namespace="ClearCanvas.Common"%>
<%@ Import Namespace="System.Threading"%>
<%@ Import namespace="ClearCanvas.ImageServer.Common"%> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="ChangePasswordDialog.ascx" TagName="ChangePasswordDialog" TagPrefix="localAsp" %>
<%@ Register Src="PasswordExpiredDialog.ascx" TagName="PasswordExpiredDialog" TagPrefix="localAsp" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">

</head>
<body class="LoginBody" runat="server">
    
    <form runat="server">

    <ccAsp:CheckJavascript runat="server" ID="CheckJavascript" />          

    <asp:ScriptManager ID="GlobalScriptManager" runat="server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true">
    </asp:ScriptManager>
    
    
    <asp:Panel ID="LoginSplash" DefaultButton="LoginButton" runat="server">
        <asp:Image ID="SplashScreen" runat="server"/>
        <div id="VersionInfoPanel">
            <table cellpadding="1">
            <tr><td align="right">
                        <asp:Label ID="Label1" runat="server" Text="<%$Resources: Labels,Version %>"></asp:Label>:
                        <asp:Label runat="server" ID="VersionLabel" />
                        <%--<%= String.IsNullOrEmpty(ServerPlatform.VersionString) ? Resources.SR.Unknown : ServerPlatform.VersionString%>--%>
                        </td>
                        </tr>
             <tr><td align="right">
                    <asp:Label runat="server" ID="LanguageLabel" />
                        <%--<%= Thread.CurrentThread.CurrentUICulture.NativeName %>--%></td></tr>
            </table>
        </div>
        <div id="WarningLabel">
            <asp:Label runat="server" ID="ManifestWarningTextLabel" CssClass="ManifestWarningTextLabel"></asp:Label>
        </div>
        <div id="Copyright">
           <asp:Label runat="server" ID="CopyrightLabel">
            <%--<%= ProductInformation.Copyright %>--%>
           </asp:Label>        
        </div>
        
        <div id="LoginCredentials">
        <table>
            <tr>
            <td align="right"><asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels,UserID %>"></asp:Label></td>
            <td align="right"><asp:TextBox runat="server" ID="UserName" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr>
            <tr>
            <td align="right"><asp:Label ID="Label3" runat="server" Text="<%$Resources: Labels,Password %>"></asp:Label></td>
            <td align="right"><asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr> 
            <tr>
                <td colspan="2" align="right"><asp:Button runat="server" ID="LoginButton" OnClick="LoginClicked"  Text="<%$Resources: Labels,ButtonLogin %>" CssClass="LoginButton"/></td>
            </tr>               
            <tr>
                <td colspan="2" align="right" ><asp:LinkButton ID="LinkButton1" runat="server" CssClass="LoginLink" OnClick="ChangePassword"><asp:Label ID="Label4" runat="server" Text="<%$Resources: Labels,ChangePassword%>"></asp:Label></asp:LinkButton></td>            
            </tr>
        </table>
          
        </div>

       

     </asp:Panel>  
     
     <div style="padding: 1px">
         <asp:Panel CssClass="LoginErrorMessagePanel" runat="server" ID="ErrorMessagePanel" 
            Visible='<%# !String.IsNullOrEmpty(Page.Request.QueryString["error"]) %>'>
             <div style="margin:5px;">
                <asp:Label runat="server" ID="ErrorMessage" ForeColor="red" Text='<%# Page.Request.QueryString["error"] %>' />
            </div>
        </asp:Panel>  
     </div>
     
     
     <asp:Panel ID="Panel1" runat="server" CssClass="BrowserWarningMessagePanel">
            <asp:Label runat="server" ID="Label5" CssClass="BrowserWarningLabel" Text="<%$Resources: Labels,PrivateBrowsing %>"></asp:Label>
     </asp:Panel> 
     
             
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <localAsp:ChangePasswordDialog runat="server" id="ChangePasswordDialog" />
            <localAsp:PasswordExpiredDialog runat="server" id="PasswordExpiredDialog" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    </form>
</body>
</html>
