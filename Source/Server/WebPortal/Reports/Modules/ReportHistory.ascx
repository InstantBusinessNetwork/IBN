<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Reports.Modules.ReportHistory" Codebehind="ReportHistory.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx"%>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
	function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
</script>
<table style="margin-top:0px;margin-left:2px" class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" title="Report History" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="ibn-navline ibn-alternating text" cellspacing="0" cellpadding="4" width="100%" border="0">
				<tr height="30">
					<td style="PADDING-LEFT: 10px; PADDING-TOP: 3px;padding-bottom:4px">
						<table cellpadding="7" cellspacing="0" border="0">
							<tr>
								<td width="115" class="text"><b><%=LocRM.GetString("tTemplateTitle")%>:</b></td>
								<td colspan="4">
									<asp:Label ID="lblTempTitle" Runat="server" CssClass="text"></asp:Label>
								</td>
							</tr>
							<tr>
								<td width="115" class="text"><b><%=LocRM.GetString("tCreator")%>:</b></td>
								<td width="160px">
									<asp:Label ID="lblCreatorName" CssClass="text" Runat="server"></asp:Label>
								</td>
								<td width="85" class="text"><b><%=LocRM.GetString("tModifier")%>:</b></td>
								<td width="160px" align="left">
									<asp:Label ID="lblModifierName" Runat="server" CssClass="text"></asp:Label>
								</td>
								<td></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<table cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td style="PADDING-TOP: 10px; padding-right:5px">
						<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%" autogeneratecolumns="False" PageSize="10" borderwidth="0" gridlines="None" cellpadding="1">
							<headerstyle></headerstyle>
							<columns>
								<asp:boundcolumn datafield="ReportItemId" Visible="False"></asp:boundcolumn>
								<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="CreationDate" sortexpression="CreationDate">
									<itemtemplate>
										<%# "<a href=\"javascript:OpenWindow('../Reports/XMLReportOutput.aspx?ReportId=" +
											DataBinder.Eval(Container.DataItem, "ReportItemId").ToString() + 
											"',screen.width,screen.height,true);\">"+
											((DateTime)DataBinder.Eval(Container.DataItem, "CreationDate")).ToString() + "</a>"%>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn sortexpression="CreatorId" headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2">
									<itemtemplate>
										<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "CreatorId"))%>
									</itemtemplate>
								</asp:templatecolumn>
								<asp:templatecolumn itemstyle-cssclass="ibn-vb2" headerstyle-cssclass="ibn-vh2-right" headerstyle-width="49" itemstyle-width="49">
									<HeaderStyle HorizontalAlign="right"></HeaderStyle>
									<ItemStyle HorizontalAlign="right"></ItemStyle>
									<ItemTemplate>
										<asp:imagebutton id="ibView" runat="server" borderwidth="0" imageurl="../../layouts/images/icon-search.gif" commandname="View" causesvalidation="False">
										</asp:imagebutton>&nbsp;
										<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" imageurl="../../layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
										</asp:imagebutton>
									</ItemTemplate>
								</asp:templatecolumn>
							</columns>
						</dg:datagridextended>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:Button ID="btnAddToFavorites" Runat="server" Visible="False" OnClick="btnAddToFavorites_Click" />