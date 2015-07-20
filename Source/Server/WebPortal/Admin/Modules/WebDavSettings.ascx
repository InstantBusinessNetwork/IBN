<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebDavSettings.ascx.cs" Inherits="Mediachase.UI.Web.Admin.Modules.WebDavSettings" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<table border="0" cellpadding="9" cellspacing="0" class="text">
				<tr>
					<td width="240px"><b><%=LocRM.GetString("tLifeTimeWebDAV")%>:</b></td>
					<td><asp:TextBox ID="txtLifeTime" runat="server"></asp:TextBox>
					<asp:CompareValidator ID="cvLifeTime" runat="server" ControlToValidate="txtLifeTime" Display="Dynamic" ErrorMessage="*" CssClass="text" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer"></asp:CompareValidator></td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<mc:IMButton id="btnSave" class="text" runat="server" />&nbsp;&nbsp;
						<mc:IMButton id="btnCancel" class="text" runat="server" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>