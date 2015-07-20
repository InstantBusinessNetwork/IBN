<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.GroupAndUserImStat" CodeBehind="GroupAndUserImStat.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Reference Control="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" Src="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<div id="filter" style="border-bottom: #cccccc 1px solid" runat="server" printable="0">

	<script type="text/javascript">
		//<![CDATA[
		function ChangeModify(obj) {
			objTbl = document.getElementById('<%=tableDate.ClientID %>');
			id = obj.value;
			if (id == "9") {
				objTbl.style.display = 'block';
			}
			else {
				objTbl.style.display = 'none';
			}
		}
		//]]>
	</script>

	<table cellspacing="0" cellpadding="5" border="0">
		<tr>
			<td class="text" width="120px">
				<b>
					<%=LocRM.GetString("Period")%>
					:</b>&nbsp;
			</td>
			<td valign="middle">
				<select class="text" id="ddPeriod" style="width: 150px" onchange="ChangeModify(this);" name="ddPeriod" runat="server">
				</select>
			</td>
			<td width="400px">
				<table id="tableDate" cellspacing="2" cellpadding="0" runat="server">
					<tr>
						<td class="text">
							&nbsp;<b><%=LocRM1.GetString("tFrom")%>:</b>&nbsp;
						</td>
						<td>
							<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
						<td class="text" style="padding-left: 10px">
							<b>
								<%=LocRM1.GetString("tTo")%>:</b>&nbsp;
						</td>
						<td>
							<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="text" width="120px">
				<b>
					<%=LocRM.GetString("ContactGroup")%>
					:</b>&nbsp;
			</td>
			<td valign="middle">
				<asp:DropDownList ID="ddContactGroup" runat="server" CssClass="text">
				</asp:DropDownList>
			</td>
			<td align="right">
				<table>
					<tr class="ibn-descriptiontext">
						<td width="80px">
						</td>
						<td>
							<asp:Button ID="btnAplly" runat="server" Width="80px" CssClass="text" Text="Show" OnClick="btnAplly_Click"></asp:Button>
						</td>
						<td>
							<input class="text" style="width: 80px" onclick="javascript:window.print()" type="button" value='<%=LocRM.GetString("tPrint")%>' />
						</td>
						<td width="400">
							<asp:Button ID="btnExport" runat="server" CssClass="text" Width="80px" Visible="False"></asp:Button>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</div>
<div id="dHeader" style="display: none; margin-bottom: 20px" printable="1">
	<ibn:up ID="_header" runat="server"></ibn:up>
</div>
<table border="0" cellpadding="0" cellspacing="5" class="text">
	<tr>
		<td>
			<b>
				<%=LocRM.GetString("ContactGroup")%>:&nbsp;</b><asp:Label ID="lblContactGroup" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;
		</td>
		<td>
			<b>
				<%=LocRM.GetString("TotalIM")%>:&nbsp;</b><asp:Label ID="lblTotalIM" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;
		</td>
		<td width="200">
			<nobr><b><%=LocRM.GetString("TotalDuration")%>:&nbsp;</b><asp:Label ID="lblTotalDuration" Runat=server></asp:Label></nobr>
		</td>
	</tr>
</table>
<table border="0" cellpadding="3" cellspacing="0" class="text" width="100%">
	<tr>
		<td class="ibn-vh2">
			<%=LocRM.GetString("Name")%>
		</td>
		<td width="300" class="ibn-vh2">
			<%=LocRM.GetString("IMSessions")%>
		</td>
		<td width="100" class="ibn-vh2">
			<%=LocRM.GetString("Duration")%>
		</td>
	</tr>
</table>
<table border="0" cellpadding="3" cellspacing="0" class="text" width="100%">
	<asp:Repeater ID="repUsers" runat="server">
		<ItemTemplate>
			<tr>
				<td class="ibn-vb2">
					<%# DataBinder.Eval(Container.DataItem,"FirstName") + 
					" " +  DataBinder.Eval(Container.DataItem,"LastName")%>
				</td>
				<td class="ibn-vb2" width="400px">
					<table border="0" cellspacing="0" cellpadding="0" class="text" width="100%">
						<asp:Repeater ID="repSessions" runat="server">
							<ItemTemplate>
								<tr>
									<td width="300px">
										<%# GetDateString
											(
												(DateTime)DataBinder.Eval(Container.DataItem,"SessionBegin"),
												DataBinder.Eval(Container.DataItem,"SessionEnd")
											)
										%>
									</td>
									<td>
										<%# GetDuration
											(
												(DateTime)DataBinder.Eval(Container.DataItem,"SessionBegin"),
												DataBinder.Eval(Container.DataItem,"SessionEnd")
											) 
										%>
									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
					<asp:Label ID="lblNone" runat="server" Text="tutu"></asp:Label>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
</table>
