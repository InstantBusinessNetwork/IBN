var CP_SourceControlId = "";
var CP_SourceNewControlUID = "";
var CP_SourceIndex = -1;

function ShowRelDivBMenu(PCntrl, Index, ActiveElId, DivID, nearObjID, _divOffsetLeft, _divWidth)
{
	var oCurDiv = document.getElementById(DivID);
	if (!oCurDiv) return;
	var oActiveElId = document.getElementById(ActiveElId);
	if (!oActiveElId) return;
	for (var i = 0; i < CP_dvMainBMIds.length; i++)
	{
		el = document.getElementById(CP_dvMainBMIds[i]);
		if(el)
		{
			if (el.style.display != "none")
			{
				if(el == oCurDiv && oActiveElId.value == Index)
					continue;
				el.style.display = "none";
			}
		}
	}
	if (oCurDiv.style.display == "none")
	{
		var obj_a = document.getElementById(nearObjID);
		if(obj_a)
		{
			off = GetTotalOffset(obj_a);
			if(off.Left + _divOffsetLeft + _divWidth + 15>document.body.clientWidth)
				oCurDiv.style.left = (document.body.clientWidth - _divWidth - 15).toString() + "px";
			else
				oCurDiv.style.left = (off.Left + _divOffsetLeft).toString() + "px";
			oCurDiv.style.top = (off.Top + 0).toString() + "px"; //18;
		}
		//var _ActiveControlObj = document.getElementById(ActiveControl);
		//_ActiveControlObj.value = PCntrl;
		oActiveElId.value = Index;
		//CP_SourceControlId = PCntrl;
		oCurDiv.style.display = "block";
	}
}

var iDivTimerID = -1;
var mcMenuDivId = "";
function PrepareClosing(_divId)
{
	mcMenuDivId = _divId;
	iDivTimerID = window.setInterval(_closeDiv, 100);
}

function CancelClosing()
{
	if(iDivTimerID>=0)
	{
		window.clearInterval(iDivTimerID);
		iDivTimerID = -1;
	}
}

function _closeDiv()
{
	if(iDivTimerID>=0)
	{
		window.clearInterval(iDivTimerID);	
		iDivTimerID = -1;
		var oCurDiv = document.getElementById(mcMenuDivId);
		mcMenuDivId = "";
		if (!oCurDiv)	return;
		oCurDiv.style.display = "none";
	}
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

function CP_ImgClick(SourceControl,SourceNewControl,SourceElement,ImgSubmit,Index)
{
	document.getElementById(ImgSubmit).value = Index;
	document.getElementById(SourceControl).value = CP_SourceControlId;
	document.getElementById(SourceNewControl).value = CP_SourceNewControlUID;
	document.getElementById(SourceElement).value = CP_SourceIndex;
}

function CP_Add(ddControlsId)
{
	var ddControlsObj = document.getElementById(ddControlsId);
	var UID = "";
	UID = ddControlsObj.options[ddControlsObj.selectedIndex].value;
	CP_Move('',UID,'');
}
function CP_Move(txtActiveElement,sId,sClientId,DivID)
{
	var cur_i = -1;
	if(txtActiveElement != "")
		cur_i = document.getElementById(txtActiveElement).value;
	if(sClientId != "")
	{
		CP_SourceNewControlUID = "";
		CP_SourceControlId = sId;
	}
	else
	{
		CP_SourceNewControlUID = sId;
		CP_SourceControlId = "";
	}
	CP_SourceIndex = cur_i;

	var imgColl = document.getElementsByTagName("img");
	for(j=0;j<imgColl.length;j++)
	{
		obj_temp = imgColl[j];
		obj_temp_attr = null;
		obj_temp_attr = obj_temp.CP_ClientID;
		if(obj_temp_attr == null)
		{
			if( obj_temp.getAttribute("CP_ClientID") )
				obj_temp_attr = obj_temp.getAttribute("CP_ClientID");
		}
		if(obj_temp_attr)
		{
			//if(obj_temp_attr != sClientId) continue;
			obj_temp.style.display = "none";
			
			obj_temp_attr_i = null;
			obj_temp_attr_i = obj_temp.CP_Index;
			if(obj_temp_attr_i == null)
			{
				if( obj_temp.getAttribute("CP_Index") )
					obj_temp_attr_i = obj_temp.getAttribute("CP_Index");
			}
			if(obj_temp_attr == sClientId && obj_temp_attr_i == cur_i) continue;
		}
		else continue;
		obj_temp.style.display = "block";
		PrepareClosing(DivID);
	}
}
function CP_Properties(sPage,sPageId,sId,txtActiveElement)
{
	var cur_i = document.getElementById(txtActiveElement).value;
	OpenPopUpWindow(sPage + "?PageId=" + sPageId + "&ControlId=" + sId + "&Index=" + cur_i);
}
function CP_Hide(txtHideSubmit,txtActiveElement)
{
	document.getElementById(txtHideSubmit).value = document.getElementById(txtActiveElement).value;
}