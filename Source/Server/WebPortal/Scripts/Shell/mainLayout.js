var viewport;
var mainWidth;

// Create cards
var cards = new Ext.Panel({title: '',id:'menu_panel',layout:'card',region:'center',border: false,activeItem: 0,bodyStyle: 'padding:0px', defaults: {border:false}});

function initLayout()
{
	// Set cookie provider
    Ext.state.Manager.setProvider(new Ext.state.CookieProvider());
    
    var str = Ext.state.Manager.get("showLeftFrame");
    var hidden = false;
    if(str && str == "0")
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
			height: 85
		}, {
			region: 'west',
			collapsible: true,
			collapsed: hidden,
			collapseMode: 'mini',
			width: 200,
			minSize: 175,
			maxSize: 400,
			split: true,	
			margins: '0 0 0 2',
			layout:'border',
			items:
		    [   
				cards,
		        {
                    region:'south',
                    border: false,
                    split:false,
                    contentEl: 'left_div'
                }
		    ],
			listeners: {
				beforecollapse: function(p) {
					HideLeft();
				},
				beforeexpand: function(p) {
					ShowLeft();
				}
			}
		}, 
		{
			region:'center',
			border: false,
			items:
				new Ext.Panel({
					region:'center',
					items:[
					{
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
	
	var s = Ext.state.Manager.get("showPartTopFrame");
	if(s && s == "0")
		HideTopPart();
	else
		ShowTopPart();
		
	ResizeIFrame();
	
	setTimeout('leftTemplate_Initialize();', 500);
}

function ResizeIFrame()
{
	if(viewport && viewport.layout && viewport.layout.center && viewport.layout.center.panel)
	{
		var obj = document.getElementById("right");
		obj.height = (viewport.layout.center.panel.getInnerHeight() - 2).toString() + "px";
	}
}

function ExpandCollapse()
{
	var s = Ext.state.Manager.get("showPartTopFrame");
	if(s && s == "0")
		ShowTopPart();
	else
		HideTopPart();
}

function HideTopPart()
{
	var fset = viewport.layout.north;
	fset.panel.hide();
	fset.panel.setHeight(25);
	fset.panel.show();
	viewport.doLayout();
	
	var TopPlaceDiv = document.getElementById("TopPlaceDiv");
	if (TopPlaceDiv)
		TopPlaceDiv.style.display = "none";
		
	Ext.state.Manager.set("showPartTopFrame", "0");
}

function ShowTopPart()
{
    var TopPlaceDiv = document.getElementById("TopPlaceDiv");
    if (TopPlaceDiv)
    {
//        var fset = viewport.layout.north;
//        fset.panel.hide();
//        fset.panel.setHeight(85);
//        fset.panel.show();
//        viewport.doLayout();
        
        TopPlaceDiv.style.display = "";
        
        window.setTimeout(function() {
        var _newHeight = TopPlaceDiv.offsetHeight;
        var fset = viewport.layout.north;
        fset.panel.hide();
        fset.panel.setHeight(_newHeight + 25);
        fset.panel.show();
        viewport.doLayout();
        }, 2000);
    }


	
//	if (TopPlaceDiv)
//		TopPlaceDiv.style.display = "";
//		
	Ext.state.Manager.set("showPartTopFrame", "1");
}

function ShowLeft()
{
	Ext.state.Manager.set("showLeftFrame", "1");
}

function HideLeft()
{
	Ext.state.Manager.set("showLeftFrame", "0");
}