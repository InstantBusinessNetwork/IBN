<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.TimeControl" Codebehind="TimeControl.ascx.cs" %>
<div id="dvControl" style="border:0px;" runat="server">
<table cellpadding="0" cellspacing="0" border="0">
	<tr style="border:1px solid black;">
		<td valign="top" id="tdSpinHMS" runat="server" style="background-color: Transparent; white-space: nowrap">
			<table cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td valign="top" class="text" style="border-width: 1px; border-style: inset; background-color: white; white-space: nowrap"><asp:TextBox style="text-align:center;border-style:none;border-width:0;border:0px solid;width:1.5em;" MaxLength="2" id="txtSpinHours" class="text" runat="server" /><span id="spSpinHM" runat="server">:</span><asp:TextBox style="text-align:center;border-style:none;border-width:0;border:0px solid;width:1.5em;" MaxLength="2" id="txtSpinMinutes" class="text" runat=server /><span id="spSpinMS" runat="server">:</span><asp:TextBox style="text-align:center;border-style:none;border-width:0;border:0px solid;width:1.5em;" MaxLength="2" id="txtSpinSeconds" class="text" runat=server /><asp:customvalidator id="SpinValidator" runat="server" Display="Dynamic" ErrorMessage="" /></td>
				</tr>
			</table>
		</td>
		<td height="100%" valign="top" id="tdBtnsImgSpin" runat="server" style="white-space: nowrap">
			<table cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td style="padding-bottom:1px;"><img alt="Up" height="8" width="11" hspace="2" border="0" style="cursor:pointer;height:8px;width:11px;" id="btnImgSpinUp" runat="server" /></td>
				</tr>
				<tr>
					<td style="padding-top:1px;"><img alt="Down" height="8" width="11" hspace="2" border="0" style="cursor:pointer;height:8px;width:11px;" id="btnImgSpinDown" runat="server" /></td>
				</tr>
			</table>
		</td>
		<td height="100%" valign="top" id="tdBtnImgReset" visible="False" runat="server" style="white-space: nowrap"><img alt="Reset" height="17" width="19" hspace="10" border="0" style="cursor:pointer;border:solid 1px #e8f0f8;height:17px;width:19px;" id="btnImgReset" visible="False" runat="server" /></td>
	</tr>
</table>
</div>