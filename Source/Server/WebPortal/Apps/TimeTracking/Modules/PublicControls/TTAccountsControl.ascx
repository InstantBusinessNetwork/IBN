<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TTAccountsControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.TTAccountsControl" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<style type="text/css">
	.cellstyle {height:22px; padding-right:5px;}
</style>
<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td class="ibn-alternating ibn-propertysheet ibn-navline" style="PADDING:7px">
			<table cellpadding="0" cellspacing="5" width="100%">
				<tr>
					<td style="width:100px;" class="ibn-label"><%=LocRM.GetString("Week")%>:</td>
					<td style="width:30%;" class="ibn-value"><asp:Label Runat="server" ID="lblWeek"></asp:Label></td>
					<td style="width:100px;" class="ibn-label"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, Finances%>" />:</td>
					<td><asp:Label Runat="server" ID="lblFinances"></asp:Label></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("Project")%>:</td>
					<td class="ibn-value"><asp:Label Runat="server" ID="lblProject"></asp:Label></td>
					<td></td>
					<td></td>
				</tr>
				<tr>
					<td class="ibn-label"><%=LocRM.GetString("tUser")%>:</td>
					<td class="ibn-value"><asp:Label Runat="server" ID="lblUser"></asp:Label></td>
					<td></td>
					<td>
						<asp:Button runat="server" ID="btnRegister" Text="<%$Resources : IbnFramework.TimeTracking, Register%>" OnClick="btnRegister_Click" />
						<asp:Button runat="server" ID="btnUnregister" Text="<%$Resources : IbnFramework.TimeTracking, Unregister%>" OnClick="btnUnregister_Click" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="PADDING-BOTTOM:5px" class="ibn-propertysheet" valign="top">
			<asp:DataGrid id="dgTimesheet" runat="server" AllowPaging="False" 
			cellpadding="0" gridlines="none" CellSpacing="0" borderwidth="0px" 
			autogeneratecolumns="False" width="100%" 
			OnCancelCommand="dgTimesheet_CancelCommand" 
			OnEditCommand="dgTimesheet_EditCommand" 
			OnUpdateCommand="dgTimesheet_UpdateCommand" ItemStyle-Height="20px">
				<columns>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle"></itemstyle>
						<ItemTemplate>
							<%# Eval("Title") %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh-right"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle" HorizontalAlign="right"></itemstyle>
						<ItemTemplate>
							<%# (Eval("TaskTime") == DBNull.Value)
								? "" 
								: Mediachase.UI.Web.Util.CommonHelper.GetHours((int)Eval("TaskTime"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh-right"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle" HorizontalAlign="right"></itemstyle>
						<ItemTemplate>
							<%# GetPostedTime(Eval("ObjectId"), Eval("ObjectTypeId"), (int)Eval("TimeTrackingEntryId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="65"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle"></itemstyle>
						<ItemTemplate>
							<%# GetTotalString(
								DataBinder.Eval(Container.DataItem,"Total"),
								DataBinder.Eval(Container.DataItem,"Day1"),
								DataBinder.Eval(Container.DataItem,"Day2"),
								DataBinder.Eval(Container.DataItem,"Day3"),
								DataBinder.Eval(Container.DataItem,"Day4"),
								DataBinder.Eval(Container.DataItem,"Day5"),
								DataBinder.Eval(Container.DataItem,"Day6"),
								DataBinder.Eval(Container.DataItem,"Day7"))
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh-right"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle" HorizontalAlign="right"></itemstyle>
						<ItemTemplate>
							<span class='<%# ((double)Eval("TotalApproved") == (double)Eval("Total")) ?	"ibn-propertysheet" : "ibn-propertysheet ibn-error" %>'>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Eval("TotalApproved"))%>
							</span>
						</ItemTemplate>
						<EditItemTemplate>
							<ibn:Time id="dtc" Value='<%# GetDateTimeFromMinutes((int)(double)Eval("TotalApproved")) %>' ShowTime="HM" HourSpinMaxValue="168" ViewStartDate="True" runat="server" />
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh-right"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle" HorizontalAlign="right"></itemstyle>
						<ItemTemplate>
							<span class='<%# ((decimal)Eval("Rate") == ProjectRate) ?	"ibn-propertysheet" : "ibn-propertysheet ibn-error" %>'>
							<%# ((decimal)Eval("Rate")).ToString("f") %>
							</span>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox CssClass ="ibn-propertysheet" Runat="server" ID="txtRate" Text='<%# ((decimal)Eval("Rate")).ToString("f") %>' Width="40" style="text-align:right;">
							</asp:TextBox><asp:RequiredFieldValidator ErrorMessage="*" Display="Dynamic" ControlToValidate="txtRate" Runat="server" ID="rfRate"></asp:RequiredFieldValidator>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh-right"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle" HorizontalAlign="right"></itemstyle>
						<ItemTemplate>
							<%# ((double)Eval("Cost")).ToString("f")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="50px"></headerstyle>
						<itemstyle cssclass="ibn-vb2 cellstyle" HorizontalAlign="Right" ></itemstyle>
						<ItemTemplate>
							<asp:imagebutton id="ibEdit" runat="server" borderwidth="0" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False"></asp:imagebutton>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:imagebutton id="ibSave" runat="server" borderwidth="0" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
							</asp:imagebutton>&nbsp;
							<asp:imagebutton id="ibCancel" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRM.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
							</asp:imagebutton></nobr>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:boundcolumn visible="False" datafield="TimeTrackingEntryId" ReadOnly="True"></asp:boundcolumn>
				</columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>