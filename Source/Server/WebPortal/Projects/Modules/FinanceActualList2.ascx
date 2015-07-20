<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.FinanceActualList2" Codebehind="FinanceActualList2.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="DTCC" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<style type="text/css">
	.pp { PADDING-RIGHT: 3px; PADDING-LEFT: 3px;Height:21px; BORDER-BOTTOM: #e4e4e4 1px solid }
	.headstyle {padding-top:5px;padding-bottom:5px; border-bottom:1px solid #e4e4e4}
	.rp {padding-right:10px; text-align:right;}
</style>
<script language="javascript" type="text/javascript">
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=0';
	var f = window.open(query, "_blank", winprops);
}
</script>
<asp:UpdatePanel runat="server" ID="PanelFilters" ChildrenAsTriggers="true">
	<ContentTemplate>
		<table cellpadding="3" cellspacing="0" class="FilterTable" border="0" style="height:35px;">
			<tr>
				<td style="width:70px;" align="right">
					<asp:Literal runat="server" ID="PeriodLiteral" Text="<%$Resources: IbnFramework.Global, _mc_TimePeriod %>"></asp:Literal>:
				</td>
				<td>
					<table cellpadding="0" cellspacing="0">
						<tr>
							<td><asp:DropDownList ID="PeriodList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PeriodList_SelectedIndexChanged" /></td>
							<td><Ibn:DTCC ID="Dtc0" runat="server" AutoPostBack="true" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" OnValueChange="Dtc_ValueChange"/></td>
							<td><Ibn:DTCC ID="Dtc1" runat="server" AutoPostBack="true" SelectedMode="Day" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" OnValueChange="Dtc_ValueChange"/></td>
							<td style="width:15px;font-weight:bold;"><asp:Label runat="server" ID="DashLabel" Text="-"></asp:Label></td>
							<td><Ibn:DTCC ID="Dtc2" runat="server" AutoPostBack="true" SelectedMode="Day" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" OnValueChange="Dtc_ValueChange"/></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		<table cellpadding="0" cellspacing="0" width="100%" border="0" style="OVERFLOW:hidden" align="left" >
			<tr>
				<td valign="top" style="padding-top:5px">
					<dg:DataGridExtended Runat="server" ID="dgAccounts" allowpaging="True" allowsorting="True" cellpadding="0" gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" pagesize="10" LayoutFixed="false">
						<columns>
							<asp:boundcolumn datafield="ActualId" Visible="False"></asp:boundcolumn>
							<asp:boundcolumn datafield="ActualId" Visible="False"></asp:boundcolumn>
							<asp:TemplateColumn SortExpression="ActualDate">
								<itemstyle cssclass="ibn-vb2 pp" width="80px"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle" width="80px"></headerstyle>
								<itemtemplate>
									<%# ((DateTime)DataBinder.Eval(Container.DataItem, "ActualDate")).ToShortDateString()%>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn SortExpression="RowId">
								<itemstyle cssclass="ibn-vb2 pp"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
								<itemtemplate>
									<%# ((string)DataBinder.Eval(Container.DataItem, "RowId") == string.Empty)? 
									"<font color='#DB4C2C'>"+LocRM.GetString("tNone")+"</font>" : 
									DataBinder.Eval(Container.DataItem, "RowId")%>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<itemstyle cssclass="ibn-vb2 pp"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
								<itemtemplate>
									<%# GetObjectLink((int)Eval("ObjectTypeId"), (int)Eval("ObjectId"), Eval("BlockId")) %>
									<br />
									<%# Eval("Description") %>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<itemstyle cssclass="ibn-vb2 pp rp" HorizontalAlign="Right"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle rp" HorizontalAlign="Right"></headerstyle>
								<itemtemplate>
									<%# Eval("TotalApproved")%>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn SortExpression="AValue">
								<itemstyle cssclass="ibn-vb2 pp rp" HorizontalAlign="Right"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle rp" HorizontalAlign="Right"></headerstyle>
								<itemtemplate>
									<%# DataBinder.Eval(Container.DataItem, "AValue", "{0:F}")%>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn SortExpression="OwnerName">
								<itemstyle cssclass="ibn-vb2 pp"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
								<itemtemplate>
									<%# Eval("OwnerDisplayName")%>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn SortExpression="LastEditorName">
								<itemstyle cssclass="ibn-vb2 pp"></itemstyle>
								<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
								<itemtemplate>
									<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "LastEditorId"))%>
								</itemtemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<itemstyle horizontalalign="right" cssclass="ibn-vb2 pp" width="52px"></itemstyle>
								<headerstyle cssclass="ibn-vh-right headstyle" width="52px"></headerstyle>
								<itemtemplate>
									<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" 
										width="16" height="16" imageurl="../../layouts/images/edit.gif"
										title='<%#LocRM.GetString("tEdit")%>' 
										causesvalidation="False" ImageAlign="AbsMiddle"/>
									&nbsp;
									<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" 
										width="16" height="16" imageurl="../../layouts/images/delete.gif" 
										commandname="Delete" causesvalidation="False" ImageAlign="AbsMiddle" 
										title='<%#LocRM.GetString("tDelete")%>'
										CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ActualId")%>'/>
								</itemtemplate>
							</asp:TemplateColumn>
						</columns>
					</dg:DataGridExtended>
				</td>
			</tr>
		</table>
		<asp:Button ID="RefreshButton" runat="server" OnClick="RefreshButton_Click" style="display:none;" />
		<asp:DataGrid ID="dgExport" Runat="server" AutoGenerateColumns="False" AllowPaging="False" 
			AllowSorting="False" EnableViewState="False" Visible="False" 
			ItemStyle-HorizontalAlign="Left" HeaderStyle-Font-Bold="True">
			<Columns>
				<asp:BoundColumn DataField="ActualDate" DataFormatString="{0:d}"></asp:BoundColumn>
				<asp:BoundColumn DataField="RowId"></asp:BoundColumn>
				<asp:BoundColumn DataField="Description"></asp:BoundColumn>
				<asp:TemplateColumn>
					<itemtemplate>
						<%# GetObjectTitle((int)Eval("ObjectTypeId"), (int)Eval("ObjectId"), Eval("BlockId"))%>
					</itemtemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn DataField="TotalApproved" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
				<asp:BoundColumn DataField="AValue" DataFormatString="{0:F}" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
				<asp:BoundColumn DataField="OwnerName"></asp:BoundColumn>
			</Columns>
		</asp:DataGrid>
		<asp:LinkButton ID="ExportButton" runat="server" Visible="false" OnClick="ExportButton_Click"></asp:LinkButton>
	</ContentTemplate>
</asp:UpdatePanel>
