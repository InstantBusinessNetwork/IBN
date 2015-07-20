<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.AlertsHistory" CodeBehind="AlertsHistory.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.UI.Web.UserReports.GlobalModules" Assembly="Mediachase.Ibn.DefaultUserReports" %>
<table style="font-size: 11px; font-family: Arial" border="0">
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tBegDate")%>:
		</td>
		<td>
			<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tEndDate")%>:
		</td>
		<td>
			<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
		</td>
	</tr>
	<tr id="GroupRow" runat="server">
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tUserGroup")%>:
		</td>
		<td>
			<lst:IndentedDropDownList ID="lstGroup1" runat="server" CssClass="text" AutoPostBack="True" Width="200px" OnSelectedIndexChanged="lstGroup1_SelectedIndexChanged">
			</lst:IndentedDropDownList>
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tUser")%>:
		</td>
		<td>
			<asp:Label ID="lblUser" runat="server" CssClass="text" Width="200px" Visible="False"></asp:Label><select class="text" id="lstUser" style="width: 200px" name="lstUserId" runat="server"></select>
		</td>
	</tr>
	<tr>
		<td style="padding-left: 10px">
			<img alt="" height="1" src="../layouts/images/spacer.gif" width="100" border="0" />
		</td>
		<td>
			<asp:CheckBox CssClass="text" runat="server" ID="chkOrder" Text="Display Last Event First"></asp:CheckBox>
		</td>
	</tr>
	<tr>
		<td style="padding-left: 10px">
			<img alt="" height="1" src="../layouts/images/spacer.gif" width="100" border="0" />
		</td>
		<td>
			<br />
			<asp:Button ID="btnSubmit" CssClass="text" Width="80" runat="server" Text="Show!" OnClick="btnSubmit_Click"></asp:Button>
		</td>
	</tr>
</table>
<hr style="color: #d8d9d9; height: 1px" />
<asp:Repeater ID="MessRep" runat="server" EnableViewState="false">
	<HeaderTemplate>
		<table style="word-wrap: break-word; width: 100%; font-size: 11px; font-family: Arial; table-layout: fixed" border="0" cellspacing="0" cellpadding="1" width="100%">
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td style="width: 2%; margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="width: 83%; margin-left: 5px">
				&nbsp;
			</td>
			<td align="right" nowrap="nowrap" class="text" style="width: 15%; margin-left: 5px">
				<%# ((DateTime)DataBinder.Eval(Container.DataItem,"send_time")).ToString()%>
			</td>
		</tr>
		<tr>
			<td style="width: 2%; margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="width: 83%; margin-left: 5px">
				<%# DataBinder.Eval(Container.DataItem,"mess_text")%><br />
				<br />
			</td>
			<td style="width: 15%; margin-left: 5px">
				&nbsp;
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="ibn-alternating">
			<td style="width: 2%; margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="width: 83%; margin-left: 5px">
				&nbsp;
			</td>
			<td align="right" nowrap="nowrap" class="text" style="width: 15%; margin-left: 5px">
				<%# ((DateTime)DataBinder.Eval(Container.DataItem,"send_time")).ToString()%>
			</td>
		</tr>
		<tr class="ibn-alternating">
			<td style="width: 2%; margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="width: 83%; margin-left: 5px">
				<%# DataBinder.Eval(Container.DataItem,"mess_text")%><br />
				<br />
			</td>
			<td style="width: 15%; margin-left: 5px">
				&nbsp;
			</td>
		</tr>
	</AlternatingItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
<hr style="color: #d8d9d9; height: 1px" />
