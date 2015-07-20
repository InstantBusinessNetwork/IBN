<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="History.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls.History" %>
<%@ Reference Control="~/Apps/ListApp/Modules/ListInfoControls/HistoryFields.ascx" %>
<%@ Register TagPrefix="ibn" TagName="HistoryFields" Src="~/Apps/ListApp/Modules/ListInfoControls/HistoryFields.ascx" %>
<style type="text/css">
.infoBlock 
{
	vertical-align:middle; 
	padding-top:10px; 
	padding-bottom:10px; 
	background-color:#ffffe1;
	border:1px solid #bbbbbb;
}
.infoBlockInner
{
	border-left:solid 2px #CE3431; 
	padding: 3px 10px 3px 10px; 
	margin: 5px 5px 5px 15px; 
}	
.infoText
{
	text-align:center; 
	padding-top:10px;
}
.actionLink
{
	font-weight:bold;
	color: red ! important;
	text-decoration: underline ! important;
}
</style>
<div class="infoBlock" runat="server" id="ServiceIsNotInstalledBlock" style="margin:17px 7px 7px 7px;">
	<blockquote class="infoBlockInner">
		<asp:Literal runat="server" ID="Literal1" Text="<%$Resources : IbnFramework.ListInfo, HistoryIsNotInstalled%>"></asp:Literal>
		<div class="infoText">
			<asp:LinkButton ID="InstallHistory" runat="server" Text="<%$Resources : IbnFramework.ListInfo, InstallHistory%>" CssClass="actionLink" OnClick="InstallHistory_Click"></asp:LinkButton>
		</div>
	</blockquote>
</div>
<table width="100%" border="0" cellpadding="0" cellspacing="7" runat="server" id="ServiceIsInstalledBlock">
	<tr>
		<td style="width:50%;" valign="top">
			<ibn:HistoryFields runat="server" id="HistoryFieldsControl"></ibn:HistoryFields>
		</td>
		<td valign="top" style="padding-top:10px;">
			<div class="infoBlock"> 
				<blockquote class="infoBlockInner">
					<asp:Literal runat="server" ID="Literal2" Text="<%$Resources : IbnFramework.ListInfo, HistoryIsInstalled%>"></asp:Literal>
					<div class="infoText">
						<asp:LinkButton ID="UnistallHistory" runat="server" Text="<%$Resources : IbnFramework.ListInfo, UninstallHistory%>" CssClass="actionLink" OnClick="UnistallHistory_Click"></asp:LinkButton>
					</div>
				</blockquote>
			</div>
		</td>
	</tr>
</table>
