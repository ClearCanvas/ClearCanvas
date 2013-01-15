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
/// This script contains the javascript component class for the work queue search panel

Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.initializeBase(this, [element]);       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.prototype = 
{
    initialize : function() {
        
        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.callBaseMethod(this, 'initialize');        
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        this._OnViewDetailsButtonClickedHandler = Function.createDelegate(this,this._OnViewDetailsButtonClickedHandler);
                        
        Sys.Application.add_load(this._OnLoadHandler);        
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
        
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad : function()
    {         
        var viewDetailsButton = $find(this._ViewStudiesButtonClientID);
        if(viewDetailsButton != null) viewDetailsButton.add_onClientClick( this._OnViewDetailsButtonClickedHandler );  
    },    

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Public methods
    _OnViewDetailsButtonClickedHandler : function(sender, event)
    {
        this._openStudy();
    },
                
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _openStudy : function()
    {
        var url = String.format('{0}?serverae={1}&siuid={2}', this._OpenStudyPageUrl, this._ServerAE, this._StudyInstanceUid);
        window.open(url);
    },
                
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_ViewStudiesButtonClientID : function() {
        return this._ViewStudiesButtonClientID;
    },
    set_ViewStudiesButtonClientID : function(value) {
        this._ViewStudiesButtonClientID = value;
        this.raisePropertyChanged('ViewStudiesButtonClientID');
    },
    
    get_OpenStudyPageUrl : function() {
        return this._OpenStudyPageUrl;
    },
    set_OpenStudyPageUrl : function(value) {
        this._OpenStudyPageUrl = value;
        this.raisePropertyChanged('OpenStudyPageUrl');
    },
    
    get_ServerAE : function() {
        return this._ServerAE;
    },
    set_ServerAE : function(value) {
        this._ServerAE = value;
        this.raisePropertyChanged('ServerAE');
    },      
        
    get_StudyInstanceUid : function() {
        return this._StudyInstanceUid;
    },
    set_StudyInstanceUid : function(value) {
        this._StudyInstanceUid = value;
        this.raisePropertyChanged('StudyInstanceUid');
    }                              
},

// Register the class as a type that inherits from Sys.UI.Control.

ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
