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
/// This script contains the javascript component class for the deleted study search panel

// Define and register the control type.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.callBaseMethod(this, 'initialize');        
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        this._OnListControlRowClickedHandler = Function.createDelegate(this,this._OnListControlRowClicked);
        Sys.Application.add_load(this._OnLoadHandler);                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
        
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad : function()
    {
        // hook up the events... It is necessary to do this every time 
        // because NEW instances of the button and the study list components
        // may have been created as the result of the post-back
        var listCtrl = $find(this.get_ListClientID());
        listCtrl.add_onClientRowClick(this._OnListControlRowClickedHandler);
            
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnListControlRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var listCtrl = $find(this.get_ListClientID());
                      
        this._enableDeleteButton(false);
        this._enableDetailsButton(false);
            
        if (listCtrl!=null )
        {
            var rows = listCtrl.getSelectedRowElements();
                
            if (rows!=null && rows.length>0)
            {
		        this._enableDetailsButton(true);
                this._enableDeleteButton(true);                   
            }
                
        }
    },
        
    _enableDetailsButton : function(en)
    {
        var button = $find(this._ViewDetailsButtonClientID);
        if(button != null) button.set_enable(en);
    },
        
    _enableDeleteButton : function(en)
    {
        var button = $find(this._DeleteButtonClientID);
        if(button != null) button.set_enable(en);
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
        
    get_ListClientID : function() {
        return this._ListClientID;
    },

    set_ListClientID : function(value) {
        this._ListClientID = value;
        this.raisePropertyChanged('ListClientID');
    },
        
    get_ViewDetailsButtonClientID : function() {
        return this._ViewDetailsButtonClientID;
    },

    set_ViewDetailsButtonClientID : function(value) {
        this._ViewDetailsButtonClientID = value;
        this.raisePropertyChanged('ViewDetailsButtonClientID');
    },
        
    get_DeleteButtonClientID : function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID : function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    }
        
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel', Sys.UI.Control);
     
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

