<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToDoQuickAdd.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.ToDoQuickAdd" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="DTCC" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<table width="100%" border="0" class="ibn-propertysheet pad5">
	<tr>
		<td style="padding:5px;"><b><%=GetGlobalResourceObject("IbnFramework.Global", "_mc_Title").ToString()%>:</b></td>
		<td style="padding:5px;" colspan="3"><asp:TextBox ID="txtTitle" runat="server" Width="470px" CssClass="text"></asp:TextBox>
		<asp:RequiredFieldValidator ID="rfTitle" runat="server" ControlToValidate="txtTitle" CssClass="text"
			Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td style="padding:5px;"><b><%=GetGlobalResourceObject("IbnFramework.Global", "FinishDate").ToString()%>:</b></td>
		<td colspan="3">
			<table>
				<tr>
					<td style="padding:5px;"><ibn:DTCC ID="dueDate" runat="server" ShowImageButton="false" DateWidth="90px" ShowTime="true" TimeWidth="60px" /></td>
					<td style="padding:5px;" id="td1" runat="server"><b><%=GetGlobalResourceObject("IbnFramework.Global", "_mc_Priority").ToString()%>:</b></td>
					<td style="padding:5px;" id="td2" runat="server"><asp:DropDownList ID="ddPriority" runat="server" CssClass="text" Width="205px"></asp:DropDownList></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding:5px;" valign="top"><b><%=GetGlobalResourceObject("IbnFramework.Global", "_mc_Description").ToString()%>:</b></td>
		<td style="padding:5px;" valign="top" colspan="3"><asp:TextBox ID="txtDescription" runat="server" Width="470px" Height="130px" TextMode="MultiLine"></asp:TextBox></td>
	</tr>
	<tr>
		<td style="padding:5px;" valign="top"><b><%=GetGlobalResourceObject("IbnFramework.Global", "_mc_Resources").ToString()%>:</b></td>
		<td style="padding:5px;" valign="top">
			<asp:UpdatePanel ID="upAvailable" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<div style="padding-bottom: 5px;">
					<ibn:IndentedDropDownList ID="ddGroups" runat="server" Width="210px" AutoPostBack="true"></ibn:IndentedDropDownList>
					</div>
					<div style="padding-bottom: 5px;">
					<asp:TextBox ID="txtSearch" runat="server" CssClass="text" Width="140px"></asp:TextBox>
					<asp:Button ID="btnSearch" runat="server" CausesValidation="false" CssClass="text" />
					</div>
					<asp:ListBox ID="lbAvailable" SelectionMode="Multiple" runat="server" Width="210px" Rows="8"></asp:ListBox>
				</ContentTemplate>
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="btnAdd" />
				</Triggers>
			</asp:UpdatePanel>
		</td>
		<td>
			<br /><br />
			<asp:Button ID="btnAdd" runat="server" Text = ">" Width="30px" CausesValidation="false" />
			<br />
			<br />
			<asp:Button ID="btnDelete" runat="server" Text="<" Width="30px" CausesValidation="false" />
		</td>
		<td style="padding:5px;" valign="top">
			<asp:UpdatePanel ID="upSelected" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:ListBox ID="lbSelected" runat="server" Width="210px" Rows="12" SelectionMode="Multiple"></asp:ListBox>
				</ContentTemplate>
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="btnDelete" />
				</Triggers>
			</asp:UpdatePanel>
		</td>
	</tr>
	<tr valign="middle">
		<td colspan="4" align="center" style="padding:2px;">
			<table cellpadding="0" cellspacing="0">
				<tr>
					<td>
						<ibn:IMButton id="btnSave" runat="server" Text="Save" style="width:110px;"></ibn:IMButton>&nbsp;
						<ibn:IMButton id="btnCancel" runat="server" Text="Cancel" style="width:110px"></ibn:IMButton>
					</td>
				</tr>
				<tr>
					<td style="padding-top:5px;" align="left"><asp:CheckBox ID="cbUpdateParent" runat="server" /></td>
				</tr>
			</table>
		</td>
	</tr>
</table>