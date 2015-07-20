<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Calendar.Modules.CalendarViewMy" Codebehind="CalendarViewMy.ascx.cs" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ie" Namespace="Mediachase.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web" %>
<script type="text/javascript">
	function ChangeOT(obj)
	{
		ChColl=document.getElementsByTagName("input");
		for(j=0;j<ChColl.length;j++)
		{
			obj_c = ChColl[j];
			_obj_id = obj_c.id;
			if(_obj_id.indexOf("cblType")>=0)
				obj_c.checked = obj.checked;
		}
	}
</script>
<table class="ibn-stylebox2 text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr runat="server" id="trFilter">
		<td class="ibn-alternating ibn-navline text"  valign="top">
			<table width="100%" cellspacing="0" cellpadding="7" border="0" class="text">
				<tr>
					<td width="150px" valign=top runat="server" id="tdShow2">
						<fieldset>
						<legend><asp:CheckBox ID="cbChkAll" onclick='javascript:ChangeOT(this)' Runat=server></asp:CheckBox></legend>
						<asp:CheckBoxList RepeatDirection="Vertical" ID="cblType" Runat="server" CssClass="text"></asp:CheckBoxList>
						</fieldset>
					</td>
					<td valign=top width="250" runat="server" id="tdPerson" style="padding: 15,5,5,10">
						<%=LocRM.GetString("Person") %>:&nbsp;
						<asp:DropDownList ID="ddlPerson" Runat="server" Width="150"></asp:DropDownList>
					</td>
					<td height="100%">
						<table width=100% height=100% cellspacing="0" cellpadding="0" style="margin-top:-5">
							<tr>
								<td align=right valign=top>
									<asp:LinkButton ID="lbHideFilter" Runat=server CssClass=text></asp:LinkButton>
								</td>
							</tr>
							<tr>
								<td valign=bottom align=right runat="server" id="tdShow3" style="padding-top:5">
									<asp:Button ID="btnApply" Runat="server" Text="Apply" CssClass="text"></asp:Button>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			
			<table class="ibn-alternating" cellspacing="0" cellpadding="3" width="100%" border="0" visible="false" runat="server" id="tblListViewFilter">
				<tr>
					<td class="text" width="80" >
						<nobr><div style="padding-left:7px"><%=LocRM.GetString("From") %>:</div></nobr>
					</td>
					<td>
						<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="false" />
					</td>
					<td class="text" width="70">
						<%=LocRM.GetString("To") %>:
					</td>
					<td>
						<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="false" />
					</td>
					<td class="text" width="60">
						<%=LocRM.GetString("Keyword") %>:
					</td>
					<td><asp:textbox id="tbKeyword" CssClass="text" Width="120" Runat="server"></asp:textbox></td>
				</tr>
				<tr>
					<td class="text" width="80" >
					<nobr><div style="padding-left:7px">
						<%=LocRM.GetString("Project") %>:</div></nobr>
					</td>
					<td class="text"><asp:dropdownlist id="ddProjects" Width="130px" Runat="server"></asp:dropdownlist></td>
					<td class="text" width="70">
						<%=LocRM.GetString("Type") %>:
					</td>
					<td class="text"><asp:dropdownlist id="ddAss" Runat="server"></asp:dropdownlist></td>
					<td class="text" align="right" colSpan="2">
						<P align="left"><asp:button id="btnApplyFilter" CssClass="text" Runat="server" Width="120px"></asp:button>&nbsp;
							<asp:button id="btnResetFilter" CssClass="text" Runat="server" CausesValidation="False" Width="120px"></asp:button>&nbsp;</P>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr runat="server" id="trFilterView">
		<td valign="top">
			<table class="ibn-navline text" cellspacing=0 cellpadding=4 border=0 width=100%>
				<tr>
					<td valign="top" style="padding-bottom:5">
						<table cellspacing=3 cellpadding=0 border=0 runat="server" id="tblFilterInfoSet" class="text">
						</table>
					</td>
					<td valign="bottom" align="right" height="100%">
						<table height="100%" cellspacing="0" cellpadding="0" style="margin-top:-5">
							<tr>
								<td valign=top align=right>
									<asp:LinkButton ID="lbShowFilter" Runat=server CssClass=text></asp:LinkButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<asp:PlaceHolder ID="phItems" Runat="server" />
			<table width="100%" cellpadding="7" cellspacing="0" border="0">
				<tr>
					<td>
						<table class="ibn-toolbar" cellspacing="0" cellpadding="2" width="100%" border="0" style="border:0">
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
						<ie:calendar bordercolor="#afc9ef" id="CalendarCtrl" runat="server" width="100%" spantype="Overflowed" 
							showtime="False" datadescriptionfield="description"  datastartdatefield="StartDate" 
							dataenddatefield="FinishDate" Font-Names="Arial" Font-Size="8" BorderWidth="1px" 
							datatextfield="title" viewtype="MonthView" calendarselecteddate="2003-05-15" 
							datalinkfield="ID" datalinkformat="#" borderstyle="Solid" height="100%" 
							enableviewstate="true">
							<CalendarItemDefaultStyle Font-Size="8"></CalendarItemDefaultStyle>
							<CalendarItemInactiveStyle Font-Size="8" />
							<CalendarItemSelectedStyle Font-Size="8" />
							<CalendarHeaderStyle CssClass="ibn-alternating"></CalendarHeaderStyle>
							<WeekItemTemplate>
								<font class="ibn-descriptiontext"><a title="<%# DataBinder.Eval(Container, "Description").ToString().Replace("\"", "&quot;") %>" href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
										(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
										<%# DataBinder.Eval(Container, "Label") %>- [
										
										<%# DataBinder.Eval(Container, "StartDate", "{0:t}") %>
										-
										<%# DataBinder.Eval(Container, "EndDate", "{0:t}") %>
										]</a></font>
							</WeekItemTemplate>
							<AllDayItemTemplate>
								<font class="ibn-descriptiontext"><%=LocRM3.GetString("tAllday")%> <a href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
										(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>' title="<%# DataBinder.Eval(Container, "Description").ToString().Replace("\"", "&quot;") %>">
										<%# DataBinder.Eval(Container, "Label") %>
									</a></font>
							</AllDayItemTemplate>
							<MonthTextItemTemplate>
								<a class="ibn-descriptiontext" title="<%# DataBinder.Eval(Container, "Description").ToString().Replace("\"", "&quot;") %>" href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
										(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
									<%# DataBinder.Eval(Container, "Label") %>
								</a>
							</MonthTextItemTemplate>
							<YearItemTemplate>
								<a title="<%# DataBinder.Eval(Container, "Description").ToString().Replace("\"", "&quot;") %>" 
								href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
								(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
									<%# DataBinder.Eval(Container, "Label") %>
								</a>
							</YearItemTemplate>
							<DayItemTemplate>
								<a class="ibn-descriptiontext" title="<%# DataBinder.Eval(Container, "Description").ToString().Replace("\"", "&quot;") %>" href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
										(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
									<%# DataBinder.Eval(Container, "Label") %>
								</a>
							</DayItemTemplate>
							<DefaultItemTemplate>
								<a class="ibn-descriptiontext" title="<%# DataBinder.Eval(Container, "Description").ToString().Replace("\"", "&quot;") %>" href='<%# GetUrl((int)DataBinder.Eval(Container.DataItem,"ID"),
										(Mediachase.IBN.Business.CalendarView.CalendarFilter)DataBinder.Eval(Container.DataItem,"Type"))%>'>
									<%# DataBinder.Eval(Container, "Label") %>
								</a>
							</DefaultItemTemplate>
						</ie:calendar>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
