<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.TaskPredecessors" Codebehind="TaskPredecessors.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP:5px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server"/>
		</td>
	</tr>
</table>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox-light">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgPredecessors" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn Visible="False" DataField="TaskId" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="Lag" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="LinkId" ReadOnly="True"></asp:BoundColumn>
					<asp:BoundColumn DataField="TaskNum" ReadOnly="True" HeaderStyle-Width="30px">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2 "></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink(
								(int)DataBinder.Eval(Container.DataItem, "TaskId"),
								0,
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId")
							)%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="FinishDate" DataFormatString="{0:d}" ReadOnly="True" HeaderStyle-Width="150px">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-Width="150px">
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# FormatTaskDuration((int)DataBinder.Eval(Container.DataItem, "Lag")) %>
						</ItemTemplate>
						<EditItemTemplate>
							<table border="0" cellpadding="0" cellspacing="0" class="text">
								<tr>
									<td width="20">
										<%# LocRM.GetString("Hours") %>:</td>
									<td width="50">
										<asp:TextBox Runat="server" CssClass="text" Width="35" ID="tbH" MaxLength="4" Font-Size="10px"></asp:TextBox><asp:RequiredFieldValidator Runat="server" CssClass="text" ControlToValidate="tbH" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
										<asp:RangeValidator Runat="server" CssClass="text" ControlToValidate="tbH" Display="Dynamic" ErrorMessage="*" ID="Requiredfieldvalidator2" MinimumValue="-9999" MaximumValue="9999" Type="Integer"/>
									</td>
									<td width="20">
										<%# LocRM.GetString("Minutes") %>:</td>
									<td width="30">
										<asp:TextBox Runat="server" CssClass="text" Width="20" ID="tbMin" MaxLength="2" Font-Size="10px"></asp:TextBox><asp:RequiredFieldValidator Runat="server" CssClass="text" ControlToValidate="tbMin" Display="Dynamic" ErrorMessage="*" ID="Requiredfieldvalidator1"></asp:RequiredFieldValidator><asp:RangeValidator Runat="server" CssClass="text" ControlToValidate="tbMin" Display="Dynamic" ErrorMessage="*" ID="Rangevalidator1" MinimumValue="0" MaximumValue="59" Type="Integer"/>
									</td>
								</tr>
							</table>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="55px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="55px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/Edit.GIF" title='<%# LocRM.GetString("tEdit")%>' commandname="Edit" causesvalidation="False" Visible='<%# !isMSProject%>'></asp:imagebutton>
							&nbsp;
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" title='<%# LocRM.GetString("Delete")%>' commandname="Delete" causesvalidation="False" Visible='<%# !isMSProject%>'></asp:imagebutton>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("tSave")%>' imageurl="../../layouts/images/SaveItem.gif" commandname="Update" causesvalidation="True" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "TaskId")%>'/>
							&nbsp;
							<asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" width="16" height="16" title='<%# LocRM.GetString("Cancel")%>' imageurl="../../layouts/images/cancel.gif" commandname="Cancel" causesvalidation="False" />
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
