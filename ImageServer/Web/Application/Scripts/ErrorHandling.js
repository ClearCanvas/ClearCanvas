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


Sys.Application.add_load(AppLoad);
 
function AppLoad()
{
  Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
  Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);
}
 
function BeginRequest(sender, args) {
  // Clear the error if it's visible from a previous request.
  if ($get('ErrorPanel').style.visibility == "visible")
    CloseError();
}
 
function EndRequest(sender, args) {
  // Check to see if there's an error on this request.
  if (args.get_error() != undefined)
  {
    // If there is, show the custom error.
    $get('ErrorPanel').style.visibility = "visible";
    $get('ErrorPanel').innerHTML = args.get_error();
 
    // Let the framework know that the error is handled, 
    //  so it doesn't throw the JavaScript alert.
    args.set_errorHandled(true);
  }
}
 
function CloseError() {
  // Hide the error div.
  $get('ErrorPanel').style.visibility = "hidden";
}