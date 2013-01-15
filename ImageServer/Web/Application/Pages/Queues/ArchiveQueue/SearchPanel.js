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
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue');
        
/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.initializeBase(this, [element]);
},
   
/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.callBaseMethod(this, 'initialize');        
            
        this._OnItemListRowClickedHandler = Function.createDelegate(this,this._OnItemListRowClicked);
        this._OnItemListRowDblClickedHandler = Function.createDelegate(this,this._OnItemListRowDblClicked);
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
	    this._OnOpenButtonClickedHandler = Function.createDelegate(this,this._OnOpenButtonClicked);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.callBaseMethod(this, 'dispose');
            
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
        var itemlist = $find(this._ItemListClientID);
        itemlist.add_onClientRowClick(this._OnItemListRowClickedHandler);
        itemlist.add_onClientRowDblClick(this._OnItemListRowDblClickedHandler);

        var openButton = $find(this._OpenButtonClientID);
        if(openButton != null) openButton.add_onClientClick( this._OnOpenButtonClickedHandler );   

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
        this._openSelectedStudies();
    },
        
    // called when the Open Study button is clicked
    _OnOpenButtonClicked : function(src, event)
    {
        this._openSelectedStudies();            
    },
        
    _openSelectedStudies : function()
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
                    var instanceuid = this._getInstanceUid(rows[i]);
                    var serverae = this._getServerPartitionAE(rows[i]);
                    if (instanceuid!=undefined && serverae!=undefined)
                    {
                        var url= String.format('{0}?serverae={1}&siuid={2}', this._OpenStudyPageUrl, serverae, instanceuid);
                        window.open(url);
                    }
                }
            }
        }
    },
                      
    _updateToolbarButtonStates : function()
    {
        var itemlist = $find(this._ItemListClientID);
                      
        if (itemlist!=null )
        {
            var rows = itemlist.getSelectedRowElements();
                
            if (rows.length>0)
            {
                this._enableDeleteButton(true);
                this._enableOpenStudyButton(true);
                    
                this._enableResetStudyButton(true);
                    
				for(i=0; i<rows.length; i++)
				{
					this._enableResetStudyButton(this._canResetItem(rows[i]));
				}
            }
            else
            {
                this._enableDeleteButton(false);
                this._enableOpenStudyButton(false);
                this._enableResetStudyButton(false);
            }
        }
        else
        {
            this._enableDeleteButton(false);
            this._enableOpenStudyButton(false);
            this._enableResetStudyButton(false);
        }
    },     
        
    _canResetItem : function(row)
    {
        //"canreset" is a custom attribute injected by the list control
        return row.getAttribute('canreset')=='true';
    },
        
    // return the study instance uid of the row 
    _getInstanceUid : function(row)
    {
        //"instanceuid" is a custom attribute injected by the study list control
        return row.getAttribute('instanceuid');
    },
        
    _getServerPartitionAE : function(row)
    {
        //"serverae" is a custom attribute injected by the study list control
        return row.getAttribute('serverae');
    },
        
    _enableOpenStudyButton : function(en)
    {
        var openButton = $find(this._OpenButtonClientID);
        if(openButton != null) openButton.set_enable(en);
    },
        
    _enableResetStudyButton : function(en)
    {
        var resetButton = $find(this._ResetButtonClientID);
        if(resetButton != null) resetButton.set_enable(en);
    },
        
    _enableDeleteButton : function(en)
    {
        var deleteButton = $find(this._DeleteButtonClientID);
        if(deleteButton != null) deleteButton.set_enable(en);
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
                        
    get_ItemListClientID : function() {
        return this._ItemListClientID;
    },

    set_ItemListClientID : function(value) {
        this._ItemListClientID = value;
        this.raisePropertyChanged('ItemListClientID');
    },
        
    get_OpenButtonClientID : function() {
        return this._OpenButtonClientID;
    },

    set_OpenButtonClientID : function(value) {
        this._OpenButtonClientID = value;
        this.raisePropertyChanged('OpenButtonClientID');
    },
        
    get_ResetButtonClientID : function() {
        return this._ResetButtonClientID;
    },

    set_ResetButtonClientID : function(value) {
        this._ResetButtonClientID = value;
        this.raisePropertyChanged('ResetButtonClientID');
    },
        
    get_OpenStudyPageUrl : function() {
        return this._OpenStudyPageUrl;
    },
       
    set_OpenStudyPageUrl : function(value) {
        this._OpenStudyPageUrl = value;
        this.raisePropertyChanged('OpenStudyPageUrl');
    }
},
   
// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
