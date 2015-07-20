var MC_ODD = new Object();
var isNewHandlerSet_objectDD = false;
var savedHandler_objectDD = null;

function MCObjectDD(id, tblMainId, divDDId, tdChId, hidSelType, hidSelId, lbSelected, OnChange)
{
  this.id = id;
  this.mainTable = document.getElementById(tblMainId);
  this.mainDiv = document.getElementById(divDDId);
  this.changeTD = document.getElementById(tdChId);
  this.hidSelType = document.getElementById(hidSelType);
  this.hidSelId = document.getElementById(hidSelId);
  this.lbSelected = document.getElementById(lbSelected);
  this.OnChange = OnChange;
}

MCObjectDD.prototype.getObjectTypeId = function() {
  return this.hidSelType.value;
}

MCObjectDD.prototype.getObjectId = function() {
  return this.hidSelId.value;
}

MCObjectDD.prototype.openObjectDD = function(e) {
  CancelBubble_objectDD(e);
 
  closeAll();
   
  this.mainDiv.firstChild.width = this.mainTable.offsetWidth;
	
	var off = GetTotalOffset(this.changeTD);
	
	this.mainDiv.style.left = off.Left - (this.mainTable.offsetWidth - this.changeTD.offsetWidth).toString() + "px";
	this.mainDiv.style.top = (off.Top+22).toString() + "px";
	
	this.mainDiv.style.display = "";
	
	if(typeof(hideSelects)!="undefined")
	  hideSelects(this.mainDiv);
	if (!isNewHandlerSet_objectDD)
	{
		savedHandler_objectDD = document.onclick;
		//document.onclick = offAction_objectDD;
		isNewHandlerSet_objectDD = true;
	}
};

MCObjectDD.prototype.ShowHideObjectDD = function(e) {
  
  if (this.mainDiv.style.display == "none")
		this.openObjectDD(e);
	else
		closeAll();
};

MCObjectDD.prototype.SelectThis = function(obj, _typeId, _Id) {
  this.lbSelected.innerHTML = obj.innerHTML;
	this.hidSelType.value = _typeId;
	this.hidSelId.value = _Id;
	closeAll();
	if(this.OnChange!="")
	  eval(this.OnChange);
};

MCObjectDD.prototype.SelectThisHTML = function(html, _typeId, _Id) {
  this.lbSelected.innerHTML = html;
	this.hidSelType.value = _typeId;
	this.hidSelId.value = _Id;
	if(this.OnChange!="")
	  eval(this.OnChange);
};

//************************************************************************************
function closeAll()
{
  var div_coll = document.getElementsByTagName("div");
  for(var i=0; i<div_coll.length; i++)
  {
    if(div_coll[i].id && div_coll[i].id.indexOf("divDropDown")>=0 && div_coll[i].style.display=="")
      div_coll[i].style.display = "none";
  }
  
  if(typeof(showSelects)!="undefined")
	  showSelects();
	  
  if (isNewHandlerSet_objectDD)
  {
    document.onclick = savedHandler_objectDD;
    savedHandler_objectDD = null;
    isNewHandlerSet_objectDD = false;
  }
}

function offAction_objectDD(e)
{
 CancelBubble_objectDD(e);
 
 closeAll();
}

function TdOver(obj)
{
  if(obj)
  {
    if(obj.className == "cellclass")
      obj.className = "hovercellclass";
  }
}
function TdOut(obj)
{
 if(obj)
  {
    if(obj.className == "hovercellclass")
      obj.className = "cellclass";
  }
}

function OpenMorePop(sLink,w,h)
{
	if (w == null)
		w = 640;
	if (h == null)
		h = 480;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'scrollbars=0, resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	var f = window.open(sLink, "_blank", winprops);
}

function GetTotalOffset(eSrc)
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

function CancelBubble_objectDD(e)
{
 e = (e) ? e : ((event) ? event : null);
 if (e)
 {
  e.cancelBubble = true;
  if(e.stopPropagation)
   e.stopPropagation();
 }
}