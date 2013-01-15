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
/// This script contains the javascript component class for the move panel
Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel = function(element) { 
    ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.prototype =
{
    initialize: function() {

        ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.callBaseMethod(this, 'initialize');

        this._OnDeviceListRowClickedHandler = Function.createDelegate(this, this._OnDeviceListRowClicked);
        this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
            
        Sys.Application.add_load(this._OnLoadHandler);

    },

    dispose: function() {
        $clearHandlers(this.get_element());

        ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.callBaseMethod(this, 'dispose');

        Sys.Application.remove_load(this._OnLoadHandler);
    },


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
   
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad: function() {
        var devicelist = $find(this._DeviceListClientID);
        devicelist.add_onClientRowClick(this._OnDeviceListRowClickedHandler);

        this._updateToolbarButtonStates();
    },

    // called when user clicked on a row in the study list
    _OnDeviceListRowClicked: function(sender, event) {
        this._updateToolbarButtonStates();
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates: function() {
        var devicelist = $find(this._DeviceListClientID);

        this._enableMoveButton(false);

        if (devicelist != null) {
            var rows = devicelist.getSelectedRowElements();

            if (rows != null && rows.length > 0) {
                this._enableMoveButton(true);
            } else {
                this._enableMoveButton(false);
            }
        }
    },

    _enableMoveButton: function(en) {
        var moveButton = $find(this._MoveButtonClientID);
        moveButton.set_enable(en);
    },

    get_MoveButtonClientID: function() {
        return this._MoveButtonClientID;
    },

    set_MoveButtonClientID: function(value) {
        this._MoveButtonClientID = value;
        this.raisePropertyChanged('MoveButtonClientID');
    },

    set_DeviceListClientID: function(value) {
        this._DeviceListClientID = value;
        this.raisePropertyChanged('DeviceListClientID');
    },

    get_DeviceListClientID: function() {
        return this._DeviceListClientID;
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel', Sys.UI.Control);     

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
