<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.EditProjectTemplate2" Codebehind="EditProjectTemplate2.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="../../Modules/BlockHeaderLightWithMenu.ascx" %>
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
	.ibn-stylebox-light {
	font-family: verdana; 
	border-left:1px solid #95b7f3;
	border-right:1px solid #95b7f3;
	border-bottom:1px solid #95b7f3;
	text-align:center;}
</style>
<asp:panel id="step1" Runat="server" Visible="False" Height="100%" HorizontalAlign="Center">
	<!-- <div class="text" style="PADDING-LEFT:20px"><%=LocRM.GetString("step1TopDiv") %></div><br>-->
	<table align="center">
		<tr><td align="left"><asp:CheckBox Runat="server" ID="cbImportMeta"></asp:CheckBox></td></tr>
		<tr><td align="left"><asp:CheckBox Runat="server" ID="cbImportSystem"></asp:CheckBox></td></tr>
	</table>
	
	<table align="center">
		<tr>
			<td valign="top" width="550px">
			<ibn:BlockHeaderLight id="hdrTitle" runat="server" />
				<table class="ibn-stylebox-light text" width="100%" height="100%" cellspacing="0" cellpadding="2" border="0">
				  <tr>
				    <td>
				      <asp:Panel ScrollBars="Auto" Width="100%" Height="400px" ID="scrollPanel" runat="server">
				      <table cellpadding="3" cellspacing="0" width="100%" border="0">
				        <tr>
						      <th align="right" style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("calendar") %> :</th>
						      <td align="left" valign="top"><asp:Literal Runat="server" ID="txtCalendar"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right;" valign="top"> <%= LocRM2.GetString("ProjectCurrency") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtCurrency"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right;" valign="top"> <%= LocRM2.GetString("type") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtType"></asp:Literal> </td>
					      </tr>
					      <tr>
						      <th style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("goals") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtGoals"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("description") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtDescription"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("deliverables") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtDeliverables"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("scope") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtScope"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("category") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtCategory"></asp:Literal></td>
					      </tr>
					      <tr>
						      <th style="text-align: right; width: 150px;" valign="top"> <%= LocRM2.GetString("ProjectCategory") %> :</th>
						      <td align="left"><asp:Literal Runat="server" ID="txtProjectCategory"></asp:Literal> </td>
					      </tr>
				      </table>
				      </asp:Panel>
				    </td>
				  </tr>
				</table>
			</td>
		</tr>
	</table>
	
	
</asp:panel>
<asp:panel id="step2" Runat="server" Height="100%" Visible="False" HorizontalAlign="Center">
	<asp:CheckBox Runat="server" ID="cbImportRole" AutoPostBack="True"/>
	<table align="center">
		<tr>
			<td valign=top runat="server" id="tdRoles" width="450px">
			<ibn:BlockHeaderLight id="hdrTitle2" runat="server" />
				<table class="ibn-stylebox-light text"  Width="100%" CellSpacing=0 cellpadding="2" border="0">
					<tr style="BORDER-BOTTOM: #dddddd thin solid;">
						<td>
							<asp:CheckBoxList Runat="server" ID="cblRoles"></asp:CheckBoxList>
						</td>
					</tr>
				</table>
			<br><br>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step3" Runat="server" Height="100%" Visible="False">
	<table align="center">
		<tr>
			<td width="450px">
				<ibn:BlockHeaderLight id="hdrTitle3Top" runat="server" />
					<table class="ibn-stylebox-light text" Width="100%" CellSpacing=0 cellpadding="2" border="0">
						<tr>
							<td align=center>
								<asp:TextBox ID="tbTitle" Runat="server" Width="70%" CssClass="text"></asp:TextBox>
							</td>
						</tr>
					</table>
			</td>
		</tr>
		<tr>
			<td valign=top width="450px">
			<ibn:BlockHeaderLight id="hdrTitle3" runat="server" />
				<table class="ibn-stylebox-light text"  Width="100%" CellSpacing=0 cellpadding="2" border="0">
					<tr style="BORDER-BOTTOM: #dddddd thin solid;">
						<td>
							<asp:RadioButtonList runat="server" ID="rblTask" AutoPostBack="True"/>
						</td>
					</tr>
				</table>
				<div style="PADDING-TOP:10px"></div>
				<asp:Panel runat="server" id="divRoles" style="WIDTH: 100%; OVERFLOW-Y: auto; HEIGHT: 210px; PADDING-BOTTOM:20px">
				<table style="TABLE-LAYOUT:fixed;" Width="99%" CellSpacing=0 cellpadding="2" border="0">
					<tr>
						<td Width="170px" class="hClass"><%=LocRM.GetString("tResource")%></td>
						<td Width="280px" class="hClass"><%=LocRM.GetString("tRole")%></td>
						<td class="hClass"></td>
					</tr>
				</table>
				
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
				</asp:Panel>
			<br><br>
			</td>
		</tr>
		<tr><td><asp:CheckBox ID="cbOnlyForMe" Runat=server CssClass="text"></asp:CheckBox></td></tr>
	</table>
	

</asp:panel>
<asp:panel id="step4" Runat="server" Height="100%" Visible="False"></asp:panel>
