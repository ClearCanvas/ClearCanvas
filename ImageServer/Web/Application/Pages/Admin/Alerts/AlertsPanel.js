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

Type.registerNamespace('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel = function(element) {
    ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.prototype =
    {
        initialize: function() {
            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.callBaseMethod(this, 'initialize');

            this._OnAlertListRowClickedHandler = Function.createDelegate(this, this._OnAlertListRowClicked);

            this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
        },

        dispose: function() {
            $clearHandlers(this.get_element());

            ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.callBaseMethod(this, 'dispose');

            Sys.Application.remove_load(this._OnLoadHandler);
        },

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Events
        _OnLoad: function() {
            var userlist = $find(this._AlertListClientID);
            userlist.add_onClientRowClick(this._OnAlertListRowClickedHandler);

            this._updateToolbarButtonStates();
        },

        // called when user clicked on a row in the study list
        _OnAlertListRowClicked: function(sender, event) {
            this._updateToolbarButtonStates();
        },

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Private Methods
        _updateToolbarButtonStates: function() {
            var alertlist = $find(this._AlertListClientID);

            this._enableDeleteButton(false);
            this._enableDeleteAllButton(false);

            if (alertlist != null) {
                var rows = alertlist.getSelectedRowElements();

                if (rows != null && rows.length > 0) {
                    this._enableDeleteButton(true);
                }

                if (alertlist.getNumberOfRows() > 0) {
                    this._enableDeleteAllButton(true);
                }
            }
        },

        _enableDeleteButton: function(en) {
            var deleteButton = $find(this._DeleteButtonClientID);
            if (deleteButton != null) deleteButton.set_enable(en);
        },

        _enableDeleteAllButton: function(en) {
            var deleteAllButton = $find(this._DeleteAllButtonClientID);
            if (deleteAllButton != null) deleteAllButton.set_enable(en);
        },

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Properties
        get_DeleteButtonClientID: function() {
            return this._DeleteButtonClientID;
        },

        set_DeleteButtonClientID: function(value) {
            this._DeleteButtonClientID = value;
            this.raisePropertyChanged('DeleteButtonClientID');
        },

        get_DeleteAllButtonClientID: function() {
            return this._DeleteAllButtonClientID;
        },

        set_DeleteAllButtonClientID: function(value) {
            this._DeleteAllButtonClientID = value;
            this.raisePropertyChanged('DeleteAllButtonClientID');
        },

        get_AlertListClientID: function() {
            return this._AlertListClientID;
        },

        set_AlertListClientID: function(value) {
            this._AlertListClientID = value;
            this.raisePropertyChanged('AlertListClientID');
        }
    },

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.registerClass('ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

