var progressDisplayTable;
var uploadStatus;
var uploadSize;
var uploadTotalSize;
var uploadTime;
var uploadRemainingTime;
var percValue;
var progressBar;
var hidInitializingValue;
var hidWaitingValue;
var psaveButton;
var trUploadFrame;

var hidCName;
var hidCKey;
var hidPFolderId;

var uid;

function startUpload()
{
	var trReName = document.getElementById("trReName");
	trReName.style.display = "none";
	
	var uploadDocument = window.frames["uploadFrame"].document;				
	var uploadForm = uploadDocument.getElementById("uploadForm");
	
	var mcUpload = uploadDocument.getElementById("McFileUp");
	var str = mcUpload.value;
	if(str=="")
		return false;
		
	hidCName = document.getElementById("hidCName");
	hidCKey = document.getElementById("hidCKey");
	hidPFolderId = document.getElementById("hidPFolderId");
	if(hidCName.value=="")
		hidCName.value = "FileLibrary";
	if(hidCKey.value=="" || hidPFolderId.value=="")
	{
		inputColl=document.getElementsByTagName("input");
		for(j=0;j<inputColl.length;j++)
		{
			obj_temp = inputColl[j];
			_obj_id = obj_temp.id;
			if(_obj_id.indexOf("txtFolderId")>=0)
				hidPFolderId.value=obj_temp.value;
			if(_obj_id.indexOf("txtContainerKey")>=0)
				hidCKey.value=obj_temp.value;
		}
	}
	if(hidCKey.value=="" || hidPFolderId.value=="")
		return false;
	
	var divTree = document.getElementById("pTreeControl");
	if(divTree)
		divTree.style.display = "none";
	if(str.lastIndexOf("\\")>=0)
		str = str.substr(str.lastIndexOf("\\")+1);
	var hidFName = uploadDocument.getElementById("hidFName");	
	var hidCFUKey = uploadDocument.getElementById("hidCFUKey");
	var hidFFUId = uploadDocument.getElementById("hidFFUId");
	
	var txtNewName = document.getElementById("txtNewName");	
	if(txtNewName.value!="")
		str = txtNewName.value
	else
		txtNewName.value = str;
	hidFName.value = txtNewName.value;
	hidCFUKey.value = hidCKey.value;
	hidFFUId.value = hidPFolderId.value;
	check(str, hidCName.value, hidCKey.value, hidPFolderId.value);
}

function continueUpload()
{
	var trRe = document.getElementById("trRe");
	trRe.style.display = "none";
	
	var uploadDocument = window.frames["uploadFrame"].document;				
	var uploadForm = uploadDocument.getElementById("uploadForm");
	
	psaveButton = document.getElementById("psaveButton");
	psaveButton.style.display = "none";	
	
	uploadForm.submit();

	trUploadFrame = document.getElementById("trUploadFrame");
	trUploadFrame.style.display = "none";				

	progressDisplayTable = document.getElementById("progressDisplayTable");
	uploadStatus = document.getElementById("uploadStatus");
	uploadSize = document.getElementById("uploadSize");
	uploadTotalSize = document.getElementById("uploadTotalSize");			
	uploadTime = document.getElementById("uploadTime");			
	percValue = document.getElementById("percValue");			
	uploadRemainingTime = document.getElementById("uploadRemainingTime");			
	progressBar = document.getElementById("progressBar");	
	hidInitializingValue = document.getElementById("hidInitializingValue");
	hidWaitingValue = document.getElementById("hidWaitingValue");

	var progressDisplay = document.getElementById("progressDisplay");
	progressDisplay.style.display = "block";

	var progressContainer = document.getElementById("progressContainer");
	progressContainerWidth = progressContainer.offsetWidth;

	// Clear the displays
	uploadSize.innerHTML = "";
	uploadTotalSize.innerHTML = "";
	uploadTime.innerHTML = "";
	percValue.innerHTML = "";
	uploadRemainingTime.innerHTML = "";

	uploadStatus.innerHTML = hidInitializingValue.value;
	uid = uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value;
	refresh(uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value);
}

function refresh(formId)
{
	FileUpload.GetProgressInfo(formId, callback);
}

function check(fName, cname, ckey, pfolder)
{
	FileUpload.GetFileExistence(fName, cname, ckey, pfolder, callback2);
}

function callback2(response)
{
	if (response.value[0]==0)
	{
		continueUpload();
	}
	else
	{
		reActions(response.value[0]);
	}
}

function reActions(resp)
{
	psaveButton = document.getElementById("psaveButton");
	psaveButton.style.display = "none";
	var trRe = document.getElementById("trRe");
	var rePlace = document.getElementById("rePlace");
	var reNew = document.getElementById("reNew");
	if(resp==1)
		reNew.style.display = "none";
	else if(resp==2)
		rePlace.style.display = "none";
	trRe.style.display = "block";
}

function showRename()
{
	var trRe = document.getElementById("trRe");
	trRe.style.display = "none";
	
	var trReName = document.getElementById("trReName");
	trReName.style.display = "block";
}

function callback(response)
{
	if (response.value[0]==-2)
	{
		var uploadError = document.getElementById("uploadError");

		uploadError.innerHTML = response.value[1];

		uploadError.style.display = "block";

		var progressDisplay = document.getElementById("progressDisplay");
		progressDisplay.style.display = "none";
		
		window.frames["uploadFrame"].location = window.frames["uploadFrame"].location;
	}
	else if (response.value[0]==-3)
	{
		var uploadSuccess = document.getElementById("uploadSuccess");

		uploadSuccess.innerHTML = response.value[1];

		uploadSuccess.style.display = "block";

		var progressDisplay = document.getElementById("progressDisplay");
		progressDisplay.style.display = "none";
		
		window.frames["uploadFrame"].location = window.frames["uploadFrame"].location;
		try
		{
		  window.opener.document.forms[0].submit();
		}
		catch(ex)
		{}
		setTimeout('closeWindow()', 1000);
	}
	else if (response.value[0]=="-1")
	{
		uploadStatus.innerHTML = hidWaitingValue.value;

		var uploadDocument = window.frames["uploadFrame"].document;
		if(uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value == uid)
			setTimeout('refresh("' + uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value + '")', 2000);
		else
		{
			var uploadSuccess = document.getElementById("uploadSuccess");

			uploadSuccess.innerHTML = response.value[1];
		
			uploadSuccess.style.display = "block";

			var progressDisplay = document.getElementById("progressDisplay");
			progressDisplay.style.display = "none";
		
			window.frames["uploadFrame"].location = window.frames["uploadFrame"].location;
			try
			{
			  window.opener.document.forms[0].submit();
			}
			catch(ex)
			{}
			setTimeout('closeWindow()', 1000);
		}
	}
	else
	{
		uploadSize.innerHTML = response.value[0];

		uploadTotalSize.innerHTML = response.value[1];

		uploadTime.innerHTML = response.value[2];

		uploadRemainingTime.innerHTML = response.value[3];

		progressBar.style.width = response.value[4] + '%';
		
		percValue.innerHTML = response.value[4] + '%';
		
		if(response.value[4]==100)
		{
			var cancelButton = document.getElementById("cancelButton");
			cancelButton.style.display = "none";
		}

		uploadStatus.innerHTML = response.value[5];

		progressDisplayTable.style.display = "block";

		var uploadDocument = window.frames["uploadFrame"].document;
		setTimeout('refresh("' + uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value + '")', 2000);
	}
}

function cancelUpload()
{
	window.location = window.location;
}

function closeWindow()
{
	var objCB = document.getElementById("cbNewFile");
	if(objCB && objCB.checked)
		window.location = window.location;
	else
		window.close();
}