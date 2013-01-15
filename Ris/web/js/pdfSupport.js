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

/* PDF ActiveX object have these supported functions.  Not all are exposed by PdfObject
	GoBackwardStack()
	GoForwardStack()
	GotoFirstPage()
	GotoLastPage()
	GotoNextPage()
	GotoPreviousPage()
	LoadFile(fileName)
	Print()
	PrintAll()
	PrintAllFit()
	PrintPages(from, to)
	PrintPagesFit()
	PrintWithDialog()
	SetCurrentHighlight(left, top, width, height)
	SetCurrentPage(pageNumber)
	SetLayoutMode(mode)
		'DontCare'      - use the current user preference
		'SinglePage'    - use single page mode (pre Acrobat 3.0 style)
		'OneColumn'     - use one-column continuous mode
		'TwoColumnLeft' - use two-column continuous mode with the first page on the left
		'TwoColumnRight'- use two-column continuous mode with the first page on the right  
	SetNamedDest(destination)
	SetPageMode(mode)
		'none'      - displays the document, but does not display bookmarks or thumbnails (default)
		'bookmarks' - displays the document and bookmarks
		'thumbs'    - displays the document and thumbnails
	SetShowScrollbars(bool)
	SetShowToolbar(bool)
	SetView(view)
		'Fit'   - Fits the entire page within the window both vertically and horizontally.
		'FitH'  - Fits the entire width of the page within the window.
		'FitV'  - Fits the entire height of the page within the window.
		'FitB'  - Fits the bounding box within the window both vertically and horizontally.
		'FitBH' - Fits the width of the bounding box within the window.
		'FitBV' - Fits the height of the bounding box within the window
	SetViewRect(left, top, width, height)
	SetViewScroll(view, offset)
		view - see SetView
		offset - The horizontal or vertical coordinate positioned either at the left or top edge.
	SetZoom(percent)
		percent -   The desired zoom factor, expressed as a percentage.
	SetZoomScroll(percent, left, top)
		percent - see SetZoom
		left - The horizontal coordinate positioned at the left edge.
		top - The vertical coordinate positioned at the top edge.
 */
function PdfObject(id, fileUrl) 
{
	var thisObject = this;
	var _activeXObject = document.getElementById(id);
	var _preDefinedZoom = [ 12.5, 25, 33.3, 50, 75, 100, 125, 150, 200, 300, 400, 600, 800];
	var _defaultZoom = 100;
	var _defaultView = "FitH";
	var _currentZoom;

	// Set the width of the ActiveX control
	this.setWidth = function(value)
		{ _activeXObject.width = value; }

	// Set the height of the ActiveX control
	this.setHeight = function(value)
		{ _activeXObject.height = value; }

	// Opens and displays the specified document within the ActiveX control.
	this.loadFile = function(fileName)
		{ _activeXObject.LoadFile(fileName); }

	// Prints the document according to the options selected in a user dialog box.
	this.printWithDialog = function()
		{ _activeXObject.PrintWithDialog(); }

	// Determines whether a toolbar will appear in the viewer.
	this.setShowToolbar = function(show)
		{ _activeXObject.SetShowToolbar(show); }

	// Sets the view of a page according to the specified string.
	this.setView = function(view)
		{ 
			_activeXObject.SetView(view); 
			if (view == _defaultView)
				_currentZoom = _defaultZoom;
		}

	// Sets the magnification according to the specified value.
	this.setZoom = function(percentage)
		{ 
			_currentZoom = percentage;
			_activeXObject.SetZoom(_currentZoom); 
		}

	// Sets the magnification to the next smaller zoom.
	this.zoomOut = function()
		{
			var currentZoomIndex = _preDefinedZoom.indexOf(_currentZoom);
			if (currentZoomIndex - 1 > 0)
				this.setZoom(_preDefinedZoom[currentZoomIndex - 1]);
		}

	// Sets the magnification to the next largest zoom.
	this.zoomIn = function()
		{
			var currentZoomIndex = _preDefinedZoom.indexOf(_currentZoom);
			if (currentZoomIndex + 1 < _preDefinedZoom.length)
				this.setZoom(_preDefinedZoom[currentZoomIndex + 1]);
		}

	this.createToolbar = function(id)
		{
			function createButton(icon, tooltip, onClickCallBack)
			{
				var button = document.createElement("<image src='" + imagePath + "/" + icon + "' alt='" + tooltip + "'>");
				button.onclick = onClickCallBack;
				return button;
			}
			
			var parentElement = document.getElementById(id);
			var printButton = createButton("Print.png", "Print", function() { thisObject.printWithDialog(); });
			var zoomInButton = createButton("ZoomInToolSmall.png", "Zoom In", function() { thisObject.zoomIn(); });
			var zoomOutButton = createButton("ZoomOutToolSmall.png", "Zoom Out", function() { thisObject.zoomOut(); });
			var restoreButton = createButton("FitWidthToolSmall.png", "Restore Zoom", function() { thisObject.setView(_defaultView); });
			parentElement.appendChild(printButton);
			parentElement.appendChild(zoomInButton);
			parentElement.appendChild(zoomOutButton);
			parentElement.appendChild(restoreButton);
		}

	if (fileUrl)
		this.loadFile(fileUrl);

	this.setWidth("100%");
	this.setHeight("100%");
	this.setView(_defaultView); // ticket #5095: Change the default page layout view of the fax so that it's "fit width"
	this.setShowToolbar(false);

	return this;
}