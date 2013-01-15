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
    This module defines JSML (Javascript Markup-language)
    
    JSML is largely based on the ideas contained in JSON (www.json.org) and some of the code has be
    adapted from the reference json implementation (www.json.org/json.js).
    
    JSML (Javascript Markup-language) is intended to provide a simple means of serializing a Javascript
    object graph to XML format.  The graph must not contain any cyclical references.
    
    This file adds these methods to JavaScript:

    JSML.create(obj, rootTag)
        Creates a JSML representation of the specified obj, using
        rootTag as the tag name of the root XML element.
        

    JSML.parse(jsml)
        This method parses JSML text to produce an object or
        array.
        
    array.toJsml()
    boolean.toJsml()
    date.toJsml()
    number.toJsml()
    object.toJsml()
    string.toJsml()
        These methods produce JSML text from a JavaScript value.
        It must not contain any cyclical references. Illegal values
        will be excluded.

        The default conversion for dates is to an ISO string. You can
        add a toJsml method to any date object to get a different
        representation.

*/

// augment the basic Javascript classes with Jsml
if(!Object.prototype.toJsml)
{

    Object.prototype.toJsml = function()
    {
        var xml = "";
        for(var prop in this)
        {
            // check that the prop belongs to this object (not its prototype) and that the value is non-null and is not a function
            if(this.hasOwnProperty(prop) && (this[prop] !== null) && (this[prop] !== undefined) && !(this[prop] instanceof Function))
                xml += JSML.create(this[prop], prop);
        }
        return [xml, "hash"];
    }
    
    Array.prototype.toJsml = function()
    {
        var xml = this.reduce("", function(jsml, item) { return jsml + JSML.create(item, "item"); });
		return [xml, "array"];
    }
    
    Boolean.prototype.toJsml = function () {
        return [String(this), null];
    };


    Date.prototype.toJsml = function () {
        return [this.toISOString(), null];
    };

    Number.prototype.toJsml = function () {
    // JSON numbers must be finite. Encode non-finite numbers as null.
        return [isFinite(this) ? String(this) : "null", null];
    };
    
    String.prototype.toJsml = function () {
/*
        // If the string contains no control characters, no quote characters, and no
        // backslash characters, then we can simply return it.
        // Otherwise we must also replace the offending characters with safe
        // sequences.

        if (/["\\\x00-\x1f]/.test(this)) {
            return '"' + this.replace(/([\x00-\x1f\\"])/g, function (a, b) {
                var c = m[b];
                if (c) {
                    return c;
                }
                c = b.charCodeAt();
                return '\\u00' +
                    Math.floor(c / 16).toString(16) +
                    (c % 16).toString(16);
            }) + '"';
        }
*/        
		// Bug: #3480 - previous escapeHtml() call did not preserve line breaks.
		var escaped = this.replace(/&/g,'&amp;').replace(/>/g,'&gt;').replace(/</g,'&lt;').replace(/"/g,'&quot;').replace(/'/g,'&apos;');
        return [escaped, null];
    };
};

// definition of the JSML object		    
var JSML = {
    // default no-op parse filter
    _parseFilter: function(key, value)
    {
        return value;
    },
    
    /* Sets a global parse filter callback.
        The filter callback is a function which can filter and
        transform the results. It receives each of the keys and values, and
        its return value is used instead of the original value. If it
        returns what it received, then structure is not modified. If it
        returns undefined then the member is deleted.

        Example:

        // Parse the text. If a key contains the string 'date' then
        // convert the value to a date.

        function myFilter(key, value) {
            return key.indexOf('date') >= 0 ? new Date(value) : value;
        });
    */
    setParseFilter: function(filterFunc)
    {
        this._parseFilter = filterFunc;
    },
    
    parse: function(jsml)
    {
        function parseXml(xml) {
           var dom = null;
           if (window.DOMParser) {
                 return (new DOMParser()).parseFromString(xml, "text/xml"); 
           }
           else if (window.ActiveXObject) {
                 dom = new ActiveXObject('Microsoft.XMLDOM');
                 dom.async = false;
				 dom.preserveWhiteSpace = true;
                 if (!dom.loadXML(xml)) // parse error ..
                    throw (dom.parseError.reason + dom.parseError.srcText);

                 return dom;
           }
        }
        
        // put child nodes in an array for convenience
        function getChildNodes(xmlNode)
        {
            var a = [];
            for (var n=xmlNode.firstChild; n; n=n.nextSibling)
                a.push(n);
            return a;
        }
        
        // convert a JSML fragment to a Javascript object
        function toObj(xmlNode)
        {
            var subElements = getChildNodes(xmlNode).select(function(n) { return n.nodeType == 1; });   // select element nodes
            var arrayAttr = xmlNode.attributes.getNamedItem("array");  // for backward compatibility
            var hashAttr = xmlNode.attributes.getNamedItem("hash");  // for backward compatibility
            var typeAttr = xmlNode.attributes.getNamedItem("type");

			var isArray = (arrayAttr && arrayAttr.text == "true") || (typeAttr && typeAttr.text == "array");
			var isHash = (hashAttr && hashAttr.text == "true") || (typeAttr && typeAttr.text == "hash");

            if(isArray)
            {
                // collect sub-elements in an array
                return subElements.reduce([], function(a, node) { a.push(toObj(node)); return a; });
            }
			else if(isHash)
			{
				// treat sub-elements as independent properties
				return subElements.reduce({},
					function(o, n)
					{
						o[n.nodeName] = JSML._parseFilter(n.nodeName, toObj(n));
						return o;
					});
			}
            else // node contains text
            {
				// find the first non-empty text node
				var textNodes = getChildNodes(xmlNode).select(function(n) { return n.nodeType==3; }); // && n.nodeValue.match(/[^ \f\n\r\t\v]/); });
				var value = textNodes.length > 0 ? textNodes[0].nodeValue : "";
				return value != null ? parseValue(value) : null;
            }
        }
        
        // deserialize value to an object
        function parseValue(str)
        {
            // since the JSML does not contain any type information,
            // we try to guess the most appropriate javascript type from the format of the string
        
            // does the string look like an integer?
            // (note that we purposely exclude strings beginning with 0 from this test,
            // because a string beginning with 0 was probably meant to be a string)
            if(str.match(/^[+-]?[1-9]\d*$/))
                return parseInt(str);
            
            // does the string look like a floating point number?    
            if(str.match(/^[+-]?\d*\.\d+$/))
                return parseFloat(str);
                
            // does the string consist only of the word "true" or "false" (case insensitive)?    
            if(str.match(/^(true|false)$/i))
                return str.toLowerCase() == "true";
            
            // is the string an ISO formatted date?    
            if(str.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$/))
                return Date.parseISOString(str);
            
            // treat it as a string
            return str;
        }
        
        if(jsml && jsml.length)
        {
            var dom = parseXml(jsml);
            return dom.documentElement ? toObj(dom.documentElement) : null;
        }
        else
        {
            return null;
        }
    },
    
    create: function(obj, tagName)
    {
        // get the JSML representation of obj, using its implementation of toJsml
        // if it does not have an implementation of toJsml defined, then convert it to a string first
        // (this is a bit of hack to deal with window.location and other such DOM objects that essentially 
        // act as strings but are not technically Javascript string objects)

		if (obj == null)
			return null;

        var r = obj.toJsml ? obj.toJsml() : obj.toString().toJsml();
		var objJsml = r[0];
		var typeAttr = r[1];
        
        // embed in tag
        return '<'+tagName+((typeAttr) ? (' type="'+typeAttr+'"'):'')+'>' + objJsml + '</'+tagName+'>';
    }    
};
