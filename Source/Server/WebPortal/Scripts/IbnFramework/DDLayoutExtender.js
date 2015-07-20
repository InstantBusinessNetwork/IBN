
Type.registerNamespace("Ibn");

var layoutExtender_tools = null;
var layoutExtender_tools2 = null;
var onDeleteDelegate = null;
var onPropertyPage = null;
var onCollapseDelegate = null;

var __curentInstanse = null;

Ibn.DDLayoutExtender = function(element) 
{
    Ibn.DDLayoutExtender.initializeBase(this, [element]);
    
    var jsonItems = null;
    
    //popup div
    var popupElement = null;
    var popupId = null;
        
    var propertyCommand = null;
    
    var addElementContainer = null;
    var addIds = null;
    var wsPageUid = null;
    var contextKey = null;
    
    //end PageRequest handler delegate
    var beginPageRequestHandler = null;
    var endPageRequestHandler = null;
    
    var deleteMsg = null;
    
    // onDropEvent list
    var onDropEventList = null;
    
  
    
    var originalContainer = null;
    
    var viewport = null;
    
    //if true, debug alert will be shown
    var __debug = null;
}

Ibn.DDLayoutExtender.prototype =
{
	get_jsonItems: function ()
	{
		return this.jsonItems;
	},
	set_jsonItems: function (value)
	{
		this.jsonItems = value;
	},	
	get_deleteMsg: function ()
	{
		return this.deleteMsg;
	},
	set_deleteMsg: function (value)
	{
		this.deleteMsg = value;
	},		
	
	get_addElementContainer: function ()
	{
		return this.addElementContainer;
	},
	set_addElementContainer: function (value)
	{
		this.addElementContainer = value;
	},
	
	get_wsPageUid: function ()
	{
		return this.wsPageUid;
	},
	set_wsPageUid: function (value)
	{
		this.wsPageUid = value;
	},
	
	get_propertyCommand: function ()
	{
		return this.propertyCommand;
	},
	set_propertyCommand: function (value)
	{
		this.propertyCommand = value;
	},
	
	get_popupElement: function ()
	{
		return this.popupElement;
	},
	set_popupElement: function (value)
	{
		this.popupElement = value;
	},	
		
	get_contextKey: function ()
	{
		return this.contextKey;
	},
	set_contextKey: function (value)
	{
		this.contextKey = value;
	},			
	
		
	// ctor()
	initialize : function() 
	{
		Ibn.DDLayoutExtender.callBaseMethod(this, 'initialize');
		this.__debug = false;		
		
		onDeleteDelegate = Function.createDelegate(this, this.onClose);
		onPropertyPage = Function.createDelegate(this, this.onPropertyClick);
		onCollapseDelegate = Function.createDelegate(this, this.onCollapse);
		
		this.onDropEventList = new Sys.EventHandlerList();
		
		//attach to endPageRequest
		this.endPageRequestHandler = Function.createDelegate(this, this.onEndPageRequest);
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(this.endPageRequestHandler);
		
		this.beginPageRequestHandler = Function.createDelegate(this, this.onBeginPageRequest);
		Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(this.beginPageRequestHandler);
		
		
		layoutExtender_tools = 
				[
				{
					id:'toggle',
					handler: function(e, target, panel) { onCollapseDelegate(e, target, panel); }
				},				
				{
					id:'close',
					handler: function(e, target, panel){ onDeleteDelegate(e, target, panel);} //function(e, target, panel){panel.ownerCt.remove(panel, true);}	
				}
				];
		layoutExtender_tools2 =
				[
				{
					id:'toggle',
					handler: function(e, target, panel) { onCollapseDelegate(e, target, panel); }
				},
				{
					id:'gear',
					handler: function(e, target, panel) { onPropertyPage(e, target, panel);}
				},
				{
					id:'close',
					handler: function(e, target, panel){ onDeleteDelegate(e, target, panel);} //function(e, target, panel){panel.ownerCt.remove(panel, true);}	
				}
				]			
				
		this.initLayout();
		__curentInstanse = this;
	},
	dispose: function()
	{
		this.jsonItems = null;
		if (this.endPageRequestHandler && Sys.WebForms.PageRequestManager.getInstance())
			Sys.WebForms.PageRequestManager.getInstance().remove_endRequest(this.endPageRequestHandler)
			
		Ibn.DDLayoutExtender.callBaseMethod(this, 'dispose');
	},
	
	initLayout: function()
	{
		this.jsonItems = "[" + this.jsonItems + "]";
		var _itemsArray = Sys.Serialization.JavaScriptSerializer.deserialize(this.jsonItems);
		this.viewport = new Ext.Viewport({			
			id: 'mainViewport',		
			layout: 'border',
			contentEl: 'containerLayout', 
			items: 
			[
				{
					id: 'mainPortal',
					xtype: 'portal',
					region: 'center',
					items: _itemsArray
				}
			]
        });
        
        var container = Ext.get('containerLayout');
        if (container)
        {
			this.viewport.renderTo = container;
			container.dom.appendChild(this.viewport.findById("mainPortal").el.dom);
        }
        
        this.viewport.findById("mainPortal").on("drop", Function.createDelegate(this, this.onDrop));
        this.viewport.findById("mainPortal").on("beforedragover", Function.createDelegate(this, this.onBeforeDragOver));
        
        if (this.popupElement && Sys.Browser.name == 'Firefox' && Sys.Browser.version == 2)
			this.viewport.findById("mainPortal").el.dom.appendChild(this.popupElement);

        if (this.addElementContainer && $get(this.addElementContainer))
        {
			this.addIds = new Array();
			for (var i = 0; i < _itemsArray.length; i++)
			{
				var tmpNode = $get(this.addElementContainer).cloneNode(true);
				
				tmpNode.style.display = 'block';
				tmpNode.id = this.addElementContainer + "_" + i;
				
				if (tmpNode.getAttribute('onclick_toExecute'))
				{
					var clickHandlerScript = tmpNode.getAttribute('onclick_toExecute');
					clickHandlerScript = clickHandlerScript.replace('%columnId%', _itemsArray[i].id);
					clickHandlerScript = clickHandlerScript.replace('%pageUid%', this.wsPageUid);
					tmpNode.setAttribute('onclick_toExecute', clickHandlerScript);
					$addHandler(tmpNode, "click", function() { eval(this.getAttribute('onclick_toExecute'));  } );
				}			
				
				document.getElementById(_itemsArray[i].id).appendChild(tmpNode);				
				this.addIds.push(tmpNode.id);
			}
        }
        
        //$get('aspnetForm').appendChild(this.viewport.findById("mainPortal").el.dom);
        
	},

	onClose: function(e, target, panel)
	{
		//delete block 
		//1) if this.deleteMsg is defnied and user confirmed delete operation, or 
		//2) if this.deleteMsg is not defined
		if (this.deleteMsg != '' && confirm(this.deleteMsg) || (this.deleteMsg == '' || this.deleteMsg == null))
		{
			var _portal = panel.ownerCt.ownerCt;
			panel.ownerCt.remove(panel, true);
			Mediachase.Ibn.Web.UI.WebServices.LayoutHandler.Delete(this.getCurrentJson(_portal), this.contextKey, null, Function.createDelegate(this, this.onRequestFailed));
		}
	},

	onPropertyClick: function (e, target, panel)
	{
		if (this.propertyCommand)
		{
			var scriptToExecute = this.propertyCommand.replace('%controlUid%', panel.id);
			eval(scriptToExecute);
		}
	},
	
	onCollapse: function (e, target, panel)
	{
		if(panel.collapsed) 
		{ 
			panel.expand();
			panel.collapsed = false;
			this.onExpandEvent(panel);
		} 
		else 
		{ 
			panel.collapse(); 
			panel.collapsed = true;
		}
		
		var _portal = panel.ownerCt.ownerCt;		
		Mediachase.Ibn.Web.UI.WebServices.LayoutHandler.OrderChange(this.getCurrentJson(_portal), this.contextKey, null, Function.createDelegate(this, this.onRequestFailed));
	},

	onDrop: function (source)
	{
		Mediachase.Ibn.Web.UI.WebServices.LayoutHandler.OrderChange(this.getCurrentJson(source.portal), this.contextKey, null, Function.createDelegate(this, this.onRequestFailed));

		//TO DO: show add buttons
		for (var i = 0; i < this.addIds.length; i++)
		{
			var _obj = $get(this.addIds[i]);
			if (_obj)
				_obj.style.display = 'block';
		}				
		//raise Event
		//var _handler = Function.createDelegate(this, this.onDropEvent);
		//_handler(source.source.el.dom)
		window.setTimeout(Function.createDelegate(this, this.onDropEvent), 250);
	},
	
	onBeforeDragOver: function(source)
	{
		//TO DO: hide add buttons
		for (var i = 0; i < this.addIds.length; i++)
		{
			var _obj = $get(this.addIds[i]);
			if (_obj)
				_obj.style.display = 'none';
		}
	},
	getCurrentJson: function(portal)
	{
		var retVal = '';
		var storeObj = new Array();

		for (var i = 0; i < portal.items.length; i++)
		{
			storeObj[storeObj.length] = {};
			storeObj[storeObj.length - 1].items = new Array();
		
			var obj = portal.items.get(i);
			storeObj[storeObj.length - 1].id = obj.id;
			
			for (var j = 0; j < obj.items.length; j++)
			{
				var newItem = {};				
				newItem.id = obj.items.get(j).id.split('_')[0];
				newItem.collapsed = obj.items.get(j).collapsed;
				
				if (obj.items.get(j).id.split('_').length > 1)
					newItem.instanseUid = obj.items.get(j).id.split('_')[1];
					
				storeObj[storeObj.length - 1].items[storeObj[storeObj.length - 1].items.length] = newItem;
			}					
		}
		retVal = Sys.Serialization.JavaScriptSerializer.serialize(storeObj);
		return retVal;	
	},
	
	onBeginPageRequest: function(obj, args)
	{
//		var _itemsArray = Sys.Serialization.JavaScriptSerializer.deserialize(this.jsonItems);
//		for  (var i = 0; i < _itemsArray.length; i++)
//		{
//			var obj = _itemsArray[i].id;
//			for (var j = 0; j < _itemsArray[i].items.length; j++)
//			{
//				var newid = _itemsArray[i].items[j].contentEl.substr(0, _itemsArray[i].items[j].contentEl.indexOf(obj) + obj.length);
//				_itemsArray[i].items[j].parentId = $get(_itemsArray[i].items[j].contentEl).parentNode.id;				
//				document.getElementById(newid).appendChild($get(_itemsArray[i].items[j].contentEl));
//			}
//		}	
//		
//		this.jsonItems = Sys.Serialization.JavaScriptSerializer.serialize(_itemsArray);
//		alert('new serialization: ' + this.jsonItems);
	},
	
	onEndPageRequest: function(obj, args)
	{		
		if (obj._postBackSettings && obj._postBackSettings.sourceElement)
		{
		
//			var _itemsArray = Sys.Serialization.JavaScriptSerializer.deserialize(this.jsonItems);
//			for  (var i = 0; i < _itemsArray.length; i++)
//			{
//				var obj = _itemsArray[i].id;
//				for (var j = 0; j < _itemsArray[i].items.length; j++)
//				{
//					var newid = _itemsArray[i].items[j].contentEl.substr(0, _itemsArray[i].items[j].contentEl.indexOf(obj) + obj.length);
//					//alert(newid + ' | ' +_itemsArray[i].items[j].parentId);
//					//_itemsArray[i].items[j].parentId = $get(_itemsArray[i].items[j].contentEl).parentNode.id;
//					document.getElementById(_itemsArray[i].items[j].parentId).appendChild($get(_itemsArray[i].items[j].contentEl));
//				}
//			}
			
		}
	},	
	
    // ----- onDrop Event ------
    onExpandEvent: function(blockNode)
    {
       var handler = this.onDropEventList.getHandler("mc_ddLayout_onexpand");
       if (handler) 
       {		   
           handler(this, blockNode);
       }   
    },
    add_expand: function(handler)
    {
        if (this.onDropEventList)
		    this.onDropEventList.addHandler("mc_ddLayout_onexpand", handler);
    },
    remove_expand: function(handler)
    {
        if (this.onDropEventList != null)
    	    this.onDropEventList.removeHandler("mc_ddLayout_onexpand", handler);
    }, 		
	
    // ----- onDrop Event ------
    onDropEvent: function(blockNode)
    {
       var handler = this.onDropEventList.getHandler("mc_ddLayout_ondrop");
       if (handler) 
       {		   
           handler(this, blockNode);
       }   
    },
    add_drop: function(handler)
    {
        if (this.onDropEventList)
		    this.onDropEventList.addHandler("mc_ddLayout_ondrop", handler);
    },
    remove_drop: function(handler)
    {
        if (this.onDropEventList != null)
    	    this.onDropEventList.removeHandler("mc_ddLayout_ondrop", handler);
    }, 	
	
	onRequestFailed: function(error)
	{
		//TO DO:		
	},
	
	debugMsg: function(str)
	{
		if (this.__debug)
			alert(str);
	}
}

function workspaceGetRefresh(params)
{
	//window.location.href = window.location.href;
	window.location.reload(true);
}

function workspaceGetRefreshLight(params)
{
	if (window.location.href.indexOf('testLocation') == -1)
	{
		if (window.location.href.indexOf('?') == -1)
			window.location.href = window.location.href + '?testLocation=0';
		else
			window.location.href = window.location.href + '&testLocation=0';
	}
	else
	{	
		var newPath = window.location.href.replace('&testLocation=0', '');
		newPath = window.location.href.replace('?testLocation=0', '');
		window.location.href = newPath;
	}
	//window.location.href = window.location.href;
	//window.location.reload(true);
}

Ibn.DDLayoutExtender.getCurrent = function()
{
	return __curentInstanse;
}

Ibn.DDLayoutExtender.registerClass("Ibn.DDLayoutExtender", Sys.UI.Control);
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();