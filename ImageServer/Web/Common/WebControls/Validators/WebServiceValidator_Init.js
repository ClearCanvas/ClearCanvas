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

function addLoadEvent(func) {
    if (window.attachEvent) { window.attachEvent('onload', func); }
    else if (window.addEventListener) { window.addEventListener('load', func, false); }
    else { document.addEventListener('load', func, false); }
}

function initwebservice() {
    //service is defined in the Master Page. This method only works in IE, other browsers will be server-side validated.
    if (navigator.appName == "Microsoft Internet Explorer") {
        service.useService('@@WEBSERVICE_URL@@?WSDL', 'ValidationServices');
        service.onserviceavailable = onserviceavailable();
    }
}
   
function onserviceavailable(){
    //alert('web service ready');
}
   
addLoadEvent(function() {
    //alert('Adding load event');
    initwebservice();
});     
