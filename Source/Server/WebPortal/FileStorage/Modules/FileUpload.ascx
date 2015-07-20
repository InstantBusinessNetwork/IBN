<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="Mediachase.UI.Web.FileStorage.Modules.FileUpload" %>
<%@ Reference VirtualPath="~/Modules/DirectoryTreeView.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DirectoryTreeView" Src="../../Modules/DirectoryTreeView.ascx" %>

<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0" style="margin: 10px 20px 10px 20px;">
	<tr id="trUploadFrame">
		<td>
			<span>
				<%=LocRM.GetString("tBrowse")%></span><br />
			<iframe name="uploadFrame" id="uploadFrame" style="border:0" marginheight="0" frameborder="0" height="60px" width="100%" src='../FileStorage/UploadHandler.aspx?FolderId=<%=ParentFolderId%>&amp;ContainerKey=<%=ContainerKey%>&amp;ContainerName=<%=ContainerName%>&amp;ExtId=<%=ExternalId %>'></iframe>
		</td>
	</tr>
	<tr>
		<td>
			<div id="pGlobalTree" runat="server">
				<div id="pTreeControl" style='display: <%=( (ParentFolderId>0)? "none":"block" )%>'>
					<span>
						<%=LocRM.GetString("tSelectFolder")%></span><br />
					<ibn:DirectoryTreeView ID="ctrlDirView" runat="server" Width="400px" Height="250px"></ibn:DirectoryTreeView>
				</div>
			</div>
			<p id="psaveButton">
				<span>
					<%=LocRM.GetString("tClickUpload")%></span><br />
				<br />
				<input type="button" id="saveButton" class="text" onclick="javascript:startUpload()" value='<%=LocRM.GetString("tUpload")%>' />
				<br />
				<input type="checkbox" id="cbNewFile" /><label for="cbNewFile" class="text"><%=LocRM.GetString("tAnotherOne")%></label>
			</p>
			<div id="progressDisplay" style="display: none">
				<%=LocRM.GetString("tUploadStatus")%>: <i id="uploadStatus"></i>
				<table id="progressDisplayTable" border="0" cellpadding="6" width="300" class="text" style="display: none; font-size: 90%;">
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tSize")%>:</b> <span id="uploadSize"></span>&nbsp;<%=LocRM.GetString("tOf")%>&nbsp; <span id="uploadTotalSize"></span>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tTime")%>:</b>
							<br />
							<%=LocRM.GetString("tEstimatedTime")%>&nbsp;<span id="uploadTime"></span><br />
							<%=LocRM.GetString("tRemainingTime")%>&nbsp;<span id="uploadRemainingTime"></span>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tCompleted")%>:</b>&nbsp;<span id="percValue"></span>
						</td>
					</tr>
					<tr>
						<td>
							<div id="progressContainer" style="border: solid 1px #008000; height: 20px; width: 100%;">
								<div id="progressBar" style="background-color: #00aa00; margin: 1px; height: 18px; display: block;">
								</div>
							</div>
						</td>
					</tr>
				</table>
				<p>
					<input class="text" type="button" id="cancelButton" onclick="javascript:cancelUpload()" value='<%=LocRM.GetString("Cancel")%>' /></p>
			</div>
			<div id="uploadError" style="display: none">
			</div>
			<div id="uploadSuccess" style="display: none">
				<%=LocRM.GetString("tUploadSuccess")%>
			</div>
		</td>
	</tr>
	<tr id="trRe" style="display: none">
		<td style="white-space:nowrap">
			<%=LocRM.GetString("tFileExists")%><br />
			<br />
			<input class="text" type="button" id="rePlace" onclick="javascript:continueUpload()" value='<%=LocRM.GetString("tReplace")%>'/>
			<input class="text" type="button" id="reNew" onclick="javascript:continueUpload()" value='<%=LocRM.GetString("tReNewVer")%>'/>&nbsp;&nbsp;
			<input class="text" type="button" id="reName" onclick="javascript:showRename()" value='<%=LocRM.GetString("tRename")%>'/>&nbsp;&nbsp;
			<input class="text" type="button" id="reCancel" onclick="javascript:window.close()" value='<%=LocRM.GetString("Cancel")%>'/>
		</td>
	</tr>
	<tr id="trReName" style="display: none">
		<td>
			<input style="width: 400px" type="text" id="txtNewName" class="text" /><br />
			<br />
			<input class="text" type="button" id="reNName" onclick="javascript:startUpload()" value='<%=LocRM.GetString("tUpload")%>' />&nbsp;&nbsp;
		</td>
	</tr>
</table>
<input type="hidden" id="hidInitializingValue" value='<%=LocRM.GetString("tInitializing")%>' />
<input type="hidden" id="hidWaitingValue" value='<%=LocRM.GetString("tWaitForUploading")%>' />
<input type="hidden" id="hidCName" value='<%=ContainerName%>' />
<input type="hidden" id="hidCKey" value='<%=ContainerKey%>' />
<input type="hidden" id="hidPFolderId" value='<%=ParentFolderId%>' />