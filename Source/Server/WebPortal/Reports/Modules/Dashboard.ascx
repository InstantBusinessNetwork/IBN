<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.Dashboard" Codebehind="Dashboard.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script language=javascript>
	function changeSRC()
	{
		var ddObj = document.getElementById('<%=ddChartView.ClientID%>');
		var rbObj = document.getElementById('<%=rbChartType.ClientID%>');
		var imgObj = document.getElementById('<%=imgGraph.ClientID%>');
		if(ddObj!=null && rbObj!=null && imgObj!=null)
		{
			var elColl = rbObj.getElementsByTagName('INPUT');
			var s = "";
			for(var i=0;i<elColl.length;i++)
				if(elColl[i].checked)
				{
					s = elColl[i].value;
					break;
				}
			if(s=="0")
				s = "Pie&";
			else if(s=="1")
				s = "Gist&";
			var sPath = "?Type=" + s + ddObj.value + "=1";
			if(imgObj.src.indexOf("Modules/ChartImage.aspx" + sPath)<0)
			{
				var d = new Date();
				sPath += "&id=" + d.getHours() + d.getMinutes() + d.getSeconds();
				imgObj.src = "../Modules/ChartImage.aspx" + sPath;
			}
		}
	}
</script>
<table class="ibn-stylebox" width="100%" cellpadding="0" cellspacing="0" style="margin-top:0px; margin-left:2px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="ibn-navline ibn-alternating text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<colgroup><col width="170px" /><col width="200px" /><col /></colgroup>
				<tr>
					<td colspan="3"><%=LocRM.GetString("tDashTopComment")%></td>
				</tr>
				<tr height="40px">
					<td width="170px" style="padding-left:10px"><b><%=LocRM.GetString("tAvailablePerspectives")%>:</b></td>
					<td width="200px"><asp:DropDownList ID="ddPerspect" AutoPostBack=True Runat=server Width="170px" CssClass="text"></asp:DropDownList></td>
					<td>&nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr>
					<td width="40%" style="padding-left:20px" valign=top>
						<table border="0" class="text" cellpadding="3" cellspacing="0">
						<tr>
							<td colspan=2>
								<h5 style="color:#676767; padding-left:20px"><%=LocRM.GetString("tAvailableMetrics")%>:</h5>
							</td>
						</tr>
						<asp:Repeater ID="repMetrics" Runat=server>
							<ItemTemplate>
								<tr>
									<td><%# DataBinder.Eval(Container.DataItem,"Title") %></td>
									<td style="padding-left:7px"><b><%# DataBinder.Eval(Container.DataItem,"Count") %></b></td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
						</table>
					</td>
					<td valign=top style="padding:10 15 10 0">
						<ibn:BlockHeaderLight id="secPieChart" runat="server" />
						<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td>
									<table class="ibn-navline ibn-alternating text" cellpadding="5" cellspacing="0" width="100%" border="0">
										<tr height="30px">
											<td colspan=3><%=LocRM.GetString("tDashGraphComment")%></td>
										</tr>
										<tr height="50px">
											<td width="100px" style="padding-left:10px"><b><%=LocRM.GetString("tSelectView")%>:</b></td>
											<td width="180px"><asp:DropDownList onchange='changeSRC();' ID="ddChartView" Runat=server CssClass="text" Width="170px"></asp:DropDownList></td>
											<td align=right><asp:RadioButtonList onclick='changeSRC();' Font-Bold=True CellPadding="3" ID="rbChartType" CssClass="text" Runat=server RepeatDirection=Horizontal></asp:RadioButtonList></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td width="100%" align="middle" valign="center">
									<asp:Image id="imgGraph" BorderWidth="0" runat="server"></asp:Image>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trCustomReps" runat=server>
		<td style="padding:10px">
			<ibn:BlockHeaderLight id="secReports" runat="server" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr><td style="padding:10px" class="ibn-navline ibn-alternating"><%=LocRM.GetString("tCustRepComment")%></td></tr>
				<tr>
					<td style="PADDING-TOP: 10px">
						<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%" autogeneratecolumns="False" PageSize="10" borderwidth="0" gridlines="None" cellpadding="1">
							<headerstyle></headerstyle>
							<columns>
								<asp:boundcolumn datafield="TemplateId" Visible="False"></asp:boundcolumn>
								<asp:Templatecolumn sortExpression="IsGlobal">
									<headerstyle cssclass="ibn-vh2" width="23px"></headerstyle>
									<itemstyle cssclass="ibn-vb2" width="23px"></itemstyle>
									<itemtemplate>
										<%# GetType((bool)DataBinder.Eval(Container.DataItem, "IsGlobal"))%>
									</itemtemplate>
								</asp:Templatecolumn>
								<asp:templatecolumn sortexpression="TemplateName" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2">
									<itemtemplate>
										<a href='../Reports/ReportHistory.aspx?TemplateId=<%# DataBinder.Eval(Container.DataItem, "TemplateId")%>'>
											<%# DataBinder.Eval(Container.DataItem, "TemplateName")%>
										</a>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn sortexpression="TemplateCreatorId" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="150" itemstyle-width="150">
									<itemtemplate>
										<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "TemplateCreatorId"))%>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Created" headerstyle-width="90px" itemstyle-width="90px" sortexpression="TemplateCreated">
									<itemtemplate>
										<%# ((DateTime)DataBinder.Eval(Container.DataItem, "TemplateCreated")).ToShortDateString()%>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn sortexpression="TemplateModifierId" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="150" itemstyle-width="150">
									<itemtemplate>
										<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "TemplateModifierId"))%>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Modified" headerstyle-width="90px" itemstyle-width="90px" sortexpression="TemplateModified">
									<itemtemplate>
										<%# ((DateTime)DataBinder.Eval(Container.DataItem, "TemplateModified")).ToShortDateString()%>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn itemstyle-cssclass="ibn-vb2" headerstyle-width="69px" itemstyle-width="69px">
									<itemtemplate>
										<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" imageurl="../../layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
										</asp:imagebutton>&nbsp;
										<asp:imagebutton id="ibView" runat="server" borderwidth="0" imageurl="../../layouts/images/report.gif" commandname="View" causesvalidation="False">
										</asp:imagebutton>&nbsp;
										<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" imageurl="../../layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
										</asp:imagebutton>
									</itemtemplate>
								</asp:templatecolumn>
							</columns>
						</dg:datagridextended>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>