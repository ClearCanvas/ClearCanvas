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

var tabsInitialized = false;

function TabControl(tabControlId)
{
	this.currentTabPage = null;
	this.tabControlId = tabControlId;

	var labelElements = document.getElementById(this.tabControlId).getElementsByTagName("LABEL");
	for(var i=0; i<labelElements.length; i++)
	{
		if(labelElements[i].getAttribute(classAttribute) == "Tab")
		{
			pageElement = document.getElementById(labelElements[i].getAttribute(forAttribute));
			page = new TabPage(this, labelElements[i], pageElement);
			if(this.currentTabPage == null)
			{
				page.Show();
			}
		}
	}
}

TabControl.prototype.HideCurrent = function ()
{
	if(this.currentTabPage != null)
	{
		this.currentTabPage.Hide();
	}
};

function TabPage(tabControl, tabElement, pageElement)
{
	this.tabControl = tabControl;
	this.tabElement = tabElement;
	this.pageElement = pageElement;
	
	var oThis = this;
	var oldOnClick = this.tabElement.onclick || function() {};
	oThis.tabElement.onclick = function() { oldOnClick(); oThis.Show() };
}

TabPage.prototype.PageClassName = "TabPage";
TabPage.prototype.ActivePageClassName = "TabPage ActiveTabPage";
TabPage.prototype.TabClassName = "Tab";
TabPage.prototype.ActiveTabClassName = "Tab ActiveTab";

TabPage.prototype.Show = function()
{
	if(this.tabControl.currentTabPage == this) return;

	this.tabControl.HideCurrent();
	
	this.pageElement.className = this.ActivePageClassName;
	this.tabElement.className = this.ActiveTabClassName;

	this.tabControl.currentTabPage = this;
};

TabPage.prototype.Hide = function()
{
	this.pageElement.className = this.PageClassName;
	this.tabElement.className = this.TabClassName;
};

function initTabs()
{
	if (tabsInitialized)
		return;

	var divs = document.getElementsByTagName("DIV");
	for(var i=0; i < divs.length; i++)
	{
		if(divs[i].getAttribute(classAttribute) == "TabControl")
		{
			new TabControl(divs[i].getAttribute("ID"));
		}
	}
	
	tabsInitialized = true;
}

function resetTabs()
{
	tabsInitialized = false;
	initTabs();
}

var classAttribute = "class";
var forAttribute = "for";

if ( typeof window.addEventListener != "undefined" )
{
  window.addEventListener( "load", initTabs, false );
}
// IE 
else if ( typeof window.attachEvent != "undefined" ) 
{
  window.attachEvent( "onload", initTabs );
  classAttribute = "className";
  forAttribute = "htmlFor";
}
