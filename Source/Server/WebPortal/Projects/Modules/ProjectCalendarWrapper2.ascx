<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectCalendarWrapper2" Codebehind="ProjectCalendarWrapper2.ascx.cs" %>
<%@ Register TagPrefix="ie" Namespace="Mediachase.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web" %>
<table cellspacing="0" border="0" class="text">
	<tr>
		<td style="PADDING-LEFT:10px;" width="110px">
			<b><%=LocRM2.GetString("Show") %>:</b></td>
		<td colspan="3">
			<asp:CheckBoxList RepeatDirection="Horizontal" ID="cblType" Runat="server" CssClass="text" CellPadding="0" CellSpacing="10" ></asp:CheckBoxList>
		</td>
	</tr>
	<tr>
		<td style="PADDING-LEFT:10px;">
			<b><%=LocRM2.GetString("Person") %>:</b>
		</td>
		<td style="PADDING-LEFT:15px;">
			<asp:DropDownList ID="ddlPerson" Runat="server" Width="150"></asp:DropDownList>
		</td>
		<td align="right">
			<asp:Button ID="btnApply" Runat="server" Text="Apply" CssClass="text"></asp:Button>
		</td>
	</tr>
</table>

<div style="margin-top:10; border-top: 1px solid #afc9ef"></div>
<table class="ibn-alternating text" cellspacing="0" cellpadding="2" width="100%" border="0">
	<tr valign="middle">
		<td width="200" align="left" style="TEXT-ALIGN:left" valign="middle" class="text">
			<a href=<%=Page.ClientScript.GetPostBackClientHyperlink(DecDateButton,"")%>>
			<img src='../layouts/images/cleft.gif' width='16' height='16' align="absmiddle" border=0>
			<asp:label id="lblDec" runat="server"></asp:label></a>
		</td>
		<td align="middle">
			<b>
			<asp:label id="lblMonth" runat="server"></asp:label></b>
		</td>
		<td  align="right" width="200" style="TEXT-ALIGN:right" valign="center" class="text">
		<a href=<%=Page.ClientScript.GetPostBackClientHyperlink(IncDateButton,"")%>>
		<asp:label id="lblInc" runat="server"></asp:label>
		<img src='../layouts/images/cright.gif' width='16' height='16' align="absmiddle" border=0>
		</a>
		</td>
	</tr>
</table>
<asp:linkbutton id="IncDateButton" runat="server" Visible="False" onclick="IncDateButton_Click"></asp:linkbutton>
<asp:linkbutton  id="DecDateButton" runat="server" Visible="False" onclick="DecDateButton_Click"></asp:linkbutton>

<ie:calendar bordercolor="#afc9ef" id="CalendarCtrl" runat="server" width="100%" spantype="Overflowed" showtime="False" datadescriptionfield="description" enableviewstate="true" datastartdatefield="StartDate" dataenddatefield="FinishDate" Font-Names="Arial" Font-Size="8" BorderWidth="1px" datatextfield="title" viewtype="MonthView" calendarselecteddate="2003-05-15" datalinkfield="ID" datalinkformat="#" borderstyle="Solid" height="100%" >
	<CalendarItemDefaultStyle Font-Size="8"></CalendarItemDefaultStyle>
	<CalendarItemInactiveStyle Font-Size="8" />
	<CalendarItemSelectedStyle Font-Size="8" />
	<CalendarHeaderStyle CssClass="ibn-alternating"></CalendarHeaderStyle>
	<WeekItemTemplate>
		<font class="ibn-descriptiontext"><a title='<%# DataBinder.Eval(Container, "Description") %>' href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
				(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
				<%# DataBinder.Eval(Container, "Label") %>
				- [<%# DataBinder.Eval(Container, "StartDate", "{0:t}") %>
				-
				<%# DataBinder.Eval(Container, "EndDate", "{0:t}") %>
				] </a></font>
	</WeekItemTemplate>
	<AllDayItemTemplate>
		<font class="ibn-descriptiontext"><%=LocRM.GetString("tAllday")%> <a href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
				(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>' title='<%# DataBinder.Eval(Container, "Description") %>'>
				<%# DataBinder.Eval(Container, "Label") %>
			</a></font>
	</AllDayItemTemplate>
	<MonthTextItemTemplate>
	
		<a class="ibn-descriptiontext" title='<%# DataBinder.Eval(Container, "Description") %>' href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
				(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
			<%# DataBinder.Eval(Container, "Label") %>
		</a>
	</MonthTextItemTemplate>
	<YearItemTemplate>
		<a title='<%# DataBinder.Eval(Container, "Description") %>' 
		href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
		(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
			<%# DataBinder.Eval(Container, "Label") %>
		</a>
	</YearItemTemplate>
	<DayItemTemplate>
		<a class="ibn-descriptiontext" title='<%# DataBinder.Eval(Container, "Description") %>' href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
				(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
			<%# DataBinder.Eval(Container, "Label") %>
		</a>
	</DayItemTemplate>
	<DefaultItemTemplate>
		<a class="ibn-descriptiontext" title='<%# DataBinder.Eval(Container, "Description") %>' href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
				(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
			<%# DataBinder.Eval(Container, "Label") %>
		</a>
	</DefaultItemTemplate>
</ie:calendar>

<asp:Label ID="lblWeekViewFix" Runat="server" Visible="False"></td></tr></table></asp:Label>
