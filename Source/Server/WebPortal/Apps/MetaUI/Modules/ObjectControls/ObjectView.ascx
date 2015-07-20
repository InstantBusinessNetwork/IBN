<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Modules.ObjectControls.ObjectView" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Reference VirtualPath="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-stylebox2 ibn-propertysheet">
	<tr id="trToolbar">
		<td>
			<mc:BlockHeader2 ID="MainBlockHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td valign="top">
			<div id="mainDiv" style="overflow:auto;">
				<div style="height:100%">
					<ibn:XMLFormBuilder ID="xmlStruct" runat="server" />
				</div>
			</div>
		</td>
	</tr>
</table>
