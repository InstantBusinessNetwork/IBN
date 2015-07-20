<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportImport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ReportManagement.Modules.ReportImport" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<table width="100%" class="ibn-propertysheet">
	<tr>
		<td>
			<table class="ibn-propertysheet">
				<tr>
					<td style="padding:5px; width:90px;">
						<b><%=GetGlobalResourceObject("IbnFramework.Report", "Title").ToString() %>:</b>
					</td>
					<td style="padding:5px;">
						<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="350px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfVal" runat="server" CssClass="text" Display="Dynamic" ControlToValidate="txtTitle" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td style="padding:5px; width:90px;">
						<b><%=GetGlobalResourceObject("IbnFramework.Report", "RdlFile").ToString()%>:</b>
					</td>
					<td style="padding:5px;">
						<mc:McHtmlInputFile style="width:350px" Size="55" id="fSourceFile" class="text" runat="server" />
					</td>
				</tr>
				<tr>
					<td style="padding:5px; width:90px;">
						<b><%=GetGlobalResourceObject("IbnFramework.Report", "FilterControl").ToString()%>:</b>
					</td>
					<td style="padding:5px;">
						<asp:DropDownList ID="ddFilters" runat="server" Width="350px" CssClass="text"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td style="padding:5px; width:90px;">
						<b><%=GetGlobalResourceObject("IbnFramework.Report", "Description").ToString()%>:</b>
					</td>
					<td style="padding:5px;">
						<asp:TextBox ID="txtDescription" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Rows="4"></asp:TextBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr><td style="padding:90px;">&nbsp;</td></tr>
	<tr>
		<td style="padding:5px;height:40px;" align="center">
			<mc2:IMButton id="btnSave" runat="server" style="width:115px;"></mc2:IMButton>&nbsp;
			<mc2:IMButton id="btnCancel" runat="server" style="width:115px;"></mc2:IMButton>
		</td>
	</tr>
</table>