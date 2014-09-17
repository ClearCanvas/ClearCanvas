/*
	Define the global SR object.
	
		SR serves as both the root namespace for all loaded resource strings,
		as well as a function for adding resources strings to the SR namespace.
*/
var SR = function(resources) {
	// mix resources into SR object
	for(var k in resources)
		SR[k] = resources[k];
};

// Load resource strings for current culture
(function() {

	function parseQueryString(url) {
		var qs = url.split('?')[1];
		if(!qs) return {};
		
		var result = {};
		var pairs = qs.split('&');
		for(var i = 0; i < pairs.length; i++) {
			var pair = pairs[i];
			var tuple = pair.split('=');
			if(tuple[0])
				result[tuple[0]] = tuple[1];
		}
		return result;
	}


	var scripts = document.getElementsByTagName("script");
	var thisScript = scripts[scripts.length - 1];
	
	// get language from query string, defaulting to en-us if not specified
	var qsMap = parseQueryString(String(window.location));
	var lang = qsMap["lang"] || "en-us";
	if (lang == "en") lang = "en-us";
	
	var script = document.createElement("script");
	// insert SR script right after this script
	thisScript.parentNode.insertBefore(script, thisScript.nextSibling);
	script.type = "text/javascript";
	script.charset = "UTF-8";
	script.src = String(thisScript.src).replace(/\/[^\/]+\.js$/i, "/resources/resources." + lang + ".js");
	
})();