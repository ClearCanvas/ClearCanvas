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
/// This script contains the javascript component class for the gridview
/// 
/// The component contains the following public methods:
///
///     getSelectedRowElements() : returns list of row elements (TR) which are being selected.
///     selectRow() : select a specific row
///     unselectRow(): unselected a specified row
///     clearSelections() : clear all current selection
///
/////////////////////////////////////////////////////////////////////////////////////////////////////////

// Define and Register the control type.
Type.registerNamespace('ClearCanvas.ImageServer.Web.Common.WebControls.UI');
        
/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Define the control constructor
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView = function(element) { 
    ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.initializeBase(this, [element]);        
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.prototype =
{
    initialize: function() {
        ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.callBaseMethod(this, 'initialize');
        this._onsubmit$delegate = Function.createDelegate(this, this._onsubmit);

        this._selectedDataKeys = null;

        this._loadClientState();

        if (this.get_element().rows != null) {
            // add click and double-click handlers on each row
            for (i = 0; i < this.get_element().rows.length; i++) {
                var row = this.get_element().rows[i];
                $addHandlers(row, {
                    'click': this._onCellClick,
                    'dblclick': this._onCellDblClick,
                    'contextmenu': this._onContextMenuButtonClick
                },
                    this);
            }
        }

        // attach an event to save the client state before a postback or updatepanel partial postback
        if (typeof (Sys.WebForms) !== "undefined" && typeof (Sys.WebForms.PageRequestManager) !== "undefined") {
            Array.add(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);
        } else {
            $addHandler(document.forms[0], "submit", this._onsubmit$delegate);
        }
    },

    dispose: function() {
        $clearHandlers(this.get_element());

        if (typeof (Sys.WebForms) !== "undefined" && typeof (Sys.WebForms.PageRequestManager) !== "undefined") {
            Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);
        } else {
            $removeHandler(document.forms[0], "submit", this._onsubmit$delegate);
        }
        ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.callBaseMethod(this, 'dispose');
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // public methods
    getSelectedRowElements: function() {

        var rows = this.get_element().rows;

        if (rows != undefined && rows != null) {
            var selectedRows = new Array();
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].getAttribute('isdatarow') == 'true' && rows[i].getAttribute('selected') == 'true') {
                    selectedRows[selectedRows.length] = rows[i];
                }
            }
            return selectedRows;
        }
        return null;
    },

    getNumberOfRows: function() {
        var rows = this.get_element().rows;

        if (rows != undefined && rows != null) {
            if (rows[0].getAttribute('isEmptyDataRow') == 'true') return 0;
            return rows.length;
        }
        else return 0;
    },

    selectRow: function(rowIndex) {
        debugger;
        var row = this.get_element().rows[rowIndex];
        if (row != null && row != undefined)
            this._selectRow(row);
    },

    unselectRow: function(rowIndex) {

        var row = this.get_element().rows[rowIndex];
        if (row != null && row != undefined)
            this._unselectRow(row);
    },

    clearSelections: function() {

        // unselect those currently selected
        var rows = this.getSelectedRowElements();
        if (rows == null || rows.length == 0)
            return;

        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            this._unselectRow(row);
        }
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    add_onClientRowClick: function(handler) {
        this.remove_onClientRowClick(handler); // make sure we don't attach the same handler twice
        this.get_events().addHandler('onClientRowClick', handler);
    },
    remove_onClientRowClick: function(handler) {
        this.get_events().removeHandler('onClientRowClick', handler);
    },
    raiseonClientRowClick: function(eventArgs) {
        var handler = this.get_events().getHandler('onClientRowClick');
        if (handler) {
            handler(this, eventArgs);
        }
    },

    add_onClientRowDblClick: function(handler) {
        this.remove_onClientRowDblClick(handler); //make sure we don't attach the same handler twice
        this.get_events().addHandler('onClientRowDblClick', handler);
    },
    remove_onClientRowDblClick: function(handler) {
        this.get_events().removeHandler('onClientRowDblClick', handler);
    },
    raiseonClientRowDblClick: function(eventArgs) {
        var handler = this.get_events().getHandler('onClientRowDblClick');
        if (handler) {
            handler(this, eventArgs);
        }
    },

    ////////////////////////////////////////////////////////////////////////////////////////////
    // Event delegates
    _onFocus: function(e) {
        if (this.get_element() && !this.get_element().disabled) {
        }
    },

    _onContextMenuButtonClick: function(e) {
        //alert('Context menu not supported');
        e.cancelBubble = true;
        e.returnValue = false;
        e.preventDefault();
        return false;
    },

    _onCellDblClick: function(e) {
        var el = e.target;
        // The event can be triggered by a descendant in the cell
        // We have to traverse the tree to find the row element
        while (el != null && el.tagName != 'TR')
            el = el.parentNode;

        var row = el;

        if (row.getAttribute('isEmptyDataRow') == "true" || row.getAttribute('isHeaderRow') == "true") return;

        if (this.get_element() && !this.get_element().disabled) {
            this._selectRow(row);

            var ev = new Sys.EventArgs();
            ev.row = row;
            this.raiseonClientRowDblClick(ev);
        }
    },


    _onCellClick: function(e) {

        var el = e.target;
        // The event can be triggered by a descendant in the cell
        // We have to traverse the tree to find the row element
        while (el != null && el.tagName != 'TR')
            el = el.parentNode;

        var row = el;

        if (row.getAttribute('isEmptyDataRow') == "true" || row.getAttribute('isHeaderRow') == "true") return;

        if (this.get_element() && !this.get_element().disabled) {
            var button = e.which || e.button;

            switch (button) {
                case 0: // left button clicked
                    {
                        if (row.getAttribute('isdatarow') == 'true') {
                            var multipleSelectionMode = e.ctrlKey;
                            if (multipleSelectionMode) {
                                var alreadyselected = row.getAttribute('selected') == 'true';
                                if (alreadyselected) {
                                    // unselect it
                                    this._unselectRow(row);
                                }
                                else {
                                    this._selectRow(row);
                                }

                            }
                            else {
                                this.clearSelections();
                                this._selectRow(row);
                            }
                        }

                        var ev = new Sys.EventArgs();
                        ev.row = row;
                        this.raiseonClientRowClick(ev);

                        break;
                    }

                default:
                    {
                        // alert('Not support');
                    }
            }
        }
    },

    _onBlur: function(e) {
        if (this.get_element() && !this.get_element().disabled) {
        }
    },

    _onsubmit: function() {
        this._saveClientState();
        return true;
    },

    ////////////////////////////////////////////////////////////////////////////////////////////
    // Private methods
    _selectRow: function(row) {
        if (this.get_element() && !this.get_element().disabled) {
            if (this._SelectionMode == 'Multiple') {
                row.style.cssText = this._SelectedRowStyle;
                row.className = this._SelectedRowCSS;
                row.setAttribute('selected', 'true');
            }
            else if (this._SelectionMode == 'Single') {
                this.clearSelections();
                row.style.cssText = this._SelectedRowStyle;
                row.className = this._SelectedRowCSS;
                row.setAttribute('selected', 'true');
            }
        }
    },

    _unselectRow: function(row) {
        if (this.get_element() && !this.get_element().disabled) {
            var rowIndex = parseInt(row.getAttribute('rowIndex'));
            if (rowIndex % 2 == 0) {
                row.style.cssText = this._UnSelectedRowStyle;
                row.className = this._UnSelectedRowCSS;
            }
            else {
                row.style.cssText = (this._AlternatingRowStyle != null && this._AlternatingRowStyle != undefined) ? this._AlternatingRowStyle : this._UnSelectedRowStyle;
                row.className = (this._AlternatingRowCSS != null && this._AlternatingRowCSS != undefined) ? this._AlternatingRowCSS : this._UnSelectedRowCSS;
            }
            row.setAttribute('selected', 'false');
        }
    },

    _loadClientState: function(value) {

        var selectedKeysField = this.get_selectedKeysStateField();

        if (selectedKeysField != null && selectedKeysField.value != "" && selectedKeysField.value != 'undefined') {
            this._selectedDataKeys = Sys.Serialization.JavaScriptSerializer.deserialize(selectedKeysField.value);

            var rows = this.get_element().rows;

            if (rows != undefined && rows != null) {
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].getAttribute('isdatarow') == 'true') {
                        if (this._containsDataKey(rows[i].getAttribute('dataKey'))) this._selectRow(rows[i]);
                        else this._unselectRow(rows[i]);
                    }
                }
            }
        }
    },

    _containsDataKey: function(value) {

        if (this._selectedDataKeys == null || this._selectedDataKeys == "") return false;

        for (var i = 0; i < this._selectedDataKeys.length; i++) {
            if (this._selectedDataKeys[i] == value) return true;
        }

        return false;
    },

    ///
    /// store the selection in a hidden client state field
    ///
    _saveClientState: function() {
        var stateField = this.get_clientStateField();
        var selectedKeysField = this.get_selectedKeysStateField();

        if (stateField != null && selectedKeysField != null) {
            var rows = this.getSelectedRowElements();
            if (rows == null || rows.length == 0) {
                stateField.value = '';
                selectedKeysField.value = '';
            }
            else {
                var selectedRowIndices = new Array();
                if (this._SelectUsingDataKeys) var selectedRowDataKeys = new Array();

                for (i = 0; i < rows.length; i++) {
                    selectedRowIndices[selectedRowIndices.length] = rows[i].getAttribute('rowIndex');
                    if (this._SelectUsingDataKeys) {
                        selectedRowDataKeys[selectedRowDataKeys.length] = rows[i].getAttribute('dataKey');
                    }
                }
                stateField.value = Sys.Serialization.JavaScriptSerializer.serialize(selectedRowIndices);

                if (this._SelectUsingDataKeys)
                    selectedKeysField.value = Sys.Serialization.JavaScriptSerializer.serialize(selectedRowDataKeys);
            }
        }
    },

    ////////////////////////////////////////////////////////////////////////////////////////////
    // properties
    get_clientStateFieldID: function() {
        return this._clientStateFieldID;
    },
    set_clientStateFieldID: function(value) {
        this._clientStateFieldID = value;
    },

    get_clientStateField: function() {

        if (this._clientStateFieldID != undefined && this._clientStateFieldID != null)
            return $get(this._clientStateFieldID);
        else
            return null;
    },

    get_selectedKeysStateFieldID: function() {
        return this._selectedsKeyStateFieldID;
    },
    set_selectedKeysStateFieldID: function(value) {
        this._selectedKeysStateFieldID = value;
    },

    get_selectedKeysStateField: function() {
        if (this._selectedKeysStateFieldID != undefined && this._selectedKeysStateFieldID != null)
            return $get(this._selectedKeysStateFieldID);
        else
            return null;
    },

    get_SelectedRowStyle: function() {
        return this._SelectedRowStyle;
    },

    set_SelectedRowStyle: function(value) {
        this._SelectedRowStyle = value;
        this.raisePropertyChanged('SelectedRowStyle');
    },

    get_SelectedRowCSS: function() {
        return this._SelectedRowCSS;
    },

    set_SelectedRowCSS: function(value) {
        this._SelectedRowCSS = value;
        this.raisePropertyChanged('SelectedRowCSS');
    },

    get_AlternatingRowStyle: function() {
        return this._AlternatingRowStyle;
    },

    set_AlternatingRowStyle: function(value) {
        this._AlternatingRowStyle = value;
        this.raisePropertyChanged('AlternatingRowStyle');
    },

    get_AlternatingRowCSS: function() {
        return this._AlternatingRowCSS;
    },

    set_AlternatingRowCSS: function(value) {
        this._AlternatingRowCSS = value;
        this.raisePropertyChanged('AlternatingRowCSS');
    },

    get_UnSelectedRowStyle: function() {
        return this._UnSelectedRowStyle;
    },

    set_UnSelectedRowStyle: function(value) {
        this._UnSelectedRowStyle = value;
        this.raisePropertyChanged('UnSelectedRowStyle');
    },

    get_UnSelectedRowCSS: function() {
        return this._UnSelectedRowCSS;
    },

    set_UnSelectedRowCSS: function(value) {
        this._UnSelectedRowCSS = value;
        this.raisePropertyChanged('UnSelectedRowCSS');
    },

    get_SelectionMode: function() {
        return this._SelectionMode;
    },

    set_SelectionMode: function(value) {
        this._SelectionMode = value;
        this.raisePropertyChanged('SelectionMode');
    },

    get_SelectUsingDataKeys: function() {
        return this._SelectUsingDataKeys;
    },

    set_SelectUsingDataKeys: function(value) {
        this._SelectUsingDataKeys = value;
        this.raisePropertyChanged('SelectUsingDataKeys');
    }
},

// Optional descriptor for JSON serialization.
//ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.descriptor = {
//    properties: [   {name: 'onClientRowClick', type: events}                    ]
//}

// Register the class as a type that inherits from Sys.UI.Control.
ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.registerClass('ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
