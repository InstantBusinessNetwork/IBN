<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.MostActiveReport" CodeBehind="MostActiveReport.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" Src="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<style type="text/css">
.top { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 thin solid; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin }
.leftright { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 thin; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin }
.bottom { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 thin; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin solid }
.all { BORDER-RIGHT: #999999 thin solid; BORDER-TOP: #999999 thin solid; BORDER-LEFT: #999999 thin solid; BORDER-BOTTOM: #999999 thin solid }
</style>
<div id="filter" runat="server" style="border-bottom: #cccccc 1px solid" printable="0">

	<script type="text/javascript">
	//<![CDATA[
	function ExportReport(sReportName)
	{
		ChangeFormAction("?action=export&filename=" + sReportName + ".xls");
	}
	
	function ChangeFormAction(sAction)
	{
		var sCurAction = window.document.forms[0].action;
		var pos = sCurAction.indexOf("?");
		if (pos > 0)
			sCurAction = sCurAction.substring(0, pos);
		window.document.forms[0].action = sCurAction + sAction;
		<%= Page.GetPostBackClientEvent(btnApplySelectPeriod, "")%>
	}
	function ChangeModify(obj)
	{
		objTbl = document.getElementById('<%=tableDate.ClientID %>');
		id=obj.value;
		if(id=="9")
		{
			objTbl.style.display = 'block';
		}
		else
		{
			objTbl.style.display = 'none';
		}
	}
	//]]>
	</script>

	<table width="100%">
		<tr>
			<td width="160px">
				<fieldset id="fsMain" style="" runat="server">
					<legend class="text" id="lgdMain" runat="server"></legend>
					<table width="">
						<tr>
							<td align="left" valign="top" width="160px">
								<asp:RadioButtonList AutoPostBack="True" CssClass="text" ID="rblSelectType" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="rblSelectType_SelectedIndexChanged">
								</asp:RadioButtonList>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
			<td>
				<table cellpadding="5" cellspacing="0" border="0">
					<tr height="45px">
						<td width="120px" class="text">
							<b>
								<%=LocRM.GetString("tPeriod")%>:</b>&nbsp;
						</td>
						<td valign="center">
							<select class="text" id="ddPeriod" style="width: 150px" onchange="ChangeModify(this);" runat="server" name="ddPeriod">
							</select>
						</td>
						<td>
							<table id="tableDate" cellspacing="2" cellpadding="0" runat="server">
								<tr>
									<td class="text">
										&nbsp;<b><%=LocRM.GetString("tFrom")%>:</b>&nbsp;
									</td>
									<td>
										<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
									</td>
									<td class="text" style="padding-left: 10px">
										<b>
											<%=LocRM.GetString("tTo")%>:</b>&nbsp;
									</td>
									<td>
										<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr height="45px">
						<td width="120px" class="text">
							<b>
								<%=LocRM.GetString("tViewBy")%>:</b>&nbsp;
						</td>
						<td valign="center">
							<asp:DropDownList ID="ddSelectView" runat="server" CssClass="text" Width="150px">
							</asp:DropDownList>
						</td>
						<td>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<table>
		<tr class="ibn-descriptiontext">
			<td width="160px">
			</td>
			<td>
				<asp:Button ID="btnApplySelectPeriod" runat="server" CssClass="text" Text="Show" Width="80" OnClick="btnApplySelectPeriod_Click"></asp:Button>&nbsp;&nbsp;
			</td>
			<td>
				<input type="button" class="text" style="width: 80px" value='<%=LocRM.GetString("tPrint")%>' onclick="javascript:window.print()">
			</td>
		</tr>
	</table>
	<table>
		<tr height="1px">
			<td>
			</td>
		</tr>
	</table>
</div>
<div id="dHeader" style="display: none; margin-bottom: 20px" printable="1">
	<ibn:up ID="_header" runat="server"></ibn:up>
</div>
<br />
<br />
<table border="0" cellpadding="3" cellspacing="0" class="text" width="100%">
	<asp:Label ID="lblTable" runat="server" CssClass="text"></asp:Label>
	<tr id="trLegend" runat="server">
		<td>
			&nbsp;
		</td>
		<td align="left" width="40" class="text">
			0
		</td>
		<td align="middle">
			<asp:Label ID="lblRepType" runat="server" CssClass="text"></asp:Label>
		</td>
		<td align="right" width="40">
			<asp:Label ID="lblMaxValue" runat="server" CssClass="text">1281</asp:Label>
		</td>
	</tr>
</table>
