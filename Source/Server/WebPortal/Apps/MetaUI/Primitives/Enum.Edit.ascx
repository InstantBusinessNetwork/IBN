<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Enum.Edit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Enum_Edit" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" width="100%" style="table-layout: fixed;">
				<tr>
					<td style="padding-top:1px;" width="100%">
						<asp:DropDownList ID="ddlValue" runat="server" Width="99%"></asp:DropDownList>
					</td>
					<td width="20px" runat="server" id="tdEdit" style="padding-left:2px;">
						<button id="btnEditItems" runat="server" style="border:0px;padding:0;height:20px;width:22px;background-color:transparent" type="button" tabindex="-1"><img 
						height="20" title='<%=HttpContext.GetGlobalResourceObject("IbnFramework.Common", "Edit") %>' src='<%=CHelper.GetAbsolutePath("/Images/IbnFramework/dictionary_edit.gif")%>' width="22" border="0" /></button>
					</td>
				</tr>
			</table>
		</td>
		<td width="20px"></td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" CausesValidation="False" style="display:none;" OnClick="btnRefresh_Click"></asp:Button>

