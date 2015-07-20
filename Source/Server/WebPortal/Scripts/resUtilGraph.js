var setX, offsetLeft, iHorizCount, iVertCount, iWidth, iHeight, leftX, zoomValue;
var _mousedown=0;
var _constValue = 504;
var _sumOffset = 0;
var _imgArray;
var globalLink, divMenuZoomId, hdnZoomTypeId;
var _headerHeight = 29;

function BindDiv(sName, _link, _zoomValue, _divMenuZoomId, _hdnZoomTypeId, lx, portionWidth, imageHeight)
{
	var objDiv = document.getElementById(sName);
	if(objDiv == null){setTimeout("BindDiv('"+sName+"', '"+_link+"',"+_zoomValue+",'"+_divMenuZoomId+"','"+_hdnZoomTypeId+"')", 300);}
	else
	{
		globalLink = _link;
		if(portionWidth)
			_constValue = portionWidth;
		if (objDiv.addEventListener)
		{
			objDiv.addEventListener("mousedown", function(e) { setFlag(e, objDiv.id); }, false);
			objDiv.addEventListener("mouseup", function(e) { unSetFlag(e, objDiv.id); }, false);
			objDiv.addEventListener("mouseout", function(e) { unSetFlag(e, objDiv.id); }, false);
			objDiv.addEventListener("mousemove", function(e) { _moving(e, objDiv.id, globalLink); }, false);
		}
		else
		{
			objDiv.attachEvent("onmousedown", function(event) { setFlag(event, objDiv.id); });
			objDiv.attachEvent("onmouseup", function(event) { unSetFlag(event, objDiv.id); });
			objDiv.attachEvent("onmouseout", function(event) { unSetFlag(event, objDiv.id); });
			objDiv.attachEvent("onmousemove", function(event) { _moving(event, objDiv.id, globalLink); });
		}  

		_imgArray = [];
		iWidth = objDiv.clientWidth;
		iHeight = objDiv.clientHeight;
		if(iWidth==0 && objDiv.parentNode.clientWidth>0)
			iWidth = objDiv.parentNode.clientWidth;
		iHorizCount = Math.floor(iWidth/_constValue) + 3;
		iVertCount = 1; //Math.floor(iHeight/_constValue) + 1;
		if(lx)
			leftX = lx;
		else
			leftX = -1;
		var objIMGDIV = document.createElement("div");
		objIMGDIV.id = "divCont"
		objIMGDIV.style.backgroundColor="#e5e3df";
		objIMGDIV.style.position = "absolute";
		objIMGDIV.style.top = "0px";
		objIMGDIV.style.zIndex = 0;
		objIMGDIV.style.left = (-_constValue).toString()+"px";
		offsetLeft = (-_constValue);
		for(var i=0; i<iHorizCount; i++)
		{
			_imgArray[i] = [];
			for(var j=-1;j<iVertCount;j++)
			{
				var iLeft = i*_constValue;
				var iTop = _headerHeight + j * _constValue;
				if (j == -1)
					iTop = 0;
				var sTagName = "";
				if(browseris.ie)
					sTagName = "img";
				else
					sTagName = "div";
				var objIMG = document.createElement(sTagName);	
				objIMG.style.position="absolute";
				objIMG.style.left=iLeft.toString()+"px";
				objIMG.style.top=iTop.toString()+"px";
				objIMG.style.width = _constValue.toString() + "px";
				if (j == -1)
					objIMG.style.height = _headerHeight.toString() + "px";
				else
					objIMG.style.height = imageHeight.toString() + "px";  // _constValue.toString()+"px";
				objIMG.style.border="0px none";
				objIMG.style.padding="0px";
				objIMG.style.margin="0px";
				objIMG.style.zIndex = 0;
				objIMG.width = _constValue.toString();
				if (j == -1)
					objIMG.height = _headerHeight.toString();
				else
					objIMG.height = imageHeight.toString();
				objIMG.setAttribute("coordX",(i + leftX));
				objIMG.setAttribute("coordY",j);
				if(browseris.ie)
				{
					objIMG.setAttribute("unselectable","on");
					objIMG.setAttribute("galleryImg","no");
				}
				else
					objIMG.style.MozUserSelect = "none";
				objIMG.setAttribute("ganttChart","yes");
				var d = new Date();
				var sPath = "&x="+(i + leftX)+"&y="+j+"&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
				if(!browseris.ie)
					objIMG.style.backgroundImage = "url('"+ globalLink + sPath + "')";
				objIMG.setAttribute("src", globalLink + sPath);
				objIMGDIV.appendChild(objIMG);
				_imgArray[i][j] = objIMG;
			}
		}
		objDiv.appendChild(objIMGDIV);
		
		if(_zoomValue!='undefined' && _zoomValue>=0)
		{
			zoomValue = _zoomValue;
			var objIMGDIV2 = document.createElement("div");
			objIMGDIV2.id = "divZoom"
			objIMGDIV2.style.position = "absolute";
			objIMGDIV2.style.top = (iHeight - 20).toString() + "px";
			objIMGDIV2.style.left = (iWidth - 20).toString() + "px";
			objIMGDIV2.style.border = "0px none";
			
			var objIMGZoom = document.createElement("img");
			objIMGZoom.id = "divZoomImg"
			objIMGZoom.style.position = "absolute";
			objIMGZoom.style.top = "1px";
			objIMGZoom.style.left = "1px";
			objIMGZoom.style.border = "0px none";
			var s = "../Layouts/Images/zoom-mini.png";
			if(browseris.ie)
				objIMGZoom.style.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"+s+"',sizingMethod=scale)";
			objIMGZoom.setAttribute("src",s);
			objIMGZoom.setAttribute("galleryimg","no");
			if(browseris.ie)
				objIMGZoom.setAttribute("unselectable","on");
			else
				objIMGZoom.style.MozUserSelect = "none";
			objIMGZoom.setAttribute("cleared","0");
			objIMGZoom.style.height = "18px";
			objIMGZoom.style.width = "18px";
			objIMGZoom.style.padding="0px";
			objIMGZoom.style.margin="0px";
			objIMGZoom.style.cursor = "pointer";
			objIMGZoom.style.zIndex = 1;
			objIMGZoom.style.MozUserSelect="none";
			objIMGZoom.onclick = function()
			{
				var objdivMenuZoom = document.getElementById(divMenuZoomId);
				if(objdivMenuZoom)
					objdivMenuZoom.style.display='';
				return false
			};
			objIMGDIV2.appendChild(objIMGZoom);
			
			objDiv.appendChild(objIMGDIV2);
			
			var objdivMenuZoom = document.getElementById(_divMenuZoomId);
			if(objdivMenuZoom)
			{
				off = GetTotalOffset(objIMGZoom);
				objdivMenuZoom.style.display = "";
				objdivMenuZoom.style.left = off.Left+18-objdivMenuZoom.clientWidth + "px";
				objdivMenuZoom.style.top = off.Top+15 + "px";
				objdivMenuZoom.style.display = "none";
				divMenuZoomId = _divMenuZoomId;
			}
			if(_hdnZoomTypeId!='undefined')
				hdnZoomTypeId = _hdnZoomTypeId;
		}
	}
}

var iDivTimerSchedAdd = -1;
function PrepareClosing()
{
	iDivTimerSchedAdd = window.setInterval(closeZoomMenu, 1000);
}

function CancelClosing()
{
	if(iDivTimerSchedAdd>=0)
	{
		window.clearInterval(iDivTimerSchedAdd);
		iDivTimerSchedAdd = -1;
	}
}
	
function closeZoomMenu()
{
	document.getElementById(divMenuZoomId).style.display = "none";
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

function _Zoom(type, koeff)
{
	if(zoomValue==type){
		closeZoomMenu();
		return;
	}
	var hdnZoomType = document.getElementById(hdnZoomTypeId);
	if(hdnZoomType)
		hdnZoomType.value = type.toString();
	var _leftX = Math.round(leftX*koeff);
	for(i=0;i<_imgArray.length;i++)
		for(j=-1;j<_imgArray[i].length;j++)
		{
			var objImg = _imgArray[i][j];
			var oldPath = objImg.attributes["src"].value;
			var _s = oldPath.substring(oldPath.indexOf("&x="),oldPath.indexOf("&y="));
			var _i = parseInt(_s.substr(3));
			objImg.coordX = _leftX + _i - leftX;
			objImg.attributes["coordX"].value = _leftX + _i - leftX;
			oldPath = oldPath.replace(_s, "&x="+(_leftX + _i - leftX).toString());
			oldPath = oldPath.replace("?Type="+zoomValue.toString(),"?Type="+type.toString());
			globalLink = globalLink.replace("?Type="+zoomValue.toString(),"?Type="+type.toString());
			oldPath = oldPath.substr(0, oldPath.indexOf("&idt="));
			var d = new Date();
			var sUniquePart = "&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
			if(!browseris.ie)
				objImg.style.backgroundImage = "url('"+ oldPath + sUniquePart + "')";
			objImg.setAttribute("src", oldPath + sUniquePart);
		}
	leftX = _leftX;
	zoomValue = type;
	closeZoomMenu();
	return false;
}

function _resizing(sName, imageHeight)
{
	var objDiv = document.getElementById(sName);
	iWidth = objDiv.clientWidth;
	if(objDiv.parentNode.clientWidth>0){iWidth = objDiv.parentNode.clientWidth;}
	var iOldHorizCount = iHorizCount;
	iHorizCount = Math.floor(iWidth/_constValue) + 3;
	if(iOldHorizCount-iHorizCount>0){iHorizCount = iOldHorizCount;}
	else if(iOldHorizCount-iHorizCount<0)
	{
		var objDivCont = document.getElementById('divCont');
		for(var i=leftX + iOldHorizCount; i<leftX + iHorizCount; i++)
			for(var j=-1;j<iVertCount;j++)
			{
				var iLeft = i*_constValue + _sumOffset;
				var iTop = _headerHeight + j * _constValue;
				if (j == -1)
					iTop = 0;
				var sTagName = "";
				if(browseris.ie)
					sTagName = "img";
				else
					sTagName = "div";
				var objIMG = document.createElement(sTagName);	
				objIMG.style.position="absolute";
				objIMG.style.left=iLeft.toString()+"px";
				objIMG.style.top=iTop.toString()+"px";
				objIMG.style.width = _constValue.toString() + "px";
				if (j == -1)
					objIMG.style.height = _headerHeight.toString() + "px";
				else
					objIMG.style.height = imageHeight.toString() + "px";
				objIMG.style.border="0px none";
				objIMG.style.padding="0px";
				objIMG.style.margin="0px";
				objIMG.style.zIndex = 0;
				objIMG.width = _constValue.toString();
				if (j == -1)
					objIMG.height = _headerHeight.toString();
				else
					objIMG.height = imageHeight.toString();
				objIMG.setAttribute("coordX",i);
				objIMG.setAttribute("coordY",j);
				if(browseris.ie)
				{
					objIMG.setAttribute("unselectable","on");
					objIMG.setAttribute("galleryImg","no");
				}
				else
					objIMG.style.MozUserSelect = "none";
				objIMG.setAttribute("ganttChart","yes");
				var d = new Date();
				var sPath = "&x="+i+"&y="+j+"&lx="+leftX.toString()+"&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
				if(!browseris.ie)
					objIMG.style.backgroundImage = "url('"+ globalLink + sPath + "')";
				objIMG.setAttribute("src", globalLink + sPath);
				objDivCont.appendChild(objIMG);
			}
	}
	var objdivImgZoom = document.getElementById('divZoom');
	if(objdivImgZoom)
	{
		objdivImgZoom.style.left = (iWidth - 20).toString() + "px";
		var objdivMenuZoom = document.getElementById(divMenuZoomId);
		if(objdivMenuZoom)
		{
			objIMGZoom = document.getElementById('divZoomImg');
			if(objIMGZoom)
			{
				off = GetTotalOffset(objIMGZoom);
				objdivMenuZoom.style.display = "";
				objdivMenuZoom.style.left = off.Left+18-objdivMenuZoom.clientWidth + "px";
				objdivMenuZoom.style.top = off.Top+15 + "px";
				objdivMenuZoom.style.display = "none";
			}
		}
	}
}

function setFlag(e, sName)
{
	if(!e && window.event)
		e = window.event;
	
	_mousedown=1;
	var objDiv = document.getElementById(sName);
	objDiv.style.cursor='w-resize';
	setX=e.clientX;
}

function unSetFlag(e, sName)
{
	if(!e && window.event)
		e = window.event;
	_mousedown=0;
	
	var objDiv = document.getElementById(sName);
	if(objDiv)
	{
		objDiv.style.cursor='pointer';
	}
}

function _moving(e, sName)
{
	if(!e && window.event)
		e = window.event;
	var objDiv = document.getElementById(sName);
	if(_mousedown==1)
	{
		var _curOffset = e.clientX - setX;
		_sumOffset += _curOffset;
		setX = e.clientX;
		var _left, _right, min_value, max_value;
		min_value = 0;
		max_value = 0;
		var fl = false;
		for(i=0;i<_imgArray.length;i++)
			for(j=-1;j<_imgArray[i].length;j++)
			{
				var objImg = _imgArray[i][j];
				_left=_curOffset + parseInt(objImg.style.left.substring(0,objImg.style.left.length-2));
				_right=_left+_constValue;
				objImg.style.left = _left + "px";
				if(_curOffset<0) //to left
				{
					if(_left<=-Math.floor(_constValue/2))
					{
						if(browseris.ie)
							objImg.src= "/layouts/images/BLANK.GIF";
						else
							objImg.style.backgroundImage= "url('../layouts/images/BLANK.GIF')";
						if(!fl)
						{
							leftX = leftX + 1;
							fl = true;
						}
						
						objImg.coordX = leftX + iHorizCount - 1;
						objImg.coordY = objImg.attributes["coordY"].value;
						
						objImg.attributes["coordX"].value = leftX + iHorizCount - 1;
						
						var d = new Date();
						var sPath = "&x="+objImg.coordX+"&y="+objImg.coordY+"&lx="+leftX.toString()+"&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
						if(!browseris.ie)
							objImg.style.backgroundImage = "url('"+ globalLink + sPath + "')";
						objImg.setAttribute("src", globalLink + sPath);
						
						objImg.style.left = (_left+_constValue*iHorizCount).toString() + "px";
					}
				}
				else if(_curOffset>0) //to right
				{
					if(_right>=_constValue*(iHorizCount+1)-Math.floor(_constValue/2))
					{
						if(browseris.ie)
							objImg.src= "/layouts/images/BLANK.GIF";
						else
							objImg.style.backgroundImage= "url('../layouts/images/BLANK.GIF')";
						if(!fl)
						{
							leftX = leftX - 1;
							fl = true;
						}
						objImg.coordX = leftX;
						objImg.coordY = objImg.attributes["coordY"].value;
						
						objImg.attributes["coordX"].value = leftX;
						
						objImg.style.left = (_left-_constValue*iHorizCount).toString() + "px";
						var d = new Date();
						var sPath = "&x="+objImg.coordX+"&y="+objImg.coordY+"&lx="+leftX.toString()+"&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
						if(!browseris.ie)
							objImg.style.backgroundImage = "url('"+ globalLink + sPath + "')";
						objImg.setAttribute("src", globalLink + sPath);
					}
				}
			}
	}
}
