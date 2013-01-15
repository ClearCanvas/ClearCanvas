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
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs.initializeBase(this, [element]);
       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs.prototype = 
{
    initialize : function() {
        ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs.callBaseMethod(this, 'initialize');        
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        this._OnSeriesListClickedHandler = Function.createDelegate(this,this._OnSeriesListClicked);
        this._OnViewSeriesButtonClickedHandler = Function.createDelegate(this,this._OnViewSeriesButtonClicked);
        this._OnMoveSeriesButtonClickedHandler = Function.createDelegate(this,this._OnMoveSeriesButtonClicked);
        this._updateToolbarButtonStates();
            
        Sys.Application.add_load(this._OnLoadHandler);      
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs.callBaseMethod(this, 'dispose');
            
        var serieslist = $find(this._SeriesListClientID);
        if (serieslist!=null)
        {
            serieslist.remove_onClientRowClick(this._OnSeriesListClickedHandler);
        }
            
        var viewSeriesBtn = $find(this._ViewSeriesButtonClientID);
        if (viewSeriesBtn!=null)
        {
            viewSeriesBtn.remove_onClientClick(this._OnViewSeriesButtonClickedHandler);
        }
                        
        var moveSeriesBtn = $find(this._MoveSeriesButtonClientID); 
        if (moveSeriesBtn!=null)
        {
            moveSeriesBtn.remove_onClientClick(this._OnMoveSeriesButtonClickedHandler);
        }
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad : function()
    {           
        var serieslist = $find(this._SeriesListClientID);
        if (serieslist!=null)
        {
            serieslist.add_onClientRowClick(this._OnSeriesListClickedHandler);
            // serieslist.add_onClientRowDblClick(this._OnSeriesListDoubleClickedHandler);
        }
            
        var viewSeriesBtn = $find(this._ViewSeriesButtonClientID);
        var moveSeriesBtn = $find(this._MoveSeriesButtonClientID);            
        if (viewSeriesBtn!=null)
        {
            viewSeriesBtn.add_onClientClick(this._OnViewSeriesButtonClickedHandler);
        }
        if (moveSeriesBtn!=null)
        {
            moveSeriesBtn.add_onClientClick(this._OnMoveSeriesButtonClickedHandler);
        }
    },
        
    // called when the user clicks on the series list
    _OnSeriesListClicked : function(src, event)
    {
        this._updateToolbarButtonStates();
    },
        
    _OnViewSeriesButtonClicked : function()
    {
        var serieslist = $find(this._SeriesListClientID);
        if (serieslist!=null)
        {
            var rows = serieslist.getSelectedRowElements();
            for(i=0;i<rows.length;i++)
            {
                var url = String.format("{0}?serverae={1}&studyuid={2}&seriesuid={3}", 
                        this._OpenSeriesPageUrl,
                        this._getServerAE(rows[i]),
                        this._getStudyUid(rows[i]),
                        this._getSeriesUid(rows[i]));
                window.open(url);
            }
        }
    },
        
    _updateToolbarButtonStates : function()
    {
        var serieslist = $find(this._SeriesListClientID);
                      
        this._enableDeleteButton(false);
        this._enableMoveButton(false);
        this._enableViewDetailsButton(false);     
            
        if (serieslist!=null )
        {
            var rows = serieslist.getSelectedRowElements();
                
            if (rows!=null && rows.length>0)
            {
                var selectedSeriesCount = rows.length; 
                var canMoveCount=0;   
		        var canDeleteCount=0;  
		            	            
		        for(i=0; i<rows.length; i++)
                {
                    if (this._canMoveSeries(rows[i]))
                    {
                        canMoveCount++;
                    }
                    if (this._canDeleteSeries(rows[i]))
                    {
                        canDeleteCount++;
                    }
                }
                
                this._enableDeleteButton(canDeleteCount==selectedSeriesCount);
                this._enableMoveButton(canMoveCount==selectedSeriesCount);
                this._enableViewDetailsButton(true);                  
            }
        }
    },
        
    _OnDeleteSeriesButtonClicked : function()
    {
        var serieslist = $find(this._SeriesListClientID);
        if (serieslist!=null)
        {
            var rows = serieslist.getSelectedRowElements();
            for(i=0;i<rows.length;i++)
            {
                var url = String.format("{0}?serverae={1}&studyuid={2}&seriesuid={3}", 
                        this._OpenSeriesPageUrl,
                        this._getServerAE(rows[i]),
                        this._getStudyUid(rows[i]),
                        this._getSeriesUid(rows[i]));
                window.open(url);
            }
        }
    },
        
    _OnMoveSeriesButtonClicked : function()
    {
        var serieslist = $find(this._SeriesListClientID);
        if (serieslist!=null)
        {
            var rows = serieslist.getSelectedRowElements();
            var urlCount = 1;
            var url = "";
                
            for(i=0;i<rows.length;i++)
            {                    
                var studyuid = this._getStudyUid(rows[i]);
                var serverae = this._getServerAE(rows[i]);
                var seriesuid = this._getSeriesUid(rows[i]);
                if (studyuid!=undefined && serverae!=undefined && seriesuid!=undefined)
                {
                    if (urlCount == 1)
                        url = String.format('{0}?serverae={1}&studyuid={2}&seriesuid{4}={3}', this._SendSeriesPageUrl, serverae, studyuid, seriesuid, urlCount);
                    else
                        url += String.format('&seriesuid{1}={0}', seriesuid, urlCount);
                            
                    urlCount++;
                }
            }          
            window.open(url);
        }
    },
        
        _enableViewDetailsButton : function(en)
    {
        var viewSeriesBtn = $find(this._ViewSeriesButtonClientID);
        if(viewSeriesBtn != null) viewSeriesBtn.set_enable(en);
    },
        
        _enableMoveButton : function(en)
    {
        var moveSeriesBtn = $find(this._MoveSeriesButtonClientID);
        if(moveSeriesBtn != null) moveSeriesBtn.set_enable(en);
    },
        
        _enableDeleteButton : function(en)
    {
        var deleteSeriesBtn = $find(this._DeleteSeriesButtonClientID);
        if(deleteSeriesBtn != null) deleteSeriesBtn.set_enable(en);
    },
                
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _getServerAE:function(row)
    {
        return row.getAttribute("serverae");
    },
        
    _getStudyUid:function(row)
    {
        return row.getAttribute("studyuid");
    },
        
    _getSeriesUid:function(row)
    {
        return row.getAttribute("seriesuid");
    },
        
        _canDeleteSeries:function(row)
    {
        //"candelete" is a custom attribute injected by the study list control
        return row.getAttribute('candelete')=='true';
    },
        
    _canMoveSeries:function(row)
    {
        //"canmove" is a custom attribute injected by the study list control
        return row.getAttribute('canmove')=='true';
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
        
    get_SeriesListClientID : function() {
        return this._SeriesListClientID;
    },

    set_SeriesListClientID: function(value) {
        this._SeriesListClientID = value;
        this.raisePropertyChanged('SeriesListClientID');
    },
        
    get_OpenSeriesPageUrl : function() {
        return this._OpenSeriesPageUrl;
    },

    set_OpenSeriesPageUrl: function(value) {
        this._OpenSeriesPageUrl = value;
        this.raisePropertyChanged('OpenSeriesPageUrl');
    },
        
    get_SendSeriesPageUrl : function() {
        return this._SendSeriesPageUrl;
    },

    set_SendSeriesPageUrl: function(value) {
        this._SendSeriesPageUrl = value;
        this.raisePropertyChanged('SendSeriesPageUrl');
    },
        
    get_ViewSeriesButtonClientID : function() {
        return this._ViewSeriesButtonClientID;
    },
       
    set_ViewSeriesButtonClientID : function(value) {
        this._ViewSeriesButtonClientID = value;
        this.raisePropertyChanged('ViewSeriesButtonClientID');
    },
        
    get_MoveSeriesButtonClientID : function() {
        return this._MoveSeriesButtonClientID;
    },
       
    set_MoveSeriesButtonClientID : function(value) {
        this._MoveSeriesButtonClientID = value;
        this.raisePropertyChanged('MoveSeriesButtonClientID');
    },
        
    get_DeleteSeriesButtonClientID : function() {
        return this._DeleteSeriesButtonClientID;
    },    
    set_DeleteSeriesButtonClientID : function(value) {
        this._DeleteSeriesButtonClientID = value;
        this.raisePropertyChanged('DeleteSeriesButtonClientID');
    },
        
    get_PatientBirthDateClientID : function() {
        return this._PatientBirthDateClientID;
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsTabs', Sys.UI.Control);
     
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();