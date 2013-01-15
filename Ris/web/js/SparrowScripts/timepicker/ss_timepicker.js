<!--
	// SparrowScripts Timepicker v2
	// www.sparrowscripts.com

	var sstp_bShow=false;
	
	if (sstp_type==0)
	{
		// IFRAME mode : creating layer

		document.write("<div style='position:absolute;visibility:hidden' id='sstp_iframe'></div>");
	}

	function showTimePicker(activatorCtl, contentCtl, callback) {

		if(sstp_bShow)
		{
			sstp_hideTimePicker();
			return;
		}
		if(ssdp_bShow)
		{
			ssdp_hideDatePicker();
		}
	
		sstp_onTimePickerClosedCallback = callback;
	
		// render time picker

		if (sstp_type==0) {

			// IFRAME mode 

			document.getElementById("sstp_iframe").innerHTML="<iframe id='sstp_iframeContent' src='"+sstp_scriptdir+"ss_timepicker.htm#"+contentCtl.id+"' width="+sstp_popupWidth+" height="+sstp_popupHeight+" frameborder='0' border='0'></iframe>";

			// determine position of activator and where to show the popup

			var leftpos = 0;
			var toppos = 0;
			var aTag = activatorCtl;
			do {
				aTag     = aTag.offsetParent;
				leftpos += aTag.offsetLeft;
				toppos  += aTag.offsetTop;
			} while (aTag.tagName != 'BODY');

			leftpos = (sstp_fixedX==-1) ? activatorCtl.offsetLeft + leftpos : sstp_fixedX;
			toppos = (sstp_fixedY==-1) ? activatorCtl.offsetTop + toppos + activatorCtl.offsetHeight + 2 : sstp_fixedY;

			// check if datepicker is outside browser

			if (leftpos+sstp_popupWidth>document.body.clientWidth)
			{
				// if datepicker appear too right, display on left side of control, or at 0 if not enough space
				leftpos=(leftpos-sstp_popupWidth+activatorCtl.clientWidth>=0)?leftpos-sstp_popupWidth+activatorCtl.clientWidth+4:0;
			}

			if (toppos+sstp_popupHeight>document.body.clientHeight)
			{
				// if datepicker appear too low, display on top of control, or remain if not enough space
				toppos=(toppos>=sstp_popupHeight)?(toppos-activatorCtl.offsetHeight)-sstp_popupHeight-4:toppos;
			}

			document.getElementById("sstp_iframe").style.left = leftpos + "px";
			document.getElementById("sstp_iframe").style.top = toppos + "px";

			// show the visibility using CSS

			document.getElementById("sstp_iframe").style.visibility="visible";
		}
		else {
			// POPUP mode : open new window

			var xPos=(sstp_fixedX==-1)?((screen.width-sstp_popupWidth)/2):sstp_fixedX;
			var yPos=(sstp_fixedY==-1)?((screen.height-sstp_popupHeight)/2):sstp_fixedY;

			window.open(sstp_scriptdir+'ss_timepicker.htm#'+contentCtl.id, 'sstp_html', 'width='+sstp_popupWidth+',height='+sstp_popupHeight+',toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,directories=no,status=no,left='+xPos+',top='+yPos);
		}

		sstp_bShow=true;
	}

	function sstp_hideTimePicker(){

		document.getElementById("sstp_iframe").style.visibility="hidden";		
		sstp_bShow=false;
	}

	function sstp_onTimePickerClosed(date)
	{
		sstp_bShow=false;
		if(sstp_onTimePickerClosedCallback)
		{
			sstp_onTimePickerClosedCallback(date);
		}
	}
	
	//document.onkeypress = 
	function sstp_escTimepicker (event) {

		  var keyPressed=0;
		  var browserName=navigator.appName;

		  if (browserName=='Microsoft Internet Explorer') {
			  keyPressed=window.event.keyCode;
		  }
		  else if (browserName == 'Netscape') {
			  keyPressed = event.keyCode;
		  }

		  if (keyPressed==27){
			  if (sstp_bShow) {
				  sstp_hideTimePicker();
			  }
		  }

	}

	if ( typeof document.addEventListener != "undefined" )
	{
	  document.addEventListener( "onkeypress", sstp_escTimepicker, false );
	}
	// IE 
	else if ( typeof document.attachEvent != "undefined" ) 
	{
	  document.attachEvent( "onkeypress", sstp_escTimepicker );
	}

	function sstp_isDigit(c) {
		
		return ((c=='0')||(c=='1')||(c=='2')||(c=='3')||(c=='4')||(c=='5')||(c=='6')||(c=='7')||(c=='8')||(c=='9'))
	}

	function sstp_isNumeric(n) {
		
		num = parseInt(n,10);

		return !isNaN(num);
	}

	function sstp_padZero(n) {
		v="";
		if (n<10){ 
			return ('0'+n);
		}
		else
		{
			return n;
		}
	}

	function sstp_validateTimePicker(ctl) {

		t=ctl.value.toLowerCase();
		t=t.replace(" ","");
		t=t.replace(".",":");
		t=t.replace("-","");

		if ((sstp_isNumeric(t))&&(t.length==4))
		{
			t=t.charAt(0)+t.charAt(1)+":"+t.charAt(2)+t.charAt(3);
		}
		
		if ((sstp_isNumeric(t))&&(t.length==3))
		{
			t=t.charAt(0)+":"+t.charAt(1)+t.charAt(2);
		}	

		var t=new String(t);
		tl=t.length;

		if (tl==1 ) {
			if (sstp_isDigit(t)) {
				t = t+":00 am";
			}
			else {
				return null;
			}
		}
		else if (tl==2) {
			if (sstp_isNumeric(t)) {
				if (parseInt(t,10)<13){
					if (t.charAt(1)!=":") {
						if (sstp_24Hr)
						{
							t = sstp_padZero(t) + ':00';
						}
						else {
							t = t + ':00 am';
						}
					} 
					else {
						if (sstp_24Hr)
						{
							t = sstp_padZero(t.substring(0,1)) + ':00';
						}
						else {
							t = t + '00 am';
						}
					}
				}
				else if (parseInt(t,10)==24) {
					if (sstp_24Hr)
					{
						t = "00:00";
					}
					else {
						t = "0:00 am";
					}
				}
				else if (parseInt(t,10)<24) {
					if (sstp_24Hr)
					{
						t = sstp_padZero(t)+':00';
					}
					else {
						if (t.charAt(1)!=":") {
							t = (t-12) + ':00 pm';
						} 
						else {
							t = (t-12) + '00 pm';
						}
					}
				}
				else if (parseInt(t,10)<=60) {
					if (sstp_24Hr)
					{
						t = '00:'+sstp_padZero(t);
					}
					else {
						t = '0:'+sstp_padZero(t)+' am';
					}
				}
				else {
					if (sstp_24Hr)
					{
						t = '01:'+sstp_padZero(t%60);
					}
					else {
						t = '1:'+sstp_padZero(t%60)+' am';
					}
				}
			}
			else
   		    {
				if ((t.charAt(0)==":")&&(sstp_isDigit(t.charAt(1)))) {
					if (sstp_24Hr) {
						t = "00:" + sstp_padZero(parseInt(t.charAt(1),10));
					}
					else {
						t = "0:" + sstp_padZero(parseInt(t.charAt(1),10)) + " am";
					}
				}
				else {
					return null;
				}
			}
		}

		var arr = t.split(":");
		if (t.indexOf(":") > 0)
		{
			hr=parseInt(arr[0],10);
			if (arr.length>2)
			{
				mn=parseInt(arr[2],10);
			}
			else {
				mn=parseInt(arr[1],10);
			}

			if (!sstp_24Hr) {
				if (t.indexOf("pm")>0) {
					mode="pm";
				}
				else {
					mode="am";
				}
			}
			else {
				mode="";
			}

			if (isNaN(hr)) {
				hr=0;
			} else {
				if (hr>24) {
					return null;
				}
				else if (hr==24) {
					if (!sstp_24Hr) {
						mode="am";
						hr=0;
					}
				}
				else if (hr>12) {
					if (!sstp_24Hr) {
						mode="pm";
						hr-=12;
					}
				}
			}
		
			if (isNaN(mn)) {
				mn=0;
			}
			else {
				if (mn>59) {
					mn=mn%60;
					hr+=1;
				}
			}
		} else {

			hr=parseInt(arr[0],10);

			if (isNaN(hr)) {
				hr=0;
			} else {
				if (hr>24) {
					return null;
				}
				else if (hr==24) {
					hr=0;
					if (!sstp_24Hr) {
						mode="am";
					}
				}
				else if (hr>12) {
					if (!sstp_24Hr) {
						mode="pm";
						hr-=12;
					}
				}
			}

			mn = 0;
		}
		
		if (hr==24) {
			hr=0;
			if (!sstp_24Hr) {
				mode="am";
			}
		}

		var date = new Date();
		date.setHours(hr);
		date.setMinutes(mn);

		if (sstp_24Hr) {
			ctl.value=sstp_padZero(hr)+":"+sstp_padZero(mn);
		}
		else {
			ctl.value=hr+":"+sstp_padZero(mn)+" "+mode;
		}

		if(!sstp_24Hr && mode==="pm" && hr < 12)
		{
			date.setHours(date.getHours() + 12);
		}

		return date;
	}
//-->