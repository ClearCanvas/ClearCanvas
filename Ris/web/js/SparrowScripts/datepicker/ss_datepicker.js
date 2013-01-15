<!--
	// SparrowScripts Datepicker v2
	// www.sparrowscripts.com

	var ssdp_bShow=false;
	
	if (ssdp_type==0)
	{
		// IFRAME mode : creating layer

		document.write("<div style='position:absolute;visibility:hidden' id='ssdp_iframe'></div>");
	}

	function displayDatePicker(contentCtl){

		if (ssdp_type==2)
		{
			// STATIC mode

			document.write("<div id='ssdp_iframe'></div>");			
			document.getElementById("ssdp_iframe").innerHTML="<iframe id='ssdp_iframeContent' src='"+ssdp_scriptdir+"ss_datepicker.htm#"+contentCtl+"' width="+ssdp_popupWidth+" height="+ssdp_popupHeight+" frameborder='0' border='0'></iframe>";
		}
	}

	function showDatePicker(activatorCtl, contentCtl, callback) {

		if(ssdp_bShow)
		{
			ssdp_hideDatePicker();
			return;
		}
		if(sstp_bShow)
		{
			sstp_hideTimePicker();
		}
	

		ssdp_onDatePickerClosedCallback = callback;
	
		// render date picker

		if (ssdp_type==0) {

			// IFRAME mode 

			document.getElementById("ssdp_iframe").innerHTML="<iframe id='ssdp_iframeContent' src='"+ssdp_scriptdir+"ss_datepicker.htm#"+contentCtl.id+"' width="+ssdp_popupWidth+" height="+ssdp_popupHeight+" frameborder='0' border='0'></iframe>";

			// determine position of activator and where to show the popup

			var leftpos = 0;
			var toppos = 0;
			var aTag = activatorCtl;
			do {
				aTag     = aTag.offsetParent;
				leftpos += aTag.offsetLeft;
				toppos  += aTag.offsetTop;
			} while (aTag.tagName != 'BODY');

			leftpos = (ssdp_fixedX==-1) ? activatorCtl.offsetLeft + leftpos : ssdp_fixedX;
			toppos = (ssdp_fixedY==-1) ? activatorCtl.offsetTop + toppos + activatorCtl.offsetHeight + 2 : ssdp_fixedY;

			// check if datepicker is outside browser
			if (leftpos+ssdp_popupWidth>document.body.clientWidth)
			{
				// if datepicker appear too right, display on left side of control, or at 0 if not enough space
				leftpos=(leftpos-ssdp_popupWidth+activatorCtl.clientWidth>=0)?leftpos-ssdp_popupWidth+activatorCtl.clientWidth+4:0;
			}

			if (toppos+ssdp_popupHeight>document.body.clientHeight
				|| ((sstp_popupHeight) && (toppos+sstp_popupHeight>document.body.clientHeight)))
			{
				// if datepicker appear too low, display on top of control, or remain if not enough space
				toppos=(toppos>=ssdp_popupHeight)?(toppos-activatorCtl.offsetHeight)-ssdp_popupHeight-4:toppos;
			}

			document.getElementById("ssdp_iframe").style.left = leftpos+"px";
			document.getElementById("ssdp_iframe").style.top = toppos+"px";

			// show the visibility using CSS

			document.getElementById("ssdp_iframe").style.visibility="visible";
		}
		else if (ssdp_type==1) {

			// POPUP mode : open new window

			var xPos=(ssdp_fixedX==-1)?((screen.width-ssdp_popupWidth)/2):ssdp_fixedX;
			var yPos=(ssdp_fixedY==-1)?((screen.height-ssdp_popupHeight)/2):ssdp_fixedY;

			window.open(ssdp_scriptdir+'ss_datepicker.htm#'+contentCtl.id, 'ssdp_html', 'width='+ssdp_popupWidth+',height='+ssdp_popupHeight+',toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,directories=no,status=no,left='+xPos+',top='+yPos);
		}
		else if (ssdp_type==2) {
			if (document.getElementById("ssdp_iframe")!=null)
			{
				document.getElementById("ssdp_iframe").innerHTML="<iframe id='ssdp_iframeContent' src='"+ssdp_scriptdir+"ss_datepicker.htm#"+contentCtl+"' width="+ssdp_popupWidth+" height="+ssdp_popupHeight+" frameborder='0' border='0'></iframe>";
			}
			else {
				alert('Calendar not displayed for static mode!');
			}
		}

		ssdp_bShow=true;
	}

	function ssdp_hideDatePicker(){

		document.getElementById("ssdp_iframe").style.visibility="hidden";		
		ssdp_bShow=false;
	}

	function ssdp_onDatePickerClosed(date)
	{
		ssdp_bShow = false;
		if(ssdp_onDatePickerClosedCallback)
		{
			ssdp_onDatePickerClosedCallback(date);
		}
	}
	
	
	//document.onkeypress = 
	function ssdp_escDatepicker (event) {

		  var keyPressed=0;
		  var browserName=navigator.appName;

		  if (browserName=='Microsoft Internet Explorer') {
			  keyPressed=window.event.keyCode;
		  }
		  else if (browserName == 'Netscape') {
			  keyPressed = event.keyCode;
		  }

		  if (keyPressed==27){
			  if (ssdp_bShow) {
				  ssdp_hideDatePicker();
			  }
		  }

	}
	
	if ( typeof document.addEventListener != "undefined" )
	{
	  document.addEventListener( "onkeypress", ssdp_escDatepicker, false );
	}
	// IE 
	else if ( typeof document.attachEvent != "undefined" ) 
	{
	  document.attachEvent( "onkeypress", ssdp_escDatepicker );
	}

//-->