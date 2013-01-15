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

// Define and register the control type.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.prototype = 
{
    initialize : function() {
        
        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.callBaseMethod(this, 'initialize');        
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        this._OnItemListRowClickedHandler = Function.createDelegate(this,this._OnItemListRowClicked);

        this._OnItemListRowDblClickedHandler = Function.createDelegate(this,this._OnItemListRowDblClicked);
        this._OnViewDetailsButtonClickedHandler = Function.createDelegate(this,this._OnItemListRowDblClicked);  //Does the same thing as the double-clicked event.
                        
                        
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
  
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
        
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad : function()
    {
        var itemlist = $find(this._ItemListClientID);
        itemlist.add_onClientRowClick(this._OnItemListRowClickedHandler);
        itemlist.add_onClientRowDblClick(this._OnItemListRowDblClickedHandler);
            
        var viewDetailsButton = $find(this._ViewDetailsButtonClientID);
        if(viewDetailsButton != null) viewDetailsButton.add_onClientClick( this._OnViewDetailsButtonClickedHandler );  
            
        this._updateToolbarButtonStates();    
    },
           
    // called when user clicked on a row in the study list
    _OnItemListRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
        
    // called when user double-clicked on a row in the study list
    _OnItemListRowDblClicked : function(sender, event)
    {
        this._updateToolbarButtonStates();
        this._openSelectedItems();
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Public methods
    _updateToolbarButtonStates : function()
    {
        var itemlist = $find(this._ItemListClientID);
                      
        this._enableViewDetailsButton(false);
        this._enableRescheduleButton(false);
        this._enableResetButton(false);
        this._enableDeleteButton(false);
        this._enableReprocessButton(false);
            
        if (itemlist!=null )
        {
            var rows = itemlist.getSelectedRowElements();
            if (rows.length>0)
            {
                for(i=0; i<rows.length; i++)
                {
                    this._enableViewDetailsButton(true);
                    this._enableRescheduleButton(this._canRescheduleItem(rows[i]));
                    this._enableResetButton(this._canResetItem(rows[i]));
                    this._enableDeleteButton(this._canDeleteItem(rows[i]));
                    this._enableReprocessButton(this._canReprocessItem(rows[i]));
                }
            }
        }
    },     
        
    _openSelectedItems : function()
    {
        var itemlist = $find(this._ItemListClientID);
        // open the selected studies
        if (itemlist!=null )
        {
            var rows = itemlist.getSelectedRowElements();
            if (rows.length>0)
            {
                for(i=0; i<rows.length; i++)
                {
                    var instanceuid = rows[i].getAttribute('WorkQueueUID');
                    if (instanceuid!=undefined)
                    {
                        var url= String.format('{0}?&uid={1}', this._ViewItemDetailsUrl, instanceuid);
                        window.open(url);
                    }
                }
                    
            }
        }
    },
                
    _canRescheduleItem:function(row)
    {
        //"canreconcile" is a custom attribute injected by the list control
        return row.getAttribute('canreschedule')=='true';
    },
        
    _canResetItem:function(row)
    {
        //"canreconcile" is a custom attribute injected by the list control
        return row.getAttribute('canreset')=='true';
    },
        
    _canDeleteItem:function(row)
    {
        //"canreconcile" is a custom attribute injected by the list control
        return row.getAttribute('candelete')=='true';
    },
        
    _canReprocessItem:function(row)
    {
        //"canreconcile" is a custom attribute injected by the list control
        return row.getAttribute('canreprocess')=='true';
    },
        
    _enableViewDetailsButton : function(en)
    {
        var viewDetailsButton = $find(this._ViewDetailsButtonClientID);
        if(viewDetailsButton != null) viewDetailsButton.set_enable(en);
    },
        
    _enableRescheduleButton : function(en)
    {
        var rescheduleButton = $find(this._RescheduleButtonClientID);
        if(rescheduleButton != null) rescheduleButton.set_enable(en);
    },
        
    _enableDeleteButton : function(en)
    {
        var deleteButton = $find(this._DeleteButtonClientID);
        if(deleteButton != null) deleteButton.set_enable(en);
    },
        
    _enableReprocessButton : function(en)
    {
        var reprocessButton = $find(this._ReprocessButtonClientID);
        if(reprocessButton != null) reprocessButton.set_enable(en);
    },
        
    _enableResetButton : function(en)
    {
        var resetButton = $find(this._ResetButtonClientID);
        if(resetButton != null) resetButton.set_enable(en);
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_ItemListClientID : function() {
        return this._ItemListClientID;
    },

    set_ItemListClientID : function(value) {
        this._ItemListClientID = value;
        this.raisePropertyChanged('ItemListClientID');
    },
        
    get_ViewDetailsButtonClientID : function() {
        return this._ViewDetailsButtonClientID;
    },

    set_ViewDetailsButtonClientID : function(value) {
        this._ViewDetailsButtonClientID = value;
        this.raisePropertyChanged('ViewDetailsButtonClientID');
    },
        
    get_RescheduleButtonClientID : function() {
        return this._RescheduleButtonClientID;
    },

    set_RescheduleButtonClientID : function(value) {
        this._RescheduleButtonClientID = value;
        this.raisePropertyChanged('RescheduleButtonClientID');
    },
        
    get_ResetButtonClientID : function() {
        return this._ResetButtonClientID;
    },

    set_ResetButtonClientID : function(value) {
        this._ResetButtonClientID = value;
        this.raisePropertyChanged('ResetButtonClientID');
    },
        
    get_DeleteButtonClientID : function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID : function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    },
        
    get_ReprocessButtonClientID : function() {
        return this._ReprocessButtonClientID;
    },

    set_ReprocessButtonClientID : function(value) {
        this._ReprocessButtonClientID = value;
        this.raisePropertyChanged('ReprocessButtonClientID');
    },
        
        get_ViewItemDetailsUrl : function() {
        return this._ViewItemDetailsUrl;
    },
       
    set_ViewItemDetailsUrl : function(value) {
        this._ViewItemDetailsUrl = value;
        this.raisePropertyChanged('ViewItemDetailsUrl');
    }         
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
