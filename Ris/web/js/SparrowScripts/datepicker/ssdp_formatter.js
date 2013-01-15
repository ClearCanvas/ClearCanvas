<!--
	function ssdp_padZero(num) {
		return (num	< 10) ? '0' + num : num;
	}

	function ssdp_constructDateLogic(d,m,y,format) {

		var selDate = new Date (y,m,d);

		sTmp = format;
		sTmp = sTmp.replace ('ddd', '<n>');
		sTmp = sTmp.replace ('dd','<e>');
		sTmp = sTmp.replace ('d',d);
		sTmp = sTmp.replace ('<e>',ssdp_padZero(d));
		sTmp = sTmp.replace ('<n>',ssdp_weekdayName[selDate.getDay()]);
		sTmp = sTmp.replace ('l',ssdp_longWeekdayName[selDate.getDay()]);
		sTmp = sTmp.replace ('mmmm','<p>');
		sTmp = sTmp.replace ('mmm','<o>');
		sTmp = sTmp.replace ('mm','<n>');
		sTmp = sTmp.replace ('m','<m>');
		sTmp = sTmp.replace ('<m>',m);
		sTmp = sTmp.replace ('<n>',ssdp_padZero(m));
		sTmp = sTmp.replace ('<o>',ssdp_monthName[m-1]);
		sTmp = sTmp.replace ('<p>',ssdp_monthName[m-1].substring(0,3).toUpperCase());
		sTmp = sTmp.replace ('yyyy',y);

		return sTmp.replace ('yy',ssdp_padZero(y%100));
	}

	function ssdp_checkDateFormat(ctl) {

		var arrMonthNames=new Array('jan','feb','mar','apr','may','jun','jul','aug','sep','oct','nov','dec');
		
		if (ctl.value=='')
		{
			return false;
		}

		var sInputString=ctl.value;
		var ss_allowedFormats="DMY,MDY,YMD,YDM";

		// rename all month name

		sInputString=sInputString.toLowerCase();
		sInputString=sInputString.replace(/,/g,'');
		nMonthPos=-1;

		for (i=0;i<12;i++)
		{
			if (nMonthPos==-1){
				if (sInputString.indexOf(arrMonthNames[i])>=0)
				{
					nMonthPos=sInputString.indexOf(arrMonthNames[i]);
					sInputString=sInputString.replace(arrMonthNames[i],ssdp_padZero(i+1));
				}
			}
		}

		// try to remove separators

		var aDateParts;
		var bSplitted=false;

		if (sInputString.indexOf(" ")>0) {
			aDateParts=sInputString.split(" ");
			bSplitted=true;
		}
		if (sInputString.indexOf("-")>0) {
			aDateParts=sInputString.split("-");
			bSplitted=true;
		}
		if (sInputString.indexOf(".")>0) {
			aDateParts=sInputString.split(".");
			bSplitted=true;
		}
		if (sInputString.indexOf("/")>0) {
			aDateParts=sInputString.split("/");
			bSplitted=true;
		}
		
		var nPart;

		if (bSplitted)
		{
			if (aDateParts.length==3)
			{
				for (i=0;i<3;i++)
				{
					nPart=parseInt(aDateParts[i],10);

					if (isNaN(nPart))
					{
						aDateParts[i]='*';
					}
					else if (aDateParts[i].length<2)
					{	
						aDateParts[i]='0'+aDateParts[i];
					}
				}
				sInputString=aDateParts[0]+'-'+aDateParts[1]+'-'+aDateParts[2];
			}
		}
		
		// remove all separators

		var ss_sDateString="";
		var i;
		var ch;

		for (i=0;i<sInputString.length;i++)
		{
			ch=sInputString.charAt(i);
			if ((ch>='0')&&(ch<='9'))
			{
				ss_sDateString+=ch;
			}
			else {
				if (i<nMonthPos)
				{
					nMonthPos--;
				}
			}
		}

		// check if date is of 6 or 8 characters

		var ss_aAllowedFormats=ss_allowedFormats.split(",");

		if ((ss_sDateString.length==6)||(ss_sDateString.length==8))
		{
			for (i=0;i<ss_aAllowedFormats.length;i++)
			{
				var bMatchedFormat=true;
				var nIndexNow=0;
				var nDatePos=0;
	
				for (var j=0;j<3;j++)
				{
					if (ss_aAllowedFormats[i].charAt(j)=='D')
					{
						if (parseInt(ss_sDateString.substring(nIndexNow,nIndexNow+2),10)>31)
						{		
							bMatchedFormat=false;
						}
						else {
							nDatePart=parseInt(ss_sDateString.substring(nIndexNow,nIndexNow+2),10);
							nDatePos=i;
						}
						nIndexNow+=2;
					}

					if (ss_aAllowedFormats[i].charAt(j)=='M')
					{
						if (parseInt(ss_sDateString.substring(nIndexNow,nIndexNow+2),10)>12)
						{		
							bMatchedFormat=false;
						}
						else {
							nMonthPart=parseInt(ss_sDateString.substring(nIndexNow,nIndexNow+2),10);
						}
						nIndexNow+=2;
					}

					if (ss_aAllowedFormats[i].charAt(j)=='Y')
					{
						nIncrement=ss_sDateString.length-4;

						nYearPart=parseInt(ss_sDateString.substring(nIndexNow,nIndexNow+nIncrement),10);
						if ((nIncrement==2)&&(nYearPart<ss_yearSplit))
						{
							nYearPart+=2000;
						}
						if (nYearPart<1900)
						{
							nYearPart+=1900;
						}

						if ((nYearPart<ssdp_minYear)||(nYearPart>ssdp_maxYear))
						{		
							bMatchedFormat=false;
						}

						nIndexNow+=nIncrement;
					}				
				}

				if (bMatchedFormat)
				{
					if (nDatePos==nMonthPos)
					{
						temp=nDatePart;
						nDatePart=nMonthPart;
						nMonthPart=temp;
					}

					if (nYearPart<1900)
					{	
						nYearPart+=1900;
					}	
					ssdp_currentYear=nYearPart;
					ssdp_currentMonth=nMonthPart;
					ssdp_currentDate=nDatePart;
					ctl.value=ssdp_constructDateLogic(nDatePart,nMonthPart,nYearPart,ssdp_dateFormat);
					showDatePicker(null,ctl.id);

					return true;
				}
			}

			if (!bMatchedFormat)
			{
				alert('No format matched!');
				return false;
			}
		}
		else {
			alert('Invalid Date!');
			return false;
		}
	}

//-->
