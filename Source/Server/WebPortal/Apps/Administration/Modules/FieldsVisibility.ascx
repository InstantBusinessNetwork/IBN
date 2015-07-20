<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldsVisibility.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Administration.Modules.FieldsVisibility" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td colspan="2">
			<mc:BlockHeader2 runat="server" ID="BlockHeaderMain" />
		</td>
	</tr>
	<tr>
		<td style="padding: 5px;">
			<table cellpadding="7" border="0">
				<tr>
					<td width="400px"><asp:CheckBox ID="allowPriority" runat="server" CssClass="text" /></td>
				</tr>
				<tr>
					<td><asp:CheckBox ID="allowGenCats" runat="server" CssClass="text" /></td>
				</tr>
				<tr>
					<td><asp:CheckBox ID="allowClient" runat="server" CssClass="text" /></td>
				</tr>
				<tr>
					<td><asp:CheckBox ID="allowTaskTime" runat="server" CssClass="text" /></td>
				</tr>
				<tr>
					<td style="padding-top:15px;" align="right">
						<btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:110px;" onserverclick="btnSave_Click"></btn:ImButton>&nbsp;
						<btn:ImButton ID="btnCancel" CausesValidation="false" Runat="server" Class="text" IsDecline="true" style="width:110px;" onserverclick="btnCancel_Click" />
					</td>
				</tr>
			</table>
		</td>
		<td style="padding:5px;" class="text" valign="top">
			<div style="background-color:#ffffe1;border:1px solid #bbb;" class="text">
			<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
				<%=GetGlobalResourceObject("IbnFramework.Admin", "GeneralVisibilityWarning").ToString()%>
				<div style="white-space:nowrap; text-align:center; padding-top:15px;">
					<asp:Label ID="lblGoToObjectSettings" runat="server" Font-Bold="true" Font-Underline="true" ForeColor="Red"></asp:Label>
				</div>
			</blockquote>
			</div>
		</td>
	</tr>
</table>