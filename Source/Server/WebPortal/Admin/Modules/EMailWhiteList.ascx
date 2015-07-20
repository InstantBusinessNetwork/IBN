<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailWhiteList" Codebehind="EMailWhiteList.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
function OpenWindow(query,w,h,scroll)
		{
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			if (scroll) winprops+=',scrollbars=1';
			var f = window.open(query, "_blank", winprops);
		}

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
</script>
<table class="ibn-stylebox text" style="MARGIN-TOP: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" title="White List" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" width="100%" cellspacing="0" border="0" class="ibn-alternating ibn-navline text">
				<tr>
					<td>
						<b><%=LocRM.GetString("tKeyword")%>:</b>&nbsp;&nbsp;
						<asp:TextBox CssClass="text" ID="txtSearch" Runat=server Width="200px"></asp:TextBox>&nbsp;&nbsp;
						<asp:Button CssClass="text" ID="btnSearch" Runat="server"></asp:Button>&nbsp;&nbsp;
						<asp:Button CssClass="text" ID="btnReset" Runat="server"></asp:Button>
					</td>
					<td align="right">
						<b><%=LocRM.GetString("tQuickAdd")%>:</b>&nbsp;&nbsp;
						<asp:TextBox ID="txtAdd" Runat="server" CssClass="text" Width="200px"></asp:TextBox>&nbsp;&nbsp;
						<asp:Button ID="btnAdd" Runat="server" CssClass="text"></asp:Button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<dg:DataGridExtended id="dgWhiteList" runat="server" allowpaging="True" 
				pagesize="10" allowsorting="False" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" LayoutFixed="True">
				<columns>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<HeaderTemplate>
							<asp:checkbox runat="server" id="chkAll" onclick="CheckAll(this);"></asp:checkbox>
						</HeaderTemplate>
						<itemtemplate>
							<input value='<%# DataBinder.Eval(Container.DataItem, "Id").ToString()%>' type="checkbox" runat="server" id="chkItem" NAME="chkItem"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn HeaderStyle-Width="50px" DataField="Id" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:boundcolumn>
					<asp:templatecolumn sortexpression="From">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<a href='mailto:<%# DataBinder.Eval(Container.DataItem, "Address")%>'><%# DataBinder.Eval(Container.DataItem, "Address")%></a>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh-right" Width="24px"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<asp:ImageButton ID="ibDelete" Runat=server CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<input type=hidden id="hidForDelete" runat=server NAME="hidForDelete"/>
<asp:LinkButton ID="lbDeleteChecked" Runat=server Visible=False></asp:LinkButton>