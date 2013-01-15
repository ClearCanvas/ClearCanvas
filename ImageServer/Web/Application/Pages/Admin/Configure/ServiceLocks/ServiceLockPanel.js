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
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.prototype = 
{
    initialize : function() {
                       
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.callBaseMethod(this, 'initialize');        
            
        this._OnServiceLockListRowClickedHandler = Function.createDelegate(this,this._OnServiceLockListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                          
        var serviceLocklist = $find(this._ServiceLockListClientID);
        serviceLocklist.add_onClientRowClick(this._OnServiceLockListRowClickedHandler);
                           
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnServiceLockListRowClicked : function(sender, event)
    {           
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var serviceLocklist = $find(this._ServiceLockListClientID);               
                               
        this._enableEditButton(false);
                                           
        if (serviceLocklist!=null )
        {
            var rows = serviceLocklist.getSelectedRowElements();

            if(rows != null && rows.length > 0) {
                this._enableEditButton(true);
            }
        }
    },

    _enableEditButton : function(en)
    {
        var editButton = $find(this._EditButtonClientID);
        editButton.set_enable(en);
    },       

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_EditButtonClientID : function() {
        return this._EditButtonClientID;
    },

    set_EditButtonClientID : function(value) {
        this._EditButtonClientID = value;
        this.raisePropertyChanged('EditButtonClientID');
    },
               
    get_ServiceLockListClientID : function() {
        return this._ServiceLockListClientID;
    },

    set_ServiceLockListClientID : function(value) {
        this._ServiceLockListClientID = value;
        this.raisePropertyChanged('ServiceLockListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();