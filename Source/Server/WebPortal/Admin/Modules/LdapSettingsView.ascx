<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.LdapSettingsView" Codebehind="LdapSettingsView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<script type="text/javascript">
	function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
	
	function ChangeLdap(obj)
	{
		var inputColl = document.getElementsByTagName("input");
		for(var i=0;i<inputColl.length;i++)
		{
			var objtxt = inputColl[i];
			if(objtxt.id.indexOf("txtLdapName")>=0)
			{
				objtxt.value = obj.value;
				break;
			}
		}
	}
</script>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" class="text">
				<tr>
					<td><b><%= LocRM.GetString("tTitle")%>:</b></td>
					<td width="300px"><asp:Label ID="lblTitle" Runat="server"></asp:Label></td>
					<td><b><%= LocRM.GetString("tDomain")%>:</b></td>
					<td><asp:Label ID="lblDomain" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%= LocRM.GetString("tUserName")%>:</b></td>
					<td><asp:Label ID="lblUser" Runat="server"></asp:Label></td>
					<td><b><%= LocRM.GetString("tFilter")%>:</b></td>
					<td><asp:Label ID="lblFilter" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%=Mediachase.UI.Web.Util.CommonHelper.ProductFamilyShort%> Key:</b></td>
					<td><asp:Label ID="lblIBN" Runat="server"></asp:Label></td>
					<td><b>LDAP Key:</b></td>
					<td><asp:Label ID="lblLDAP" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td></td>
					<td><asp:Label ID="lblActivated" Runat="server"></asp:Label></td>
					<td></td>
					<td><asp:Label ID="lblDeactivated" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td></td>
					<td><asp:Label ID="lblAutosinc" Runat="server"></asp:Label></td>
					<td><b><%=LocRM.GetString("tLastSynch")%>:</b></td>
					<td><asp:Label ID="lblLastSynch" Runat="server"></asp:Label></td>
				</tr>
				<tr id="trAuto" runat="server">
					<td><b><%= LocRM.GetString("tStart")%>:</b></td>
					<td><asp:Label ID="lblStart" Runat="server"></asp:Label></td>
					<td><b><%= LocRM.GetString("tInterval")%>:</b></td>
					<td><asp:Label ID="lblInterval" Runat="server"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding: 5px">
			<asp:DataGrid Runat="server" ID="dgFields" AutoGenerateColumns="False" 
				AllowPaging="False" AllowSorting="False" cellpadding="5" 
				gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" 
				ShowHeader="True">
				<Columns>
					<asp:BoundColumn DataField="FieldId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="50px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="50px"/>
						<ItemTemplate>
							<%# GetIsBit((bool)DataBinder.Eval(Container.DataItem, "IsBit"))%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:CheckBox CssClass="text" ID="cbIsBit" Runat="server" Checked='<%# (bool)DataBinder.Eval(Container.DataItem, "IsBit")%>'></asp:CheckBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "IbnName")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="ddIbnName" Runat="server" Width="90%"></asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2"/>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "LdapName")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="ddLdapName" Width="50%" Runat="server"></asp:DropDownList>
							<asp:TextBox ID="txtLdapName" Width="45%" CssClass="text" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LdapName")%>'></asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="210px"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh2" Width="210px"/>
						<ItemTemplate>
							<%# GetSets((bool)DataBinder.Eval(Container.DataItem, "IsBit"),
									DataBinder.Eval(Container.DataItem, "BitMask"),
									DataBinder.Eval(Container.DataItem, "Equal"),
									DataBinder.Eval(Container.DataItem, "CompareTo"))
							%>
						</ItemTemplate>
						<EditItemTemplate>
							AND&nbsp;
							<asp:TextBox ID="txtBitMask" Runat="server" CssClass="text" Width="40px" Text='<%#DataBinder.Eval(Container.DataItem, "BitMask")%>'></asp:TextBox>
							<asp:DropDownList ID="ddEqual" Runat="server" Width="80px">
								<asp:ListItem Value="false">Not Equal</asp:ListItem>
								<asp:ListItem Value="true">Equal</asp:ListItem>
							</asp:DropDownList>
							<asp:TextBox ID="txtCompare" Runat="server" CssClass="text" Width="40px" Text='<%#DataBinder.Eval(Container.DataItem, "CompareTo")%>'></asp:TextBox>
							<asp:Label Visible="False" ID="lblError" Text="*" ForeColor="#ff0000" Runat="server" CssClass="text"></asp:Label>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle HorizontalAlign=Right CssClass="ibn-vb2" Width="52"></ItemStyle>
						<HeaderStyle CssClass="ibn-vh-right" Width="52" />
						<HeaderTemplate>
							<asp:ImageButton ImageAlign="AbsMiddle" ID="ibNew" Runat="server" BorderWidth="0" ImageUrl='<%# ResolveUrl("~/layouts/images/newitem.gif")%>' CommandName="NewField" CausesValidation="False"></asp:ImageButton>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:ImageButton ImageAlign="AbsMiddle" ID="ibEdit" Runat="server" BorderWidth="0" ImageUrl="~/layouts/images/edit.gif" CommandName="Edit" CausesValidation="False">
							</asp:ImageButton>&nbsp;
							<asp:imagebutton ImageAlign="AbsMiddle" id="ibDelete" runat="server" borderwidth="0" imageurl="~/layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:ImageButton ImageAlign="AbsMiddle" ID="ibSave" Runat="server" BorderWidth="0" ImageUrl="~/layouts/images/saveitem.gif" CommandName="Update" CausesValidation="True">
							</asp:ImageButton>&nbsp;
							<asp:imagebutton ImageAlign="AbsMiddle" id="ibCancel" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbSynch" Runat="server" Visible="False"></asp:LinkButton>