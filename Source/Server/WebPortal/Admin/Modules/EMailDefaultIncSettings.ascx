<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailDefaultIncSettings" Codebehind="EMailDefaultIncSettings.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<table cellspacing="0" cellpadding="7" border="0" class="text">
				<tr>
					<td><b>Creator:</b></td>
					<td><asp:DropDownList ID="ddCreator" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b>Manager:</b></td>
					<td><asp:DropDownList ID="ddManager" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b>Priority:</b></td>
					<td><asp:DropDownList ID="ddPriority" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b>Project:</b></td>
					<td><asp:DropDownList ID="ddProject" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b>Severity:</b></td>
					<td><asp:DropDownList ID="ddSeverity" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b>Type:</b></td>
					<td><asp:DropDownList ID="ddType" Runat="server" Width="200px"></asp:DropDownList></td>
				</tr>
				<tr>
					<td><b>Task Time:</b></td>
					<td><ibn:Time id="ucTaskTime" ShowTime="HM" HourSpinMaxValue="999" ViewStartDate="True" runat="server" /></td>
				</tr>
				<tr height="50px" valign="bottom">
					<td align="right" colspan="2">
						<btn:ImButton runat="server" class='text' ID="imbSave" style="width:110px"></btn:ImButton>&nbsp;
						<btn:ImButton runat="server" class='text' ID="imbCancel" style="width:110px" CausesValidation="false"></btn:ImButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>