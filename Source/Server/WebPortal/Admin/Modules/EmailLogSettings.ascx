<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailLogSettings.ascx.cs" Inherits="Mediachase.UI.Web.Admin.Modules.EmailLogSettings" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="text" cellspacing="10" cellpadding="0" border="0" width="100%" >
	<tr>
		<td>
			<%=LocRM.GetString("tEmailLogPeriodHint1")%>&nbsp;
			<asp:TextBox Runat="server" ID="tbLogPeriod" Width="30px" CssClass="text"></asp:TextBox>
			<%=LocRM.GetString("tEmailLogPeriodHint2")%>
		</td>
	</tr>
	<tr>
		<td align="center" style="padding-top:10px;">
			<btn:IMButton ID="UpdateLogSettingsButton" Runat="server" Class="text" style="width:110px;" onserverclick="UpdateLogSettingsButton_ServerClick"></btn:IMButton>&nbsp;&nbsp;
			<btn:IMButton runat="server" class="text" ID="CancelButton" onclick="javascript:window.close();" style="width:110px" CausesValidation="false"></btn:IMButton>
		</td>
	</tr>
</table>