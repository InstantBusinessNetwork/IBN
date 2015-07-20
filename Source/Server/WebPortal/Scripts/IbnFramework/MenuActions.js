function menuItems_OnClickHandler(item)
{
	if (item.disabledClass && item.disabledClass.split('||').length == 2)
	{
		raiseScriptCommandHandler(item.disabledClass.split('||')[0], item.disabledClass.split('||')[1]);
	}	
}

function menuOnMouseOut(obj)
{
	if (obj)
	{
		if (document.all)
		{
			obj.parentNode.style.backgroundColor = obj.parentNode.getAttribute('currentBackground');
		}
		else
		{
			obj.parentNode.style.backgroundColor = "Transparent";
		}
		getLastDomElementChild(obj).style.display = 'none';
	}
}

function menuOnMouseMove(obj)
{
	if (obj)
	{
		if (document.all)
		{
			if (!obj.parentNode.getAttribute('currentBackground'))
			{
				obj.parentNode.setAttribute('currentBackground', obj.parentNode.currentStyle.backgroundColor);
			}
			obj.parentNode.style.backgroundColor = "#F7E8D4";
		}
		else
		{
			obj.parentNode.style.backgroundColor = "#F7E8D4";
		}
		getLastDomElementChild(obj).style.display = 'inline';
	}
}

function getLastDomElementChild(obj)
{
	var retVal = null;
	if (obj)
	{
		retVal = obj.lastChild;
		if (retVal)
		{
			while (retVal.previousSibling != null && retVal.nodeType != 1)
			{
				retVal = retVal.previousSibling;
			}
		}
		
		return retVal;
	}
	
	return null;
}