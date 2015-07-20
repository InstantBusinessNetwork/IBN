<%@ Reference Control="~/Directory/Modules/PrefsEdit2.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.PrefsEdit" CodeBehind="PrefsEdit.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="usr" TagName="Prefs" Src="..\..\Directory\Modules\PrefsEdit2.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" style="width:100%">
	<tr>
		<td>
			<ibn:BlockHeader ID="tbEditPrefs" Title="Edit User Preferences" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-right: 8px; padding-left: 8px; padding-top: 8px">
			<usr:Prefs ID="ctlEditPrefs" runat="server"></usr:Prefs>
		</td>
	</tr>
	<tr>
		<td style="padding-right: 8px" valign="middle" align="right" height="60">
			<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 110px;" OnServerClick="btnSave_ServerClick"></btn:IMButton>
			&nbsp;&nbsp;
			<btn:IMButton class="text" CausesValidation="false" ID="btnCancel" runat="server"
				Text="" IsDecline="true" style="width: 110px;" OnServerClick="btnCancel_ServerClick">
			</btn:IMButton>
		</td>
	</tr>
</table>
