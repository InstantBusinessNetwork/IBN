<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.LdapLogs" Codebehind="LdapLogs.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script language=javascript>
	function CheckAll(obj)
	{
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0)
			{
				oInput.checked = obj.checked;
			}
		}
	}
	function ActionDelete()
	{
		var Ids = "";
		
		aInputs = document.getElementsByTagName("input");
		for (var i=0; i<aInputs.length; i++)
		{
			oInput = aInputs[i];
			if(oInput.type == "checkbox" && oInput.name.indexOf("chkItem") >= 0 && oInput.checked)
			{
				var str = oInput.value;
				Ids += str+",";
			}
		}
		if(Ids!="" && confirm('<%=LocRM.GetString("tWarningChecked")%>'))
		{
			document.forms[0].<%= hidForDelete.ClientID%>.value = Ids;
			<%= Page.ClientScript.GetPostBackEventReference(lbDeleteChecked, "")%>
		}
	}
	
	function ActionDelete2()
	{
		if(confirm('<%=LocRM.GetString("tWarningEmpty")%>'))
			<%= Page.ClientScript.GetPostBackEventReference(lbDeleteEmpty, "")%>
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<dg:DataGridExtended id="dgLogs" runat="server" allowpaging="True" 
				pagesize="10" allowsorting="True" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" LayoutFixed="True">
				<Columns>
					<asp:BoundColumn DataField="LogId" Visible="False"></asp:BoundColumn>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="27"
						itemstyle-width="27">
						<HeaderTemplate>
							<asp:checkbox runat="server" id="chkAll" onclick="CheckAll(this);"></asp:checkbox>
						</HeaderTemplate>
						<itemtemplate>
							<input value='<%# DataBinder.Eval(Container.DataItem, "LogId").ToString()%>' type="checkbox" runat="server" id="chkItem" NAME="chkItem"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:TemplateColumn SortExpression="Dt">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<a href='<%# ResolveClientUrl("~/Admin/LdapLogView.aspx") %>?LogId=<%# DataBinder.Eval(Container.DataItem, "LogId")%>'><%# GetDate(DataBinder.Eval(Container.DataItem, "Dt"))%></a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="Title">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<a href='LdapSettingsView.aspx?SetId=<%#DataBinder.Eval(Container.DataItem, "LdapId")%>'><%# DataBinder.Eval(Container.DataItem, "Title")%></a>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn SortExpression="UserCount">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "UserCount")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle HorizontalAlign=Right CssClass="ibn-vb2" Width="52"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="52" />
						<ItemTemplate>
							<a href='<%# ResolveClientUrl("~/Admin/LdapLogView.aspx") %>?LogId=<%# DataBinder.Eval(Container.DataItem, "LogId")%>'><img align="absmiddle" border="0" src='<%# ResolveClientUrl("~/layouts/images/icon-search.gif") %>' /></a>&nbsp;
							<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<input type=hidden id="hidForDelete" runat=server NAME="hidForDelete"/>
<asp:LinkButton ID="lbDeleteChecked" Runat=server Visible=False></asp:LinkButton>
<asp:LinkButton ID="lbDeleteEmpty" Runat=server Visible=False></asp:LinkButton>