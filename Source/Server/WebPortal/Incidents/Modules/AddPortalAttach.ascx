<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddPortalAttach.ascx.cs" Inherits="Mediachase.UI.Web.Incidents.Modules.AddPortalAttach" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" Src="~/Modules/ObjectDropDownNew.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr id="pathTr">
		<td class="ibn-navline text" style="padding: 2px 2px 2px 8px;">
			<table cellpadding="0" cellspacing="0" class="ibn-propertysheet">
				<tr>
					<td style="padding: 3px;">
						<asp:Label ID="lblProject" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td style="padding: 3px;">
						<ibn:ObjectDD ID="ucProject" OnChange="ChangeProject()" ObjectTypes="3" runat="server"
							Width="300px" ItemCount="7" PlaceName="IncidentView" ViewName="" ClassName="Incident"
							CommandName="MC_HDM_PM_ObjectDD" />
					</td>
				</tr>
				<tr>
					<td style="padding: 3px;">
						<asp:Label ID="lblPathTitle" CssClass="text" runat="server"></asp:Label>:
					</td>
					<td style="padding: 3px;">
						<asp:Label ID="lblPath" runat="server" CssClass="ibn-nav"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<div id="tableDiv" style="height: 450px; width: 640px; overflow-y: auto; overflow-x: auto;
				overflow: auto;">
				<dg:DataGridExtended ID="grdMain" runat="server" AllowSorting="True" AllowPaging="True"
					Width="97%" AutoGenerateColumns="False" BorderWidth="0" GridLines="None" CellPadding="1"
					PageSize="10" LayoutFixed="True" EnableViewState="false">
					<Columns>
						<asp:BoundColumn Visible="false" DataField="Id"></asp:BoundColumn>
						<asp:BoundColumn Visible="false" DataField="Weight"></asp:BoundColumn>
						<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="25"
							ItemStyle-Width="25">
							<HeaderTemplate>
								<asp:CheckBox runat="server" ID="chkAll" onclick="CheckAll(this);"></asp:CheckBox>
							</HeaderTemplate>
							<ItemTemplate>
								<input value='<%#DataBinder.Eval(Container.DataItem, "Id").ToString()%>' type="checkbox"
									runat="server" id="chkItem" visible='<%# (int)DataBinder.Eval(Container.DataItem, "Weight")>1%>' />
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="21"
							ItemStyle-Width="21">
							<ItemTemplate>
								<img src='<%# DataBinder.Eval(Container.DataItem, "Icon")%>' width="16" height="16">
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="Name"
							SortExpression="sortName"></asp:BoundColumn>
						<asp:TemplateColumn>
							<ItemStyle CssClass="ibn-vb2" Width="25px" />
							<HeaderStyle CssClass="ibn-vh2" Width="25px" />
							<HeaderTemplate>
								<img src='../Layouts/Images/selectall.gif' title='<%# LocRM.GetString("tAddSelected") %>'
									style="cursor: pointer;" width="16" height="16" onclick='SelectChecked();' />
							</HeaderTemplate>
							<ItemTemplate>
								<asp:ImageButton CommandName="Select" title='<%# LocRM.GetString("tAddFile") %>'
									ID="ibRelate" runat="server" ImageUrl="~/Layouts/Images/Select.gif" Width="16"
									Height="16" Visible='<%# (int)DataBinder.Eval(Container.DataItem, "Weight")>1%>'>
								</asp:ImageButton>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</dg:DataGridExtended>
			</div>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbSelectChecked" runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbChangeProject" runat="server" OnClick="lbChangeProject_Click" Style="display: none;"></asp:LinkButton>
<input type="hidden" id="hidForSelect" runat="server" />

<script type="text/javascript"> 
	//<![CDATA[
	window.onresize = ResizeAttachForm;
	window.setTimeout("ResizeAttachForm()", 100);
	function SelectChecked()
	{
		var Ids = GetSelectedCbString();
		if(Ids!="")
		{
			document.forms[0].<%= hidForSelect.ClientID%>.value = Ids;
			<%= Page.ClientScript.GetPostBackEventReference(lbSelectChecked, "")%>
		}
	}
	function CloseAll(str)
	{
	  window.opener.top.updateAttachments2(str);
	  window.close();
	}
	function ChangeProject()
	{
		<%= Page.ClientScript.GetPostBackEventReference(lbChangeProject, "")%>
	}
	//]]>
</script>

