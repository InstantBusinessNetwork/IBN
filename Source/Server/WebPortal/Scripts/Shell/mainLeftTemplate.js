leftTemplate_Initialize = function()
{
	if (typeof Ext == 'undefined') 
        return;       
       
    var menuView = Ext.getCmp('menu_panel');
    
    // O.R.: no print
    var leftMenu = $get("menu_panel");
    if (leftMenu)
    {
		while (leftMenu.parentElement && leftMenu.parentElement && leftMenu.parentElement.tagName && leftMenu.parentElement.tagName.toLowerCase() != "body")
		{
			leftMenu = leftMenu.parentElement;
		}
		if (leftMenu && leftMenu.tagName && leftMenu.tagName.toLowerCase() == "div")
		{
			if (leftMenu.className)
				leftMenu.className = leftMenu.className + " noprint";
			else
				leftMenu.className = "noprint";
		}
	}
	//
    
    // Save state info
    var selectedIndex = Ext.state.Manager.get("menu-view-selected", 0);
    
    try{
		var buttons = Ext.get('left_div');
		var divItems = buttons.select("div");
		if(selectedIndex >= divItems.getCount())
		{
			Ext.state.Manager.set("menu-view-selected", 0);
			selectedIndex = 0;
		}
		divItems.item(selectedIndex).replaceClass('NavigationNavBarItem', 'NavigationNavBarItemActive');		
    }
    catch(ex){}
    menuView.getLayout().setActiveItem(selectedIndex);
    
    //DV: create left menu resizer
    Sys.Application.add_init(function() {
		$create(Mediachase.JsLeftTemplateResizer, {"leftDiv": $get("left_div"), "imageBaseUrl": globalImageBaseUrl }, null, null, $get(menuView.getEl().id).parentNode);
    });    
}

function leftTemplate_RegisterMenuView(view)
{
	if (typeof Ext == 'undefined') 
        return;       
       
    var menuView = Ext.getCmp('menu_panel');
    if(!menuView)
		return;
	if(!menuView.items)
		menuView.add(view);
    else if(!menuView.items.containsKey(view.id));
        menuView.add(view);
}

leftTemplate_onMouseOver = function(button)
{
    if(leftTemplate_trim(button.className) == 'NavigationNavBarItem')
    {
        button.className='NavigationNavBarItemHover';
    }
    else
    {
    }
};

leftTemplate_onMouseOut = function(button)
{
    if(leftTemplate_trim(button.className) == 'NavigationNavBarItemHover')
    {
        button.className='NavigationNavBarItem';
    }
    else
    {
    }
};   

leftTemplate_onNavMenuSelect = function(button, index)
{
    var buttons = Ext.get('left_div');   
    leftTemplate_Animate(button);
    buttons.select("div").each(function(member) 
		{ 
			if (member.dom.className.indexOf('NavigationNavBarItemActive') > -1)// == 'NavigationNavBarItemActive') 
				member.replaceClass('NavigationNavBarItemActive','NavigationNavBarItem');
			if (member.dom.className.indexOf('NavigationNavBarItemSmallActive') > -1) //== 'NavigationNavBarItemSmallActive')
				member.replaceClass('NavigationNavBarItemSmallActive','NavigationNavBarItemSmall');
		}
    );
	
    button.className = 'NavigationNavBarItemActive';        
    leftTemplate_ChangeActiveTab(index);
};

/*DV: create container for animation (absolute positioned div element)*/
function leftTemplate_Animate(obj, e)
{
//	if (!e)
//		e = this.Ext.EventObject.browserEvent;
//	
//	var newDiv = document.createElement('DIV');
//	newDiv.id = 'leftTemplate_AnimateElem';
//	newDiv.style.position = 'absolute';
//	newDiv.style.height = obj.offsetHeight + 'px';
//	newDiv.style.width = obj.offsetWidth + 'px';
//	//alert(obj.parentNode.parentNode.parentNode.scrollLeft);
//	if (document.all)
//	{
//		newDiv.style.left = obj.offsetLeft + 'px';
//	}
//	else
//	{
//		newDiv.style.left = obj.offsetLeft - obj.parentNode.parentNode.parentNode.scrollLeft + 'px';
//	}
//	//newDiv.style.top = e.clientY + 'px';
//	
//	if (Sys.Browser.name == 'Firefox' && Sys.Browser.version < 3)
//		newDiv.style.top = e.screenY + 'px';
//	else
//		newDiv.style.top = e.clientY + 'px';
//		
//	newDiv.style.zIndex = '10000';
//	newDiv.appendChild(obj.cloneNode(true));
//	
//	var parentElem = document.getElementById('left_div');
//	var curHeight = parseInt(parentElem.offsetHeight);
//	while (parentElem.parentNode != null)
//	{
//		if (parseInt(parentElem.offsetHeight) != curHeight)
//			break;
//		parentElem = parentElem.parentNode;
//	}
//	
//	document.body.appendChild(newDiv);
//	//parentElem.appendChild(newDiv);
//	window.setTimeout(function() { leftTemplate_AnimateTimer(newDiv, parentElem);}, 50);
}


function leftTemplate_AnimateTimer(obj, parentElem)
{	
//	var attributes = { 
//	        top: { to: 30 },
//	        opacity: { to: 0.5 }
//	    }; 
//	    
//	var anim = new YAHOO.util.Anim('leftTemplate_AnimateElem', attributes); 
//	
//	anim.duration = 0.45;
//	
//	var leftTemplate_removeElement = function ()
//	{
//		//parentElem.removeChild(obj);	
//		document.body.removeChild(obj);
//	};
//	
//	anim.onComplete.subscribe(leftTemplate_removeElement); 
//	anim.animate();
	
}

function leftTemplate_ChangeActiveTab(index)
{
   if (typeof Ext == 'undefined') 
        return;       
 
    var menuView = Ext.getCmp('menu_panel');
    
    menuView.getLayout().setActiveItem(index);
    
    // Save state info
    Ext.state.Manager.set("menu-view-selected", index);
}  

leftTemplate_trim = function(val)
{
    return val.replace(/^\s+|\s+$/g, '');
};

function leftTemplate_AddMenuTab(panelId, panelTitle, dataUrlString)
{
	if (typeof Ext == 'undefined') 
	{
		setTimeout('leftTemplate_AddMenuTab(' + panelId +', '+panelTitle+', '+dataUrlString+')', 300);
        return;
	}
	//Ext.onReady(function(){
		var tree = new Ext.tree.TreePanel({
			id: panelId,
			autoScroll:true,
			animate:true,
			rootVisible:false,
			enableDD:false,
			title: '<div class="text">' + panelTitle + '</div>',
			containerScroll: true, 
			loader: new Ext.tree.TreeLoader({
				dataUrl: dataUrlString
			})
		});
	    tree.defaultLoaderUrl = dataUrlString;
	    
		leftTemplate_RegisterMenuView(tree);
	    
		tree.loader.on("beforeload", function(treeLoader, node) {
			treeLoader.baseParams = node.attributes;
			if(node.attributes.treeLoader)
			{
				treeLoader.dataUrl = node.attributes.treeLoader;
			}
			else
			{
				treeLoader.dataUrl = node.ownerTree.defaultLoaderUrl;
			}
		}, this);
		    
		// set the root node
		var root = new Ext.tree.AsyncTreeNode({
			id: 'leftTemplate_tree_rootId',
			draggable: false,
			expanded: true,
			type: 'root'
		});
		tree.setRootNode(root);    		
	//});
}