<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.Modules.ChatHistory" CodeBehind="ChatHistory.ascx.cs" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<table style="font-size: 11px; font-family: Arial" border="0">
	<tr>
		<td class="text" style="padding-left: 10px">
			<%=LocRM.GetString("tConf")%>:
		</td>
		<td style="width: 200px;">
			<asp:Label ID="noConferences" runat="server" CssClass="text" Visible="False"></asp:Label>
			<select class="text" id="lstChats" style="width: 200px" name="lstChatId" runat="server">
			</select>
		</td>
	</tr>
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
			<asp:Button CssClass="text" runat="server" ID="btnSubmit" Width="80px" Text="Show!" OnClick="btnSubmit_Click"></asp:Button>
		</td>
	</tr>
</table>
<hr style="color: #d8d9d9; height: 1px" />
<asp:Repeater ID="MessRep" runat="server">
	<HeaderTemplate>
		<table style="font-size: 11px; font-family: Arial; table-layout: fixed" border="0" cellspacing="0" cellpadding="5" width="100%">
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td width="2%" style="margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" width="6%" style="font-weight: bold; font-size: 11px; font-family: Arial; margin-left: 5px">
				<%=LocRM.GetString("tFrom")%>:
			</td>
			<td class="text" style="margin-left: 5px">
				<%# DataBinder.Eval(Container.DataItem,"first_name")%>
				<%# DataBinder.Eval(Container.DataItem,"last_name")%>
			</td>
			<td class="text" align="right" width="15%" style="margin-left: 5px">
				<%# ((DateTime)DataBinder.Eval(Container.DataItem,"send_time")).ToString()%>
			</td>
		</tr>
		<tr>
			<td width="2%" style="margin-left: 5px">
				&nbsp;
			</td>
			<td width="6%" style="margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="margin-left: 5px">
				<%# DataBinder.Eval(Container.DataItem,"mess_text")%>
			</td>
			<td width="15%" style="margin-left: 5px">
				&nbsp;
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="ibn-alternating">
			<td width="2%" style="margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" width="6%" style="font-weight: bold; font-size: 11px; font-family: Arial; margin-left: 5px">
				<%=LocRM.GetString("tFrom")%>:
			</td>
			<td class="text" style="margin-left: 5px">
				<%# DataBinder.Eval(Container.DataItem,"first_name")%>
				<%# DataBinder.Eval(Container.DataItem,"last_name")%>
			</td>
			<td class="text" align="right" width="15%" style="margin-left: 5px">
				<%# ((DateTime)DataBinder.Eval(Container.DataItem,"send_time")).ToString()%>
			</td>
		</tr>
		<tr class="ibn-alternating">
			<td width="2%" style="margin-left: 5px">
				&nbsp;
			</td>
			<td width="6%" style="margin-left: 5px">
				&nbsp;
			</td>
			<td class="text" style="margin-left: 5px">
				<%# DataBinder.Eval(Container.DataItem,"mess_text")%>
			</td>
			<td width="15%" style="margin-left: 5px">
				&nbsp;
			</td>
		</tr>
	</AlternatingItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
<hr style="color: #d8d9d9; height: 1px" />
