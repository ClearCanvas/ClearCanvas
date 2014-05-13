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

// create a global $ function that acts as an alias for document.getElementById
var $ = function(id) { return document.getElementById(id); }

var membersOf = function(obj) { var a = []; for(var k in obj) a.add(k); return a; }


/*
    Augments the javascript Array prototype with a number of convenience and functional-style methods, and some events.
    The following methods are added:
	add
	each
	map
	reduce
	select
	find
	indexOf
	removeAt
	remove
	isArray
	firstElement
        
    The following events are added:
        itemAdded (note: this event is only fired when the "add" method is called - assignment by [] does not invoke this event)
        itemRemoved
*/

// isArray always returns true - provides an easy way to test if an unknown object is an instance of an array or not
if(!Array.prototype.isArray)
{
    Array.prototype.isArray = true;
}

// adds an item to the end of the array
if(!Array.prototype.add){
	Array.prototype.add = function(obj)
	{
		var i = this.length;
		this[i] = obj;
		if(this.itemAdded)
			this.itemAdded(this, {item: obj, index: i});
	};
}

// iterates over the array, passing each element to the supplied function
if(!Array.prototype.each)
{
    Array.prototype.each = function(func)
    {
        for(var i = 0; i < this.length; i++)
            func(this[i]);
    }
}

// maps this array onto a new array, using the the supplied mapping function
if(!Array.prototype.map)
{
    Array.prototype.map = function(func)
    {
        var result = [];
        for(var i = 0; i < this.length; i++)
            result[i] = func(this[i]);
        return result;
    }
}

// reduces this array to a scalar value by calling the specified function for each item in the array,
// an taking the return value of the function as the next value of the "accumlator"
//      initial - the initial value of the accumlator
//      func - a function of the form func(accumlator, element), that returns a new value for the accumlator
// e.g. if x is an array of ints, then sum(x) = x.reduce(0, function(sum, y) { return sum + y; });
if(!Array.prototype.reduce)
{
    Array.prototype.reduce = function(initial, func)
    {
        var memo = initial;
        for(var i = 0; i < this.length; i++)
            memo = func(memo, this[i]);
        return memo;
    }
}

// returns a new array containing only those elements of this array that satisfy the specified predicate function
if(!Array.prototype.select)
{
    Array.prototype.select = function(func)
    {
        var result = [];
        for(var i = 0; i < this.length; i++)
            if(func(this[i]))
                result.push(this[i]);
        return result;
    }
}

// returns the first element of this array that satisfies the specified predicate function, or null
if(!Array.prototype.find)
{
    Array.prototype.find = function(func)
    {
        for(var i = 0; i < this.length; i++)
            if(func(this[i]))
                return this[i];
        return null;
    }
}

// returns the index of the specified object, or -1 if not found
if(!Array.prototype.indexOf)
{
	Array.prototype.indexOf = function(obj)
	{
        for(var i = 0; i < this.length; i++)
            if(this[i] == obj)
                return i;
        return -1;
	};
}

// returns an array containing only the unique elements of this array
if(!Array.prototype.unique)
{
	Array.prototype.unique = function(obj)
	{
		var result = [];
		for(var i = 0; i < this.length; i++)
		{
			var obj = this[i];
			if(result.indexOf(obj) < 0)
				result.push(obj);
		}
		return result;
	};
}

// removes the item at the specified index
if(!Array.prototype.removeAt)
{
	Array.prototype.removeAt = function(i)
	{
		var obj = this.splice(i, 1);
		if(this.itemRemoved)
			this.itemRemoved(this, {item: obj, index: i});
		return obj;
	};
}

// removes the specified item from the array, or does nothing if the item is not contained in this array
if(!Array.prototype.remove)
{
	Array.prototype.remove = function(obj)
	{
		var i = this.indexOf(obj);
		return (i > -1) ? this.removeAt(i) : null;
	};
}

// returns the first element, or null if the array is empty
if(!Array.prototype.firstElement)
{
	Array.prototype.firstElement = function()
	{
		return this.length > 0 ? this[0] : null;
	};
}

// transforms this array into an object that is a hash representation of the same data,
// mapping each element of the array to the corresponding key
if(!Array.prototype.toHash)
{
	// keys is an array of strings, e.g. ["Name", "Age", ...] that name the elements in this array
	Array.prototype.toHash = function(keys)
	{
		var obj = {};
		for(var i=0; i < keys.length; i++)
			obj[keys[i]] = this[i];
		return obj;
	};
}

// this method implements the same pattern as the LINQ group By operator 
// it is NOT the same as the SQL group by operator
// it transforms this array into a set of sub-arrays, partitioned according to the specified keyFunc
// the keyFunc maps an item in the array to a key
// this function returns an array of sub-arrays, each sub-array containing the set of items that map to a given key
// a "key" property is added to each sub-array, containing the key that was used to group it
// for example:
// 	["Adam", "Ana", "Bob", "Bill"].groupBy(function(item) { return item.substring(1); } )
// returns
//	[ ["Adam", "Ana"], ["Bob", "Bill"] ]
// where ["Adam", "Ana"].key => "A" and ["Bob", "Bill"].key => "B"
if(!Array.prototype.groupBy)
{
	Array.prototype.groupBy = function(keyFunc)
	{
		var keys = [];
		var groups = {};
        for(var i = 0; i < this.length; i++)
		{
			var value = this[i];
			var key = keyFunc(value);
			keys.push(key);	// store list of all keys encountered
			groups[key] = groups[key] || [];
			groups[key].push(value);
		}
		
		return keys.unique().map(function(k) { return groups[k]; });
	};
}

if(!String.prototype.escapeHTML)
{
    // from Prototype.js library (www.prototypejs.org)
    String.prototype.escapeHTML = function()
    {
        var div = document.createElement('div');
        var text = document.createTextNode(this);
        div.appendChild(text);
        return div.innerHTML;
    }
    
    // from Prototype.js library (www.prototypejs.org)
    String.prototype.unescapeHTML = function()
    {
        var div = document.createElement('div');
        div.innerHTML = this.stripTags();
        return div.childNodes[0].nodeValue;
    }
}

// Utility to perform string variable interpolation (kind of)
// returns a new string with the values plugged in:
// e.g.		"Hi %0, today is %1".interp("Bob", new Date()) -> "Hi Bob, today is 2014-05-12 ..."
if (!String.prototype.interp)
{
	String.prototype.interp = function(/* arguments */)
	{
		var result = this;
		for(var i = 0; i < arguments.length; i++) {
			var value = arguments[i];
			result = result.replace("%" + i, value);
		}
		return result;
	};
}

if(!String.replaceLineBreak)
{
	String.replaceLineBreak = function(value)
	{
		if (value === undefined || value === null)
			return "";

		// Ensure that the reportText object is actually a string (not a Number or Date or whatever)
		var newString = String(value);
		newString = newString.replace(/\\r\\n/g, "<br>"); 
		newString = newString.replace(/\r\n/g, "<br>");
		return newString.replace(/[\r\n]/g, "<br>");
    };
}

// utility to combine a list of strings with separator
if (!String.combine)
{
	String.combine = function(values, separator)
	{
		separator = separator ? (separator + "") : "";

		if (values == null || values.length == 0)
			return "";
			
		return values.reduce("", 
			function(memo, item) 
			{
				return memo.length == 0 ? item : memo + separator + item;
			});
	};
}


// add some decent date serialization support
if(!Date.prototype.toISOString)
{
    Date.prototype.toISOString = function () {

        function f(n) {
            // Format integers to have at least two digits.
            return n < 10 ? '0' + n : n;
        }

        return this.getFullYear() + '-' +
                f(this.getMonth() + 1) + '-' +
                f(this.getDate()) + 'T' +
                f(this.getHours()) + ':' +
                f(this.getMinutes()) + ':' +
                f(this.getSeconds());
    }
    
    Date.parseISOString = function(isoDateString)
    {
        var y = isoDateString.substring(0, 4);
        var m = isoDateString.substring(5, 7) - 1;
        var d = isoDateString.substring(8, 10);
        var h = isoDateString.substring(11, 13);
        var n = isoDateString.substring(14, 16);
        var s = isoDateString.substring(17, 19);

        return new Date(y, m, d, h, n, s);
   };
}

// add some methods to *safely* mutate the Date and Time portions of a date object without producing odd side-effects
if (!Date.prototype.setYMD)
{
	Date.prototype.setYMD = function(y, m, d)
	{
		// first we set the day of the month to 1, because every month has a day 1
		// this is to avoid rollover into the next month, when the month is subsequently modified
		this.setDate(1);
		
		// now we can safely set the year and month
		this.setFullYear(y);
		this.setMonth(m);
		
		// now we can set the day of the month safely
		// if d < 1 or d > numberOfDays(m), jscript will automatically roll the month backward or forward
		this.setDate(d);
	}
	
	Date.prototype.setHMS = function(h, m, s)
	{
		this.setHours(h);
		this.setMinutes(m);
		
		if(s)
		{
			this.setSeconds(s);
		}
	}
}
    
if (!Date.prototype.addDays)
{
	Date.prototype.addDays = function(offset)
	{
		var newDate = new Date(this);
		newDate.setDate(newDate.getDate() + offset);	
		return newDate;
	};
}

if (!Date.today)
{
	Date.today = function()
	{
		var today = new Date();
		today.setHours(0);
		today.setMinutes(0);
		today.setSeconds(0);
		today.setMilliseconds(0);
		return today;
	};
}

if (!Date.compare)
{
	// compare two date, a null date object is infinite into the future
	Date.compare = function(date1, date2)
	{
		if (date1 == null && date2 == null)
			return 0;
		else if (date1 == null)
			return 1;
		else if (date2 == null)
			return -1;
		else
			return Date.parse(date1) - Date.parse(date2);
	}

	// Compare whether date1 is more recent than date2.
	// Any time after the beginning of today is considered more recent than the past.
	// Time of null is considered infinite into the future.
	Date.compareMoreRecent = function(date1, date2)
	{
		if (date1 == date2)  // also take care of both equal null
			return 0;

		var today = Date.today();
		var dateOneMoreRecent = false;
		
		if (date1 == null)
		{
			if (Date.compare(date2, today) < 0)
				dateOneMoreRecent = true;  // time1 in the future, time2 in the past
		}
		else if (date2 == null)
		{
			if (Date.compare(date1, today) < 0)
				dateOneMoreRecent = false;  // time1 in the past, time2 in the future
		}
		else // both are not null
		{
			var timeOneSpan = Date.parse(date1) - Date.parse(today);
			var timeTwoSpan = Date.parse(date2) - Date.parse(today);

			if (timeOneSpan > 0 && timeTwoSpan > 0)
			{
				// Both in the future
				dateOneMoreRecent = timeOneSpan < timeTwoSpan;
			}
			else if (timeOneSpan < 0 && timeTwoSpan < 0)
			{
				// Both in the past
				dateOneMoreRecent = timeOneSpan > timeTwoSpan;
			}
			else
			{
				dateOneMoreRecent = timeOneSpan > timeTwoSpan;
			}
		}

		return dateOneMoreRecent ? -1 : 1;
	}
}

if(!Number.prototype.roundTo)
{
	Number.prototype.roundTo = function(precision)
	{
		// any falsy values
		if(!precision)
		{
			return Math.round(this);
		}
		else
		{
			return Number(this.toFixed(precision));
		}
	};
}
