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

// This script defines the client-side validator extension class @@CLIENTID@@_ClientSideEvaludator
// to validate an input using a web service.
//
// This class defines how the validation is carried and what to do afterwards.


// derive this class from BaseClientValidator
ClassHelper.extend(@@CLIENTID@@_ClientSideEvaluator, BaseClientValidator);

//constructor
function @@CLIENTID@@_ClientSideEvaluator()
{
    BaseClientValidator.call(this, 
            '@@INPUT_CLIENTID@@',
            '@@INPUT_NORMAL_BKCOLOR@@',
            '@@INPUT_INVALID_BKCOLOR@@',
            '@@INPUT_NORMAL_BORDERCOLOR@@',
            '@@INPUT_INVALID_BORDERCOLOR@@',            
            '@@INPUT_NORMAL_CSS@@',
            '@@INPUT_INVALID_CSS@@',                                    
            '@@INVALID_INPUT_INDICATOR_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'),
            '@@IGNORE_EMPTY_VALUE@@'
    );
    
    this.webserviceURL = '@@WEBSERVICE_URL@@';
    this.webserviceOperation = '@@WEBSERVICE_OPERATION@@';
    this.paramFunc =  @@PARAMETER_FUNCTION@@;
    this.timeout = 1000;
}

// override BaseClientValidator.OnEvaludate() 
// This function is called to evaluate the input
@@CLIENTID@@_ClientSideEvaluator.prototype.OnEvaluate = function()
{
    result = BaseClientValidator.prototype.OnEvaluate.call(this);
    
    if (result.OK==false)
    {
        return result;
    }
    
    
    callObj = service.createCallOptions();
    callObj.funcName = this.webserviceOperation;
    callObj.async = false;
    callObj.timeOut = this.timeout;
    callObj.params = this.paramFunc();

    var res;  
    resObj = service.ValidationServices.callService(callObj, res);

    if (!resObj.error)
    {
        wsResult = eval(resObj.value); 

        if (!wsResult.Success)
        {
            // errorCode > 0 is treated as warning from the web service. The client can ignore warning
            if (wsResult.ErrorCode > 0)
            {
                var ans = window.confirm('Unable to validate @@INPUT_NAME@@ : ' + wsResult.ErrorText + '\nWould you like to ignore it?');
                if (ans)
                {
                    result.OK = true;
                }
                else
                {
                    result.OK = false;
                    result.Message = 'Unable to validate this field: '+ wsResult.ErrorText;
                }
            }
            else
            {
                result.OK = false;
                result.Message = wsResult.ErrorText;
            }
        }
        else
        {
        }
        
    }
    else
    {
        alert('Error occured while calling ' + this.webserviceOperation + ' at ' + this.webserviceURL +' : ' + resObj.errorDetail.string);
        result.OK = false;
        result.Message = resObj.errorDetail.string;
    }
    
    return result;

};

//@@CLIENTID@@_ClientSideEvaluator.prototype.OnValidationPassed = function()
//{
//    //alert('Length validator: input is valid');
//    BaseClientValidator.prototype.OnValidationPassed.call(this);
//}

//@@CLIENTID@@_ClientSideEvaluator.prototype.OnValidationFailed = function(error)
//{
//    //alert('Length validator: input is valid : ' + error);
//    BaseClientValidator.prototype.OnValidationFailed.call(this, error);
//        
//}

//@@CLIENTID@@_ClientSideEvaluator.prototype.SetErrorMessage = function(result)
//{
//    BaseClientValidator.prototype.SetErrorMessage.call(this, result);
//    alert(result.Message);
//}
