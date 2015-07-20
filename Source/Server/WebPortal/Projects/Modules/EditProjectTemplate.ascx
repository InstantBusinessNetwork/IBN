<%@ Reference Control="~/Wizards/Modules/WizardTemplate.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.EditProjectTemplate" Codebehind="EditProjectTemplate.ascx.cs" %>
<style type="text/css">
	.hClass { 
		font-family: verdana; 
		color: #808080;
		text-align: left; 
		text-decoration: none; 
		font-weight: bold; 
		vertical-align: top;}
	.bClass { 
		font-family: verdana; 
		text-decoration: none; 
		font-weight: normal; 
		vertical-align: top;
		border-top: 1px solid #9e9e9e;}
</style>
<asp:panel id="step1" Runat="server" Visible="False" Height="100%" HorizontalAlign="Center">
	<div class="text" style="PADDING-LEFT:20px"><%=LocRM.GetString("s1TopDiv") %></div><br>
	<table>
		<tr>
			<td valign=top width="450px">
			<FIELDSET style="HEIGHT: 40px;margin:0;padding:2px; width:460px;">
				<LEGEND class="text ibn-legend-greyblack" id="lgdTitle" runat="server"></LEGEND>
				<table style="TABLE-LAYOUT:fixed;" Width="99%" CellSpacing=0 cellpadding="2" border="0">
					<tr>
						<td align=center>
							<asp:TextBox ID="tbTitle" Runat="server" Width="70%" CssClass="text"></asp:TextBox>
						</td>
					</tr>
				</table>
			</fieldset><br><br>
			<FIELDSET style="HEIGHT: 250px;margin:0;padding:2px">
				<LEGEND class="text ibn-legend-greyblack" id="lgdRolesAssigning" runat="server"></LEGEND>
				<div style="PADDING-TOP:10px"></div>
				<table style="TABLE-LAYOUT:fixed;" Width="99%" CellSpacing=0 cellpadding="2" border="0">
					<tr>
						<td Width="170px" class="hClass"><%=LocRM.GetString("tResource")%></td>
						<td Width="280px" class="hClass"><%=LocRM.GetString("tRole")%></td>
						<td class="hClass"></td>
					</tr>
				</table>
				<div style="WIDTH: 100%; OVERFLOW-Y: auto; HEIGHT: 210px; PADDING-BOTTOM:20px">
				<asp:datagrid id="dgRoles" Runat="server" AutoGenerateColumns="False" AllowSorting="True" 
					AllowPaging=False cellpadding="3" gridlines="Horizontal" CellSpacing="0" 
					borderwidth="0px" Width="99%" ShowHeader="False" style="table-layout:fixed">
					<Columns>
						<asp:TemplateColumn HeaderText="Resource">
							<HeaderStyle CssClass="hClass" Width="170px"></HeaderStyle>
							<ItemStyle CssClass="bClass" Width="170px"></ItemStyle>
							<ItemTemplate>
								<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem,"PrincipalId")) %>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Role">
							<HeaderStyle CssClass="hClass" Width="280px"></HeaderStyle>
							<ItemStyle HorizontalAlign=Left CssClass="bClass" Width="280px"></ItemStyle>
							<ItemTemplate>
								<asp:TextBox ID="tbRole" Runat=server Width="90%" CssClass="text">
								</asp:TextBox>
								<asp:RequiredFieldValidator ID="rfRole" Runat=server CssClass="text" ErrorMessage="*" Display=Dynamic ControlToValidate="tbRole"></asp:RequiredFieldValidator>
 							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:boundcolumn datafield="PrincipalId" Visible="False"></asp:boundcolumn>
					</Columns>
				</asp:DataGrid>
				</div>
			</fieldset>
			<asp:CheckBox ID="cbOnlyForMe" Runat=server CssClass="text"></asp:CheckBox>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step2" Runat="server" Height="100%" Visible="False"></asp:panel>
