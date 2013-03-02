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

/*

Helper class for creating class hiearchy based on javascript prototype.
    
*/    
ClassHelper = {};

ClassHelper.extend = function(subClass, baseClass) {
   function inheritance() {}
   inheritance.prototype = baseClass.prototype;

   subClass.prototype = new inheritance();
   subClass.prototype.constructor = subClass;
   subClass.baseConstructor = baseClass;
   subClass.superClass = baseClass.prototype;
}


function ValidationResult()
{
    this.OK = false;
    this.Skipped = false;
    this.Message = null;
    this.ErrorCode = 0;
}


function BaseClientValidator(
    inputID,
    inputNormalColor,
    inputInvalidColor,
    inputNormalBorderColor,
    inputInvalidBorderColor,
    inputNormalCSS,
    inputInvalidCSS,    
    errorIndicator,
    errorIndicatorTooltip,
    errorIndicatorTooltipPanel,
    ignoreEmptyValue,
    conditionalCtrl,
    validateWhenUnchecked
    )
{
    //alert('BaseClientValidator constructor');
    //alert('inputID='+inputID);
    this.inputID = inputID;
    
    //alert('inputNormalColor='+inputNormalColor);
    this.inputNormalColor = inputNormalColor;
    
    //alert('inputInvalidColor='+inputInvalidColor);
    this.inputInvalidColor = inputInvalidColor;
    
    //alert('inputNormalBorderColor='+inputNormalBorderColor);
    this.inputNormalBorderColor = inputNormalBorderColor;
    
    //alert('inputInvalidBorderColor='+inputInvalidBorderColor);
    this.inputInvalidBorderColor = inputInvalidBorderColor;

    //alert('inputNormalBorderColor='+inputNormalBorderColor);
    this.inputNormalCSS = inputNormalCSS;

    //alert('inputInvalidBorderColor='+inputInvalidBorderColor);
    this.inputInvalidCSS = inputInvalidCSS;    
    
    //alert('errorIndicator='+errorIndicator);
    this.errorIndicator = errorIndicator;
    
    this.input = document.getElementById(inputID);
    //alert('input='+this.input);
    
    this.errorIndicatorTooltip = errorIndicatorTooltip;
    //alert('errorIndicatorTooltip='+errorIndicatorTooltip);
    
    this.errorIndicatorTooltipPanel = errorIndicatorTooltipPanel;
    //alert('errorIndicatorTooltipPanel='+errorIndicatorTooltipPanel);
    
    this.ignoreEmptyValue = ignoreEmptyValue;
    
    this.conditionalCtrl = conditionalCtrl;
    this.validateWhenUnchecked = validateWhenUnchecked;
}

BaseClientValidator.prototype.ShouldSkip = function() {
        
    if(this.input.style.display == 'none') return true;

    if (this.conditionalCtrl != null) {
        if (this.validateWhenUnchecked) {
            // SKIP IF CHECKED
            if (this.conditionalCtrl.checked)
                return true;
        }
        else {
            // SKIP IF UNCHECKED
            if (!this.conditionalCtrl.checked)
                return true;
        }
    }

    var val = this.input.value;

    if (val == null || val == '') {
        return this.ignoreEmptyValue;
    }        

    return false;
}

BaseClientValidator.prototype.OnEvaluate = function() {
    var result = new ValidationResult();
    result.OK = true; // init

    if (this.ShouldSkip()) {
        result.Skipped = true;
        result.OK = true;
        return result;
    }


    return result;
};

BaseClientValidator.prototype.OnValidationPassed = function() {
    //alert('Base validator: input is valid: color=' + this.inputNormalColor);
    if (this.input['validatorscounter'] == '1') {
        // only myself is attached to this input, it's ok to clear the background
        this.input.style.backgroundColor = this.inputNormalColor;
        this.input.style.borderColor = this.inputNormalBorderColor;
        this.input.className = this.inputNormalCSS;
    }
    else {
        if (this.input['calledvalidatorcounter'] == 0) {
            // I am the first validator called to check the input, it's safe to clear the background
            this.input.style.backgroundColor = this.inputNormalColor;
            this.input.style.borderColor = this.inputNormalBorderColor;
            this.input.className = this.inputNormalCSS;
        }
    }

    this.UpdateErrorIndicator(null, true);
    
    this.input['calledvalidatorcounter']++;
    if (this.input['calledvalidatorcounter'] == this.input['validatorscounter']) {
        // no more validator in the pipe, let's reset the calledvalidatorcounter value so that 
        // next time we validate the input we start from 0.
        this.input['calledvalidatorcounter'] = 0;
    }

};

BaseClientValidator.prototype.OnValidationFailed = function(error) {
    //alert('Base validator: input is invalid');
    this.input.style.backgroundColor = this.inputInvalidColor;
    this.input.style.borderColor = this.inputInvalidBorderColor;
    this.input.className = this.inputInvalidCSS;

    this.UpdateErrorIndicator(error, false);

    this.input['calledvalidatorcounter']++;
    if (this.input['calledvalidatorcounter'] == this.input['validatorscounter']) {
        // no more validator in the pipe, let's reset the calledvalidatorcounter value so that 
        // next time we validate the input we start from 0.
        this.input['calledvalidatorcounter'] = 0;
    }
};

BaseClientValidator.prototype.UpdateErrorIndicator = function(error, passed) {

    if (this.errorIndicator != null) {
        if (passed) {
            // I am not sharing the popup help control with any other validators. It's safe to hide it
            if (this.errorIndicator['shared'] != 'true')
                this.errorIndicator.style.visibility = 'hidden';
            else {
                //The error indicator is shared. Since this control has passed validation,
                //subtract it from the number of invalid fields.
                if (this.errorIndicator['numberofinvalidfields'] > 0)
                    this.errorIndicator['numberofinvalidfields']--;

                //If this error indicator has at least one input that failed, then 
                //we make sure the indicator visible.    
                if (this.errorIndicator['numberofinvalidfields'] > 0)
                    this.errorIndicator.style.visibility = 'visible';
                else
                    this.errorIndicator.style.visibility = 'hidden';
            }

        } else {
            this.errorIndicator['numberofinvalidfields']++;
            this.errorIndicator.style.visibility = 'visible';
            this.SetErrorMessage(error);
        }
    }
}

function GetStringWidth(ElemStyle, text) 
{
    var newSpan = document.createElement('span');
    newSpan.style.fontSize = ElemStyle.fontSize;
    newSpan.style.fontFamily = ElemStyle.fontFamily;
    newSpan.style.fontStyle = ElemStyle.fontStyle;
    newSpan.style.fontVariant = ElemStyle.fontVariant;
    newSpan.style.fontWieght = ElemStyle.fontWieght;
    newSpan.innerText = text;
    document.body.appendChild(newSpan);
    var iStringWidth = newSpan.offsetWidth;
    newSpan.removeNode(true);
    return iStringWidth;
}


BaseClientValidator.prototype.SetErrorMessage = function(result)
{
   
    if (this.errorIndicatorTooltip!=null)
    {
        this.errorIndicatorTooltip.innerText= result.Message;
        w = GetStringWidth(this.errorIndicatorTooltip.style, result.Message);
        
        // break it up into multiple lines
        this.errorIndicatorTooltipPanel.style.width = w;
        w = parseInt(this.errorIndicatorTooltipPanel.style.width);        
        if (isNaN(w) || w > 300)
        {
            this.errorIndicatorTooltipPanel.style.width = '300px';
        }
        
    }
}
