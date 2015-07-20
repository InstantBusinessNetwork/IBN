var trUploadFrame;
var divUploadButton;
var progressBar;
var divProgress
var divAddAttach;
var percValue;
var divWait;
var divAttachs;

function ResizeForm()
{
    var obj = document.getElementById('divHTML');
	var objTbl = document.getElementById('topTable');
	var _fck1 = document.getElementById('fckEditor');
	var _fck2 = document.getElementById('fckEditor_previewPane');
	var _fck3 = document.getElementById('fckEditor_designEditor');
	if(obj && objTbl)
	{
		if(document.body.clientHeight - objTbl.offsetHeight - 11>0)
		{
			var iheight = document.body.clientHeight - objTbl.offsetHeight - 11;
			var iwidth = document.body.clientWidth;
			obj.style.width = iwidth + "px";
			obj.style.height = iheight + "px";
			var _fck_obj = document.getElementById('fckEditor_TabRow');
			if(_fck_obj && _fck1 && _fck2 && _fck3)
			{
				if(iheight - _fck_obj.offsetHeight - 52 > 0)
				{
					_fck1.style.height = iheight - _fck_obj.offsetHeight - 52 + "px";
					_fck2.style.height = iheight - _fck_obj.offsetHeight - 52 + "px";
					_fck3.style.height = iheight - _fck_obj.offsetHeight - 52 + "px";
				}
			}
		}
	}
	trUploadFrame = document.getElementById("trUploadFrame");
	if(trUploadFrame && trUploadFrame.style.display != "none")
	{
		divUploadButton = document.getElementById("divUploadButton");
		divUploadButton.style.left = trUploadFrame.offsetWidth - 100 + "px";
		divUploadButton.style.top = trUploadFrame.offsetTop + 12 + "px";
	}
}

function ResizeAttachForm()
{
	var obj = document.getElementById('tableDiv');
	var objTbl = document.getElementById('pathTr');
	if(obj && objTbl)
	{
		var intHeight = 0;
		var intWidth = 0;
		if (typeof(window.innerWidth) == "number" && typeof(window.innerHeight) == "number")
		{
			intHeight = window.innerHeight;
			intWidth = window.innerWidth;
		}
		else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
		{
			intHeight = document.documentElement.clientHeight;
			intWidth = document.documentElement.clientWidth;
		}
		else if (document.body && (document.body.clientWidth || document.body.clientHeight))
		{
			intHeight = document.body.clientHeight;
			intWidth = document.body.clientWidth;
		}
		obj.style.height = intHeight - objTbl.offsetHeight - 2 + "px";
		obj.style.width = intWidth - 2 + "px";
	}
}

function NextFile()
{
	var coll = document.getElementsByName("McFileUp");
	var LatestFile = coll[coll.length - 1];
	var objBr = document.createElement("BR");

	LatestFile.parentNode.appendChild(objBr);

	objBr = document.createElement("BR");

	LatestFile.parentNode.appendChild(objBr);

	var objFile = document.createElement("input");
	objFile.type="file";
	objFile.name="McFileUp";
	objFile.style.width="60%";
	objFile.setAttribute("size","60");
	objFile.onchange=NextFile;
	LatestFile.parentNode.appendChild(objFile);
}

function SetInvisible()
{
	trUploadFrame = document.getElementById("trUploadFrame");
	divUploadButton = document.getElementById("divUploadButton");
	if(trUploadFrame && divUploadButton)
	{
		if(trUploadFrame.style.display == "none")
		{
			trUploadFrame.style.display = "";
			divUploadButton.style.left = trUploadFrame.offsetWidth - 100 + "px";
			divUploadButton.style.top = trUploadFrame.offsetTop + 12 + "px";
			divUploadButton.style.display = "";
		}
		else
		{
			trUploadFrame.style.display = "none";
			divUploadButton.style.display = "none";
		}
		ResizeForm();
	}
}

function StartUpload()
{
	trUploadFrame = document.getElementById("trUploadFrame");
	divUploadButton = document.getElementById("divUploadButton");
	var uploadDocument = window.frames["uploadFrame"].document;				
	var uploadForm = uploadDocument.getElementById("uploadForm");

	var fl = false;
	var colls = uploadDocument.getElementsByName("McFileUp");
	for(var i=0; i<colls.length; i++)
	{
		var col = colls[i];
		if(col.value!="")
		{
			fl = true;
			break;
		}
	}
	
	if(!fl)
	{
		if(uploadFlag)
		{
			uploadFlag = false;
			setTimeout('eval("' + globalId + '")', 50);
			return;
		}
		else
			return;
	}
	
	//start upload
  
	uploadForm.submit();

	if(trUploadFrame && divUploadButton)
	{
		trUploadFrame.style.display = "none";
		divUploadButton.style.display = "none";
	}
  
	progressBar = document.getElementById("progressBar");	
	progressBar.style.width = '0%';

	divWait = document.getElementById("divWait");
	divWait.style.display = "";

	divAttachs = document.getElementById("divAttachs");	
	divAttachs.style.display = "none";

	divAddAttach = document.getElementById("divAddAttach");	
	divAddAttach.style.display = "none";

	percValue = document.getElementById("percValue");
	percValue.innerHTML = "0%";

	ResizeForm();

	refresh(uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value);
}

function refresh(formId)
{
	AddEMailMessage.GetProgressInfo(formId, callback);
}

function callback(response)
{
	if (response.value[0]==-2)
	{
		divWait = document.getElementById("divWait");
		divWait.style.display = "none";
		divAttachs = document.getElementById("divAttachs");	
		divAttachs.style.display = "";
		divProgress = document.getElementById("divProgress");
		divProgress.style.display = "none";
		divAddAttach = document.getElementById("divAddAttach");	
		divAddAttach.style.display = "";
		percValue.innerHTML = "";

		alert('Files were not upload! Try again!');
		window.frames["uploadFrame"].location = window.frames["uploadFrame"].location;
		globalButton.disabled = false;
		uploadFlag = false;
	}
	else if (response.value[0]==-3)
	{
		divWait = document.getElementById("divWait");
		divWait.style.display = "none";
		divAttachs = document.getElementById("divAttachs");	
		divAttachs.style.display = "";
		divProgress = document.getElementById("divProgress");
		divProgress.style.display = "none";
		divAddAttach = document.getElementById("divAddAttach");	
		divAddAttach.style.display = "";
		percValue.innerHTML = "";
		
		if(uploadFlag)
		{
			uploadFlag = false;
			setTimeout('eval("' + globalId + '")', 50);
		}
		else
		{
			window.frames["uploadFrame"].location = window.frames["uploadFrame"].location;
			updateAttachments();
		}
	}
	else if (response.value[0]==-1)
	{
		var uploadDocument = window.frames["uploadFrame"].document;
		setTimeout('refresh("' + uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value + '")', 2000);
	}
	else
	{
		divWait = document.getElementById("divWait");
		divWait.style.display = "none";

		divProgress = document.getElementById("divProgress");
		divProgress.style.display = "block";

		progressBar.style.width = response.value[4] + '%';
		percValue.innerHTML = response.value[4] + '%&nbsp;';

		var uploadDocument = window.frames["uploadFrame"].document;
		setTimeout('refresh("' + uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value + '")', 2000);
	}
}

function updateAttachments()
{
	var _hid = document.getElementById("hidGuid");
	if(_hid)
	AddEMailMessage.GetAttachments(_hid.value, callback2);
}

function updateAttachments2(sFiles)
{
	divAttachs = document.getElementById("divAttachs");	
	divAttachs.innerHTML = sFiles;
	ResizeForm();
}

function updateEMails(sMails)
{
	var colls = document.getElementsByTagName("textarea");
	for(var i=0; i<colls.length; i++)
	{
		var col = colls[i];
		if(col.id.indexOf("txtTo")>=0)
		{
			col.value = sMails;
			break;
		}
	}
}

function _deleteFile(_id)
{
	var _hid = document.getElementById("hidGuid");
	if(_hid)
		AddEMailMessage.DeleteAttachment(_hid.value, _id, callback2);
}

function callback2(response)
{
	divAttachs = document.getElementById("divAttachs");	
	divAttachs.innerHTML = response.value[0];
	ResizeForm();
}

function openAddPortalFiles(_guid)
{
	var w = 640;
	var h = 480;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	var f = window.open("AddPortalAttach.aspx?guid="+_guid, "_blank", winprops);
}

function openAddArticle(_guid)
{
	var w = 640;
	var h = 480;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	var f = window.open("ArticleList.aspx?guid="+_guid, "_blank", winprops);
}

function GetRefreshArticle(sFiles, sText)
{
	var mobj = FTB_API['fckEditor'];
	if(mobj)
	{
		if(mobj.mode != FTB_MODE_HTML)	
		{
			// O.R.: no focus fix
			if(!browseris.ie && !mobj.hasFocus)
			{
				mobj.GoToHtmlMode();
				mobj.GoToDesignMode();
			}

			mobj.InsertHtml(sText);
			
			// O.R.: no focus fix
			if(!browseris.ie)
			{
				mobj.GoToHtmlMode();
				mobj.GoToDesignMode();
			}
		}
		else
		{
			mobj.GoToDesignMode();
			mobj.InsertHtml(sText);
			mobj.GoToHtmlMode();
		}
	}
	else
	{
		var txtobj;
		var txtcols = document.getElementsByTagName("TEXTAREA");
		for(var i=0; i<txtcols.length; i++)
			if(txtcols[i].id.indexOf("fckEditor")>=0)
			{
				txtobj = txtcols[i];
				break;
			}
		if(!txtobj)
			txtobj = document.getElementById("fckEditor");
			
		var designArea;
		var framescols = document.getElementsByTagName("IFRAME");
		for(var j=0; j<framescols.length; j++)
			if(framescols[j].id && framescols[j].id.indexOf("designEditor")>=0)
			{
				designArea = framescols[j];
				break;
			}
		if(!designArea)
			designArea = document.getElementById("fckEditor_designEditor");

		var div_obj = document.getElementById("fckEditor_htmlEditorArea");
		if(div_obj.style.display == "none")
		{
			if(designArea && designArea.contentWindow.document)
			{
				designArea.contentWindow.focus();
				if(browseris.ie)//IE
				{
					sel = designArea.contentWindow.document.selection.createRange();
					sel.pasteHTML(sText);
				}
				else//Mozilla
				{
					selection = designArea.contentWindow.getSelection();
					if (selection)
						range = selection.getRangeAt(0);
					else 
						range = designArea.contentWindow.document.createRange(); 
					var fragment = designArea.contentWindow.document.createDocumentFragment();
					var div = designArea.contentWindow.document.createElement("div");
					div.innerHTML = sText;

					while (div.firstChild)
						fragment.appendChild(div.firstChild);

					selection.removeAllRanges();
					range.deleteContents();

					var node = range.startContainer;
					var pos = range.startOffset;

					switch (node.nodeType) 
					{
						case 3:
							if (fragment.nodeType == 3) 
							{
								node.insertData(pos, fragment.data);
								range.setEnd(node, pos + fragment.length);
								range.setStart(node, pos + fragment.length);
							}
							else 
							{
								node = node.splitText(pos);
								node.parentNode.insertBefore(fragment, node);
								range.setEnd(node, pos + fragment.length);
								range.setStart(node, pos + fragment.length);
							}
							break;
						case 1:
							node = node.childNodes[pos];
							node.parentNode.insertBefore(fragment, node);
							range.setEnd(node, pos + fragment.length);
							range.setStart(node, pos + fragment.length);
							break;
					}
					selection.addRange(range);
				}
			}
		}
		else
		{
			if(txtobj)
				txtobj.value = sText + txtobj.value;
		}
	}

	updateAttachments2(sFiles);
}

function openAddEMails(_id)
{
	var w = 550;
	var h = 310;
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	var obj = document.getElementById(_id);
	var str = "";
	if(obj)
	str = obj.value;
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	var f = window.open("AddEMailAddresses.aspx?emails="+str, "_blank", winprops);
}

function CheckAll(obj)
{
	aInputs = document.getElementsByTagName("input");
	for (var i=0; i<aInputs.length; i++)
	{
		oInput = aInputs[i];
		if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0)
		{
			oInput.checked = obj.checked;
		}
	}
}

function GetSelectedCbString()
{
	var Ids = "";

	aInputs = document.getElementsByTagName("input");
	for (var i=0; i<aInputs.length; i++)
	{
		oInput = aInputs[i];
		if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0 && oInput.checked)
		{
			var str = oInput.value;
			Ids += str+",";
		}
	}

	return Ids;
}

function CheckExistence(objTo,str)
{
	for(var j=0;j<objTo.options.length;j++)
		if(objTo.options[j].text==str)
			return true;
	return false;
}

function AddOption(objTo,Option)
{
	var oOption = document.createElement("OPTION");
	oOption.text = Option.text;
	oOption.value = Option.value;
	objTo.options[objTo.options.length] = oOption;
}

function ToSMTPSettings()
{
	window.opener.location.href='../Admin/SMTPSettings.aspx';
}

var globalId = "";
var uploadFlag=false;
var globalButton = null;

function CheckForUpload(str, btn)
{
	if(btn)
	{
		btn.disabled = true;
		globalButton = btn;
	}
	trUploadFrame = document.getElementById("trUploadFrame");
	divUploadButton = document.getElementById("divUploadButton");
	if(trUploadFrame && divUploadButton)
	{
		if(trUploadFrame.style.display=="")
		{
			globalId = str;
			uploadFlag = true;
			StartUpload();
		}
		else
			setTimeout('eval("' + str + '")', 50);
	}
	else
	{
		setTimeout('eval("' + str + '")', 50);
	}
}