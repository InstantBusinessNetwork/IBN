<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceUtilGraph.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.ResourceUtilGraph" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ Reference Control="~/Projects/Modules/ResourceUtilGraphControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/PageViewMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="GraphControl" Src="~/Projects/Modules/ResourceUtilGraphControl.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox text" style="margin-top: 0px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td>
			<table class="ibn-navline ibn-alternating" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td valign="top" style="width:300px;">
						<table class="text">
							<tr style="height:30px;" id="trGroup" runat="server">
								<td class="text" style="width:100px;"><%=LocRM.GetString("tGroup")%>:&nbsp;</td>
								<td style="width:200px;">
									<lst:indenteddropdownlist id="GroupsList" AutoPostBack="True" runat="server" Width="190px" OnSelectedIndexChanged="GroupsList_SelectedIndexChanged"></lst:indenteddropdownlist>
								</td>
							</tr>
							<tr style="height:30px" id="trUser" runat="server">
								<td class="text"><%=LocRM.GetString("tUser")%>:</td>
								<td>
									<asp:DropDownList id="UsersList" runat="server" Width="190"></asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
					<td valign="top" style="width:150px;">
						<fieldset >
							<legend class="text"><%=LocRM.GetString("tObjects")%></legend>
							<table class="text" cellspacing="0">
								<tr>
									<td><asp:CheckBox ID="CalEntriesCheckbox" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="IssuesCheckBox" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="TasksCheckBox" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="ToDoCheckBox" Runat="server"></asp:CheckBox></td>
								</tr>
								<tr>
									<td><asp:CheckBox ID="DocumentsCheckBox" Runat="server"></asp:CheckBox></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td valign="bottom" align="right">
						<asp:Button ID="ApplyButton" Runat="server" CssClass="text" Width="80px" OnClick="ApplyButton_Click"></asp:Button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<ibn:GraphControl runat="server" id="GraphControlMain"></ibn:GraphControl>
		</td>
	</tr>
</table>
<asp:Button ID="ViewButton" runat="server" Visible="false" OnCommand="ViewButton_Command"/>