<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceStatusEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Modules.ResourceStatusEdit" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<style type="text/css">.pad5 {padding: 4px;}</style>
<div class="pad5">
<ibn:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secHeader" runat="server" />
<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td class="pad5" align="center">
			<ibn:IMButton id="btnAccept" runat="server" style="width:110px"></ibn:IMButton>
			&nbsp;
			<ibn:IMButton id="btnTentative" runat="server" style="width:130px"></ibn:IMButton>
			&nbsp;
			<ibn:IMButton id="btnDecline" runat="server" style="width:110px"></ibn:IMButton>
		</td>
	</tr>
</table>
</div>