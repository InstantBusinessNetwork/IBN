<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.FinanceActualList" Codebehind="FinanceActualList.ascx.cs" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<STYLE type=text/css>
	.pp { PADDING-RIGHT: 3px; PADDING-LEFT: 3px;Height:21px; BORDER-BOTTOM: #e4e4e4 1px solid }
	.headstyle {padding-top:5px;padding-bottom:5px; border-bottom:1px solid #e4e4e4}
</STYLE>
<script language="javascript">
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
</script>
<table cellpadding="0" cellspacing="0" width="100%" border="0" style="OVERFLOW:hidden" align="left" >
	<tr>
		<td valign="top" style="padding-top:5px">
			<dg:DataGridExtended Runat="server" ID="dgAccounts" allowpaging="True" allowsorting="False" cellpadding="0" gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" pagesize="10" LayoutFixed="false">
				<columns>
					<asp:boundcolumn datafield="ActualId" Visible="False"></asp:boundcolumn>
					<asp:boundcolumn datafield="AccountId" Visible="False"></asp:boundcolumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 pp"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
						<itemtemplate>
							<%# DataBinder.Eval(Container.DataItem, "Description") %>
						</itemtemplate>
						<edititemtemplate>
							<asp:TextBox ID="tbDescr" Runat=server CssClass="text" Width="90%" Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:TextBox>
						</edititemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 pp"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle"></headerstyle>
						<itemtemplate>
							<%# ((int)DataBinder.Eval(Container.DataItem, "OutlineLevel")>1) ?
								DataBinder.Eval(Container.DataItem, "Title").ToString() :
								LocRM.GetString("tRoot")
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 pp" width="100px"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle" width="100px"></headerstyle>
						<itemtemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "ActualDate")).ToShortDateString()%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 pp" width="80px"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle" width="80px"></headerstyle>
						<itemtemplate>
							<%# ((decimal)DataBinder.Eval(Container.DataItem, "AValue")).ToString("f")%>
						</itemtemplate>
						<edititemtemplate>
							<asp:TextBox ID="tbValue" Runat=server Text='<%# ((decimal)DataBinder.Eval(Container.DataItem, "AValue")).ToString("f")%>' CssClass="text" Width="55px"></asp:TextBox>
							<asp:CompareValidator ID="cvVal" Runat=server ErrorMessage="*" CssClass="text" ControlToValidate="tbValue" ValueToCompare="0" Type=Currency Operator=GreaterThanEqual Display=Dynamic></asp:CompareValidator>
						</edititemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 pp" width="150px"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle" width="150px"></headerstyle>
						<itemtemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "LastEditorId"))%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle cssclass="ibn-vb2 pp" width="120px"></itemstyle>
						<headerstyle cssclass="ibn-vh2 headstyle" width="120px"></headerstyle>
						<itemtemplate>
							<%# GetObjectLink(
								(int)DataBinder.Eval(Container.DataItem, "ObjectTypeId"),
								(int)DataBinder.Eval(Container.DataItem, "ObjectId"),
								DataBinder.Eval(Container.DataItem, "ObjectTitle").ToString())
							%>
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<itemstyle horizontalalign=right cssclass="ibn-vb2 pp" width="52px"></itemstyle>
						<headerstyle cssclass="ibn-vh-right headstyle" width="52px"></headerstyle>
						<itemtemplate>
							<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" 
								width="16" height="16" imageurl="../../layouts/images/edit.gif"
								title='<%#LocRM.GetString("tEdit")%>' 
								commandname="Edit" causesvalidation="False" ImageAlign="AbsMiddle"/>
							&nbsp;
							<asp:imagebutton 
								id="ibDelete" runat="server" borderwidth="0" 
								width="16" height="16" imageurl="../../layouts/images/delete.gif" 
								commandname="Delete" causesvalidation="False" ImageAlign="AbsMiddle" 
								title='<%#LocRM.GetString("tDelete")%>'
								CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ActualId")%>'/>
						</itemtemplate>
						<edititemtemplate>
							<asp:imagebutton id="ibSave" runat="server" borderwidth="0" 
								width="16" height="16" imageurl="../../layouts/images/saveitem.gif" 
								title='<%#LocRM.GetString("tSave")%>'
								commandname="Update" ImageAlign="AbsMiddle"/>
							&nbsp;
							<asp:imagebutton id="ibCancel" runat="server" borderwidth="0" 
								width="16" height="16" imageurl="../../layouts/images/cancel.gif" 
								title='<%#LocRM.GetString("tCancel")%>'
								commandname="Cancel" causesvalidation="False" ImageAlign="AbsMiddle" />
						</edititemtemplate>
					</asp:TemplateColumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>