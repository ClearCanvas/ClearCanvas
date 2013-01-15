// License

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

// End License

/////////////////////////////////////////////////////////////////////////////////////////////////////////
/// This script contains the javascript component class for the study search panel

// Define and register the control type.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.initializeBase(this, [element]);
       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.callBaseMethod(this, 'initialize');        
            
        this._OnUserGroupsListRowClickedHandler = Function.createDelegate(this,this._OnUserGroupsListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                    
        var userlist = $find(this._UserGroupsListClientID);
        userlist.add_onClientRowClick(this._OnUserGroupsListRowClickedHandler);
                 
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnUserGroupsListRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var userlist = $find(this._UserGroupsListClientID);
                      
        this._enableEditButton(false);
        this._enableDeleteButton(false);
                               
        if (userlist!=null )
        {
            var rows = userlist.getSelectedRowElements();

            if(rows != null && rows.length > 0) {
                this._enableEditButton(true);
                this._enableDeleteButton(true);
            }
        }
    },
        
    _enableDeleteButton : function(en)
    {
        var deleteButton = $find(this._DeleteButtonClientID);
        deleteButton.set_enable(en);
    },
        
    _enableEditButton : function(en)
    {
        var editButton = $find(this._EditButtonClientID);
        editButton.set_enable(en);
    },       


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_DeleteButtonClientID : function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID : function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    },
        
    get_EditButtonClientID : function() {
        return this._EditButtonClientID;
    },

    set_EditButtonClientID : function(value) {
        this._EditButtonClientID = value;
        this.raisePropertyChanged('EditButtonClientID');
    },
        
    get_UserGroupsListClientID : function() {
        return this._UserGroupsListClientID;
    },

    set_UserGroupsListClientID : function(value) {
        this._UserGroupsListClientID = value;
        this.raisePropertyChanged('UserGroupsListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel', Sys.UI.Control);
     
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();