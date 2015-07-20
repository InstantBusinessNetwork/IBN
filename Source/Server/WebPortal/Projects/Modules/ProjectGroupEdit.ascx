<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGroupEdit" Codebehind="ProjectGroupEdit.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
				<tr>
					<td width="90"><b><%=LocRM.GetString("title")%>:</b></td>
					<td width="350">
						<asp:textbox id="txtTitle" runat="server" CssClass="text" Width="300px"></asp:textbox>
						<asp:requiredfieldvalidator id="rfTitle" runat="server" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtTitle"></asp:requiredfieldvalidator>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td valign=top><b><%=LocRM.GetString("description")%>:</b></td>
					<td valign="top">
						<asp:TextBox ID="txtDescr" CssClass="text" Runat=server TextMode=MultiLine Height="150px" Width="300px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td vAlign="top"></td>
					<td vAlign="top" align=right>
						<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" CausesValidation="false" onserverclick="btnCancel_ServerClick"></btn:imbutton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>