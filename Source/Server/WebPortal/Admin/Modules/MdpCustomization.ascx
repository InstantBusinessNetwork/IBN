<%@ Reference Control="~/Modules/ControlPlace/ControlPlace.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" TagName="ControlPlace" Src="~/Modules/ControlPlace/ControlPlace.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.MdpCustomization" CodeBehind="MdpCustomization.ascx.cs" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" width="50%">
			<table class="ibn-propertysheet ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0" id="tblContent" runat="server">
				<tr id="trElemBlockHeader" runat="server">
					<td>
						<ibn:BlockHeader ID="elementHeader" runat="server" />
					</td>
				</tr>
				<tr id="trDropDowns" runat="server">
					<td>
						<table class="ibn-alternating ibn-navline text" cellspacing="0" cellpadding="3" width="100%" border="0">
							<tr>
								<td colspan="3">
									<span style="width: 20px">
										<img alt='' width="16" height="16" border="0" src='<%= ResolveClientUrl("~/Layouts/Images/quicktip.gif") %>'/></span><%=LocRM.GetString("tBlockHelpSelectedFields")%>
								</td>
							</tr>
							<tr height="30">
								<td width="10">
								</td>
								<td class="ibn-label" style="white-space:nowrap">
									<%=LocRM.GetString("SelectElement") %>:
								</td>
								<td width="100%">
									<asp:DropDownList ID="ddlSelectedElement" runat="server" AutoPostBack="True" Width="200" OnSelectedIndexChanged="ddlSelectedElement_SelectedIndexChanged"></asp:DropDownList>
								</td>
							</tr>
							<tr height="30">
								<td>
								</td>
								<td style="white-space:nowrap">
									<asp:Label ID="lblSelectedType" runat="server" CssClass="ibn-label"></asp:Label>
								</td>
								<td>
									<asp:DropDownList ID="ddlSelectedType" runat="server" AutoPostBack="True" Width="200"
										OnSelectedIndexChanged="ddlSelectedType_SelectedIndexChanged">
									</asp:DropDownList>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<table cellpadding="3" cellspacing="0" class="text" width="100%" id="tblControlPlaces" runat="server">
							<tr>
								<td colspan="2" id="tdCPTop" runat="server">
									<mc:ControlPlace ID="CntrlPlTop" runat="server"></mc:ControlPlace>
								</td>
							</tr>
							<tr>
								<td width="50%" valign="top" id="tdCPLeft" runat="server">
									<mc:ControlPlace ID="CntrlPlLeft" runat="server"></mc:ControlPlace>
								</td>
								<td width="50%" valign="top" id="tdCPRight" runat="server">
									<mc:ControlPlace ID="CntrlPlRight" runat="server"></mc:ControlPlace>
								</td>
							</tr>
							<tr>
								<td colspan="2" align="center" id="tdCPBottom" runat="server">
									<mc:ControlPlace ID="CntrlPlBottom" runat="server"></mc:ControlPlace>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
