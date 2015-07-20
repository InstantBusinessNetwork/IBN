var setX, offsetLeft, iHorizCount, iVertCount, iWidth, iHeight, leftX;
var _mousedown = 0;
var _constValue = 336;
var _sumOffset = 0;
var _imgArray;
var globalLink;
var _horizHiddAdd = 7;
var _headerHeight = 29;

function BindDiv(sName, sNameHeader, _link, lx, portionWidth) {
	var objDiv = document.getElementById(sName);
	if (objDiv == null) { setTimeout("BindDiv('" + sName + "', '" + _link + "'," + lx + "," + portionWidth + ")", 300); }
	else {
		globalLink = _link;
		if (portionWidth)
			_constValue = portionWidth;
		if (objDiv.addEventListener) {
			objDiv.addEventListener("mousedown", function(e) { setFlag(e, objDiv.id); }, false);
			objDiv.addEventListener("mouseup", function(e) { unSetFlag(e, objDiv.id); }, false);
			objDiv.addEventListener("mouseout", function(e) { unSetFlag(e, objDiv.id); }, false);
			objDiv.addEventListener("mousemove", function(e) { _moving(e); }, false);
		}
		else {
			objDiv.attachEvent("onmousedown", function(event) { setFlag(event, objDiv.id); });
			objDiv.attachEvent("onmouseup", function(event) { unSetFlag(event, objDiv.id); });
			objDiv.attachEvent("onmouseout", function(event) { unSetFlag(event, objDiv.id); });
			objDiv.attachEvent("onmousemove", function(event) { _moving(event); });
		}

		var objHeaderDiv = document.getElementById(sNameHeader);
		if (objHeaderDiv.addEventListener) {
			objHeaderDiv.addEventListener("mousedown", function(e) { setFlag(e, objHeaderDiv.id); }, false);
			objHeaderDiv.addEventListener("mouseup", function(e) { unSetFlag(e, objHeaderDiv.id); }, false);
			objHeaderDiv.addEventListener("mouseout", function(e) { unSetFlag(e, objHeaderDiv.id); }, false);
			objHeaderDiv.addEventListener("mousemove", function(e) { _moving(e); }, false);
		}
		else {
			objHeaderDiv.attachEvent("onmousedown", function(event) { setFlag(event, objHeaderDiv.id); });
			objHeaderDiv.attachEvent("onmouseup", function(event) { unSetFlag(event, objHeaderDiv.id); });
			objHeaderDiv.attachEvent("onmouseout", function(event) { unSetFlag(event, objHeaderDiv.id); });
			objHeaderDiv.attachEvent("onmousemove", function(event) { _moving(event); });
		}

		_imgArray = [];
		iWidth = objDiv.clientWidth;
		iHeight = objDiv.clientHeight;
		if (iWidth == 0 && objDiv.parentNode.clientWidth > 0)
			iWidth = objDiv.parentNode.clientWidth;
		iHorizCount = Math.floor(iWidth / _constValue) + _horizHiddAdd;
		iVertCount = Math.floor(iHeight / _constValue) + 1;
		if (lx)
			leftX = lx;
		else
			leftX = -1;
		var objIMGDIV = document.createElement("div");
		objIMGDIV.id = "divCont"
		objIMGDIV.style.backgroundColor = "#e5e3df";
		objIMGDIV.style.position = "absolute";
		objIMGDIV.style.top = "0px";
		objIMGDIV.style.zIndex = 0;
		objIMGDIV.style.left = (-_constValue).toString() + "px";

		var objIMGHDRDIV = document.createElement("div");
		objIMGHDRDIV.id = "divHdrCont";
		objIMGHDRDIV.style.backgroundColor = "#e5e3df";
		objIMGHDRDIV.style.position = "absolute";
		objIMGHDRDIV.style.top = "0px";
		objIMGHDRDIV.style.zIndex = 0;
		objIMGHDRDIV.style.left = (-_constValue).toString() + "px";
		
		offsetLeft = (-_constValue);
		for (var i = 0; i < iHorizCount; i++) {
			_imgArray[i] = [];
			for (var j = -1; j < iVertCount; j++) {
				var iLeft = i * _constValue;
				var iTop = j * _constValue;
				var sTagName = "";
				if (browseris.ie)
					sTagName = "img";
				else
					sTagName = "div";
				var objIMG = document.createElement(sTagName);
				objIMG.style.position = "absolute";
				objIMG.style.left = iLeft.toString() + "px";
				if(j==-1)
					objIMG.style.top = "0px";
				else
					objIMG.style.top = iTop.toString() + "px";
				objIMG.style.width = _constValue.toString() + "px";
				if(j==-1)
					objIMG.style.height = _headerHeight.toString() + "px";
				else
					objIMG.style.height = _constValue.toString() + "px";
				objIMG.style.border = "0px none";
				objIMG.style.padding = "0px";
				objIMG.style.margin = "0px";
				objIMG.style.zIndex = 0;
				objIMG.width = _constValue.toString();
				if(j==-1)
					objIMG.height = _headerHeight.toString();
				else
					objIMG.height = _constValue.toString();
				objIMG.setAttribute("coordX", (i + leftX));
				objIMG.setAttribute("coordY", j);
				if (browseris.ie) {
					objIMG.setAttribute("unselectable", "on");
					objIMG.setAttribute("galleryImg", "no");
				}
				else
					objIMG.style.MozUserSelect = "none";
				objIMG.setAttribute("ganttChart", "yes");
				var d = new Date();
				var sPath = "&x=" + (i + leftX) + "&y=" + j + "&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
				if (!browseris.ie)
					objIMG.style.backgroundImage = "url('" + globalLink + sPath + "')";
				objIMG.setAttribute("src", globalLink + sPath);
				if(j==-1)
					objIMGHDRDIV.appendChild(objIMG);
				else
					objIMGDIV.appendChild(objIMG);
				_imgArray[i][j] = objIMG;
			}
		}
		objDiv.appendChild(objIMGDIV);
		objHeaderDiv.appendChild(objIMGHDRDIV);
	}
}

function GetTotalOffset(eSrc) {
	this.Top = 0;
	this.Left = 0;
	while (eSrc) {
		this.Top += eSrc.offsetTop;
		this.Left += eSrc.offsetLeft;
		eSrc = eSrc.offsetParent;
	}
	return this;
}

function _resizing(sName) {
	var objDiv = document.getElementById(sName);
	iWidth = objDiv.clientWidth;
	if (objDiv.parentNode.clientWidth > 0) { iWidth = objDiv.parentNode.clientWidth; }
	var iOldHorizCount = iHorizCount;
	iHorizCount = Math.floor(iWidth / _constValue) + _horizHiddAdd;
	if (iOldHorizCount - iHorizCount > 0) { iHorizCount = iOldHorizCount; }
	else if (iOldHorizCount - iHorizCount < 0) {
		var objDivCont = document.getElementById('divCont');
		var objDivHdrCont = document.getElementById('divHdrCont');
		for (var i = leftX + iOldHorizCount; i < leftX + iHorizCount; i++)
			for (var j = -1; j < iVertCount; j++) {
			var iLeft = i * _constValue + _sumOffset;
			var iTop = j * _constValue;
			if (j == -1)
				iTop = 0;
			var sTagName = "";
			if (browseris.ie)
				sTagName = "img";
			else
				sTagName = "div";
			var objIMG = document.createElement(sTagName);
			objIMG.style.position = "absolute";
			objIMG.style.left = iLeft.toString() + "px";
			objIMG.style.top = iTop.toString() + "px";
			objIMG.style.width = _constValue.toString() + "px";
			if (j == -1)
				objIMG.style.height = _headerHeight.toString() + "px";
			else
				objIMG.style.height = _constValue.toString() + "px";
			objIMG.style.border = "0px none";
			objIMG.style.padding = "0px";
			objIMG.style.margin = "0px";
			objIMG.style.zIndex = 0;
			objIMG.width = _constValue.toString();
			if (j == -1)
				objIMG.height = _headerHeight.toString();
			else
				objIMG.height = _constValue.toString();
			objIMG.setAttribute("coordX", i);
			objIMG.setAttribute("coordY", j);
			if (browseris.ie) {
				objIMG.setAttribute("unselectable", "on");
				objIMG.setAttribute("galleryImg", "no");
			}
			else
				objIMG.style.MozUserSelect = "none";
			objIMG.setAttribute("ganttChart", "yes");
			var d = new Date();
			var sPath = "&x=" + i + "&y=" + j + "&lx=" + leftX.toString() + "&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
			if (!browseris.ie)
				objIMG.style.backgroundImage = "url('" + globalLink + sPath + "')";
			objIMG.setAttribute("src", globalLink + sPath);
			if(j==-1)
				objDivHdrCont.appendChild(objIMG);
			else
				objDivCont.appendChild(objIMG);
		}
	}
}

function setFlag(e, sName) {
	if (!e && window.event)
		e = window.event;

	_mousedown = 1;
	var objDiv = document.getElementById(sName);
	objDiv.style.cursor = 'w-resize';
	setX = e.clientX;
}

function unSetFlag(e, sName) {
	if (!e && window.event)
		e = window.event;
	_mousedown = 0;

	var objDiv = document.getElementById(sName);
	if (objDiv) {
		objDiv.style.cursor = 'pointer';
	}
}

function _moving(e) {
	if (!e && window.event)
		e = window.event;
	if (_mousedown == 1) {
		var _curOffset = e.clientX - setX;
		_sumOffset += _curOffset;
		setX = e.clientX;
		var _left, _right, min_value, max_value;
		min_value = 0;
		max_value = 0;
		var fl = false;
		for (i = 0; i < _imgArray.length; i++)
			for (j = -1; j < _imgArray[i].length; j++) {
			var objImg = _imgArray[i][j];
			_left = _curOffset + parseInt(objImg.style.left.substring(0, objImg.style.left.length - 2));
			_right = _left + _constValue;
			objImg.style.left = _left + "px";
			if (_curOffset < 0) //to left
			{
				if (_left <= -Math.floor(_constValue / 2)) {
					if (browseris.ie)
						objImg.src = "../layouts/images/BLANK.GIF";
					else
						objImg.style.backgroundImage = "url('../layouts/images/BLANK.GIF')";
					if (!fl) {
						leftX = leftX + 1;
						fl = true;
					}

					objImg.coordX = leftX + iHorizCount - 1;
					objImg.coordY = objImg.attributes["coordY"].value;

					objImg.attributes["coordX"].value = leftX + iHorizCount - 1;

					var d = new Date();
					var sPath = "&x=" + objImg.coordX + "&y=" + objImg.coordY + "&lx=" + leftX.toString() + "&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
					if (!browseris.ie)
						objImg.style.backgroundImage = "url('" + globalLink + sPath + "')";
					objImg.setAttribute("src", globalLink + sPath);

					objImg.style.left = (_left + _constValue * iHorizCount).toString() + "px";
				}
			}
			else if (_curOffset > 0) //to right
			{
				if (_right >= _constValue * (iHorizCount + 1) - Math.floor(_constValue / 2)) {
					if (browseris.ie)
						objImg.src = "../layouts/images/BLANK.GIF";
					else
						objImg.style.backgroundImage = "url('../layouts/images/BLANK.GIF')";
					if (!fl) {
						leftX = leftX - 1;
						fl = true;
					}
					objImg.coordX = leftX;
					objImg.coordY = objImg.attributes["coordY"].value;

					objImg.attributes["coordX"].value = leftX;

					objImg.style.left = (_left - _constValue * iHorizCount).toString() + "px";
					var d = new Date();
					var sPath = "&x=" + objImg.coordX + "&y=" + objImg.coordY + "&lx=" + leftX.toString() + "&idt=" + d.getHours() + d.getMinutes() + d.getSeconds();
					if (!browseris.ie)
						objImg.style.backgroundImage = "url('" + globalLink + sPath + "')";
					objImg.setAttribute("src", globalLink + sPath);
				}
			}
		}
	}
}
