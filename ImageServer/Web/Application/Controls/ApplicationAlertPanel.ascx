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


<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ApplicationAlertPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ApplicationAlertPanel" %>
<%@ Import Namespace="Resources" %>


   <!--[if gte IE 5.5]>
    <![if lt IE 7]>
    <style type="text/css">
        .FixedPos {
            /* used by  IE6 */
	        left: expression( ( ( ignoreMe2 = document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft ) ) + 'px' );
            top: expression( ( ( ignoreMe = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ) ) + 'px' );
            position: absolute;
//            z-index: 10000000;
        }
        
    </style>
    <![endif]>
    <![endif]-->

    <!--[if IE]>
    <style type="text/css">
        .FixedPos {
            /* used by  IE6 */
	        position: fixed;
//            z-index: 10000000;
        }
        
        .ScreenBlocker {
            /* used by  IE */
            left: expression( ( ( ignoreMe2 = document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft ) ) + 'px' );
            top: expression( ( ( ignoreMe = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ) ) + 'px' );
            width:expression( ( ( ignoreMe = document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body.clientWidth ) ) + 'px' );
            height:expression( ( ( ignoreMe = document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight ) ) + 'px' );
            position:absolute;
            background-color:white;
	        filter:alpha(opacity=0);
	
        }        

        html,body {height:100%}    
    </style>
    <![endif]-->
    
    <!--[if !IE]>
    <style type="text/css">
        .ScreenBlocker {
            /* used by  IE */
            left: 0;
            top:0;
            width:100%;
            height:100%;
            position:absolute;
            background-color:white;
	        opacity: 0; /* Safari, Opera */
	        -moz-opacity: 0; /* FireFox */
        }   
    </style>
    <![endif]-->

<script type="text/javascript">

function RaiseAppAlert(html, stay)
        {
            $("#<%= AppAlertMessagePanel.ClientID %>").html(html).slideDown("slow",
                function()
                {
                    setTimeout(function(){ $("#<%= AppAlertMessagePanel.ClientID %>").hide();}, stay);
                }
            );
        }

</script>

<div class="FixedPos">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:Panel runat="server" ID="AppAlertMessagePanel" CssClass="AppAlertMessagePanel" />
            </td>
        </tr>
    </table>
</div>
