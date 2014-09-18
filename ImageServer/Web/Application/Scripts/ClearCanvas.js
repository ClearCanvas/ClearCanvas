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

function InputHover() {
$(document).ready(function(){
    $(".SearchTextBox").mouseover(function() { $(this).addClass('TextInputHover'); }).mouseout(function() { $(this).removeClass('TextInputHover'); }).focus( function() {	$(this).addClass('TextInputFocus'); }).blur( function() { $(this).removeClass('TextInputFocus') });
    $(".SearchDateBox").mouseover(function() { $(this).addClass('DateInputHover'); }).mouseout(function() { $(this).removeClass('DateInputHover'); }).focus( function() {	$(this).addClass('DateInputFocus'); }).blur( function() { $(this).removeClass('DateInputFocus') });
    $(".GridViewTextBox").mouseover(function() { $(this).addClass('TextInputhover'); }).mouseout(function() { $(this).removeClass('TextInputHover'); }).focus( function() {	$(this).addClass('TextInputFocus'); }).blur( function() { $(this).removeClass('TextInputFocus') });
});
}

function UserInformationLink_Hover(objectID) {
    $(document).ready(function() {
        $("#" + objectID).hover(
            function() { $(this).css("text-decoration", "underline"); }, 
            function() { $(this).css("text-decoration", "none"); 
        });
    });
}

/*
 * Fits a string to a pixel width and adds "..." to the end of the string to indicate the string has been truncated if it's too long.
 *
 * str    A string where html-entities are allowed but no tags.
 * width  The maximum allowed width in pixels
 * className  A CSS class name with the desired font-name and font-size. (optional)  
 *
 * From: http://stackoverflow.com/questions/282758/truncate-a-string-nicely-to-fit-within-a-given-pixel-width
 */

function fitStringToWidth(str,width,className) {

  // _escTag is a helper to escape 'less than' and 'greater than'
  function _escTag(s){ return s.replace("<","&lt;").replace(">","&gt;");}

  //Create a span element that will be used to get the width
  var span = document.createElement("span");
  //Allow a classname to be set to get the right font-size.
  if (className) span.className=className;
  span.style.display='inline';
  span.style.visibility = 'hidden';
  span.style.padding = '0px';
  document.body.appendChild(span);

  var result = _escTag(str); // default to the whole string
  span.innerHTML = result;
  // Check if the string will fit in the allowed width. NOTE: if the width
  // can't be determinated (offsetWidth==0) the whole string will be returned.
  if (span.offsetWidth > width) {
    var posStart = 0, posMid, posEnd = str.length, posLength;
    // Calculate (posEnd - posStart) integer division by 2 and
    // assign it to posLength. Repeat until posLength is zero.
    while (posLength = (posEnd - posStart) >> 1) {
      posMid = posStart + posLength;
      //Get the string from the begining up to posMid;
      span.innerHTML = _escTag(str.substring(0,posMid)) + '&hellip;';

      // Check if the current width is too wide (set new end)
      // or too narrow (set new start)
      if ( span.offsetWidth > width ) posEnd = posMid; else posStart=posMid;
    }

    result = '<abbr title="' +
      str.replace("\"","&quot;") + '">' +
      _escTag(str.substring(0,posStart)) +
      '&hellip;<\/abbr>';
  }
  document.body.removeChild(span);
  return result;
}

/*
 * CheckDateRange checks two dates and ensures that the first is less than the second.
 *
 * fromDate the date that should be less than the second date
 * toDate  the date that should be greater than the second date
 * textBoxId  the id of the textbox where the date value being checked is from
 * calendarExtenderId the calendar extender id where the date was selected from
 * message an error message that is displayed if the range is invalid
 *
 */

function CheckDateRange(fromDate, toDate, message) {
    if(new Date(fromDate) > new Date(toDate)) {
        alert(message);
        return false;
    }
	return true;
}

/*
    C#-style string format 
*/
String.prototype.format = String.prototype.format = function() {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};
