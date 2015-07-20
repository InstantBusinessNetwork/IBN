var viewport;
var mainWidth;
var firstLeftLoad = true;

function initLayout(showLeftFrame, showPartTopFrame, showTopTabsFrame, leftFrameWidth)
{
	mainWidth = parseInt(leftFrameWidth);
	if(mainWidth < 175)
		mainWidth = 176;
	if(mainWidth > 400)
		mainWidth = 399;
	var str = showLeftFrame;
	var hidden = false;
	if(str=="0")
		hidden = true;
	
	viewport = new Ext.Viewport({
		layout: 'border',
		autoShow: true,
		items: [{
			region: 'north',
			xtype: 'panel',
			contentEl: 'up_div',
			border: false,
			margins: '0 0 0 0',
			height: 89
		}, {
			region: 'west',
			html: '<iframe frameborder="0" scrolling="no" name="leftFrame" id="leftFrame" width="100%" marginheight="0" marginwidth="0" height="100%" src="" ></iframe>', 
			collapsible: true,
			collapsed: hidden,
			collapseMode: 'mini',
			width: mainWidth,
			minSize: 175,
			maxSize: 400,
			split: true,
			listeners: {
				beforecollapse: function(p) {
					HideLeft();
				},
				beforeexpand: function(p) {
					ShowLeft();
				},
				resize: function(p1,p2,p3,p4,p5) {
					if(viewport && p4>0 && mainWidth != p4)
					{
						mainWidth = p4;
						MoveFrame(p4);
					}
				}
			}
		}, 
		{
			region:'center',
			border: false,
			items:
				new Ext.Panel({
					region:'center',
					items:[{
						region:  'north',
						html: '<iframe frameborder="0" scrolling="auto" name="topright" id="topright" width="100%" height="200px" marginheight="0" marginwidth="0" src="../Common/Empty.html" ></iframe>',
						border: false,
						margins: '0 0 0 0',
						height: 200
					},{
						region:'center',
						border: false,
						contentEl:'center_div'
					}]
			}),
			listeners: {
					resize: function(p1,p2,p3,p4,p5) {
						ResizeIFrame();
					}
			}
		}]
	});
	
	if(str=="1")
		ShowLeft();
	else
		HideLeft();
		
	var str1 = showPartTopFrame;
	if(str1=="1")
		ShowTopPart();
	else
		HideTopPart();
		
	var str2 = showTopTabsFrame;
	if(str2=="1")
		viewport.layout.center.items.items.items[0].show();
	else
		viewport.layout.center.items.items.items[0].hide();
	
	firstLeftLoad = false;
	ResizeIFrame();
}

function ResizeIFrame()
{
	if(viewport && viewport.layout && viewport.layout.center && viewport.layout.center.panel)
	{
		var obj = document.getElementById("right");
		var sHeight = 0;
		if(!viewport.layout.center.items.items.items[0].hidden)
			sHeight = viewport.layout.center.items.items.items[0].height;
		obj.height = (viewport.layout.center.panel.getInnerHeight() - sHeight - 2).toString() + "px";
	}
}

function ShowLeft()
{
	if(!firstLeftLoad)
		MoveFrame(null);
	var f_Left = document.getElementById("leftFrame");
	if(f_Left.src.indexOf("/Modules/leftTemplate1.aspx")<0)
		f_Left.src = "../Modules/leftTemplate1.aspx";
}

function HideLeft()
{
	if(!firstLeftLoad)
		MoveFrame(null);
	var f_Left = document.getElementById("leftFrame");
	if(f_Left.src.indexOf("/Modules/leftTemplate1.aspx")>=0)
		f_Left.src = "";
}

function initIframe()
{
	var iframe = document.getElementById("right");
	if (iframe)
	{	
		if (iframe.addEventListener)
		{
			iframe.addEventListener("load", iframeSrcChangeHandler, false);
		}
		else
		{
			iframe.attachEvent("onload", iframeSrcChangeHandler);
		}
	}
}

function iframeSrcChangeHandler()
{
	var newState, currentState, iframe; 
	iframe = document.getElementById("right");
	if (iframe)
	{
		newState = window.frames["right"].document.location.href;
        currentState = YAHOO.util.History.getCurrentState("right"); 
			
		if (newState != currentState && newState.toLowerCase().indexOf('common/empty.html') == -1)  
		{
			YAHOO.util.History.navigate("right", newState);	
		}
	}
}

function iframeSrcChangeHandler2()
{
	var newState, currentState, iframe; 
	iframe = document.getElementById("right");
	
	if (iframe)
	{
		newState = window.frames["right"].document.location.href;
		currentState = YAHOO.util.History.getCurrentState("right"); 
		if (newState != currentState)
		{
			//COMMENTED - FIX FAV ICON BUG - isGecko
			if(!Ext.isGecko)
			{
				YAHOO.util.History.navigate("right", currentState);
			}
		}
	}
}

function yahooHMChange()
{	
	var newState, currentState, iframe; 
	currentState = YAHOO.util.History.getCurrentState("right"); 
	iframe = document.getElementById("right");
	if (currentState != '' && currentState != 'none' && iframe)
	{
		newState = window.frames["right"].document.location.href;

	    if(currentState != newState)
		    iframe.setAttribute("src", currentState);
	}				
}

function yahooOnReady()
{
	iframeSrcChangeHandler2();
	yahooHMChange();
}

function ExpandCollapse()
{
	var req = window.XMLHttpRequest? 
			new XMLHttpRequest() : 
			new ActiveXObject("Microsoft.XMLHTTP");
	req.onreadystatechange = function() 
	{
		if (req.readyState != 4 ) return ;
		if (req.readyState == 4)
		{
			if (req.status == 200)
			{
				if(req.responseText.toString()=='1')
					HideTopPart();
				else
					ShowTopPart();
			}
			else
				alert("There was a problem retrieving the XML data:\n" + req.statusText);
		}
	}
	var dt = new Date();
	var sID = dt.getMinutes() + "_" + dt.getSeconds() + "_" + dt.getMilliseconds();
	req.open("GET", "XmlForTreeView.aspx?MoveFrame=t&sID="+sID, true);
	req.send(null);
}

function ExpandCollapse2()
{
	var img = document.getElementById("imgExpandCollapse2");
	var span = document.getElementById("HiddenItems");
	if (span == null)
		return;
	if (span.style.display == "none")
	{
		span.style.display = "inline";
		img.src = "../layouts/images/scrollright_hover_white.gif";
	}
	else
	{
		span.style.display = "none";
		img.src = "../layouts/images/scrollleft_hover_white.gif";
	}
}

function HideTopPart()
{
	var fset = viewport.layout.north;
	fset.panel.hide();
	fset.panel.setHeight(25);
	fset.panel.show();
	viewport.doLayout();
	
	var img = document.getElementById("imgExpandCollapse");
	if (img)
		img.src = "../layouts/images/scrolldown_hover_white.gif";
		
	var bannerTable = document.getElementById("BannerTable");
	if (bannerTable)
		bannerTable.style.display = "none";
}

function ShowTopPart()
{
	var fset = viewport.layout.north;
	fset.panel.hide();
	fset.panel.setHeight(85);
	fset.panel.show();
	viewport.doLayout();
	
	var img = document.getElementById("imgExpandCollapse");
	if (img)
		img.src = "../layouts/images/scrollup_hover_white.gif";
		
	var bannerTable = document.getElementById("BannerTable");
	if (bannerTable)
		bannerTable.style.display = "";
}