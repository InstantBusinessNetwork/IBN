<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SystemRemindersForObject" Codebehind="SystemRemindersForObject.ascx.cs" %>
<br>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td style=padding:10px align="center">
			<table cellpadding="3" cellspacing="0" class="text">
				<tr height="25">
					<td align="right" class="ibn-label"><%= LocRM.GetString("Start")%>:</td>
					<td style="padding-left:10px" width="150px">
						<asp:DropDownList Runat="server" ID="ddStart" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblStart" CssClass="ibn-value"></asp:Label>
					</td>
					<td width="60px">
						<asp:ImageButton Runat="server" ID="ibEditStart" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreStart" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveStart" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelStart" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
				<tr height="25">
					<td align="right" class="ibn-label"><%= LocRM.GetString("Finish")%>:</td>
					<td style="padding-left:10px">
						<asp:DropDownList Runat="server" ID="ddFinish" Width="150px" Visible="False"></asp:DropDownList>
						<asp:Label Runat="server" ID="lblFinish" CssClass="ibn-value"></asp:Label>
					</td>
					<td>
						<asp:ImageButton Runat="server" ID="ibEditFinish" ImageUrl="~/layouts/images/edit.gif" BorderWidth="0" Width="16px" Height="16px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibRestoreFinish" ImageUrl="~/layouts/images/import.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibSaveFinish" ImageUrl="~/layouts/images/Saveitem.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False"></asp:ImageButton>
						<asp:ImageButton Runat="server" ID="ibCancelFinish" ImageUrl="~/layouts/images/cancel.gif" BorderWidth="0" Width="16px" Height="16px" Visible="False" style="margin-left:10px"></asp:ImageButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td vAlign="center" align="center" height="60">
			<asp:Button Runat="server" ID="btnClose"></asp:Button>
		</td>
	</tr>
</table>
