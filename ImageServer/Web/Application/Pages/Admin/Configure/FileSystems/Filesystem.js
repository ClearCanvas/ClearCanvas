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

var sizeTotal;
var sizeUsed;

// This function calls the Web Service method.  
function LoadFilesystemInfo()
{
     path = document.getElementById('@@PATH_INPUT_CLIENTID@@').value;

     ClearCanvas.ImageServer.Web.Application.Services.FilesystemInfoService.GetFilesystemInfo(path, OnLoadFilesystemInfoSuccess, OnLoadFilesystemInfoError);     
}
    
function RecalculateWatermark()
{
    HighWatermarkSize = document.getElementById('@@HW_SIZE_CLIENTID@@');
    hwtxt = document.getElementById('@@HW_PERCENTAGE_INPUT_CLIENTID@@').value;
    hwpct = parseNumber(hwtxt) / 100.0;
    if (!isNaN(hwpct))
    {
        hwsize = hwpct * sizeTotal.value;
                                     
        HighWatermarkSize.value = FormatSize(hwsize);
    }   
    else
    {
        HighWatermarkSize.value = '';
    }

    LowWatermarkSize = document.getElementById('@@LW_SIZE_CLIENTID@@');
    lwtxt = document.getElementById('@@LW_PERCENTAGE_INPUT_CLIENTID@@').value;
    lwpct = parseNumber(lwtxt) / 100.0;
    if (!isNaN(lwpct))
    {
        lwsize = lwpct * sizeTotal.value;
                                     
        LowWatermarkSize.value = FormatSize(lwsize);
    }   
    else
    {
        LowWatermarkSize.value = '';
    }
}

function DisableWatermarkInput() {

    $("#@@LW_PERCENTAGE_INPUT_CLIENTID@@").attr('disabled', 'disabled');
    $("#@@HW_PERCENTAGE_INPUT_CLIENTID@@").attr('disabled', 'disabled');
}

function EnableWatermarkInput() {

    $("#@@LW_PERCENTAGE_INPUT_CLIENTID@@").removeAttr('disabled');
    $("#@@HW_PERCENTAGE_INPUT_CLIENTID@@").removeAttr('disabled');
}

function FormatFloatNumber(x) {
    return x.toFixed(5);
}

// Returns a string containing a percentage value formatted to 
// the number of decimal places specified by "decimalpoints"
function FormatPercentage(value, decimalpoints)
{
    var pct = value * 100.0;
    return pct.toFixed(decimalpoints) + '%';
}

// Returns a string formatted to the appropriate size (MB, GB, TB)
// provided a number that is the size in kilobytes (KB)
function FormatSize(sizeInKB)
{
        MB = 1024; //kb
        GB = 1024*MB;
        TB = 1024 * GB;
        
        if (sizeInKB > TB)
        {
            var num = sizeInKB / TB;
            return FormatFloatNumber(num)  + ' TB';
        }
        else if (sizeInKB > GB)
        {
            var num = (sizeInKB / GB);
            return FormatFloatNumber(num) + ' GB';
        }
        else if (sizeInKB > MB)
        {
            var num = sizeInKB / MB;
            return FormatFloatNumber(num) + ' MB';
        }
        else {
            
            return '' + FormatFloatNumber(num)  + ' KB';
        }
}

function ValidationFilesystemPathParams()
{
    control = document.getElementById('@@PATH_INPUT_CLIENTID@@');
    params = new Array();
    params.path=control.value;

    return params;
}

function OnLoadFilesystemInfoError(result) 
{
    DisableWatermarkInput();
}


// This is the callback function that
// processes the Web Service return value.
function OnLoadFilesystemInfoSuccess(result)
{
    filesysteminfo = result;
    if (filesysteminfo!=null && filesysteminfo.Exists)
    {
    
        sizeTotal = document.getElementById('@@TOTAL_SIZE_CLIENTID@@');
        sizeTotal.value = filesysteminfo.SizeInKB;
        
        sizeUsed = document.getElementById('@@USED_SIZE_CLIENTID@@');
        sizeUsed.value = sizeTotal.value - filesysteminfo.FreeSizeInKB;

        total = document.getElementById('@@TOTAL_SIZE_INDICATOR_CLIENTID@@');                                       
        total.innerHTML = FormatSize(sizeTotal.value);
        
        used = document.getElementById('@@USED_SIZE_INDICATOR_CLIENTID@@');
        used.innerHTML = FormatSize(sizeUsed.value) + ' (' + FormatPercentage(sizeUsed.value/sizeTotal.value, 4) +')';

        EnableWatermarkInput();
        RecalculateWatermark();
        
    }
    else
    {
        sizeTotal = document.getElementById('@@TOTAL_SIZE_CLIENTID@@');
        sizeTotal.value = 0;
        
        sizeUsed = document.getElementById('@@USED_SIZE_CLIENTID@@');
        sizeUsed.value  = 0;
        
        total = document.getElementById('@@TOTAL_SIZE_INDICATOR_CLIENTID@@');                                       
        total.innerHTML = SR.Unknown;
        
        used = document.getElementById('@@USED_SIZE_INDICATOR_CLIENTID@@');
        used.innerHTML = SR.Unknown;

        RecalculateWatermark();

        DisableWatermarkInput();
    }
    
}

