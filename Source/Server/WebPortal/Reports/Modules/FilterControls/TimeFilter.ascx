<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeFilter.ascx.cs" Inherits="Mediachase.UI.Web.Reports.Modules.TimeFilter" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<script type="text/javascript">
	function ChangeModifyTime(obj)
	{
		var objTd = document.getElementById('<%=tdSecond.ClientID %>');
		var id = obj.value;
		if(id == "3")
		{
			objTd.style.display = '';
		}
		else
		{
			objTd.style.display = 'none';
		}
	}
</script>
<table cellspacing="0" cellpadding="2" border="0" style="padding-top:10px;">
	<tr>
		<td runat="server" id="Migrated_tdTitle" class="text">
			<b><asp:Label ID="lblTitle" Runat="server" CssClass="text"></asp:Label>:</b>&nbsp;&nbsp;&nbsp;
		</td>
		<td valign="top" class="text">
			<asp:DropDownList ID="ddType" runat="server" Width="115px" onchange="ChangeModifyTime(this);"></asp:DropDownList>
		</td>
		<td align="left" width="70px" class="text">
			<ibn:Time id="dtcFromDate" ShowTime="HM" ViewStartDate="True" runat="server" HourSpinMaxValue="999"/>
		</td>
		<td id="tdSecond" runat="server" align="left" width="100px" class="text">
			<table cellpadding="0" cellspacing="0">
				<tr>
					<td align="center" width="20px"><b>-</b></td>
					<td><ibn:Time id="dtcToDate" Path_Img="../Layouts/Images/" Path_JS="../Scripts/" File_JS="mcCalendScript.js" ControlViewType="OnlyTime" ShowTime="HM" TimeSpin="true" ViewStartDate="True" runat="server" HourSpinMaxValue="999"/></td>
				</tr>
			</table>
		</td>
	</tr>
</table>