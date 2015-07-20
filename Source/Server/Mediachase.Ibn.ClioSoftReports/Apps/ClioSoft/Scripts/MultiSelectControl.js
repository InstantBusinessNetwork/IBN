// ---------------- Common functions -------------
function msc_GetTotalOffset(eSrc)
{
	this.Top = 0;
	this.Left = 0;
	while (eSrc)
	{
		this.Top += eSrc.offsetTop;
		this.Left += eSrc.offsetLeft;
		eSrc = eSrc.offsetParent;
	}
	return this;
}

function msc_CancelBubble(e)
{
	e = (e) ? e : ((event) ? event : null);
	if (e)
	{
		e.cancelBubble = true;
		if (e.stopPropagation)
			e.stopPropagation();
	}
}

//--------- msc_Object specific -----------------
var msc_Collection = new Object();
var msc_isNewHandlerSet = false;
var msc_savedHandler = null;
var msc_savedItems = new Array();
var msc_skipObjectId = "SaveButton";

function msc_Object(id, mainTableId, dropDownDivId, allItemsCheckBoxId)
{
	this.id = id;
	this.mainTable = document.getElementById(mainTableId);
	this.mainDiv = document.getElementById(dropDownDivId);
	this.mainCheckBox = document.getElementById(allItemsCheckBoxId);
}

function msc_IsSkipObject(e)
{
	var retval = false;
	e = (e) ? e : ((event) ? event : null);
	if (e)
	{
		var src = (e.target) ? e.target : ((e.srcElement) ? e.srcElement : null);
		if (src && src.id && src.id.indexOf(msc_skipObjectId) > 0)
			retval = true;
	}
	return retval;
}

// closes all open dropdowns and resets the values
function msc_CloseAll(e)
{
	if (msc_IsSkipObject(e))
		return;
			
	for (var key in msc_Collection)
	{
		var obj = msc_Collection[key];
		if (obj.mainDiv.style.display == "block")
		{
			obj.mainDiv.style.display = "none";
			
			var boxes = obj.mainDiv.getElementsByTagName('input');
			if(boxes != null && boxes.length > 0)
			{
				for(i = 0; i < boxes.length; i++)
				{
					if(boxes[i].type == "checkbox")
						boxes[i].checked = msc_savedItems[i];
				}
			}
			
			break;
		}
	}
	
	if (msc_isNewHandlerSet)
	{
		document.onclick = msc_savedHandler;
		msc_savedHandler = null;
		msc_isNewHandlerSet = false;
	}
}

function msc_OffAction(e)
{
	msc_CancelBubble(e);
	msc_CloseAll(e);
}

msc_Object.prototype.msc_OpenDropDown = function (topOffset, e)
{
	msc_CancelBubble(e);
	msc_CloseAll(e);
	
	// Width, position
	this.mainDiv.firstChild.width = this.mainTable.offsetWidth;
	
	var off = msc_GetTotalOffset(this.mainTable);
	this.mainDiv.style.left = off.Left + "px";
	this.mainDiv.style.top = (off.Top + topOffset).toString() + "px";

	this.mainDiv.style.display = "block";
	
	// Save click handler
	if (!msc_isNewHandlerSet)
	{
		msc_savedHandler = document.onclick;
		document.onclick = msc_OffAction;
		msc_isNewHandlerSet = true;
	}

	// Save current state
	msc_savedItems = new Array();
	var boxes = this.mainDiv.getElementsByTagName('input');
	if(boxes != null && boxes.length > 0)
	{
		for(i = 0; i < boxes.length; i++)
		{
			if(boxes[i].type == "checkbox")
				msc_savedItems[i] = boxes[i].checked;
		}
	}
	
}

msc_Object.prototype.msc_ShowHideDropDown = function(e) 
{
	if (this.mainDiv.style.display == "none")
		this.msc_OpenDropDown(21, e);
	else
		msc_CloseAll(e);
}

// Checks/Unchecks all checkboxes within the same parent table
msc_Object.prototype.msc_CheckAll = function(e)
{
	var boxes = this.mainDiv.getElementsByTagName('input');
	if(boxes != null && boxes.length > 0)
	{
		var value = this.mainCheckBox.checked;
		for(i = 0; i < boxes.length; i++)
		{
			if(boxes[i].type == "checkbox" && boxes[i].id != this.mainCheckBox.id)
				boxes[i].checked = value;
		}
	}
}

// Unchecks checkbox with given 'objToUncheckId' if 'obj' checkbox is unchecked
msc_Object.prototype.msc_UncheckAllIfNeed = function(obj, e)
{
	if (!obj.checked)
	{
		this.mainCheckBox.checked = false;
	}
}

// we'll get a sript error if will try to make CancelBubble + postback
// so we check the srcElement
function msc_SafeCancelBubble(e)
{
	if (msc_IsSkipObject(e))
		return;
		
	msc_CancelBubble(e);
}