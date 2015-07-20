<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailPendingMessages" Codebehind="EMailPendingMessages.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
	function OpenMessage(sId)
	{
		var w = 750;
		var h = 550;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=1,,scrollbars=0, height='+h+',width='+w+',top='+t+',left='+l;
		var f = window.open('<%= ResolveClientUrl("~/Incidents/EMailView.aspx") %>?EMailId='+sId, "EMailView", winprops);
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
	function ActionChecked(str1)
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
		var mes = "";
		if(str1=="delete")
			mes = '<%=LocRM.GetString("tWarningCheckedDelete")%>';
		else
			mes = '<%=LocRM.GetString("tWarningCheckedApprove")%>';
		if(Ids!="" && confirm(mes))
		{
			document.forms[0].<%= hidForDelete.ClientID%>.value = Ids;
			if(str1=="delete")
				<%= Page.ClientScript.GetPostBackEventReference(lbDeleteChecked, "")%>
			else
				<%= Page.ClientScript.GetPostBackEventReference(lbApproveChecked, "")%>
		}
	}
	
	function AddChecked(str1)
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
		
		if(Ids!="")
		{
			var re = /xxxsel/g;
			var r = str1.replace(re, Ids); 
			OpenPopUpNoScrollWindow('<%= ResolveClientUrl("~/Common/SelectIncident.aspx") %>?btn=' + r, 640, 480);
		}
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<dg:DataGridExtended id="dgPendMess" runat="server" allowpaging="True" 
				pagesize="10" allowsorting="True" cellpadding="0" gridlines="None" 
				CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" LayoutFixed="True">
				<columns>
					<asp:templatecolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="25"
						itemstyle-width="25">
						<HeaderTemplate>
							<asp:checkbox runat="server" id="chkAll" onclick="CheckAll(this);"></asp:checkbox>
						</HeaderTemplate>
						<itemtemplate>
							<input value='<%# DataBinder.Eval(Container.DataItem, "PendingMessageId").ToString()%>' type="checkbox" runat="server" id="chkItem"  NAME="chkItem"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn HeaderStyle-Width="50px" SortExpression="PendingMessageId" DataField="PendingMessageId" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2"></asp:boundcolumn>
					<asp:templatecolumn sortexpression="Subject">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True></itemstyle>
						<itemtemplate>
							<%# GetSubjectLink(DataBinder.Eval(Container.DataItem, "PendingMessageId").ToString(), DataBinder.Eval(Container.DataItem, "Subject").ToString()) %>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="From">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# "<a href='mailto:" + DataBinder.Eval(Container.DataItem, "From").ToString() + "'>" + 
									DataBinder.Eval(Container.DataItem, "From").ToString() + "</a>"%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="To">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2"></itemstyle>
						<itemtemplate>
							<%# "<a href='mailto:" + DataBinder.Eval(Container.DataItem, "To").ToString() + "'>" + 
									DataBinder.Eval(Container.DataItem, "To").ToString() + "</a>"%>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn sortexpression="Created">
						<headerstyle CssClass="ibn-vh2"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Wrap=True></itemstyle>
						<itemtemplate>
							<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Created")).ToString("g")%>
						</itemtemplate>
					</asp:templatecolumn>
					
					<asp:templatecolumn>
						<headerstyle CssClass="ibn-vh-right" Width="90px"></headerstyle>
						<itemstyle CssClass="ibn-vb2" Width="90px"></itemstyle>
						<itemtemplate>
							<%# "<a href='" + ResolveClientUrl("~/Incidents/IncidentEdit.aspx") + "?EMailMessageId="+DataBinder.Eval(Container.DataItem, "PendingMessageId").ToString()+ "'><img width='16' height='16' border='0' title='"+LocRM.GetString("tMessView")+"' align='top' src='"+
							ResolveUrl("~/Layouts/Images/icon-search.gif")+
							"'/></a>&nbsp;"%>
							<%# String.Format("<a href=\"javascript:OpenPopUpNoScrollWindow(&quot;{3}?btn={0}&quot;, 640, 480);\"><img width='16' height='16' border='0' src='{1}' title='{2}' alt=''/></a>",
										Page.ClientScript.GetPostBackEventReference(btnAddToExIss, DataBinder.Eval(Container.DataItem, "PendingMessageId").ToString() + ";xxxtypeid;xxxid"),
										ResolveClientUrl("~/Layouts/Images/icons/incident_plus.gif"), 
										LocRM.GetString("tAddToExistIss"),
										ResolveClientUrl("~/Common/SelectIncident.aspx")					
								)
							%>
							<asp:ImageButton ID="ibApprove" Runat=server CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PendingMessageId")%>' CommandName="Approve" ImageUrl="~/layouts/Images/icon-key.gif"></asp:ImageButton>&nbsp;
							<asp:ImageButton ID="ibDelete" Runat=server CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PendingMessageId")%>' CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif"></asp:ImageButton>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</dg:DataGridExtended>
		</td>
	</tr>
</table>
<input type=hidden id="hidForDelete" runat=server NAME="hidForDelete"/>
<asp:Button ID="btnAddToExIss" Runat="server" Visible="False" />
<asp:LinkButton ID="lbDeleteChecked" Runat=server Visible=False></asp:LinkButton>
<asp:LinkButton ID="lbApproveChecked" Runat=server Visible=False></asp:LinkButton>