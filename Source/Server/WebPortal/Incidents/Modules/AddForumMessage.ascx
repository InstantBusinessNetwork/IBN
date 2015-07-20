<%@ Reference Page="~/Incidents/AddForumMessage.aspx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.AddForumMessage" Codebehind="AddForumMessage.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
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

function startUpload()
{
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
	refresh(uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value);
}

function refresh(formId)
{
	AddForumMessage.GetProgressInfo(formId, callback);
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
		window.opener.location.href = window.opener.location.href;
		setTimeout('closeWindow()', 1000);
	}
	else if (response.value[0]==-1)
	{
		uploadStatus.innerHTML = hidWaitingValue.value;

		var uploadDocument = window.frames["uploadFrame"].document;
		setTimeout('refresh("' + uploadDocument.forms[0].__MEDIACHASE_FORM_UNIQUEID.value + '")', 2000);
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
	window.close();
}
</script>

<table cellpadding="0" cellspacing="0" width="100%" border="0" class="text">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr id="trUploadFrame">
		<td>
			<iframe name="uploadFrame" id="uploadFrame" frameborder="0" marginwidth="0" style="margin-right:-15" marginheight="0" height="380px" width="100%" src='AddForumMessageUploadHandler.aspx?IncidentId=<%=IncidentId%><%=AddGuid%>'></iframe>
		</td>
	</tr>
	<tr>
		<td style="padding:7px">
			<p id="psaveButton" align="right" style="padding-right:10px;">
				<mc:ImButton runat="server" class='text' onclick="{startUpload();return false;}" ID="imbSave" style="width:110px"></mc:ImButton>&nbsp;
				<mc:ImButton runat="server" class='text' onclick="closeWindow();" ID="imbCancel" style="width:110px"></mc:ImButton>
			</p>
			<div id="progressDisplay" style="display:none">
				<%=LocRM3.GetString("tUploadStatus")%>: <i id="uploadStatus"></i>
				<table id="progressDisplayTable" border="0" cellpadding="6" width="300" class="text" style="display:none;font-size:90%;">
					<tr>
						<td><b><%=LocRM3.GetString("tSize")%>:</b>
							<span id="uploadSize"></span>
							&nbsp;<%=LocRM3.GetString("tOf")%>&nbsp;
							<span id="uploadTotalSize"></span></td>
					</tr>
					<tr>
						<td><b><%=LocRM3.GetString("tTime")%>:</b>
							<br><%=LocRM3.GetString("tEstimatedTime")%>&nbsp;<span id="uploadTime"></span><br><%=LocRM3.GetString("tRemainingTime")%>&nbsp;<span id="uploadRemainingTime"></span></td>
					</tr>
					<tr><td><b><%=LocRM3.GetString("tCompleted")%>:</b>&nbsp;<span id="percValue"></span></td></tr>
					<tr>
						<td>
							<div id="progressContainer" style="border:solid 1px #008000;height:20px;width:100%;"><div id="progressBar" style="background-color:#00aa00;margin:1px;height:18px;display:block;"></div>
							</div>
						</td>
					</tr>
				</table>
				<p><input class="text" type=button id="cancelButton" onclick="javascript:cancelUpload()" value='<%=LocRM2.GetString("tbSaveCancel")%>'/></p>
			</div>
			<div id="uploadError" style="display:none">
			</div>
			<div id="uploadSuccess" style="display:none">
				<%=LocRM3.GetString("tUploadSuccess")%>
			</div>
		</td>
	</tr>
</table>
<input type=hidden id="hidInitializingValue" value='<%=LocRM3.GetString("tInitializing")%>' />
<input type=hidden id="hidWaitingValue" value='<%=LocRM3.GetString("tWaitForUploading")%>' />