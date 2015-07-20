<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.GlobalModules.DateTimeClientControl" Codebehind="DateTimeClientControl.ascx.cs" %>
<!-- BEGIN mcCalendar -->
<div id="dvControl" style="border:0px;" runat="server">
<table cellpadding="0" cellspacing="0" border="0">
<tr style="border:1px solid black;">
<td valign="top" nowrap id="tdtxtHiddens" runat="server">
<INPUT id="txtYear" type="hidden" Runat="server">
<INPUT id="txtMonth" type="hidden" Runat="server">
<INPUT id="txtDay" type="hidden" Runat="server">
<INPUT id="txtYear0" type="hidden" Runat="server">
<INPUT id="txtMonth0" type="hidden" Runat="server">
<INPUT id="txtDay0" type="hidden" Runat="server">
<INPUT id="txtYear1" type="hidden" Runat="server">
<INPUT id="txtMonth1" type="hidden" Runat="server">
<INPUT id="txtDay1" type="hidden" Runat="server">
<INPUT id="txtShowYear" type="hidden" Runat="server">
<INPUT id="txtShowMonth" type="hidden" Runat="server">
<INPUT id="txtSelDayWeek" type="hidden" Runat="server">
<INPUT id="txtFirstDW" type="hidden" Runat="server"></td>
<td valign="top" nowrap id="tdtxtDtInt" runat="server"><asp:textbox id="txtDtInt" Runat="server" style="margin:0;" class="text"></asp:textbox></td>
<td valign="top" nowrap id="tdbtnMain" runat="server"><img id="btnMain" style="cursor:pointer;cursor:hand;height:19px;width:25px;" src="mcCalendar.gif" width="25" height="19" border="0" runat="server"></td>
<td valign="top" nowrap id="tdreqtxtDtInt" runat="server"><asp:requiredfieldvalidator id="reqtxtDtInt" runat="server" Display="Dynamic" ControlToValidate="txtDtInt" ErrorMessage="*" /><asp:customvalidator id="OnChangeValidator" runat="server" Display="Dynamic" ErrorMessage="" /></td>
<td valign="top" nowrap id="tdSelHMS" runat="server"><span id="spSelTimePad" runat="server">&nbsp;&nbsp;</span><asp:dropdownlist id="selDtHours" style="" runat="server"></asp:dropdownlist><span id="spTimeHM" runat="server">&nbsp;:&nbsp;</span><asp:dropdownlist id="selDtMinutes" runat="server"></asp:dropdownlist><span id="spTimeMS" runat="server">&nbsp;:&nbsp;</span><asp:dropdownlist id="selDtSeconds" runat="server" /></td>
<td valign="top" nowrap id="tdSpinHMS" runat=server style="background-color:Transparent;">
<table cellpadding="0" cellspacing="0" border="0">
<tr>
<td id="tdSpinPad" runat="server"><span style="font-size:10px;">&nbsp;&nbsp;</span></td>
<td valign="top" nowrap class="text" style="border-width:1px;border-style:inset;background-color:white;"><asp:TextBox style="text-align:center;border-style:none;border-width:0;border:0px solid;width:1.5em;" MaxLength="2" id="txtSpinHours" class="text" runat=server /><span style="font-size:10px;" id="spSpinHM" runat="server">:</span><asp:TextBox style="text-align:center;border-style:none;border-width:0;border:0px solid;width:1.5em;" MaxLength="2" id="txtSpinMinutes" class="text" runat=server /><span style="font-size:10px;" id="spSpinMS" runat="server">:</span><asp:TextBox style="text-align:center;border-style:none;border-width:0;border:0px solid;width:1.5em;" MaxLength="2" id="txtSpinSeconds" class="text" runat=server /><asp:customvalidator id="SpinValidator" runat="server" Display="Dynamic" ErrorMessage="" /></td>
</tr>
</table>
</td>
<td height="100%" valign="top" nowrap id="tdBtnsImgSpin" runat=server>
<table cellpadding="0" cellspacing="0" border="0">
<tr><td style="padding-bottom:1px;"><img alt="Up" src="mcCalendarSpinUp.gif" height="8" width="11" hspace="2" border="0" style="cursor:pointer;cursor:hand;height:8px;width:11px;" id="btnImgSpinUp" runat="server"></td></tr>
<tr><td style="padding-top:1px;"><img alt="Down" src="mcCalendarSpinDn.gif" height="8" width="11" hspace="2" border="0" style="cursor:pointer;cursor:hand;height:8px;width:11px;" id="btnImgSpinDown" runat="server"></td></tr>
</table>
</td>
<td height="100%" valign="top" nowrap id="tdBtnImgReset" Visible="False" runat="server"><img alt="Reset" src="reset17.gif" height="17" width="19" hspace="10" border="0" style="cursor:pointer;cursor:hand;border:solid 1px #e8f0f8;height:17px;width:19px;" id="btnImgReset" Visible="False" runat="server"></td>
</tr>
</table>
<div id="dvMain" style="border:0px;display:none;z-index:255;position:absolute;top:0px;left:0px;" runat="server">
<table id="tbMain" cellSpacing="1" cellPadding="0" border="0" style="border-top:solid 1px black;border-bottom:solid 1px black;border-right:solid 1px black;border-left:solid 1px black;" bgcolor="#ffffff" runat="server">
<tr id="trMonth" runat="server">
	<td class="mcCalend_StdYM" noWrap bgColor="#ddecfe" colSpan="7"><center><div class="mcCalend_SdvM" id="dvM" runat="server">
		<table cellpadding="0" cellspacing="0" style="width:100%;table-layout:fixed;word-wrap:break-word;wordBreak:break-all;">
			<tr>
				<td style="width:18px;"><div><img alt="&lt;" src="mcCalendarPrev1.gif" height="18" width="18" border="0" id="btnImgDecMonth" style="height:18px;width:18px;" runat="server"></div></td>
				<td class="mcCalend_StdHeader" id="tdHeaderM" align="center" runat="server">-------- ----</td>
				<td style="width:18px;"><div><img alt="&gt;" src="mcCalendarNext1.gif" height="18" width="18" border="0" id="btnImgIncMonth" style="height:18px;width:18px;" runat="server"></div></td>
			</tr>
		</table>
	</div></center></td>
</tr>
<tr id="trYear" runat="server">
	<td class="mcCalend_StdYM" noWrap bgColor="#8eb8e8" colSpan="7"><center><div class="mcCalend_SdvY">
		<table cellpadding="0" cellspacing="0" style="width:100%;table-layout:fixed;word-wrap:break-word;wordBreak:break-all;">
			<tr>
				<td style="width:18px;"><div><img alt="&lt;" src="mcCalendarPrev2.gif" height="18" width="18" border="0" id="btnImgDecYear" style="height:18px;width:18px;" runat="server"></div></td>
				<td class="mcCalend_StdHeader" id="tdHeaderY" align="center" runat="server">----</td>
				<td style="width:18px;"><div><img alt="&gt;" src="mcCalendarNext2.gif" height="18" width="18" border="0" id="btnImgIncYear" style="height:18px;width:18px;" runat="server"></div></td>
			</tr>
		</table>
	</div></center></td>
</tr>
<tr id="trWDs" class="mcCalend_StrWDs" bgcolor="#efebde" runat="server">
	<td>*</td>
	<td>*</td>
	<td>*</td>
	<td>*</td>
	<td>*</td>
	<td>*</td>
	<td>*</td>
</tr>
<tr id="trDays1" class="mcCalend_StrDays" bgcolor="#ddecfe">
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
</tr>
<tr id="trDays2" class="mcCalend_StrDays" bgcolor="#ddecfe">
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
</tr>
<tr id="trDays3" class="mcCalend_StrDays" bgcolor="#ddecfe">
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
</tr>
<tr id="trDays4" class="mcCalend_StrDays" bgcolor="#ddecfe">
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
</tr>
<tr id="trDays5" class="mcCalend_StrDays" bgcolor="#ddecfe">
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
</tr>
<tr id="trDays6" class="mcCalend_StrDays" bgcolor="#ddecfe">
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
	<td>--</td>
</tr>
</table>
</div>
</div>
<!-- END mcCalendar -->