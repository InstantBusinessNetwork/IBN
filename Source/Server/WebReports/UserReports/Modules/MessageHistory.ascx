<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.MessageHistory" CodeBehind="MessageHistory.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.UI.Web.UserReports.GlobalModules" Assembly="Mediachase.Ibn.DefaultUserReports" %>
<table style="font-size: 11px; font-family: Arial" border="0">
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tBegDate")%>:
		</td>
		<td width="100">
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
	<tr runat="server" id="GroupRow">
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tUserGroup")%>:
		</td>
		<td>
			<lst:IndentedDropDownList CssClass="text" ID="lstGroup1" runat="server" Width="200px" AutoPostBack="True" Style="width: 200px" OnSelectedIndexChanged="lstGroup1_SelectedIndexChanged">
			</lst:IndentedDropDownList>
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tUser")%>:
		</td>
		<td>
			<asp:Label Visible="False" CssClass="text" ID="lblUser" Style="width: 200px" runat="server"></asp:Label>
			<select class="text" id="lstUser" style="width: 200px" name="lstUserId" runat="server">
			</select>
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tContUserGroup")%>:
		</td>
		<td>
			<lst:IndentedDropDownList CssClass="text" ID="lstGroup2" runat="server" Width="200px" AutoPostBack="True" Style="width: 200px" OnSelectedIndexChanged="lstGroup2_SelectedIndexChanged">
			</lst:IndentedDropDownList>
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tContUser")%>:
		</td>
		<td>
			<select class="text" id="lstContact" style="width: 200px" name="lstContactId" runat="server">
			</select>
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tMess")%>:
		</td>
		<td>
			<select class="text" id="lstInOut" style="width: 200px" name="lstInOutId" runat="server">
			</select>
		</td>
	</tr>
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tKeyWord")%>:
		</td>
		<td>
			<asp:TextBox CssClass="text" runat="server" Width="200" ID="txtKeyword" Style="width: 200px"></asp:TextBox>
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
			<asp:Button CssClass="text" runat="server" ID="btnSubmit" Width="80" Text="Show!" OnClick="btnSubmit_Click"></asp:Button>
		</td>
	</tr>
</table>
<hr style="color: #d8d9d9; height: 1px">
<asp:Repeater ID="MessRep" runat="server" EnableViewState="false">
	<HeaderTemplate>
		<table style="word-wrap: break-word; width: 100%; font-size: 11px; font-family: Arial; table-layout: fixed" border="0" cellspacing="0" cellpadding="1">
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td style="width: 2%; margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="font-weight: bold; font-size: 11px; font-family: Arial; width: 5%; margin-left: 5px;">
				<%=LocRM.GetString("tFrom")%>:
			</td>
			<td class="text" style="width: 78%; margin-left: 5px">
				<%# Eval("FromLast")%> <%# Eval("FromFirst")%>
			</td>
			<td align="right" style="width: 15%; margin-left: 5px">
				&nbsp;
			</td>
		</tr>
		<tr>
			<td style="width: 2%; margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="font-weight: bold; font-size: 11px; font-family: Arial; width: 5%; margin-left: 5px">
				<%=LocRM.GetString("tTo")%>:
			</td>
			<td class="text" style="width: 78%; margin-left: 5px">
				<%# Eval("ToLast")%> <%# Eval("ToFirst")%>
			</td>
			<td align="right" nowrap class="text" style="width: 15%; margin-left: 5px">
				<%# ((DateTime)Eval("send_time")).ToString()%>
			</td>
		</tr>
		<tr>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td class="text" style="width: 78%; margin-left: 5px">
				<%# Eval("mess_text")%>
			</td>
			<td>&nbsp;</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="ibn-alternating">
			<td style="width: 2%; margin-left: 5px">&nbsp;</td>
			<td class="text" style="font-weight: bold; font-size: 11px; font-family: Arial; width: 5%; margin-left: 5px">
				<%=LocRM.GetString("tFrom")%>:
			</td>
			<td class="text" style="width: 78%; margin-left: 5px">
				<%# Eval("FromLast")%> <%# Eval("FromFirst")%>
			</td>
			<td align="right" style="width: 15%; margin-left: 5px">&nbsp;</td>
		</tr>
		<tr class="ibn-alternating">
			<td style="width: 2%; margin-left: 5px">&nbsp;</td>
			<td class="text" style="font-weight: bold; font-size: 11px; font-family: Arial; width: 5%; margin-left: 5px">
				<%=LocRM.GetString("tTo")%>:
			</td>
			<td class="text" style="width: 78%; margin-left: 5px">
				<%# Eval("ToLast")%> <%# Eval("ToFirst")%>
			</td>
			<td align="right" nowrap class="text" style="width: 15%; margin-left: 5px">
				<%# ((DateTime)Eval("send_time")).ToString()%>
			</td>
		</tr>
		<tr class="ibn-alternating">
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td class="text" style="width: 78%; margin-left: 5px">
				<%# Eval("mess_text")%>
			</td>
			<td>&nbsp;</td>
		</tr>
	</AlternatingItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
<hr style="color: #d8d9d9; height: 1px" />
